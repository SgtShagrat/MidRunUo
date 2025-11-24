using Midgard.Engines.AdvancedCooking;
using Server;
using Server.Items;
using Server.Targeting;

namespace Midgard.Items
{
    /* Hay
    public class Hay : Item
    {
        [Constructable]
        public Hay()
            : this( 1 )
        {
        }

        [Constructable]
        public Hay( int amount )
            : base( 0xF36 )
        {
            Name = "Hay Sheath";
            Weight = 4.0;
            Stackable = true;
            Amount = amount;
        }

        public Hay( Serial serial )
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
    */

    public class OatSheath : Item
    {
        [Constructable]
        public OatSheath()
            : this( 1 )
        {
        }

        [Constructable]
        public OatSheath( int amount )
            : base( 0x1EBD )
        {
            Amount = amount;
            Weight = 3.0;
            Stackable = true;
            Name = "Oat Sheath";
        }

        public OatSheath( Serial serial )
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

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            from.Target = new InternalTarget( this );
        }

        private class InternalTarget : Target
        {
            private OatSheath m_Item;

            public InternalTarget( OatSheath item )
                : base( 1, false, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Item.Deleted )
                    return;

                else if( FoodHelper.IsFlourMill( targeted ) )
                {
                    if( m_Item.Amount >= 8 )
                    {
                        m_Item.Consume( 8 );
                        from.SendMessage( "You made a bag of oats" );
                        from.AddToBackpack( new BagOfOats() );
                    }
                    else
                        from.SendMessage( "You don't have enough oats." );
                }
            }
        }
    }

    public class RiceSheath : Item
    {
        [Constructable]
        public RiceSheath()
            : this( 1 )
        {
        }

        [Constructable]
        public RiceSheath( int amount )
            : base( 0x1A9D )
        {
            Amount = amount;
            Weight = 0.1;
            Hue = 0x2FE;
            Stackable = true;
            Name = "Rice Sheath";
        }

        public RiceSheath( Serial serial )
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

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            from.Target = new InternalTarget( this );
        }

        private class InternalTarget : Target
        {
            private RiceSheath m_Item;

            public InternalTarget( RiceSheath item )
                : base( 1, false, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Item.Deleted )
                    return;

                else if( FoodHelper.IsFlourMill( targeted ) )
                {
                    if( m_Item.Amount >= 20 )
                    {
                        m_Item.Consume( 20 );
                        from.SendMessage( "You made a bag of ricemeal." );
                        from.AddToBackpack( new BagOfRicemeal() );
                    }
                    else
                        from.SendMessage( "You don't have enough rice sheathes." );
                }
            }
        }
    }

    public class Sugarcane : Item
    {
        [Constructable]
        public Sugarcane()
            : this( 1 )
        {
        }

        [Constructable]
        public Sugarcane( int amount )
            : base( 0x1A9D )
        {
            Amount = amount;
            Weight = 0.1;
            Hue = 0x23F;
            Stackable = true;
            Name = "Sugarcane";
        }

        public Sugarcane( Serial serial )
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

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            from.Target = new InternalTarget( this );
        }

        private class InternalTarget : Target
        {
            private Sugarcane m_Item;

            public InternalTarget( Sugarcane item )
                : base( 1, false, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Item.Deleted )
                    return;

                else if( FoodHelper.IsFlourMill( targeted ) )
                {
                    if( m_Item.Amount >= 20 )
                    {
                        m_Item.Consume( 20 );
                        from.SendMessage( "You made a bag of sugar." );
                        from.AddToBackpack( new BagOfSugar() );
                    }
                    else
                        from.SendMessage( "You don't have enough sugarcane." );
                }
            }
        }
    }

    public class Wheat : Item
    {
        [Constructable]
        public Wheat()
            : this( 1 )
        {
        }

        [Constructable]
        public Wheat( int amount )
            : base( 0x1EBD )
        {
            Amount = amount;
            Weight = 3.0;
            Stackable = true;
        }

        public Wheat( Serial serial )
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

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            from.Target = new InternalTarget( this );
        }

        private class InternalTarget : Target
        {
            private Wheat m_Item;

            public InternalTarget( Wheat item )
                : base( 1, false, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Item.Deleted )
                    return;

                else if( FoodHelper.IsFlourMill( targeted ) )
                {
                    if( m_Item.Amount >= 4 )
                    {
                        m_Item.Consume( 4 );
                        from.SendMessage( "You made a sack of flour" );
                        from.AddToBackpack( new SackFlour() );
                    }
                    else
                        from.SendMessage( "You don't have enough wheat" );
                }
            }
        }
    }
}