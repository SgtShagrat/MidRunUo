namespace Server.Items
{
    public abstract class Juice : Item
    {
        private Mobile m_Poisoner;
        private Poison m_Poison;
        private int m_FillFactor;

        public virtual Item EmptyItem
        {
            get { return null; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Poisoner
        {
            get { return m_Poisoner; }
            set { m_Poisoner = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Poison Poison
        {
            get { return m_Poison; }
            set { m_Poison = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int FillFactor
        {
            get { return m_FillFactor; }
            set { m_FillFactor = value; }
        }

        public Juice( int itemID )
            : base( itemID )
        {
            FillFactor = 4;
        }

        public Juice( Serial serial )
            : base( serial )
        {
        }

        public virtual void Drink( Mobile from )
        {
            int bac = 5;
            if( Thirsty( from, m_FillFactor ) )
            {
                // Play a random drinking sound
                from.PlaySound( Utility.Random( 0x30, 2 ) );

                if( from.Body.IsHuman && !from.Mounted )
                    from.Animate( 34, 5, 1, true, false, 0 );

                if( m_Poison != null )
                    from.ApplyPoison( m_Poisoner, m_Poison );

                from.BAC += bac;
                if( from.BAC > 60 )
                    from.BAC = 60;

                BaseBeverage.CheckHeaveTimer( from );

                Consume();

                Item item = EmptyItem;

                if( item != null )
                    from.AddToBackpack( item );
            }
        }

        public static bool Thirsty( Mobile from, int fillFactor )
        {
            if( from.Thirst >= 20 )
            {
                from.SendMessage( "You are simply too full to drink any more!" );
                return false;
            }

            int iThirst = from.Thirst + fillFactor;
            if( iThirst >= 20 )
            {
                from.Thirst = 20;
                from.SendMessage( "You manage to drink the beverage, but you are full!" );
            }
            else
            {
                from.Thirst = iThirst;

                if( iThirst < 5 )
                    from.SendMessage( "You drink the beverage, but are still extremely thirsty." );
                else if( iThirst < 10 )
                    from.SendMessage( "You drink the beverage, and begin to feel more satiated." );
                else if( iThirst < 15 )
                    from.SendMessage( "After drinking the beverage, you feel much less thirsty." );
                else
                    from.SendMessage( "You feel quite full after consuming the beverage." );
            }

            return true;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            if( from.InRange( GetWorldLocation(), 1 ) )
                Drink( from );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version

            writer.Write( m_Poisoner );

            Server.Poison.Serialize( m_Poison, writer );
            writer.Write( m_FillFactor );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        m_Poisoner = reader.ReadMobile();

                        goto case 0;
                    }
                case 0:
                    {
                        m_Poison = Server.Poison.Deserialize( reader );
                        m_FillFactor = reader.ReadInt();
                        break;
                    }
            }
        }
    }

    public class BottleOfGrapeJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfGrapeJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of grape juice";
        }

        public BottleOfGrapeJuice( Serial serial )
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

    public class BottleOfOrangeJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfOrangeJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of orange juice";
        }

        public BottleOfOrangeJuice( Serial serial )
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

    public class BottleOfPumpkinJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfPumpkinJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of pumpkin juice";
        }

        public BottleOfPumpkinJuice( Serial serial )
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

    public class BottleOfLemonJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfLemonJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of lemon juice";
        }

        public BottleOfLemonJuice( Serial serial )
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

    public class BottleOfGrapeFruitJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfGrapeFruitJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of grape fruit juice";
        }

        public BottleOfGrapeFruitJuice( Serial serial )
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

    public class BottleOfAppleJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfAppleJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of apple juice";
        }

        public BottleOfAppleJuice( Serial serial )
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

    public class BottleOfPearJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfPearJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of pear juice";
        }

        public BottleOfPearJuice( Serial serial )
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

    public class BottleOfBananaJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfBananaJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of banana juice";
        }

        public BottleOfBananaJuice( Serial serial )
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

    public class BottleOfPruneJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfPruneJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of prune juice";
        }

        public BottleOfPruneJuice( Serial serial )
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

    public class BottleOfWatermelonJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfWatermelonJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of watermelon juice";
        }

        public BottleOfWatermelonJuice( Serial serial )
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

    public class BottleOfPeachJuice : Juice
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfPeachJuice()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of peach juice";
        }

        public BottleOfPeachJuice( Serial serial )
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