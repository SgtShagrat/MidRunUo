namespace Server.Items
{
    public abstract class Mead : Item
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

        public Mead( int itemID )
            : base( itemID )
        {
            FillFactor = 4;
        }

        public Mead( Serial serial )
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

    public class BottleOfGrapeMead : Mead
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfGrapeMead()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of grape mead";
        }

        public BottleOfGrapeMead( Serial serial )
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

    public class BottleOfPumpkinMead : Mead
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfPumpkinMead()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of pumpkin mead";
        }

        public BottleOfPumpkinMead( Serial serial )
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

    public class BottleOfLemonMead : Mead
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfLemonMead()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of lemon mead";
        }

        public BottleOfLemonMead( Serial serial )
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

    public class BottleOfAppleMead : Mead
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfAppleMead()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of apple mead";
        }

        public BottleOfAppleMead( Serial serial )
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

    public class BottleOfPearMead : Mead
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfPearMead()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of pear mead";
        }

        public BottleOfPearMead( Serial serial )
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

    public class BottleOfBananaMead : Mead
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfBananaMead()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of banana mead";
        }

        public BottleOfBananaMead( Serial serial )
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

    public class BottleOfWatermelonMead : Mead
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfWatermelonMead()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of watermelon mead";
        }

        public BottleOfWatermelonMead( Serial serial )
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

    public class BottleOfPeachMead : Mead
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfPeachMead()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of peach mead";
        }

        public BottleOfPeachMead( Serial serial )
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

    public class BottleOfHoneyMead : Mead
    {
        public override Item EmptyItem
        {
            get { return new Bottle(); }
        }

        [Constructable]
        public BottleOfHoneyMead()
            : base( 0x0f09 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "bottle of honey mead";
        }

        public BottleOfHoneyMead( Serial serial )
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