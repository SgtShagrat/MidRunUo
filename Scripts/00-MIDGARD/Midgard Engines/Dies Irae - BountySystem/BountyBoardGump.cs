/***************************************************************************
 *                               BountyBoardGump.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.BountySystem
{
    public class BountyBoardGump : Gump
    {
        private class InternalSorter : IComparer<BountyBoardEntry>
        {
            private SortType m_SortType;

            public InternalSorter( SortType sortType )
            {
                m_SortType = sortType;
            }

            public int Compare( BountyBoardEntry x, BountyBoardEntry y )
            {
                if( x == null && y == null )
                    return 0;
                else if( x == null )
                    return -1;
                else if( y == null )
                    return 1;

                int result = 0;

                switch( m_SortType )
                {
                    case SortType.Price:
                        {
                            result = y.Price.CompareTo( x.Price );
                            // result = a.Price.CompareTo( b.Price );
                            break;
                        }
                    case SortType.Owner:
                        {
                            result = x.Owner.Name.CompareTo( y.Owner.Name );
                            break;
                        }
                    case SortType.Wanted:
                        {
                            result = x.Wanted.Name.CompareTo( y.Wanted.Name );
                            break;
                        }
                    case SortType.Expires:
                        {
                            result = x.ExpireTime.CompareTo( y.ExpireTime );
                            break;
                        }
                }

                return result;
            }
        }

        public enum SortType
        {
            None,
            Price,
            Owner,
            Wanted,
            Expires
        }

        private Mobile m_From;
        private BountyBoard m_Board;
        private List<BountyBoardEntry> m_List;
        private SortType m_SortType;

        private int m_Page;

        private const int HtmlHue = 0xFFFFFF;

        public int GetIndexForPage( int page )
        {
            int index = 0;

            while( page-- > 0 )
                index += 10;

            return index;
        }

        public BountyBoardGump( Mobile from, BountyBoard board )
            : this( from, board, 0, null, SortType.None )
        {
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            int index = info.ButtonID;

            switch( index )
            {
                case 0: // EXIT
                    {
                        break;
                    }
                case 1: // sort by wanted
                    {
                        m_From.SendGump( new BountyBoardGump( m_From, m_Board, 0, null, SortType.Wanted ) );
                        break;
                    }
                case 2: // sort by owner
                    {
                        m_From.SendGump( new BountyBoardGump( m_From, m_Board, 0, null, SortType.Owner ) );
                        break;
                    }
                case 3: // sort by price
                    {
                        m_From.SendGump( new BountyBoardGump( m_From, m_Board, 0, null, SortType.Price ) );
                        break;
                    }
                case 4: // Previous page
                    {
                        if( m_Page > 0 )
                            m_From.SendGump( new BountyBoardGump( m_From, m_Board, m_Page - 1, m_List, m_SortType ) );

                        return;
                    }
                case 5: // Next page
                    {
                        if( GetIndexForPage( m_Page + 1 ) < m_List.Count )
                            m_From.SendGump( new BountyBoardGump( m_From, m_Board, m_Page + 1, m_List, m_SortType ) );

                        break;
                    }
                default:
                    {
                        index -= 6;

                        int type = index % 3;
                        index /= 3;

                        if( index < 0 || index >= m_List.Count )
                            break;

                        BountyBoardEntry obj = m_List[ index ];

                        if( !Core.Entries.Contains( obj ) )
                        {
                            m_From.SendMessage( "The bounty selected is not available." );
                            m_From.SendGump( new BountyBoardGump( m_From, m_Board, 0, null, m_SortType ) );
                            return;
                        }

                        BountyBoardEntry entry = obj;

                        if( type == 0 ) // remove
                        {
                            Core.RemoveEntry( obj, false );
                            m_From.SendMessage( "The bounty has been removed." );

                            if( Core.Entries.Count > 0 )
                                m_From.SendGump( new BountyBoardGump( m_From, m_Board, 0, null, m_SortType ) );
                            else
                                m_From.SendMessage( "The bounty board is empty." );
                        }
                        else if( type == 1 ) // edit
                        {
                            m_From.SendGump( new EditBountyGump( m_From, obj ) );
                        }
                        else // request
                        {
                            if( !entry.Requested.Contains( m_From ) )
                            {
                                entry.Requested.Add( m_From );
                                m_From.SendMessage( "You have requested this bounty." );

                                BountySystemLog.WriteInfo( entry, BountySystemLog.LogType.Requested );
                                BountySystemLog.WriteInfo( string.Format( "Entry requested by {0}, (account {1}, serial{2} ).", m_From.Name, m_From.Account.Username, m_From.Serial ) );

                                m_From.SendGump( new BountyBoardGump( m_From, m_Board, 0, null, m_SortType ) );

                                string msg = String.Format( "{0} has requested permission to seek the bounty on {1}, accept him by using a bounty board.",
                                m_From.Name, entry.Wanted.Name );

                                if( NetState.Instances.Contains( entry.Owner.NetState ) )
                                    entry.Owner.SendMessage( msg );
                                else
                                {
                                    ( (Midgard2PlayerMobile)entry.Owner ).ShowBountyUpdate = true;
                                    ( (Midgard2PlayerMobile)entry.Owner ).BountyUpdateList.Add( msg );
                                }
                            }
                            else
                                m_From.SendMessage( "You have already requested this bounty." );
                        }

                        break;
                    }
            }
        }

        public string Color( string text, int color )
        {
            return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
        }

        public BountyBoardGump( Mobile from, BountyBoard board, int page, List<BountyBoardEntry> list, SortType sortType )
            : base( 0, 24 )
        {
            from.CloseGump( typeof( BountyBoardGump ) );
            from.CloseGump( typeof( EditBountyGump ) );

            m_From = from;
            m_Board = board;
            m_Page = page;
            m_SortType = sortType;

            if( list == null )
            {
                list = new List<BountyBoardEntry>();

                for( int i = Core.Entries.Count - 1; Core.Entries != null && i >= 0; --i )
                {
                    if( i >= Core.Entries.Count )
                        continue;

                    BountyBoardEntry entry = Core.Entries[ i ];

                    if( entry.Expired )
                        Core.RemoveEntry( entry, false );
                    else
                        list.Add( entry );
                }
            }

            if( m_SortType != SortType.None )
                list.Sort( new InternalSorter( m_SortType ) );

            m_List = list;

            int index = GetIndexForPage( page );
            int count = 10;

            int tableIndex = 0;

            AddPage( 0 );
            AddBackground( 10, 10, 750, 439, 5054 );
            AddImageTiled( 18, 20, 733, 420, 2624 );

            AddImageTiled( 18, 64, 46, 352, 1416 ); // remove
            AddImageTiled( 67, 64, 170, 352, 200 ); // wanted
            AddImageTiled( 240, 64, 170, 352, 1416 ); // owner
            AddImageTiled( 413, 64, 65, 352, 200 ); // price
            AddImageTiled( 481, 64, 30, 352, 1416 ); // edit
            AddImageTiled( 514, 64, 60, 352, 200 ); // request
            AddImageTiled( 577, 64, 170, 352, 200 ); // Expires

            for( int i = index; i < ( index + count ) && i >= 0 && i < list.Count; ++i )
            {
                //object obj = list[ i ];

                AddImageTiled( 24, 94 + ( tableIndex * 32 ), 723, 2, 2624 );

                ++tableIndex;
            }

            AddAlphaRegion( 18, 20, 733, 420 );
            AddImage( 5, 5, 10460 );
            AddImage( 735, 5, 10460 );
            AddImage( 5, 424, 10460 );
            AddImage( 735, 424, 10460 );

            AddHtmlLocalized( 22, 64, 200, 32, 1011403, HtmlHue, false, false );          // Remove	
            AddButton( 120, 67, 2117, 2118, 1, GumpButtonType.Reply, 0 );                   // wanted sort
            AddHtml( 70, 64, 200, 32, Color( "Wanted", HtmlHue ), false, false );
            AddButton( 283, 67, 2117, 2118, 2, GumpButtonType.Reply, 0 );                   // owner sort
            AddHtml( 243, 64, 200, 32, Color( "Owner", HtmlHue ), false, false );
            AddButton( 446, 67, 2117, 2118, 3, GumpButtonType.Reply, 0 );                   // price sort
            AddHtmlLocalized( 416, 64, 200, 32, 1062218, HtmlHue, false, false );         // Price
            AddHtmlLocalized( 484, 64, 200, 32, 3005101, HtmlHue, false, false );         // Edit			
            AddHtml( 517, 64, 200, 32, Color( "Request", HtmlHue ), false, false );
            AddHtml( 580, 64, 200, 32, Color( "Expires", HtmlHue ), false, false );
            // AddButton(446, 67, 2117, 2118, 4, GumpButtonType.Reply, 0);                  // expire sort
            AddHtml( 350, 32, 200, 32, Color( "BOUNTIES", HtmlHue ), false, false );
            AddHtmlLocalized( 710, 416, 120, 20, 1011441, HtmlHue, false, false );        // EXIT
            AddButton( 675, 416, 4017, 4018, 0, GumpButtonType.Reply, 0 );                  //exit btn	

            if( page > 0 )
            {
                AddHtmlLocalized( 110, 416, 150, 20, 1011067, HtmlHue, false, false ); // Previous page
                AddButton( 75, 416, 4014, 4016, 4, GumpButtonType.Reply, 0 );
            }

            if( GetIndexForPage( page + 1 ) < list.Count )
            {
                AddHtmlLocalized( 410, 416, 150, 20, 1011066, HtmlHue, false, false ); // Next page
                AddButton( 375, 416, 4005, 4007, 5, GumpButtonType.Reply, 0 );
            }

            tableIndex = 0;

            for( int i = index; i < ( index + count ) && i >= 0 && i < list.Count; ++i )
            {
                object obj = list[ i ];
                int y = 96 + ( tableIndex++ * 32 );

                BountyBoardEntry entry = (BountyBoardEntry)obj;
                if( m_From == entry.Owner || m_From.AccessLevel > AccessLevel.Player )
                {
                    AddButton( 31, y + 2, 5602, 5606, 6 + ( i * 3 ), GumpButtonType.Reply, 0 ); // remove btn
                    AddButton( 488, y + 2, 2117, 2118, 7 + ( i * 3 ), GumpButtonType.Reply, 0 ); //edit btn
                }
                else if( entry.Accepted.Contains( m_From ) )
                {
                    AddHtml( 517, y, 200, 32, Color( "Accepted", HtmlHue ), false, false );
                }
                else if( m_From != entry.Wanted && m_From.Account != entry.Wanted.Account )
                {
                    AddButton( 536, y + 2, 2117, 2118, 8 + ( i * 3 ), GumpButtonType.Reply, 0 ); //request btn
                }
                /*
                            else if( ( m_From != entry.Wanted && !entry.Accepted.Contains( m_From ) && m_From.Account != entry.Wanted.Account ) )
                            {
                                AddButton(559, y+2, 2117, 2118, 8 + (i * 3), GumpButtonType.Reply, 0); //request btn
                            }

                            if( entry.Accepted.Contains( m_From ))
                            {
                                AddHtml( 550, y, 200, 32, Color( "Accepted", HtmlColor ), false, false );
                            }
                            */
                AddHtml( 70, y, 200, 32, Color( entry.Wanted.Name, HtmlHue ), false, false );
                AddHtml( 243, y, 200, 32, Color( entry.Owner.Name, HtmlHue ), false, false );
                AddHtml( 416, y, 200, 32, Color( entry.Price.ToString(), HtmlHue ), false, false );
                AddHtml( 580, y, 200, 32, Color( entry.ExpireTime.ToString(), HtmlHue ), false, false );
            }
        }
    }
}