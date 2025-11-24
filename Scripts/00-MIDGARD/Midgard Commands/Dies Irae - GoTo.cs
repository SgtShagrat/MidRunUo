/***************************************************************************
 *                                    Goto.cs
 *                            		-----------
 *  begin                	: Aprile, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:	
 * 			Comando per andare iterativamente dai player.
 * 			Uso del comando: [GoTo
 * 
 *	Versionamento:
 * 	1.1		Modificato BuildList() ora dipende anche dall'owner.
 * 			Gi admin vedono i developers e possono andare da loro.
 *  		Chi ha piu' privs vede chi ha meno privs.
 * 			Chi e' online visible e' sempre addato alla lista.
 ***************************************************************************/

// #define DebugGoTo

using System;
using System.Collections.Generic;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Commands
{
    public class GoToCommand
    {
        #region registration
        public static void Initialize()
        {
            CommandSystem.Register( "GoTo", AccessLevel.Counselor, new CommandEventHandler( GoTo_OnCommand ) );
        }
        #endregion

        #region callback
        [Usage( "GoTo" )]
        [Description( "Open the goto gump to teleport staff member to other players." )]
        public static void GoTo_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( from != null )
            {
                if( e.Length == 0 )
                    from.SendGump( new GoToGump( from ) );
                else
                    from.SendMessage( "Command Use: [GoTo" );
            }
        }
        #endregion
    }

    public class GoToGump : Gump
    {
        #region campi
        private static readonly int m_Fields = 20;
        private static readonly int m_HueTit = 15;
        private readonly List<Mobile> m_Mobiles;
        private int m_Page;
        #endregion

        #region costruttori
        public GoToGump( Mobile owner )
            : this( owner, BuildList( owner ), 1 )
        {
        }

        public GoToGump( Mobile owner, List<Mobile> list, int page )
            : base( 50, 50 )
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            owner.CloseGump( typeof( GoToGump ) );

            m_Mobiles = list;

            Initialize( page );
        }
        #endregion

        #region metodi
        private static int GetHueFor( Mobile m )
        {
            switch( m.AccessLevel )
            {
                case AccessLevel.Owner:
                case AccessLevel.Developer:
                case AccessLevel.Administrator:
                    return 0x516;
                case AccessLevel.Seer:
                    return 0x144;
                case AccessLevel.GameMaster:
                    return 0x21;
                case AccessLevel.Counselor:
                    return 0x2;
                case AccessLevel.Player:
                default:
                    return 0x58;
            }
        }

        private class InternalComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new InternalComparer();

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                if( x.AccessLevel > y.AccessLevel )
                    return -1;
                else if( x.AccessLevel < y.AccessLevel )
                    return 1;
                else
                    return Insensitive.Compare( x.Name, y.Name );
            }
        }

        private static void AddBlackAlpha( int x, int y, int width, int height, Gump g )
        {
            if( g == null )
                return;

            g.AddImageTiled( x, y, width, height, 2624 );
            g.AddAlphaRegion( x, y, width, height );
        }

        private static List<Mobile> BuildList( Mobile owner )
        {
            List<Mobile> list = new List<Mobile>();
            List<NetState> states = NetState.Instances;

            foreach( NetState t in states )
            {
                Mobile m = t.Mobile;

                if( m == null || m.Deleted || owner == m )
                    continue;

                if( owner.AccessLevel > AccessLevel.Seer )
                    list.Add( m );
                else if( owner.AccessLevel >= m.AccessLevel )
                    list.Add( m );
                else if( ( (PlayerMobile)m ).VisibilityList.Contains( owner ) )
                    list.Add( m );
                else if( m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).OnlineVisible )
                    list.Add( m );
            }

            list.Sort( InternalComparer.Instance );

            return list;
        }

        private void Initialize( int page )
        {
            m_Page = page;

            AddPage( 0 );
            AddBackground( 0, 0, 261, 481, 83 );
            AddBlackAlpha( 10, 10, 241, 461, this );

            AddLabel( 13, 11, m_HueTit, "Select player to goto:" );

            if( m_Page > 1 )
                AddButton( ( 231 - 20 ), 13, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0 ); // Previous Page

            if( m_Page < Math.Ceiling( m_Mobiles.Count / (double)m_Fields ) )
                AddButton( 231, 13, 0x15E1, 0x15E5, 300, GumpButtonType.Reply, 0 ); // NextPage

            int indMax = ( m_Page * m_Fields ) - 1;
            int indMin = ( m_Page * m_Fields ) - m_Fields;
            int indTemp = 0;

            for( int i = 0; i < m_Mobiles.Count; ++i )
            {
                if( i >= indMin && i <= indMax )
                {
                    Mobile m = m_Mobiles[ i ];
                    if( m != null )
                    {
                        AddLabelCropped( 13, 33 + ( indTemp * 22 ), 150, 21, GetHueFor( m ), m.Name );
                        AddButton( 231, 33 + ( indTemp * 22 ) + 3, 0x15E1, 0x15E5, i + 1, GumpButtonType.Reply, 0 );
                        indTemp++;
                    }
                }
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == 0 ) // Close
                return;

            if( info.ButtonID == 200 ) // Previous Page
            {
                m_Page--;
                from.SendGump( new GoToGump( from, m_Mobiles, m_Page ) );
            }
            else if( info.ButtonID == 300 ) // NextPage
            {
                m_Page++;
                from.SendGump( new GoToGump( from, m_Mobiles, m_Page ) );
            }
            else
            {
                try
                {
                    Mobile m = m_Mobiles[ info.ButtonID - 1 ];

                    if( from.AccessLevel < m.AccessLevel )
                    {
                        from.SendMessage( "You cannot move yourself upon him." );
                        return;
                    }

                    if( m.Map != Map.Internal )
                    {
                        from.MoveToWorld( m.Location, m.Map );
                        from.SendMessage( "You've gone to {0}", m.Name );
                    }
                    else
                        from.SendMessage( "Player has logged-out." );

                    from.SendGump( new GoToGump( from, m_Mobiles, m_Page ) );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "Error in [GoTo: {0}", ex );
                }
            }
        }
        #endregion
    }
}