/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 27 aprile 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Server;
using Server.Mobiles;

namespace Midgard.Engines.CommercialSystem
{
    internal class Core
    {
        #region Singleton pattern
        private static Core m_Instance;

        private Core()
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Commercial System singleton instanced." );

            CommercialInfosDict = new Dictionary<Type, MidgardCommercialInfo>();
            CommercialCompactInfosDict = new Dictionary<Type, MidgardCommercialCompactInfo>();
        }

        public static Core Instance
        {
            get
            {
                if( m_Instance == null )
                    m_Instance = new Core();

                return m_Instance;
            }
        }
        #endregion

        private CommercialTimer m_Timer;

        public Dictionary<Type, MidgardCommercialCompactInfo> CommercialCompactInfosDict { get; set; }

        public Dictionary<Type, MidgardCommercialInfo> CommercialInfosDict { get; set; }

        private MidgardCommercialInfo GetInfoFromType( Type type )
        {
            if( !Config.Enabled || Instance.CommercialInfosDict == null || type == null )
                return null;

            MidgardCommercialInfo info;
            if( CommercialInfosDict.TryGetValue( type, out info ) && info != null )
                return info;
            else
                Config.Pkg.LogWarningLine( "Error in GetInfoFromType for type: {0}", type.Name );

            return null;
        }

        private MidgardCommercialCompactInfo GetCompactInfoFromType( Type type )
        {
            if( !Config.Enabled || Instance.CommercialCompactInfosDict == null || type == null )
                return null;

            MidgardCommercialCompactInfo info;
            if( CommercialCompactInfosDict.TryGetValue( type, out info ) && info != null )
                return info;
            else
                Config.Pkg.LogWarningLine( "Error in GetCompactInfoFromType for type: {0}", type.Name );

            return null;
        }

        internal void ProcessCommercialSystem()
        {
            foreach( KeyValuePair<Type, MidgardCommercialInfo> keyValuePair in CommercialInfosDict )
            {
                MidgardCommercialInfo info = keyValuePair.Value;

                if( info.RequiresRefresh )
                    info.ProcessDecay();
            }
        }

        internal void StartTimer()
        {
            if( m_Timer == null )
                m_Timer = new CommercialTimer();

            Config.Pkg.LogInfoLine( "Commercial system timer started." );

            m_Timer.Start();
        }

