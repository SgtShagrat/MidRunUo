/***************************************************************************
 *                               HolyCircleSpell.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class HolyCircleSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Holy Circle", "Augus Luminos",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( HolyCircleSpell ),
            m_Info.Name,
            "This Miracle damages the evil creatures around the Paladin.",
            "Questo Miracolo danneggia le creature malvagie vicine al Paladino.",
            "Questo Miracolo danneggia le creature malvagie vicine al Paladino." +
            "Il danno vale: int damage = PowerValueScaled / 10.",
            0x510D
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Modificato il danno in PowerValueScaled / 10 danni.<br>" +
                "Modificto il raggio d'azione in PowerValueScaled / 20 passi.<br>";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Second; }
        }

        public HolyCircleSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override bool DelayedDamage
        {
            get { return true; }
        }

        public override int DelayOfReuseInSeconds
        {
            get { return 3; }
        }

        public override void OnCast()
        {
            if( SpellEffectContextHelper.IsUnderEffect( Caster, typeof( InvulnerabilitySpell ) ) )
            {
                Caster.SendMessage( "Thou cannot use this mighty miracle while in a such status." );
                FinishSequence();
                return;
            }

            if( Caster.CanBeginAction( typeof( HolyCircleSpell ) ) )
            {
                if( CheckSequence() )
                {
                    Caster.BeginAction( typeof( HolyCircleSpell ) );

                    List<Mobile> targets = new List<Mobile>();

                    int range = PowerValueScaled / 40; // from 0 to 285 / 40 = 7,125 = 7;
                    if( range < 3 )
                        range = 3;
                    else if( range > 7 )
                        range = 7;

                    foreach( Mobile m in Caster.GetMobilesInRange( range ) )
                    {
                        if( Caster != m && SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) && m.InLOS( Caster ) )
                        {
                            if( IsEnemy( m ) )
                                targets.Add( m );
                        }
                    }

                    Caster.PlaySound( 0x212 );
                    Caster.PlaySound( 0x206 );

                    Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 );
                    Effects.SendLocationParticles( EffectItem.Create( new Point3D( Caster.X, Caster.Y, Caster.Z - 7 ), Caster.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 );

                    foreach( Mobile m in targets )
                    {
                        int damage = PowerValueScaled / 10;

                        if( CheckResisted( m ) )
                        {
                            damage = (int)( damage * GetResistScalar( m ) );
                            m.SendMessage( "Thou feel yourself resisting the sacred power." );
                        }

                        if( damage < 8 )
                            damage = 8;

                        bool evil = m.Karma < 0 || m.Kills > 0 || m.Criminal;

                        if( !evil )
                            Caster.DoHarmful( m );

                        SpellHelper.Damage( this, m, damage, 0, 0, 0, 0, 100 );
                    }

                    Timer.DelayCall( GetDelayOfReuseInSeconds(), new TimerStateCallback( delegate( object o )
                                                                                            {
                                                                                                ( (Mobile)o ).EndAction( typeof( HolyCircleSpell ) );
                                                                                            } ), Caster );
                }
            }
            else
            {
                Caster.SendMessage( "Not enough time has passed from last circle." );
            }

            FinishSequence();
        }
    }
}