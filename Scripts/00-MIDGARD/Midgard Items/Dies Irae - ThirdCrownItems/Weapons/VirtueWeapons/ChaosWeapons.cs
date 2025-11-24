using Server;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Items
{
    public interface IChaosWeapon
    {
    }

    public class ChaosBardiche : Bardiche, IChaosWeapon
    {
        public override string DefaultName { get { return "Bardiche of Chaos"; } }

        public override int OldStrengthReq { get { return 40; } }
        public override int OldSpeed { get { return 25; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 20; } }
        public override int DiceBonus { get { return 5; } }

        [Constructable]
        public ChaosBardiche()
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

            bool isChaos = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsChaos;

            if( !isChaos )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public ChaosBardiche( Serial serial )
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

    public class ChaosTwoHandedSword : TwoHandedSword, IChaosWeapon
    {
        public override string DefaultName { get { return "Two Handed Sword of Chaos"; } }

        public override int OldStrengthReq { get { return 75; } }
        public override int OldSpeed { get { return 22; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 6; } }
        public override int NumSides { get { return 6; } }
        public override int DiceBonus { get { return 5; } }

        [Constructable]
        public ChaosTwoHandedSword()
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

            bool isChaos = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsChaos;

            if( !isChaos )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public ChaosTwoHandedSword( Serial serial )
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

    public class ChaosSpear : Spear, IChaosWeapon
    {
        public override string DefaultName { get { return "Spear of Chaos"; } }

        public override int OldStrengthReq { get { return 30; } }
        public override int OldSpeed { get { return 40; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 18; } }
        public override int DiceBonus { get { return 2; } }

        [Constructable]
        public ChaosSpear()
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

            bool isChaos = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsChaos;

            if( !isChaos )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public ChaosSpear( Serial serial )
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

    public class ChaosDoubleAxe : DoubleAxe, IChaosWeapon
    {
        public override string DefaultName { get { return "Double Axe of Chaos"; } }

        public override int OldStrengthReq { get { return 35; } }
        public override int OldSpeed { get { return 37; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 15; } }
        public override int DiceBonus { get { return 6; } }

        [Constructable]
        public ChaosDoubleAxe()
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

            bool isChaos = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsChaos;

            if( !isChaos )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public ChaosDoubleAxe( Serial serial )
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

    public class ChaosTwoHandedAxe : TwoHandedAxe, IChaosWeapon
    {
        public override string DefaultName { get { return "Two Handed Axe of Chaos"; } }

        public override int OldStrengthReq { get { return 40; } }
        public override int OldSpeed { get { return 30; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 18; } }
        public override int DiceBonus { get { return 5; } }

        [Constructable]
        public ChaosTwoHandedAxe()
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

            bool isChaos = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsChaos;

            if( !isChaos )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public ChaosTwoHandedAxe( Serial serial )
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

    public class ChaosMageStaff : MageStaff, IChaosWeapon
    {
        public override string DefaultName { get { return "Mage Staff of Chaos"; } }

        public override int OldStrengthReq { get { return 25; } }
        public override int OldSpeed { get { return 40; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 1; } }
        public override int NumSides { get { return 15; } }
        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public ChaosMageStaff()
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

            bool isChaos = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsChaos;

            if( !isChaos )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public ChaosMageStaff( Serial serial )
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

    public class ChaosBow : Bow, IChaosWeapon
    {
        public override string DefaultName { get { return "Bow of Chaos"; } }

        public override int OldStrengthReq { get { return 20; } }
        public override int OldSpeed { get { return 30; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int NumDice { get { return 4; } }
        public override int NumSides { get { return 9; } }
        public override int DiceBonus { get { return 7; } }

        [Constructable]
        public ChaosBow()
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

            bool isChaos = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsChaos;

            if( !isChaos )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public ChaosBow( Serial serial )
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

    public class BagOfChaosWeapons : Bag
    {
        [Constructable]
        public BagOfChaosWeapons()
        {
            DropItem( new ChaosBardiche() );
            DropItem( new ChaosTwoHandedSword() );
            DropItem( new ChaosSpear() );
            DropItem( new ChaosDoubleAxe() );
            DropItem( new ChaosTwoHandedAxe() );
            DropItem( new ChaosMageStaff() );
            DropItem( new ChaosBow() );
        }

        #region serialization
        public BagOfChaosWeapons( Serial serial )
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