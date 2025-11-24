/***************************************************************************
 *                               Dies Irae - SimpleListGump.cs
 *
 *   begin                : 07 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
    public abstract class SimpleListGump : Gump
    {
        protected const int Fields = 9;
        protected const int HueTit = 32767;
        protected const int HuePrim = 92;
        protected const int HueEnt = 32767;
        protected const int DeltaBut = 2;
        protected const int FieldsDist = 25;

        public abstract string Title { get; }

        private readonly int m_Page;
        private Mobile m_Owner;
        private readonly List<string> m_Labels;

        protected SimpleListGump( Mobile owner )
            : this( owner, null, 1 )
        {
        }

        protected SimpleListGump( Mobile owner, List<string> labels, int page )
            : base( 50, 50 )
        {
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_Owner = owner;
            m_Labels = labels;
            m_Page = page;

            Design();
        }

        public void Design()
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

            AddLabelCropped( 14, 12, 255, 25, HueTit, Title );

            if( m_Page > 1 )
                AddButton( 225, 297, 5603, 5607, 200, GumpButtonType.Reply, 0 ); // Previous page

            if( m_Page < Math.Ceiling( m_Labels.Count / (double)Fields ) )
                AddButton( 245, 297, 5601, 5605, 300, GumpButtonType.Reply, 0 ); // Next Page

            int indMax = ( m_Page * Fields ) - 1;
            int indMin = ( m_Page * Fields ) - Fields;
            int indTemp = 0;

            for( int i = 0; i < m_Labels.Count; i++ )
            {
                if( i >= indMin && i <= indMax )
                {
                    AddLabelCropped( 35, 52 + ( indTemp * FieldsDist ), 225, 20, HuePrim, Title );
                    AddButton( 15, 52 + DeltaBut + ( indTemp * FieldsDist ), 1209, 1210, i + 1, GumpButtonType.Reply, 0 );
                    indTemp++;
                }
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == 0 )
                return;
        }
    }
}