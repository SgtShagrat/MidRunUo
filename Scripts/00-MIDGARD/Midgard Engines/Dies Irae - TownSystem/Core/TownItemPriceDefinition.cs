/***************************************************************************
 *                                  TownCommercials.cs
 *                            		------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownItemPriceDefinition
    {
        private static readonly List<TownItemPriceDefinition> m_PriceDefinitions = new List<TownItemPriceDefinition>();
        private static readonly List<Type> m_VendorTypes = new List<Type>();

        public static TownItemPriceDefinition[] TownPriceDefinitionsList;

        public Type ItemType { get; private set; }
        public string ItemName { get; private set; }
        public int Figure { get; private set; }
        public int Hue { get; private set; }
        public int OriginalPrice { get; private set; }
        public bool Editable { get; private set; }

        public TownItemPriceDefinition( Type itemType, string itemName, int figure, int origPrice )
            : this( itemType, itemName, figure, origPrice, 0, true )
        {
        }

        public TownItemPriceDefinition( Type itemType, string itemName, int figure, int origPrice, int hue )
            : this( itemType, itemName, figure, origPrice, hue, true )
        {
        }

        public TownItemPriceDefinition( Type itemType, string itemName, int figure, int origPrice, int hue, bool editable )
        {
            ItemType = itemType;
            ItemName = itemName;
            Figure = figure;
            OriginalPrice = origPrice;
            Hue = hue;
            Editable = editable;
        }

        public static void RegisterCommands()
        {
            CommandSystem.Register( "GenItemPricesXml", AccessLevel.Developer, GenItemPricesXml_OnCommand );
        }

        [Usage( "GenItemPricesXml" )]
        [Description( "Generates xml data for town commercial system." )]
        private static void GenItemPricesXml_OnCommand( CommandEventArgs e )
        {
            Config.Pkg.StartWatcher( "Generating xml data for town commercial system...", true );

            Populate();

            SaveXml();

            Config.Pkg.StopWatcher( true );

            Load();
        }

        internal static void Populate()
        {
            m_PriceDefinitions.Clear();

            ProcessTypes();

            foreach( Type type in m_VendorTypes )
            {
                BaseVendor dummy = Construct( type ) as BaseVendor;
                if( dummy == null )
                    continue;

                IBuyItemInfo[] buyInfo = dummy.GetBuyInfo();

                // Config.Pkg.LogWarningLine( "{0} has {1} gbi's", type.Name, buyInfo.Length );

                foreach( IBuyItemInfo bii in buyInfo )
                {
                    GenericBuyInfo gbi = (GenericBuyInfo)bii;
                    if( gbi == null )
                        continue;

                    if( gbi.Type == null )
                        continue;

                    if( gbi.Type.Name == null )
                    {
                        Config.Pkg.LogWarningLine( "Warning: null type or name: {0}", gbi.Type );
                        continue;
                    }

                    if( IsAlreadyInList( gbi.Type ) )
                        continue;

                    object o = ConstructObject( gbi.Type );

                    string name = MidgardUtility.GetFriendlyClassName( gbi.Type.Name );
                    int itemId = 0;
                    int hue = 0;

                    if( o is Item )
                    {
                        name = StringList.GetClilocString( ( (Item)o ).Name, ( (Item)o ).LabelNumber );
                        itemId = ( (Item)o ).ItemID;
                        hue = ( (Item)o ).Hue;

                        ( (Item)o ).Delete();
                    }
                    else if( o is Mobile )
                    {
                        if( ( (Mobile)o ).Name != null )
                            name = ( (Mobile)o ).Name;
                        itemId = ShrinkTable.Lookup( ( (Mobile)o ) );
                        hue = ( (Mobile)o ).Hue;

                        ( (Mobile)o ).Delete();
                    }

                    name = StringUtility.RemoveArticle( name );

                    TownItemPriceDefinition info = new TownItemPriceDefinition( gbi.Type, name.ToLower(), itemId, gbi.OriginalPrice, hue );
                    m_PriceDefinitions.Add( info );

                    if( dummy.Map == Map.Internal )
                        dummy.Delete();
                }
            }

            if( m_PriceDefinitions != null )
                m_PriceDefinitions.Sort( InternalComparer.Instance );

            Config.Pkg.LogInfoLine( "PriceDefinitions has {0} members.", m_PriceDefinitions.Count );
        }

        internal static void SaveXml()
        {
            XDocument doc = new XDocument( new XElement( "infos", from TownItemPriceDefinition definition in m_PriceDefinitions
                                                                  where definition != null
                                                                  select ( new XElement( "info", new XAttribute( "type", String.Format( "{0}", definition.ItemType.Name ) ),
                                                                      new XAttribute( "name", String.Format( "{0}", definition.ItemName ) ),
                                                                      new XAttribute( "id", String.Format( "0x{0:X}", definition.Figure ) ),
                                                                      new XAttribute( "price", String.Format( "{0}", definition.OriginalPrice ) ),
                                                                      new XAttribute( "hue", String.Format( "0x{0:X}", definition.Hue ) ) ) ) ) );

            doc.Save( "Data/TownSystem/commercialInfos.xml" );
        }

        internal static void Load()
        {
            if( !Directory.Exists( "Data/TownSystem" ) )
            {
                Config.Pkg.LogInfoLine( "Error: Data/TownSystem does not exist" );
                return;
            }

            if( !File.Exists( "Data/TownSystem/commercialInfos.xml" ) )
            {
                Config.Pkg.LogInfoLine( "Error: Data/commercialInfos.xml does not exist" );
                return;
            }

            Config.Pkg.StartWatcher( "Item commercial info: Loading", false );

            XmlDocument doc = new XmlDocument();
            doc.Load( Path.Combine( Core.BaseDirectory, "Data/TownSystem/commercialInfos.xml" ) );

            XmlElement root = doc[ "infos" ];

            if( root == null )
                Config.Pkg.LogInfoLine( "Could not find root element 'infos' in commercialInfos.xml" );
            else
            {
                List<TownItemPriceDefinition> defList = new List<TownItemPriceDefinition>();

                foreach( XmlElement info in root.SelectNodes( "info" ) )
                    defList.Add( Parse( info ) );

                TownPriceDefinitionsList = defList.ToArray();
            }

            Config.Pkg.StopWatcher( false );
            Config.Pkg.LogInfoLine( "TownPriceDefinitionsList has {0} members.", TownPriceDefinitionsList.Length );
        }

        public static TownItemPriceDefinition GetDefFromType( Type t )
        {
            if( TownPriceDefinitionsList == null )
                return null;

            if( t == null )
                return null;

            foreach( TownItemPriceDefinition tpd in TownPriceDefinitionsList )
            {
                if( tpd.ItemType == t )
                    return tpd;
            }

            return null;
        }

        public static TownItemPriceDefinition Parse( XmlElement node )
        {
            Type itemType = null;
            string itemName = null;
            int figure = 0;
            int hue = 0;
            int origPrice = 0;
            bool editable = true;

            Region.ReadType( node, "type", ref itemType, true );
            Region.ReadString( node, "name", ref itemName, true );
            Region.ReadInt32FromHex( node, "id", ref figure, true );
            Region.ReadInt32FromHex( node, "hue", ref hue, false );
            Region.ReadInt32( node, "price", ref origPrice, true );
            Region.ReadBoolean( node, "editable", ref editable, false );

            return new TownItemPriceDefinition( itemType, itemName, figure, origPrice, hue, editable );
        }

        private static bool IsAlreadyInList( Type t )
        {
            foreach( TownItemPriceDefinition definition in m_PriceDefinitions )
            {
                if( definition.ItemType == t )
                    return true;
            }

            return false;
        }

        private static Mobile Construct( Type t )
        {
            try
            {
                return Activator.CreateInstance( t ) as Mobile;
            }
            catch
            {
                return null;
            }
        }

        private static object ConstructObject( Type t )
        {
            try
            {
                return Activator.CreateInstance( t );
            }
            catch
            {
                return null;
            }
        }

        private static void ProcessTypes()
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            foreach( Assembly asm in asms )
            {
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                foreach( Type type in types )
                {
                    if( !type.IsSubclassOf( typeof( BaseVendor ) ) )
                        continue;

                    if( type.IsAbstract )
                        continue;

                    m_VendorTypes.Add( type );
                }
            }

            Config.Pkg.LogInfoLine( "VendorTypes has {0} members.", m_VendorTypes.Count );
        }

        private enum InternalComparerType
        {
            ByTypeName,
            ByName,
            ByItemId,
            ByPrice
        }

        private class InternalComparer : IComparer<TownItemPriceDefinition>
        {
            public static readonly IComparer<TownItemPriceDefinition> Instance = new InternalComparer();
            private readonly InternalComparerType m_Type;

            private InternalComparer()
                : this( InternalComparerType.ByTypeName )
            {
            }

            private InternalComparer( InternalComparerType type )
            {
                m_Type = type;
            }

            #region IComparer<TownItemPriceDefinition> Members

            public int Compare( TownItemPriceDefinition x, TownItemPriceDefinition y )
            {
                if( x == null || y == null )
                    return 0;

                switch( m_Type )
                {
                    case InternalComparerType.ByTypeName:
                        {
                            return Insensitive.Compare( x.ItemType.Name, y.ItemType.Name );
                        }
                    case InternalComparerType.ByName:
                        {
                            return Insensitive.Compare( x.ItemName, y.ItemName );
                        }
                    case InternalComparerType.ByItemId:
                        {
                            return x.Figure.CompareTo( y.Figure );
                        }
                    case InternalComparerType.ByPrice:
                        {
                            return x.OriginalPrice.CompareTo( y.OriginalPrice );
                        }
                    default:
                        return Insensitive.Compare( x.ItemType.Name, y.ItemType.Name );
                }
            }

            #endregion
        }
    }
}