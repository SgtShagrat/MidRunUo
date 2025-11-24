/***************************************************************************
 *								  DefilerKissSpell.cs
 *									-------------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Questo potente maleficio trasferisce al necromante forza vitale 
 * 			prendendola dalle creature e dai cadaveri attorno al necromante
 * 			stesso.
 * 			
 * 			Il raggio d'azione è SpiritSpeak /15 tiles.
 * 			Da ogni cadavere nel raggio d'azione del maleficio il necromante 
 * 			assorbe 1d4 + 1 punti ferita.
 * 			Da ogni creatura viva nel raggio il necromante assorbe 1d10 + 5
 * 			punti ferita.
 * 	
 * 			Se la creatura è un Paladino egli subità un effetto doppio dal
 * 			drenaggio.
 * 
 * 			Il maleficio è usabile ogni 30 secondi.
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Targeting;

using Midgard.Gumps;

namespace Midgard.Engines.SpellSystem
{
	public class DefilerKissSpell : RPGNecromancerSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
			"Defiler Kiss", "In Vas Bal Mani",
			-1,
			9002,
			true,
			Reagent.GraveDust,
			Reagent.NoxCrystal
			);

		private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( DefilerKissSpell ),
			"This curse drains out life force from the corpses and creatures surrounding the necromancer.",
			"Questo potente maleficio trasferisce al necromante forza vitale prendendola dalle creature e dai cadaveri attorno al necromante stesso.",
			0x5004
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override int RequiredMana{ get { return 5; }}
		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 1.5 ); }}
		public override double DelayOfReuse{get { return 50.0; }}
		public override double RequiredSkill{get { return 55.0; }}
		public override bool BlocksMovement{get { return true; }}

		//private Timer m_Timer;
		private static readonly int ToHealCap = 30;
		private static readonly int PaladinMultiplierPercent = 150;
		private static int range = 5;

		private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

		public DefilerKissSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if (UnderEffect( Caster ))
				StopRegen();
			else
				Caster.Target = new InternalTarget( this );
		}

		public static bool UnderEffect( Mobile m )
		{
			return m != null && m_Table.ContainsKey( m );
		}

		public void Target( Mobile m )
		{
			if( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( SpellHelper.CheckTown( m, Caster ) && CheckSequence() )
			{
				//int level = GetFocusLevel( Caster ) + (int)(GetPowerLevel()/2);
				//double skill = Caster.Skills[ CastSkill ].Value;

				//int tiles = 2 + level;
				//int damage = 10 + ( level / 2 );
				//int durationInSeconds = (int)Math.Max( 1, skill / 24 ) + ( level * 5 );

                        	//Timer.DelayCall( TimeSpan.FromSeconds( 0.75 ), TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( LeechRadiusOnTick ), new object[] { Caster, 3 } );
				//Timer.DelayCall( GetDelayOfReuse(), new TimerStateCallback( ReleaseDefilerKissLock ), Caster );
				//if( m_Table != null && m_Table.ContainsKey( Caster ) )
				//{
				//	Timer t = m_Table[ Caster ];

				//	if( t != null )
				//		t.Stop();
				//}

				range = 5 + GetPowerLevel();
				StartRegen();
			}

			FinishSequence();
		}

		/*
		public override void OnCast()
		{
			if( Caster.CanBeginAction( typeof( DefilerKissSpell ) ) )
			{
				if( CheckSequence() )
				{
					Caster.BeginAction( typeof( DefilerKissSpell ) );

					int range = 2 + ( ( GetPowerLevel() * 20 ) / 15 );

					List<Corpse> corpseList = new List<Corpse>();
					foreach( Item item in Caster.GetItemsInRange( range ) )
					{
						if( item is Corpse && !( (Corpse)item ).Channeled )
							corpseList.Add( (Corpse)item );
					}

					List<Mobile> mobileList = new List<Mobile>();
					foreach( Mobile mobile in Caster.GetMobilesInRange( range ) )
					{
						if( mobile == Caster || !Caster.CanBeHarmful( mobile ) || !Caster.InLOS( mobile ) )
							continue;

						if( mobile is BaseCreature )
						{
							BaseCreature bc = (BaseCreature)mobile;

							Mobile master = bc.GetMaster();
							if( master != null && master == Caster )
								continue;

							if( bc.Summoned )
								continue;
						}

						mobileList.Add( mobile );
					}

					int toHeal = 0;

					foreach( Corpse corpse in corpseList )
					{
						toHeal += Utility.Dice( 1, 10, 1 );
						corpse.Channeled = true;
						corpse.Hue = 1157;
						corpse.PublicOverheadMessage( MessageType.Regular, 37, true, Caster.Language == "ITA" ? "*il corpo si decompone*" : "*the corpse get decomposed*" );
					}

					foreach( Mobile mobile in mobileList )
					{
						if( mobile == null || mobile.Deleted )
							continue;

						int damage = Utility.Dice( 1, 10, 5 );

						// Paladins suffer more damage from this spell
						if( IsSuperVulnerable( mobile ) )
							damage = AOS.Scale( damage, PaladinMultiplierPercent );

						Caster.DoHarmful( mobile );

						mobile.FixedParticles( 0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist );
						mobile.PlaySound( 0x231 );

						mobile.SendMessage( "You feel the life drain out of you!" );
						mobile.PublicOverheadMessage( MessageType.Regular, 37, true, "* the evil powers will prevail! *" );

						mobile.Damage( damage, Caster );

						toHeal += damage;
					}

					if( toHeal > ToHealCap )
						toHeal = ToHealCap;

					Caster.Hits += toHeal;
					Caster.SendMessage( "Your necromantic life has been restored!" );

					Timer.DelayCall( GetDelayOfReuse(), new TimerStateCallback( ReleaseDefilerKissLock ), Caster );
				}
				FinishSequence();
			}
			else
				Caster.SendMessage( "You are not ready to drain life." );
		}
		*/

		public void StartRegen( )
		{
			Caster.SendMessage( Caster.Language == "ITA" ? "La rigenerazione attraverso i cadaveri è attiva." : "You start to leech nearby corpses." );
			Caster.SendGump(new DefilerKissGump());

			if( m_Table != null && m_Table.ContainsKey( Caster ) )
			{
				Timer t = m_Table[ Caster ];

				if( t != null )
					t.Stop();
			}

			m_Table[ Caster ] = new InternalTimer( this );
		}

		public void StopRegen()
		{
			Caster.SendMessage( Caster.Language == "ITA" ? "La rigenerazione è finita." : "Your regen ended." );
			if (Caster.HasGump(typeof(DefilerKissGump)))
				Caster.CloseGump(typeof(DefilerKissGump));

			if( m_Table != null && m_Table.ContainsKey( Caster ) )
			{
				Timer t = m_Table[ Caster ];

				if( t != null )
					t.Stop();
			}

			m_Table.Remove( Caster );
			FinishSequence();
		}

		private static void ReleaseDefilerKissLock( object state )
		{
			((Mobile)state).EndAction( typeof( DefilerKissSpell ) );
			((Mobile)state).SendMessage( "Your mortal kiss ended." );
		}

		public class InternalTarget : Target
		{
			private readonly DefilerKissSpell m_Owner;

			public InternalTarget( DefilerKissSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				if( o is Mobile )
					m_Owner.Target( (Mobile)o );
				else if( m_Owner.Caster != null )
					m_Owner.Caster.SendMessage( m_Owner.Caster.Language == "ITA" ? "Devi selezionare un essere vivente." : "Thou must target a valid living target." );
			}

			protected override void OnTargetFinish( Mobile m )
			{
				m_Owner.FinishSequence();
			}
		}

		private class InternalTimer : Timer
		{
			private readonly DefilerKissSpell m_Owner;
			private readonly Mobile m_Caster;
			private int m_Counter = 0;
			private int m_Level = 1;

			public InternalTimer( DefilerKissSpell owner ) : base( TimeSpan.FromSeconds( 0.1 ), TimeSpan.FromSeconds( 0.1 ) )
			{
				m_Owner = owner;
				m_Caster = m_Owner.Caster;
				m_Level = m_Owner.GetPowerLevel();
				this.Start();
			}

			private bool CheckContinue()
			{
				if( m_Caster.Hits >= m_Caster.HitsMax || !m_Caster.Alive )
					return false;

				return true;
			}

			protected override void OnTick()
			{
				m_Counter++;//6000 = 10 minuti
				if ( 6 - m_Level == 1 || (m_Counter % (6 - m_Level) ) == 0 )
				{ 
					m_Caster.RevealingAction( false );
					//m_Caster.SendMessage (m_Counter.ToString());
					if( m_Level == 0 || m_Counter == 6000*m_Level )
					{
						m_Owner.StopRegen();
						return;
					}

					Map map = m_Caster.Map;
					if( map == null || map == Map.Internal )
						return;

					//1 mob/corpo per livello
					int objAdded = 0;

					List<Corpse> corpseList = new List<Corpse>();
					foreach( Item item in m_Caster.GetItemsInRange( range ) )
					{
						if( objAdded < m_Level && item is Corpse && !((Corpse)item).Channeled )
						{
							corpseList.Add( (Corpse)item );
							if ( m_Caster.Hits < m_Caster.HitsMax )
								objAdded++;
						}
					}

					List<Mobile> mobileList = new List<Mobile>();
					foreach( Mobile mobile in m_Caster.GetMobilesInRange( range ) )
					{
						if( objAdded >= m_Level || mobile == m_Caster || !m_Caster.InLOS( mobile ) ) //!m_Caster.CanBeHarmful( mobile )
							continue;

						if( mobile is BaseNecroFamiliar )
						{
							BaseCreature bc = (BaseCreature)mobile;

							Mobile master = bc.GetMaster();
							if( master != null && master == m_Caster && bc.Summoned && bc.Hits > 0 )
							{
								mobileList.Add( mobile );
								if ( m_Caster.Hits < m_Caster.HitsMax )
									objAdded++;
							}
						}
					}

					Point3D startLoc;
					Point3D endLoc = new Point3D( m_Caster.X, m_Caster.Y, m_Caster.Z + 5 );
					if ( m_Caster.Alive )//&& m_Caster.Hits < m_Caster.HitsMax )
					{
						//prima il caster leecha hp dai cadaveri
						foreach( Corpse corpse in corpseList )
						{
							if (corpse.LeechableHp > 0 && m_Caster.Hits < m_Caster.HitsMax )
							{
								corpse.LeechableHp--;
								m_Caster.Hits++;

								startLoc = new Point3D( corpse.X, corpse.Y, corpse.Z + 1 );
								endLoc = new Point3D( m_Caster.X, m_Caster.Y, m_Caster.Z + (m_Caster.Mounted ? 10 : 5) );
								Effects.SendMovingEffect( new Entity( Serial.Zero, startLoc, map ), new Entity( Serial.Zero, endLoc, map ),
								0x3893, 1, 0, false, false, 1960, 0 );//0x36E4
							}
							//se il caster è a piena vita i suoi minions leechano hp dai cadaveri
							else if ( corpse.LeechableHp > 0 )//&& m_Caster.Hits == m_Caster.HitsMax )
							{
								foreach( Mobile mobile in mobileList )
								{
									if ( mobile.Hits < mobile.HitsMax )
									{
										corpse.LeechableHp--;
										mobile.Hits++;
										if (((BaseCreature)mobile).Experience > 0 )
											mobile.Mana += ((BaseCreature)mobile).Experience;

										startLoc = new Point3D( corpse.X, corpse.Y, corpse.Z + 1 );
										endLoc = new Point3D( mobile.X, mobile.Y, mobile.Z + 5 );
										Effects.SendMovingEffect( new Entity( Serial.Zero, startLoc, map ), new Entity( Serial.Zero, endLoc, map ),
										0x3893, 1, 0, false, false, 1960, 0 );//0x36E4 from, to, itemid, speed, fixeddirection, explodes
									}
									//if ( mobile.Mana < mobile.ManaMax && ((BaseCreature)mobile).Experience > 0 )
									//	mobile.Mana++;
								}
							}
							else if ( corpse.LeechableHp <= 0 && !corpse.Channeled )
							{
								corpse.Channeled = true;
								corpse.Hue = 1157;
								corpse.PublicOverheadMessage( MessageType.Regular, 37, true, m_Caster.Language == "ITA" ? "*il cadavere si decompone*" : "*the corpse get decomposed*" );
							}
						}
						//se non ci sono cadaveri il caster leecha dai suoi minions
						foreach( Mobile mobile in mobileList )
						{
							if (mobile.Hits > 0 && m_Caster.Hits < m_Caster.HitsMax )
							{
								mobile.Hits--;
								m_Caster.Hits++;

								startLoc = new Point3D( mobile.X, mobile.Y, mobile.Z + 5 );
								endLoc = new Point3D( m_Caster.X, m_Caster.Y, m_Caster.Z + (m_Caster.Mounted ? 10 : 5) );
								Effects.SendMovingEffect( new Entity( Serial.Zero, startLoc, map ), new Entity( Serial.Zero, endLoc, map ),
								0x3893, 1, 0, false, false, 1960, 0 );//from, to, itemid, speed, fixeddirection, explodes
							}
						}
					}
					//else
					//	m_Caster.SendMessage( "HP 100%" );
				}
			}
		}
	}
}