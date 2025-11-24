using Server;
using Server.Items;
using Server.Targeting;
using Midgard.Engines.AdvancedCooking;

namespace Midgard.Items
{
    public class Corn : Item
    {
        [Constructable]
        public Corn()
            : this( 1 )
        {
        }

        [Constructable]
        public Corn( int amount )
            : base( 0x0C81 )
        {
            Name = "An Ear of Corn";
            Stackable = true;
            Weight = 4.0;
            Amount = amount;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            from.Target = new InternalTarget( this );
        }

        public Corn( Serial serial )
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

        private class InternalTarget : Target
        {
            private Corn m_Item;

            public InternalTarget( Corn item )
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
                        from.SendMessage( "You got a sack of cornflour." );
                        from.AddToBackpack( new SackcornFlour() );
                    }
                    else
                        from.SendMessage( "You need more ears of corn." );
                }
            }
        }
    }

    public class EdibleSun : Food
    {
        [Constructable]
        public EdibleSun()
            : this( 1 )
        {
        }

        [Constructable]
        public EdibleSun( int amount )
            : base( amount, 0xF27 )
        {
            Weight = 0.1;
            Stackable = true;
            FillFactor = 1;
            Hue = 0xF7E;
            Name = "Sunflower Seeds";
        }

        public EdibleSun( Serial serial )
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

    public class FieldCorn : Item
    {
        [Constructable]
        public FieldCorn()
            : this( 1 )
        {
        }

        [Constructable]
        public FieldCorn( int amount )
            : base( 0xC81 )
        {
            Name = "Field Corn";
            Amount = amount;
            Weight = 3.0;
            Hue = 0x1C5;
            Stackable = true;
        }

        public FieldCorn( Serial serial )
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
            private FieldCorn m_Item;

            public InternalTarget( FieldCorn item )
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
                    if( m_Item.Amount >= 10 )
                    {
                        m_Item.Consume( 10 );
                        from.SendMessage( "You made a bag of cornmeal" );
                        from.AddToBackpack( new BagOfCornmeal() );
                    }
                    else
                        from.SendMessage( "You don't have enough field corn." );
                }
            }
        }
    }

    /* TeaLeaf
    public class TeaLeaf : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} tealeaf" : "{0} tealeaves", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public TeaLeaf()
            : this( 1 )
        {
        }

        [Constructable]
        public TeaLeaf( int amount )
            : base( 0x103F )
        {
            Name = "TeaLeaf";
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public TeaLeaf( Serial serial )
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

    /* TeaLeaves
    public class TeaLeaves : Item
    {
        [Constructable]
        public TeaLeaves()
            : this( 1 )
        {
        }

        [Constructable]
        public TeaLeaves( int amount )
            : base( 0x1AA2 )
        {
            Weight = 0.1;
            Stackable = true;
            Amount = amount;
            Hue = 0x44;
            Name = "Tea Leaves";
        }

        public TeaLeaves( Serial serial )
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
}