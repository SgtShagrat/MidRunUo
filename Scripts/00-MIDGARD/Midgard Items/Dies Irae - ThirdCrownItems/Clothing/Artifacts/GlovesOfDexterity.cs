using System.Text;
using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    public abstract class BaseGlovesOfDexterity : LeatherGloves
    {
        public override int InitMinHits { get { return 20; } }
        public override int InitMaxHits { get { return 30; } }

        public override string DefaultName { get { return "magic leather gloves"; } }

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

            if( !base.CanEquip( from ) )
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
                from.SendMessage( "This item is out of charges and can not be equipped anymore." );
                return false;
            }

            return true;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsAccessibleTo( from ) && IsChildOf( from.Backpack ) )
            {
                if( Owner == null )
                {
                    from.SendMessage( "You get this for the first time." );
                    Owner = from;
                }
                else if( Owner != from )
                {
                    from.SendMessage( "This isn't your's! You are not worthy to own this!!!" );
                    return;
                }
            }

            base.OnDoubleClick( from );
        }

        protected override void OnPreAosUse( Mobile from )
        {
            ConsumeCharge( from );
            InvalidateSecondAgeNames();
        }

        private string m_SecondAgeFullName;

        public override void InvalidateSecondAgeNames()
        {
            StringBuilder sb = new StringBuilder();

            if( Owner != null )
                sb.AppendFormat( "{0}'s {1}", Owner.Name, GlovesName );
            else
                sb.AppendFormat( GlovesName );

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
                LabelTo( from, "magic leather gloves" );
                return;
            }

            LabelTo( from, m_SecondAgeFullName );
        }

        public virtual string GlovesName
        {
            get { return "magic leather gloves of dexterity"; }
        }

        public void Doc( StringBuilder builder )
        {
            if( OldStrBonus != 0 )
                builder.AppendFormat( "Strenght bonus: {0}\n", OldStrBonus );
            if( OldDexBonus != 0 )
                builder.AppendFormat( "Dexterity bonus: {0}\n", OldDexBonus );
            if( OldIntBonus != 0 )
                builder.AppendFormat( "Intelligence bonus: {0}\n", OldIntBonus );
        }

        public BaseGlovesOfDexterity( int hue )
        {
            Hue = hue;

            Weight = 1.0;
            MagicalCharges = 5;
        }

        #region serialization
        public BaseGlovesOfDexterity( Serial serial )
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

    public class GlovesOfDexterityOne : BaseGlovesOfDexterity
    {
        public override int OldStrBonus { get { return -5; } }
        public override int OldDexBonus { get { return +10; } }
        public override int OldIntBonus { get { return -5; } }

        [Constructable]
        public GlovesOfDexterityOne()
            : base( Utility.RandomMinMax( 0x2, 0x384 ) )
        {
        }

        #region serialization
        public GlovesOfDexterityOne( Serial serial )
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

    public class GlovesOfDexterityTwo : BaseGlovesOfDexterity
    {
        public override int OldStrBonus { get { return 0; } }
        public override int OldDexBonus { get { return +10; } }
        public override int OldIntBonus { get { return 0; } }

        public override string GlovesName
        {
            get { return "enchanted gloves of dexterity"; }
        }

        [Constructable]
        public GlovesOfDexterityTwo()
            : base( Utility.RandomMinMax( 0x2, 0x384 ) )
        {
        }

        #region serialization
        public GlovesOfDexterityTwo( Serial serial )
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

    public class GlovesOfDexterityThree : BaseGlovesOfDexterity, ITreasureOfMidgard
    {
        public override int OldStrBonus { get { return 0; } }
        public override int OldDexBonus { get { return 20; } }
        public override int OldIntBonus { get { return 0; } }

        public override string GlovesName
        {
            get { return "gloves Of Riujey the thief"; }
        }

        [Constructable]
        public GlovesOfDexterityThree()
            : base( Utility.RandomMinMax( 0x2, 0x384 ) )
        {
        }

        #region serialization
        public GlovesOfDexterityThree( Serial serial )
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