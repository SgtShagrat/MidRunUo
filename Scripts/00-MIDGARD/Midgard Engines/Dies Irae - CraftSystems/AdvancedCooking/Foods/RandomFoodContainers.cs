namespace Server.Items
{
    public class RandomFoodBarrel : Item
    {
        [Constructable]
        public RandomFoodBarrel()
            : base( Utility.RandomList( 0x3B87, 0x3B88, 0x3B89, 0x3B8A, 0x3B8B, 0x3B8C, 0x3B8D, 0x3B8E, 0x3B8F, 0x3B90 ) )
        {
            Name = "Barrel of food";
        }

        public RandomFoodBarrel( Serial serial )
            : base( serial )
        {
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

    public class RandomFoodCrate : Item
    {
        [Constructable]
        public RandomFoodCrate()
            : base(
                Utility.RandomList( 0x3BB2, 0x3BB1, 0x3BB0, 0x3BAF, 0x3BAE, 0x3BAD, 0x3BAC, 0x3BAB, 0x3BAA, 0x3BA9,
                                   0x3BA7, 0x3BA6, 0x3BA8 ) )
        {
            Name = "Crate of food";
        }

        public RandomFoodCrate( Serial serial )
            : base( serial )
        {
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

    public class RandomFoodBasket : Item
    {
        [Constructable]
        public RandomFoodBasket()
            : base( Utility.RandomList( 0x3BC8, 0x3B91, 0x3B92, 0x3B93, 0x3B94, 0x3B95, 0x3B96, 0x3B97, 0x3B98, 0x3B99,
                                      0x3B9A, 0x3B9B, 0x3B9C, 0x3B9D, 0x3B9E, 0x3B9F, 0x3BA0, 0x3BA1, 0x3BA2, 0x3BA3,
                                      0x3BA4, 0x3BA5 ) )
        {
            Name = "Basket of food";
        }

        public RandomFoodBasket( Serial serial )
            : base( serial )
        {
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