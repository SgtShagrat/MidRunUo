/***************************************************************************
 *                                  CitizenInfoGump.cs
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
using Midgard.Misc;
using Server.Accounting;

namespace Midgard.Engines.MidgardTownSystem
{
    public class CitizenInfoGump : TownGump
    {
        public enum Buttons
        {
            SetTownOffice = 1,
            SetProfession,
            SetMail,
            SetICQ,
            SetMSN,

            RemoveCitizen,
            BanCitizen,
	    PermaBanCitizen,
            ToggleDisplayTitle
        }

        #region design variables
        protected override int NumLabels { get { return 13; } }
        protected override int NumButtons { get { return 3; } }
        protected override int MainWindowWidth { get { return 325; } }
        #endregion

        public CitizenInfoGump( TownSystem system, Mobile citizen, Mobile owner )
            : base( system, owner, citizen, 50, 50 )
        {
            owner.CloseGump( typeof( CitizenInfoGump ) );

            Design();

            base.RegisterUse( typeof( CitizenInfoGump ) );
        }

        private void Design()
        {
            Midgard2PlayerMobile m2Pm = Citizen as Midgard2PlayerMobile;
            if( m2Pm == null )
                return;

            TownPlayerState tsp = TownPlayerState.Find( Citizen );

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 190, "Citizen Details:" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int tabbedLabelX = labelOffsetX + 20;

            AddLabel( tabbedLabelX, labelOffsetY, LabelsHue, "Name:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY, DefaultValueHue, Citizen.Name );

            #region Citizen Infoes
            AddLabel( labelOffsetX, labelOffsetY + ( 1 * LabelsOffset ), GroupsHue, "Citizen Infoes:" );

            AddLabel( tabbedLabelX, labelOffsetY + ( 2 * LabelsOffset ), LabelsHue, "Town Office:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 2 * LabelsOffset ), DefaultValueHue, FormatOffice( tsp ) );
            AddMainWindowButton( labelOffsetX, labelOffsetY + ( 2 * LabelsOffset ) + 4, (int)Buttons.SetTownOffice, (int)TownAccessFlags.SetTownOffice );

            AddLabel( tabbedLabelX, labelOffsetY + ( 3 * LabelsOffset ), LabelsHue, "Citizen Kills:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 3 * LabelsOffset ), DefaultValueHue, ( tsp == null ) ? "0" : tsp.CitizenKills.ToString() );

            AddLabel( tabbedLabelX, labelOffsetY + ( 4 * LabelsOffset ), LabelsHue, "Profession: " );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 4 * LabelsOffset ), DefaultValueHue, FormatProfession( tsp ) );
            if( Citizen == Owner || HasAccess( (int)TownAccessFlags.SetProfession ) )
                AddMainWindowButton( labelOffsetX, labelOffsetY + ( 4 * LabelsOffset ) + 4, (int)Buttons.SetProfession, (int)TownAccessFlags.SetProfession, ( Citizen == Owner ) );

            string guildName = Citizen.Guild == null ? "None" : Citizen.Guild.Name;
            AddLabel( tabbedLabelX, labelOffsetY + ( 5 * LabelsOffset ), LabelsHue, "Guild:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 5 * LabelsOffset ), DefaultValueHue, guildName );
            #endregion

            #region Personal Infoes
            AddLabel( labelOffsetX, labelOffsetY + ( 6 * LabelsOffset ), GroupsHue, "Personal Infoes:" );

            AddLabel( tabbedLabelX, labelOffsetY + ( 7 * LabelsOffset ), LabelsHue, "Account:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 7 * LabelsOffset ), DefaultValueHue, FormatPrivateInfo( Citizen.Account.Username ) );

            string lastLogin = ( (PlayerMobile)Citizen ).LastOnline.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" );
            AddLabel( tabbedLabelX, labelOffsetY + ( 8 * LabelsOffset ), LabelsHue, "Last Login:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 8 * LabelsOffset ), DefaultValueHue, FormatPrivateInfo( lastLogin ) );
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
                if( Citizen == Owner || HasAccess( (int)TownAccessFlags.SetInfo ) )
                    AddMainWindowButton( labelOffsetX, labelOffsetY + ( 10 * LabelsOffset ) + 4, (int)Buttons.SetMail, (int)TownAccessFlags.SetInfo, Owner == Citizen );

                AddLabel( tabbedLabelX + 110, labelOffsetY + ( 11 * LabelsOffset ), DefaultValueHue, info.IcqContact );
                if( Citizen == Owner || HasAccess( (int)TownAccessFlags.SetInfo ) )
                    AddMainWindowButton( labelOffsetX, labelOffsetY + ( 11 * LabelsOffset ) + 4, (int)Buttons.SetICQ, (int)TownAccessFlags.SetInfo, Owner == Citizen );

                AddLabel( tabbedLabelX + 110, labelOffsetY + ( 12 * LabelsOffset ), DefaultValueHue, info.MsnContact );
                if( Citizen == Owner || HasAccess( (int)TownAccessFlags.SetInfo ) )
                    AddMainWindowButton( labelOffsetX, labelOffsetY + ( 12 * LabelsOffset ) + 4, (int)Buttons.SetMSN, (int)TownAccessFlags.SetInfo, Owner == Citizen );
            }
            #endregion

            // buttons
            AddActionButton( 1, "remove from town", (int)Buttons.RemoveCitizen, Owner, (int)TownAccessFlags.RemoveCitizen );
            AddActionButton( 2, "ban from town", (int)Buttons.BanCitizen, Owner, (int)TownAccessFlags.BanCitizen );
            AddActionButton( 3, "permaban from town", (int)Buttons.PermaBanCitizen, Owner, (int)TownAccessFlags.PermaBanCitizen );

            bool isShowingTitle = TownSystem.IsShowingTownTitle( Citizen );
            AddActionButton( 4, isShowingTitle ? "hide town title" : "show town title", (int)Buttons.ToggleDisplayTitle, Owner, (int)TownAccessFlags.SetInfo, Owner == Citizen );

            AddCloseButton();
        }

        private static string FormatOffice( TownPlayerState tsp )
        {
            if( tsp == null )
                return "None";
            else if( !String.IsNullOrEmpty( tsp.CustomTownOffice ) )
                return tsp.CustomTownOffice;
            else
                return MidgardUtility.GetFriendlyClassName( tsp.TownOffice.ToString() );
        }

        private static string FormatProfession( TownPlayerState tsp )
        {
            if( tsp == null )
                return "None";
            else if( !String.IsNullOrEmpty( tsp.CustomProfession ) )
                return tsp.CustomProfession;
            else
                return MidgardUtility.GetFriendlyClassName( tsp.TownProfession.ToString() );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.SetTownOffice:
                    from.SendGump( new SetTownOfficeGump( Town, Citizen, Owner ) );
                    break;
                case (int)Buttons.SetProfession:
                    from.SendGump( new SetProfessionGump( Town, Citizen, Owner ) );
                    break;
                case (int)Buttons.SetMail:
                    from.SendMessage( "Please, enter a new e-mail for that citizen." );
                    from.Prompt = new SetInfoPrompt( Owner, Citizen, InfoType.Email );
                    break;
                case (int)Buttons.SetICQ:
                    from.SendMessage( "Please, enter a new icq contact for that citizen." );
                    from.Prompt = new SetInfoPrompt( Owner, Citizen, InfoType.ICQ );
                    break;
                case (int)Buttons.SetMSN:
                    from.SendMessage( "Please, enter a new msn contact for that citizen." );
                    from.Prompt = new SetInfoPrompt( Owner, Citizen, InfoType.MSN );
                    break;
                case (int)Buttons.RemoveCitizen:
                    TownHelper.DoPlayerReset( Citizen );
                    from.SendMessage( "Players from account '{0}' are townable again.", Citizen.Account.Username );
                    break;
                case (int)Buttons.BanCitizen:
                    TownHelper.DoTownBan( Citizen, Town );
                    from.SendMessage( "Player '{0}' has been permanently exiled from {1}.", Citizen.Name, Town.Definition.TownName );
                    break;
                case (int)Buttons.PermaBanCitizen:
                    TownHelper.DoTownPermaBan( Citizen, Town );
                    from.SendMessage( "Player '{0}' has been permanently Perma-exiled from {1}.", Citizen.Name, Town.Definition.TownName );
                    break;
                case (int)Buttons.ToggleDisplayTitle:
                    TownSystem.ToggleCitizenStatusDisplay( Citizen );
                    from.SendMessage( "That citizen will {0}show the town title.", TownSystem.IsShowingTownTitle( Citizen ) ? "" : "not " );
                    from.SendGump( new CitizenManagementGump( Town, from ) );
                    break;
                default:
                    from.SendGump( new CitizenManagementGump( Town, from ) );
                    break;
            }
        }
    }
}