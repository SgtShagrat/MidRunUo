using System;

using Midgard.Engines.Events.Items;

using XMas = Midgard.Engines.Events.Items;

using Server;
using Server.Misc;

namespace Midgard.Engines.Events
{
    public class Christmas2010 : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register( new Christmas2010() );
        }

        public override DateTime Start { get { return new DateTime( 2010, 12, 25 ); } }
        public override DateTime Finish { get { return new DateTime( 2011, 01, 06 ); } }

        public override void GiveGift( Mobile mob )
        {
            ChristmasBasket box = new ChristmasBasket();

            box.DropItem( new ChristmasCandyCane() );
            box.DropItem( new ChristmasGingerBreadCookie() );
            box.DropItem( new ChristmasHolidayBell() );
            box.DropItem( new ChristmasPileOfGlacialSnow() );
            box.DropItem( new ChristmasMistletoeDeed() );
            box.DropItem( new ChristmasFestiveCactus() );
            box.DropItem( new ChristmasSnowyTree() );
            box.DropItem( new ChristmasDecorativeTopiary() );

            box.DropItem( new ChristmasCard( string.Format( "Buon Natale da {0}", mob.Name ) ) );

            switch( GiveGift( mob, box ) )
            {
                case GiftResult.Backpack:
                    mob.SendMessage( 0x482, "Buon Natale dallo staff di Midgard! Cerca un piccolo regalo nel tuo zaino." );
                    break;
                case GiftResult.BankBox:
                    mob.SendMessage( 0x482, "Buon Natale dallo staff di Midgard! Cerca un piccolo regalo nella tua banca." );
                    break;
            }
        }
    }
}