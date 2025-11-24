using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Midgard.Engines.AdvancedCooking;
using Midgard.Engines.BrewCrafing;
using Midgard.Engines.Craft;
using Server;
using Server.Commands;
using Server.Engines.Craft;

namespace Midgard.Engines.OldCraftSystem
{
    public class GererateCraftSystemXmlCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register( "GenerateCraftSystemXml", AccessLevel.Developer, new CommandEventHandler( GenerateCraftSystemXml_OnCommand ) );
            CommandSystem.Register( "GenerateCraftSystemLog", AccessLevel.Developer, new CommandEventHandler( GenerateCraftSystemLog_OnCommand ) );
            CommandSystem.Register( "GenChanceReport", AccessLevel.Developer, new CommandEventHandler( GenChanceReport_OnCommand ) );

            if( Core.Debug )
            {
                Timer.DelayCall( TimeSpan.Zero, delegate { GenChanceReport( DefTailoring.CraftSystem ); } );
                Timer.DelayCall( TimeSpan.Zero, delegate { GenChanceReport( DefCarpentry.CraftSystem ); } );
                Timer.DelayCall( TimeSpan.Zero, delegate { GenChanceReport( DefTinkering.CraftSystem ); } );
            }
        }

        private static void GenChanceReport_OnCommand( CommandEventArgs e )
        {
            GenChanceReport( DefTailoring.CraftSystem );
        }

        private static void GenChanceReport( CraftSystem sys )
        {
            List<CraftItem> craftList = new List<CraftItem>();

            foreach( CraftItem craftItem in sys.CraftItems )
                craftList.Add( craftItem );

            for( int skill = 0; skill < 100; skill++ )
            {
                Dictionary<CraftItem, double> dict = new Dictionary<CraftItem, double>();

                foreach( CraftItem craftItem in craftList )
                    dict[ craftItem ] = GetChance( sys, craftItem, skill );

                LogResult( sys, skill, GetBestFromDict( dict ), "Logs/" + sys.Name.ToLower() + ".log" );
            }
        }

        private static void LogResult( CraftSystem sys, int skill, CraftItem best, string log )
        {
            using( StreamWriter sw = new StreamWriter( log, true ) )
                sw.WriteLine( String.Format( "{0} - {1} - {2:F5}", skill, MidgardUtility.GetFriendlyClassName( best.ItemType.Name ), GetChance( sys, best, skill ) ) );
        }

        private static CraftItem GetBestFromDict( Dictionary<CraftItem, double> dict )
        {
            double best = 0.0;
            CraftItem bestItem = null;

            foreach( KeyValuePair<CraftItem, double> keyValuePair in dict )
            {
                if( keyValuePair.Value <= best )
                    continue;

                bestItem = keyValuePair.Key;
                best = keyValuePair.Value;
            }

            return bestItem;
        }

        private static double GetChance( CraftSystem craftSystem, CraftItem craftItem, double skill )
        {
            CraftSkill craftSkill = craftItem.Skills.GetAt( 0 );

            double minSkill = craftSkill.MinSkill;
            double maxSkill = craftSkill.MaxSkill;
            double valSkill = skill;

            if( skill < minSkill )
                return 0.0;

            double chance = craftSystem.GetChanceAtMin( craftItem ) + ( ( valSkill - minSkill ) / ( maxSkill - minSkill ) * ( 1.0 - craftSystem.GetChanceAtMin( craftItem ) ) );

            return SkillSystem.Core.PolGetChance( craftSystem.MainSkill, chance, skill );
        }

        [Usage( "GenerateCraftSystemXml" )]
        [Description( "Generate midgard third crown craft system xml definition files" )]
        private static void GenerateCraftSystemLog_OnCommand( CommandEventArgs e )
        {
            GenerateCraftSystemLog( DefAlchemy.CraftSystem, "Alchemy" );
            GenerateCraftSystemLog( DefBlacksmithy.CraftSystem, "Blacksmithy" );
            GenerateCraftSystemLog( DefBowFletching.CraftSystem, "BowFletching" );
            GenerateCraftSystemLog( DefCarpentry.CraftSystem, "Carpentry" );
            GenerateCraftSystemLog( DefCartography.CraftSystem, "Cartography" );
            GenerateCraftSystemLog( DefCooking.CraftSystem, "Cooking" );
            GenerateCraftSystemLog( DefGlassblowing.CraftSystem, "Glassblowing" );
            GenerateCraftSystemLog( DefInscription.CraftSystem, "Inscription" );
            GenerateCraftSystemLog( DefMasonry.CraftSystem, "Masonry" );
            GenerateCraftSystemLog( DefTailoring.CraftSystem, "Tailoring" );
            GenerateCraftSystemLog( DefTinkering.CraftSystem, "Tinkering" );

            GenerateCraftSystemLog( DefBaking.CraftSystem, "Baking" );
            GenerateCraftSystemLog( DefBoiling.CraftSystem, "Boiling" );
            GenerateCraftSystemLog( DefGrilling.CraftSystem, "Grilling" );
            GenerateCraftSystemLog( DefBrewing.CraftSystem, "Brewing" );
            GenerateCraftSystemLog( DefCrystalCrafting.CraftSystem, "CrystalCrafting" );
            GenerateCraftSystemLog( DefWaxCrafting.CraftSystem, "WaxCrafting" );
        }

        [Usage( "GenerateCraftSystemLog" )]
        [Description( "Generate midgard third crown craft system log definition files" )]
        private static void GenerateCraftSystemXml_OnCommand( CommandEventArgs e )
        {
            GenerateCraftSystemXml( DefAlchemy.CraftSystem, "Alchemy" );
            GenerateCraftSystemXml( DefBlacksmithy.CraftSystem, "Blacksmithy" );
            GenerateCraftSystemXml( DefBowFletching.CraftSystem, "BowFletching" );
            GenerateCraftSystemXml( DefCarpentry.CraftSystem, "Carpentry" );
            GenerateCraftSystemXml( DefCartography.CraftSystem, "Cartography" );
            GenerateCraftSystemXml( DefCooking.CraftSystem, "Cooking" );
            GenerateCraftSystemXml( DefGlassblowing.CraftSystem, "Glassblowing" );
            GenerateCraftSystemXml( DefInscription.CraftSystem, "Inscription" );
            GenerateCraftSystemXml( DefMasonry.CraftSystem, "Masonry" );
            GenerateCraftSystemXml( DefTailoring.CraftSystem, "Tailoring" );
            GenerateCraftSystemXml( DefTinkering.CraftSystem, "Tinkering" );

            GenerateCraftSystemXml( DefBaking.CraftSystem, "Baking" );
            GenerateCraftSystemXml( DefBoiling.CraftSystem, "Boiling" );
            GenerateCraftSystemXml( DefGrilling.CraftSystem, "Grilling" );
            GenerateCraftSystemXml( DefBrewing.CraftSystem, "Brewing" );
            GenerateCraftSystemXml( DefCrystalCrafting.CraftSystem, "CrystalCrafting" );
            GenerateCraftSystemXml( DefWaxCrafting.CraftSystem, "WaxCrafting" );
        }

        private static void GenerateCraftSystemLog( CraftSystem system, string fileName )
        {
            CraftDefinitionTree tree = system.DefinitionTree;
            if( tree == null )
                return;

            using( TextWriter writer = File.CreateText( "Logs/" + fileName + ".log" ) )
                LogParentNode( tree.Root, writer );
        }

        private static void LogParentNode( ParentNode node, TextWriter writer )
        {
            writer.WriteLine( "" );
            writer.WriteLine( node.ToString() );

            foreach( object o in node.Children )
            {
                if( o is ParentNode )
                    LogParentNode( (ParentNode)o, writer );
                else if( o is ChildNode )
                    LogChildNode( (ChildNode)o, writer );
            }
        }

        private static void LogChildNode( ChildNode node, TextWriter writer )
        {
            writer.WriteLine( "" );
            writer.WriteLine( "\t" + node );
        }

        private static void GenerateCraftSystemXml( CraftSystem system, string systemName )
        {
            Item crafted = null;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild( doc.CreateXmlDeclaration( "1.0", "UTF-8", null ) );

                // <parent name="Alchemy" title="What would you like to make?" selectresource="true">
                XmlElement craftDefinitionElement = doc.CreateElement( "craftDefinition" );

                XmlElement systemElement = doc.CreateElement( "parent" );
                systemElement.SetAttribute( "name", systemName );
                systemElement.SetAttribute( "title", "What would you like to make?" );
                systemElement.SetAttribute( "selectresource", "true" );
                craftDefinitionElement.InsertAfter( systemElement, craftDefinitionElement.LastChild );

                foreach( CraftGroup group in system.CraftGroups )
                {
                    // <parent name="refresh potions" itemID ="0xF0B" title="What do you want to make?">
                    XmlElement groupParent = doc.CreateElement( "parent" );

                    groupParent.SetAttribute( "name", StringList.GetClilocString( group.NameString, group.NameNumber ) );

                    CraftItem firstItem = group.CraftItems.GetAt( 0 );

                    try
                    {
                        if( !typeof( CustomCraft ).IsAssignableFrom( firstItem.ItemType ) )
                            crafted = Activator.CreateInstance( firstItem.ItemType ) as Item;
                    }
                    catch
                    {
                    }

                    groupParent.SetAttribute( "itemID", String.Format( "0x{0:X}", CraftItem.ItemIDOf( firstItem.ItemType ) ) );
                    if( crafted != null && crafted.Hue > 0 )
                        groupParent.SetAttribute( "hue", String.Format( "0x{0:X}", crafted.Hue ) );
                    groupParent.SetAttribute( "title", "What do you want to make?" );

                    foreach( CraftItem craftItem in group.CraftItems )
                    {
                        bool isCustomCraft = typeof( CustomCraft ).IsAssignableFrom( craftItem.ItemType );

                        try
                        {
                            if( !isCustomCraft )
                                crafted = Activator.CreateInstance( craftItem.ItemType ) as Item;
                        }
                        catch( Exception ex )
                        {
                            Config.Pkg.LogWarningLine( "Warning: null item for type: {0}", craftItem.ItemType.Name );
                            Config.Pkg.LogInfoLine( ex.ToString() );
                        }

                        if( !isCustomCraft && crafted == null )
                        {
                            Config.Pkg.LogWarningLine( "Warning: null item for type: {0}", craftItem.ItemType.Name );
                            continue;
                        }

                        // <child name="Refresh Potion" typename="RefreshPotion" />
                        XmlElement childElement = doc.CreateElement( "child" );
                        childElement.SetAttribute( "name", StringList.GetClilocString( craftItem.NameString, craftItem.NameNumber ) );
                        childElement.SetAttribute( "typename", craftItem.ItemType.Name );

                        if( crafted != null && crafted.Hue != 0 )
                            childElement.SetAttribute( "hue", String.Format( "0x{0:X}", crafted.Hue ) );

                        groupParent.InsertAfter( childElement, groupParent.LastChild );
                    }

                    systemElement.InsertAfter( groupParent, systemElement.LastChild );
                }

                craftDefinitionElement.AppendChild( systemElement );
                doc.AppendChild( craftDefinitionElement );

                using( XmlTextWriter writer = new XmlTextWriter( systemName + ".xml", Encoding.UTF8 ) )
                {
                    writer.Formatting = Formatting.Indented;
                    writer.IndentChar = '\t';
                    writer.Indentation = 1;

                    doc.Save( writer );
                }
            }
            catch( Exception ex )
            {
                Config.Pkg.LogInfoLine( ex.ToString() );
            }
        }
    }
}