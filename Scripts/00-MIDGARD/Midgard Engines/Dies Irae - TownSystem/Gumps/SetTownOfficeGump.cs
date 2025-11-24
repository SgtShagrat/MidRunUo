/***************************************************************************
 *                                  SetProfessionGump.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class SetTownOfficeGump : TownGump
    {
        public enum Buttons
        {
            Close,

            SetCustomTownOffice = 100,
            ResetTownOffice,
            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 16; } }
        protected override int NumButtons { get { return 2; } }
        protected override int MainWindowWidth { get { return 195; } }
        protected override bool HasCloseBtn { get { return false; } }
        #endregion

        public SetTownOfficeGump( TownSystem system, Mobile citizen, Mobile owner )
            : base( system, owner, citizen, 50, 50 )
        {
            owner.CloseGump( typeof( SetTownOfficeGump ) );

            Design();

            base.RegisterUse( typeof( SetTownOfficeGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 150, "Town Offices" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int hue = HuePrim;

            int pageX = HorBorder + MainWindowWidth - 25;
            int pageY = GetMainWindowsStartY() + 10;

            // buttons
            AddActionButton( 1, "set custom town office", (int)Buttons.SetCustomTownOffice, Owner, (int)TownAccessFlags.SetCustomTownOffice );
            AddActionButton( 2, "reset town office", (int)Buttons.ResetTownOffice, Owner, (int)TownAccessFlags.SetCustomTownOffice );

            for( int i = 0; i < Enum.GetValues( typeof( TownOffices ) ).Length; i++ )
            {
                int page = i / NumLabels;
                int pos = i % NumLabels;

                if( pos == 0 )
                {
                    if( page == 0 )
                        AddLabel( labelOffsetX, labelOffsetY, TitleHue, "Choose from:" );

                    if( page > 0 )
                        AddButton( pageX, pageY, NextPageBtnIdNormal, NextPageBtnIdPressed, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                    AddPage( page + 1 );

                    if( page > 0 )
                        AddButton( pageX - 20, pageY, PrevPageBtnIdNormal, PrevPageBtnIdPressed, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                }

                if( i == 0 )
                    continue; // do not display 'none' case

                int y = pos * LabelsOffset + labelOffsetY;

                hue = GetHueFor( hue );

                AddMainWindowButton( labelOffsetX, y + 5, i, (int)TownAccessFlags.SetTownOffice );
                AddLabel( labelOffsetX + 20, y, GetHueFor( hue ), MidgardUtility.GetFriendlyClassName( Enum.GetName( typeof( TownOffices ), i ) ) );
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            TownPlayerState tps = TownPlayerState.Find( Citizen );
            if( tps == null )
                return;

            int id = info.ButtonID;

            if( id > 0 && id < Enum.GetValues( typeof( TownOffices ) ).Length )
            {
                tps.TownOffice = (TownOffices)id;
                from.SendMessage( "You have set town office of {0} to '{1}.", Citizen.Name, Enum.GetName( typeof( TownOffices ), id ) );
                from.SendGump( new CitizenInfoGump( Town, Citizen, Owner ) );
                return;
            }

            switch( id )
            {
                case (int)Buttons.SetCustomTownOffice:
                    from.SendMessage( "Please, enter a new custom town office for that citizen." );
                    from.Prompt = new SetCustomTownOfficePrompt( Owner, Citizen );
                    break;
                case (int)Buttons.ResetTownOffice:
                    tps.TownOffice = TownOffices.None;
                    from.SendMessage( "You have reset that town office." );
                    break;
                default:
                    from.SendGump( new CitizenInfoGump( Town, Citizen, Owner ) );
                    break;
            }
        }
    }
}