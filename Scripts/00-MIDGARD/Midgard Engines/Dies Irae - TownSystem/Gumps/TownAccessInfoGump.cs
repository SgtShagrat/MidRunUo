/***************************************************************************
 *                                  TownAccessInfoGump.cs
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
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownAccessInfoGump : TownGump
    {
        public enum Buttons
        {
            Close = 0,

            SetTownLevel = 1000000,
            ResetAccess
        }

        #region design variables
        protected override int NumLabels { get { return TownAccessLevel.GetTownFlags() + 1; } }
        protected override int NumButtons { get { return 2; } }
        protected override int MainWindowWidth { get { return 290; } }
        #endregion

        public TownAccessInfoGump( TownSystem system, Mobile citizen, Mobile owner )
            : base( system, owner, citizen, 50, 50 )
        {
            owner.CloseGump( typeof( TownAccessInfoGump ) );

            Design();

            base.RegisterUse( typeof( TownAccessInfoGump ) );
        }

        private void Design()
        {
            TownPlayerState tps = TownPlayerState.Find( Citizen );
            if( tps == null )
                return;

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 190, "Access Details:" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            AddLabel( labelOffsetX, labelOffsetY, TitleHue, "Town Commands:" );

            int offset = 1;
            foreach( int i in Enum.GetValues( typeof( TownAccessFlags ) ) )
            {
                TownAccessFlags flag = (TownAccessFlags)i;

                // livelli speciali delle flags non vanno mostrati nella main window
                if( flag == TownAccessFlags.None || flag == TownAccessFlags.Citizen || flag == TownAccessFlags.Staff )
                    continue;

                AddLabel( labelOffsetX + 20, labelOffsetY + ( offset * LabelsOffset ), GroupsHue, MidgardUtility.GetFriendlyClassName( Enum.GetName( typeof(TownAccessFlags), i ) ) );

                int hue;
                string label = FormatFlagState( tps, flag, out hue );

                AddLabel( labelOffsetX + 20 + 150, labelOffsetY + ( offset * LabelsOffset ), hue, label );
                AddMainWindowButton( labelOffsetX, labelOffsetY + ( offset * LabelsOffset ) + 4, i, (int) TownAccessFlags.Staff );

                offset++;
            }

            // buttons
            AddActionButton( 1, "set town level", (int)Buttons.SetTownLevel, Owner, (int) TownAccessFlags.SetTownAccess );
            AddActionButton( 2, "reset town access", (int)Buttons.ResetAccess, Owner, (int) TownAccessFlags.SetTownAccess );

            AddCloseButton();
        }

        private string FormatFlagState( TownPlayerState state, TownAccessFlags flag, out int hue )
        {
            if( state != null && state.TownLevel != null && state.TownLevel.GetFlag( flag ) )
            {
                hue = DefaultValueHue;
                return "enabled";
            }
            else
            {
                hue = 37;
                return "disabled";
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            TownPlayerState tps = TownPlayerState.Find( Citizen );
            if( tps == null )
                return;

            int id = info.ButtonID;

            if( id > 0 && id <= TownAccessLevel.GetMaxFlagValue() )
            {
                TownAccessLevel level = tps.TownLevel ?? TownAccessLevel.Citizen;

                bool enabled = level.GetFlag( (TownAccessFlags)id );
                level.SetFlag( (TownAccessFlags)id, !enabled );

                from.SendMessage( "You have set property of {0} of player '{1} to {2}.", (TownAccessFlags)id, Citizen.Name, !enabled );

                from.SendGump( new TownAccessInfoGump( Town, Citizen, Owner ) );
                return;
            }

            switch( id )
            {
                case (int)Buttons.ResetAccess:
                    if( tps.TownLevel != null )
                        tps.TownLevel.ResetFlags();

                    from.SendMessage( "You have reset access level of {0}", Citizen.Name );
                    from.SendGump( new TownAccessInfoGump( Town, Citizen, Owner ) );
                    break;
                case (int)Buttons.SetTownLevel:
                    from.SendMessage( "Not implemented yet." );
                    from.SendGump( new TownAccessInfoGump( Town, Citizen, Owner ) );
                    break;
                default:
                    from.SendGump( new EditTownAccessGump( Town, Owner ) );
                    break;
            }
        }
    }
}