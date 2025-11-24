namespace Server.Items
{
    public class BasketRegular : BaseDecorationContainerArtifact
    {
        [Constructable]
        public BasketRegular()
            : base( 0x0990 )
        {
        }

        public BasketRegular( Serial serial )
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

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}