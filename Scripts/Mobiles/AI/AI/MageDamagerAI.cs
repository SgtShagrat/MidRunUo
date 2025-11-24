using System;
using System.Collections;
using System.Collections.Generic;

using Server.Spells.Eighth;
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
	public class MageDamagerAI : BaseAI
	{
		protected DateTime m_NextCastTime; // mod by Dies Irae
		//protected DateTime m_NextHealTime; // mod by Dies Irae

		public MageDamagerAI( BaseCreature m ) : base( m )
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

		public virtual bool SmartAI
		{
			get{ return ( m_Mobile is BaseVendor || m_Mobile is BaseEscortable || m_Mobile.Summoned ); } // mod by Dies Irae
		}

		public virtual double ScaleByMagery( double v )
		{
			return m_Mobile.Skills[SkillName.Magery].Value * v * 0.01;
		}

		public override bool DoActionWander()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
				m_NextCastTime = DateTime.Now;
			}
			else if ( SmartAI && m_Mobile.Mana < m_Mobile.ManaMax )
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

		public void RunTo( Mobile m )
		{
			if ( !SmartAI )
			{
				if ( !MoveTo( m, true, m_Mobile.RangeFight ) )
					OnFailedMove();

				return;
			}

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

		public virtual Spell GetRandomDamageSpell()
		{
			int maxCircle = (int)((m_Mobile.Skills[SkillName.Magery].Value + 20.0) / (100.0 / 7.0));

			if ( maxCircle < 1 )
				maxCircle = 1;
			
			#region Mod by Magius(CHE): best efficency
			
			var castable = new List<Type>();
			
			if ( maxCircle >= 1)
			{
				castable.Add(typeof( MagicArrowSpell ));
			}
			if ( maxCircle >= 2)
			{
				castable.Add(typeof( HarmSpell ));
			}
			if ( maxCircle >= 3)
			{
				castable.Add(typeof( FireballSpell ));
				castable.Add(typeof( PoisonSpell ));
			}
			if ( maxCircle >= 4)
			{
				castable.Add(typeof( LightningSpell ));
				castable.Add(typeof( LightningSpell ));
				castable.Add(typeof( FireFieldSpell ));
			}
			if ( maxCircle >= 5)
			{
				castable.Add(typeof( MindBlastSpell ));
				castable.Add(typeof( MindBlastSpell ));
				castable.Add(typeof( ParalyzeSpell ));
				castable.Add(typeof( PoisonFieldSpell ));
			}
			if ( maxCircle >= 6)
			{
				castable.Add(typeof( EnergyBoltSpell ));
				castable.Add(typeof( EnergyBoltSpell ));
				castable.Add(typeof( ExplosionSpell ));
				castable.Add(typeof( ExplosionSpell )); // TODO fix a explosion, Dies
				castable.Add(typeof( ParalyzeFieldSpell ));
			}
			if ( maxCircle >= 7)
			{
				castable.Add(typeof( FlameStrikeSpell ));
				castable.Add(typeof( FlameStrikeSpell ));
				castable.Add(typeof( EnergyFieldSpell ));
				/* if multienemy checked */
				if (m_Mobile.Aggressors.Count>0)
				{
					castable.Add(typeof( MeteorSwarmSpell ));
					castable.Add(typeof( ChainLightningSpell ));
				}
			}
			if ( maxCircle >= 8)
			{
				castable.Add(typeof( EarthquakeSpell ));
			}
			
			if (castable.Count==0)
				return null;
			
			var spelltype = castable[Utility.Random(castable.Count)];
			
			return (Spell)Activator.CreateInstance(spelltype, m_Mobile, null);
			#endregion
		}

		public virtual Spell ChooseSpell( Mobile c )
		{
			//Spell spell = GetRandomDamageSpell();
			return GetRandomDamageSpell();
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

			if ( SmartAI && !m_Mobile.StunReady && m_Mobile.Skills[SkillName.Wrestling].Value >= 80.0 && m_Mobile.Skills[SkillName.Anatomy].Value >= 80.0 )
				EventSink.InvokeStunRequest( new StunRequestEventArgs( m_Mobile ) );

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
				// We are ready to cast a spell

				Spell spell = ChooseSpell( c );
				//RunTo( c );//RunFrom?
				Mobile master = m_Mobile.GetMaster();
				if ( master != null )
					RunTo( master );

				if ( spell != null )
					spell.Cast();

				m_NextCastTime = DateTime.Now + GetDelay();
			}
			else if ( m_Mobile.Spell == null || !m_Mobile.Spell.IsCasting )
			{
				Mobile master = m_Mobile.GetMaster();
				if ( master != null )
					RunTo( master );
				//else
				//	RunTo( c );
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
				base.DoActionGuard();

			return true;
		}

		public override bool DoActionFlee()
		{
			Mobile c = m_Mobile.Combatant;

			if ( (m_Mobile.Mana > 20 || m_Mobile.Mana == m_Mobile.ManaMax) && m_Mobile.Hits > (m_Mobile.HitsMax / 2) )
			{
				m_Mobile.DebugSay( "I am stronger now, my guard is up" );
				Action = ActionType.Guard;
			}
			else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am scared of {0}", m_Mobile.FocusMob.Name );

				RunFrom( m_Mobile.FocusMob );
				m_Mobile.FocusMob = null;
			}
			else
			{
				m_Mobile.DebugSay( "Area seems clear, but my guard is up" );

				Action = ActionType.Guard;
				m_Mobile.Warmode = true;
			}

			return true;
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

			//bool isDispel = ( targ is DispelSpell.InternalTarget );
			//bool isParalyze = ( targ is ParalyzeSpell.InternalTarget );
			//bool isTeleport = ( targ is TeleportSpell.InternalTarget );
			bool isAreaDamage = ( targ is ExplosionSpell.OldInternalTarget ) || ( targ is PoisonFieldSpell.InternalTarget ) || ( targ is FireFieldSpell.InternalTarget ) || ( targ is EnergyFieldSpell.InternalTarget ) || ( targ is ChainLightningSpell.InternalTarget ) || ( targ is MeteorSwarmSpell.InternalTarget );
			
			//bool isSummoning = ( targ is EnergyVortexSpell.InternalTarget ); // Mod by Magius(CHE) : Minions

			Mobile toTarget;

			#region Mod by Magius(CHE): area spells
			if (isAreaDamage)
			{
				toTarget = m_Mobile.Combatant;
				
				if (toTarget!=null && targ !=null)
				{
					//dovrebbe individuare il punto migliore ma invece casta all'aggressor + lontano...
					foreach(var aggr in m_Mobile.Aggressors)
					{
						toTarget = aggr.Attacker;						
						if (m_Mobile.InRange( toTarget, targ.Range ) && m_Mobile.InLOS(toTarget) )
							break;
						else
							toTarget=null;
					}
					if (toTarget!=null)
					{
						var p = toTarget.Location;
						targ.Invoke( m_Mobile, new LandTarget(p,toTarget.Map));
					}
					else
						targ.Cancel( m_Mobile, TargetCancelType.Canceled );
				}
			}
			#endregion
			else
			{
				toTarget = m_Mobile.Combatant;

				if ( m_Mobile.GetMaster() != null )
					RunTo( m_Mobile.GetMaster() );

				//if ( toTarget != null )
				//	RunTo( toTarget );
			}

			if ( (targ.Flags & TargetFlags.Harmful) != 0 && toTarget != null )
			{
				if ( (targ.Range == -1 || m_Mobile.InRange( toTarget, targ.Range )) && m_Mobile.CanSee( toTarget ) && m_Mobile.InLOS( toTarget ) )
					targ.Invoke( m_Mobile, toTarget );
			}
			else if ( (targ.Flags & TargetFlags.Beneficial) != 0 )
			{
				targ.Invoke( m_Mobile, m_Mobile );
			}
			else
			{
				targ.Cancel( m_Mobile, TargetCancelType.Canceled );
			}

			return true;
		}
	}
}
