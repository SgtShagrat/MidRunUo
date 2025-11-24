/***************************************************************************
 *                               PendingAccount.cs
 *
 *   begin                : 11 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Xml.Linq;

using Server.Accounting;
using Server.Misc;

namespace Midgard.Engines.RemoteAccounting
{
    public class PendingAccount
    {
        private XElement Source;
        public const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";

        public PendingAccount( string user, string mail )
            : this( user, mail, false, DateTime.Now )
        {
        }

        public PendingAccount( string user, string mail, bool activated, DateTime requestedat )
            : this( new XElement( "Account",
                new XAttribute( "User", user ),
                new XAttribute( "Mail", mail ),
                new XAttribute( "Activated", activated ),
                new XAttribute( "RequestedAt", requestedat.ToString( DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture )
                ) ) )
        {
        }

        internal XElement SourceXElement
        {
            get
            {
                return Source;
            }
        }

        public PendingAccount( XElement source )
        {
            Source = source;
            PendingAccounts.Register( this );
        }

        public string User { get { return Source.Attribute( "User" ).Value; } }
        public string Mail { get { return Source.Attribute( "Mail" ).Value; } }
        public DateTime RequestAt { get { return DateTime.ParseExact( Source.Attribute( "RequestedAt" ).Value, DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture ); } }

        public bool Activated
        {
            get { return bool.Parse( Source.Attribute( "Activated" ).Value ); }
            set
            {
                Source.Attribute( "Activated" ).Value = value.ToString();
                if( value && Source.Parent != null )
                    Source.Remove(); //remove from parent :-)
            }
        }

        public bool IsValid
        {
            get
            {
                if( string.IsNullOrEmpty( Mail ) || !Email.IsValid( Mail ) )
                {
                    Console.WriteLine( "Invalid email." );
                    return false;
                }

                if( Accounts.GetAccount( User ) != null )
                {
                    Console.WriteLine( "Account already exist" );
                    return false;
                }

                return true;
            }
        }

        public override string ToString()
        {
            return String.Format( "user: {0} mail: {1} activated: {2}", User, Mail, Activated );
        }

        internal XElement GetElementToSend()
        {
            var ret = new XElement( Source );
            ret.Add( new XAttribute( "PassedSeconds", (int)DateTime.Now.Subtract( RequestAt ).TotalSeconds ) );
            return ret;
        }

        internal void Destroy()
        {
            if( Source.Parent != null )
                Source.Remove();
        }
    }
}