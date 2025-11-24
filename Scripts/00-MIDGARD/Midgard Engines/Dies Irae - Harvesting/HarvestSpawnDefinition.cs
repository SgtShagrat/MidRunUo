using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Server.Commands;
using Server.Engines.Harvest;
using Server;

namespace Midgard.Engines.Harvesting
{
    public class HarvestSpawnDefinition
    {
        private static readonly string XmlFilePath = Path.Combine( Core.BaseDirectory, Path.Combine( "Data", "harvestSpawnDefinitions.xml" ) );

        public HarvestSpawnDefinition( Type type, double[] value )
        {
            SpawnType = type;
            MinValue = value[ 0 ];
            MaxValue = value[ 1 ];
        }

        private static readonly Dictionary<Type, double[]> m_RespawnTable = new Dictionary<Type, double[]>();

        public Type SpawnType { get; private set; }

        public double MinValue { get; private set; }

        public double MaxValue { get; private set; }

        /// <summary>
        /// Get delay of respawn for a given targeted object and a definition
        /// </summary>
        public static void GetRespawnDelay( HarvestDefinition def, object targeted, out double minDelay, out double maxDelay )
        {
            Type t = targeted.GetType();

            double[] values;
            m_RespawnTable.TryGetValue( t, out values );

            if( def != null )
            {
                if( values != null )
                {
                    minDelay = values[ 0 ];
                    maxDelay = values[ 1 ];
                }
                else
                {
                    minDelay = def.MinRespawn.TotalMinutes;
                    maxDelay = def.MaxRespawn.TotalMinutes;
                }
            }
            else
            {
                minDelay = 0.0;
                maxDelay = 0.0;
            }

            if( def != null && Core.Debug )
                Utility.Log( "HarvestSpawnDefinitionGetRespawnDelay.log", "Debug GetRespawnDelay: {0} - {1} - {2} - {3}", def.Skill.ToString(), t.Name, minDelay.ToString( "F2" ), maxDelay.ToString( "F2" ) );
        }

