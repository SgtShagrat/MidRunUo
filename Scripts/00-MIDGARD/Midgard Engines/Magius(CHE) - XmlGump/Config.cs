using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using Midgard.Engines.Packager;
namespace Midgard.Engines.XmlGumps
{
	public class Config
	{
		public static object[] Package_Info =
        {
            "Script Title", "Xml Gump Engine",
            "Enabled by Default", true,
            "Script Version", new Version( 1, 0, 0, 0 ),
            "Author name", "Magius(CHE)",
            "Creation Date", new DateTime( 2011, 5, 13 ),
            "Author mail-contact", "cheghe@tiscali.it",
            "Author home site", "http://www.magius.it",
            //"Author notes",           null,
            "Script Copyrights", "(C) Midgard Shard - Magius(CHE)",
            "Provided packages", new string[] { "Midgard.Engines.XmlGumps" },
            /*"Required packages",       new string[]{"Midgard.Engines.SkillSystem"},*/
            //"Conflicts with packages",new string[0],
            "Research tags", new string[] { "Xml", "Gump"}
        };
		
		#region [ Initialization ]
		
		public const string DataDir = "XmlGumps";
		
#if DEBUG
		internal static bool Debug = true;
#else
		internal static bool Debug = false;
#endif
		internal static Package Pkg;
		
        public static bool Enabled { get { return Pkg.Enabled; } set { Pkg.Enabled = value; } }
		#endregion

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Midgard.Engines.XmlGumps.Config ) ];
        }
		
		private class LoadedGumpInfo
		{
			public XDocument Xml;
			public DateTime Time;
		}
		
		private static Dictionary<string,LoadedGumpInfo> LoadedGumps = new Dictionary<string, LoadedGumpInfo>();
		internal static XDocument LoadGumpXml( string gumppath )
		{
			var tot = Path.Combine("Data",Path.Combine (DataDir, gumppath));
			if (LoadedGumps.ContainsKey(tot))
			{
				var gi = LoadedGumps[tot];
				if ( File.GetLastWriteTime(tot) != gi.Time )
				{
					Pkg.LogInfoLine("Xml {0} re-loaded.",gumppath);
					gi.Time = File.GetLastWriteTime(tot);
					gi.Xml = XDocument.Load( tot );					
				}
				return gi.Xml;
			}
			else
			{
				Pkg.LogInfoLine("Xml {0} loaded.",gumppath);
				var gi = new LoadedGumpInfo();
				gi.Time = File.GetLastWriteTime(tot);
				gi.Xml = XDocument.Load( tot );	
				LoadedGumps.Add(tot,gi);
				return gi.Xml;
			}
		}
	}
}

