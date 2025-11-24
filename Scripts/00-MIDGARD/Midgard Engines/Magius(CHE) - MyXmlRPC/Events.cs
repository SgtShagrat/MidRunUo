using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

using Server.Accounting;

namespace Midgard.Engines.MyXmlRPC
{
    public delegate void MyXmlEventHandler( MyXmlEventArgs e );

    public class MyXmlEventArgs : EventArgs
    {
        public readonly Dictionary<string, string> Args = new Dictionary<string, string>();
        public List<Exception> Warnings { get; private set; }
        public readonly Dictionary<string, string> Returns = new Dictionary<string, string>();
        public StringBuilder ReturnMessage = new StringBuilder();
        public object Tag;

        internal MyXmlEventArgs (XElement result, List<Exception> warnings, IEnumerable<KeyValuePair<string, string>> args, object tag, IAccount caller)
        {
        	Caller = caller;
        	Tag = tag;
        	CustomResultTree = result;
        	Warnings = warnings;

            foreach (var elem in args)
            {
				//System.Web.HttpServerUtility.UrlTokenDecode(
                Args.Add( elem.Key, elem.Value );
            }
        }

        public XElement CustomResultTree { get; private set; }
        public IAccount Caller { get; private set; }
        public int Exitcode { get; set; }
    }
}