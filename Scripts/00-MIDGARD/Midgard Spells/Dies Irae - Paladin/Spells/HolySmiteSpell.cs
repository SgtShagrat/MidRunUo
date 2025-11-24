/***************************************************************************
 *							   HolySmiteSpell.cs
 *
 *   begin				: 29 aprile 2011
 *   author			   :	Dies Irae
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.Classes;

using Server;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class HolySmiteSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "One Enemy One Shot", "Impetus Ut Inimicum Exstinguam",
            -1,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( HolySmiteSpell ),
            "Holy Smite",
            "This miracle blesses the next shot the Paladine strikes aganist evil foes.",
            "Con questo Miracolo, il Paladino benedisce il successivo colpo contro le creature malvagie.",
            "Conferisce al prossimo colpo del paladino particolare potenza contro i malvagi. Dura powerValue / 60 minuti." +
            "L'entita' del bonus e': level d 20 + 10. Raddoppiata contro creature malvagie o player necromanti.",
            0x5104
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Modificato l'effetto grafico dello spell in fulmine e aggiunto messaggio quando il colpo va a segno.<br>" +
                "La durata dell'effetto e' di PowerValueScaled secondi.<br>";
        }

        public override SpellCircle Circle { get { return SpellCircle.Third; } }

        public HolySmiteSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            if( SpellEffectContextHelper.IsUnderEffect( Caster, typeof( InvulnerabilitySpell ) ) )
            {
                Caster.SendLangMessage( 10000401 ); // "Thou cannot use this mighty miracle while in a such status." );
                FinishSequence();
                return;
            }

            if( CheckSequence() )
            {
                Caster.PlaySound( 0x0F5 );
                Caster.PlaySound( 0x1ED );
                Caster.FixedParticles( 0x375A, 1, 30, 9966, 1154, 2, EffectLayer.Head );
                Caster.FixedParticles( 0x37B9, 1, 30, 9502, 2024, 3, EffectLayer.Head );

                if( m_Table != null && m_Table.ContainsKey( Caster ) )
                {
                    Timer t = m_Table[ Caster ];

                    if( t != null )
                        t.Stop();
                }

                m_Table[ Caster ] = Timer.DelayCall( TimeSpan.FromSeconds( PowerValueScaled ), new TimerStateCallback( Expire_Callback ), Caster );

                ClassPlayerState state = ClassPlayerState.Find( Caster );
                if( state != null )
                {
                    state.IsWaitingCriticalShot = true;
                    Caster.SendLangMessage( 10000402 ); // "Your next shot to Evils will be defiant!"
                }
            }

            FinishSequence();
        }

        public override void OnCasterDamaged( Mobile from, int damage )
        {
            if( Caster.PlayerDebug )
                Caster.SendMessage( "Debug PaladinSpell: chance to resist disruption: 100%." );
        }

        public override bool CheckDisturb( DisturbType type, bool firstCircle, bool resistable )
        {
            return false;
        }

        public static bool UnderEffect( Mobile m )
        {
            return m != null && m_Table.ContainsKey( m );
        }

        public static bool IsWaitingPaladine( Mobile m )
        {
            ClassPlayerState state = ClassPlayerState.Find( m );
            return state != null && ( m != null && UnderEffect( m ) && state.IsWaitingCriticalShot );
        }

        public static int GetBonusDamage( Mobile attacker, Mobile defender )
        {
            if( attacker == null || defender == null )
                return 0;

            int level = RPGSpellsSystem.GetPowerLevel( attacker, typeof( HolySmiteSpell ) );
            int bonus = Utility.Dice( level, 10, 10 ); // lvlD10+10

            if( !defender.Player || IsSuperVulnerable( defender ) )
                bonus *= 3 / 2;

            return bonus;
        }

        public static void DoEffect( Mobile attacker, Mobile defender )
        {
            if( attacker == null || defender == null )
                return;

            if( IsEnemy( attacker, defender ) )
            {
                defender.BoltEffect( 0 );
                attacker.SendLangMessage( 10000403 ); // "Your enemy were slayed by your migthy force!"
                defender.SendLangMessage( 10000404 ); // "The Paladin force broke in your soul."
            }

            ExpireEffect( attacker, false );
        }

        private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

        private static void Expire_Callback( object state )
        {
            ExpireEffect( (Mobile)state, true );
        }

        public static void ExpireEffect( Mobile m, bool message )
        {
            if( message && IsWaitingPaladine( m ) )
            {
                m.PlaySound( 0x1F8 );
                m.SendLangMessage( 10000405 ); // "Time has gone. Your power has been called invane."
            }

            m_Table.Remove( m );

            ClassPlayerState state = ClassPlayerState.Find( m );
            if( state != null )
                state.IsWaitingCriticalShot = false;
        }
    }
}