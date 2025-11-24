namespace Server.Items
{
    [Flipable( 0x14EB, 0x14EC )]
    public class NavigatorsWorldMap : WorldMap
    {
        [Constructable]
        public NavigatorsWorldMap()
        {
            LootType = LootType.Blessed;
        }

        public NavigatorsWorldMap( Serial serial )
            : base( serial )
        {
        }

        public override int LabelNumber
        {
            get { return 1075500; } // // Navigator's World Map
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