/***************************************************************************
 *                                  TownSystemInfoGump.cs
 *                            		---------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasiaalice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownSystemInfoGump : TownGump
    {
        public enum Buttons
        {
            Close,

            CitizenList,
            Welfare,
            WarFare,
            Fields,
            Houses,
            Traps,
            News,
            AdvancedGump,
            Guide,
            Rank,
            Criminals,
            WantedCriminals
        }

        #region design variables
        protected override int NumLabels { get { return Labels.Length; } }
        protected override int NumButtons { get { return 11; } }
        protected override int MainWindowWidth { get { return 380; } }
        #endregion

        private static readonly string[] Labels = new string[]
        {
            "Midgard Town",
            "Total Citizens",
            "Town Treasure",
            "Town Guilds",
            "Town Fields",
            "Town Traps"
        };

        public TownSystemInfoGump( TownSystem system, Mobile owner )
            : base( system, owner, 50, 50 )
        {
            if( Town == null )
                return;

            Owner.CloseGump( typeof( TownSystemInfoGump ) );

            Design();

            base.RegisterUse( typeof( TownSystemInfoGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 225, "Midgard - Town System Info" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            for( int i = 0; i < Labels.Length; i++ )
                AddLabel( labelOffsetX, labelOffsetY + ( LabelsOffset * i ), GroupsHue, String.Format( "{0}:", Labels[ i ] ) );

            List<string> values = new List<string>();
            values.Add( Town.Definition.TownName );
            values.Add( Town.Players.Count.ToString() );
            values.Add( Town.TownTreasure.ToString() );
            values.Add( Town.NumTownGuilds.ToString() );
            values.Add( Town.SystemFields.Count.ToString() );
            values.Add( Town.TownTraps.Count.ToString() );

            for( int i = 0; i < values.Count; i++ )
                AddLabel( labelOffsetX + 100, labelOffsetY + ( 20 * i ), DefaultValueHue, values[ i ] );

            bool isGM = Owner.AccessLevel > AccessLevel.Counselor;

            // Buttons
            AddActionButton( 1, "citizen list", (int)Buttons.CitizenList, Owner, (int)TownAccessFlags.Citizen, isGM );
            AddActionButton( 2, "welfare", (int)Buttons.Welfare, Owner, (int)TownAccessFlags.Citizen, isGM );
            AddActionButton( 3, "warfare", (int)Buttons.WarFare, Owner, (int)TownAccessFlags.Citizen, isGM );
            AddActionButton( 4, "fields", (int)Buttons.Fields, Owner, (int)TownAccessFlags.Citizen, isGM );
            AddActionButton( 5, "houses", (int)Buttons.Houses, Owner, (int)TownAccessFlags.Citizen, isGM );
            AddActionButton( 6, "traps", (int)Buttons.Traps, Owner, (int)TownAccessFlags.Citizen, isGM );
            AddActionButton( 7, "news", (int)Buttons.News, Owner, (int)TownAccessFlags.ClearNews, isGM );
            AddActionButton( 8, "advanced features", (int)Buttons.AdvancedGump, Owner, (int)TownAccessFlags.Citizen, isGM );
            AddActionButton( 9, "town ranks", (int)Buttons.Rank, Owner, (int)TownAccessFlags.Citizen, isGM );
            AddActionButton( 10, "crimes", (int)Buttons.Criminals, Owner, (int)TownAccessFlags.Citizen, isGM );
            AddActionButton( 11, "wanted criminals", (int)Buttons.WantedCriminals, Owner, (int)TownAccessFlags.Citizen, isGM );

            AddCloseButton();
            AddGuideButton( (int)Buttons.Guide );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( Town == null )
                return;

            Mobile from = sender.Mobile;
            if( from == null )
                return;

            switch( info.ButtonID )
            {
                case (int)Buttons.CitizenList:
                    from.SendGump( new CitizenManagementGump( Town, Owner ) );
                    break;
                case (int)Buttons.Welfare:
                    from.SendGump( new WelfareGump( Town, Owner ) );
                    break;
                case (int)Buttons.WarFare:
                    from.SendGump( new WarfareGump( Town, Owner ) );
                    break;
                case (int)Buttons.Fields:
                    from.SendGump( new FieldsGump( Town, Owner ) );
                    break;
                case (int)Buttons.Houses:
                    from.SendGump( new HousesGump( Town, Owner ) );
                    break;
                case (int)Buttons.Traps:
                    from.SendGump( new TrapsGump( Town, Owner ) );
                    break;
                case (int)Buttons.News:
                    from.SendGump( new TownCrierGump( Owner, Town ) );
                    break;
                case (int)Buttons.AdvancedGump:
                    from.SendGump( new AdvancedFeaturesGump( Town, Owner ) );
                    break;
                case (int)Buttons.Rank:
                    from.SendGump( new TownRanksGump( Town, Owner ) );
                    break;
                case (int)Buttons.Criminals:
                    from.SendGump( new TownCriminalsGump( Town, Owner ) );
                    break;
                case (int)Buttons.WantedCriminals:
                    from.SendGump( new CriminalProfilesGump( Town, Owner ) );
                    break;
                case (int)Buttons.Guide:
                    from.LaunchBrowser( Config.SiteGuide );
                    break;
                default:
                    from.SendMessage( "You closed that info menu." );
                    break;
            }
        }
    }
}