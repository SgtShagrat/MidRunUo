/***************************************************************************
 *                               TownWebCommands.cs
 *
 *   begin                : 16 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;

using Server;

using Core = Midgard.Engines.MyXmlRPC.Core;
using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.MidgardTownSystem
{
    public class WebCommands
    {
        #region AccountComandType enum
        public enum AccountComandType
        {
            Invalid = 0,
            Info
        }
        #endregion

        #region ErrorResultTypes enum
        public enum ErrorResultTypes
        {
            InvalidAdminCommand = -50,
            InvalidTown
        }
        #endregion

        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=townRemoting&towncmd=GetTowns
            Core.Register( "townRemoting", new MyXmlEventHandler( TownRemotingHandler ), null );
        }

        public static void TownRemotingHandler( MyXmlEventArgs e )
        {
            try
            {
                e.Exitcode = -1;

                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "TownRemoting command called..." );

                var commandType = Utility.SafeGetKey( e.Args, "towncmd", null );

                if( string.IsNullOrEmpty( commandType ) )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidAdminCommand;
                    throw new ArgumentNullException( "towncmd" );
                }

                var town = Utility.SafeGetKey( e.Args, "town", null );

                if( string.IsNullOrEmpty( town ) )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidTown;
                    throw new ArgumentNullException( "town" );
                }

                TownSystem sys = TownSystem.Parse( town );
                if( sys == null )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidTown;
                    throw new ArgumentNullException( "town" );
                }

                var handle = typeof( WebCommands ).GetMethod( "Handle" + commandType,
                                                                BindingFlags.Static | BindingFlags.Public |
                                                                BindingFlags.NonPublic );

                if( handle == null )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidAdminCommand;
                    throw new ArgumentException( "Invalid command \"" + commandType + "\".", "towncmd" );
                }

                // All Handle...methods can use Throw to raise an exception.
                // Exit code will be automatically setted to -1 if errors occours.                
                handle.Invoke( null, new object[]
                                    {
                                        e
                                    } );

                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        private static XElement GetTownInfoXml( TownSystem system )
        {
            XElement noticeElement = new XElement( "Notice", new XElement( "town", new XAttribute( "type", system.GetType().Name ) ) );

            XElement townElement = noticeElement.Descendants( "town" ).Last();

            AppendTownInfoXml( townElement, system );

            AppendCitizensInfoXml( townElement, system );

            return noticeElement;
        }

        private static void AppendTownInfoXml( XContainer element, TownSystem system )
        {
            element.Add( new XElement( "infos",
                                     new XElement( "name", Utility.SafeString( system.Definition.TownName ) ),
                                     new XElement( "regionName", Utility.SafeString( system.Definition.StandardRegionName ) ),
                                     new XElement( "extraRegions", from s in system.Definition.ExtraRegionNames
                                                                   where s != null
                                                                   select new XElement( "region", Utility.SafeString( s ) ) ),
                                     new XElement( "stoneLocation", Utility.SafeString( system.Definition.TownstoneLocation.ToString() ) ) ) );

            //public MidgardTowns Town { get; private set; }
            //public TextDefinition TownName { get; private set; }
            //public int LocalizedWelcomeMessage { get; private set; }
            //public string WelcomeMessage { get; private set; }
            //public string StandardRegionName { get; private set; }
            //public string[] ExtraRegionNames { get; private set; }
            //public CityInfo StartCityInfo { get; private set; }
            //public TownBanFlag BanFlag { get; private set; }
            //public Point3D TownstoneLocation { get; private set; }
        }

        private static void AppendCitizensInfoXml( XContainer element, TownSystem system )
        {
            var players = system.GetMobilesFromStates();
            if( players == null )
                element.Add( new XElement( "citizens", "- no citizen found -" ) );
            else
            {
                element.Add( new XElement( "citizens", from s in players
                                                       where s != null && !s.Deleted
                                                       select new XElement( "citizen", Utility.SafeString( s.Name ) ) ) );
            }
        }

        public static List<TownSystem> GetTownSystems( bool sortedByName )
        {
            List<TownSystem> list = new List<TownSystem>();

            foreach( TownSystem system in TownSystem.TownSystems )
                list.Add( system );

            if( sortedByName )
                list.Sort( InternalComparer.Instance );

            return list;
        }

        #region [Handlers]
        private static void HandleGetTowns( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "GetTowns command called." );

            e.CustomResultTree.Add( new XElement( "Notice",
                                                new XElement( "Towns", from t in GetTownSystems( true )
                                                                       select
                                                                           t.Definition.TownName ) ) );
        }

        public static void HandleGetTownInfo( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "GetTownInfo command called." );

            e.Exitcode = -1;

            var town = Utility.SafeGetKey( e.Args, "town" );
            if( string.IsNullOrEmpty( town ) )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidTown;
                throw new ArgumentNullException( "town" );
            }

            TownSystem sys = TownSystem.Parse( town );
            if( sys == null )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidTown;
                throw new ArgumentNullException( "town" );
            }

            try
            {
                e.CustomResultTree.Add( GetTownInfoXml( sys ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }
        #endregion

        private class InternalComparer : IComparer<TownSystem>
        {
            public static readonly IComparer<TownSystem> Instance = new InternalComparer();

            #region IComparer<TownSystem> Members
            public int Compare( TownSystem x, TownSystem y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Definition.TownName, y.Definition.TownName );
            }
            #endregion
        }
    }
}