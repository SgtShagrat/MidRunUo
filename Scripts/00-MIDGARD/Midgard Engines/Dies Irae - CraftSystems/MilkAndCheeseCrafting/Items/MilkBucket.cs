using System;
using Midgard.Engines.CheeseCrafting;
using Midgard.Engines.AdvancedCooking;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.CheeseCrafting
{
    public class MilkBucket : Item
    {
        public int m_Milk;
        public MilkTypes m_MilkType;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsFull
        {
            get { return m_Milk >= 50; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Milk
        {
            get { return m_Milk; }
            set
            {
                m_Milk = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public MilkTypes MilkType
        {
            get { return m_MilkType; }
            set
            {
                m_MilkType = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public MilkBucket()
            : base( 0x0FFA )
        {
            Weight = 10.0;
            Name = "Milk Bucket";
            Hue = 1001;
        }

        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            if( m_MilkType != MilkTypes.None && m_Milk > 0 )
            {
                list.Add( 1060658, "Type\t{0}", CheeseCrafingHelper.GetMilkName( m_MilkType ) ); // ~1_val~ ~2_val~
                list.Add( 1060659, "Milk liters\t{0}/50", m_Milk ); // ~1_val~ ~2_val~
            }
            else
                list.Add( 1075613 ); // (Empty)
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !from.InRange( GetWorldLocation(), 1 ) )
            {
                from.LocalOverheadMessage( MessageType.Regular, 906, 1019045 );
            }
            else
            {
                from.Target = new InternalTarget( this );
                from.SendMessage( 0x96D, "What do you want to use that with?" );
            }
        }

        public virtual void DoMilkHarvesting( Mobile creature, Mobile from )
        {
            if( creature is Sheep || creature is Goat || creature is Cow )
            {
                Type t = creature.GetType();
                MilkTypes milkType = CheeseCrafingHelper.GetMilkType( t );

                if( m_MilkType == MilkTypes.None )
                    m_MilkType = CheeseCrafingHelper.GetMilkType( t );

                if( milkType == m_MilkType )
                {
                    if( !IsFull )
                    {
                        if( creature.Stam > 3 )
                        {
                            ++m_Milk;

                            creature.Stam = creature.Stam - 3;

                            InvalidateProperties();

                            from.PlaySound( 0X4D1 );
                            from.SendMessage( 0x96D,
                                             String.Format( "You obtain 1 liter of {0} milk.", CheeseCrafingHelper.GetMilkName( m_MilkType ) ) );
                        }
                        else
                            from.SendMessage( 0x84B, "This animal is too tired to give more milk!" );
                    }
                    else
                        from.SendMessage( 0x84B, "This bucklet is already full." );
                }
                else
                {
                    from.SendMessage( 0x84B, "There is already some different milk in this bucklet." );
                    from.CloseGump( typeof( MilkSystemHelperGump ) );
                    from.SendGump( new MilkSystemHelperGump() );
                }
            }
            else
            {
                from.SendMessage( 0x96D, "You can obtain milk only from sheep, goats or cows!" );
                from.CloseGump( typeof( MilkSystemHelperGump ) );
                from.SendGump( new MilkSystemHelperGump() );
            }
        }

        public virtual void DoMilkBottle( Mobile to )
        {
            if( m_Milk > 1 )
            {
                if( to.Backpack != null && to.Backpack.ConsumeTotal( typeof( Bottle ), 1 ) )
                {
                    m_Milk--;

                    if( m_Milk <= 0 )
                        MilkType = MilkTypes.None;

                    InvalidateProperties();

                    FoodHelper.GiveFood( to, CheeseCrafingHelper.GetMilkBottle( m_MilkType, to, Utility.RandomDouble() < 0.1 ),
                                        String.Format( "You fill a bottle with {0} milk.",
                                        CheeseCrafingHelper.GetMilkName( m_MilkType ) ) );

                    to.PlaySound( 0X240 );
                }
                else
                    to.SendMessage( 0x96D, "You need a bootle in your pack to do that." );
            }
            else
            {
                to.SendMessage( 0x96D, "That bucklet is empty." );
                to.CloseGump( typeof( MilkSystemHelperGump ) );
                to.SendGump( new MilkSystemHelperGump() );
            }
        }

        public virtual void DoFillBeverage( Mobile to, BaseBeverage beverage )
        {
            if( m_Milk >= beverage.MaxQuantity )
            {
                if( beverage.Quantity == 0 )
                {
                    if( m_MilkType != MilkTypes.None )
                    {
                        beverage.Content = BeverageType.Milk;
                        beverage.Quantity = beverage.MaxQuantity;
                        m_Milk -= beverage.MaxQuantity;

                        InvalidateProperties();

                        to.SendMessage( 0x96D, String.Format( "You fill the container with {0} milk.", CheeseCrafingHelper.GetMilkName( m_MilkType ) ) );
                        to.PlaySound( 0X240 );

                        if( m_Milk <= 0 )
                        {
                            m_Milk = 0;
                            m_MilkType = 0;
                        }
                    }
                    else
                    {
                        to.SendMessage( 0x84B, "That bucklet has not a defined cheese type." );
                        to.CloseGump( typeof( MilkSystemHelperGump ) );
                        to.SendGump( new MilkSystemHelperGump() );
                    }
                }
                else
                    to.SendMessage( 0x84B, "There is already some liquid in that container" );
            }
            else
                to.SendMessage( 0x84B, "the milk bucket doesn't have enough milk left in it." );
        }

        public virtual void DoFillCheeseForm( Mobile to, CheeseForm cheeseForm )
        {
            if( m_Milk >= 15 )
            {
                if( !cheeseForm.HasMilk )
                {
                    if( cheeseForm.MilkType == MilkTypes.None )
                        cheeseForm.MilkType = m_MilkType;

                    m_Milk -= 15;
                    cheeseForm.HasMilk = true;

                    InvalidateProperties();

                    if( m_Milk <= 0 )
                        m_MilkType = MilkTypes.None;
                }
                else
                {
                    to.SendMessage( 0x84C, "The cheese form has already milk in it." );
                }
            }
            else
            {
                to.SendMessage( 0x84C, "The milk bucket didn't contain enough milk." );
                to.CloseGump( typeof( CheeseFormHelpGump ) );
                to.SendGump( new CheeseFormHelpGump() );
            }
        }

        #region serialization
        public MilkBucket( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
            writer.Write( m_Milk );
            writer.Write( (int)m_MilkType );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
            switch( version )
            {
                case 0:
                    {
                        m_Milk = reader.ReadInt();
                        m_MilkType = (MilkTypes)reader.ReadInt();
                        break;
                    }
            }
        }
        #endregion

        private class InternalTarget : Target
        {
            private MilkBucket m_Bucket;
            private Mobile m_Creature;

            public InternalTarget( MilkBucket bucket )
                : base( 2, false, TargetFlags.None )
            {
                m_Bucket = bucket;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is Mobile )
                    m_Creature = (Mobile)o;

                if( m_Bucket == null )
                    return;

                if( m_Creature != null )
                {
                    m_Bucket.DoMilkHarvesting( m_Creature, from );
                }
                else if( ( o is Bottle ) )
                {
                    m_Bucket.DoMilkBottle( from );
                }
                else if( ( o is BaseBeverage ) )
                {
                    m_Bucket.DoFillBeverage( from, (BaseBeverage)o );
                }

                else if( o is CheeseForm )
                {
                    m_Bucket.DoFillCheeseForm( from, (CheeseForm)o );
                }
                else
                {
                    from.CloseGump( typeof( MilkSystemHelperGump ) );
                    from.SendGump( new MilkSystemHelperGump() );
                }
            }
        }
    }
}