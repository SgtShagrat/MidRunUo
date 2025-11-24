using System;
using System.Collections;
using System.Collections.Generic;

using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Misc;
using Server.Regions;
using Server.SkillHandlers;

namespace Server.Mobiles
{
	public class MageHealerAI : BaseAI
	{
		protected DateTime m_NextCastTime;
		protected DateTime m_NextHealTime;
		protected Mobile m_found;

		public MageHealerAI( BaseCreature m ) : base( m )
		{
		}

		public override bool Think()
		{
			if ( m_Mobile.Deleted )
				return false;

			if ( ProcessTarget() )
				return true;
			else
				return base.Think();
		}

		public override bool Obey()
		{
			if ( ProcessTarget() )
				return true;
			else if (DateTime.Now > m_NextCastTime)
			{
				Spell spell = CheckCastHealingSpell();

				if ( spell != null )
					spell.Cast();

				m_NextCastTime = DateTime.Now + GetDelay();
			}
			else if (m_NextCastTime == null)
			{
				m_NextCastTime = DateTime.Now + GetDelay();
			}

			return base.Obey();
		}

		//private const double HealChance = 0.10; // 10% chance to heal at gm magery

		public virtual double ScaleByMagery( double v )
		{
			return m_Mobile.Skills[SkillName.Magery].Value * v * 0.01;
		}

		public override bool DoActionWander()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
				m_NextCastTime = DateTime.Now;
			}
			else if ( m_Mobile.Mana < m_Mobile.ManaMax )
			{
				m_Mobile.UseSkill( SkillName.Meditation );
			}
			else
			{
				m_Mobile.Warmode = false;

				base.DoActionWander();

			}

