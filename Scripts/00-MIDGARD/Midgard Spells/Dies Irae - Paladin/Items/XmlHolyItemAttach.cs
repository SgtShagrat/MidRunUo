/***************************************************************************
 *                               XmlHolyItemAttach.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.SpellSystem
{
    public class XmlHolyItemAttach : XmlAttachment
    {
        private int m_OriginalHue;
        private readonly int m_NewHue;
        private Mobile m_Owner;
        private readonly bool m_ForceUnmovable;
        private bool m_IsAlreadySilver;

        [Attachable]
        public XmlHolyItemAttach( double durationInMinutes, int hue, Mobile owner )
            : this( durationInMinutes, hue, owner, true )
        {
        }

        [Attachable]
        public XmlHolyItemAttach( double durationInMinutes, int hue, Mobile owner, bool forceUnmovable )
        {
            m_OriginalHue = 0;
            m_NewHue = hue;
            m_Owner = owner;
            m_ForceUnmovable = forceUnmovable;

            Expiration = TimeSpan.FromMinutes( durationInMinutes );
            Name = "XmlHolyArmorAttach";
        }

        public XmlHolyItemAttach( ASerial serial )
            : base( serial )
        {
        }

        public override void OnAttach()
        {
            base.OnAttach();

            Item i = AttachedTo as Item;
            if( i != null )
            {
                m_OriginalHue = i.Hue;
                i.Hue = m_NewHue;

                if( i is BaseWeapon )
                {
                    m_IsAlreadySilver = ( (BaseWeapon)i ).Slayer == SlayerName.Silver;

                    if( !m_IsAlreadySilver )
                        ( (BaseWeapon)i ).Slayer = SlayerName.Silver;
                }

                if( m_ForceUnmovable )
                    i.Movable = false;
            }
            else
                Delete();
        }

        public override bool HandlesOnDeath
        {
            get { return true; }
        }

        public override void OnBeforeDeath( Midgard2PlayerMobile player )
        {
            Delete();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            Item i = AttachedTo as Item;
            if( i == null )
                return;

            i.Hue = m_OriginalHue;
            if( m_ForceUnmovable )
                i.Movable = true;

            // check for light levels if this is holy weapon
            if( i is BaseWeapon && m_Owner != null )
                m_Owner.CheckLightLevels( false );

            if( i is BaseWeapon && ( (BaseWeapon)i ).Slayer == SlayerName.Silver && !m_IsAlreadySilver )
                ( (BaseWeapon)i ).Slayer = SlayerName.None;
        }

        public override void OnWeaponHit( Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven )
        {
            // attacker è il mobile che tiene l'arma in mano
            // defender è colui che viene colpito (e che quindi ha l'armor)

            if( weapon.IsXmlHolyWeapon )
                SwordOfLightSpell.OnHitOld( attacker, defender );

            base.OnWeaponHit( attacker, defender, weapon, damageGiven );
        }

        public override int OnArmorHit( Mobile attacker, Mobile defender, Item armor, BaseWeapon weapon, int damageGiven )
        {
            // attacker è il mobile che tiene l'arma in mano
            // defender è colui che viene colpito (e che quindi ha l'armor)

            if( armor is BaseShield && ( (BaseShield)armor ).IsXmlHolyArmor )
                ShieldOfRighteousnessSpell.OnHitOld( attacker, defender );

            return base.OnArmorHit( attacker, defender, armor, weapon, damageGiven );
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 );

            writer.Write( m_IsAlreadySilver );
            writer.Write( m_OriginalHue );
            writer.WriteMobile( m_Owner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    m_IsAlreadySilver = reader.ReadBool();
                    goto case 0;
                case 0:
                    m_OriginalHue = reader.ReadInt();
                    m_Owner = reader.ReadMobile();
                    break;
            }

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Delete_Callback ), this );
        }

        public static void Delete_Callback( object state )
        {
            XmlHolyItemAttach a = (XmlHolyItemAttach)state;
            if( a != null && !a.Deleted )
                a.Delete();
        }
        #endregion
    }
}