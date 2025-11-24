using System.Text;
using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    public abstract class BaseOldMagicWizardsHat : BaseHat
    {
        private Mobile m_Owner;

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; InvalidateSecondAgeName(); }
        }

        public override int ArmorBase { get { return 1; } }
        public override int InitMinHits { get { return 21; } }
        public override int InitMaxHits { get { return 23; } }
        public override string OldInitHits { get { return "1d3+20"; } }

        public override string DefaultName { get { return "magic wizard hat"; } }

        public override bool CanEquip( Mobile from )
        {
            if( from == null )
                return false;

            if( from.Player && from.Skills[ SkillName.Magery ].Value < 100 )
            {
                from.SendMessage( "Your magery power is to low to use this powerful item." );
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
                if( m_Owner == null )
                {
                    from.SendMessage( "You get this for the first time" );
                    Owner = from;
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
            ConsumeCharge( from );
            InvalidateSecondAgeNames();
        }

        public BaseOldMagicWizardsHat()
            : this( 0 )
        {
        }

        public BaseOldMagicWizardsHat( int hue )
            : base( 0x1718, hue )
        {
            Weight = 1.0;
            MagicalCharges = 5;
        }

        private string m_SecondAgeFullName;

        public void InvalidateSecondAgeName()
        {
            StringBuilder sb = new StringBuilder();

            if( m_Owner != null )
                sb.AppendFormat( "{0}'s {1}", m_Owner.Name, DefaultName );
            else
                sb.AppendFormat( "a {0}\n", DefaultName );

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
                LabelTo( from, "a magic wizard hat" );
                return;
            }

            LabelTo( from, m_SecondAgeFullName );
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

        #region serialization
        public BaseOldMagicWizardsHat( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_Owner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Owner = reader.ReadMobile();
        }
        #endregion
    }

    public class OldMagicWizardsHatOne : BaseOldMagicWizardsHat
    {
        public override int BaseStrBonus { get { return -5; } }
        public override int BaseDexBonus { get { return -5; } }
        public override int BaseIntBonus { get { return +10; } }

        [Constructable]
        public OldMagicWizardsHatOne()
        {
            Hue = Utility.RandomMinMax( 0x2, 0x384 );
        }

        #region serialization
        public OldMagicWizardsHatOne( Serial serial )
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

    public class OldMagicWizardsHatTwo : BaseOldMagicWizardsHat
    {
        public override int BaseStrBonus { get { return -0; } }
        public override int BaseDexBonus { get { return -0; } }
        public override int BaseIntBonus { get { return +10; } }

        [Constructable]
        public OldMagicWizardsHatTwo()
            : base( Utility.RandomMinMax( 0x2, 0x384 ) )
        {
        }

        public override string DefaultName { get { return "enchanted wizard hat"; } }

        #region serialization
        public OldMagicWizardsHatTwo( Serial serial )
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

    public class OldMagicWizardsHatThree : BaseOldMagicWizardsHat, ITreasureOfMidgard
    {
        public override int BaseStrBonus { get { return -0; } }
        public override int BaseDexBonus { get { return -0; } }
        public override int BaseIntBonus { get { return +20; } }

        [Constructable]
        public OldMagicWizardsHatThree()
            : base( Utility.RandomMinMax( 0x2, 0x384 ) )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            StringBuilder sb = new StringBuilder();

            if( !IsIdentifiedFor( from ) )
                sb.Append( "a magic wizard hat" );
            else
                sb.AppendFormat( "The Hat of Ornius the Mage" );

            if( IsIdentifiedFor( from ) && MagicalCharges > 0 )
                sb.AppendFormat( " with {0} charges", MagicalCharges );

            LabelTo( from, sb.ToString() );
        }

        #region serialization
        public OldMagicWizardsHatThree( Serial serial )
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