/***************************************************************************
 *                                  ItemPricesSetFilterGump.cs
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
    public class ItemPricesSetFilterGump : TownGump
    {
        public enum Buttons
        {
            Close = 0,

            SetFilter = 1000,
            ResetFilter
        }

        #region design variables
        protected override int NumLabels { get { return ItemFilter.GetMaxFlagValue() + 1; } }
        protected override int NumButtons { get { return 2; } }
        protected override int MainWindowWidth { get { return 290; } }
        #endregion

        private readonly ItemFilter m_Filter;

        public ItemPricesSetFilterGump( TownSystem system, Mobile citizen, Mobile owner )
            : this( system, citizen, owner, null )
        {
        }

        public ItemPricesSetFilterGump( TownSystem system, Mobile citizen, Mobile owner, ItemFilter filter )
            : base( system, owner, citizen, 50, 50 )
        {
            owner.CloseGump( typeof( ItemPricesSetFilterGump ) );

            m_Filter = filter ?? new ItemFilter();

            Design();

            base.RegisterUse( typeof( ItemPricesSetFilterGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 190, "Filter Details:" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            AddLabel( labelOffsetX, labelOffsetY, TitleHue, "Filters:" );

            int offset = 1;
            foreach( int i in Enum.GetValues( typeof( ItemFilterFlags ) ) )
            {
                ItemFilterFlags flag = (ItemFilterFlags)i;

                // livelli speciali delle flags non vanno mostrati nella main window
                if( flag == ItemFilterFlags.None )
                    continue;

                AddLabel( labelOffsetX + 20, labelOffsetY + ( offset * LabelsOffset ), GroupsHue, MidgardUtility.GetFriendlyClassName( Enum.GetName( typeof( ItemFilterFlags ), i ) ) );

                int hue;
                string label = FormatFlagState( flag, out hue );

                AddLabel( labelOffsetX + 20 + 150, labelOffsetY + ( offset * LabelsOffset ), hue, label );
                AddMainWindowButton( labelOffsetX, labelOffsetY + ( offset * LabelsOffset ) + 4, i, (int) TownAccessFlags.Staff );

                offset++;
            }

            // buttons
            AddActionButton( 1, "set filter", (int)Buttons.SetFilter, Owner, (int) TownAccessFlags.Citizen );
            AddActionButton( 2, "reset filter", (int)Buttons.ResetFilter, Owner, (int) TownAccessFlags.Citizen );

            AddCloseButton();
        }

        private string FormatFlagState( ItemFilterFlags flag, out int hue )
        {
            if( m_Filter.GetFlag( flag ) )
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

            int id = info.ButtonID;

            if( id > 0 && id <= ItemFilter.GetMaxFlagValue() + 1 )
            {
                bool enabled = m_Filter.GetFlag( (ItemFilterFlags)id );

                m_Filter.SetFlag( (ItemFilterFlags)id, !enabled );

                from.SendGump( new ItemPricesSetFilterGump( Town, Citizen, Owner, m_Filter ) );
                return;
            }

            switch( id )
            {
                case (int)Buttons.ResetFilter:
                    from.SendGump( new ItemPricesSetFilterGump( Town, Citizen, Owner ) );
                    break;
                case (int)Buttons.SetFilter:
                    from.SendGump( new ItemPricesGump( Town, Owner, m_Filter ) );
                    break;
                default:
                    from.SendGump( new ItemPricesGump( Town, Owner ) );
                    break;
            }
        }
    }
}