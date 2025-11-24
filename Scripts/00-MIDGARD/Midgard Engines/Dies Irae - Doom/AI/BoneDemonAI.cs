using System;

namespace Server.Mobiles
{
    public class BoneDemonAI : MageAI
    {
        private bool m_AntiDeadEnd_01_Active;
        private int m_AntiDeadEnd_01_State;

        public BoneDemonAI( BaseCreature m )
            : base( m )
        {
            m_AntiDeadEnd_01_Active = false;
            m_AntiDeadEnd_01_State = 0;
        }

        public virtual bool CheckAntiDeadEnd_01()
        {
            m_Mobile.DebugSay( "Where am I?" );

            if(
            ( ( m_Mobile.Location.X >= 370 ) && ( m_Mobile.Location.X <= 390 ) ) &&
            ( ( m_Mobile.Location.Y >= 122 ) && ( m_Mobile.Location.Y <= 143 ) ) &&
            ( ( m_Mobile.Location.Z >= -3 ) && ( m_Mobile.Location.Z <= 1 ) ) &&
            ( m_AntiDeadEnd_01_State == 0 )
               )
            {
                Map map = m_Mobile.Map;
                m_Mobile.DebugSay( "Bad players!" );

                if( map != null )
                {
                    IPooledEnumerable eable = map.GetMobilesInRange( m_Mobile.Location, 12 );
                    bool DeadEnd = false;

                    foreach( Mobile m in eable )
                    {
                        if( m.AccessLevel == AccessLevel.Player && m.Alive && !m.Blessed && !m.Deleted && ( m.Location.Z >= 30 ) )
                        {
                            m_AntiDeadEnd_01_Active = true;
                            m_AntiDeadEnd_01_State = 1;
                            m_Mobile.DebugSay( "Newbie players!" );
                            DeadEnd = true;

                            break;
                        }
                    }

                    eable.Free();
                    return DeadEnd;
                }


            }

            m_Mobile.DebugSay( "Good location..." );
            return false;
        }

        public virtual bool AntiDeadEnd_01_Think()
        {
            if( !m_AntiDeadEnd_01_Active )
                return true;

            if( ( m_Mobile.Location.X >= 380 ) && ( m_Mobile.Location.X <= 382 ) &&
                  ( m_Mobile.Location.Y >= 140 ) && ( m_Mobile.Location.Y <= 141 ) &&
                  ( m_Mobile.Location.Z <= 3 ) &&
                  ( m_AntiDeadEnd_01_State == 1 ) )
            {
                m_Mobile.DebugSay( "Go to stair..." );
                m_AntiDeadEnd_01_State = 2;

            }
            else if( ( m_Mobile.Location.X >= 380 ) && ( m_Mobile.Location.X <= 382 ) &&
                  ( m_Mobile.Location.Y >= 135 ) && ( m_Mobile.Location.Y <= 136 ) &&
                  ( m_Mobile.Location.Z >= 32 ) &&
                  ( m_AntiDeadEnd_01_State == 2 ) )
            {
                m_Mobile.DebugSay( "I am here..." );
                m_AntiDeadEnd_01_State = 3;

            }


            if( m_AntiDeadEnd_01_State == 1 )
            {
                m_Mobile.DebugSay( "Go to stair..." );
                Run( m_Mobile.GetDirectionTo( new Point3D( 381, 140, 0 ) ) );

                return false;
            }

            if( m_AntiDeadEnd_01_State == 2 )
            {
                m_Mobile.DebugSay( "Go to stair..." );
                Run( m_Mobile.GetDirectionTo( new Point3D( 381, 135, 33 ) ) );

                return false;
            }

            if( m_AntiDeadEnd_01_State == 3 )
            {
                return true;
            }

            return true;
        }

        public bool AntiDeadEnd_01()
        {
            bool think = m_AntiDeadEnd_01_Active;

            if( !think )
                think = CheckAntiDeadEnd_01();

            if( think )
            {
                if( AntiDeadEnd_01_Think() )
                {
                    m_Mobile.DebugSay( "Clear area..." );
                    m_AntiDeadEnd_01_Active = false;
                    m_AntiDeadEnd_01_State = 0;

                    if( AcquireFocusMob( 6, FightMode.Closest, false, false, true ) )
                    {
                        m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

                        m_Mobile.Combatant = m_Mobile.FocusMob;
                        Action = ActionType.Combat;
                        m_NextCastTime = DateTime.Now;
                    }
                }

                return true;
            }

            return false;
        }

        public override bool DoActionWander()
        {
            if( AntiDeadEnd_01() )
                return true;

            return base.DoActionWander();

            /*
            if( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
            {
                m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
                m_NextCastTime = DateTime.Now;
            }
            else if( SmartAI && m_Mobile.Mana < m_Mobile.ManaMax )
            {
                m_Mobile.DebugSay( "I am going to meditate" );

                m_Mobile.UseSkill( SkillName.Meditation );
            }
            else
            {
                m_Mobile.DebugSay( "I am wandering" );

                m_Mobile.Warmode = false;

                base.DoActionWander();

                if( m_Mobile.Poisoned )
                {
                    new CureSpell( m_Mobile, null ).Cast();
                }
                else if( SmartAI || ( ScaleByMagery( HealChance ) > Utility.RandomDouble() ) )
                {
                    if( m_Mobile.Hits < ( m_Mobile.HitsMax - 50 ) )
                    {
                        if( !new GreaterHealSpell( m_Mobile, null ).Cast() )
                            new HealSpell( m_Mobile, null ).Cast();
                    }
                    else if( m_Mobile.Hits < ( m_Mobile.HitsMax - 10 ) )
                    {
                        new HealSpell( m_Mobile, null ).Cast();
                    }
                }
            }

            return true;
            */
        }

        public override bool DoActionCombat()
        {
            m_Mobile.Warmode = true;

            if( AntiDeadEnd_01() )
                return true;

            return base.DoActionCombat();
        }
    }
}