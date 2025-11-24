using System;
using System.Collections.Generic;
using Server;
using Server.Network;

namespace Midgard.Engines.TownHouses
{
    public class Errors
    {
        static Errors()
        {
            ErrorLog = new List<string>();
            Checked = new List<Mobile>();
        }

        public static List<string> ErrorLog { get; private set; }
        public static List<Mobile> Checked { get; private set; }

        public static void RegisterCommands()
        {
            Commands.AddCommand( "TownHouseErrors", AccessLevel.Counselor, new TownHouseCommandHandler( OnErrors ) );

            EventSink.Login += new LoginEventHandler( OnLogin );
        }

        private static void OnErrors( CommandInfo e )
        {
            if( string.IsNullOrEmpty( e.ArgString ) )
                new ErrorsGump( e.Mobile );
            else
                Report( e.ArgString + " - " + e.Mobile.Name );
        }

        private static void OnLogin( LoginEventArgs e )
        {
            if( e.Mobile.AccessLevel != AccessLevel.Player
            && ErrorLog.Count != 0
            && !Checked.Contains( e.Mobile ) )
                new ErrorsNotifyGump( e.Mobile );
        }

        public static void Report( string error )
        {
            ErrorLog.Add( String.Format( "<B>{0}</B><BR>{1}<BR>", DateTime.Now, error ) );

            Checked.Clear();

            Notify();
        }

        private static void Notify()
        {
            foreach( NetState state in NetState.Instances )
            {
                if( state.Mobile == null )
                    continue;

                if( state.Mobile.AccessLevel != AccessLevel.Player )
                    Notify( state.Mobile );
            }
        }

        public static void Notify( Mobile m )
        {
            if( m.HasGump( typeof( ErrorsGump ) ) )
                new ErrorsGump( m );
            else
                new ErrorsNotifyGump( m );
        }
    }
}