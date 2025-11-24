using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Commands
{
    public class ListXmlSpawners
    {
        public static void Initialize()
        {
            CommandSystem.Register( "ListXml", AccessLevel.Developer, new CommandEventHandler( ListXml_OnCommand ) );
            CommandSystem.Register( "FixTreasureXmlSpawner", AccessLevel.Developer, new CommandEventHandler( FixXml_OnCommand ) );

            FixXml( null );
        }

        private static string[] m_BadTypes = new string[]
                                             {
                                        "TreasureLevel1",
                                        "TreasureLevel2",
                                        "TreasureLevel3",
                                        "TreasureLevel4",
                                       };

        private static bool IsBadType( string type )
        {
            foreach( string badType in m_BadTypes )
            {
                if( type.Equals( badType, StringComparison.InvariantCultureIgnoreCase ) )
                    return true;
            }

            return false;
        }

        [Usage( "FixTreasureXmlSpawner" )]
        [Description( "Removes errors from xmlspawners in the world" )]
        public static void FixXml_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( e.Length == 0 )
                FixXml( e.Mobile );
        }

        private static void FixXml( Mobile from )
        {
            List<XmlSpawner> list = new List<XmlSpawner>();
            foreach( Item i in World.Items.Values )
            {
                if( i != null && !i.Deleted && i is XmlSpawner )
                    list.Add( (XmlSpawner)i );
            }

            list.Sort( InternalComparer.Instance );

            using( StreamWriter op = new StreamWriter( "Logs/FixXmls.log" ) )
            {
                op.WriteLine( "List generated on {0} in time {1}.", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                op.WriteLine( "Total xmlspawner processed {0}", list.Count );
                op.WriteLine( "" );

                for( int i = 0; i < list.Count; i++ )
                {
                    try
                    {
                        XmlSpawner spawner = list[ i ];

                        if( spawner != null && !spawner.Deleted && spawner.SpawnObjects != null )
                        {
                            for( int j = 0; j < spawner.SpawnObjects.Length; j++ )
                            {
                                if( IsBadType( spawner.SpawnObjects[ j ].TypeName ) )
                                {
                                    op.WriteLine( "{0}\t{1}", j + 1, spawner.SpawnObjects[ j ].TypeName );

                                    spawner.SpawnObjects[ j ].MaxCount = 0;
                                    spawner.DeleteSpawnObject( from, spawner.SpawnObjects[ j ].TypeName );
                                    spawner.SortSpawns();
                                }
                            }
                        }

                        if( spawner != null && !spawner.Deleted && spawner.SpawnObjects != null )
                        {
                            if( spawner.SpawnObjects.Length == 0 )
                            {
                                op.WriteLine( "Deleted\t{0} {1}", spawner.Location, spawner.Name ?? "" );
                                spawner.Delete();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        [Usage( "ListXml" )]
        [Description( "List all xmlspawner info on midgard shard" )]
        public static void ListXml_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( e.Length == 0 )
            {
                List<XmlSpawner> list = new List<XmlSpawner>();
                foreach( Item i in World.Items.Values )
                {
                    if( i != null && !i.Deleted && i is XmlSpawner )
                        list.Add( (XmlSpawner)i );
                }

                list.Sort( InternalComparer.Instance );

                using( StreamWriter op = new StreamWriter( "Logs/MidgardXmls.log" ) )
                {
                    op.WriteLine( "List generated on {0} in time {1}.", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                    op.WriteLine( "Total xmlspawner processed {0}", list.Count );
                    op.WriteLine( "" );

                    for( int i = 0; i < list.Count; i++ )
                    {
                        try
                        {
                            XmlSpawner spawner = list[ i ];

                            if( spawner != null && !spawner.Deleted )
                            {

                                op.WriteLine( "Name: {0}", spawner.Name ?? "null" );
                                op.WriteLine( "Location: {0}", spawner.Location );
                                op.WriteLine( "MaxCount: {0}", spawner.MaxCount );

                                if( spawner.SpawnObjects != null )
                                {
                                    op.WriteLine( "Number of valid entries: {0}", spawner.SpawnObjects.Length );
                                    for( int j = 0; j < spawner.SpawnObjects.Length; j++ )
                                    {
                                        SpawnObject spawnObject = spawner.SpawnObjects[ j ];
                                        if( spawnObject != null )
                                            op.WriteLine( "{0}\t{1}", j + 1, spawnObject.TypeName );
                                    }
                                }
                                else
                                {
                                    op.WriteLine( "No valid entry." );
                                }

                                op.WriteLine( "" );
                            }
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }
                    }
                }

                from.SendMessage( "Xml spawner log has been generated. See the file : <runuo root>/Logs/MidgardXmls.log" );
            }
        }

        private class InternalComparer : IComparer<XmlSpawner>
        {
            public static readonly IComparer<XmlSpawner> Instance = new InternalComparer();

            private InternalComparer()
            {
            }

            public int Compare( XmlSpawner x, XmlSpawner y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
        }
    }
}