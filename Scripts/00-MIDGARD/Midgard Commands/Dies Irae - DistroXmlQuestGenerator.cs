/***************************************************************************
 *                               DistroXmlQuestGenerator.cs
 *                            --------------------------------
 *   begin                : 21 agosto, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using Server;
using Server.Commands;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Commands
{
    public class DistroXmlQuestGenerator
    {
        public static void Initialize()
        {
            CommandSystem.Register( "GenerateXmlQuesterFiles", AccessLevel.Developer, GenerateXmlQuesterFiles_OnCommand );
        }

        [Usage( "[GenerateXmlQuesterFiles" )]
        [Description( "Generates xml data for quest system." )]
        private static void GenerateXmlQuesterFiles_OnCommand( CommandEventArgs e )
        {
            ProcessTypes();
            ProcessItemTypes();
            ProcessRegions();
            ProcessQuests();
            GenerateSettingsXml();
            GenerateQuestsXml();
        }

        private class InternalComparer : IComparer<Type>
        {
            public static readonly IComparer<Type> Instance = new InternalComparer();

            public int Compare( Type x, Type y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
        }

        private class InternalRegionComparer : IComparer<Region>
        {
            public static readonly IComparer<Region> Instance = new InternalRegionComparer();

            public int Compare( Region x, Region y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
        }

        private static List<Type> m_Types = new List<Type>();
        private static List<Type> m_ItemTypes = new List<Type>();
        private static List<Region> m_RegionTypes = new List<Region>();
        private static List<Type> m_Quests = new List<Type>();
        private static Dictionary<Type, List<Type>> m_BaseGroups = new Dictionary<Type, List<Type>>();
        private static Dictionary<SlayerGroup, List<Type>> m_BaseSlayerGroups = new Dictionary<SlayerGroup, List<Type>>();
        private static SlayerGroup[] m_Slayers = SlayerGroup.Groups;

        private static string GetString( object message )
        {
            if( message == null )
                return "";

            if( message is int && (int)message > 0 )
            {
                return StringList.Localization[ (int)message ];
            }
            else if( message is string )
            {
                return (string)message;
            }

            return string.Empty;
        }

        private static void GenerateServerFaunaView()
        {
            m_BaseGroups = new Dictionary<Type, List<Type>>();

            List<Type> toRemove = new List<Type>();

            try
            {
                for( int i = 0; i < m_Types.Count; i++ )
                {
                    Type t = m_Types[ i ];

                    if( t.IsAbstract )
                    {
                        toRemove.Add( t );

                        List<Type> subclasses = new List<Type>();
                        for( int j = 0; j < m_Types.Count; j++ )
                        {
                            Type tt = m_Types[ j ];
                            if( tt.IsSubclassOf( t ) )
                            {
                                subclasses.Add( tt );
                                toRemove.Add( tt );
                            }
                        }

                        m_BaseGroups.Add( t, subclasses );
                    }
                }

                for( int i = 0; i < m_Slayers.Length; i++ )
                {
                    m_BaseSlayerGroups.Add( m_Slayers[ i ], null );

                    foreach( Type t in m_Slayers[ i ].Super.Types )
                    {
                        if( m_BaseSlayerGroups[ m_Slayers[ i ] ] == null )
                            m_BaseSlayerGroups[ m_Slayers[ i ] ] = new List<Type>();

                        m_BaseSlayerGroups[ m_Slayers[ i ] ].Add( t );

                        toRemove.Add( t );
                    }
                }

                foreach( Type type in toRemove )
                {
                    m_Types.Remove( type );
                }

                m_Types.Sort( InternalComparer.Instance );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        private static void GenerateSettingsXml()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild( doc.CreateXmlDeclaration( "1.0", "UTF-8", null ) );

                XmlElement rootElement = doc.CreateElement( "definitions" );
                doc.AppendChild( rootElement );

                XmlElement creaturesElement = doc.CreateElement( "creatureTypes" );
                AppendCreatureDefinitions( creaturesElement, false );
                rootElement.AppendChild( creaturesElement );

                XmlElement itemsElement = doc.CreateElement( "obtainTypes" );
                AppendItemDefinitions( itemsElement, false );
                rootElement.AppendChild( itemsElement );

                XmlElement skillsElement = doc.CreateElement( "skillsTypes" );
                AppendSkillDefinitions( skillsElement, false );
                rootElement.AppendChild( skillsElement );

                XmlElement regionElement = doc.CreateElement( "escortRegions" );
                AppendRegionDefinitions( regionElement, false );
                rootElement.AppendChild( regionElement );

                XmlElement deliveryElement = doc.CreateElement( "deliveryTypes" );
                AppendDeliveryItemDefinitions( deliveryElement, false );
                rootElement.AppendChild( deliveryElement );

                XmlElement rewardsElement = doc.CreateElement( "rewardTypes" );
                AppendRewardDefinitions( rewardsElement, false );
                rootElement.AppendChild( rewardsElement );

                using( XmlTextWriter writer = new XmlTextWriter( "definitions.xml", Encoding.UTF8 ) )
                {
                    writer.Formatting = Formatting.Indented;
                    writer.IndentChar = '\t';
                    writer.Indentation = 1;

                    doc.Save( writer );
                }
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception.ToString() );
            }
        }

        private static void AppendRewardDefinitions( XmlNode element, bool grouped )
        {
            try
            {
                XmlDocument doc = element.OwnerDocument;

                foreach( Type t in m_ItemTypes )
                {
                    XmlElement itemTypeElement = doc.CreateElement( "rewardType" );
                    itemTypeElement.InnerText = t.Name;
                    element.InsertAfter( itemTypeElement, element.LastChild );
                }
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception.ToString() );
            }
        }

        private static void AppendDeliveryItemDefinitions( XmlNode element, bool grouped )
        {
            try
            {
                XmlDocument doc = element.OwnerDocument;

                foreach( Type t in m_ItemTypes )
                {
                    XmlElement itemTypeElement = doc.CreateElement( "deliveryType" );
                    itemTypeElement.InnerText = t.Name;
                    element.InsertAfter( itemTypeElement, element.LastChild );
                }
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception.ToString() );
            }
        }

        private static void AppendRegionDefinitions( XmlNode element, bool grouped )
        {
            try
            {
                XmlDocument doc = element.OwnerDocument;

                foreach( Region region in m_RegionTypes )
                {
                    if( string.IsNullOrEmpty( region.Name ) )
                        continue;

                    XmlElement itemTypeElement = doc.CreateElement( "escortRegion" );
                    itemTypeElement.InnerText = region.Name ?? "-no named-";
                    element.InsertAfter( itemTypeElement, element.LastChild );
                }
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception.ToString() );
            }
        }

        private static void AppendSkillDefinitions( XmlNode element, bool grouped )
        {
            try
            {
                XmlDocument doc = element.OwnerDocument;

                foreach( string skill in Enum.GetNames( typeof( SkillName ) ) )
                {
                    XmlElement skillElement = doc.CreateElement( "skillType" );
                    skillElement.InnerText = skill;
                    element.InsertAfter( skillElement, element.LastChild );
                }
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception.ToString() );
            }
        }

        private static void AppendCreatureDefinitions( XmlNode element, bool grouped )
        {
            try
            {
                XmlDocument doc = element.OwnerDocument;

                foreach( Type t in m_Types )
                {
                    XmlElement creatureTypeElement = doc.CreateElement( "creatureType" );
                    creatureTypeElement.InnerText = t.Name;
                    element.InsertAfter( creatureTypeElement, element.LastChild );
                }

                Dictionary<Type, List<Type>>.KeyCollection baseg = m_BaseGroups.Keys;
                foreach( Type type in baseg )
                {
                    List<Type> list;
                    m_BaseGroups.TryGetValue( type, out list );

                    foreach( Type type1 in list )
                    {
                        XmlElement creatureTypeElement = doc.CreateElement( "creatureType" );
                        creatureTypeElement.InnerText = type1.Name;
                        element.InsertAfter( creatureTypeElement, element.LastChild );
                    }
                }

                Dictionary<SlayerGroup, List<Type>>.KeyCollection baseSlayerg = m_BaseSlayerGroups.Keys;
                foreach( SlayerGroup g in baseSlayerg )
                {
                    List<Type> list;
                    m_BaseSlayerGroups.TryGetValue( g, out list );

                    foreach( Type type1 in list )
                    {
                        XmlElement creatureTypeElement = doc.CreateElement( "creatureType" );
                        creatureTypeElement.InnerText = type1.Name;
                        element.InsertAfter( creatureTypeElement, element.LastChild );
                    }
                }
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception.ToString() );
            }
        }

        private static void AppendItemDefinitions( XmlNode element, bool grouped )
        {
            try
            {
                XmlDocument doc = element.OwnerDocument;

                foreach( Type t in m_ItemTypes )
                {
                    XmlElement itemTypeElement = doc.CreateElement( "itemType" );
                    itemTypeElement.InnerText = t.Name;
                    element.InsertAfter( itemTypeElement, element.LastChild );
                }
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception.ToString() );
            }
        }

        private static void ProcessQuests()
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            for( int i = 0; i < asms.Length; ++i )
            {
                Assembly asm = asms[ i ];
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                for( int j = 0; j < types.Length; ++j )
                {
                    Type type = types[ j ];

                    if( type.IsSubclassOf( typeof( BaseQuest ) ) )
                    {
                        m_Quests.Add( type );
                    }
                }
            }

            m_Quests.Sort( InternalComparer.Instance );
        }

        private static void ProcessTypes()
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            for( int i = 0; i < asms.Length; ++i )
            {
                Assembly asm = asms[ i ];
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                for( int j = 0; j < types.Length; ++j )
                {
                    Type type = types[ j ];

                    if( type.IsSubclassOf( typeof( BaseCreature ) ) )
                    {
                        m_Types.Add( type );
                    }
                }
            }
        }

        private static void ProcessItemTypes()
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            for( int i = 0; i < asms.Length; ++i )
            {
                Assembly asm = asms[ i ];
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                for( int j = 0; j < types.Length; ++j )
                {
                    Type type = types[ j ];

                    if( type.IsSubclassOf( typeof( Item ) ) )
                    {
                        m_ItemTypes.Add( type );
                    }
                }
            }

            m_ItemTypes.Sort( InternalComparer.Instance );
        }

        private static void ProcessRegions()
        {
            foreach( Region region in Region.Regions )
            {
                if( region.Map == Map.Felucca || region.Map == Map.Ilshenar )
                    m_RegionTypes.Add( region );
            }

            m_RegionTypes.Sort( InternalRegionComparer.Instance );
        }

        private static void GenerateQuestsXml()
        {
            ScriptCompiler.EnsureDirectory( "XmlQuests" );

            foreach( Type quest in m_Quests )
            {
                //try
                //{
                BaseQuest instance = Activator.CreateInstance( quest ) as BaseQuest;

                if( instance != null )
                    GenerateQuestXml( instance );
                else
                    Console.WriteLine( "Quest of type {0} failed to inizialize.", quest.Name );
                //}
                //catch( Exception ex )
                //{
                //    Console.WriteLine( ex.ToString() );
                //}
            }
        }

        private static void GenerateQuestXml( BaseQuest quest )
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild( doc.CreateXmlDeclaration( "1.0", "UTF-8", null ) );

            XmlElement rootElement = doc.CreateElement( "Quest" );
            doc.AppendChild( rootElement );

            AppendStringElement( rootElement, "Name", quest.Title );
            AppendStringElement( rootElement, "Description", quest.Description );
            AppendStringElement( rootElement, "RefuseMessage", quest.Refuse );
            AppendStringElement( rootElement, "CompleteMessage", quest.Complete );
            AppendStringElement( rootElement, "UncompleteMessage", quest.Uncomplete );

            AppendBoolElement( rootElement, "AllObjectives", quest.AllObjectives );
            AppendBoolElement( rootElement, "DoneOnce", quest.DoneOnce );

            XmlElement child = doc.CreateElement( "RestartDelay" );
            child.InnerText = quest.RestartDelay.ToString();
            rootElement.AppendChild( child );

            #region objectives
            XmlElement objectives = doc.CreateElement( "Objectives" );
            XmlElement tempobj = doc.CreateElement( "SlayObjectives" );

            foreach( BaseObjective obj in quest.Objectives )
            {
                if( obj is SlayObjective )
                    AppendObjective( tempobj, (SlayObjective)obj );
            }
            objectives.AppendChild( tempobj );

            tempobj = doc.CreateElement( "ObtainObjectives" );
            foreach( BaseObjective obj in quest.Objectives )
            {
                if( obj is ObtainObjective )
                    AppendObjective( tempobj, (ObtainObjective)obj );
            }
            objectives.AppendChild( tempobj );

            tempobj = doc.CreateElement( "EscortObjectives" );
            foreach( BaseObjective obj in quest.Objectives )
            {
                if( obj is EscortObjective )
                    AppendObjective( tempobj, (EscortObjective)obj );
            }
            objectives.AppendChild( tempobj );

            tempobj = doc.CreateElement( "DeliverObjectives" );
            foreach( BaseObjective obj in quest.Objectives )
            {
                if( obj is DeliverObjective )
                    AppendObjective( tempobj, (DeliverObjective)obj );
            }
            objectives.AppendChild( tempobj );

            tempobj = doc.CreateElement( "ApprenticeObjectives" );
            foreach( BaseObjective obj in quest.Objectives )
            {
                if( obj is ApprenticeObjective )
                    AppendObjective( tempobj, (ApprenticeObjective)obj );
            }
            objectives.AppendChild( tempobj );

            rootElement.AppendChild( objectives );
            #endregion

            #region rewards
            XmlElement rewards = doc.CreateElement( "Rewards" );

            foreach( BaseReward rew in quest.Rewards )
            {
                AppendReward( rewards, rew );
            }

            rootElement.AppendChild( rewards );
            #endregion

            string path = Path.Combine( "XmlQuests", string.Format( "{0}.xml", quest.GetType().Name ) );

            using( XmlTextWriter writer = new XmlTextWriter( path, Encoding.UTF8 ) )
            {
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = '\t';
                writer.Indentation = 1;

                doc.Save( writer );
            }
        }

        private static void AppendStringElement( XmlNode element, string tag, object value )
        {
            XmlElement child = element.OwnerDocument.CreateElement( tag );
            child.InnerText = GetString( value );
            element.AppendChild( child );
        }

        private static void AppendBoolElement( XmlNode element, string tag, bool value )
        {
            XmlElement child = element.OwnerDocument.CreateElement( tag );
            child.InnerText = value.ToString();
            element.AppendChild( child );
        }

        private static void AppendObjective( XmlNode element, SlayObjective objective )
        {
            XmlElement child = element.OwnerDocument.CreateElement( "SlayObjective" );

            if( objective.Creature != null )
                child.SetAttribute( "creatureType", objective.Creature.Name );
            else
                child.SetAttribute( "creatureType", "" );

            child.SetAttribute( "creatureName", objective.Name ?? "" );
            child.SetAttribute( "amount", objective.MaxProgress.ToString() );

            if( objective.Region != null )
                child.SetAttribute( "region", objective.Region.Name ?? "" );
            else
                child.SetAttribute( "region", "" );

            child.SetAttribute( "seconds", objective.Seconds.ToString() );

            element.AppendChild( child );
        }

        private static void AppendObjective( XmlNode element, ObtainObjective objective )
        {
            XmlElement child = element.OwnerDocument.CreateElement( "ObtainObjective" );

            if( objective.Obtain != null )
                child.SetAttribute( "obtainType", objective.Obtain.Name );
            else
                child.SetAttribute( "obtainType", "" );

            child.SetAttribute( "name", objective.Name ?? "" );
            child.SetAttribute( "amount", objective.MaxProgress.ToString() );
            child.SetAttribute( "image", objective.Image.ToString() );
            child.SetAttribute( "seconds", objective.Seconds.ToString() );

            element.AppendChild( child );
        }

        private static void AppendObjective( XmlNode element, EscortObjective objective )
        {
            XmlElement child = element.OwnerDocument.CreateElement( "EscortObjective" );

            if( objective.Region != null )
                child.SetAttribute( "region", objective.Region.Name ?? "-" );
            else
                child.SetAttribute( "region", "" );

            child.SetAttribute( "fame", objective.Fame.ToString() );

            element.AppendChild( child );
        }

        private static void AppendObjective( XmlNode element, DeliverObjective objective )
        {
            XmlElement child = element.OwnerDocument.CreateElement( "DeliverObjective" );

            child.SetAttribute( "delivery", objective.Delivery.ToString() );
            child.SetAttribute( "deliveryName", objective.DeliveryName ?? "" );

            if( objective.Destination != null )
                child.SetAttribute( "destination", objective.Destination.Name );
            else
                child.SetAttribute( "destination", "" );

            child.SetAttribute( "destName", objective.DestName ?? "" );
            child.SetAttribute( "maxProgress", objective.MaxProgress.ToString() );

            element.AppendChild( child );
        }

        private static void AppendObjective( XmlNode element, ApprenticeObjective objective )
        {
            XmlElement child = element.OwnerDocument.CreateElement( "ApprenticeObjective" );

            child.SetAttribute( "skill", objective.Skill.ToString() );
            child.SetAttribute( "cap", objective.MaxProgress.ToString() );

            if( objective.Region != null )
                child.SetAttribute( "region", objective.Region.ToString() );
            else
                child.SetAttribute( "region", "" );

            child.SetAttribute( "enterRegion", GetString( objective.Enter ) );
            child.SetAttribute( "leaveRegion", GetString( objective.Leave ) );

            element.AppendChild( child );
        }

        private static void AppendReward( XmlNode element, BaseReward reward )
        {
            XmlElement child = element.OwnerDocument.CreateElement( "Reward" );

            child.SetAttribute( "name", GetString( reward.Name ) );

            if( reward.Type != null )
                child.SetAttribute( "type", reward.Type.Name );
            else
                child.SetAttribute( "type", "" );

            child.SetAttribute( "amount", reward.Amount.ToString() );

            element.AppendChild( child );
        }
    }
}