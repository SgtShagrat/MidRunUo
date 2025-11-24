/***************************************************************************
 *                               BaseCandidateApprovalGump.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
    public abstract class BaseCandidateApprovalGump : Gump
    {
        private readonly List<Mobile> m_List;

        private const int Fields = 9;
        private const int HueTit = 662;
        private const int DeltaBut = 2;
        private const int FieldsDist = 25;
        private const int HuePrim = 92;

        protected BaseCandidateApprovalGump( Mobile from, IEnumerable<Mobile> list )
            : base( 50, 50 )
        {
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_List = new List<Mobile>( list );

            Design();
        }

        public virtual void Design()
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

            AddLabelCropped( 14, 12, 255, 25, HueTit, "Choose a candidate" );

            for( int i = 0; i < m_List.Count; ++i )
            {
                if( ( i % Fields ) == 0 )
                {
                    if( i != 0 )
                        AddButton( 245, 297, 5601, 5605, 300, GumpButtonType.Page, ( i / Fields ) + 1 ); // Next page

                    AddPage( ( i / Fields ) + 1 );

                    if( i != 0 )
                        AddButton( 225, 297, 5603, 5607, 200, GumpButtonType.Page, ( i / Fields ) ); // Previous page
                }

                Mobile m = m_List[ i ];

                string name;
                if( ( name = m.Name ) != null && ( name = name.Trim() ).Length <= 0 )
                    name = "(empty)";

                AddLabelCropped( 50, 52 + ( ( i % 11 ) * FieldsDist ), 225, 20, HuePrim, name );
                AddButton( 30, 52 + DeltaBut + ( ( i % 11 ) * FieldsDist ), RedBtnNormal, RedBtnPressed, i + 1 + 100, GumpButtonType.Reply, 0 );
                AddButton( 15, 52 + DeltaBut + ( ( i % 11 ) * FieldsDist ), GreenBtnNormal, GreenBtnPressed, i + 1, GumpButtonType.Reply, 0 );
            }
        }

        private const int GreenBtnNormal = 0x2C88;
        private const int GreenBtnPressed = 0x2C8A;
        private const int RedBtnNormal = 0x2C92;
        private const int RedBtnPressed = 0x2C94;

        public virtual void ResponseHandler( Mobile from, Mobile candidate, bool reject )
        {
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            int index = info.ButtonID;
            if( index <= 0 )
                return;

            bool reject = false;
            if( index > 100 )
            {
                index -= 100;
                reject = true;
            }

            index--;

            if( index > -1 && index < m_List.Count )
            {
                Mobile candidate = m_List[ index ];
                ResponseHandler( sender.Mobile, candidate, reject );
            }
        }
    }
}