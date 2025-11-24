namespace Server.Items
{
    public class WoodenHorseStatue : Item
    {
        public override string DefaultName
        {
            get
            {
                return "wooden horse statue";
            }
        }

        [Constructable]
        public WoodenHorseStatue()
            : base( 0x3FFE )
        {
            Weight = 10.0;
        }

        public WoodenHorseStatue( Serial serial )
            : base( serial )
        {
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

            if( Name == "wooden horse statue" )
                Name = null;
        }
    }
}