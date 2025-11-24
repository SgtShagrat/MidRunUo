namespace Server.Items
{
    public class FruitBasketA : BaseDecorationArtifact
    {
        [Constructable]
        public FruitBasketA()
            : base( 0x993 )
        {
        }

        public FruitBasketA( Serial serial )
            : base( serial )
        {
        }

        public override int ArtifactRarity
        {
            get { return 5; }
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