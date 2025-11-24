using System;
using System.IO;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;

using RpvCore = Midgard.Engines.RazorRpvRecorder.Config;
using XmpRpcCore = Midgard.Engines.MyXmlRPC.Core;

namespace Midgard.Engines.RazorRpvRecorder
{
    public class WebCommands
    {
        public static void Register ()
        {
        	// http://93.63.153.178/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getRPVList
        	Core.Register ("getRPVList", new MyXmlEventHandler (OnCommand_getRPVList), null);
			Core.Register ("getRPVFile", new MyXmlEventHandler (OnCommand_getRPVFile), null);
        }

		public static void OnCommand_getRPVFile (MyXmlEventArgs e)
		{
			if (Config.Debug)
				Config.Pkg.LogInfoLine ("getRPVFile command called...");
			
			e.Exitcode = -1;
			
			try {
				string path = Path.Combine (Server.Core.BaseDirectory, Config.StoragePath);
				string rpv = Path.Combine (path, e.Args["file"]);
				if (!File.Exists (rpv))
					throw new FileNotFoundException ("RPV Video not found!", rpv);
				string xml = Path.Combine (Path.GetDirectoryName (rpv), Path.GetFileNameWithoutExtension (rpv) + ".xml");
				if (!File.Exists (xml))
					throw new FileNotFoundException ("Xml Props of RPV Video not found!", xml);
				
				XDocument xmldoc = XDocument.Load (xml);
				
				string hash = xmldoc.Element ("video").Attribute ("hash").Value;
				
				var ret = File.ReadAllBytes (rpv);
				
				var filedata = new XElement ("filedata", new XAttribute ("mimetype", "base64string"), new XAttribute ("hash", hash));
				
				filedata.Add(new XCData(Convert.ToBase64String (ret)));
				
				e.CustomResultTree.Add(filedata);
				
				e.Exitcode = 0;
			} catch (Exception warning) {
				e.Warnings.Add (warning);
			}
		}
		
        public static void OnCommand_getRPVList( MyXmlEventArgs e )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "getRPVList command called..." );

            e.Exitcode = -1;

            try
            {
                string path = Path.Combine( Server.Core.BaseDirectory, Config.StoragePath );

                foreach( string file in Directory.GetFiles( path, "*.rpv" ) )
                {
                    string rpv = file;
                    string xml = Path.Combine( Path.GetDirectoryName( rpv ), Path.GetFileNameWithoutExtension( rpv ) + ".xml" );

                    e.CustomResultTree.Add( new XElement( "videos" ) );

                    if( File.Exists( xml ) )
                    {
                        XDocument xmldoc = XDocument.Load( xml );

                        string hash = xmldoc.Element( "video" ).Attribute( "hash" ).Value;
                        e.CustomResultTree.Element( "videos" ).Add( xmldoc.Element( "video" ) );
                    }
                }

                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }
    }
}