			return true;
		}

		private Spell CheckCastHealingSpell()
		{
			if ( DateTime.Now < m_NextHealTime )
				return null;

			m_found = FindCureTarget();

			//check se ci sono più poisonati
			if ( m_found != null && m_found != m_Mobile && m_found.Poisoned && m_Mobile.Poisoned )
				return new ArchCureSpell( m_Mobile, null );
			else if ( m_found != null && m_found.Poisoned )
				return new CureSpell( m_Mobile, null );

			m_found = FindHealTarget();

			//if ( toTarget == null )
			//	return null;

			//if ( Utility.Random( 0, 4 + (m_Mobile.Hits == 0 ? m_Mobile.HitsMax : (m_Mobile.HitsMax / m_Mobile.Hits)) ) < 3 )
			//	return null;

			Spell spell = null;

			if ( m_found != null && ( m_found.Hits < m_found.HitsMax - 20) )
			{
				spell = new GreaterHealSpell( m_Mobile, null );

				if ( spell == null )
					spell = new HealSpell( m_Mobile, null );
			}
			else if ( m_found != null && ( m_found.Hits < m_found.HitsMax - 8) )
				spell = new HealSpell( m_Mobile, null );

			double delay = Utility.RandomMinMax( 7, 15 );

			if (m_Mobile.Experience > 0)
				delay -= m_Mobile.Experience;

			if (delay < 1 )
				delay = 1;

			m_NextHealTime = DateTime.Now + TimeSpan.FromSeconds( delay );

			return spell;
		}

		public void RunTo( Mobile m )
		{
			if ( m.Paralyzed || m.Frozen )
			{
				if ( m_Mobile.InRange( m, 1 ) )
					RunFrom( m );
				else if ( !m_Mobile.InRange( m, m_Mobile.RangeFight > 2 ? m_Mobile.RangeFight : 2 ) && !MoveTo( m, true, 1 ) )
					OnFailedMove();
			}
			else
			{
				if ( !m_Mobile.InRange( m, m_Mobile.RangeFight ) )
				{
					if ( !MoveTo( m, true, 1 ) )
						OnFailedMove();
				}
				else if ( m_Mobile.InRange( m, m_Mobile.RangeFight - 1 ) )
				{
					RunFrom( m );
				}
			}
		}

		public void RunFrom( Mobile m )
		{
			Run( (m_Mobile.GetDirectionTo( m ) - 4) & Direction.Mask );
		}

		public void OnFailedMove()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
		}

		public void Run( Direction d )
		{
			if ( (m_Mobile.Spell != null && m_Mobile.Spell.IsCasting) || m_Mobile.Paralyzed || m_Mobile.Frozen || m_Mobile.DisallowAllMoves )
				return;

			m_Mobile.Direction = d | Direction.Running;

			if ( !DoMove( m_Mobile.Direction, true ) )
				OnFailedMove();
		}

		public virtual Spell ChooseSpell()
		{
			return CheckCastHealingSpell();
		}

		private TimeSpan GetDelay()
		{
			double del = ScaleByMagery( 3.0 );
			double min = 6.0 - (del * 0.75);
			double max = 6.0 - (del * 1.25);

			return TimeSpan.FromSeconds( min + ((max - min) * Utility.RandomDouble()) );
		}

		public override bool DoActionCombat()
		{
			Mobile c = m_Mobile.Combatant;
			m_Mobile.Warmode = true;

			if ( c == null || c.Deleted || !c.Alive || c.IsDeadBondedPet || !m_Mobile.CanSee( c ) || !m_Mobile.CanBeHarmful( c, false ) || c.Map != m_Mobile.Map )
			{
				// Our combatant is deleted, dead, hidden, or we cannot hurt them
				// Try to find another combatant

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "Something happened to my combatant, so I am going to fight {0}", m_Mobile.FocusMob.Name );

					m_Mobile.Combatant = c = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else
				{
					m_Mobile.DebugSay( "Something happened to my combatant, and nothing is around. I am on guard." );
					Action = ActionType.Guard;					
					return true;
				}
			}

			if ( !m_Mobile.InLOS( c ) )
			{
				m_Mobile.DebugSay( "I can't see my target" );

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.DebugSay( "Nobody else is around" );
					m_Mobile.Combatant = c = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
			}

			if ( !m_Mobile.InRange( c, m_Mobile.RangePerception ) )
			{
				// They are somewhat far away, can we find something else?

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if ( !m_Mobile.InRange( c, m_Mobile.RangePerception * 3 ) )
				{
					m_Mobile.Combatant = null;
				}

				c = m_Mobile.Combatant;

				if ( c == null )
				{
					m_Mobile.DebugSay( "My combatant has fled, so I am on guard" );
					Action = ActionType.Guard;

					return true;
				}
			}

			if ( !m_Mobile.Controlled && !m_Mobile.Summoned && !m_Mobile.IsParagon )
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax * 20/100 )
				{
					// We are low on health, should we flee?

					bool flee = false;

					if ( m_Mobile.Hits < c.Hits )
					{
						// We are more hurt than them

						int diff = c.Hits - m_Mobile.Hits;

						flee = ( Utility.Random( 0, 100 ) > (10 + diff) ); // (10 + diff)% chance to flee
					}
					else
					{
						flee = Utility.Random( 0, 100 ) > 10; // 10% chance to flee
					}
					
					if ( flee )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "I am going to flee from {0}", c.Name );

						Action = ActionType.Flee;
						return true;
					}
				}
			}

			if ( m_Mobile.Spell == null && DateTime.Now > m_NextCastTime && m_Mobile.InRange( c, Core.ML ? 10 : 12 ) )
			{
				Spell spell = ChooseSpell();

				// Now we have a spell picked
				// Move first before casting
				Mobile master = m_Mobile.GetMaster();
				if ( master != null )
					RunTo( master );

				if ( spell != null )
					spell.Cast();

				TimeSpan delay = GetDelay();

				m_NextCastTime = DateTime.Now + delay;
			}
			else if ( m_Mobile.Spell == null || !m_Mobile.Spell.IsCasting )
			{
				Mobile master = m_Mobile.GetMaster();
				if ( master != null )
					RunTo( master );
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				ProcessTarget();

				base.DoActionGuard();
			}

			return true;
		}

		public override bool DoActionFlee()
		{
			Mobile c = m_Mobile.Combatant;

			if ( (m_Mobile.Mana > 20 || m_Mobile.Mana == m_Mobile.ManaMax) && m_Mobile.Hits > (m_Mobile.HitsMax / 2) )
			{
				Action = ActionType.Guard;
			}
			else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				RunFrom( m_Mobile.FocusMob );
				m_Mobile.FocusMob = null;

				if ( m_Mobile.Poisoned && Utility.Random( 0, 5 ) == 0 )
					new CureSpell( m_Mobile, null ).Cast();
			}
			else
			{
				Action = ActionType.Guard;
				m_Mobile.Warmode = true;
			}

			return true;
		}

		public Mobile FindHealTarget()
		{
			m_Mobile.DebugSay( "*Searching Heal target*" );

			if ( m_Mobile.Deleted || !(m_Mobile is BaseCreature) )
				return null;

			BaseCreature bc = (BaseCreature)m_Mobile;
			Mobile master = bc.GetMaster();

			if ( master != null && (master.Hits < master.HitsMax - 5) )
				return master;
			else if ( m_Mobile.Hits < m_Mobile.HitsMax - 5 && bc.Experience > 0)
				return m_Mobile;
			else if ( bc.Experience > 1 )
			{
				m_Mobile.DebugSay( "*other*" );

				foreach( Mobile mobile in m_Mobile.GetMobilesInRange( 6 ) )
				{
					if( !m_Mobile.InLOS( mobile ) )
						continue;

					if( mobile is BaseNecroFamiliar )
					{
						BaseCreature bctwo = (BaseCreature)mobile;

						Mobile masterTwo = bctwo.GetMaster();
						if( masterTwo != null && masterTwo == master && (mobile.Hits < mobile.HitsMax -5))//altro summonato
							return mobile;

					}
				}//TODO: aggiungere stesso party/gruppo
			}

			return null;
		}

		public Mobile FindCureTarget()
		{
			if ( m_Mobile.Deleted || !(m_Mobile is BaseCreature) )
				return null;

			BaseCreature bc = (BaseCreature)m_Mobile;
			Mobile master = bc.GetMaster();

			if ( master != null && master.Poisoned )
				return master;
			else if ( m_Mobile.Poisoned )
				return m_Mobile;
			else if ( bc.Experience > 1 )
			{
				foreach( Mobile mobile in m_Mobile.GetMobilesInRange( 6 ) )
				{
					if( !m_Mobile.InLOS( mobile ) )
						continue;

					if( mobile is BaseNecroFamiliar )
					{
						BaseCreature bctwo = (BaseCreature)mobile;

						Mobile masterTwo = bctwo.GetMaster();
						if( masterTwo != null && masterTwo == master && mobile.Poisoned)//altro summonato
							return mobile;

					}
				}//TODO: aggiungere stesso party/gruppo
			}

			return null;
		}

		private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				 0, -1,
				 0,  1,
				 1, -1,
				 1,  0,
				 1,  1,

				-2, -2,
				-2, -1,
				-2,  0,
				-2,  1,
				-2,  2,
				-1, -2,
				-1,  2,
				 0, -2,
				 0,  2,
				 1, -2,
				 1,  2,
				 2, -2,
				 2, -1,
				 2,  0,
				 2,  1,
				 2,  2
			};

		private bool ProcessTarget()
		{
			Target targ = m_Mobile.Target;
			
			if ( targ == null )
				return false;

			//bool isHeal = ( targ is HealSpell.InternalTarget || targ is GreaterHealSpell.InternalTarget );
			//bool isCure = ( targ is CureSpell.InternalTarget || targ is ArchCureSpell.InternalTarget );
			Mobile toTarget = m_found;//(isHeal ? FindHealTarget() : FindCureTarget());

			if ( toTarget != null && (targ.Range == -1 || m_Mobile.InRange( toTarget, targ.Range )) && m_Mobile.CanSee( toTarget ) && m_Mobile.InLOS( toTarget ) )
			{
				targ.Invoke( m_Mobile, toTarget );
				m_found = null;
			}
			else
				targ.Cancel( m_Mobile, TargetCancelType.Canceled );

			return true;
		}
	}
}
