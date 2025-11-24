/***************************************************************************
 *                               SetAcademyOfficeGump.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.Academies
{
    public class SetAcademyOfficeGump : BaseAcademyGump
    {
        public enum Buttons
        {
            Close,

            SetCustomAcademyOffice = 100,
            ResetAcademyOffice,
            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return Enum.GetValues( typeof( AcademyOffices ) ).Length + 1; } }
        protected override int MainWindowWidth { get { return 195; } }
        protected override int NumButtons { get { return 2; } }
        #endregion

        public SetAcademyOfficeGump( AcademySystem system, Mobile citizen, Mobile owner )
            : base( system, owner, citizen, 50, 50 )
        {
            owner.CloseGump( typeof( SetAcademyOfficeGump ) );

            Design();

            base.RegisterUse( typeof( SetAcademyOfficeGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( "Academy Offices" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int hue = HuePrim;

            int pageX = HorBorder + MainWindowWidth - 25;
            int pageY = GetMainWindowsStartY() + 10;

            // buttons
            AddActionButton( 1, "set custom office", (int)Buttons.SetCustomAcademyOffice, Owner, (int)AcademyAccessFlags.SetAcademyOffice );
            AddActionButton( 2, "reset office", (int)Buttons.ResetAcademyOffice, Owner, (int)AcademyAccessFlags.SetAcademyOffice );

            for( int i = 0; i < Enum.GetValues( typeof( AcademyOffices ) ).Length; i++ )
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

                AddMainWindowButton( y + 5, i, (int)( (int)AcademyAccessFlags.SetAcademyOffice ), false );
                AddLabel( labelOffsetX + 20, y, GetHueFor( hue ), MidgardUtility.GetFriendlyClassName( Enum.GetName( typeof( AcademyOffices ), i ) ) );
            }

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            AcademyPlayerState tps = AcademyPlayerState.Find( Academic );
            if( tps == null )
                return;

            int id = info.ButtonID;

            if( id > 0 && id < Enum.GetValues( typeof( AcademyOffices ) ).Length )
            {
                tps.AcademyOffice = (AcademyOffices)id;
                from.SendMessage( "You have set academy office of {0} to '{1}.", Academic.Name, Enum.GetName( typeof( AcademyOffices ), id ) );
                from.SendGump( new AcademyInfoGump( Academy, Academic, Owner ) );
                return;
            }

            switch( id )
            {
                case (int)Buttons.SetCustomAcademyOffice:
                    from.SendMessage( "Please, enter a new custom academy office." );
                    from.Prompt = new SetCustomAcademyOfficePrompt( Academic );
                    break;
                case (int)Buttons.ResetAcademyOffice:
                    tps.AcademyOffice = AcademyOffices.None;
                    from.SendMessage( "You have reset that academy office." );
                    break;
                default:
                    from.SendGump( new AcademyInfoGump( Academy, Academic, Owner ) );
                    break;
            }
        }
    }
}