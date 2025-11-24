/***************************************************************************
 *                               Christmas2008
 *                            -------------------
 *   begin                : 24 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Items.MusicBox;

using Server;
using Server.Items;
using Server.Misc;
using Midgard.Items;
using Xmas2008 = Server.Items.Christmas2008;

namespace Midgard.Misc
{
    public class Christmas2008 : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register( new Christmas2008() );
        }

        public override DateTime Start { get { return new DateTime( 2008, 12, 25 ); } }
        public override DateTime Finish { get { return new DateTime( 2008, 12, 26 ); } }

        public override void GiveGift( Mobile mob )
        {
            ChristmasBasket2008 box = new ChristmasBasket2008();

            box.Name = "Natale 2008";
            box.Hue = Utility.RandomBlueHue();

            DawnsMusicBox musicBox = new DawnsMusicBox();
            musicBox.Name = "Buon Natale 2008!";
            box.DropItem( musicBox );
            box.DropItem( new MusicBoxGears() );

            box.DropItem( new Xmas2008.CandyCane() );
            box.DropItem( new Xmas2008.GingerBreadCookie() );
            box.DropItem( new Xmas2008.MistletoeDeed2008() );
            box.DropItem( new Xmas2008.PileOfGlacialSnow2008() );
            box.DropItem( new Xmas2008.SnowPile2008() );

            box.DropItem( new ChristmasCard2008( string.Format( "Buon Natale da {0}", mob.Name ) ) );

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

namespace Midgard.Items
{
    public class ChristmasBasket2008 : Container
    {
        [Constructable]
        public ChristmasBasket2008()
            : base( 0x990 )
        {
            Name = "Christmas Basket";
            Hue = Utility.RandomList( 0x36, 0x17, 0x120 );
            Weight = 1.0;
        }

        public ChristmasBasket2008( Serial serial )
            : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Buon Natale 2008 dallo Staff di Midgard!" );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class ChristmasCard2008 : Item
    {
        [Constructable]
        public ChristmasCard2008( string name )
            : base( 0x14EF )
        {
            Name = name;
            Hue = Utility.RandomList( 0x36, 0x17, 0x120 );
            Weight = 1.0;
        }

        public ChristmasCard2008( Serial serial )
            : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Buon Natale 2008 dallo Staff di Midgard!" );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}