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
	public class MageAI : BaseAI
	{
	    protected DateTime m_NextCastTime; // mod by Dies Irae
	    protected DateTime m_NextHealTime; // mod by Dies Irae

		public MageAI( BaseCreature m ) : base( m )
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

		public virtual bool SmartAI
		{
			get{ return ( m_Mobile is BaseVendor || m_Mobile is BaseEscortable || m_Mobile.Summoned ); } // mod by Dies Irae
		}

		private const double HealChance = 0.10; // 10% chance to heal at gm magery
		private const double TeleportChance = 0.05; // 5% chance to teleport at gm magery
		private const double DispelChance = 0.75; // 75% chance to dispel at gm magery

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
			{
				m_Mobile.DebugSay( "I am going to meditate" );

				m_Mobile.UseSkill( SkillName.Meditation );
			}
			else
			{
				m_Mobile.DebugSay( "I am wandering" );

				m_Mobile.Warmode = false;

				base.DoActionWander();

				if ( !m_Mobile.Controlled )
				{
					Spell spell = CheckCastHealingSpell();

					if ( spell != null )
						spell.Cast();
				}
			}

			return true;
		}

		private Spell CheckCastHealingSpell()
		{
			// If I'm poisoned, always attempt to cure.
			if ( m_Mobile.Poisoned )
				return new CureSpell( m_Mobile, null );

			// Summoned creatures never heal themselves.
			if ( m_Mobile.Summoned )
				return null;

			if ( m_Mobile.Controlled )
			{
				if ( DateTime.Now < m_NextHealTime )
					return null;
			}

			if ( !SmartAI )
			{
				if ( ScaleByMagery( HealChance ) < Utility.RandomDouble() )
					return null;
			}
			else
			{
				if ( Utility.Random( 0, 4 + (m_Mobile.Hits == 0 ? m_Mobile.HitsMax : (m_Mobile.HitsMax / m_Mobile.Hits)) ) < 3 )
					return null;
			}

			Spell spell = null;

			if ( m_Mobile.Hits < (m_Mobile.HitsMax - 50) )
			{
				spell = new GreaterHealSpell( m_Mobile, null );

				if ( spell == null )
					spell = new HealSpell( m_Mobile, null );
			}
			else if ( m_Mobile.Hits < (m_Mobile.HitsMax - 10) )
				spell = new HealSpell( m_Mobile, null );

			double delay;

			if ( m_Mobile.Int >= 500 )
				delay = Utility.RandomMinMax( 7, 10 );
			else
				delay = Math.Sqrt( 600 - m_Mobile.Int );

			m_NextHealTime = DateTime.Now + TimeSpan.FromSeconds( delay );

			return spell;
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
			if ( !m_Mobile.DisallowAllMoves && (SmartAI ? Utility.Random( 4 ) == 0 : ScaleByMagery( TeleportChance ) > Utility.RandomDouble()) )
			{
				if ( m_Mobile.Target != null )
					m_Mobile.Target.Cancel( m_Mobile, TargetCancelType.Canceled );

				new TeleportSpell( m_Mobile, null ).Cast();

				m_Mobile.DebugSay( "I am stuck, I'm going to try teleporting away" );
			}
			else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
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
			if (m_Mobile.Summoned)
				return; //summoned creatures do not controls it's minions :)
			
			var minions = new List<BaseCreature>();
			
			IPooledEnumerable eable = m_Mobile.Map.GetMobilesInRange( new Point3D( m_Mobile.Location ), 8 );
			
			try
			{
			
				foreach ( Mobile minion in eable )
				{				
					var bc = minion as BaseCreature;
					if (bc!=null && bc.GetMaster() == m_Mobile)
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
					//m_Mobile.Emote("All Attack");
				} 
				else if ( 
				         ( minion.ControlOrder == OrderType.Attack && minion.Combatant==m_Mobile )
				      || ( minion.ControlOrder != OrderType.Guard && minion.ControlOrder != OrderType.Attack )
				        )
				{
					//m_Mobile.Emote("All Guard");
					minion.FocusMob = null;
					minion.ControlTarget = null;
					minion.Combatant = null;
					minion.ControlOrder = OrderType.Guard;
				}
			}
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
		public virtual Spell GetRandomStrongDamageSpell()
		{
			int maxCircle = (int)((m_Mobile.Skills[SkillName.Magery].Value + 20.0) / (5.0) );//0-24

			if ( maxCircle < 1 )
				maxCircle = 1;

			switch( Utility.Random( maxCircle ) )
			{
				case 0: {m_Mobile.DebugSay( "Lightning"); return new LightningSpell( m_Mobile, null );}
				case 1: {m_Mobile.DebugSay( "Firefield"); return new FireFieldSpell( m_Mobile, null );}
				case 2: {m_Mobile.DebugSay( "Mindb"); return new MindBlastSpell( m_Mobile, null );}
				case 3: {m_Mobile.DebugSay( "Par"); return new ParalyzeSpell( m_Mobile, null );}

				case 4: {m_Mobile.DebugSay( "EnergyB"); return new EnergyBoltSpell( m_Mobile, null );}
				case 5: {m_Mobile.DebugSay( "Explo"); return new ExplosionSpell( m_Mobile, null );}

				case 6: {m_Mobile.DebugSay( "Chain"); return new ChainLightningSpell( m_Mobile, null );}
				case 7: {m_Mobile.DebugSay( "Flame"); return new FlameStrikeSpell( m_Mobile, null );}

				case 8: {m_Mobile.DebugSay( "Flame"); return new FlameStrikeSpell( m_Mobile, null );}
				case 9: {m_Mobile.DebugSay( "Meteor"); return new MeteorSwarmSpell( m_Mobile, null );}
				case 10: {m_Mobile.DebugSay( "Lightning"); return new LightningSpell( m_Mobile, null );}
				case 11: {m_Mobile.DebugSay( "flame"); return new FlameStrikeSpell( m_Mobile, null );}

				case 12: {m_Mobile.DebugSay( "Mind"); return new MindBlastSpell( m_Mobile, null );}
				case 13: {m_Mobile.DebugSay( "PAral"); return new ParalyzeSpell( m_Mobile, null );}
				case 14: {m_Mobile.DebugSay( "Poisonf"); return new PoisonFieldSpell( m_Mobile, null );}

				case 15: {m_Mobile.DebugSay( "Energb"); return new EnergyBoltSpell( m_Mobile, null );}
				case 16: {m_Mobile.DebugSay( "Explo"); return new ExplosionSpell( m_Mobile, null );}
				case 17: {m_Mobile.DebugSay( "MassCurs"); return new MassCurseSpell( m_Mobile, null );}
				case 18: {m_Mobile.DebugSay( "Paral"); return new ParalyzeFieldSpell( m_Mobile, null );}

				case 19: {m_Mobile.DebugSay( "chainLightning"); return new ChainLightningSpell( m_Mobile, null );}
				case 20: {m_Mobile.DebugSay( "energf"); return new EnergyFieldSpell( m_Mobile, null );}
				case 21: {m_Mobile.DebugSay( "flames"); return new FlameStrikeSpell( m_Mobile, null );}
				case 22: {m_Mobile.DebugSay( "meteorsw"); return new MeteorSwarmSpell( m_Mobile, null );}

				case 23: {m_Mobile.DebugSay( "earthq"); return new EarthquakeSpell( m_Mobile, null );}

				default: {m_Mobile.DebugSay( "flame"); return new FlameStrikeSpell( m_Mobile, null );}
			}
		}

		public virtual Spell GetRandomDamageSpell()
		{
			int maxCircle = (int)((m_Mobile.Skills[SkillName.Magery].Value + 20.0) / (100.0 / 7.0));

			if ( maxCircle < 1 )
				maxCircle = 1;
			
			#region Mod by Magius(CHE): best efficency
			
			var castable = new List<Type>();
			
			if ( maxCircle >= 1)			
				for(var chance = 0; chance<2; chance++)
					castable.Add(typeof( MagicArrowSpell ));
			if ( maxCircle >= 2)			
				for(var chance = 0; chance<2; chance++)
					castable.Add(typeof( HarmSpell ));
			if ( maxCircle >= 3)			
				for(var chance = 0; chance<2; chance++)
					castable.Add(typeof( FireballSpell ));
			if ( maxCircle >= 4)			
				for(var chance = 0; chance<2; chance++)
					castable.Add(typeof( LightningSpell ));
			if ( maxCircle >= 5)			
				for(var chance = 0; chance<2; chance++)
					castable.Add(typeof( MindBlastSpell ));
			if ( maxCircle >= 6)
			{
				castable.Add(typeof( EnergyBoltSpell ));
				// castable.Add(typeof( ExplosionSpell )); // TODO fix a explosion, Dies
			}
			if ( maxCircle >= 7)
			{
				for(var chance = 0; chance<2; chance++)
					castable.Add(typeof( FlameStrikeSpell ));
				/* if multienemy checked */
				if (m_Mobile.Aggressors.Count>0)
				{
					castable.Add(typeof( MeteorSwarmSpell ));
					castable.Add(typeof( ChainLightningSpell ));
				}
			}
			
			if (castable.Count==0)
				return null;
			
			var spelltype = castable[Utility.Random(castable.Count)];
			
			return (Spell)Activator.CreateInstance(spelltype, m_Mobile, null);
			#endregion
            
            /*
            switch( Utility.Random( maxCircle * 4 ) )
            {
                case 0: return new ClumsySpell( m_Mobile, null );
                case 1: return new FeeblemindSpell( m_Mobile, null );
                case 2: return new MagicArrowSpell( m_Mobile, null );
                case 3: return new WeakenSpell( m_Mobile, null );

                case 4: return new CunningSpell( m_Mobile, null );
                case 5: return new HarmSpell( m_Mobile, null );

                case 6: return new FireballSpell( m_Mobile, null );
                case 7: return new PoisonSpell( m_Mobile, null );

                case 8: return new CurseSpell( m_Mobile, null );
                case 9: return new FireFieldSpell( m_Mobile, null );
                case 10: return new LightningSpell( m_Mobile, null );
                case 11: return new ManaDrainSpell( m_Mobile, null );

                case 12: return new MindBlastSpell( m_Mobile, null );
                case 13: return new ParalyzeSpell( m_Mobile, null );
                case 14: return new PoisonFieldSpell( m_Mobile, null );

                case 15: return new EnergyBoltSpell( m_Mobile, null );
                case 16: return new ExplosionSpell( m_Mobile, null );
                case 17: return new MassCurseSpell( m_Mobile, null );
                case 18: return new ParalyzeFieldSpell( m_Mobile, null );

                case 19: return new ChainLightningSpell( m_Mobile, null );
                case 20: return new EnergyFieldSpell( m_Mobile, null );
                case 21: return new ManaVampireSpell( m_Mobile, null );
                case 22: return new MeteorSwarmSpell( m_Mobile, null );

                case 23: return new EarthquakeSpell( m_Mobile, null );

                default: return new FlameStrikeSpell( m_Mobile, null );
            }
            */
		}

		public virtual Spell GetRandomCurseSpell()
		{
			if ( Utility.RandomBool() )
			{
				if ( m_Mobile.Skills[SkillName.Magery].Value >= 40.0 )
					return new CurseSpell( m_Mobile, null );
			}

			switch ( Utility.Random( 3 ) )
			{
				default:
				case 0: return new WeakenSpell( m_Mobile, null );
				case 1: return new ClumsySpell( m_Mobile, null );
				case 2: return new FeeblemindSpell( m_Mobile, null );
			}
		}

		public virtual Spell GetRandomManaDrainSpell()
		{
			if ( Utility.RandomBool() || m_Mobile.Skills[SkillName.Magery].Value >= 80.0 )
			{
				if ( m_Mobile.Skills[SkillName.Magery].Value >= 60.0 )
					return new ManaVampireSpell( m_Mobile, null );
			}

			return new ManaDrainSpell( m_Mobile, null );
		}

		public virtual Spell DoDispel( Mobile toDispel )
		{
			if ( !SmartAI )
			{
				if ( ScaleByMagery( DispelChance ) > Utility.RandomDouble() )
					return new DispelSpell( m_Mobile, null );

				return ChooseSpell( toDispel );
			}

			Spell spell = CheckCastHealingSpell();

			if ( spell == null )
			{
				if ( !m_Mobile.DisallowAllMoves && Utility.Random( (int)m_Mobile.GetDistanceToSqrt( toDispel ) ) == 0 )
					spell = new TeleportSpell( m_Mobile, null );
				else if ( Utility.Random( 3 ) == 0 && !m_Mobile.InRange( toDispel, 3 ) && !toDispel.Paralyzed && !toDispel.Frozen )
					spell = new ParalyzeSpell( m_Mobile, null );
				else
					spell = new DispelSpell( m_Mobile, null );
			}

			return spell;
		}

		public virtual Spell ChooseSpell( Mobile c )
		{
			Spell spell = null;

			if ( m_Mobile.Controlled && c.Hits > 50 )
			{
				spell = CheckCastHealingSpell();

				if ( spell != null )
					return spell;

				switch ( Utility.Random( 18 ) )
				{
					case 0:
					case 1:
					case 2:	// Poison them
					{
						m_Mobile.DebugSay( "Attempting to poison" );

						if ( !c.Poisoned )
							spell = new PoisonSpell( m_Mobile, null );

						break;
					}
					case 3:	// Bless ourselves.
					{
						m_Mobile.DebugSay( "Blessing myself" );

						spell = new BlessSpell( m_Mobile, null );
						break;
					}
					case 4:	// Paralyze them.
					{
						m_Mobile.DebugSay( "Attempting to paralyze" );

						if ( m_Mobile.Skills[SkillName.Magery].Value > 50.0 && !c.Paralyzed)
							spell = new ParalyzeSpell( m_Mobile, null );

						break;
					}
					case 5:
					case 6:
					case 7:
					case 8: // Drain mana
					{
						if ( m_Mobile.Mana < c.Mana || c.Mana > 50 )
						{
							m_Mobile.DebugSay( "Attempting to drain mana" );
							spell = GetRandomManaDrainSpell();
						}
						break;
					}
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					case 16: // Damage them.
					{
						m_Mobile.DebugSay( "Just doing damage!!!!!" );

						spell = GetRandomStrongDamageSpell();
						m_Mobile.DebugSay( "{0}", spell.Name );
						break;
					}
					#region Mod by Magius(CHE): summon minions
					default: // summon minions
					{
					    //m_Mobile.DebugSay( "Minions CAME!!!!" );

						//spell = GetRandomSummonSpell();
						break;
					}
					#endregion
				}
				return spell;
			}

			if ( !SmartAI )
			{
				spell = CheckCastHealingSpell();

				if ( spell != null )
					return spell;

				switch ( Utility.Random( 18 ) )
				{
					case 0:
					case 1:
					case 2:	// Poison them
					{
						m_Mobile.DebugSay( "Attempting to poison" );

						if ( !c.Poisoned )
							spell = new PoisonSpell( m_Mobile, null );

						break;
					}
					case 3:	// Bless ourselves.
					{
						m_Mobile.DebugSay( "Blessing myself" );

						spell = new BlessSpell( m_Mobile, null );
						break;
					}
					case 4:
					case 5:
					case 6: // Curse them.
					{
						m_Mobile.DebugSay( "Attempting to curse" );

						spell = GetRandomCurseSpell();
						break;
					}
					case 7:	// Paralyze them.
					{
						m_Mobile.DebugSay( "Attempting to paralyze" );

						if ( m_Mobile.Skills[SkillName.Magery].Value > 50.0 )
							spell = new ParalyzeSpell( m_Mobile, null );

						break;
					}
					case 8: // Drain mana
					{
						m_Mobile.DebugSay( "Attempting to drain mana" );

						spell = GetRandomManaDrainSpell();
						break;
					}
				    case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					case 16: // Damage them.
					{
						m_Mobile.DebugSay( "Just doing damage" );

						spell = GetRandomDamageSpell();
						break;
					}
					#region Mod by Magius(CHE): summon minions
					default: // summon minions
					    m_Mobile.DebugSay( "Minions CAME!!!!" );

						spell = GetRandomSummonSpell();
						break;
					#endregion
				}

				return spell;
			}

			spell = CheckCastHealingSpell();

			if ( spell != null )
				return spell;

			switch ( Utility.Random( 3 ) )
			{
				default:
				case 0: // Poison them
				{
					if ( !c.Poisoned )
						spell = new PoisonSpell( m_Mobile, null );

					break;
				}
				case 1: // Deal some damage
				{
					spell = GetRandomDamageSpell();

					break;
				}
				case 2: // Set up a combo
				{
					if ( m_Mobile.Mana < 40 && m_Mobile.Mana > 15 )
					{
						if ( c.Paralyzed && !c.Poisoned )
						{
							m_Mobile.DebugSay( "I am going to meditate" );

							m_Mobile.UseSkill( SkillName.Meditation );
						}
						else if ( !c.Poisoned )
						{
							spell = new ParalyzeSpell( m_Mobile, null );
						}
					}
					else if ( m_Mobile.Mana > 60 )
					{
						if ( Utility.Random( 2 ) == 0 && !c.Paralyzed && !c.Frozen && !c.Poisoned )
						{
							m_Combo = 0;
							spell = new ParalyzeSpell( m_Mobile, null );
						}
						else
						{
							m_Combo = 1;
							spell = new ExplosionSpell( m_Mobile, null );
						}
					}

					break;
				}
			}

			return spell;
		}

		protected int m_Combo = -1;

		public virtual Spell DoCombo( Mobile c )
		{
			Spell spell = null;

			if ( m_Combo == 0 )
			{
				spell = new ExplosionSpell( m_Mobile, null );
				++m_Combo; // Move to next spell
			}
			else if ( m_Combo == 1 )
			{
				spell = new WeakenSpell( m_Mobile, null );
				++m_Combo; // Move to next spell
			}
			else if ( m_Combo == 2 )
			{
				if ( !c.Poisoned )
					spell = new PoisonSpell( m_Mobile, null );

				++m_Combo; // Move to next spell
			}

			if ( m_Combo == 3 && spell == null )
			{
				switch ( Utility.Random( 3 ) )
				{
					default:
					case 0:
					{
						if ( c.Int < c.Dex )
							spell = new FeeblemindSpell( m_Mobile, null );
						else
							spell = new ClumsySpell( m_Mobile, null );

						++m_Combo; // Move to next spell

						break;
					}
					case 1:
					{
						spell = new EnergyBoltSpell( m_Mobile, null );
						m_Combo = -1; // Reset combo state
						break;
					}
					case 2:
					{
						spell = new FlameStrikeSpell( m_Mobile, null );
						m_Combo = -1; // Reset combo state
						break;
					}
				}
			}
			else if ( m_Combo == 4 && spell == null )
			{
				spell = new MindBlastSpell( m_Mobile, null );
				m_Combo = -1;
			}

			return spell;
		}

		private TimeSpan GetDelay( Spell sp )
		{
			MagerySpell spell = sp as MagerySpell;

			int circle = (int)spell.Circle;
			//double del = ScaleByMagery( 3.0 );
			double min = circle;//6.0 - (del * 0.75);
			double max = circle+2.0;//6.0 - (del * 1.25);

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

				Spell spell = null;
				Mobile toDispel = FindDispelTarget( true );

				if ( m_Mobile.Poisoned ) // Top cast priority is cure
				{
					m_Mobile.DebugSay( "I am going to cure myself" );

					spell = new CureSpell( m_Mobile, null );
				}
				else if ( toDispel != null ) // Something dispellable is attacking us
				{
					m_Mobile.DebugSay( "I am going to dispel {0}", toDispel );

					spell = DoDispel( toDispel );
				}
                else if( /* SmartAI && */m_Combo != -1 ) // We are doing a spell combo
				{
					spell = DoCombo( c );
				}
                else if( /* SmartAI && */ ( c.Spell is HealSpell || c.Spell is GreaterHealSpell ) && !c.Poisoned ) // They have a heal spell out
				{
					spell = new PoisonSpell( m_Mobile, null );
				}
				else
				{
					spell = ChooseSpell( c );
				}

				// Now we have a spell picked
				// Move first before casting

				if ( SmartAI && toDispel != null )
				{
					if ( m_Mobile.InRange( toDispel, 10 ) )
						RunFrom( toDispel );
					else if ( !m_Mobile.InRange( toDispel, Core.ML ? 10 : 12 ) )
						RunTo( toDispel );
				}
				else
				{
					RunTo( c );
				}

				if ( spell != null )
					spell.Cast();

				TimeSpan delay = TimeSpan.FromSeconds( 4 );;

				if ( SmartAI || ( spell is DispelSpell ) )
					delay = TimeSpan.FromSeconds( m_Mobile.ActiveSpeed );
				else if (spell is MagerySpell)
					delay = GetDelay( spell );

				m_NextCastTime = DateTime.Now + delay;
			}
			else if ( m_Mobile.Spell == null || !m_Mobile.Spell.IsCasting )
			{
				RunTo( c );
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
			{
				if ( !m_Mobile.Controlled )
				{
					ProcessTarget();

					Spell spell = CheckCastHealingSpell();

					if ( spell != null )
						spell.Cast();
				}

				base.DoActionGuard();
			}

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

				if ( m_Mobile.Poisoned && Utility.Random( 0, 5 ) == 0 )
					new CureSpell( m_Mobile, null ).Cast();
			}
			else
			{
				m_Mobile.DebugSay( "Area seems clear, but my guard is up" );

				Action = ActionType.Guard;
				m_Mobile.Warmode = true;
			}

			return true;
		}

		public Mobile FindDispelTarget( bool activeOnly )
		{
			if ( m_Mobile.Deleted || m_Mobile.Int < 95 || CanDispel( m_Mobile ) || m_Mobile.AutoDispel )
				return null;

			if ( activeOnly )
			{
				List<AggressorInfo> aggressed = m_Mobile.Aggressed;
				List<AggressorInfo> aggressors = m_Mobile.Aggressors;

				Mobile active = null;
				double activePrio = 0.0;

				Mobile comb = m_Mobile.Combatant;

				if ( comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && m_Mobile.InRange( comb, Core.ML ? 10 : 12 ) && CanDispel( comb ) )
				{
					active = comb;
					activePrio = m_Mobile.GetDistanceToSqrt( comb );

					if ( activePrio <= 2 )
						return active;
				}

				for ( int i = 0; i < aggressed.Count; ++i )
				{
					AggressorInfo info = aggressed[i];
					Mobile m = info.Defender;

					if ( m != comb && m.Combatant == m_Mobile && m_Mobile.InRange( m, Core.ML ? 10 : 12 ) && CanDispel( m ) )
					{
						double prio = m_Mobile.GetDistanceToSqrt( m );

						if ( active == null || prio < activePrio )
						{
							active = m;
							activePrio = prio;

							if ( activePrio <= 2 )
								return active;
						}
					}
				}

				for ( int i = 0; i < aggressors.Count; ++i )
				{
					AggressorInfo info = aggressors[i];
					Mobile m = info.Attacker;

					if ( m != comb && m.Combatant == m_Mobile && m_Mobile.InRange( m, Core.ML ? 10 : 12 ) && CanDispel( m ) )
					{
						double prio = m_Mobile.GetDistanceToSqrt( m );

						if ( active == null || prio < activePrio )
						{
							active = m;
							activePrio = prio;

							if ( activePrio <= 2 )
								return active;
						}
					}
				}

				return active;
			}
			else
			{
				Map map = m_Mobile.Map;

				if ( map != null )
				{
					Mobile active = null, inactive = null;
					double actPrio = 0.0, inactPrio = 0.0;

					Mobile comb = m_Mobile.Combatant;

					if ( comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && CanDispel( comb ) )
					{
						active = inactive = comb;
						actPrio = inactPrio = m_Mobile.GetDistanceToSqrt( comb );
					}

					foreach ( Mobile m in m_Mobile.GetMobilesInRange( Core.ML ? 10 : 12 ) )
					{
						if ( m != m_Mobile && CanDispel( m ) )
						{
							double prio = m_Mobile.GetDistanceToSqrt( m );

							if ( !activeOnly && (inactive == null || prio < inactPrio) )
							{
								inactive = m;
								inactPrio = prio;
							}

							if ( (m_Mobile.Combatant == m || m.Combatant == m_Mobile) && (active == null || prio < actPrio) )
							{
								active = m;
								actPrio = prio;
							}
						}
					}

					return active != null ? active : inactive;
				}
			}

			return null;
		}

		public bool CanDispel( Mobile m )
		{
			return ( m is BaseCreature && ((BaseCreature)m).Summoned && m_Mobile.CanBeHarmful( m, false ) && !((BaseCreature)m).IsAnimatedDead );
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

			bool isDispel = ( targ is DispelSpell.InternalTarget );
			bool isParalyze = ( targ is ParalyzeSpell.InternalTarget );
			bool isTeleport = ( targ is TeleportSpell.InternalTarget );
			bool isAreaDamage = ( targ is ExplosionSpell.OldInternalTarget ) || ( targ is PoisonFieldSpell.InternalTarget ) || ( targ is FireFieldSpell.InternalTarget ) || ( targ is EnergyFieldSpell.InternalTarget ) || ( targ is ChainLightningSpell.InternalTarget ) || ( targ is MeteorSwarmSpell.InternalTarget );
			
			bool isSummoning = ( targ is EnergyVortexSpell.InternalTarget ); // Mod by Magius(CHE) : Minions
			
			bool teleportAway = false;

			Mobile toTarget;

			if ( isDispel )
			{
				toTarget = FindDispelTarget( false );

				if ( !SmartAI && toTarget != null )
					RunTo( toTarget );
				else if ( toTarget != null && m_Mobile.InRange( toTarget, 10 ) )
					RunFrom( toTarget );
			}
			else if ( SmartAI && (isParalyze || isTeleport) )
			{
				toTarget = FindDispelTarget( true );

				if ( toTarget == null )
				{
					toTarget = m_Mobile.Combatant;

					if ( toTarget != null )
						RunTo( toTarget );
				}
				else if ( m_Mobile.InRange( toTarget, 10 ) )
				{
					RunFrom( toTarget );
					teleportAway = true;
				}
				else
				{
					teleportAway = true;
				}
			}
			#region Mod by Magius(CHE) : Minions
			else if (isSummoning)
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
			#endregion
			#region Mod by Magius(CHE): area spells
			else if (isAreaDamage)
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

				if ( toTarget != null )
					RunTo( toTarget );
			}

			if ( (targ.Flags & TargetFlags.Harmful) != 0 && toTarget != null )
			{
				if ( (targ.Range == -1 || m_Mobile.InRange( toTarget, targ.Range )) && m_Mobile.CanSee( toTarget ) && m_Mobile.InLOS( toTarget ) )
				{
					targ.Invoke( m_Mobile, toTarget );
				}
				else if ( isDispel )
				{
					targ.Cancel( m_Mobile, TargetCancelType.Canceled );
				}
			}
			else if ( (targ.Flags & TargetFlags.Beneficial) != 0 )
			{
				targ.Invoke( m_Mobile, m_Mobile );
			}
			else if ( isTeleport && toTarget != null )
			{
				Map map = m_Mobile.Map;

				if ( map == null )
				{
					targ.Cancel( m_Mobile, TargetCancelType.Canceled );
					return true;
				}

				int px, py;

				if ( teleportAway )
				{
					int rx = m_Mobile.X - toTarget.X;
					int ry = m_Mobile.Y - toTarget.Y;

					double d = m_Mobile.GetDistanceToSqrt( toTarget );

					px = toTarget.X + (int)(rx * (10 / d));
					py = toTarget.Y + (int)(ry * (10 / d));
				}
				else
				{
					px = toTarget.X;
					py = toTarget.Y;
				}

				for ( int i = 0; i < m_Offsets.Length; i += 2 )
				{
					int x = m_Offsets[i], y = m_Offsets[i + 1];

					Point3D p = new Point3D( px + x, py + y, 0 );

					LandTarget lt = new LandTarget( p, map );

					if ( (targ.Range == -1 || m_Mobile.InRange( p, targ.Range )) && m_Mobile.InLOS( lt ) && map.CanSpawnMobile( px + x, py + y, lt.Z ) && !SpellHelper.CheckMulti( p, map ) )
					{
						targ.Invoke( m_Mobile, lt );
						return true;
					}
				}

				int teleRange = targ.Range;

				if ( teleRange < 0 )
					teleRange = Core.ML ? 11 : 12;

				for ( int i = 0; i < 10; ++i )
				{
					Point3D randomPoint = new Point3D( m_Mobile.X - teleRange + Utility.Random( teleRange * 2 + 1 ), m_Mobile.Y - teleRange + Utility.Random( teleRange * 2 + 1 ), 0 );

					LandTarget lt = new LandTarget( randomPoint, map );

					if ( m_Mobile.InLOS( lt ) && map.CanSpawnMobile( lt.X, lt.Y, lt.Z ) && !SpellHelper.CheckMulti( randomPoint, map ) )
					{
						targ.Invoke( m_Mobile, new LandTarget( randomPoint, map ) );
						return true;
					}
				}

				targ.Cancel( m_Mobile, TargetCancelType.Canceled );
			}
			else
			{
				targ.Cancel( m_Mobile, TargetCancelType.Canceled );
			}

			return true;
		}
	}
}
