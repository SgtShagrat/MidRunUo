using Server.Commands;
using Server.Mobiles;

namespace Server.Engines.XmlPoints
{
    public class TogglePvpStatusDisplayCommand
    {
        [Usage( "TogglePvpStatusDisplay" )]
        [Description( "Toggles the state of pvp tags under player name." )]
        public static void TogglePvpStatusDisplay_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null || e.Length != 0 )
                return;

            Midgard2PlayerMobile from = e.Mobile as Midgard2PlayerMobile;
            if( from != null )
            {
                if( from.DisplayPvpStatus )
                    from.SendMessage( "You have chosen to hide your combact status." );
                else
                    from.SendMessage( "You have chosen to display your combact status." );

                from.SendLocalizedMessage( 1064252 ); // You will be disconnected to update your status.

                from.DisplayPvpStatus = !from.DisplayPvpStatus;
            }
        }
    }
}
