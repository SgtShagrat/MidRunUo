using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Midgard.Engines.PetSystem
{
    public class PetMoves
    {
        public static void DoMoves( BaseCreature from, Mobile target )
        {
            if( !PetLeveling.AosPetMovesEnabled )
                return;

            switch( Utility.Random( 3 ) )
            {
                case 0:
                    #region roarattack
                    if( Utility.Random( 500 ) <= from.RoarAttack )
                    {
                        int power;
                        int mindam;
                        int maxdam;

                        if( from.RoarAttack > 3 )
                        {
                            mindam = from.RoarAttack / 3;
                            maxdam = from.RoarAttack / 2;
                        }
                        else
                        {
                            mindam = 1;
                            maxdam = 3;
                        }

                        if( from.RoarAttack > 10 )
                            power = from.RoarAttack / 10;
                        else
                            power = 1;

                        List<Mobile> targets = new List<Mobile>();

                        foreach( Mobile m in from.GetMobilesInRange( power ) )
                        {
                            if( m != from )
                                targets.Add( m );
                        }

                        for( int i = 0; i < targets.Count; ++i )
                        {
                            Mobile m = targets[ i ];

                            if( m is BaseCreature )
                            {
                                BaseCreature bc = (BaseCreature)m;

                                bc.BeginFlee( TimeSpan.FromSeconds( 30.0 ) );
                                AOS.Damage( target, from, Utility.RandomMinMax( mindam, maxdam ), 20, 20, 20, 20, 20 );
                            }
                        }
                    }
                    break;
                    #endregion

                case 1:
                    #region poisonattack
                    if( Utility.Random( 500 ) <= from.PetPoisonAttack )
                    {
                        Effects.SendLocationParticles( EffectItem.Create( target.Location, target.Map, EffectItem.DefaultDuration ), 0x36B0, 1, 14, 63, 7, 9915, 0 );
                        Effects.PlaySound( target.Location, target.Map, 0x229 );

                        int mindam;
                        int maxdam;

                        if( from.PetPoisonAttack > 3 )
                        {
                            mindam = from.PetPoisonAttack / 3;
                            maxdam = from.PetPoisonAttack / 2;
                        }
                        else
                        {
                            mindam = 1;
                            maxdam = 3;
                        }

                        int level = from.PetPoisonAttack / 20;

                        if( level > 5 )
                            level = 5;

                        target.ApplyPoison( from.ControlMaster, Poison.GetPoison( level ) );
                        AOS.Damage( target, from, Utility.RandomMinMax( mindam, maxdam ), 0, 0, 0, 0, 100 );
                    }
                    #endregion
                    break;

                case 2:
                    #region firebreath
                    if( Utility.Random( 500 ) <= from.FireBreathAttack )
                    {
                        int mindam;
                        int maxdam;

                        if( from.FireBreathAttack > 3 )
                        {
                            mindam = from.FireBreathAttack / 3;
                            maxdam = from.FireBreathAttack / 2;
                        }
                        else
                        {
                            mindam = 1;
                            maxdam = 3;
                        }

                        from.MovingParticles( target, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160 );
                        from.PlaySound( Core.AOS ? 0x15E : 0x44B );
                        target.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
                        target.PlaySound( 0x208 );

                        AOS.Damage( target, from, Utility.RandomMinMax( mindam, maxdam ), 0, 0, 0, 0, 100 );

                        Timer t = new FireBreathDOT( target, from, from.FireBreathAttack );
                        t.Start();
                    }
                    #endregion
                    break;
            }
        }

        public class FireBreathDOT : Timer
        {
            private Mobile m_Mobile;
            private Mobile m_From;
            private int m_Count;

            public FireBreathDOT( Mobile m, Mobile from, int count )
                : base( TimeSpan.FromSeconds( 10.0 ), TimeSpan.FromSeconds( 20.0 ) )
            {
                m_Mobile = m;
                m_From = from;
                m_Count = count;
            }

            protected override void OnTick()
            {
                int mindam;
                int maxdam;

                if( m_Count > 3 )
                {
                    mindam = m_Count / 3;
                    maxdam = m_Count / 2;
                }
                else
                {
                    mindam = 1;
                    maxdam = 3;
                }

                int total = 0;
                int tally = 0;

                if( m_Count > 20 )
                    total = m_Count / 20;
                else
                    total = 1;

                if( tally < total )
                {
                    tally += 1;

                    m_Mobile.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
                    m_Mobile.PlaySound( 0x208 );
                    AOS.Damage( m_Mobile, m_From, Utility.RandomMinMax( mindam, maxdam ), 0, 100, 0, 0, 0 );
                    m_Mobile.SendMessage( "You are on fire." );
                }

                if( tally == total )
                {
                    m_Mobile.SendMessage( "You have stoped burning." );
                    Stop();
                }
            }
        }
    }
}
