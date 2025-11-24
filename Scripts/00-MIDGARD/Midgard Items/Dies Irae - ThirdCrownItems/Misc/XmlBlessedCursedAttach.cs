using System;
using Server;
using Server.Engines.XmlSpawner2;

namespace Midgard.Items
{
    public class XmlBlessedCursedAttach : XmlAttachment
    {
        public enum MagicalTypes
        {
            None,
            Bless,
            Curse
        }

        private int m_Charges;
        private Mobile m_Mob;
        private MagicalTypes m_MagicType;

        [Attachable]
        public XmlBlessedCursedAttach( int charges, Mobile mob, MagicalTypes type )
        {
            m_Charges = charges;
            m_Mob = mob;
            m_MagicType = type;

            Name = "XmlBlessedCursedAttach";
        }

        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        public Mobile Mob
        {
            get { return m_Mob; }
            set { m_Mob = value; }
        }

        public MagicalTypes MagicType
        {
            get { return m_MagicType; }
            set { m_MagicType = value; }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            Item i = AttachedTo as Item;

            if( i == null )
                Delete();
        }

        public static void OnSingleClick( Mobile from, Item item )
        {
            XmlBlessedCursedAttach att = XmlAttach.FindAttachment( item, typeof( XmlBlessedCursedAttach ) ) as XmlBlessedCursedAttach;

            if( att != null && item != null && from != null && att.Charges > 0 && att.MagicType == MagicalTypes.Bless )
                item.LabelTo( from, "[Bless charges {0}]", att.Charges );
        }

        public static bool CanEquip( Mobile mob, Item item )
        {
            XmlBlessedCursedAttach att = XmlAttach.FindAttachment( item, typeof( XmlBlessedCursedAttach ) ) as XmlBlessedCursedAttach;

            if( att == null )
                return true;

            bool canEquip = item != null && mob != null && mob == att.Mob;

            if( mob != null && !canEquip )
                mob.SendMessage( "This item does not belong to you." );

            return canEquip;
        }

        public static bool CheckOnParentDeath( Mobile from, Item item, out DeathMoveResult result )
        {
            XmlBlessedCursedAttach att = XmlAttach.FindAttachment( item, typeof( XmlBlessedCursedAttach ) ) as XmlBlessedCursedAttach;
            result = DeathMoveResult.MoveToBackpack;

            if( att != null && att.Charges > 0 )
            {
                att.ConsumeCharge();

                if( att.MagicType == MagicalTypes.Bless )
                    result = DeathMoveResult.MoveToBackpack;
                if( att.MagicType == MagicalTypes.Curse )
                    result = DeathMoveResult.RemainEquiped;

                return true;
            }

            return false;
        }

        public void ConsumeCharge()
        {
            --m_Charges;
        }

        #region serialization
        public XmlBlessedCursedAttach( ASerial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( (int)m_MagicType );
            writer.Write( m_Charges );
            writer.WriteMobile( m_Mob );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_MagicType = (MagicalTypes)reader.ReadInt();
            m_Charges = reader.ReadInt();
            m_Mob = reader.ReadMobile();

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Delete_Callback ), this );
        }

        public static void Delete_Callback( object state )
        {
            XmlBlessedCursedAttach a = (XmlBlessedCursedAttach)state;
            if( a != null && !a.Deleted )
                a.Delete();
        }
        #endregion
    }
}