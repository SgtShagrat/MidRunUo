namespace Server.Items
{
    [Flipable( 0x9AA, 0xE7D )]
    public class HeartwoodChest : LockableContainer
    {
        [Constructable]
        public HeartwoodChest()
            : base( 0x9AA )
        {
            Hue = 1193;
            Weight = 4;

            KeyValue = Key.RandomValue();
            DropItem( new HeartwoodChestKey( KeyValue, this ) );
        }

        public HeartwoodChest( Serial serial )
            : base( serial )
        {
        }

        public override int DefaultGumpID
        {
            get { return 0x43; }
        }

        public override int DefaultDropSound
        {
            get { return 0x42; }
        }

        public override Rectangle2D Bounds
        {
            get { return new Rectangle2D( 16, 51, 168, 73 ); }
        }

        public override int LabelNumber
        {
            get { return 1075503; }
        } // HeartwoodChest

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class HeartwoodChestKey : Key
    {
        [Constructable]
        public HeartwoodChestKey( uint keyvalue, Item link )
            : base( KeyType.Copper, keyvalue )
        {
            LootType = LootType.Blessed;
            Link = link;
        }

        public HeartwoodChestKey( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}