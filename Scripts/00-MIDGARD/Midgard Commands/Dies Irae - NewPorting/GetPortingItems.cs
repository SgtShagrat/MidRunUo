using Midgard.Items;

using Server;
using Server.Commands;
using Server.Items;

namespace Midgard.Engines.ThirdCrownPorting
{
    public class GetPortingItems
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "OggettiPorting", AccessLevel.Player, new CommandEventHandler( GetPortingItem_OnCommand ) );
        }

        [Usage( "OggettiPorting" )]
        [Description( "Crea nello zaino gli oggetti necessari al porting" )]
        private static void GetPortingItem_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            Backpack pack = e.Mobile.Backpack as Backpack;
            if( pack == null )
            {
                from.SendMessage( "Non hai uno zaino valido." );
                return;
            }
            else
            {
                pack.DropItem( new PortingBag( from.Account.Username ) );
                pack.DropItem( new OldShrinkItem() );
                pack.DropItem( new OldShrinkItem() );
                pack.DropItem( new OldShrinkItem() );

                from.SendMessage( "La sacca porting e le tre gemme sono state create nel tuo zaino. Tali oggetti sono personali e nn cedibili." );
            }
        }
    }
}