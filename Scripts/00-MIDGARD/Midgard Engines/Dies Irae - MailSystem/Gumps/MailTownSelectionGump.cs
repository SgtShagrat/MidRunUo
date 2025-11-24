/***************************************************************************
 *                               MailTownSelectionGump.cs
 *
 *   begin                : 31 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MailSystem
{
    public class MailTownSelectionGump : Gump
    {
        #region constants
        private const int Fields = 9;
        private const int HueTit = 662;
        private const int DeltaBut = 2;
        private const int FieldsDist = 25;
        private const int HuePrim = 92;
        #endregion

        private int m_Page;
        private Mobile m_From;
        private MailScroll m_Scroll;
        private List<TownSystem> m_Towns;

        public MailTownSelectionGump( Mobile from, MailScroll scroll )
            : this( from, scroll, BuildList(), 1 )
        {
        }

        public MailTownSelectionGump( Mobile from, MailScroll scroll, List<TownSystem> list, int page )
            : base( 50, 50 )
        {
            Console.WriteLine( "MailTownSelectionGump" );

            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_From = from;
            m_Scroll = scroll;
            m_Page = page;
            m_Towns = list;

            Design();
        }

        private void Design()
        {
            AddPage( 0 );

            AddBackground( 0, 0, 275, 325, 9200 );

            AddImageTiled( 10, 10, 255, 25, 2624 );
            AddImageTiled( 10, 45, 255, 240, 2624 );
            AddImageTiled( 40, 295, 225, 20, 2624 );

            AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
            AddHtmlLocalized( 45, 295, 75, 20, 1011012, HueTit, false, false ); // CANCEL

            AddAlphaRegion( 10, 10, 255, 285 );
            AddAlphaRegion( 40, 295, 225, 20 );

            AddLabelCropped( 14, 12, 255, 25, HueTit, "Choose a town:" );

            if( m_Page > 1 )
                AddButton( 225, 297, 5603, 5607, 200, GumpButtonType.Reply, 0 ); // Previous page

            if( m_Page < Math.Ceiling( m_Towns.Count / (double)Fields ) )
                AddButton( 245, 297, 5601, 5605, 300, GumpButtonType.Reply, 0 ); // Next Page

            int indMax = ( m_Page * Fields ) - 1;
            int indMin = ( m_Page * Fields ) - Fields;
            int indTemp = 0;

            for( int i = 0; i < m_Towns.Count; i++ )
            {
                if( i >= indMin && i <= indMax )
                {
                    AddLabelCropped( 35, 52 + ( indTemp * FieldsDist ), 225, 20, HuePrim, m_Towns[ i ].Definition.TownName );
                    AddButton( 15, 52 + DeltaBut + ( indTemp * FieldsDist ), 1209, 1210, i + 1, GumpButtonType.Reply, 0 );
                    indTemp++;
                }
            }
        }

        private static List<TownSystem> BuildList()
        {
            List<TownSystem> list = new List<TownSystem>();

            foreach( TownSystem system in TownSystem.TownSystems )
                list.Add( system );

            list.Sort( InternalComparer.Instance );

            return list;
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == 0 )
                return;
            else if( info.ButtonID == 200 ) // Previous page
            {
                m_Page--;
                from.SendGump( new MailTownSelectionGump( m_From, m_Scroll, m_Towns, m_Page ) );
            }
            else if( info.ButtonID == 300 )  // Next Page
            {
                m_Page++;
                from.SendGump( new MailTownSelectionGump( m_From, m_Scroll, m_Towns, m_Page ) );
            }
            else
            {
                int index = info.ButtonID - 1;
                if( index > -1 && index < m_Towns.Count )
                    m_Scroll.Town = m_Towns[ index ].Definition.Town;

                from.SendGump( new MailCompositionGump( m_From, m_Scroll ) );
            }
        }

        private class InternalComparer : IComparer<TownSystem>
        {
            public static readonly IComparer<TownSystem> Instance = new InternalComparer();

            public int Compare( TownSystem x, TownSystem y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Definition.TownName, y.Definition.TownName );
            }
        }
    }
}