/***************************************************************************
 *                                      ListContainer.cs
 *                            		------------------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server;
using Server.Commands;
using Server.Items;
using Server.Targeting;

namespace Midgard.Commands
{
    public class ListContainer
    {
        public static void Initialize()
        {
            CommandSystem.Register( "ListContainer", AccessLevel.Counselor, new CommandEventHandler( ListContainer_OnCommand ) );
        }

        [Usage( "ListContainer" )]
        [Description( "List all items in a given container" )]
        public static void ListContainer_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 1 )
            {
                from.SendMessage( "Choose the container." );
                from.Target = new InternalTarget( e.ArgString );
            }
            else
            {
                from.SendMessage( "Command use: ListContainer" );
            }
        }

        private class InternalTarget : Target
        {
            private string m_Args;

            private bool m_NeedType;
            private bool m_NeedSerial;
            private bool m_NeedItemID;

            public InternalTarget( string args )
                : base( 12, false, TargetFlags.None )
            {
                m_Args = args;

                m_NeedType = m_Args.Contains( "t" );
                m_NeedSerial = m_Args.Contains( "s" );
                m_NeedItemID = m_Args.Contains( "i" );
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( !( targeted is Container ) )
                    return;

                List<Item> rawItems = new List<Item>( ( (Container)targeted ).Items );

                try
                {
                    List<Item> toLog = new List<Item>();
                    List<Container> packs = new List<Container>();

                    for( int i = 0; i < rawItems.Count; ++i )
                    {
                        object obj = rawItems[ i ];

                        if( obj is Container )
                        {
                            Container cont = (Container)obj;
                            packs.Add( cont );
                        }
                        else if( obj is Item )
                        {
                            Item item = (Item)obj;
                            if( !toLog.Contains( item ) )
                                toLog.Add( item );
                        }
                    }

                    foreach( Container c in packs )
                    {
                        foreach( Item i in c.Items )
                        {
                            if( !toLog.Contains( i ) )
                                toLog.Add( i );
                        }
                    }

                    toLog.Sort();

                    try
                    {
                        using( StreamWriter tw = new StreamWriter( ( (Container)targeted ).Serial.Value.ToString( "X4" ) + ".txt", true ) )
                        {
                            foreach( Item i in toLog )
                            {
                                StringBuilder sb = new StringBuilder();

                                if( m_NeedType )
                                    sb.AppendFormat( "Type: {0} ", i.GetType().Name );
                                if( m_NeedItemID )
                                    sb.AppendFormat( "ID: 0x{0:X4} ", i.ItemID );
                                if( m_NeedSerial )
                                    sb.AppendFormat( "Serial: 0x{0:X4} ", i.Serial.Value );

                                tw.WriteLine( sb.ToString() );
                            }
                        }
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }
    }
}