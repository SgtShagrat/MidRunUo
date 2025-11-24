/***************************************************************************
 *                               WarChoiceGump.cs
 *                            -------------------
 *   begin                : 01 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.OrderChaosWars
{
    public class WarChoiceGump : Gump
    {
        protected const int Fields = 9;
        protected const int HueTit = 662;
        protected const int HuePrim = 92;
        protected const int DeltaBut = 2;
        protected const int FieldsDist = 25;

        private int m_Page;

        private readonly Mobile m_Owner;
        private readonly BaseWar[] m_Wars;

        public WarChoiceGump( Mobile owner )
            : this( owner, 1 )
        {
        }

        public WarChoiceGump( Mobile owner, int page )
            : base( 50, 50 )
        {
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_Owner = owner;
            m_Page = page;
            m_Wars = BaseWar.Wars;

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

            AddLabelCropped( 14, 12, 255, 25, HueTit, "Choose your war:" );

            if( m_Page > 1 )
                AddButton( 225, 297, 5603, 5607, 200, GumpButtonType.Reply, 0 ); // Previous page

            if( m_Page < Math.Ceiling( m_Wars.Length / (double)Fields ) )
                AddButton( 245, 297, 5601, 5605, 300, GumpButtonType.Reply, 0 ); // Next Page

            int indMax = ( m_Page * Fields ) - 1;
            int indMin = ( m_Page * Fields ) - Fields;
            int indTemp = 0;

            for( int i = 0; i < m_Wars.Length; i++ )
            {
                if( i < indMin || i > indMax )
                    continue;

                AddLabelCropped( 35, 52 + ( indTemp * FieldsDist ), 225, 20, HuePrim, m_Wars[ i ].Definition.WarName );
                AddButton( 15, 52 + DeltaBut + ( indTemp * FieldsDist ), 1209, 1210, i + 1, GumpButtonType.Reply, 0 );
                indTemp++;
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == 0 )
            {
                return;
            }
            else if( info.ButtonID == 200 ) // Previous page
            {
                m_Page--;
                from.SendGump( new WarChoiceGump( m_Owner, m_Page ) );
            }
            else if( info.ButtonID == 300 )  // Next Page
            {
                m_Page++;
                from.SendGump( new WarChoiceGump( m_Owner, m_Page ) );
            }
            else
            {
                Core.Instance.CurrentBattle = m_Wars[ info.ButtonID - 1 ];
                m_Owner.SendMessage( "Valid war plan given... server war is starting up." );
            }
        }
    }
}