        private static void RegisterSpawnDefinition( HarvestSpawnDefinition definition )
        {
            if( !m_RespawnTable.ContainsKey( definition.SpawnType ) )
                m_RespawnTable.Add( definition.SpawnType, new double[] { definition.MinValue, definition.MaxValue } );
        }

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "GenHarvestDefinitions", AccessLevel.Developer, GenHarvestDefinitions_OnCommand );
        }

        [Usage( "[GenHarvestDefinitions" )]
        [Description( "Generates xml data for harvest system." )]
        private static void GenHarvestDefinitions_OnCommand( CommandEventArgs e )
        {
            GenerateXml();
        }

        #region old initialization
        /*
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( AppleTree1 ), new double[] { 120.0, 240.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( AppleTree2 ), new double[] { 120.0, 240.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( BananaTree1 ), new double[] { 120.0, 240.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CedarTree1 ), new double[] { 60.0, 120.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CedarTree2 ), new double[] { 120.0, 240.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CherryTree1 ), new double[] { 120.0, 240.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CherryTree2 ), new double[] { 120.0, 240.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CoconutPalm1 ), new double[] { 240.0, 360.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CommonTree1 ), new double[] { 60.0, 120.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CommonTree2 ), new double[] { 60.0, 120.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CommonTree3 ), new double[] { 60.0, 120.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CypressTree1 ), new double[] { 60.0, 120.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CypressTree2 ), new double[] { 60.0, 120.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CypressTree3 ), new double[] { 60.0, 120.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( CypressTree4 ), new double[] { 60.0, 120.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( DatePalm1 ), new double[] { 120.0, 240.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( MapleTree1 ), new double[] { 120.0, 240.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( MapleTree2 ), new double[] { 120.0, 240.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( OakTree1 ), new double[] { 60.0, 120.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( OakTree2 ), new double[] { 60.0, 120.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( OhiiTree1 ), new double[] { 60.0, 120.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( PeachTree1 ), new double[] { 120.0, 240.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( PeachTree2 ), new double[] { 120.0, 240.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( PearTree1 ), new double[] { 120.0, 240.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( PearTree2 ), new double[] { 120.0, 240.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( PlumTree1 ), new double[] { 120.0, 240.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( PlumTree2 ), new double[] { 120.0, 240.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( WalnutTree1 ), new double[] { 60.0, 120.0 } ) );
            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( WalnutTree2 ), new double[] { 60.0, 120.0 } ) );

            RegisterSpawnDefinition( new HarvestSpawnDefinition( typeof( WillowTree1 ), new double[] { 60.0, 120.0 } ) );
            */
        #endregion

        private static void GenerateXml()
        {
            const string path = "harvestSpawnDefinitions.xml";

            XmlDocument doc = new XmlDocument();

            using( XmlTextWriter textWriter = new XmlTextWriter( path, null ) )
            {
                textWriter.Formatting = Formatting.Indented;
                textWriter.WriteStartElement( "definitions" );
                textWriter.WriteEndElement();
                textWriter.Flush();
            }

            using( FileStream fileStream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
                doc.Load( fileStream );

            foreach( KeyValuePair<Type, double[]> kvp in m_RespawnTable )
            {
                try
                {
                    Type key = kvp.Key;
                    double[] value = kvp.Value;

                    XmlElement element = doc.CreateElement( "definition" );

                    element.SetAttribute( "type", key.Name );
                    element.SetAttribute( "minValue", value[ 0 ].ToString() );
                    element.SetAttribute( "maxValue", value[ 1 ].ToString() );

                    doc.DocumentElement.InsertAfter( element, doc.DocumentElement.LastChild );
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }

            using( FileStream outStream = new FileStream( path, FileMode.Truncate, FileAccess.Write, FileShare.Write ) )
                doc.Save( outStream );

            Load();
        }

        internal static void Load()
        {
            var pkg = Packager.Core.Singleton[ typeof( Config ) ];

            if( !File.Exists( XmlFilePath ) )
            {
                Console.WriteLine( "Error: " + XmlFilePath + " does not exist" );
                return;
            }

            if( Config.Debug )
                pkg.LogInfo( "Regional Harvest Definitions: Loading..." );

            XmlDocument doc = new XmlDocument();
            doc.Load( XmlFilePath );

            XmlElement root = doc[ "definitions" ];

            if( root == null )
            {
                pkg.LogWarningLine( "Could not find root element 'definitions' in harvestSpawnDefinitions.xml" );
                pkg.LogInfo( "Loading..." );
            }
            else
            {
                foreach( XmlElement info in root.SelectNodes( "definition" ) )
                {
                    try
                    {
                        HarvestSpawnDefinition def = Parse( info );
                        if( def != null )
                            RegisterSpawnDefinition( def );
                    }
                    catch( Exception e )
                    {
                        Console.WriteLine( e );
                    }
                }
            }

            if( Config.Debug )
            {
                pkg.LogInfoLine( "done" );
                pkg.LogInfoLine( "Harvest System has {0} definitions.", m_RespawnTable.Count );
            }
        }

        public static HarvestSpawnDefinition Parse( XmlElement node )
        {
            Type t = null;
            string name = "";
            double minValue = 0.0;
            double maxValue = 0.0;

            if( Region.ReadString( node, "type", ref name ) )
                if( !string.IsNullOrEmpty( name ) )
                    t = ScriptCompiler.FindTypeByName( name );

            Region.ReadDouble( node, "minValue", ref minValue, false );
            Region.ReadDouble( node, "maxValue", ref maxValue, false );

            if( t != null )
                return new HarvestSpawnDefinition( t, new double[] { minValue, maxValue } );
            else
            {
                Console.WriteLine( "Error while parsing a HarvestSpawnDefinition." );
                return null;
            }
        }
    }
}