/***************************************************************************
 *                                  FieldsGump.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Midgard.Engines.TownHouses;
using Server;
using Server.Gumps;
using Server.Network;
using System.Collections.Generic;

namespace Midgard.Engines.MidgardTownSystem
{
    public class HousesGump : TownGump
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

        public HousesGump( TownSystem system, Mobile owner )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( HousesGump ) );

            Design();

            base.RegisterUse( typeof( HousesGump ) );
        }

        private void Design()
        {
            List<TownHouseSign> houses = Town.SystemTownHouses;

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 205, String.Format( "Houses of {0}", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Name" );
            AddSubTitle( labelOffsetX + 150, "Location" );
            AddSubTitle( labelOffsetX + 290, "Rented To" );
            AddSubTitle( labelOffsetX + 440, "Until" );
            
            AddCloseButton();

            int hue = HuePrim;

            if( houses.Count > 0 )
            {
                TownHouseSign house;

                for( int i = 0; i < houses.Count; ++i )
                {
                    int page = i / NumLabels;
                    int pos = i % NumLabels;

                    if( pos == 0 )
                    {
                        if( page > 0 )
                            AddButton( 575, 10, 0x15E1, 0x15E5, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                        AddPage( page + 1 );

                        if( page > 0 )
                            AddButton( 555, 10, 0x15E3, 0x15E7, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                    }

                    house = houses[ i ];
                    hue = GetHueFor( hue );

                    if( house != null && !house.Deleted )
                    {
                        int y = pos * LabelsOffset + labelOffsetY;

                        AddLabel( labelOffsetX, y, hue, !String.IsNullOrEmpty( house.Name ) ? house.Name : "town field" );
                        AddLabel( labelOffsetX + 150, y, hue, house.Location.ToString() );
                        AddLabel( labelOffsetX + 290, y, hue, ( house.House != null ) ? house.House.Owner.Name : "- not rent -" );

                        AddLabel( labelOffsetX + 440, y, hue, house.RentTime.ToShortDateString() );
                    }
                }
            }
            else
                AddLabel( labelOffsetX, labelOffsetY, hue, "- no house found -" );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            if( from == null )
                return;

            switch( info.ButtonID )
            {
                default:
                    from.SendGump( new TownSystemInfoGump( Town, Owner ) );
                    break;
            }
        }
    }
}