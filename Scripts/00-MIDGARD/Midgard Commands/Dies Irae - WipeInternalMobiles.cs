/***************************************************************************
 *                                  WipeInternalMobiles.cs
 *                            		----------------------
 *  begin                	: Aprile, 2008
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Midgard.Engines.CommercialSystem;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Midgard.Engines.ConsoleCommands;
using System.Collections;
using Server.Items;
using Server.Multis;
using Server.Engines.XmlSpawner2;
using Server.Regions;
using Midgard.Engines.HardLabour;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.MurderInfo;
using Midgard.Engines.WorldForging;
using Midgard.Misc;
using Server.Ethics;

namespace Midgard.Commands
{
    public class WipeInternalMobiles
    {
        public static void Initialize()
        {
            CommandSystem.Register( "WipeInternalMobiles", AccessLevel.Developer, new CommandEventHandler( WipeInternalMobiles_OnCommand ) );
            CommandSystem.Register( "WipeInternalItems", AccessLevel.Developer, new CommandEventHandler( WipeInternalItems_OnCommand ) );
            CommandSystem.Register( "ClearDisplayCache", AccessLevel.Developer, new CommandEventHandler( ClearDisplayCache_OnCommand ) );
            CommandSystem.Register( "DisplayCacheStats", AccessLevel.Developer, new CommandEventHandler( DisplayCacheStats_OnCommand ) );
        }

        [Usage( "ClearDisplayCache" )]
        [Description( "Clear chace for vendor system." )]
        public static void ClearDisplayCache_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "BEFORE CLEAR DisplayChace have {0} items and {1} mobiles.", GenericBuyInfo.GetCacheItemsCount(), GenericBuyInfo.GetCacheMobilesCount() );
            GenericBuyInfo.ClearCache();
            e.Mobile.SendMessage( "AFTER CLEAR DisplayChace have {0} items and {1} mobiles.", GenericBuyInfo.GetCacheItemsCount(), GenericBuyInfo.GetCacheMobilesCount() );
        }

        [Usage( "DisplayCacheStats" )]
        [Description( "Display chace stats for vendor system." )]
        public static void DisplayCacheStats_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "DisplayChace have {0} items and {1} mobiles.", GenericBuyInfo.GetCacheItemsCount(), GenericBuyInfo.GetCacheMobilesCount() );
        }

        [Usage( "WipeInternalMobiles <force>" )]
        [Description( "Wipe all irregular internal mobiles. If force is specified, they are wiped." )]
        public static void WipeInternalMobiles_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            List<Mobile> list = new List<Mobile>( World.Mobiles.Values );
            List<Mobile> invalid = new List<Mobile>();

            bool onlyList = true;

            if( e.Length == 1 )
            {
                if( e.Arguments[ 0 ] == "force" )
                    onlyList = false;
                else
                {
                    from.SendMessage( "Command Use: [WipeInternalMobiles <force>" );
                    return;
                }
            }

            try
            {
                foreach( Mobile mobile in list )
                {
                    // mobile non su mappa internal sono ignorati
                    if( mobile.Map != Map.Internal || mobile.Account != null )
                        continue;

                    // cavalcature con sopra un player sono ignorate
                    if( mobile is IMount && ( (IMount)mobile ).Rider != null )
                        continue;

                    // casi eccezionali vengono trattati a parte
                    if( IsSpecialType( mobile ) )
                        continue;

                    if( mobile is BaseCreature )
                    {
                        BaseCreature bc = (BaseCreature)mobile;

                        // craeture stablate, hitchate o shrinkate sono ignorate
                        if( bc.IsStabled || bc.IsHitched || bc.IsShrinked )
                            continue;

                        // creature controllate o con controlMaster non nulla vengono ignorate
                        if( bc.Controlled || bc.ControlMaster != null )
                            continue;
                    }

                    invalid.Add( mobile );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex );
                from.SendGump( new NoticeGump( 1060635, 30720, ex, 0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), null ) );
            }

            if( invalid == null || invalid.Count < 1 )
            {
                from.SendMessage( "No valid mobile found." );
                return;
            }

            invalid.Sort( InternalComparerMobile.Instance );

            #region statistiche percentuali sui tipi
            Hashtable table = new Hashtable();

            foreach( Mobile mo in invalid )
            {
                Type type = mo.GetType();

                object o = (object)table[ type ];

                if( o == null )
                    table[ type ] = 1;
                else
                    table[ type ] = 1 + (int)o;
            }

            ArrayList mobiles = new ArrayList( table );
            mobiles.Sort( new CountSorter() );
            #endregion

            #region report
            using( TextWriter tw = File.AppendText( "Logs/WipeInternalMobiles.log" ) )
            {
                tw.WriteLine( "############################################" );
                tw.WriteLine( "" );
                tw.WriteLine( "WipeInternalMobiles generated on {0}", DateTime.Now );
                tw.WriteLine( "" );
                tw.WriteLine( "############################################" );
                tw.WriteLine( "" );
                tw.WriteLine( "" );

                for( int i = 0; i < invalid.Count; i++ )
                {
                    Mobile m = invalid[ i ];

                    tw.WriteLine( "Type {0} - Name {1} - Hue {2} - Location {3} - Serial {4}",
                        m.GetType().Name,
                        m.Name != null ? m.Name : "null",
                        m.Hue,
                        m.Location,
                        m.Serial
                        );

                    if( !onlyList && !m.Deleted )
                        m.Delete();
                }

                tw.WriteLine( "" );
                tw.WriteLine( "" );

                tw.WriteLine( "Total mobiles wiped: {0}", invalid.Count );

                tw.WriteLine( "" );
                tw.WriteLine( "" );

                foreach( DictionaryEntry de in mobiles )
                {
                    tw.WriteLine( "{0}\t{1:F2}%\t{2}", de.Value, ( 100 * (int)de.Value ) / (double)World.Mobiles.Count, de.Key );
                }
            }
            #endregion

            from.SendMessage( "Task completed. See <runuo>/WipeInternalMobiles.log for infoes." );
        }

        [Usage( "WipeInternalItems <force>" )]
        [Description( "Wipe all irregular internal items. If force is specified, they are wiped." )]
        public static void WipeInternalItems_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            List<Item> list = new List<Item>( World.Items.Values );
            List<Item> invalid = new List<Item>();
            ArrayList ignored = new ArrayList();

            bool onlyList = true;

            if( e.Length == 1 )
            {
                if( e.Arguments[ 0 ] == "force" )
                    onlyList = false;
                else
                {
                    from.SendMessage( "Command Use: [WipeInternalItems <force>" );
                    return;
                }
            }

            try
            {
                foreach( Item i in list )
                {
                    ManageInternals( i, ref ignored );

                    if( i.Map != Map.Internal )
                        continue;

                    if( i.Parent != null )
                        continue;

                    if( i.HeldBy != null )
                        continue;

                    if( !( i is Backgammon ) && !( i is CheckerBoard ) && !( i is Chessboard ) )
                    {
                        if( i is Container && ( (Container)i ).Items.Count > 0 )
                            continue;
                    }

                    if( i is CommodityDeed && ( (CommodityDeed)i ).Commodity != null )
                        continue;

                    if( i is BankCheck && ((BankCheck)i).Worth > 0 )
                        continue;

                    if( IsSpecialType( i ) )
                        continue;

                    invalid.Add( i );
                }

                foreach( object o in ignored )
                {
                    if( o is Item )
                    {
                        Item i = (Item)o;
                        if( invalid.Contains( i ) )
                        {
                            invalid.Remove( i );
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex );
                from.SendGump( new NoticeGump( 1060635, 30720, ex, 0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), null ) );
            }

            if( invalid == null || invalid.Count < 1 )
            {
                from.SendMessage( "No valid item found." );
                return;
            }

            invalid.Sort( InternalComparerItem.Instance );

            #region statistiche percentuali sui tipi
            Hashtable table = new Hashtable();

            foreach( Item i in invalid )
            {
                Type type = i.GetType();

                object o = (object)table[ type ];

                if( o == null )
                    table[ type ] = 1;
                else
                    table[ type ] = 1 + (int)o;
            }

            ArrayList items = new ArrayList( table );
            items.Sort( new CountSorter() );
            #endregion

            #region report
            using( TextWriter tw = File.AppendText( "Logs/WipeInternalItems.log" ) )
            {
                tw.WriteLine( "############################################" );
                tw.WriteLine( "" );
                tw.WriteLine( "WipeInternalItems generated on {0}", DateTime.Now );
                tw.WriteLine( "" );
                tw.WriteLine( "############################################" );
                tw.WriteLine( "" );
                tw.WriteLine( "" );

                for( int i = 0; i < invalid.Count; i++ )
                {
                    Item item = invalid[ i ];

                    tw.WriteLine( "Type {0} - Name {1} - Hue {2} - Location {3} - Serial {4}",
                        item.GetType().Name,
                        item.Name != null ? item.Name : "null",
                        item.Hue,
                        item.Location,
                        item.Serial
                        );

                    if( !onlyList && !item.Deleted )
                        item.Delete();
                }

                tw.WriteLine( "" );
                tw.WriteLine( "" );

                tw.WriteLine( "Total items wiped: {0}", invalid.Count );

                tw.WriteLine( "" );
                tw.WriteLine( "" );

                foreach( DictionaryEntry de in items )
                {
                    tw.WriteLine( "{0}\t{1:F2}%\t{2}", de.Value, ( 100 * (int)de.Value ) / (double)World.Items.Count, de.Key );
                }
            }
            #endregion

            from.SendMessage( "Task completed. See <runuo>/WipeInternalItems.log for infoes." );
        }

        private static bool IsSpecialType( object o )
        {
            return o is CCPersistance || o is Fists || o is MountItem || o is BankCheck
                || o is EffectItem || o is MovingCrate || o is SpawnPersistence
                || o is Midgard2Persistance
                || o is XmlSaveItem || o is HardLabourPersistance || o is TownSystemPersistance 
                || o is MurderInfoPersistance || o is WorldForgingPersistance || o is StealableArtifactsSpawner || o is EthicsPersistance;
        }

        private static void ManageInternals( Item i, ref ArrayList ignoreList )
        {
            // ignore valid internalized commodity deed items
            if( i is CommodityDeed )
            {
                CommodityDeed deed = (CommodityDeed)i;

                if( deed.Commodity != null && deed.Commodity.Map == Map.Internal )
                    ignoreList.Add( deed.Commodity );
            }

            // ignore valid internalized keyring keys
            if( i is KeyRing )
            {
                KeyRing keyring = (KeyRing)i;

                if( keyring.Keys != null )
                {
                    foreach( Key k in keyring.Keys )
                    {
                        ignoreList.Add( k );
                    }
                }
            }

            // ignore valid internalized relocatable house items
            if( i is BaseHouse )
            {
                BaseHouse house = (BaseHouse)i;

                foreach( RelocatedEntity relEntity in house.RelocatedEntities )
                {
                    if( relEntity.Entity is Item )
                        ignoreList.Add( relEntity.Entity );
                }

                foreach( VendorInventory inventory in house.VendorInventories )
                {
                    foreach( Item subItem in inventory.Items )
                        ignoreList.Add( subItem );
                }
            }
        }

        private static void CloseNoticeCallback( Mobile From, object state )
        {
        }

        private class InternalComparerMobile : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new InternalComparerMobile();

            public InternalComparerMobile()
            {
            }

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                string typeX = x.GetType().Name;
                string typeY = y.GetType().Name;

                if( typeX != typeY )
                    return Insensitive.Compare( typeX, typeY );
                else if( x.Name != null && y.Name != null )
                    return Insensitive.Compare( x.Name, y.Name );
                else
                    return 0;
            }
        }

        private class InternalComparerItem : IComparer<Item>
        {
            public static readonly IComparer<Item> Instance = new InternalComparerItem();

            public InternalComparerItem()
            {
            }

            public int Compare( Item x, Item y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                string typeX = x.GetType().Name;
                string typeY = y.GetType().Name;

                if( typeX != typeY )
                    return Insensitive.Compare( typeX, typeY );
                else if( x.Name != null && y.Name != null )
                    return Insensitive.Compare( x.Name, y.Name );
                else
                    return 0;
            }
        }

        private class CountSorter : IComparer
        {
            public int Compare( object x, object y )
            {
                DictionaryEntry a = (DictionaryEntry)x;
                DictionaryEntry b = (DictionaryEntry)y;

                int aCount = (int)a.Value;
                int bCount = (int)b.Value;

                int v = -aCount.CompareTo( bCount );

                if( v == 0 )
                {
                    Type aType = (Type)a.Key;
                    Type bType = (Type)b.Key;

                    v = aType.FullName.CompareTo( bType.FullName );
                }

                return v;
            }
        }
    }
}