/***************************************************************************
 *                                  TrapsGump.cs
 *                            		------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Midgard.Items;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TrapsGump : TownGump
    {
        public enum Buttons
        {
            Close,
            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 10; } }
        protected override int NumButtons { get { return 0; } }
        protected override int MainWindowWidth { get { return 585; } }
        protected override bool HasSubtitles { get { return true; } }
        #endregion

        public TrapsGump( TownSystem system, Mobile owner )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( TrapsGump ) );

            Design();

            base.RegisterUse( typeof( TrapsGump ) );
        }

        private void Design()
        {
            List<BaseCraftableTrap> traps = Town.TownTraps;

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 205, String.Format( "Traps of {0}", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Type" );
            AddSubTitle( labelOffsetX + 160, "Location" );
            AddSubTitle( labelOffsetX + 300, "Placed By" );
            AddSubTitle( labelOffsetX + 450, "Until" );

            int hue = HuePrim;

            if( traps.Count > 0 )
            {
                BaseCraftableTrap trap;

                for( int i = 0; i < traps.Count; ++i )
                {
                    int page = i / NumLabels;
                    int pos = i % NumLabels;

                    if( pos == 0 )
                    {
                        if( page > 0 )
                            AddButton( 460, 10, NextPageBtnIdNormal, NextPageBtnIdPressed, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                        AddPage( page + 1 );

                        if( page > 0 )
                            AddButton( 440, 10, PrevPageBtnIdNormal, PrevPageBtnIdPressed, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                    }

                    int y = pos * LabelsOffset + labelOffsetY;

                    trap = traps[ i ];
                    hue = GetHueFor( hue );

                    if( trap != null && !trap.Deleted )
                    {
                        AddLabel( labelOffsetX, y, hue, MidgardUtility.GetFriendlyClassName( trap.GetType().Name ) );
                        AddLabel( labelOffsetX + 160, y, hue, trap.Location.ToString() );
                        AddLabel( labelOffsetX + 300, y, hue, trap.Placer.Name );
                        if( trap.TimedDecayEnabled )
                            AddLabel( labelOffsetX + 450, y, hue, trap.TimeOfPlacement.Add( trap.DecayPeriod ).ToShortDateString() );
                    }
                }
            }
            else
                AddLabel( labelOffsetX, labelOffsetY, hue, "- no trap placed -" );

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            if( from == null )
                return;

            if( info.ButtonID == (int)Buttons.Close )
            {
                from.SendGump( new TownSystemInfoGump( Town, Owner ) );
                return;
            }
        }
    }
}