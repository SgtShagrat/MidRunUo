/***************************************************************************
 *                               Dies Irae - MidgardCleanup.cs
 *
 *   begin                : 21 settembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Midgard.Engines.MidgardTownSystem;
using Midgard.Items;

using Server;
using Server.Commands;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Misc
{
    public class MidgardCleanup
    {
        public static void Initialize()
        {
            CommandSystem.Register( "ForceCleanup", AccessLevel.Administrator, new CommandEventHandler( Cleanup_OnCommand ) );

            if( World.Loading )
            {
                Console.WriteLine( "Midgard Cleanup skipped. Will be executed at next server restart." );
                return;
            }

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( RunZero ) );
            Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( RunFive ) );
        }

        [Usage( "ForceCleanup" )]
        [Description( "Force server cleanup as done by midgard cleanup engine." )]
        private static void Cleanup_OnCommand( CommandEventArgs e )
        {
            Server.Misc.Cleanup.Run();
            RunZero();
            RunFive();
        }

        private static readonly List<BaseCreature> m_Creatures = new List<BaseCreature>();
        private static readonly List<Item> m_Items = new List<Item>();
        private static readonly List<BaseVendor> m_Vendors = new List<BaseVendor>();
        private static readonly List<Midgard2PlayerMobile> m_Players = new List<Midgard2PlayerMobile>();
        private static readonly List<Mobile> m_Mobiles = new List<Mobile>();

        public delegate void CleanupCallback();

        private static void ExecuteCallbackAndProfile( CleanupCallback callback )
        {
            Stopwatch sw = Stopwatch.StartNew();

            if( callback != null )
                callback();

            sw.Stop();

            ScriptCompiler.EnsureDirectory( "Profiles" );

            try
            {
                using( StreamWriter op = new StreamWriter( Path.Combine( "Logs", "cleanup-profile.log" ), true ) )
                    op.WriteLine( string.Format( "{0} - {1} ms.", FormatDelegate( callback ), sw.ElapsedMilliseconds ) );
            }
            catch
            {
            }
        }

        private static string FormatDelegate( Delegate callback )
        {
            return callback == null ? "null" : String.Format( "{0}.{1}", callback.Method.DeclaringType.FullName, callback.Method.Name );
        }

        private static void RunZero()
        {
            ExecuteCallbackAndProfile( FixCreatureLayerCompfictions );

            ExecuteCallbackAndProfile( FixGuildItems );

            ExecuteCallbackAndProfile( FixCreaturesOriginalNames );

            ExecuteCallbackAndProfile( FixHorses );

            ExecuteCallbackAndProfile( FixVendorsMorph );

            ExecuteCallbackAndProfile( FixShrinkItems );

            ExecuteCallbackAndProfile( FixPlayers );

            ExecuteCallbackAndProfile( InitTownSystems );

            ExecuteCallbackAndProfile( VerifyTownCommercialSystem );

            ExecuteCallbackAndProfile( FixFillableContainers );

            ExecuteCallbackAndProfile( FixNullableMobiles );

            FixJewels();
        }

        private static void FixNullableMobiles()
        {
            PopulateMobiles();

            int count = 0;

            for( int index = 0; index < m_Mobiles.Count; index++ )
            {
                Mobile mobile = m_Mobiles[ index ];
                if( IsPreAOSBuggable( mobile ) )
                {
                    if( !mobile.Deleted )
                    {
                        Utility.Log( "nullable-mobiles.log", "serial 0x{0:X} creation {1} name {2}", mobile.Serial.ToString(), mobile.CreationTime.ToString(), mobile.Name ?? "" );
                        mobile.Delete();
                        count++;
                    }
                }
            }

            Console.WriteLine( "Midgard Cleanup: Detected {0} invalid mobiles, removing..", count );
        }

        private static void FixShrinkItems()
        {
            PopulateItems();

            foreach( Item item in m_Items )
            {
                if( item is ShrinkItem )
                    ShrinkItem.VerifyMob_Callback( item );
            }
        }

        private static void FixJewels()
        {
            PopulateItems();

            int count = 0;
            foreach( Item item in m_Items )
            {
                if( item is BaseJewel )
                {
                    BaseJewel jewel = item as BaseJewel;
                    if( jewel.MagicalAttribute != JewelMagicalAttribute.None )
                    {
                        int min, max;
                        Engines.SecondAgeLoot.Magics.GetJewelCharges( jewel.MagicalAttribute, out min, out max );

                        if( jewel.MagicalCharges > max )
                        {
                            Utility.Log( "fix-jewels.log", "serial 0x{0:X} - old charges {1} - new charges {2}", jewel.Serial.ToString(), jewel.MagicalCharges, max );

                            jewel.MagicalCharges = max;

                            if( jewel.MagicalCharges > max )
                                jewel.MagicalCharges = max;

                            count++;
                        }
                    }
                }
            }

            if( count > 0 )
                Console.WriteLine( "Fixed {0} bugged jewels having too much magical charges.", count );
        }

        private static void FixHorses()
        {
            PopulateCreatures();

            foreach( BaseCreature creature in m_Creatures )
            {
                if( creature is Horse )
                    ( (Horse)creature ).UpdateItemid();
            }
        }

        private static void FixFillableContainers()
        {
            PopulateItems();

            foreach( Item item in m_Items )
            {
                if( item is FillableContainer )
                    FillableContainer.Relock_Callback( item );
            }
        }

        private static void RunFive()
        {
            ExecuteCallbackAndProfile( CleanupBuggedItems );
        }

        private static void VerifyTownCommercialSystem()
        {
            foreach( TownSystem system in TownSystem.TownSystems )
                system.CommercialStatus.RegisterVendorStatuses();
        }

        private static void InitTownSystems()
        {
            foreach( TownSystem system in TownSystem.TownSystems )
                system.OnTownSystemInitialized();
        }

        private static void FixPlayers()
        {
            PopulatePlayers();

            foreach( Midgard2PlayerMobile player in m_Players )
                player.VerifyAccountCallback();

            PopulatePlayers();

            foreach( Midgard2PlayerMobile player in m_Players )
                player.PlayerStoneAttributes.InvalidateAll();
        }

        #region populate lists
        private static void PopulateCreatures()
        {
            m_Creatures.Clear();

            lock( World.Mobiles.Values )
            {
                foreach( Mobile mobile in World.Mobiles.Values )
                {
                    if( mobile is BaseCreature )
                        m_Creatures.Add( (BaseCreature)mobile );
                }
            }
        }

        private static void PopulateItems()
        {
            m_Items.Clear();

            lock( World.Items.Values )
            {
                foreach( Item item in World.Items.Values )
                    m_Items.Add( item );
            }
        }

        private static void PopulateVendors()
        {
            m_Vendors.Clear();

            lock( World.Mobiles.Values )
            {
                foreach( Mobile mobile in World.Mobiles.Values )
                {
                    if( mobile is BaseVendor )
                        m_Vendors.Add( (BaseVendor)mobile );
                }
            }
        }

        private static void PopulatePlayers()
        {
            m_Players.Clear();

            lock( World.Mobiles.Values )
            {
                foreach( Mobile mobile in World.Mobiles.Values )
                {
                    if( mobile.Player )
                        m_Players.Add( (Midgard2PlayerMobile)mobile );
                }
            }
        }

        private static void PopulateMobiles()
        {
            m_Mobiles.Clear();

            lock( World.Mobiles.Values )
            {
                foreach( Mobile mobile in World.Mobiles.Values )
                {
                    m_Mobiles.Add( mobile );
                }
            }
        }
        #endregion

        private static void FixVendorsMorph()
        {
            PopulateVendors();

            foreach( BaseVendor vendor in m_Vendors )
                vendor.CheckMorph();
        }

        private static void FixCreaturesOriginalNames()
        {
            PopulateCreatures();

            foreach( BaseCreature creature in m_Creatures )
                creature.UpdateOriginalName();
        }

        private static void FixGuildItems()
        {
            PopulateItems();

            for( int index = 0; index < m_Items.Count; index++ )
            {
                Item item = m_Items[ index ];

                if( item is BaseJewel )
                    ( (BaseJewel)item ).VerifyGuildItem();
                else if( item is BaseArmor )
                    ( (BaseArmor)item ).VerifyGuildItem();
                else if( item is BaseClothing )
                    ( (BaseClothing)item ).VerifyGuildItem();
                else if( item is BaseInstrument )
                    ( (BaseInstrument)item ).VerifyGuildItem();
            }
        }

        private static void FixCreatureLayerCompfictions()
        {
            PopulateCreatures();

            foreach( BaseCreature creature in m_Creatures )
                creature.VerifyLayersConfliction();
        }

        private static void CleanupBuggedItems()
        {
            PopulateItems();

            List<Item> items = new List<Item>();

            foreach( Item item in m_Items )
            {
                if( !IsPreAOSBuggable( item ) )
                    continue;

                items.Add( item );
            }

            if( items.Count <= 0 )
                return;

            Console.WriteLine( "Midgard Cleanup: Detected {0} invalid items, removing..", items.Count );

            using( StreamWriter tw = new StreamWriter( "Logs/midgard-cleanup.log", true ) )
            {
                foreach( Item i in items )
                {
                    if( i == null || i.Deleted )
                        continue;

                    Type t = i.GetType();

                    string name = "";
                    IEntity ie = i.RootParentEntity;
                    if( ie is Mobile )
                        name = ( (Mobile)ie ).Name;
                    else if( ie is Item )
                        name = ( (Item)ie ).Name;

                    tw.WriteLine( "Type: {0} Serial: {1} ItemID: {2} RootParent: {3}{4} Location: {5}",
                                  t.Name, i.Serial, i.ItemID, i.RootParentEntity == null ? "none" : i.RootParentEntity.Serial.ToString(),
                                  i.RootParentEntity == null ? "" : string.Format( " ({0})", name ), i.Location );

                    i.Delete();
                }
            }
        }

        private static bool IsPreAOSBuggable( Mobile m )
        {
            if( m is PlayerMobile && m.Account == null )
                return true;

            if( !( m is PlayerMobile ) && !( m is BaseCreature ) && ( m.Map == null || m.Map == Map.Internal ) )
                return true;

            if( m is BaseVendor && ( m.Map == null || m.Map == Map.Internal ) )
                return true;

            if( m is SummonedDaemon && ( m.Map == null || m.Map == Map.Internal ) )
                return true;

            return false;
        }

        private static bool IsPreAOSBuggable( Item i )
        {
            if( IsPreAOSBuggable( i, Loot.MajorArtifacts ) )
                return true;

            if( IsPreAOSBuggable( i, Loot.MinorArtifacts ) )
                return true;

            if( IsPreAOSBuggable( i, BaseCreature.MLArtifacts ) )
                return true;

            if( i is BaseTalisman )
                return true;

            if( i is CompositeBow || i is RepeatingCrossbow )
                return true;

            if( i is ITreasureOfMidgard && TreasuresOfMidgard.IsMajorArtifact( i.GetType() ) )
            {
                XmlBlessedCursedAttach att = XmlAttach.FindAttachment( i, typeof( XmlBlessedCursedAttach ) ) as XmlBlessedCursedAttach;
                if( att == null )
                    return true;
            }

            return false;
        }

        private static bool IsPreAOSBuggable( Item i, Type[] items )
        {
            return Array.IndexOf( items, i.GetType() ) > -1;
        }
    }
}