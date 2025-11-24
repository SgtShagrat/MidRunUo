/***************************************************************************
 *                                  GetInfoTimer.cs
 *                            		---------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.AdvancedDisguise
{
    public class GetInfoTimer : Timer
    {
        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        private readonly int m_MaxCount;
        private int m_Count;
        private readonly DisguiseEntry m_Entry;

        public GetInfoTimer( DisguiseEntry entry, Mobile from, Mobile target, int count )
            : base( TimeSpan.FromSeconds( 10.0 ), TimeSpan.FromSeconds( 10.0 ), count )
        {
            m_From = from;
            m_Target = target;
            m_MaxCount = count;
            m_Entry = entry;

            Priority = TimerPriority.TwoFiftyMS;
        }

        protected override void OnTick()
        {
            m_Count++;

            if( m_Entry == null || m_From == null || m_Target == null )
            {
                ReleaseTimer();
                return;
            }

            if( m_Entry.IsFullEntry )
            {
                ReleaseTimer();
                return;
            }

            if( !m_From.InRange( m_Target, 6 ) )
            {
                m_Target.SendMessage( "You are too far away from your target." );
                ReleaseTimer();
            }
            else if( !m_From.CheckAlive() )
            {
                m_Target.SendMessage( "You are dead. Yuor study ends now." );
                ReleaseTimer();
            }
            else if( !m_From.CanSee( m_Target ) || !m_From.InLOS( m_Target ) )
            {
                m_Target.SendMessage( "You cannot see clearly your target. Yuor study ends now." );
                ReleaseTimer();
            }
            else if( m_Count < m_MaxCount )
            {
                bool success = CheckInfo( m_From, m_Target );

                if( !success )
                {
                    switch( Utility.Random( 3 ) )
                    {
                        case 0:
                            m_From.SendMessage( "Well... just stay near your target and continue studying." );
                            break;
                        case 1:
                            m_From.SendMessage( "Another little bit..." );
                            break;
                        case 2:
                            m_From.SendMessage( "Not bad, you are near to get some relevant info." );
                            break;
                    }
                }
                else
                {
                    DisguiseEntry.AdvanceEntry( m_Entry, m_Target );
                    if( !m_Entry.IsFullEntry )
                        m_From.SendMessage( "Well done: you got some important info. Your alias has advanced." );
                    else
                        m_From.SendMessage( "Perfect. Your alias is completed." );

                    ReleaseTimer();
                }
            }
            else
            {
                m_From.SendMessage( "You cannot focus on that target. Let's retry." );
                ReleaseTimer();
            }
        }

        private void ReleaseTimer()
        {
            if( Running )
                Stop();

            if( m_From != null )
                m_From.EndAction( typeof( SketchGump ) );
        }

        private bool CheckInfo( Mobile from, Mobile target )
        {
            if( target == null || !( target is Midgard2PlayerMobile ) )
                return false;

            double baseChance = from.Skills[ SkillName.Stealing ].Value / 120.0;

            double bonusInt = from.Int / 100.0;
            if( bonusInt < 0.7 )
                bonusInt = 0.7;
            else if( bonusInt > 1.2 )
                bonusInt = 1.2;

            double bonusDinstance = -0.14 * from.GetDistanceToSqrt( target.Location ) + 1.5;
            if( bonusDinstance < 0.1 )
                bonusDinstance = 0.1;
            else if( bonusDinstance > 1.5 )
                bonusDinstance = 1.5;

            double bonusInfoType = 1;
            DisguiseEntry.InfoTypes nextReq = m_Entry.NextTypeRequired();
            switch( nextReq )
            {
                case DisguiseEntry.InfoTypes.Physical:
                    bonusInfoType = 1.0;
                    break;
                case DisguiseEntry.InfoTypes.Reputation:
                    bonusInfoType = 0.7;
                    break;
                case DisguiseEntry.InfoTypes.Town:
                    bonusInfoType = 0.5;
                    break;
                case DisguiseEntry.InfoTypes.Other:
                    bonusInfoType = 0.3;
                    break;
            }

            double chance = baseChance * bonusInt * bonusDinstance * bonusInfoType;

            from.SendMessage( "Debug CheckInfo: baseChance {0} - bonusInt {1} - bonusDinstance {2} - bonusInfoType {3} - chance {4}.",
                             baseChance.ToString( "F2" ), bonusInt.ToString( "F2" ), bonusDinstance.ToString( "F2" ), bonusInfoType.ToString( "F2" ), chance.ToString( "F2" ) );

            return ( chance > Utility.RandomDouble() );
        }
    }
}