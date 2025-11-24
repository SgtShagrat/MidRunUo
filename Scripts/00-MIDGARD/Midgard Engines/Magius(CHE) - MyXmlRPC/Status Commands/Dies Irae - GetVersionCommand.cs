/***************************************************************************
 *                               Dies Irae - GetVersionCommand.cs
 *
 *   begin                : 14 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

namespace Midgard.Engines.MyXmlRPC
{
    public class GetVersionCommand
    {
        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getVersion
            Core.Register( "getVersion", new MyXmlEventHandler( GetVersionOnCommand ), null );
        }

        public static void GetVersionOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "GetVersion command called..." );

            e.Returns.Add( "Version", "" + Core.Pkg.Version );
            e.Exitcode = 0;
        }
    }
}