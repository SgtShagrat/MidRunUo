using System.Text;
using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    public abstract class BaseBodySashOfStrength : BodySash
    {
        public override int InitMinHits { get { return 20; } }
        public override int InitMaxHits { get { return 30; } }

        public override string DefaultName { get { return "magic body sash"; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; InvalidateSecondAgeNames(); }
        }

        private Mobile m_Owner;

        public override bool IsMagical
        {
            get { return true; }
        }

        public override bool CanEquip( Mobile from )
        {
            if( from == null )
                return false;

            if( Owner != from )
            {
                if( Owner != null )
                    from.PrivateOverheadMessage( MessageType.Regular, 37, true, string.Format( "I can't use that, it belongs to {0}\n", Owner.Name ), from.NetState );
                else
                    from.PrivateOverheadMessage( MessageType.Regular, 37, true, "I can't use that. It must belong to somebody before being used", from.NetState );

                return false;
            }
            else if( MagicalCharges <= 0 )
            {
                from.SendMessage( "This item can not be equipped anymore." );
                return false;
            }

            return base.CanEquip( from );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsAccessibleTo( from ) && IsChildOf( from.Backpack ) )
            {
                if( Owner == null )
                {
                    from.SendMessage( "You get this for the first time" );
                    m_Owner = from;
                }
                else if( Owner != from )
                {
                    from.SendMessage( "This isn't your's! You are not worthy to own this!!!" );
                    return;
                }
            }

            base.OnDoubleClick( from );
        }

        public override void OnPreAosUse( Mobile from )
        {
            ConsumeCharge( from );
            InvalidateSecondAgeNames();
        }

        private string m_SecondAgeFullName;

        public override void InvalidateSecondAgeNames()
        {
            StringBuilder sb = new StringBuilder();

            if( Owner != null )
                sb.AppendFormat( "{0}'s {1}", Owner.Name, SashName );
            else
                sb.AppendFormat( SashName );

            if( MagicalCharges > 0 )
                sb.AppendFormat( " with {0} charges", MagicalCharges );

            m_SecondAgeFullName = sb.ToString();
        }

        public override void OnSingleClick( Mobile from )
        {
            if( m_SecondAgeFullName == null )
                InvalidateSecondAgeNames();

            if( !IsIdentifiedFor( from ) )
            {
                LabelTo( from, "a magic body sash" );
                return;
            }

            LabelTo( from, m_SecondAgeFullName );
        }

        public virtual string SashName
        {
            get { return "magic sash of strength"; }
        }

        public void Doc( StringBuilder builder )
        {
            if( BaseStrBonus != 0 )
                builder.AppendFormat( "Strenght bonus: {0}\n", BaseStrBonus );
            if( BaseDexBonus != 0 )
                builder.AppendFormat( "Dexterity bonus: {0}\n", BaseDexBonus );
            if( BaseIntBonus != 0 )
                builder.AppendFormat( "Intelligence bonus: {0}\n", BaseIntBonus );
        }

        public BaseBodySashOfStrength()
            : base( Utility.RandomMinMax( 0x2, 0x384 ) )
        {
            MagicalCharges = 5;
        }

        #region serialization
        public BaseBodySashOfStrength( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( Owner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Owner = reader.ReadMobile();
        }
        #endregion
    }

    public class BodySashOfStrengthOne : BaseBodySashOfStrength
    {
        public override int BaseStrBonus { get { return +10; } }
        public override int BaseDexBonus { get { return -5; } }
        public override int BaseIntBonus { get { return -5; } }

        [Constructable]
        public BodySashOfStrengthOne()
        {
        }

        #region serialization
        public BodySashOfStrengthOne( Serial serial )
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

    public class BodySashOfStrengthTwo : BaseBodySashOfStrength
    {
        public override int BaseStrBonus { get { return +10; } }
        public override int BaseDexBonus { get { return -0; } }
        public override int BaseIntBonus { get { return 0; } }

        [Constructable]
        public BodySashOfStrengthTwo()
        {
        }

        public override string SashName
        {
            get { return "enchanted sash Of strength"; }
        }

        #region serialization
        public BodySashOfStrengthTwo( Serial serial )
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

    public class BodySashOfStrengthThree : BaseBodySashOfStrength, ITreasureOfMidgard
    {
        public override int BaseStrBonus { get { return 20; } }
        public override int BaseDexBonus { get { return 0; } }
        public override int BaseIntBonus { get { return 0; } }

        [Constructable]
        public BodySashOfStrengthThree()
        {
        }

        public override string SashName
        {
            get { return "enchanted sash Of berserk"; }
        }

        #region serialization
        public BodySashOfStrengthThree( Serial serial )
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