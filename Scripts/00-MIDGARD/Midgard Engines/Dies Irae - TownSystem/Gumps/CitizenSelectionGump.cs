using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class CitizenSelectionGump : TownGump
    {
        public enum Buttons
        {
            Close,
            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 20; } }
        protected override int NumButtons { get { return 0; } }
        protected override int MainWindowWidth { get { return 300; } }
        protected override bool HasSubtitles { get { return true; } }
        #endregion

        private readonly List<Mobile> m_Mobiles;

        public CitizenSelectionGump( TownSystem system, Mobile owner )
            : this( system, owner, null )
        {
        }

        public CitizenSelectionGump( TownSystem system, Mobile owner, IEnumerable<Mobile> list )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( CitizenSelectionGump ) );

            if( list == null )
                m_Mobiles = BuildList( system );
            else
                m_Mobiles = new List<Mobile>( list );

            Design();

            base.RegisterUse( typeof( CitizenSelectionGump ) );
        }

        private class InternalComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new InternalComparer();

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
        }

        private static List<Mobile> BuildList( TownSystem system )
        {
            List<Mobile> list = system.GetMobilesFromStates();
            list.Sort( InternalComparer.Instance );
            return list;
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 200, String.Format( "Citizens of {0}", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Name" );

            int hue = HuePrim;

            for( int i = 0; i < m_Mobiles.Count; ++i )
            {
                Mobile m = m_Mobiles[ i ];
                if( m == null || m.Deleted )
                    continue;

                int page = i / NumLabels;
                int pos = i % NumLabels;

                if( pos == 0 )
                {
                    if( page > 0 )
                        AddButton( 460, 10, 0x15E1, 0x15E5, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                    AddPage( page + 1 );

                    if( page > 0 )
                        AddButton( 440, 10, 0x15E3, 0x15E7, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                }

                int y = pos * LabelsOffset + labelOffsetY;

                AddLabel( labelOffsetX + 20, y, GetHueFor( m, hue ), m.Name );
                AddMainWindowButton( labelOffsetX, y + 4, i + 1, (int) TownAccessFlags.Citizen );
            }

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == (int)Buttons.Close )
            {
                from.SendGump( new TownSystemInfoGump( Town, from ) );
                return;
            }

            if( info.ButtonID < 0 || info.ButtonID > m_Mobiles.Count )
                return;

            Mobile m = m_Mobiles[ info.ButtonID - 1 ];
            if( m == null || m.Deleted )
                return;

            from.SendMessage( "You have chosen {0}", m.Name );
           //  from.SendGump( new MailComposerGump( from, m ) );
        }
    }
}