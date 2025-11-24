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
using Server;
using Server.Gumps;
using Server.Network;
using System.Collections.Generic;

namespace Midgard.Engines.MidgardTownSystem
{
    public class FieldsGump : TownGump
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

        public FieldsGump( TownSystem system, Mobile owner )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( FieldsGump ) );

            Design();

            base.RegisterUse( typeof( FieldsGump ) );
        }

        private void Design()
        {
            List<TownFieldSign> fields = Town.SystemFields;

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 205, String.Format( "Fields of {0}", Town.Definition.TownName ) );
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

            if( fields.Count > 0 )
            {
                TownFieldSign field;

                for( int i = 0; i < fields.Count; ++i )
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

                    field = fields[ i ];
                    hue = GetHueFor( hue );

                    if( field != null && !field.Deleted )
                    {
                        int y = pos * LabelsOffset + labelOffsetY;

                        AddLabel( labelOffsetX, y, hue, !String.IsNullOrEmpty( field.Name ) ? field.Name : "town field" );
                        AddLabel( labelOffsetX + 150, y, hue, field.Location.ToString() );
                        AddLabel( labelOffsetX + 290, y, hue, ( field.House != null ) ? field.House.Owner.Name : "- not rent -" );
                        AddLabel( labelOffsetX + 440, y, hue, field.RentTime.ToShortDateString() );

                        if( field.House != null && field.House.Owner != null )
                            AddMainWindowButton( y + 5, i + 1, (int)TownAccessFlags.RemoveInactiveStates, false );
                    }
                }
            }
            else
                AddLabel( labelOffsetX, labelOffsetY, hue, "- no field found -" );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            if( from == null )
                return;
            int id = info.ButtonID - 1;

            if( id < 0 || id >= Town.SystemFields.Count )
            {
                from.SendGump( new TownSystemInfoGump( Town, Owner ) );
                return;
            }

            TownFieldSign field = Town.SystemFields[ id ];
            if( field == null )
                return;

            from.SendGump( new WarningGump( 1060635, 30720,
                "You are about to pack up that field. " +
                "All items locked down and secured will be placed in the Owner bank box. " +
                "All movable items will be deleted. " +
                "The town field will be effective rentable again. " +
                "Do you wish to continue?",
                0xFFC000, 360, 260, new WarningGumpCallback( EndTownFieldRent_Callback ), field ) );
        }

        protected void EndTownFieldRent_Callback( Mobile from, bool okay, object state )
        {
            TownFieldSign field = state as TownFieldSign;
            if( field == null )
                return;

            if( okay )
                field.PackUpHouse();

            from.SendGump( new TownSystemInfoGump( Town, Owner ) );
        }
    }
}