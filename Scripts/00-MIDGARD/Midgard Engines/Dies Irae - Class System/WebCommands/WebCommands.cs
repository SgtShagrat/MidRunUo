using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;

namespace Midgard.Engines.Classes
{
    public class WebCommands
    {
        public static void RegisterCommands()
        {
            // http://93.63.153.178/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getClassStatus
            Core.Register( "getClassStatus", new MyXmlEventHandler( GetClassStatusOnCommand ), null );
        }

        public static void GetClassStatusOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "GetClassStatus command called..." );

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "status", from state in BuildList()
                                                                where state != null
                                                                select state.ToXElement() ) );

                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        public static List<ClassPlayerState> BuildList()
        {
            List<ClassPlayerState> states = new List<ClassPlayerState>();

            foreach( ClassSystem system in ClassSystem.ClassSystems )
            {
                foreach( ClassPlayerState state in system.Players )
                {
                    if( state.Mobile != null )
                        states.Add( state );
                }
            }

            return states;
        }
    }
}