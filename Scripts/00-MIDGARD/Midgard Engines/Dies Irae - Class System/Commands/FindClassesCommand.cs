/***************************************************************************
 *                                    FindClassesCommand.cs
 *                            		-----------------------
 *  begin                	: Marzo, 2013
 *  version					: 2.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.Classes
{
    public class FindClassesCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register( "FindClasses", AccessLevel.Seer, new CommandEventHandler( FindClasses_OnCommand ) );
        }

        #region IComparers
        public enum ComparerType { Name, Account, Class };

        private class NameComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new NameComparer();

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
        }

        private class AccountComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new AccountComparer();

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Account.Username, y.Account.Username );
            }
        }

        private class ClassComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new ClassComparer();

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                ClassPlayerState cps1 = ClassPlayerState.Find( x );
                ClassPlayerState cps2 = ClassPlayerState.Find( y );

                return Insensitive.Compare( cps1.ClassSystem.Definition.ClassName, cps2.ClassSystem.Definition.ClassName );
            }
        }
        #endregion

        [Usage( "FindClasses {name | account | class} {<list> <online>}" )]
        [Description( "List all classes pgs. If list argument is present a log is built. If online argument is present only active netstates are processed." )]
        public static void FindClasses_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( e.Length > 0 )
            {
                string args0 = e.GetString( 0 );

                ComparerType cryteria = ComparerType.Class;
                if( Enum.IsDefined( typeof( ComparerType ), args0 ) )
                    cryteria = (ComparerType)Enum.Parse( typeof( ComparerType ), args0 );

                bool online = false;
                bool list = false;

                foreach( string t in e.Arguments )
                {
                    if( Utility.InsensitiveCompare( t, "list" ) == 0 )
                        list = true;
                    if( Utility.InsensitiveCompare( t, "online" ) == 0 )
                        online = true;
                }

                if( list )
                {
                    ListClassed();
                    from.SendMessage( "Class table has been generated. See the file : <runuo root>/Logs/MidgardClassedPG.log" );
                }

                from.SendMessage( "Criterya: {0} - Arguments: list {1} - online {2}", cryteria.ToString(), list, online );
                from.SendGump( new FindClassesGump( from, cryteria, online ) );
            }
            else
                from.SendMessage( "Command Use: FindClasses {name | account | class} {<list> <online>}" );
        }

        private static List<Mobile> BuildList( bool online, ComparerType cryteria )
        {
            List<Mobile> list = new List<Mobile>();

            foreach( Account acct in Accounts.GetAccounts() )
            {
                for( int i = 0; i < acct.Count; i++ )
                {
                    Mobile m = acct[ i ];
                    if( m == null || m.AccessLevel > AccessLevel.Player )
                        continue;

                    if( ClassPlayerState.Find( m ) != null )
                    {
                        if( ( online && m.NetState != null ) || !online )
                            list.Add( m );
                    }
                }
            }

            switch( cryteria )
            {
                case ComparerType.Name: list.Sort( NameComparer.Instance ); break;
                case ComparerType.Account: list.Sort( AccountComparer.Instance ); break;
                case ComparerType.Class: list.Sort( ClassComparer.Instance ); break;
                default: list.Sort( ClassComparer.Instance ); break;
            }

            return list;
        }

        private static void ListClassed()
        {
            using( StreamWriter op = new StreamWriter( "Logs/MidgardClassedPG.log" ) )
            {
                try
                {
                    List<Mobile> list = BuildList( false, ComparerType.Account );

                    op.WriteLine( "## Class List generated on {0} ##", DateTime.Now );
                    op.WriteLine( "#################################################" );
                    op.WriteLine();
                    op.WriteLine();

                    op.WriteLine( "# Classed PGs:" );
                    op.WriteLine();

                    foreach( Mobile t in list )
                    {
                        ClassPlayerState cps = ClassPlayerState.Find( t );
                        if( cps != null )
                            op.WriteLine( "{0} - {1} - {2}", t.Name, t.Account.Username, cps.ClassSystem.Definition.ClassName );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        public class FindClassesGump : Gump
        {
            private static readonly int m_Fields = 20;
            private static readonly int m_HueTit = 15;

            private readonly List<Mobile> m_Mobiles;
            private int m_Page;
            private readonly ComparerType m_Cryteria;
            private readonly bool m_Online;

            public FindClassesGump( Mobile owner )
                : this( owner, BuildList( false, ComparerType.Class ), 1, ComparerType.Class, false )
            {
            }

            public FindClassesGump( Mobile owner, bool online )
                : this( owner, BuildList( online, ComparerType.Class ), 1, ComparerType.Class, online )
            {
            }

            public FindClassesGump( Mobile owner, ComparerType cryteria, bool online )
                : this( owner, BuildList( online, cryteria ), 1, cryteria, online )
            {
            }

            public FindClassesGump( Mobile owner, List<Mobile> list, int page, ComparerType cryteria, bool online )
                : base( 50, 50 )
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                owner.CloseGump( typeof( FindClassesGump ) );

                m_Mobiles = list;
                m_Cryteria = cryteria;
                m_Online = online;

                Initialize( page );
            }

            private void AddBlackAlpha( int x, int y, int width, int height )
            {
                AddImageTiled( x, y, width, height, 2624 );
                AddAlphaRegion( x, y, width, height );
            }

            private void Initialize( int page )
            {
                m_Page = page;

                AddPage( 0 );
                AddBackground( 0, 0, 405, 481, 83 );
                AddBlackAlpha( 10, 10, 385, 461 );

                AddLabel( 13, 11, m_HueTit, "Class list:" );

                if( m_Page > 1 )
                    AddButton( ( 381 - 20 ), 13, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0 ); 	// Previous Page

                if( m_Page < Math.Ceiling( m_Mobiles.Count / (double)m_Fields ) )
                    AddButton( 381, 13, 0x15E1, 0x15E5, 300, GumpButtonType.Reply, 0 ); 		// NextPage

                int IndMax = ( m_Page * m_Fields ) - 1;
                int IndMin = ( m_Page * m_Fields ) - m_Fields;
                int IndTemp = 0;

                for( int i = 0; i < m_Mobiles.Count; ++i )
                {
                    if( i >= IndMin && i <= IndMax )
                    {
                        Mobile m = m_Mobiles[ i ];
                        ClassPlayerState cps = ClassPlayerState.Find( m );

                        if( m != null && cps != null )
                        {
                            AddLabelCropped( 13, 33 + ( IndTemp * 22 ), 150, 22, 0x58, m.Name );
                            AddLabelCropped( 163, 33 + ( IndTemp * 22 ), 150, 22, 0x516, m.Account.Username );
                            AddLabelCropped( 263, 33 + ( IndTemp * 22 ), 150, 22, 0x144, cps.ClassSystem.Definition.ClassName );

                            // Goto if online
                            if( m.NetState != null )
                                AddButton( 363, 33 + ( IndTemp * 22 ) + 3, 0x15E1, 0x15E5, 1000 + i + 1, GumpButtonType.Reply, 0 );

                            // Remove class
                            AddButton( 383, 33 + ( IndTemp * 22 ) + 3, 0x15E1, 0x15E5, 2000 + i + 1, GumpButtonType.Reply, 0 );

                            IndTemp++;
                        }
                    }
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == 0 )							// Close
                    return;

                if( info.ButtonID == 200 ) 							// Previous Page
                {
                    m_Page--;
                    from.SendGump( new FindClassesGump( from, m_Mobiles, m_Page, m_Cryteria, m_Online ) );
                }
                else if( info.ButtonID == 300 ) 					// NextPage
                {
                    m_Page++;
                    from.SendGump( new FindClassesGump( from, m_Mobiles, m_Page, m_Cryteria, m_Online ) );
                }
                else if( info.ButtonID > 1000 && info.ButtonID < 2000 )
                {
                    try
                    {
                        Mobile m = m_Mobiles[ info.ButtonID - 1000 - 1 ];

                        if( m.Map != Map.Internal )
                        {
                            from.MoveToWorld( m.Location, m.Map );
                            from.SendMessage( "You've gone to {0}", m.Name );
                        }
                        else
                            from.SendMessage( "Player has logged-out." );

                        from.SendGump( new FindClassesGump( from, m_Mobiles, m_Page, m_Cryteria, m_Online ) );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( "Error in [FindClassesGump: {0}", ex );
                    }
                }
                else if( info.ButtonID > 2000 )
                {
                    try
                    {
                        ClassSystemCommands.DoPlayerReset( m_Mobiles[ info.ButtonID - 2000 - 1 ] );
                        from.SendGump( new FindClassesGump( from, m_Cryteria, m_Online ) );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( "Error in [FindClassesGump: {0}", ex );
                    }
                }
            }
        }
    }
}
