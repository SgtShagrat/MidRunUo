/***************************************************************************
 *                               ShieldOfRighteousnessSpell.cs
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
using Server.Network;
using Server.Spells;
using Core = Midgard.Engines.Races.Core;

namespace Midgard.Engines.SpellSystem
{
    public class ShieldOfRighteousnessSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Shield Of Righteousness", "Scutum Iustitiae",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( ShieldOfRighteousnessSpell ),
            "Shield Of Right.",
            "This Miracle summons a powerful artifact shield to be used by the Paladin.",
            "Questo Miracolo crea un potente scudo artefatto per essere usato dal Paladino.",
            "Cra la lo scudo del bene per ( 60 * level ) secondi. Lo scudo e' blessata e quando viene colpito ha colpisce ha (faith / 120)% di usare un potere magico a scelta tra" +
            "Heal e Light sul paladino. Ha un delay al riuso di 600 secondi.",
            0x5101
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return string.Empty;
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Second; }
        }

        public ShieldOfRighteousnessSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            BaseShield shield = FindValidShield();
            if( shield == null )
            {
                Caster.SendLangMessage( 10001001 ); // "You must wield a valid shield su summon this power."
                FinishSequence();
                return;
            }

            if( Caster.CanBeginAction( typeof( ShieldOfRighteousnessSpell ) ) )
            {
                if( CheckSequence() )
                {
                    if( ToggleShieldStatus( shield, PowerValueScaled ) )
                    {
                        Caster.BeginAction( typeof( ShieldOfRighteousnessSpell ) );

                        string message = TextHelper.Text( 10001002, Caster.TrueLanguage ); // "* Your shield of Righteousness will be your aid *"
                        Caster.PublicOverheadMessage( MessageType.Regular, 37, true, message );
                        Caster.PlaySound( 311 );
                        
                        Timer.DelayCall( TimeSpan.FromSeconds( PowerValueScaled ) + GetDelayOfReuseInSeconds(), new TimerStateCallback( ReleaseShieldOfRighteousnessLock ), Caster );
                    }
                }
            }
            else
                Caster.SendLangMessage( 10001003 ); // "You cannot summon another shield yet."

            FinishSequence();
        }

        private BaseShield FindValidShield()
        {
            return Caster.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;
        }

        private bool ToggleShieldStatus( BaseShield shield, double durationInSeconds )
        {
            int hue = RPGSpellsSystem.IsAtCap( Caster, SkillName.Chivalry ) ? 0x47E : HolyHue;
            if( Core.IsElfRace( Caster.Race ) )
                hue = HolyHueForElves; // green hue

            XmlHolyItemAttach attach = new XmlHolyItemAttach( durationInSeconds / 60.0, hue, Caster );
            return XmlAttach.AttachTo( shield, attach );
        }

        private static void ReleaseShieldOfRighteousnessLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( ShieldOfRighteousnessSpell ) );
            ( (Mobile)state ).SendLangMessage( 10001004 ); // "You can summon a new holy shield now." );
        }

        public static bool OnHitOld( Mobile attacker, Mobile paladin )
        {
            // attacker è il mobile che tiene l'arma in mano
            // paladin è colui che viene colpito (e che quindi ha lo scudo)

            if( Utility.RandomDouble() <= paladin.Skills[ SkillName.Chivalry ].Value / 200.0 )
            {
                bool success = false;
                switch( Utility.Random( 2 ) )
                {
                    case 0:
                        success = DoThirst( paladin );
                        break;
                    case 1:
                        success = DoHeal( paladin );
                        break;
                }

                if( success )
                {
                    string message = TextHelper.Text( 10001005, paladin.TrueLanguage ); // "* the might of light will prevail *"
                    paladin.PublicOverheadMessage( MessageType.Regular, 1154, true, message );
                    return true;
                }
            }

            return false;
        }

        #region [effects]
        private static bool DoHeal( Mobile paladin )
        {
            if( paladin == null )
                return false;

            if( paladin.Hits < paladin.HitsMax )
            {
                int toHeal = Utility.Dice( 1, 6, 6 );
                paladin.Heal( toHeal );
                paladin.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
                paladin.PlaySound( 0x1EA );
                return true;
            }
            else
            {
                paladin.SendLangMessage( 10001006 ); // "You are already at maximum health!" );
                return false;
            }
        }

        /*
        private static bool DoLight( Mobile attacker, Mobile defender )
        {
            if( defender == null || attacker == null )
                return false;

            if( attacker.BeginAction( typeof( LightCycle ) ) )
            {
                new LightCycle.NightSightTimer( attacker ).Start();
                attacker.LightLevel = LightCycle.DayLevel;

                attacker.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
                attacker.PlaySound( 0x1EA );
                return true;
            }

            return false;
        }
        */

        private static bool DoThirst( Mobile paladin )
        {
            if( paladin == null )
                return false;

            if( paladin.Thirst < 20 )
            {
                paladin.Thirst++;
                paladin.SendLangMessage( 10001007 ); // "Your holy weapon filled your hunger!"
                paladin.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
                paladin.PlaySound( 0x1E0 );
                return true;
            }
            else
                return DoHeal( paladin );
        }
        #endregion
    }
}