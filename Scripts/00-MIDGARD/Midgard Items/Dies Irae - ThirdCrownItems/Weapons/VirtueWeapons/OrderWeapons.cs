using Server;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Items
{
    public interface IOrderWeapon
    {
    }

    public class OrderBardiche : Bardiche, IOrderWeapon
    {
        public override string DefaultName { get { return "Bardiche of Order"; } }

        public override int OldStrengthReq { get { return 40; } }
        public override int OldSpeed { get { return 25; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 20; } }
        public override int DiceBonus { get { return 5; } }

        [Constructable]
        public OrderBardiche()
        {
            Weight = 20.0;
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Validate( Parent as Mobile ) )
                base.OnSingleClick( from );
        }

        public virtual bool Validate( Mobile m )
        {
            if( m == null || !m.Player || m.AccessLevel != AccessLevel.Player )
                return true;

            bool isOrder = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsOrder;

            if( !isOrder )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public OrderBardiche( Serial serial )
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
        }
        #endregion
    }

    public class OrderTwoHandedSword : TwoHandedSword, IOrderWeapon
    {
        public override string DefaultName { get { return "Two Handed Sword of Order"; } }

        public override int OldStrengthReq { get { return 75; } }
        public override int OldSpeed { get { return 22; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 6; } }
        public override int NumSides { get { return 6; } }
        public override int DiceBonus { get { return 5; } }

        [Constructable]
        public OrderTwoHandedSword()
        {
            Weight = 17.0;
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Validate( Parent as Mobile ) )
                base.OnSingleClick( from );
        }

        public virtual bool Validate( Mobile m )
        {
            if( m == null || !m.Player || m.AccessLevel != AccessLevel.Player )
                return true;

            bool isOrder = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsOrder;

            if( !isOrder )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public OrderTwoHandedSword( Serial serial )
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
        }
        #endregion
    }

    public class OrderSpear : Spear, IOrderWeapon
    {
        public override string DefaultName { get { return "Spear of Order"; } }

        public override int OldStrengthReq { get { return 30; } }
        public override int OldSpeed { get { return 40; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 18; } }
        public override int DiceBonus { get { return 2; } }

        [Constructable]
        public OrderSpear()
        {
            Weight = 14.0;
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Validate( Parent as Mobile ) )
                base.OnSingleClick( from );
        }

        public virtual bool Validate( Mobile m )
        {
            if( m == null || !m.Player || m.AccessLevel != AccessLevel.Player )
                return true;

            bool isOrder = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsOrder;

            if( !isOrder )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public OrderSpear( Serial serial )
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
        }
        #endregion
    }

    public class OrderDoubleAxe : DoubleAxe, IOrderWeapon
    {
        public override string DefaultName { get { return "Double Axe of Order"; } }

        public override int OldStrengthReq { get { return 35; } }
        public override int OldSpeed { get { return 37; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 15; } }
        public override int DiceBonus { get { return 6; } }

        [Constructable]
        public OrderDoubleAxe()
        {
            Weight = 11.0;
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Validate( Parent as Mobile ) )
                base.OnSingleClick( from );
        }

        public virtual bool Validate( Mobile m )
        {
            if( m == null || !m.Player || m.AccessLevel != AccessLevel.Player )
                return true;

            bool isOrder = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsOrder;

            if( !isOrder )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public OrderDoubleAxe( Serial serial )
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
        }
        #endregion
    }

    public class OrderTwoHandedAxe : TwoHandedAxe, IOrderWeapon
    {
        public override string DefaultName { get { return "Two Handed Axe of Order"; } }

        public override int OldStrengthReq { get { return 40; } }
        public override int OldSpeed { get { return 30; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 18; } }
        public override int DiceBonus { get { return 5; } }

        [Constructable]
        public OrderTwoHandedAxe()
        {
            Weight = 14.0;
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Validate( Parent as Mobile ) )
                base.OnSingleClick( from );
        }

        public virtual bool Validate( Mobile m )
        {
            if( m == null || !m.Player || m.AccessLevel != AccessLevel.Player )
                return true;

            bool isOrder = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsOrder;

            if( !isOrder )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public OrderTwoHandedAxe( Serial serial )
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
        }
        #endregion
    }

    public class OrderMageStaff : MageStaff, IOrderWeapon
    {
        public override string DefaultName { get { return "Mage Staff of Order"; } }

        public override int OldStrengthReq { get { return 25; } }
        public override int OldSpeed { get { return 40; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 1; } }
        public override int NumSides { get { return 15; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public OrderMageStaff()
        {
            Weight = 8.0;
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Validate( Parent as Mobile ) )
                base.OnSingleClick( from );
        }

        public virtual bool Validate( Mobile m )
        {
            if( m == null || !m.Player || m.AccessLevel != AccessLevel.Player )
                return true;

            bool isOrder = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsOrder;

            if( !isOrder )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public OrderMageStaff( Serial serial )
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
        }
        #endregion
    }

    public class OrderBow : Bow, IOrderWeapon
    {
        public override string DefaultName { get { return "Bow of Order"; } }

        public override int OldStrengthReq { get { return 20; } }
        public override int OldSpeed { get { return 30; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 4; } }
        public override int NumSides { get { return 9; } }
        public override int DiceBonus { get { return 7; } }

        [Constructable]
        public OrderBow()
        {
            Weight = 6.0;
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Validate( Parent as Mobile ) )
                base.OnSingleClick( from );
        }

        public virtual bool Validate( Mobile m )
        {
            if( m == null || !m.Player || m.AccessLevel != AccessLevel.Player )
                return true;

            bool isOrder = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsOrder;

            if( !isOrder )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public OrderBow( Serial serial )
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
        }
        #endregion
    }

    public class BagOfOrderWeapons : Bag
    {
        [Constructable]
        public BagOfOrderWeapons()
        {
            DropItem( new OrderBardiche() );
            DropItem( new OrderTwoHandedSword() );
            DropItem( new OrderSpear() );
            DropItem( new OrderDoubleAxe() );
            DropItem( new OrderTwoHandedAxe() );
            DropItem( new OrderMageStaff() );
            DropItem( new OrderBow() );
        }

        #region serialization
        public BagOfOrderWeapons( Serial serial )
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
        }
        #endregion
    }
}