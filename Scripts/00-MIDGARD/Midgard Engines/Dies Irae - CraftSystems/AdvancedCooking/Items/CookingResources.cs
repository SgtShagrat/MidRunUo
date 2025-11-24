using Midgard.Engines.CheeseCrafting;
using Midgard.Engines.AdvancedCooking;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class Dough : Item
    {
        private int m_MinSkill;
        private int m_MaxSkill;
        private Food m_CookedFood;

        public int MinSkill
        {
            get { return m_MinSkill; }
        }

        public int MaxSkill
        {
            get { return m_MaxSkill; }
        }

        public Food CookedFood
        {
            get { return m_CookedFood; }
        }

        [Constructable]
        public Dough()
            : base( 0x103d )
        {
            m_MinSkill = 0;
            m_MaxSkill = 10;
            m_CookedFood = new BreadLoaf();
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            from.Target = new InternalTarget( this );
        }

        #region serialization

        public Dough( Serial serial )
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

        #endregion

        private class InternalTarget : Target
        {
            private Dough m_Item;

            public InternalTarget( Dough item )
                : base( 1, false, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Item.Deleted )
                    return;

                if( targeted is Item && !( (Item)targeted ).Movable )
                    return;

                if( FoodHelper.IsHeatSource( targeted ) )
                {
                    if( from.BeginAction( typeof( Item ) ) )
                    {
                        m_Item.Consume();
                        from.PlaySound( 0x225 );

                        FoodHelper.InternalTimer t = new FoodHelper.InternalTimer( from, targeted as IPoint3D, from.Map,
                                                                                  m_Item.MinSkill, m_Item.MaxSkill,
                                                                                  m_Item.CookedFood );
                        t.Start();
                    }
                    else
                    {
                        from.SendLocalizedMessage( 500119 );
                    }
                }
                else if( targeted is Eggs )
                {
                    m_Item.Consume();
                    ( (Eggs)targeted ).Consume();
                    FoodHelper.GiveFood( from, new UnbakedQuiche(), "You made an unbaked quiche" );
                    FoodHelper.GiveFood( from, new Eggshells( m_Item.Hue ) );
                }
                else if( targeted is CheeseWedgeSmall )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UncookedPizza( "cheese" ), "You made an uncooked cheese pizza" );
                    ( (CheeseWedgeSmall)targeted ).Consume();
                }
                else if( targeted is JarHoney )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new SweetDough(), "You made a sweet dough" );
                    ( (JarHoney)targeted ).Consume();
                }
                else if( targeted is ChickenLeg || targeted is RawChickenLeg )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UnbakedChickenPotPie(), "You made a chicken pot pie" );
                    ( (Item)targeted ).Consume();
                }
                else if( targeted is Apple )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UnbakedApplePie(), "You made an unbaked apple pie" );
                    ( (Apple)targeted ).Consume();
                }
                else if( targeted is Peach )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UnbakedPeachCobbler(), "You made an unbaked peach cobbler" );
                    ( (Peach)targeted ).Consume();
                }
                else if( targeted is Pumpkin )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UnbakedPumpkinPie(), "You made an unbaked pumpkin pie" );
                    ( (Pumpkin)targeted ).Consume();
                }
                else if( targeted is Lime )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UnbakedKeyLimePie(), "You made an unbaked key lime pie" );
                    ( (Lime)targeted ).Consume();
                }
                else if( targeted is Dough )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UncookedFrenchBread(), "You ... add some more dough onto the dough" );
                    ( (Dough)targeted ).Consume();
                }
                else if( targeted is UncookedFrenchBread )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UncookedDonuts(),
                                        "You fumble around for a bit with even more dough, and eventually make these round doughy things" );
                    ( (UncookedFrenchBread)targeted ).Consume();
                }
                else if( targeted is CowRawCheeseWedgeSmall )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UncookedPizza( "cheese" ), "You made an uncooked cheese pizza" );
                    ( (CowRawCheeseWedgeSmall)targeted ).Consume();
                }
                else if( targeted is SheepRawCheeseWedgeSmall )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UncookedPizza( "sheep cheese" ),
                                        "You made an uncooked sheep cheese pizza" );
                    ( (SheepRawCheeseWedgeSmall)targeted ).Consume();
                }
                else if( targeted is GoatRawCheeseWedgeSmall )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new UncookedPizza( "goat cheese" ), "You made an uncooked goat cheese pizza" );
                    ( (GoatRawCheeseWedgeSmall)targeted ).Consume();
                }
            }
        }
    }

    public class SweetDough : Item
    {
        public override int LabelNumber
        {
            get { return 1041340; }
        }

        private int m_MinSkill;
        private int m_MaxSkill;
        private Food m_CookedFood;

        public int MinSkill
        {
            get { return m_MinSkill; }
        }

        public int MaxSkill
        {
            get { return m_MaxSkill; }
        }

        public Food CookedFood
        {
            get { return m_CookedFood; }
        }

        [Constructable]
        public SweetDough()
            : base( 0x103d )
        {
            Hue = 51;
            m_MinSkill = 5;
            m_MaxSkill = 20;
            m_CookedFood = new Muffins( 3 );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            from.Target = new InternalTarget( this );
        }

        #region serialization

        public SweetDough( Serial serial )
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

        #endregion

        private class InternalTarget : Target
        {
            private SweetDough m_Item;

            public InternalTarget( SweetDough item )
                : base( 1, false, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Item.Deleted )
                    return;

                if( targeted is Item && !( (Item)targeted ).Movable )
                    return;


                if( targeted is BowlFlour )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new CakeMix(), "You made a cake mix" );
                    ( (BowlFlour)targeted ).Use( from );
                }

                else if( targeted is JarHoney )
                {
                    m_Item.Consume();
                    FoodHelper.GiveFood( from, new CookieMix(), "You made a cookie mix" );
                    ( (JarHoney)targeted ).Consume();
                }
                else if( FoodHelper.IsHeatSource( targeted ) )
                {
                    if( from.BeginAction( typeof( Item ) ) )
                    {
                        m_Item.Consume();
                        from.PlaySound( 0x225 );

                        FoodHelper.InternalTimer t = new FoodHelper.InternalTimer( from, targeted as IPoint3D, from.Map,
                                                                                  m_Item.MinSkill, m_Item.MaxSkill,
                                                                                  m_Item.CookedFood );
                        t.Start();
                    }
                    else
                    {
                        from.SendLocalizedMessage( 500119 );
                    }
                }
            }
        }
    }

    public class BowlFlour : Item, IUsesRemaining
    {
        private int m_Uses;

        public bool ShowUsesRemaining
        {
            get { return true; }
            set { }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int UsesRemaining
        {
            get { return m_Uses; }
            set
            {
                m_Uses = value;
                InvalidateProperties();
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1060584, m_Uses.ToString() );
        }

        public virtual void DisplayDurabilityTo( Mobile m )
        {
            LabelToAffix( m, 1017323, AffixType.Append, ": " + m_Uses );
        }

        public override void OnSingleClick( Mobile from )
        {
            DisplayDurabilityTo( from );

            base.OnSingleClick( from );
        }

        [Constructable]
        public BowlFlour()
            : this( 10 )
        {
        }

        [Constructable]
        public BowlFlour( int StartingUses )
            : base( 0xa1e )
        {
            Weight = 2.0;
            m_Uses = StartingUses;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            from.Target = new InternalTarget( this );
        }

        public void Use( Mobile from )
        {
            m_Uses--;
            InvalidateProperties();

            if( m_Uses <= 0 )
            {
                if( Parent == null )
                    new WoodenBowl().MoveToWorld( Location, Map );
                else
                    from.AddToBackpack( new WoodenBowl() );
                Consume();
            }
        }

        #region serialization

        public BowlFlour( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 );

            writer.Write( m_Uses );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        m_Uses = reader.ReadInt();
                        break;
                    }
            }
        }

        #endregion

        private class InternalTarget : Target
        {
            private BowlFlour m_Item;

            public InternalTarget( BowlFlour item )
                : base( 1, false, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Item.Deleted )
                    return;

                if( targeted is Item && !( (Item)targeted ).Movable )
                    return;

                if( targeted is Pitcher )
                {
                    if( BaseBeverage.ConsumeTotal( from.Backpack, typeof( Pitcher ), BeverageType.Water, 1 ) )
                    {
                        Effects.PlaySound( from.Location, from.Map, 0x240 );
                        from.AddToBackpack( new Dough() );
                        from.SendMessage( "You made some dough and put it them in your backpack" );
                        m_Item.Use( from );
                    }
                }

                if( targeted is SweetDough )
                {
                    FoodHelper.GiveFood( from, new CakeMix(), "You made a cake mix" );
                    ( (SweetDough)targeted ).Consume();
                    m_Item.Use( from );
                }

                if( targeted is TribalBerry )
                {
                    if( from.Skills[SkillName.Cooking].Base >= 80.0 )
                    {
                        m_Item.Use( from );
                        ( (TribalBerry)targeted ).Delete();

                        from.AddToBackpack( new TribalPaint() );
                        from.SendLocalizedMessage( 1042002 );
                    }
                    else
                    {
                        from.SendLocalizedMessage( 1042003 );
                    }
                }
            }
        }
    }

    public class RawScaledFish : Item, ICarvable
    {
        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;

            base.ScissorHelper( from, new RawHeadlessFish(), 1 );
            from.AddToBackpack( new FishHeads( item.Amount ) );
        }

        [Constructable]
        public RawScaledFish()
            : this( 1 )
        {
        }

        [Constructable]
        public RawScaledFish( int amount )
            : base( Utility.Random( 7701, 2 ) )
        {
            Stackable = true;
            Weight = 0.8;
            Amount = amount;
        }

        public RawScaledFish( Serial serial )
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

    [TypeAlias( "Server.Items.SackcornFlourOpen" )]
    public class SackcornFlour : Item, IHasQuantity
    {
        private int m_Quantity;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Quantity
        {
            get { return m_Quantity; }
            set
            {
                if( value < 0 )
                    value = 0;
                else if( value > 20 )
                    value = 20;

                m_Quantity = value;

                InvalidateProperties();

                if( m_Quantity == 0 )
                    Delete();
                else if( m_Quantity < 20 && ( ItemID == 0x1039 || ItemID == 0x1045 ) )
                    ++ItemID;
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1060584, m_Quantity.ToString() );
        }

        [Constructable]
        public SackcornFlour()
            : base( 0x1039 )
        {
            Name = "A Sack of Cornflour";
            m_Quantity = 20;
        }

        public SackcornFlour( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 );

            writer.Write( m_Quantity );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        m_Quantity = reader.ReadInt();
                        break;
                    }
                case 0:
                    {
                        m_Quantity = 20;
                        break;
                    }
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            if( ( ItemID == 0x1039 || ItemID == 0x1045 ) )
                ++ItemID;
        }
    }
}