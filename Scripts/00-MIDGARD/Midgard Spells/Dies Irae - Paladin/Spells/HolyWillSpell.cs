/***************************************************************************
 *                               HolyWillSpell.cs
 *
 *   begin                : 29 aprile 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Spells;
using Core = Midgard.Engines.Races.Core;

namespace Midgard.Engines.SpellSystem
{
    public class HolyWillSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Holy Will", "Deorum Iussa",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( HolyWillSpell ),
            "Holy Will",
            "This Miracle enhances the armor power or the Paladine for a limited time.",
            "Questo Miracolo aumenta la difesa dell'armatura del Paladino per una certa durata.",
            "La durata e' powerValue / 60 minuti. Il bonus è (level d 4 + 1) punti armor.",
            0x510B
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Modificato il bonus all'armor in PowerValueScaled / 10 punti. Cappato a 20 punti.<br>" +
                "La durata dell'effetto e' di PowerValueScaled / 60.0 minuti.<br>";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Third; }
        }

        public HolyWillSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            if( !HasAnyValidArmor() )
            {
                Caster.SendLangMessage( 10000501 ); // "You must wield a valid armor su summon this power."
                FinishSequence();
                return;
            }

            if( Caster.CanBeginAction( typeof( HolyWillSpell ) ) )
            {
                if( CheckSequence() )
                {
                    Caster.BeginAction( typeof( HolyWillSpell ) );

                    Caster.PlaySound( 0x20F );
                    Caster.PlaySound( Caster.Body.IsFemale ? 0x338 : 0x44A );

                    Caster.VirtualArmorMod += GetArmorBonusByLevel( GetPowerLevel() );

                    new OldInternalTimer( this, Caster, PowerValueScaled / 10, TimeSpan.FromMinutes( PowerValueScaled / 60.0 ) ).Start();

                    ToggleArmorStatus( PowerValueScaled / 60.0 );

                    Caster.SendLangMessage( 10000502 ); // "The wind of Truth blows on your armor!"
                }
            }
            else
                Caster.SendLangMessage( 10000503 ); // "You cannot bless your armor now!"

            FinishSequence();
        }

        private static readonly int[] m_Bonuses = new int[] { 3, 5, 7, 12, 18 };

        private static int GetArmorBonusByLevel( int level )
        {
            return ( level < 1 || level > 5 ) ? 0 : m_Bonuses[ level - 1 ];
        }

        private bool HasAnyValidArmor()
        {
            foreach( Item i in Caster.Items )
            {
                BaseArmor armor = i as BaseArmor;
                if( armor is BaseShield )
                    continue;

                if( armor != null )
                    return true;
            }

            return false;
        }

        private void ToggleArmorStatus( double durationInMinutes )
        {
            int hue = RPGSpellsSystem.IsAtCap( Caster, SkillName.Chivalry ) ? HolyHueAtCap : HolyHue;
            if( Core.IsElfRace( Caster.Race ) )
                hue = HolyHueForElves; // green hue

            foreach( Item i in Caster.Items )
            {
                BaseArmor armor = i as BaseArmor;

                if( armor is BaseShield )
                    continue;

                if( armor != null )
                {
                    XmlHolyItemAttach attach = new XmlHolyItemAttach( durationInMinutes, hue, Caster );
                    XmlAttach.AttachTo( armor, attach );
                }
            }
        }

        private static void ReleaseHolyWillLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( HolyWillSpell ) );
            ( (Mobile)state ).SendLangMessage( 10000504 ); // "Your soul force returns normal."
        }

        private class OldInternalTimer : Timer
        {
            private readonly Mobile m_Owner;
            private readonly int m_Val;
            private readonly HolyWillSpell m_Spell;
            private readonly DateTime m_Expiration;

            public OldInternalTimer( HolyWillSpell potion, Mobile owner, int val, TimeSpan duration )
                : base( duration )
            {
                Priority = TimerPriority.OneSecond;

                m_Expiration = DateTime.Now + duration;

                m_Spell = potion;
                m_Owner = owner;
                m_Val = val;
            }

            protected override void OnTick()
            {
                if( m_Owner == null || m_Owner.Deleted )
                {
                    Stop();
                    return;
                }

                if( !m_Owner.Alive || DateTime.Now > m_Expiration )
                {
                    m_Owner.VirtualArmorMod -= m_Val;
                    if( m_Owner.VirtualArmorMod < 0 )
                        m_Owner.VirtualArmorMod = 0;

                    DelayCall( m_Spell.GetDelayOfReuseInSeconds(), new TimerStateCallback( ReleaseHolyWillLock ), m_Owner );

                    m_Owner.SendLangMessage( 10000505 ); // "The effect of Holy Will vanishes. Your armor is turned normal."
                    Stop();
                }
            }
        }
    }
}