using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
    public class DarkFatherAI : NecromageAI
    {
        public DarkFatherAI( BaseCreature m )
            : base( m )
        {
        }

        public override Spell ChooseSpell( Mobile c )
        {
            if( Utility.RandomDouble() < 0.10 )
            {
                if( m_Mobile.Mana < 40 && m_Mobile.Mana > 15 )
                {
                    if( c.Paralyzed && !c.Poisoned )
                    {
                        m_Mobile.DebugSay( "I am going to meditate" );

                        m_Mobile.UseSkill( SkillName.Meditation );
                    }
                    else if( !c.Poisoned )
                    {
                        return new ParalyzeSpell( m_Mobile, null );
                    }
                }
                else if( m_Mobile.Mana > 60 )
                {
                    if( Utility.Random( 2 ) == 0 && !c.Paralyzed && !c.Frozen && !c.Poisoned )
                    {
                        m_Combo = 0;
                        return new ParalyzeSpell( m_Mobile, null );
                    }
                    else
                    {
                        m_Combo = 1;
                        return new ExplosionSpell( m_Mobile, null );
                    }
                }
            }

            return base.ChooseSpell( c );
        }

        private bool MobileOnDoomShip( Mobile m )
        {
            if( ( m.Location.X >= 427 ) && ( m.Location.X <= 429 ) &&
                ( m.Location.Y >= 316 ) && ( m.Location.Y <= 329 ) &&
                ( m.Location.Z >= -1 ) && ( m.Location.Z <= 3 ) )
            {
                m_Mobile.DebugSay( "{0} at boat", m.Name );
                return true;
            }
            else
            {
                m_Mobile.DebugSay( "{0} at boat", m.Name );
                return false;
            }
        }

        public int PlayersOnDoomShip()
        {
            int Players = 0;
            Map map = m_Mobile.Map;

            IPooledEnumerable mobiles = map.GetMobilesInRange( new Point3D( 428, 323, 2 ), 8 );

            if( map == Map.Internal )
                return 0;

            foreach( Mobile m in mobiles )
            {
                bool bCheckIt = false;

                if( m.Player )
                {
                    if( m.AccessLevel == AccessLevel.Player && m.Alive && !m.Blessed && !m.Deleted )
                    {
                        bCheckIt = true;
                    }
                }

                if( bCheckIt && !MobileOnDoomShip( m ) )
                    bCheckIt = false;

                if( bCheckIt )
                    Players++;
            }

            mobiles.Free();

            m_Mobile.DebugSay( "At boat {0} players.", Players );

            return Players;
        }

        public bool NeedJumpToDoomShip()
        {
            if( !
                ( ( ( m_Mobile.Location.X >= 424 ) && ( m_Mobile.Location.X <= 425 ) &&
                ( m_Mobile.Location.Y >= 325 ) && ( m_Mobile.Location.Y <= 334 ) &&
                ( m_Mobile.Location.Z >= -2 ) && ( m_Mobile.Location.Z <= 0 ) )
                ||
                ( ( m_Mobile.Location.X >= 432 ) && ( m_Mobile.Location.X <= 433 ) &&
                ( m_Mobile.Location.Y >= 323 ) && ( m_Mobile.Location.Y <= 334 ) &&
                ( m_Mobile.Location.Z >= -3 ) && ( m_Mobile.Location.Z <= 0 ) ) )
                )
            {
                m_Mobile.DebugSay( "no boat" );

                return false;
            }

            if( PlayersOnDoomShip() == 0 )
            {
                m_Mobile.DebugSay( "nothing" );

                return false;
            }

            m_Mobile.DebugSay( "go to boat" );
            return true;
        }

        public override bool DoSpecialActions()
        {
            if( MobileOnDoomShip( m_Mobile ) )
            {
                if( PlayersOnDoomShip() == 0 )
                {
                    if( Utility.RandomMinMax( 0, 1 ) == 0 )
                        m_Mobile.Location = new Point3D( 424, 328, -1 );
                    else
                        m_Mobile.Location = new Point3D( 432, 331, -2 );

                    Action = ActionType.Guard;

                    return true;
                }

            }
            else if( NeedJumpToDoomShip() )
            {
                m_Mobile.Location = new Point3D( 428, 327, 2 );

                Action = ActionType.Guard;

                return true;
            }

            return false;
        }
    }
}