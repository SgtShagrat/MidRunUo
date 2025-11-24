namespace Server.Items
{
    public class TallCandle : BaseDecorationArtifact
    {
        [Constructable]
        public TallCandle()
            : base( 0x0B1A )
        {
            Light = LightType.Circle150;
        }

        public TallCandle( Serial serial )
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