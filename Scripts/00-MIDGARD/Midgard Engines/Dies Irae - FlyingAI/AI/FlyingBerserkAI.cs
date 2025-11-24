using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Regions;

namespace Server.Mobiles
{
	public class FlyingBerserkAI : BaseAI
	{
		public FlyingBerserkAI(BaseCreature m) : base (m)
		{
		}
	
		public override bool DoActionWander()
		{
			if ( ((FlyingCreature)m_Mobile).CanFly == false )
			{
				((FlyingCreature)m_Mobile).IsFlying = false;
			}

			m_Mobile.DebugSay( "I have No Combatant" );
			
			if( AcquireFocusMob( m_Mobile.RangePerception, FightMode.Closest, false, true, true) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected " + m_Mobile.FocusMob.Name + " and I will attack" );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else if ( ((FlyingCreature)m_Mobile).IsFlying == true )
			{
				return false;
			}		
			else
			{
				base.DoActionWander();
			}

			return true;			
		}
			
		public override bool DoActionCombat()
		{
			FlyingCreature fbc = (FlyingCreature)m_Mobile;
			
			if ( fbc.IsFlying == true )
				fbc.IsFlying = false;

			if( m_Mobile.Combatant == null || m_Mobile.Combatant.Deleted )
			{
				m_Mobile.DebugSay("My combatant is deleted");
				Action = ActionType.Guard;
				return true;
			}

			if( WalkMobileRange( m_Mobile.Combatant, 1, true, m_Mobile.RangeFight, m_Mobile.RangeFight ) )
			{
				// Be sure to face the combatant
				m_Mobile.Direction = m_Mobile.GetDirectionTo( m_Mobile.Combatant.Location );
			}
			else
			{
				if( m_Mobile.Combatant != null )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay("I am still not in range of " + m_Mobile.Combatant.Name);

					if( (int) m_Mobile.GetDistanceToSqrt( m_Mobile.Combatant ) > m_Mobile.RangePerception + 1 )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "I have lost " + m_Mobile.Combatant.Name );

						Action = ActionType.Guard;
						return true;
					}
				}
			}
			
			return true;
		}
		
		public override bool DoActionGuard()
		{
			if ( ((FlyingCreature)m_Mobile).CanFly == true && ((FlyingCreature)m_Mobile).IsFlying == false )
			{
				((FlyingCreature)m_Mobile).FlyStam = ((FlyingCreature)m_Mobile).FlyStamMax;
			}

			if ( AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, true, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionGuard();
			}

			return true;
		}
	}
}
