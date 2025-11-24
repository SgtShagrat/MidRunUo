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
	public class MageSummonerAI : BaseAI
	{
	    protected DateTime m_NextCastTime; // mod by Dies Irae
	    protected DateTime m_NextSummonTime; // mod by Dies Irae

		public MageSummonerAI( BaseCreature m ) : base( m )
		{
		}

		public override bool Think()
		{
			if ( m_Mobile.Deleted )
				return false;

			if ( ProcessTarget() )
				return true;
			else
			{
				CheckMinions(); //mod by Magius(CHE): minions
				return base.Think();
			}
		}

		public override bool Obey()
		{
			if ( ProcessTarget() )
				return true;
			else if (DateTime.Now > m_NextSummonTime)
			{
				if (Utility.Random(100) < 4)
					m_Mobile.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( "Master, at your service." ) );
				CheckMinions();

				m_NextSummonTime = DateTime.Now + GetDelay() + GetDelay() + GetDelay();
			}
			else if (m_NextSummonTime == null)
			{
				m_NextSummonTime = DateTime.Now + GetDelay();
			}

			return base.Obey();
		}

		public virtual bool SmartAI
		{
			get{ return ( m_Mobile is BaseVendor || m_Mobile is BaseEscortable || m_Mobile.Summoned ); } // mod by Dies Irae
		}

		//private const double HealChance = 0.10; // 10% chance to heal at gm magery
		//private const double TeleportChance = 0.05; // 5% chance to teleport at gm magery
		//private const double DispelChance = 0.75; // 75% chance to dispel at gm magery

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
			else if ( SmartAI && m_Mobile.Mana < m_Mobile.ManaMax )
				m_Mobile.UseSkill( SkillName.Meditation );
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
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				m_Mobile.DebugSay( "I am stuck" );
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
		
		#region Mod by Magius(CHE): summon minions
		
		public virtual void CheckMinions()
		{
			//if (m_Mobile.Summoned)
			//	return; //summoned creatures do not controls it's minions :)
			
			var minions = new List<BaseCreature>();
			
			IPooledEnumerable eable = m_Mobile.Map.GetMobilesInRange( new Point3D( m_Mobile.Location ), 8 );
			
			try
			{
			
				foreach ( Mobile minion in eable )
				{				
					var bc = minion as BaseCreature;
					if (bc!=null && bc.GetMaster() == m_Mobile.GetMaster() )
						minions.Add( bc );
				}
			}			
			finally
			{
				eable.Free();
			}
			
			if (minions.Count == 0)
				return;

			
			foreach(var minion in minions)
			{
				if (m_Mobile.Combatant != null && minion.Combatant != m_Mobile.Combatant && m_Mobile.Combatant!=minion)
				{
					minion.ControlTarget = m_Mobile.Combatant;
					minion.ControlOrder = OrderType.Attack;	
					minion.Combatant = m_Mobile.Combatant;
					minion.FocusMob = minion.Combatant;
					if (Utility.Random(100) < 4)
						m_Mobile.Emote("Minions, serve your master!");
				} 
				else if ( 
				         ( minion.ControlOrder == OrderType.Attack && minion.Combatant==m_Mobile )
				      || ( minion.ControlOrder != OrderType.Guard && minion.ControlOrder != OrderType.Attack )
				        )
				{
					if (Utility.Random(100) < 4)
						m_Mobile.Emote("Follow me!");
					minion.FocusMob = null;
					minion.ControlTarget = null;
					minion.Combatant = null;
					minion.ControlOrder = OrderType.Guard;
				}
			}
		}

		public override bool DoOrderRelease()
		{
			var minions = new List<BaseCreature>();
			
			IPooledEnumerable eable = m_Mobile.Map.GetMobilesInRange( new Point3D( m_Mobile.Location ), 8 );
			
			foreach ( Mobile minion in eable )
			{				
				var bc = minion as BaseCreature;
				if (bc!=null && bc.GetMaster() == m_Mobile.GetMaster() )
					minions.Add( bc );
			}

			eable.Free();

			foreach(var minion in minions)
			{
				minion.ControlTarget = null;
				minion.ControlOrder = OrderType.Release;
			}

			return base.DoOrderRelease();
		}

		public virtual Spell GetRandomSummonSpell()
		{
			int maxCircle = (int)((m_Mobile.Skills[SkillName.Magery].Value /*+ 20.0*/) / (10.0 /*/ 7.0*/));
			if ( maxCircle < 1 )
				maxCircle = 1;
			
			var castable = new List<Type>();
			
			if (maxCircle>=4 && SummonMinionSpell.CanSummonOtherMinions(m_Mobile))
			{
				for(var chance = 0; chance<4; chance++)
					castable.Add(typeof( SummonMinionSpell ));
			}
			if (maxCircle>=8)
			{
				switch (Utility.Random(5))
				{
				case 0:
					castable.Add(typeof( AirElementalSpell ));
					break;
				case 1:
					castable.Add(typeof( EarthElementalSpell ));
					break;
				case 2:
					castable.Add(typeof( EnergyVortexSpell ));
					break;
				case 3:
					castable.Add(typeof( FireElementalSpell ));
					break;
				case 4:
					castable.Add(typeof( WaterElementalSpell ));
					break;
				}
			}
			if (maxCircle>=9)
				castable.Add(typeof( SummonDaemonSpell ));
				
			if (castable.Count==0)
				return null;
			
			var spelltype = castable[Utility.Random(castable.Count)];
			
			return (Spell)Activator.CreateInstance(spelltype, m_Mobile, null);
		}
		#endregion

		public virtual Spell ChooseSpell()
		{
			return GetRandomSummonSpell();
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

				Spell spell = ChooseSpell();

				// Now we have a spell picked
				// Move first before casting

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
		
			bool isSummoning = ( targ is EnergyVortexSpell.InternalTarget ); // Mod by Magius(CHE) : Minions

			Mobile toTarget;

			if (isSummoning)
			{
				toTarget = m_Mobile.Combatant;
				
				if (toTarget!=null && targ !=null)
				{
					if (( targ is EnergyVortexSpell.InternalTarget ) && targ.Range == -1 || m_Mobile.InRange( toTarget, targ.Range ))
					{
						var p = m_Mobile.Combatant.Location;
						if (!SpellHelper.FindValidSpawnLocation( m_Mobile.Combatant.Map, ref p, false ))
						{
							p = m_Mobile.Location;
							if (!SpellHelper.FindValidSpawnLocation( m_Mobile.Map, ref p, false )) //if fails, no valid location will be founded.
							{
								
								p = Point3D.Zero;
							}
						}
						if (p != Point3D.Zero)
							targ.Invoke( m_Mobile, new LandTarget(p,m_Mobile.Combatant.Map));
						else
							targ.Cancel( m_Mobile, TargetCancelType.Canceled );
					}
					else
						targ.Invoke( m_Mobile, m_Mobile );
				}
			}
			else
			{
				toTarget = m_Mobile.Combatant;

				Mobile master = m_Mobile.GetMaster();
				if ( master != null )
					RunTo( master );
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