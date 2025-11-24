using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Server;
using Server.Commands;
using Server.Engines.Harvest;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.Harvesting
{
    public class RegionalHarvestDefinition
    {
        public static readonly int MaxZ = sbyte.MaxValue + 1;
        public static readonly int MinZ = sbyte.MinValue;

        private static RegionalHarvestDefinition m_DefaultDefinition;

        private readonly Map m_Map;

        public RegionalHarvestDefinition( int[] chanches, string name, Rectangle3D[] area, Map map, bool isCoilRegion )
        {
            Chanches = chanches;
            Name = name;
            Area = area;
            m_Map = map;
            IsCoilRegion = isCoilRegion;
        }

        public static RegionalHarvestDefinition[] List { get; private set; }

        public int GetChanchesLength
        {
            get { return Chanches.Length; }
        }

        public string Name { get; private set; }

        public int[] Chanches { get; private set; }

        public Rectangle3D[] Area { get; private set; }

        public bool IsCoilRegion { get; private set; }

        public static void RegisterCommands()
        {
            CommandSystem.Register( "HarvestInfo", AccessLevel.Administrator, new CommandEventHandler( HarvestInfo_OnCommand ) );
        }

        [Usage( "HarvestInfo" )]
        [Description( "Get harvest info for a given point" )]
        private static void HarvestInfo_OnCommand( CommandEventArgs e )
        {
            RegionalHarvestDefinition def = GetRegHarDefByPoint( e.Mobile.Map, e.Mobile.Location );

            e.Mobile.SendMessage( "You are in region: {0}", def.Name );
            e.Mobile.SendGump( new InternalGump( e.Mobile ) );
        }

        public bool Contains( Map map, Point3D p )
        {
            if( map == null || map != m_Map )
                return false;

            foreach (Rectangle3D rect in Area)
            {
                if( rect.Contains( p ) )
                    return true;
            }

            return false;
        }

        public static RegionalHarvestDefinition GetRegHarDefByPoint( Map map, Point3D point )
        {
            foreach( RegionalHarvestDefinition definition in List )
            {
                if( definition.Name == "default" )
                    continue;

                if( definition.Contains( map, point ) )
                    return definition;
            }

            return m_DefaultDefinition;
        }

        public int GetChanceByIndex( int index )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "---> " + index );

            if( index < 0 || index > Chanches.Length )
            {
                Config.Pkg.LogWarningLine( "Warning: index for GetChanceByIndex is out of range of m_Chanches." );
                return 0;
            }

            return Chanches[ index ];
        }

        public static RegionalHarvestDefinition Parse( XmlElement node )
        {
            int[] chanches = new int[ 24 ];
            string name = "";
            Map map = Map.Felucca;

            Region.ReadMap( node, "map", ref map, true );
            Region.ReadString( node, "name", ref name, true );

            bool ilCoilRegion = false;
            Region.ReadBoolean( node, "coilRegion", ref ilCoilRegion, false );

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "{0} {1}", name, ilCoilRegion );

            List<Rectangle3D> list = new List<Rectangle3D>();
            foreach( XmlElement xmlRect in node.SelectNodes( "rect" ) )
            {
                Rectangle3D rect = new Rectangle3D();
                if( Region.ReadRectangle3D( xmlRect, MinZ, MaxZ, ref rect ) )
                    list.Add( rect );
            }

            if( list.Count == 0 )
            {
                Config.Pkg.LogWarningLine( "Regional harvest error: definition without extension." );
                return null;
            }

            Rectangle3D[] areaArray = list.ToArray();

            foreach( XmlElement ore in node.SelectNodes( "ore" ) )
            {
                CraftResource oRes = CraftResource.None;
                
                if( Region.ReadEnum( ore, "type", ref oRes ) )
                {
                    int index = 0;//CraftResources.GetIndex( oRes );//non va bene veins[ i ].PrimaryResource.Types[ 0 ].Name
			//l'index di craftresource NON è uguale all'index delle VEIN!!! fix temporaneo:
			switch( oRes )
			{
				case CraftResource.Iron:{
					index = 0;
					break;}
				case CraftResource.OldDullCopper:{
					index = 1;
					break;}
				case CraftResource.OldShadowIron:{
					index = 2;
					break;}
				case CraftResource.OldBronze:{
					index = 3;
					break;}
				case CraftResource.OldCopper:{
					index = 4;
					break;}
				case CraftResource.OldAgapite:{
					index = 5;
					break;}
				case CraftResource.OldGraphite:{
					index = 6;
					break;}
				case CraftResource.OldVerite:{
					index = 7;
					break;}
				case CraftResource.OldValorite:{
					index = 8;
					break;}
				case CraftResource.OldPyrite:{
					index = 9;
					break;}
				case CraftResource.OldAzurite:{
					index = 10;
					break;}
				case CraftResource.OldVanadium:{
					index = 11;
					break;}
				case CraftResource.OldSilver:{
					index = 12;
					break;}
				case CraftResource.OldPlatinum:{
					index = 13;
					break;}
				case CraftResource.OldAmethyst:{
					index = 14;
					break;}
				case CraftResource.OldTitanium:{
					index = 15;
					break;}
				case CraftResource.OldGold:{
					index = 16;
					break;}
				case CraftResource.OldXenian:{
					index = 17;
					break;}
				case CraftResource.OldRubidian:{
					index = 18;
					break;}
				case CraftResource.OldObsidian:{
					index = 19;
					break;}
				case CraftResource.OldEbonSapphire:{
					index = 20;
					break;}
				case CraftResource.OldDarkRuby:{
					index = 21;
					break;}
				case CraftResource.OldRadiantDiamond:{
					index = 22;
					break;}
	
				default:{//Coil
					index = 23;
					break;}
			}
                    if( index > -1 && index < chanches.Length )
                    {
                        int chance = 0;
                        if( Region.ReadInt32( ore, "chance", ref chance ) )
                            chanches[ index ] = chance;

                        if( Config.Debug )
                            Config.Pkg.LogInfoLine( "Regional harvest parse: {0} - {1} - {2}", index, oRes, chance );
                    }
                    else
                        Config.Pkg.LogWarningLine( "Regional harvest parse: Invalid resurce type." );
                }
            }

            if( chanches[ chanches.Length - 1 ] != 0 )
                Config.Pkg.LogWarningLine( "Warning: coil chance is not 0." );
            else
                chanches[ chanches.Length - 1 ] = ilCoilRegion ? 100 : 0;

            return new RegionalHarvestDefinition( chanches, name, areaArray, map, ilCoilRegion );
        }

        internal static void Load()
        {
            var pkg = Packager.Core.Singleton[ typeof( Config ) ];

            if( !File.Exists( "Data/regionalHarvestDefinitions.xml" ) )
            {
                Config.Pkg.LogErrorLine( "Error: Data/regionalHarvestDefinitions.xml does not exist" );
                return;
            }

            if( Config.Debug )
                pkg.LogInfo( "Regional Harvest Definitions: Loading..." );

            XmlDocument doc = new XmlDocument();
            doc.Load( Path.Combine( Core.BaseDirectory, "Data/regionalHarvestDefinitions.xml" ) );

            XmlElement root = doc[ "regions" ];

            if( root == null )
            {
                pkg.LogWarningLine( "Could not find root element 'regions' in regionalHarvestDefinitions.xml" );
                pkg.LogInfo( "Loading..." );
            }
            else
            {
                List<RegionalHarvestDefinition> defList = new List<RegionalHarvestDefinition>();

                foreach( XmlElement regEl in root.SelectNodes( "region" ) )
                {
                    RegionalHarvestDefinition definition = Parse( regEl );

                    if( definition.Name == "default" )
                        m_DefaultDefinition = definition;

                    defList.Add( definition );
                }

                List = defList.ToArray();
            }

            if( Config.Debug )
            {

                pkg.LogInfoLine( "done." );
                pkg.LogInfoLine( "Regional harvest list has {0} members.", List.Length );
            }
        }

        private class InternalGump : Gump
        {
            private const int HuePrim = Colors.Violet;
            private const int HueSec = Colors.Gold;
            private const int m_Fields = 9;
            private const int m_FieldsDist = 25;
            private const int m_HueTit = 662;

            private readonly RegionalHarvestDefinition m_Definition;
            private readonly Mobile m_From;
            private int m_Page;

            public InternalGump( Mobile from )
                : this( from, 1 )
            {
            }

            private InternalGump( Mobile from, int page )
                : base( 50, 50 )
            {
                Closable = false;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_From = from;
                m_Page = page;

                m_Definition = GetRegHarDefByPoint( m_From.Map, m_From.Location );

                Design();
            }

            private void Design()
            {
                int[] chances = m_Definition.Chanches;
                HarvestVein[] veins = Mining.System.OreAndStone.Veins;

                AddPage( 0 );

                AddBackground( 0, 0, 275, 325, 9200 );

                AddImageTiled( 10, 10, 255, 25, 2624 );
                AddImageTiled( 10, 45, 255, 240, 2624 );
                AddImageTiled( 40, 295, 225, 20, 2624 );

                AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 45, 295, 75, 20, 1011012, m_HueTit, false, false ); // CANCEL

                AddAlphaRegion( 10, 10, 255, 285 );
                AddAlphaRegion( 40, 295, 225, 20 );

                AddLabelCropped( 14, 12, 255, 25, m_HueTit, m_Definition.Name + " " + ( m_Definition.IsCoilRegion ? "(Coil Region)" : "" ) );

                if( m_Page > 1 )
                    AddButton( 225, 297, 5603, 5607, 200, GumpButtonType.Reply, 0 ); // Previous page

                if( m_Page < Math.Ceiling( chances.Length / (double)m_Fields ) )
                    AddButton( 245, 297, 5601, 5605, 300, GumpButtonType.Reply, 0 ); // Next Page

                int indMax = ( m_Page * m_Fields ) - 1;
                int indMin = ( m_Page * m_Fields ) - m_Fields;
                int indTemp = 0;

                for( int i = 0; i < chances.Length; i++ )
                {
                    if( i >= indMin && i <= indMax )
                    {
                        string name = MidgardUtility.GetFriendlyClassName( veins[ i ].PrimaryResource.Types[ 0 ].Name );

                        AddOldHtmlHued( 35, 52 + ( indTemp * m_FieldsDist ), 225, 20, name, HuePrim );
                        AddOldHtmlHued( 200, 52 + ( indTemp * m_FieldsDist ), 30, 20, chances[ i ].ToString(), HueSec );
                        indTemp++;
                    }
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == 0 )
                    return;
                else if( info.ButtonID == 200 ) // Previous page
                {
                    m_Page--;
                    from.SendGump( new InternalGump( from, m_Page ) );
                }
                else if( info.ButtonID == 300 )  // Next Page
                {
                    m_Page++;
                    from.SendGump( new InternalGump( from, m_Page ) );
                }
            }
        }
    }
}