using System.Text;
using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    public abstract class BaseRingOfProtection : BaseRing
    {
        private const double MageryRequisite = 100.0;

        private Mobile m_Owner;

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; InvalidateSecondAgeName(); }
        }

        public abstract int Bonus { get; }

        public bool CheckRequisites( Mobile from, bool message )
        {
            string result = string.Empty;
            bool ok = true;

            if( from.Player && from.Skills[ SkillName.Magery ].Value < MageryRequisite )
            {
                result =  "Your magery power is to low to use this powerful item.";
                ok = false;
            }
            if( from.VirtualArmorMod > 0 )
            {
                result = "Your magical protection prevent using this item.";
                ok = false;
            }
            else if( MagicalCharges <= 0 )
            {
                result = "This item is out of charges and can not be equipped anymore.";
                ok = false;
            }

            if( !ok && message )
                from.SendMessage( result );

            return ok;
        }

        public override bool CanEquip( Mobile from )
        {
            if( from == null )
                return false;

            if( !CheckRequisites( from, true ) )
            {
                return false;
            }
            else if( m_Owner != from )
            {
                if( m_Owner != null )
                    from.PrivateOverheadMessage( MessageType.Regular, 37, true, string.Format( "I can't use that, it belongs to {0}\n", m_Owner.Name ), from.NetState );
                else
                    from.PrivateOverheadMessage( MessageType.Regular, 37, true, "I can't use that. It must belong to somebody before being used", from.NetState );

                return false;
            }

            return base.CanEquip( from );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsAccessibleTo( from ) && IsChildOf( from.Backpack ) )
            {
                if( !CheckRequisites( from, true ) )
                {
                    return;
                }
                else if( m_Owner == null )
                {
                    from.SendMessage( "You get this for the first time" );
                    m_Owner = from;
                }
                else if( m_Owner != from )
                {
                    from.SendMessage( "This isn't your's! You are not worthy to own this!!!" );
                    return;
                }
            }

            base.OnDoubleClick( from );
        }

        public override void OnPreAosUse( Mobile from )
        {
            from.VirtualArmorMod += Bonus;
            from.FixedParticles( 0x375A, 9, 20, 5016, EffectLayer.Waist );
            from.PlaySound( 0x1ED );

            ConsumeCharge( from );
            InvalidateSecondAgeName();
        }

        public override void OnRemoved( object parent )
        {
            base.OnRemoved( parent );

            if( parent is Mobile )
            {
                Mobile p = (Mobile)parent;

                if( p.VirtualArmorMod >= Bonus )
                    p.VirtualArmorMod -= Bonus;
                else
                    p.VirtualArmorMod = 0;
            }
        }

        public BaseRingOfProtection()
            : base( Utility.RandomList( 0x108A, 0x1F09 ) )
        {
            MagicalCharges = Utility.Dice( 1, 20, 20 );
        }

        private string m_SecondAgeFullName;

        public void InvalidateSecondAgeName()
        {
            StringBuilder sb = new StringBuilder();

            if( m_Owner != null )
                sb.AppendFormat( "{0}'s magic ring +{1} of protection", m_Owner.Name, Bonus );
            else
                sb.AppendFormat( "a magic ring +{0} of protection", Bonus );

            if( MagicalCharges > 0 )
                sb.AppendFormat( " with {0} charges", MagicalCharges );

            m_SecondAgeFullName = sb.ToString();
        }

        public override void OnSingleClick( Mobile from )
        {
            if( m_SecondAgeFullName == null )
                InvalidateSecondAgeName();

            if( !IsIdentifiedFor( from ) )
            {
                LabelTo( from, "a magic ring" );
                return;
            }

            LabelTo( from, m_SecondAgeFullName );
        }

        public void Doc( StringBuilder builder )
        {
            if( Bonus != 0 )
                builder.AppendFormat( "Armor bonus: {0}\n", Bonus );
        }

        #region serialization
        public BaseRingOfProtection( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version

            writer.Write( Owner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    Owner = reader.ReadMobile();
                    goto case 0;
                case 0:
                    break;
            }

            if( Parent is Mobile )
            {
                if( !CheckRequisites( ( (Mobile)Parent ), true ) )
                {
                    Owner = null;
                    InvalidateSecondAgeName();
                }
                else if ( Bonus != 0 )
                    ( (Mobile)Parent ).VirtualArmorMod += Bonus;
            }
        }
        #endregion
    }

    public class RingOfProtectionOne : BaseRingOfProtection
    {
        public override int Bonus { get { return 1; } }

        [Constructable]
        public RingOfProtectionOne()
        {
            Hue = Utility.RandomMinMax( 0x2, 0x384 );
        }

        #region serialization
        public RingOfProtectionOne( Serial serial )
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

    public class RingOfProtectionTwo : BaseRingOfProtection
    {
        public override int Bonus { get { return 2; } }

        [Constructable]
        public RingOfProtectionTwo()
        {
            Hue = Utility.RandomMinMax( 0x2, 0x384 );
        }

        #region serialization
        public RingOfProtectionTwo( Serial serial )
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

    public class RingOfProtectionThree : BaseRingOfProtection
    {
        public override int Bonus { get { return 3; } }

        [Constructable]
        public RingOfProtectionThree()
        {
            Hue = Utility.RandomMinMax( 0x2, 0x384 );
        }

        #region serialization
        public RingOfProtectionThree( Serial serial )
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

    public class RingOfProtectionFour : BaseRingOfProtection
    {
        public override int Bonus { get { return 4; } }

        [Constructable]
        public RingOfProtectionFour()
        {
            Hue = Utility.RandomMinMax( 0x2, 0x384 );
        }

        #region serialization
        public RingOfProtectionFour( Serial serial )
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

    public class RingOfProtectionFive : BaseRingOfProtection
    {
        public override int Bonus { get { return 5; } }

        [Constructable]
        public RingOfProtectionFive()
        {
            Hue = Utility.RandomMinMax( 0x2, 0x384 );
        }

        #region serialization
        public RingOfProtectionFive( Serial serial )
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

    public class RingOfProtectionSix : BaseRingOfProtection, ITreasureOfMidgard
    {
        public override int Bonus { get { return 6; } }

        [Constructable]
        public RingOfProtectionSix()
            : base( 6 )
        {
            Hue = Utility.RandomMinMax( 0x2, 0x384 );
        }

        #region serialization
        public RingOfProtectionSix( Serial serial )
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