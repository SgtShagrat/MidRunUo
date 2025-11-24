/***************************************************************************
 *                                  SetProfessionGump.cs
 *                            		-------------------
 *  begin                	: Marzo, 20088
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class SetProfessionGump : TownGump
    {
        public enum Buttons
        {
            SetCustomProfession = 100,
            ResetProfession,
            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 16; } }
        protected override int NumButtons { get { return 2; } }
        protected override int MainWindowWidth { get { return 195; } }
        protected override bool HasCloseBtn { get { return false; } }
        #endregion

        public SetProfessionGump( TownSystem system, Mobile citizen, Mobile owner )
            : base( system, owner, citizen, 50, 50 )
        {
            owner.CloseGump( typeof( SetProfessionGump ) );

            Design();

            base.RegisterUse( typeof( SetProfessionGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 150, "Town Professions" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int hue = HuePrim;

            int pageX = HorBorder + MainWindowWidth - 25;
            int pageY = GetMainWindowsStartY() + 10;

            // buttons
            AddActionButton( 1, "set custom profession", (int)Buttons.SetCustomProfession, Owner, (int) TownAccessFlags.SetCustomProfession );
            AddActionButton( 2, "reset profession", (int)Buttons.ResetProfession, Owner, (int) TownAccessFlags.SetProfession, Owner == Citizen );

            for( int i = 1; i < Enum.GetValues( typeof( Professions ) ).Length; i++ )
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

                AddLabel( labelOffsetX + 20, y, GetHueFor( hue ), MidgardUtility.GetFriendlyClassName( Enum.GetName( typeof( Professions ), i ) ) );
                AddMainWindowButton( labelOffsetX, y + 5, i, (int) TownAccessFlags.SetProfession, Owner == Citizen );
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            TownPlayerState tps = TownPlayerState.Find( Citizen );
            if( tps == null )
                return;

            int id = info.ButtonID;

            if( id > 0 && id < Enum.GetValues( typeof( Professions ) ).Length )
            {
                tps.TownProfession = (Professions)id;
                from.SendMessage( "You have set profession of {0} to '{1}.", Citizen.Name, Enum.GetName( typeof( Professions ), id ) );
                from.SendGump( new CitizenInfoGump( Town, Citizen, Owner ) );
                return;
            }

            switch( id )
            {
                case (int)Buttons.SetCustomProfession:
                    from.SendMessage( "Please, enter a new custom profession for that citizen." );
                    from.Prompt = new SetCustomProfessionPrompt( Owner, Citizen );

                   //from.SendStringQuery( new TownConfirmationQuery( "Please, enter a new custom profession for that citizen.",
                   //     "Enter a new valid string.(max 8 ch.)",
                   //     false,
                   //     8,
                   //     delegate( Mobile f, bool okay, string text, object state )
                   //         {
                   //             if( okay )
                   //             {
                   //                 int newScalar;

                   //                 if( int.TryParse( text, out newScalar ) )
                   //                 {
                   //                     TownVendorState vendorState = state as TownVendorState;

                   //                     if( vendorState != null )
                   //                     {
                   //                         vendorState.Scalar = newScalar / 100.0;
                   //                         from.SendMessage( "The new scalar is set to: " + newScalar );
                   //                     }
                   //                     else
                   //                     {
                   //                         from.SendMessage( "Vendor state is null for this mobile." );
                   //                     }
                   //                 }
                   //                 else
                   //                 {
                   //                     from.SendMessage( "That is not a valid value. Try with an interger." );
                   //                 }
                   //             }  
                   //         },
                   //     m_State ) );

                    break;
                case (int)Buttons.ResetProfession:
                    tps.TownProfession = Professions.None;
                    from.SendMessage( "You have reset that profession." );
                    break;
                default:
                    from.SendGump( new CitizenInfoGump( Town, Citizen, Owner ) );
                    break;
            }
        }
    }
}