        #region serialization
        internal static void ConfigSystem()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( Load );
            EventSink.WorldSave += new WorldSaveEventHandler( Save );
        }

        public static void Save( WorldSaveEventArgs e )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "{0}: Saving...", Config.Pkg.Title );

            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                string dir = Path.Combine( Path.GetPathRoot( Config.CommercialSavePath ), Path.GetDirectoryName( Config.CommercialSavePath ) );
                if( !Directory.Exists( dir ) )
                    Directory.CreateDirectory( dir );

                BinaryFileWriter writer = new BinaryFileWriter( Config.CommercialSavePath, true );
                Instance.Serialize( writer );
                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch
            {
                Config.Pkg.LogErrorLine( "Error serializing {0}.", Config.Pkg.Title );
            }

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        public static void Load()
        {
            if( Config.Debug )
                Console.Write( "{0}: Loading...", Config.Pkg.Title );

            while( !File.Exists( Config.CommercialSavePath ) )
            {
                Console.WriteLine( "Warning: {0} not found.", Config.CommercialSavePath );
                Console.WriteLine( " - Press return to continue, or R to try again." );
                string str = Console.ReadLine();

                if( str == null || str.ToLower() != "r" )
                    return;
            }

            try
            {
                BinaryReader bReader = new BinaryReader( File.OpenRead( Config.CommercialSavePath ) );
                BinaryFileReader reader = new BinaryFileReader( bReader );
                new Core( reader );

                bReader.Close();
            }
            catch
            {
                Console.WriteLine( "Error deserializing {0}.", Config.Pkg.Title );
            }

            if( Config.Debug )
                Console.WriteLine( "done." );
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( 0 ); // version

            if( CommercialInfosDict == null || CommercialInfosDict.Count == 0 )
            {
                writer.Write( 0 );
                return;
            }
            else
                writer.Write( CommercialInfosDict.Count );

            foreach( KeyValuePair<Type, MidgardCommercialInfo> keyValuePair in CommercialInfosDict )
                keyValuePair.Value.Serialize( writer );
        }

        public Core( GenericReader reader )
        {
            int version = reader.ReadInt();

            int counter = reader.ReadInt();

            if( CommercialInfosDict == null )
                CommercialInfosDict = new Dictionary<Type, MidgardCommercialInfo>();

            for( int i = 0; i < counter; i++ )
            {
                MidgardCommercialInfo info = new MidgardCommercialInfo( reader );

                Type t = ScriptCompiler.FindTypeByFullName( info.ItemType.FullName );
                if( t == null )
                {
                    Console.WriteLine( "Warning: null type under comercial rules. Type: {0}", info.ItemType.FullName );
                    continue;
                }

                if( CommercialInfosDict.ContainsKey( t ) )
                {
                    Console.WriteLine( "Warning: already defined type under commercial system. Type: {0}", info.ItemType.FullName );
                    continue;
                }

                CommercialInfosDict.Add( t, info );
            }
        }
        #endregion

        /// <summary>
        /// Check if our type is under commercial system modifications
        /// </summary>
        public static bool IsUnderCommercialRules( Type type )
        {
            return ( type != null && Config.Enabled && Instance.CommercialInfosDict.ContainsKey( type ) );
        }

        /// <summary>
        /// Get the scalar for vendor to player purchases
        /// </summary>
        public static double GetBuyFromVendorScalarFor( Type type )
        {
            // MidgardCommercialInfo info = Instance.GetInfoFromType( type );

            return 1.0; // TODO: rimuovere quando il sistema andra' effettivamente abilitato.

            // return ( Config.Enabled && info != null ) ? info.BuyFromVendorScalar : 1.0;
        }

        /// <summary>
        /// Get the scalar for player to vendor purchases 
        /// </summary>
        public static double GetSellToVendorScalarFor( Type type )
        {
            // MidgardCommercialInfo info = Instance.GetInfoFromType( type );

            return 1.0; // TODO: rimuovere quando il sistema andra' effettivamente abilitato.

            // return ( Config.Enabled && info != null ) ? info.SellToVendorScalar : 1.0;
        }

        public static int GetSellToVendorBasePriceFor( Type type )
        {
            MidgardCommercialCompactInfo info = Instance.GetCompactInfoFromType( type );

            return ( Config.Enabled && info != null ) ? info.SellToVendorPrice : 0;
        }

        public static int GetButFromVendBasePriceFor( Type type )
        {
            MidgardCommercialCompactInfo info = Instance.GetCompactInfoFromType( type );

            return ( Config.Enabled && info != null ) ? info.BuyFromVendorPrice : 0;
        }

        /// <summary>
        /// Register a new MidgardCommercialInfo associated with our type
        /// </summary>
        public static void RegisterCommercialBuy( GenericBuyInfo buy )
        {
            if( buy.Type == null || Instance.CommercialInfosDict.ContainsKey( buy.Type ) )
                return;

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Registered commercial definition for type: {0}", buy.Type );

            Instance.CommercialInfosDict.Add( buy.Type, new MidgardCommercialInfo( buy.Type ) );

            if( Instance.CommercialCompactInfosDict.ContainsKey( buy.Type ) )
                Instance.CommercialCompactInfosDict[ buy.Type ].BuyFromVendorPrice = buy.Price;
            else
                Instance.CommercialCompactInfosDict[ buy.Type ] = new MidgardCommercialCompactInfo( buy.Type, 0, buy.Price );
        }

        /// <summary>
        /// Register a new MidgardCommercialInfo associated with our type
        /// </summary>
        public static void RegisterCommercialSell( Type t, int price )
        {
            if( t == null || Instance.CommercialInfosDict.ContainsKey( t ) )
                return;

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Registered commercial definition for type: {0}", t );

            Instance.CommercialInfosDict.Add( t, new MidgardCommercialInfo( t ) );

            if( Instance.CommercialCompactInfosDict.ContainsKey( t ) )
                Instance.CommercialCompactInfosDict[ t ].SellToVendorPrice = price;
            else
                Instance.CommercialCompactInfosDict[ t ] = new MidgardCommercialCompactInfo( t, price, 0 );
        }

        /// <summary>
        /// Register a valid purchare in the commercial system
        /// </summary>
        public static void RegisterPurchase( Mobile from, Type type, int amount, int price, bool boughtFromVendor )
        {
            if( type == null || !IsUnderCommercialRules( type ) )
                return;

            MidgardCommercialInfo info = Instance.GetInfoFromType( type );
            if( info != null )
            {
                info.RegisterDelta( boughtFromVendor ? CommercialDeltaType.BuyFromVendor : CommercialDeltaType.SellToVendor, amount );

                if( boughtFromVendor )
                    CommercialLog.WriteBuyFromVendorAction( from, type, amount, price );
                else
                    CommercialLog.WriteSellToVendorAction( from, type, amount, price );
            }
            else
                Config.Pkg.LogWarningLine( "Warning: RegisterPurchase with null commercial info. Type: {0}", type.Name );
        }

        public void DoCommercialSystemReset( Mobile from )
        {
            foreach( KeyValuePair<Type, MidgardCommercialInfo> keyValuePair in Instance.CommercialInfosDict )
            {
                keyValuePair.Value.Reset();
            }

            Config.Pkg.LogInfoLine( "System successfully reset." );
            from.SendMessage( "Commercial system: all commercial infos have been reset." );
        }

        public void ToggleCommercialSystemStatus( Mobile from, bool enable )
        {
            Config.Enabled = enable;

            Config.Pkg.LogInfoLine( "System successfully toggled to: {0}", Config.Enabled ? "enabled" : "disabled" );
            from.SendMessage( "Commercial system: system is now {0}.", Config.Enabled ? "enabled" : "disabled" );
        }

        public void DoCommercialSystemReport( Mobile from )
        {
            try
            {
                using( TextWriter writer = File.CreateText( "Logs/commercial.log" ) )
                {
                    writer.WriteLine( "\n{0}:", DateTime.Now.ToShortTimeString() );

                    List<MidgardCommercialInfo> list = new List<MidgardCommercialInfo>( CommercialInfosDict.Values );
                    list.Sort();

                    foreach( MidgardCommercialInfo info in list )
                    {
                        writer.WriteLine( info.ToString() );
                    }
                }

                ProcessTypes();

                using( TextWriter writer = File.CreateText( "Logs/sell.log" ) )
                {
                    foreach( Type type in m_SellInfoTypes )
                    {
                        GenericSellInfo info = Activator.CreateInstance( type ) as GenericSellInfo;
                        if( info != null && info.Types.Length > 0 )
                        {
                            writer.WriteLine( type.Name );
                            foreach( Type t in info.Types )
                            {
                                int price;
                                info.GetSellPriceFor( t, out price );

                                if( info.Demultiplicator > 0.0 )
                                    price = (int)( price * info.Demultiplicator );

                                if( price < 1 )
                                    price = 1;

                                writer.WriteLine( "\t{0} - {1}", t.Name, price );
                            }
                        }
                    }
                }

                from.SendMessage( "Commercial report has been generated. See the file : <runuo root>/Logs/commercial.log" );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        private static List<Type> m_SellInfoTypes = new List<Type>();

        private static void ProcessTypes()
        {
            foreach( Assembly asm in ScriptCompiler.Assemblies )
            {
                foreach( Type type in ScriptCompiler.GetTypeCache( asm ).Types )
                {
                    if( type.IsSubclassOf( typeof( GenericSellInfo ) ) )
                        m_SellInfoTypes.Add( type );
                }
            }

            m_SellInfoTypes.Sort( InternalComparer.ComparerInstance );
        }

        private class InternalComparer : IComparer<Type>
        {
            public static readonly IComparer<Type> ComparerInstance = new InternalComparer();

            public int Compare( Type x, Type y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
        }
    }
}