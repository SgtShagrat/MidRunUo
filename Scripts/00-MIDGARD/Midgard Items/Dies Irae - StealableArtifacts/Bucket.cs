namespace Server.Items
{
    public class Bucket : BaseDecorationArtifact
    {
        [Constructable]
        public Bucket()
            : base( 0x14E0 )
        {
        }

        public Bucket( Serial serial )
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