/***************************************************************************
 *                                  TownSystemCommands.cs
 *                            		---------------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Midgard.Engines.TownHouses;
using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownSystemCommands
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "GenerateTownFields", AccessLevel.Developer, GenerateTownFields_OnCommand );
            CommandSystem.Register( "ResetCommercials", AccessLevel.Developer, ResetCommercials_OnCommand );
            CommandSystem.Register( "FixTownPlayerStates", AccessLevel.Developer, FixTownPlayerStates_OnCommand );
            CommandSystem.Register( "FixCitizenLevel", AccessLevel.Developer, FixCitizenLevel_OnCommand );
            CommandSystem.Register( "ListPlayerStates", AccessLevel.Developer, ListPlayerStates_OnCommand );
            CommandSystem.Register( "GenerateTownCommercialInfo", AccessLevel.Developer, GenerateTownCommercialInfo_OnCommand );
            CommandSystem.Register( "GenerateTownVendors", AccessLevel.Developer, GenerateTownVendors_OnCommand );
            CommandSystem.Register( "GenerateTownStones", AccessLevel.Developer, new CommandEventHandler( GenerateTownStones_OnCommand ) );

            CommandSystem.Register( "DisableTownSystemAccess", AccessLevel.Administrator, DisableTownSystemAccess_OnCommand );
            CommandSystem.Register( "RefreshWarLordStatus", AccessLevel.Administrator, RefreshWarLordStatus_OnCommand );
            CommandSystem.Register( "ResetTownAccount", AccessLevel.Administrator, ResetTownAccount_OnCommand );
            CommandSystem.Register( "SetTown", AccessLevel.Seer, SetTown_OnCommand );
            CommandSystem.Register( "KickTown", AccessLevel.Administrator, KickTown_OnCommand );
            CommandSystem.Register( "AddTownStatue", AccessLevel.Administrator, AddTownStatue_OnCommand );
            CommandSystem.Register( "SetGlobalTownEnemyPoints", AccessLevel.Administrator, SetGlobalTownEnemyPoints_OnCommand );
            CommandSystem.Register( "AddTownField", AccessLevel.Seer, AddTownField_OnCommand );

            CommandSystem.Register( "PuntiCittadini", AccessLevel.Player, CheckTownKills_OnCommand );
            CommandSystem.Register( "Lavoro", AccessLevel.Player, new CommandEventHandler( ToggleJob_OnCommand ) );

        }

        [Usage( "Lavoro <skill>" )]
        [Description( "Setta il valore della skill come base per il calcolo del titolo." )]
        public static void ToggleJob_OnCommand( CommandEventArgs e )
        {
            Midgard2PlayerMobile m = e.Mobile as Midgard2PlayerMobile;
            if( m == null || !m.CheckAlive() )
                return;

            SkillName index;

            try
            {
                index = (SkillName)Enum.Parse( typeof( SkillName ), e.GetString( 0 ), true );
            }
            catch
            {
                m.SendLocalizedMessage( 1005631 ); // You have specified an invalid skill to set.
                return;
            }

            Skill skill = m.Skills[ index ];
            if( skill != null )
            {
                m.CustomJobTitle = index;
                SkillInfo info = SkillInfo.Table[ (int)index ];
                m.SendMessage( "You have chosen your new title: " + info.Title );
            }
        }

        [Usage( "NascondiTitoloCittadino" )]
        [Description( "Abilita o disabilita lo status cittadino per il giocatore." )]
        public static void ToggleCitizenStatusDisplay_OnCommand( CommandEventArgs e )
        {
            Midgard2PlayerMobile m = e.Mobile as Midgard2PlayerMobile;
            if( m == null )
                return;

            if( !m.CheckAlive() || TownSystem.Find( m ) == null )
            {
                m.SendMessage( "Thou cannot use this command." );
                return;
            }

            // 1064250 You have chosen to hide your citizen status.
            // 1064251 You have chosen to display your citizen status.
            m.SendLocalizedMessage( m.DisplayCitizenStatus ? 1064250 : 1064251 );
            m.SendLocalizedMessage( 1064252 ); // You will be disconnected to update your status.

            m.DisplayCitizenStatus = !m.DisplayCitizenStatus;
        }

        [Usage( "GenerateTownStones" )]
        [Description( "Generate townstones" )]
        public static void GenerateTownStones_OnCommand( CommandEventArgs e )
        {
            TownSystem[] systems = TownSystem.TownSystems;

            foreach( TownSystem system in systems )
                Generate( system.Definition );
        }

        public static void Generate( TownDefinition def )
        {
            if( !CheckExistance( def.TownstoneLocation, Map.Felucca, typeof( MidgardTownStone ) ) )
            {
                MidgardTownStone stone = new MidgardTownStone( def.Town );
                stone.MoveToWorld( def.TownstoneLocation, Map.Felucca );
            }
        }

        private static bool CheckExistance( Point3D loc, Map facet, Type type )
        {
            foreach( Item item in facet.GetItemsInRange( loc, 0 ) )
            {
                if( type.IsAssignableFrom( item.GetType() ) )
                    return true;
            }

            return false;
        }

        [Usage( "DisableTownSystemAccess <true | false>" )]
        [Description( "Enables or disables townsystem access" )]
        public static void DisableTownSystemAccess_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 1 )
            {
                TownSystem.TownAccessEnabled = e.GetBoolean( 0 );
                e.Mobile.SendMessage( "Town system access has been {0}.", TownSystem.TownAccessEnabled ? "enabled" : "disabled" );
            }
            else
            {
                e.Mobile.SendMessage( "Format: DisableTownSystemAccess <true | false>" );
            }
        }

        [Usage( "SetGlobalTownEnemyPoints" )]
        [Description( "Set all enemy points to a given value. Values of -1000 are ignored." )]
        public static void SetGlobalTownEnemyPoints_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 1 )
            {
                int value = e.GetInt32( 0 );

                foreach( TownSystem t in TownSystem.TownSystems )
                {
                    foreach( TownPlayerState tps in t.Players )
                    {
                        if( tps != null )
                        {
                            if( tps.EnemyKills > -1000 ) // valore di default
                            {
                                tps.EnemyKills = value;
                            }
                        }
                    }
                }

                from.SendMessage( "Process completed successfully." );
            }
            else
            {
                from.SendMessage( "Command Use: [SetGlobalTownEnemyPoints <value>" );
            }
        }

        [Usage( "AddTownStatue" )]
        [Description( "Generate a town statue." )]
        public static void AddTownStatue_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            from.Target = new TownCharacterStatueTarget( StatueType.Marble );
        }

        [Usage( "RefreshWarLordStatus" )]
        [Description( "Refresh the status of warlord status" )]
        public static void RefreshWarLordStatus_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            for( int i = 0; i < TownSystem.TownSystems.Length; i++ )
                TownSystem.TownSystems[ i ].RefreshWarLordStatus();

            from.SendMessage( "Warlord refresh process completed." );
        }

        [Usage( "GenerateTownFields" )]
        [Description( "Generate town fields." )]
        public static void GenerateTownFields_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
            {
                foreach( TownSystem t in TownSystem.TownSystems )
                {
                    Console.WriteLine( "Generating fields for {0}", t.Definition.TownName );

                    if( t.FieldDefinitions != null )
                    {
                        foreach( TownFieldDefinition d in t.FieldDefinitions )
                        {
                            if( !FindTownField( Map.Felucca, d.ContractLocation ) )
                            {
                                Console.WriteLine( "TownFieldSign added in location {0} for town {1}.", d.ContractLocation, d.System.Definition.TownName );
                                new TownFieldSign( d );
                            }
                        }
                    }

                    Console.WriteLine( "TownSystem {0} has now {1} fields registered.", t.Definition.TownName, t.SystemFields.Count );
                }
                e.Mobile.SendMessage( "Process done." );
            }
            else
                e.Mobile.SendMessage( "Command Use: [GenerateTownFields" );
        }

        [Usage( "AddTownField" )]
        [Description( "Generate one single townfield." )]
        public static void AddTownField_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            TownFieldSign sign = new TownFieldSign();
            from.AddToBackpack( sign );
            from.SendMessage( "A new sign is now in your backpack.  It will move on it's own during setup, but if you don't complete setup you may want to delete it." );

            from.SendGump( new TownHouseSetupGump( from, sign ) );
        }

        private static bool FindTownField( Map map, Point3D p )
        {
            IPooledEnumerable eable = map.GetItemsInRange( p, 0 );

            foreach( Item item in eable )
            {
                if( item is TownFieldSign )
                {
                    return true;
                }
            }

            eable.Free();

            return false;
        }

        [Usage( "ResetCommercials" )]
        [Description( "Reset town system commercial infoes." )]
        public static void ResetCommercials_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
            {
                foreach( TownSystem t in TownSystem.TownSystems )
                {
                    t.ItemPrices.Clear();
                }

                e.Mobile.SendMessage( "Process done." );
            }
        }

        [Usage( "GenerateTownCommercialInfo" )]
        [Description( "Generate info for commercial system." )]
        public static void GenerateTownCommercialInfo_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
            {
                List<Type> list = new List<Type>();
                List<GenericBuyInfo> toLog = new List<GenericBuyInfo>();

                foreach( Mobile m in World.Mobiles.Values )
                {
                    if( m is BaseVendor )
                    {
                        foreach( IBuyItemInfo info in ( (BaseVendor)m ).GetBuyInfo() )
                        {
                            if( info is GenericBuyInfo )
                            {
                                GenericBuyInfo gbi = (GenericBuyInfo)info;

                                if( list.Contains( gbi.Type ) )
                                    continue;

                                toLog.Add( gbi );
                                list.Add( gbi.Type );
                            }
                        }
                    }
                }

                // toLog.Sort( InternalComparer.Instance );

                TextWriter tw = File.AppendText( "Logs/CommercialInfoes.log" );

                foreach( GenericBuyInfo gbi in toLog )
                {
                    if( gbi == null )
                        continue;

                    if( String.IsNullOrEmpty( gbi.Name ) )
                        continue;

                    Item item = null;

                    try
                    {
                        if( gbi.Args == null || gbi.Args.Length == 0 )
                            item = (Item)Activator.CreateInstance( gbi.Type );
                        else
                            item = (Item)Activator.CreateInstance( gbi.Type, gbi.Args );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex );
                    }

                    /*
                    try
                    {
                        tw.WriteLine( String.Format( "new TownItemPriceDefinition( typeof({0}), \"{1}\", {2}, {3} )",
                            gbi.Type.Name, gbi.Name, gbi.ItemID, gbi.Price ) );
                    }
                    catch { }
                    */

                    try
                    {
                        tw.WriteLine( String.Format( "new TownItemPriceDefinition( typeof({0}), \"{1}\", {2}, {3} )",
                            gbi.Type.Name, StringList.Localization[ int.Parse( gbi.Name ) ], gbi.ItemID, gbi.Price ) );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex );
                    }

                    try
                    {
                        if( item != null && item.Name != null )
                        {
                            tw.WriteLine( String.Format( "new TownItemPriceDefinition( typeof({0}), \"{1}\", {2}, {3} )",
                                gbi.Type.Name, item.Name, gbi.ItemID, gbi.Price ) );
                        }
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex );
                    }

                    if( item != null && !item.Deleted )
                        item.Delete();

                    tw.WriteLine( "" );
                }

                tw.Close();

                e.Mobile.SendMessage( "Process done." );
            }
        }

        /*
        private class InternalComparer : IComparer<GenericBuyInfo>
        {
            public static readonly IComparer<GenericBuyInfo> Instance = new InternalComparer();

            public InternalComparer()
            {
            }

            public int Compare( GenericBuyInfo x, GenericBuyInfo y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                if( x.Type == null || y.Type == null )
                    return 0;

                return Insensitive.Compare( x.Type.Name, y.Type.Name );
            }
        }
        */

        [Usage( "FixTownPlayerStates" )]
        [Description( "Fix null town player states." )]
        public static void FixTownPlayerStates_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
                TownHelper.FixPlayerStates();
            else
                e.Mobile.SendMessage( "Command Use: [FixTownPlayerStates" );
        }

        [Usage( "ListPlayerStates" )]
        [Description( "List ll states for town system" )]
        public static void ListPlayerStates_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
            {
                using( StreamWriter tw = new StreamWriter( "Logs/PlayerStateList.log", true ) )
                {

                    foreach( TownSystem ts in TownSystem.TownSystems )
                    {
                        tw.WriteLine( "Playerstates for {0}", ts.Definition.TownName );

                        List<TownPlayerState> list = new List<TownPlayerState>( ts.Players );

                        list.Sort( InternalComparer.Instance );

                        foreach( TownPlayerState tps in ts.Players )
                        {
                            tw.WriteLine( "{0}\t\t{1}", tps.Mobile.Name, tps.Mobile.Account.Username );
                        }
                    }
                }
            }
            else
                e.Mobile.SendMessage( "Command Use: [FixTownPlayerStates" );
        }

        private class InternalComparer : IComparer<TownPlayerState>
        {
            public static readonly IComparer<TownPlayerState> Instance = new InternalComparer();

            public int Compare( TownPlayerState x, TownPlayerState y )
            {
                if( x == null || y == null )
                    return 0;

                return Insensitive.Compare( x.Mobile.Name, y.Mobile.Name );
            }
        }

        [Usage( "FixCitizenLevel" )]
        [Description( "Fix citizen access level." )]
        public static void FixCitizenLevel_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
                TownHelper.SetBasicCitienAccess();
            else
                e.Mobile.SendMessage( "Command Use: [FixCitizenLevel" );
        }

        [Usage( "KickTown" )]
        [Description( "Remove TownState from Target Player." )]
        public static void KickTown_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( e.Length == 0 )
                from.Target = new InternalTarget( null, ActionType.Kick );
            else
                from.SendMessage( "Command Use: [KickTown" );
        }

        [Usage( "GenerateTownVendors <town>" )]
        [Description( "Generate vendors for townsystem." )]
        public static void GenerateTownVendors_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( e.Length == 1 )
            {
                TownSystem system = TownSystem.Parse( e.GetString( 0 ) );
                if( system != null )
                    system.CommercialStatus.GenerateVendors( from );
                else
                    from.SendMessage( "That is not a valid Midgard Town." );
            }
            else
                from.SendMessage( "Command Use: [GenerateTownVendors <town>" );
        }

        [Usage( "SetTown <newTown>" )]
        [Description( "Set Town of Target Player." )]
        public static void SetTown_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( e.Length == 1 )
            {
                TownSystem system = TownSystem.Parse( e.GetString( 0 ) );
                if( system != null )
                    from.Target = new InternalTarget( system, ActionType.Set );
                else
                    from.SendMessage( "That is not a valid Midgard Town (\"Serpent's Hold\", \"Britain\", \"Buccaneer's Den\")." );
            }
            else
                from.SendMessage( "Command Use: [SetTown <newTown>" );
        }

        [Usage( "ResetTownAccount <account>" )]
        [Description( "Remove a citizen from it's Townstone" )]
        public static void ResetTownAccount_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            string msg;

            if( e.Length == 1 )
            {
                string userName = e.GetString( 0 );
                Account a = Accounts.GetAccount( userName ) as Account;

                if( a != null )
                {
                    if( a.GetTag( "Cittadinanza" ) != null )
                    {
                        TownHelper.DoAccountReset( a );
                        msg = string.Format( "Player pgs from account '{0}' are townable again.", a.Username );
                    }
                    else
                        msg = string.Format( "Account exist but has no town bounded to it." );
                }
                else
                    msg = string.Format( "This account does not exist." );
            }
            else
                msg = string.Format( "Command use: ResetTownAccount <em>account</em>" );

            from.SendGump( new NoticeGump( 1060635, 30720, msg, 0xFFC000, 420, 280, CloseNoticeCallback, null ) );
        }

        [Usage( "PuntiCittadini" )]
        [Description( "Verifica il proprio livello di uccisioni cittadine e nemiche." )]
        public static void CheckTownKills_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            TownPlayerState tps = TownPlayerState.Find( from );
            if( tps != null )
                from.SendMessage( "Il tuo livello-rank cittadino è di {0} punti.", tps.TownRankPoints );
            else
                from.SendMessage( "Devi essere un valido cittadino per poter usare questo comando." );
        }

        public enum ActionType
        {
            Kick,
            Set,
        }

        private class InternalTarget : Target
        {
            private TownSystem m_Town;
            private ActionType m_Action;

            public InternalTarget( TownSystem town, ActionType action )
                : base( 10, false, TargetFlags.None )
            {
                m_Town = town;
                m_Action = action;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted is Midgard2PlayerMobile )
                {
                    Midgard2PlayerMobile m = (Midgard2PlayerMobile)targeted;

                    switch( m_Action )
                    {
                        case ActionType.Kick:
                            TownHelper.DoPlayerReset( m );
                            break;
                        case ActionType.Set:
                            TownHelper.DoSetCitizen( m, m_Town );
                            break;
                        default:
                            break;
                    }
                }
                else if( targeted is TownHouseSign && m_Town != null )
                {
                    ( (TownHouseSign)targeted ).System = m_Town;
                    if( targeted is TownFieldSign )
                        m_Town.RegisterField( (TownFieldSign)targeted );
                    else
                        m_Town.RegisterTownHouse( (TownHouseSign)targeted );
                }
                else if( targeted is TownCharacterStatue && m_Town != null )
                {
                    ( (TownCharacterStatue)targeted ).System = m_Town;
                }
                else
                    from.SendMessage( "You have to target a mobile or a town house or field sign or a statue." );
            }
        }

        private static void CloseNoticeCallback( Mobile from, object state )
        {
        }
    }
}