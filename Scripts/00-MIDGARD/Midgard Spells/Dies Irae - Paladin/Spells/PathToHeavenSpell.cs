/***************************************************************************
 *                               PathToHeavenSpell.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using Midgard.Engines.Classes;
using Server;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class PathToHeavenSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Path To Heaven", "Iter ad Domus Caelestis",
            (int) Animations.AreaEffectSpellcast,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( PathToHeavenSpell ),
            "Path To Heaven",
            "This miracle gets the Paladine to the Holy Chamber of Virtues.",
            "Questo potente miracolo trasporta il Paladino nella la Sacra Stanza delle Virtu'.",
            "Questo potente miracolo trasporta il Paladino nella la Sacra Stanza delle Virtu'." +
            "Ogni gate porta in una locazione diversa. Il delay di riuso vale 60 secondi.",
            0x5107
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Modificato delay di riuso in 60 secondi.<br>" +
                "Nella stanza del paladino antico ora i gate eterei saranno utilizzabili a seconda del livello dello spell per il paladino che li attraversa<br>";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.First; }
        }

        public override TimeSpan CastDelayBase
        {
            get { return TimeSpan.FromSeconds( 3.0 ); }
        }

        private static readonly Point3D m_HeavenLocation = new Point3D( 5274, 1114, 0 );
        private static readonly Map m_HeavenMap = Map.Felucca;

        public PathToHeavenSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override bool CheckCast()
        {
            return base.CheckCast() && Caster.CanBeginAction( typeof( PathToHeavenSpell ) );
        }

        public override void SayMantra()
        {
            Caster.PlaySound( 0x24A );
            if( !Caster.Mounted && Caster.Body.IsHuman )
                Caster.Animate( (int)Animations.AreaEffectSpellcast, 7, 1, true, false, 0 );

            base.SayMantra();
        }

        public override void OnAnimationStarted()
        {
            // additional effect on spell animation
            Caster.FixedEffect( PaladinSystem.RandomSparkle(), 10, 30 );
            Caster.PlaySound( 0x24a ); // 586
        }

        public override void OnCast()
        {
            if( Caster.CanBeginAction( typeof( PathToHeavenSpell ) ) )
            {
                Map map = Caster.Map;

                if( !m_HeavenMap.CanSpawnMobile( m_HeavenLocation.X, m_HeavenLocation.Y, m_HeavenLocation.Z ) )
                {
                    Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
                }
                else if( SpellHelper.IsFeluccaDungeon( map, Caster.Location ) || SpellHelper.IsIlshenar( map, Caster.Location ) )
                {
                    Caster.SendLangMessage( 10000701 ); // "You cannot use here this powerfull miracle."
                }
                else if( CheckSequence() )
                {
                    Caster.BeginAction( typeof( PathToHeavenSpell ) );

                    PaladinSystem.PlayRandomWhiteSparkle( Caster );
                    Caster.PlaySound( 0x215 );

                    Caster.MoveToWorld( m_HeavenLocation, m_HeavenMap );
                    BaseCreature.TeleportPets( Caster, m_HeavenLocation, m_HeavenMap, false );

                    Caster.PlaySound( 0x215 );
                    PaladinSystem.PlayRandomWhiteSparkle( Caster );

                    Timer.DelayCall( GetDelayOfReuseInSeconds(), new TimerStateCallback( ReleasePathToHeavenLock ), Caster );
                }
            }
            else
                Caster.SendLangMessage( 10000702 ); // "You cannot find another path to heaven yet!"

            FinishSequence();
        }

        private static void ReleasePathToHeavenLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( PathToHeavenSpell ) );
            ( (Mobile)state ).SendLangMessage( 10000703 ); // "Heaven is now really near..."
        }
    }
}