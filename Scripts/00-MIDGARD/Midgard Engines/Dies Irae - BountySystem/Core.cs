/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.BountySystem
{
    public static class Core
    {
        static Core()
        {
            Entries = new List<BountyBoardEntry>();
        }

        internal static void RegisterSinks()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( Load );
            EventSink.WorldSave += new WorldSaveEventHandler( Save );
        }

        private static void Save( WorldSaveEventArgs e )
        {
            if( !File.Exists( Config.XmlSchema ) )
            {
                Config.Pkg.LogInfoLine( "Could not open {0}.", Config.XmlSchema );
                Config.Pkg.LogInfoLine( "{0} must be in {1}", Config.XmlSchema, Config.XmlRoot );
                Config.Pkg.LogInfoLine( "Cannot save bounties." );
                return;
            }

            WorldSaveProfiler.Instance.StartHandlerProfile( Save );
            
            if( File.Exists( Config.XmlFileBackup ) )
                File.Delete( Config.XmlFileBackup );

            if( File.Exists( Config.XmlFile ) )
                File.Move( Config.XmlFile, Config.XmlFileBackup );

            DataSet ds = new DataSet();
            ds.ReadXmlSchema( Config.XmlSchema );
            DataRow bountyRow, ownerRow, wantedRow, requestedRow, acceptedRow;
            foreach( BountyBoardEntry entry in Entries )
            {
                if( entry == null )
                    continue;

                if( entry.Owner == null || entry.Owner.Deleted )
                    continue;

                if( entry.Wanted == null || entry.Wanted.Deleted )
                {
                    RemoveEntry( entry, true );
                    continue;
                }

                bountyRow = ds.Tables[ "Bounty" ].NewRow();
                bountyRow[ "Price" ] = entry.Price;
                bountyRow[ "ExpireTime" ] = entry.ExpireTime;
                ds.Tables[ "Bounty" ].Rows.Add( bountyRow );

                ownerRow = ds.Tables[ "Owner" ].NewRow();
                ownerRow[ "Name" ] = entry.Owner.Name;
                ownerRow[ "Serial" ] = entry.Owner.Serial.Value;
                ownerRow.SetParentRow( bountyRow, ds.Relations[ "Bounty_Owner" ] );
                ds.Tables[ "Owner" ].Rows.Add( ownerRow );

                wantedRow = ds.Tables[ "Wanted" ].NewRow();
                wantedRow[ "Name" ] = entry.Wanted.Name;
                wantedRow[ "Serial" ] = entry.Wanted.Serial.Value;
                wantedRow.SetParentRow( bountyRow, ds.Relations[ "Bounty_Wanted" ] );
                ds.Tables[ "Wanted" ].Rows.Add( wantedRow );

                foreach( Mobile requested in entry.Requested )
                {
                    if( requested == null || requested.Deleted )
                        continue;

                    requestedRow = ds.Tables[ "Requested" ].NewRow();
                    requestedRow[ "Name" ] = requested.Name;
                    requestedRow[ "Serial" ] = requested.Serial.Value;
                    requestedRow.SetParentRow( bountyRow, ds.Relations[ "Bounty_Requested" ] );
                    ds.Tables[ "Requested" ].Rows.Add( requestedRow );
                }

                foreach( Mobile accepted in entry.Accepted )
                {
                    if( accepted == null || accepted.Deleted )
                        continue;

                    acceptedRow = ds.Tables[ "Accepted" ].NewRow();
                    acceptedRow[ "Name" ] = accepted.Name;
                    acceptedRow[ "Serial" ] = accepted.Serial.Value;
                    acceptedRow.SetParentRow( bountyRow, ds.Relations[ "Bounty_Accepted" ] );
                    ds.Tables[ "Accepted" ].Rows.Add( acceptedRow );
                }

                ds.WriteXml( Config.XmlFile );
            }

            WorldSaveProfiler.Instance.EndHandlerProfile();
        }

        internal static void Load()
        {
            if( !File.Exists( Config.XmlSchema ) )
            {
                Config.Pkg.LogInfoLine( "Could not open {0}.", Config.XmlSchema );
                Config.Pkg.LogInfoLine( "{0} must be in {1}", Config.XmlSchema, Config.XmlRoot );
                Config.Pkg.LogInfoLine( "Cannot save bounties." );
                return;
            }

            if( !File.Exists( Config.XmlFile ) )
            {
                Config.Pkg.LogInfoLine( "Could not open {0}.", Config.XmlFile );
                Config.Pkg.LogInfoLine( "{0} must be in {1}", Config.XmlFile, Config.XmlRoot );
                Config.Pkg.LogInfoLine( "This is okay if this is the first run after installation of the Bounty system." );
                return;
            }

            DataSet ds = new DataSet();
            try
            {
                ds.ReadXmlSchema( Config.XmlSchema );
                ds.ReadXml( Config.XmlFile );
            }
            catch
            {
                Config.Pkg.LogInfoLine( "Error reading {0}.  File may be corrupt.", Config.XmlFile );
                return;
            }

            Mobile owner = null;
            Mobile wanted = null;
            Mobile requested = null;
            Mobile accepted = null;
            int price;
            DateTime expireTime;
            BountyBoardEntry entry;

            foreach( DataRow bountyRow in ds.Tables[ "Bounty" ].Rows )
            {
                foreach( DataRow childRow in bountyRow.GetChildRows( "Bounty_Owner" ) )
                {
                    owner = World.FindMobile( (int)childRow[ "Serial" ] );
                }

                if( owner == null || owner.Deleted || !( owner is Midgard2PlayerMobile ) )
                    continue;

                foreach( DataRow childRow in bountyRow.GetChildRows( "Bounty_Wanted" ) )
                {
                    wanted = World.FindMobile( (int)childRow[ "Serial" ] );
                }

                price = (int)bountyRow[ "Price" ];
                expireTime = (DateTime)bountyRow[ "ExpireTime" ];

                entry = new BountyBoardEntry( owner, wanted, price, expireTime );

                foreach( DataRow childRow in bountyRow.GetChildRows( "Bounty_requested" ) )
                {
                    requested = World.FindMobile( (int)childRow[ "Serial" ] );
                    if( requested != null && !requested.Deleted && requested is Midgard2PlayerMobile )
                        entry.Requested.Add( requested );
                }

                foreach( DataRow childRow in bountyRow.GetChildRows( "Bounty_accepted" ) )
                {
                    accepted = World.FindMobile( (int)childRow[ "Serial" ] );
                    if( accepted != null && !accepted.Deleted && accepted is Midgard2PlayerMobile )
                        entry.Accepted.Add( accepted );
                }

                if( !entry.Expired )
                    AddEntry( entry );
                else
                {
                    NotifyBountyEnd( entry );
                    if( !Banker.Deposit( entry.Owner, price ) )
                        entry.Owner.AddToBackpack( new Gold( price ) );
                }

                if( wanted == null || wanted.Deleted || !( wanted is Midgard2PlayerMobile ) )
                    RemoveEntry( entry, true );
            }
        }

        internal static List<BountyBoardEntry> Entries { get; private set; }

        internal static BountyBoardEntry AddEntry( Mobile owner, Mobile wanted, int price, DateTime expireTime )
        {
            foreach( BountyBoardEntry entry in Entries )
            {
                if( entry.Owner == owner && entry.Wanted == wanted )
                {
                    entry.Price += price;
                    entry.ExpireTime = expireTime;
                    return entry;
                }
            }

            BountyBoardEntry be = new BountyBoardEntry( owner, wanted, price, expireTime );

            Entries.Add( be );

            BountySystemLog.WriteInfo( be, BountySystemLog.LogType.Added );

            List<BountyBoard> instances = BountyBoard.Instances;

            for( int i = 0; i < instances.Count; ++i )
                instances[ i ].InvalidateProperties();

            return be;
        }

        internal static void AddEntry( BountyBoardEntry be )
        {
            Entries.Add( be );

            BountySystemLog.WriteInfo( be, BountySystemLog.LogType.Added );

            List<BountyBoard> instances = BountyBoard.Instances;

            for( int i = 0; i < instances.Count; ++i )
                instances[ i ].InvalidateProperties();
        }

        internal static void RemoveEntry( BountyBoardEntry be, bool refund )
        {
            Entries.Remove( be );

            BountySystemLog.WriteInfo( be, BountySystemLog.LogType.Removed );

            List<BountyBoard> instances = BountyBoard.Instances;

            for( int i = 0; i < instances.Count; ++i )
                instances[ i ].InvalidateProperties();

            if( refund && be.Owner != null )
            {
                string msg = String.Format( "Your bounty in the amount of {0} on {1}'s head has ended.", be.Price, be.Wanted.Name );

                if( NetState.Instances.Contains( be.Owner.NetState ) )
                {
                    be.Owner.SendMessage( msg );
                }
                else
                {
                    ( (Midgard2PlayerMobile)be.Owner ).ShowBountyUpdate = true;
                    ( (Midgard2PlayerMobile)be.Owner ).BountyUpdateList.Add( msg );
                }

                if( Banker.Deposit( be.Owner, be.Price ) )
                    be.Owner.SendLocalizedMessage( 1060397, be.Price.ToString() ); // ~1_AMOUNT~ gold has been deposited into your bank box.
                else
                {
                    be.Owner.AddToBackpack( new Gold( be.Price ) );
                    be.Owner.SendMessage( "The bounty of {0} has been added to your backpack.", be.Price );
                }
            }
            else if( be.Owner != null )
            {
                string msg = String.Format( "Your bounty in the amount of {0} on {1}'s head has been claimed.", be.Price, be.Wanted.Name );

                if( NetState.Instances.Contains( be.Owner.NetState ) )
                {
                    be.Owner.SendMessage( msg );
                }
                else
                {
                    ( (Midgard2PlayerMobile)be.Owner ).ShowBountyUpdate = true;
                    ( (Midgard2PlayerMobile)be.Owner ).BountyUpdateList.Add( msg );
                }
            }

            NotifyBountyEnd( be );
        }

        internal static void NotifyBountyEnd( BountyBoardEntry be )
        {
            foreach( Midgard2PlayerMobile player in be.Accepted )
            {
                string msg = String.Format( "The bounty hunt on {0}'s head is over.", be.Wanted.Name );

                if( NetState.Instances.Contains( player.NetState ) )
                {
                    player.SendMessage( msg );
                }
                else
                {
                    player.ShowBountyUpdate = true;
                    player.BountyUpdateList.Add( msg );
                }
            }
        }

        internal static bool HasBounty( Mobile claimer, Mobile killer, out BountyBoardEntry bountyEntry, out bool canClaim )
        {
            bountyEntry = null;
            canClaim = false;
            DateTime expireTime = DateTime.MaxValue;
            bool hasBounty = false;

            foreach( BountyBoardEntry entry in Entries )
            {
                if( entry.Wanted == killer )
                {
                    hasBounty = true;
                    canClaim = entry.Accepted.Contains( claimer );

                    if( /*canClaim &&*/ entry.ExpireTime < expireTime ) // crash bugfix
                    {
                        bountyEntry = entry;
                        expireTime = entry.ExpireTime;
                    }
                }
            }
            return hasBounty;
        }

        internal static bool Attackable( Mobile attacker, Mobile attackee )
        {
            foreach( BountyBoardEntry entry in Entries )
            {
                if( entry.Wanted == attackee && entry.Accepted.Contains( attacker ) )
                    return true;
            }

            return false;
        }
    }
}