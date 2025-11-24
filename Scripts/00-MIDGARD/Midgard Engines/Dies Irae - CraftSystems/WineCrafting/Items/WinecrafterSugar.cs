namespace Midgard.Engines.WineCrafting
{
    /*
    public class WinecrafterSugar : Item, ICommodity
    {
        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} Sugar" : "{0} Sugar", Amount );
            }
        }

        int ICommodity.DescriptionNumber { get { return 0; } }

        [Constructable]
        public WinecrafterSugar()
            : this( 1 )
        {
        }

        [Constructable]
        public WinecrafterSugar( int amount )
            : base( 0xF8F )
        {
            Stackable = true;
            Hue = 1150;
            Name = "a jar of sugar";
            ItemID = 4102;
            Amount = amount;
            Weight = 1;
        }

        public WinecrafterSugar( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
  */
}