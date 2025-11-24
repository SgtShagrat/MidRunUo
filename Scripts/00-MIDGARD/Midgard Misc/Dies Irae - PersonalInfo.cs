using System;
using Server;
using Server.Mobiles;

namespace Midgard.Misc
{
    public enum InfoType
    {
        Email,
        ICQ,
        MSN
    }

    [PropertyObject]
    public class PersonalInfo
    {
        private string m_MsnContact;
        private string m_IcqContact;
        private string m_Email;

        private Mobile m_Owner;

        [CommandProperty( AccessLevel.GameMaster )]
        public string MsnContact
        {
            get { return m_MsnContact; }
            set { m_MsnContact = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public string IcqContact
        {
            get { return m_IcqContact; }
            set { m_IcqContact = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public string Email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }

        public override string ToString()
        {
            return "...";
        }

        public static PersonalInfo FindInfo( Mobile m )
        {
            if ( m is Midgard2PlayerMobile )
            {
                return ( (Midgard2PlayerMobile)m ).Info;
            }
            else
            {
                return null;
            }
        }

        public PersonalInfo( Mobile owner )
        {
            m_Owner = owner;

            m_MsnContact = String.Empty;
            m_IcqContact = String.Empty;
            m_Email = String.Empty;
        }

        public PersonalInfo( Mobile owner, GenericReader reader )
        {
            m_Owner = owner;

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        m_MsnContact = reader.ReadString();
                        m_IcqContact = reader.ReadString();
                        m_Email = reader.ReadString();

                        break;
                    }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( 0 ); // version

            writer.Write( m_MsnContact );
            writer.Write( m_IcqContact );
            writer.Write( m_Email );
        }
    }
}