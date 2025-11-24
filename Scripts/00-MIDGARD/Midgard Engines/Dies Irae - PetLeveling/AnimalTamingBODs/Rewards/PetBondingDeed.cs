using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class PetBondingDeed : Item
    {
        public override string DefaultName
        {
            get
            {
                return "a Pet Bonding Deed";
            }
        }

        [Constructable]
        public PetBondingDeed()
            : base( 0x14F0 )
        {
            Weight = 1.0;
        }

        public PetBondingDeed( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
            else if( from.InRange( GetWorldLocation(), 1 ) )
            {
                SendLocalizedMessageTo( from, 1010086 );
                from.Target = new BondTarget( this );
            }
            else
            {
                from.SendLocalizedMessage( 500446 ); // That is too far away. 
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( Name == "Pet Bonding Deed" )
                Name = null;
        }

        private class BondTarget : Target
        {
            private PetBondingDeed m_Deed;

            public BondTarget( PetBondingDeed deed )
                : base( 10, false, TargetFlags.None )
            {
                m_Deed = deed;
            }

            protected override void OnTarget( Mobile from, object target )
            {
                if( m_Deed == null || m_Deed.Deleted )
                    return;

                if( target == from )
                {
                    from.SendMessage( "You cant do that." );
                }
                else if( target is BaseCreature )
                {
                    BaseCreature c = (BaseCreature)target;

                    if( !c.Controlled )
                    {
                        from.SendMessage( "That Creature is not tamed." );
                    }
                    else if( c.ControlMaster != from )
                    {
                        from.SendMessage( "This is not your pet." );
                    }
                    else if( c.Controlled && c.ControlMaster == from )
                    {
                        c.IsBonded = true;
                        c.BondingBegin = DateTime.MinValue;

                        from.SendLocalizedMessage( 1049666 ); // Your pet has bonded with you!

                        from.PlaySound( 503 );
                        m_Deed.Delete();
                    }
                }
                else
                {
                    from.SendMessage( "You cant do that." );
                }
            }
        }
    }

    public class NewbiePetBondingDeed : PetBondingDeed
    {
        private Mobile m_Owner;
        private DateTime m_Created;

        [Constructable]
        public NewbiePetBondingDeed( Mobile owner )
        {
            LootType = LootType.Blessed;
            Hue = 1154;

            m_Owner = owner;
            m_Created = DateTime.Now;
        }

        public NewbiePetBondingDeed( Serial serial )
            : base( serial )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( m_Owner != null )
                list.Add( 1041602, m_Owner.Name ); // Owner: ~1_val~

            list.Add( "Valid until: {0}", m_Created.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" ) );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( m_Owner == null )
                from.SendMessage( "That is not an usable deed." );
            else
            {
                if( m_Owner == from )
                {
                    if( DateTime.Now <= m_Created + TimeSpan.FromDays( 15.0 ) )
                    {
                        base.OnDoubleClick( from );
                    }
                    else
                    {
                        from.SendMessage( "That deed is out of time. It's deleted!" );
                        Delete();
                    }
                }
                else
                    from.SendMessage( "You do not own that deed!" );
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
            writer.WriteMobile( m_Owner );
            writer.Write( m_Created );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
            m_Owner = reader.ReadMobile();
            m_Created = reader.ReadDateTime();
        }
    }
}