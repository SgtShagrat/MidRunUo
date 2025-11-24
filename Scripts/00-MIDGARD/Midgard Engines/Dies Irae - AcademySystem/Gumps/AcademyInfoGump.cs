/***************************************************************************
 *                               AcademyInfoGump.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Midgard.Misc;

namespace Midgard.Engines.Academies
{
    public class AcademyInfoGump : BaseAcademyGump
    {
        public enum Buttons
        {
            SetAcademyOffice = 1,

            RemoveAcademic
        }

        #region design variables
        protected override int NumLabels { get { return 13; } }
        protected override int NumButtons { get { return 1; } }
        protected override int MainWindowWidth { get { return 325; } }
        #endregion

        public AcademyInfoGump( AcademySystem system, Mobile citizen, Mobile owner )
            : base( system, owner, citizen, 50, 50 )
        {
            owner.CloseGump( typeof( AcademyInfoGump ) );

            Design();

            base.RegisterUse( typeof( AcademyInfoGump ) );
        }

        private void Design()
        {
            Midgard2PlayerMobile m2Pm = Academic as Midgard2PlayerMobile;
            if( m2Pm == null )
                return;

            AcademyPlayerState tsp = AcademyPlayerState.Find( Academic );

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( "Academic Details:" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int tabbedLabelX = labelOffsetX + 20;

            AddLabel( tabbedLabelX, labelOffsetY, LabelsHue, "Name:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY, DefaultValueHue, Academic.Name );

            #region Academic Infoes
            AddLabel( labelOffsetX, labelOffsetY + ( 1 * LabelsOffset ), GroupsHue, "Academic Infoes:" );

            AddLabel( tabbedLabelX, labelOffsetY + ( 2 * LabelsOffset ), LabelsHue, "Academy Office:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 2 * LabelsOffset ), DefaultValueHue, FormatOffice( tsp ) );
            AddMainWindowButton( labelOffsetY + ( 2 * LabelsOffset ) + 4, (int)Buttons.SetAcademyOffice, (int)AcademyAccessFlags.RemoveAcademic, false );

            string guildName = Academic.Guild == null ? "None" : Academic.Guild.Name;
            AddLabel( tabbedLabelX, labelOffsetY + ( 3 * LabelsOffset ), LabelsHue, "Guild:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 3 * LabelsOffset ), DefaultValueHue, guildName );
            #endregion

            #region Personal Infoes
            AddLabel( labelOffsetX, labelOffsetY + ( 5 * LabelsOffset ), GroupsHue, "Personal Infoes:" );

            AddLabel( tabbedLabelX, labelOffsetY + ( 6 * LabelsOffset ), LabelsHue, "Account:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 6 * LabelsOffset ), DefaultValueHue, FormatPrivateInfo( Academic.Account.Username ) );

            string lastLogin = ( (PlayerMobile)Academic ).LastOnline.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" );
            AddLabel( tabbedLabelX, labelOffsetY + ( 7 * LabelsOffset ), LabelsHue, "Last Login:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 7 * LabelsOffset ), DefaultValueHue, FormatPrivateInfo( lastLogin ) );
            #endregion

            #region General Infoes
            AddLabel( labelOffsetX, labelOffsetY + ( 9 * LabelsOffset ), GroupsHue, "General Infoes:" );

            AddLabel( tabbedLabelX, labelOffsetY + ( 10 * LabelsOffset ), LabelsHue, "Mail:" );
            AddLabel( tabbedLabelX, labelOffsetY + ( 11 * LabelsOffset ), LabelsHue, "ICQ contact:" );
            AddLabel( tabbedLabelX, labelOffsetY + ( 12 * LabelsOffset ), LabelsHue, "Msn contact:" );

            PersonalInfo info = m2Pm.Info;
            if( info != null )
            {
                AddLabel( tabbedLabelX + 110, labelOffsetY + ( 10 * LabelsOffset ), DefaultValueHue, info.Email );

                AddLabel( tabbedLabelX + 110, labelOffsetY + ( 11 * LabelsOffset ), DefaultValueHue, info.IcqContact );

                AddLabel( tabbedLabelX + 110, labelOffsetY + ( 12 * LabelsOffset ), DefaultValueHue, info.MsnContact );
            }
            #endregion

            // buttons
            AddActionButton( 1, "remove from academy", (int)Buttons.RemoveAcademic, Owner, (int)AcademyAccessFlags.RemoveAcademic );

            AddCloseButton();
        }

        private static string FormatOffice( AcademyPlayerState tsp )
        {
            if( tsp == null )
                return "None";
            else if( !String.IsNullOrEmpty( tsp.CustomAcademyOffice ) )
                return tsp.CustomAcademyOffice;
            else
                return MidgardUtility.GetFriendlyClassName( tsp.AcademyOffice.ToString() );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.SetAcademyOffice:
                    from.SendGump( new SetAcademyOfficeGump( Academy, Academic, Owner ) );
                    break;
                case (int)Buttons.RemoveAcademic:
                    AcademySystemCommands.DoPlayerReset( Academic );
                    from.SendMessage( "Academic has been removed." );
                    break;
                default:
                    from.SendGump( new AcademyManagementGump( Academy, from ) );
                    break;
            }
        }
    }
}