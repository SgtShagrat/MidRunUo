/***************************************************************************
 *                               WebCommands.cs
 *
 *   begin                : 07 ottobre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;

namespace Midgard.Engines.AdvancedFishing
{
    public class WebCommands
    {
        public static void RegisterCommands()
        {
            // http://93.63.153.178/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getFishStatus
            MyXmlRPC.Core.Register( "getFishStatus", new MyXmlEventHandler( GetFishStatusOnCommand ), null );
        }

        public static void GetFishStatusOnCommand( MyXmlEventArgs e )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "GetFishStatus command called..." );

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "status", from state in FishRanks.BuildList()
                                                                where state != null
                                                                select state.ToXElement() ) );

                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }
    }
}