using System;
using System.Collections.Generic;

using Midgard.Engines.Races;
using Midgard.Items;
using Midgard.Menus;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

using Midgard.Engines.PlagueBeastLordPuzzle;

namespace Server.Items
{
	public class Bandage : Item, IDyable
	{
		public static int Range = ( Core.AOS ? 2 : 1 ); 

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public Bandage() : this( 1 )
		{
		}

		[Constructable]
		public Bandage( int amount ) : base( 0xE21 )
		{
			Stackable = true;
			Amount = amount;
		}

		public Bandage( Serial serial ) : base( serial )
		{
		}

		public virtual bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( GetWorldLocation(), Range ) )
			{
				#region mod by Dies Irae
				if( !Morph.CheckItemAllowed( this, from, true ) )
					return;
				#endregion

				from.RevealingAction( true );

				from.SendLocalizedMessage( 500948 ); // Who will you use the bandages on?

				from.Target = new InternalTarget( this );
			}
			else
			{
				from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
			}
		}

		#region mod by Dies Irae
		public override void Consume()
		{
			if( RootParent is Mobile )
				( (Mobile)RootParent ).AddToBackpack( new BloodyBandage() );
			else
				new BloodyBandage().MoveToWorld( Location, Map );

			base.Consume();
		}
		#endregion

		private class InternalTarget : Target
		{
			private Bandage m_Bandage;

			public InternalTarget( Bandage bandage ) : base( Bandage.Range, false, TargetFlags.Beneficial )
			{
				m_Bandage = bandage;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Bandage.Deleted )
					return;

				#region Modifica by Dies Irae
				if ( targeted is BloodWound )
				{
					BloodWound blood = targeted as BloodWound;
					if( !blood.IsAided )
					{
						blood.DoAid( from );
						m_Bandage.Consume();
					}
				}
				#endregion

				if ( targeted is Mobile )
				{
					if ( from.InRange( m_Bandage.GetWorldLocation(), Bandage.Range ) )
					{
						if ( BandageContext.BeginHeal( from, (Mobile)targeted, m_Bandage is EnhancedBandage ) != null )
						{
							m_Bandage.Consume();
						}
					}
					else
					{
						from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
					}
				}
				/*
				else if ( targeted is PlagueBeastInnard )
				{
					if ( ((PlagueBeastInnard) targeted).OnBandage( from ) )
						m_Bandage.Consume();
				}
				*/
				else
				{
					from.SendLocalizedMessage( 500970 ); // Bandages can not be used on that.
				}
			}

			#region Modifica by Dies Irae
			protected override void OnNonlocalTarget( Mobile from, object targeted )
			{
				/*
				if ( targeted is PlagueBeastInnard )
				{
					if ( ((PlagueBeastInnard) targeted).OnBandage( from ) )
						m_Bandage.Consume();
				}
				else
					base.OnNonlocalTarget( from, targeted );
				*/

				if( m_Bandage.Deleted )
					return;
				if( !(targeted is Item && ((Item)targeted).Parent is PlagueBackpack) )
					from.SendLocalizedMessage( 500447 ); // That is not accessible.
				else if( targeted is BloodWound )
				{
					from.RevealingAction( true );
					
					BloodWound blood = targeted as BloodWound;
					if( !blood.IsAided )
					{
						blood.DoAid( from );
						m_Bandage.Consume();
					}
				}
			}
			#endregion
		}
	}

	public class BandageContext
	{
		private Mobile m_Healer;
		private Mobile m_Patient;
		private int m_Slips;
		private Timer m_Timer;

		public Mobile Healer{ get{ return m_Healer; } }
		public Mobile Patient{ get{ return m_Patient; } }
		public int Slips{ get{ return m_Slips; } set{ m_Slips = value; } }
		public Timer Timer{ get{ return m_Timer; } }

		#region Heritage Items
		private bool m_Enhanced;

		public bool Enhanced{ get{ return m_Enhanced; } }
		#endregion

		public void Slip()
		{
			m_Healer.SendLocalizedMessage( 500961 ); // Your fingers slip!
			++m_Slips;
		}

	    public bool HealerDamaged { get; set; } // mod by Dies Irae

		public BandageContext( Mobile healer, Mobile patient, TimeSpan delay, bool enhanced )
		{
			m_Healer = healer;
			m_Patient = patient;

			m_Timer = new InternalTimer( this, delay );
			m_Timer.Start();

			m_Enhanced = enhanced;
		}

		public BandageContext( Mobile healer, Mobile patient )
		{
			m_Healer = healer;
			m_Patient = patient;

			m_Timer = new OldInternalTimer( this );
			m_Timer.Start();
		}

		public void StopHeal()
		{
			m_Table.Remove( m_Healer );

			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;
		}

		private static Dictionary<Mobile, BandageContext> m_Table = new Dictionary<Mobile, BandageContext>();

		public static BandageContext GetContext( Mobile healer )
		{
			BandageContext bc = null;
			m_Table.TryGetValue( healer, out bc );
			return bc;
		}

		public static SkillName GetPrimarySkill( Mobile m )
		{
			if ( !m.Player && (m.Body.IsMonster || m.Body.IsAnimal) )
				return SkillName.Veterinary;
			else
				return SkillName.Healing;
		}

		public static SkillName GetSecondarySkill( Mobile m )
		{
			if ( !m.Player && (m.Body.IsMonster || m.Body.IsAnimal) )
				return SkillName.AnimalLore;
			else
				return SkillName.Anatomy;
		}

		public void EndHeal()
		{
			StopHeal();

			int healerNumber = -1, patientNumber = -1;
			bool playSound = true;
			bool checkSkills = false;

			SkillName primarySkill = GetPrimarySkill( m_Patient );
			SkillName secondarySkill = GetSecondarySkill( m_Patient );

			BaseCreature petPatient = m_Patient as BaseCreature;

			if ( !m_Healer.Alive )
			{
				healerNumber = 500962; // You were unable to finish your work before you died.
				patientNumber = -1;
				playSound = false;
			}
			else if ( !m_Healer.InRange( m_Patient, IsAnimal( m_Patient ) ? Bandage.Range + 2 : Bandage.Range ) )
			{
				healerNumber = 500963; // You did not stay close enough to heal your target.
				patientNumber = -1;
				playSound = false;
			}
			else if ( !m_Patient.Alive || (petPatient != null && petPatient.IsDeadPet) )
			{
				double healing = m_Healer.Skills[primarySkill].Value;
				double anatomy = m_Healer.Skills[secondarySkill].Value;
				double chance = ((healing - 68.0) / 50.0) - (m_Slips * 0.02);
				checkSkills = (healing >= 80.0 && anatomy >= 80.0);

				#region mod by Dies Irae
				if( Core.T2A )
				{
					// A minimum level of 80 for both Anatomy and Healing will give a chance of 25% to resurrect a ghost.
					if( chance < 0.25 )
						chance = 0.25;
				}
				#endregion

				if ( checkSkills && ( chance > Utility.RandomDouble() ) 
					  || ( Core.SE && petPatient is Factions.FactionWarHorse && petPatient.ControlMaster == m_Healer) )	//TODO: Dbl check doesn't check for faction of the horse here?
				{
					if ( m_Patient.Map == null || !m_Patient.Map.CanFit( m_Patient.Location, 16, false, false ) )
					{
						healerNumber = 501042; // Target can not be resurrected at that location.
						patientNumber = 502391; // Thou can not be resurrected there!
					}
					else if ( m_Patient.Region != null && m_Patient.Region.IsPartOf( "Khaldun" ) )
					{
						healerNumber = 1010395; // The veil of death in this area is too strong and resists thy efforts to restore life.
						patientNumber = -1;
					}
					else
					{
						CheckSkills( m_Healer, m_Patient, primarySkill, secondarySkill, 0.0 );
						healerNumber = 500965; // You are able to resurrect your patient.
						patientNumber = -1;

						m_Patient.PlaySound( 0x214 );
						m_Patient.FixedEffect( 0x376A, 10, 16 );

						if ( petPatient != null && petPatient.IsDeadPet )
						{

							Mobile master = petPatient.ControlMaster;

							if( master != null && m_Healer == master )
							{
								petPatient.ResurrectPet();

								for ( int i = 0; i < petPatient.Skills.Length; ++i )
								{
									petPatient.Skills[i].Base -= 0.1;
								}
							}
							else if ( master != null && master.InRange( petPatient, 3 ) )
							{
								healerNumber = 503255; // You are able to resurrect the creature.

								if( Core.AOS )
								{
									master.CloseGump( typeof( PetResurrectGump ) );
									master.SendGump( new PetResurrectGump( m_Healer, petPatient ) );
								}
								else
								{
									if( !master.HasMenu( typeof( PetResurrectionMenu ) ) )
										master.SendMenu( new PetResurrectionMenu( petPatient ) );
								}
							}
							else
							{
								bool found = false;

								List<Mobile> friends = petPatient.Friends;

								for ( int i = 0; friends != null && i < friends.Count; ++i )
								{
									Mobile friend = friends[i];

									if ( friend.InRange( petPatient, 3 ) )
									{
										healerNumber = 503255; // You are able to resurrect the creature.

										if( Core.AOS )
										{
											friend.CloseGump( typeof( PetResurrectGump ) );
											friend.SendGump( new PetResurrectGump( m_Healer, petPatient ) );
										}
										else
										{
											if( !friend.HasMenu( typeof( PetResurrectionMenu ) ) )
												friend.SendMenu( new PetResurrectionMenu( petPatient ) );
										}

										found = true;
										break;
									}
								}

								if ( !found )
									healerNumber = 1049670; // The pet's owner must be nearby to attempt resurrection.
							}
						}
						else
						{
							m_Patient.CloseGump( typeof( ResurrectGump ) );
							m_Patient.SendGump( new ResurrectGump( m_Patient, m_Healer ) );
						}
					}
				}
				else
				{
					if ( checkSkills )
						CheckSkills( m_Healer, m_Patient, primarySkill, secondarySkill, 0.5 );

					if ( petPatient != null && petPatient.IsDeadPet )
						healerNumber = 503256; // You fail to resurrect the creature.
					else
						healerNumber = 500966; // You are unable to resurrect your patient.

					patientNumber = -1;
				}
			}
			else if ( m_Patient.Poisoned )
			{
				m_Healer.SendLocalizedMessage( 500969 ); // You finish applying the bandages.

				double healing = m_Healer.Skills[primarySkill].Value;
				double anatomy = m_Healer.Skills[secondarySkill].Value;
				checkSkills = (healing >= 60.0 && anatomy >= 60.0);

				#region Mondain's Legacy mod
				double chance = ((healing - 30.0) / 50.0) - (m_Patient.Poison.RealLevel * 0.1) - (m_Slips * 0.02);
				#endregion

				#region mod by Dies Irae
				if( Core.T2A )
				{
					// A minimum level of 60 for both Anatomy and Healing will give a chance of 80% to cure someone who is poisoned.
					if( chance < 0.8 )
						chance = 0.8;
				}
				#endregion

				if ( checkSkills && chance > Utility.RandomDouble() )
				{
					if ( m_Patient.CurePoison( m_Healer ) )
					{
						CheckSkills( m_Healer, m_Patient, primarySkill, secondarySkill, 0.5 );
						healerNumber = (m_Healer == m_Patient) ? -1 : 1010058; // You have cured the target of all poisons.
						patientNumber = 1010059; // You have been cured of all poisons.
					}
					else
					{
						healerNumber = -1;
						patientNumber = -1;
					}
				}
				else
				{
					if (checkSkills)
						CheckSkills( m_Healer, m_Patient, primarySkill, secondarySkill, 0.75 );

					healerNumber = 1010060; // You have failed to cure your target!
					patientNumber = -1;
				}
			}
			else if ( BleedAttack.IsBleeding( m_Patient ) )
			{
				healerNumber = 1060088; // You bind the wound and stop the bleeding
				patientNumber = 1060167; // The bleeding wounds have healed, you are no longer bleeding!

				BleedAttack.EndBleed( m_Patient, false );
			}
			else if ( MortalStrike.IsWounded( m_Patient ) )
			{
				healerNumber = ( m_Healer == m_Patient ? 1005000 : 1010398 );
				patientNumber = -1;
				playSound = false;
			}
			else if ( m_Patient.Hits == m_Patient.HitsMax )
			{
				healerNumber = 500967; // You heal what little damage your patient had.
				patientNumber = -1;
			}
			else
			{
				checkSkills = true;
				patientNumber = -1;

				double healing = m_Healer.Skills[primarySkill].Value;
				double anatomy = m_Healer.Skills[secondarySkill].Value;
				double chance = ((healing /* + 10.0 */ ) / 100.0) - (m_Slips * 0.02);

				#region Heritage Items
				healing += EnhancedBandage.HealingBonus;
				#endregion
				
				if ( chance > Utility.RandomDouble() )
				{
					healerNumber = 500969; // You finish applying the bandages.

					double min, max;

					if ( Core.AOS )
					{
						min = (anatomy / 8.0) + (healing / 5.0) + 4.0;
						max = (anatomy / 6.0) + (healing / 2.5) + 4.0;
					}
					else
					{
						min = (anatomy / 5.0) + (healing / 5.0) + 3.0;
						max = (anatomy / 5.0) + (healing / 2.0) + 10.0;//80.0
					}
					int difficulty = m_Patient.HitsMax - m_Patient.Hits;
					double toHeal = min + (Utility.RandomDouble() * (max - min));

					if( toHeal > difficulty )
						toHeal = difficulty;

					if ( m_Patient.Body.IsMonster || m_Patient.Body.IsAnimal )
						toHeal += m_Patient.HitsMax / 100;

					if( Core.T2A )
					{
						if( Core.AOS )
							toHeal -= toHeal * m_Slips * 0.35; // TODO: Verify algorithm
						else
							toHeal -= m_Slips * 4;
					}

					if ( toHeal < 1 )
					{
						toHeal = 1;
						healerNumber = 500968; // You apply the bandages, but they barely help.
					}

					#region mod by Dies Irae
					//double primaryChance = ( 50.0 + ( ( healing - ( m_Patient.HitsMax - m_Patient.Hits ) ) * 2 ) ) / 100;
					//double secondaryChance = ( 50.0 + ( ( anatomy - ( m_Patient.HitsMax - m_Patient.Hits ) ) * 2 ) ) / 100;

					//m_Healer.CheckSkill( secondarySkill, Math.Max( secondaryChance, 0.1 ) );
					//m_Healer.CheckSkill( primarySkill, Math.Max( primaryChance, 0.1 ) );

					double percent = ( toHeal < 80 ? (80 - toHeal)*0.01 : 0.01 );
					CheckSkills( m_Healer, m_Patient, primarySkill, secondarySkill, percent );

					checkSkills = false;
					#endregion

					m_Patient.Heal( (int) toHeal, m_Healer, false );
				}
				else
				{
					//CheckSkills( SkillName.Healing, SkillName.Anatomy, 0.9 );
					healerNumber = 500968; // You apply the bandages, but they barely help.
					playSound = false;
				}
			}

			if ( healerNumber != -1 )
				m_Healer.SendLocalizedMessage( healerNumber );

			if ( patientNumber != -1 )
				m_Patient.SendLocalizedMessage( patientNumber );

			if ( playSound )
				m_Patient.PlaySound( 0x57 );

			//if ( checkSkills )
			//{
			//	if( !m_Patient.Alive && m_Patient.Player )
			//	{
			//		CheckSkills( m_Healer, m_Patient, primarySkill, secondarySkill, 0.01 );
					//m_Healer.CheckSkill( secondarySkill, 0.50 );
					//m_Healer.CheckSkill( primarySkill, 0.50 );
			//	}
				//else
				//{
					//m_Healer.CheckSkill( secondarySkill, 0.0, Core.AOS ? 120.0 : 100.0 ); // mod by Dies Irae
					//m_Healer.CheckSkill( primarySkill, 0.0, Core.AOS ? 120.0 : 100.0 ); // mod by Dies Irae
				//}
			//}
		}

		private class InternalTimer : Timer
		{
			private BandageContext m_Context;

			public InternalTimer( BandageContext context, TimeSpan delay ) : base( delay )
			{
				m_Context = context;
				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				m_Context.EndHeal();
			}
		}

		#region mod by Dies Irae : [Pre-AOS]
		private class OldInternalTimer : Timer
		{
			private readonly BandageContext m_Context;
			private readonly Mobile m_Healer;
			private readonly Mobile m_Patient;
			private readonly bool m_OnSelf;
			private bool m_PoisonFirst;
			private readonly Point3D m_HealerStartLocation;
			private readonly Point3D m_PatientStartLocation;

			private int m_Counter;
			private bool m_HealerOrPatientMoved;

			public OldInternalTimer( BandageContext context )
				: base( TimeSpan.FromSeconds( 0.1 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Context = context;
				m_Healer = m_Context.Healer;
				m_Patient = m_Context.Patient;
				m_OnSelf = m_Healer == m_Patient;
				m_PoisonFirst = m_Patient.Poisoned;
				m_HealerStartLocation = m_Healer.Location;
				m_PatientStartLocation = m_Patient.Location;
				m_HealerOrPatientMoved = false;

				PrintMessages( 1 );
			}

			private void PrintMessages( int step )//1 start 2 continue 3 stop
			{
				string type = (m_PoisonFirst ? "cure" : "heal");

				if( m_OnSelf )
				{
					if ( m_Healer.Language == "ITA" )
						m_Healer.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( step == 1 ? "*inizi a curarti*" : (step == 2 ? "*continui a curarti*" : "*finisci di curarti*" ) ) );
					else
						m_Healer.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( step == 1 ? "*you start to {0} yourself*" : (step == 2 ? "*you continue to {0} yourself*" : "*you stop to {0} yourself*" ), type ) );
				}
				else
				{
					string fem =m_Patient.Female ? "a" : "o";
					if ( m_Healer.Language == "ITA" )
						m_Healer.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( step == 1 ? "*inizi a curarl" + fem + "*" : (step == 2 ? "*continui a curarl" + fem +"*" : "*finisci di curarl" + fem +"*" ) ) );
					else
						m_Healer.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( step == 1 ? "*you start to {0} him*" : (step == 2 ? "*you continue to {0} him*" : "*you stop to {0} him*" ), type ) );
					
					if ( m_Patient.Language == "ITA" )
						m_Patient.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( step == 1 ? "*{0} inizia a curarti*" : (step == 2 ? "*{0} continua a curarti*" : "*{0} finisce di curarti*" ), m_Healer.Name ) );
					else
						m_Patient.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( step == 1 ? "*{0} starts to {1} you*" : (step == 2 ? "*{0} continues to {1} you*" : "*{0} stops to {1} you*" ), m_Healer.Name, type ) );
				}
			}

			private bool CheckContinue()
			{
				if( /*m_Healer.Frozen ||*/ (!m_Patient.Poisoned && m_Patient.Hits >= m_Patient.HitsMax) || !m_Healer.InLOS( m_Patient ) )
					return false;
				else if( !m_Healer.Alive )
				{
					m_Healer.SendMessage( (m_Healer.Language == "ITA" ? "Ti è stato impossibile finire il lavoro prima di morire." : "You were unable to finish your work before you died.") );
					return false;
				}
				else if( !m_Healer.InRange( m_Patient, IsAnimal( m_Patient ) ? Bandage.Range + 2 : Bandage.Range ) )
				{
					m_Healer.SendMessage( (m_Healer.Language == "ITA" ? "Non sei rimasto abbastanza vicino al target di cura." : "You did not stay close enough to heal your target.")  );
					return false;
				}

				return true;
			}

			private bool HealerOrPatientMoved()
			{
				return ( m_Healer.Location != m_HealerStartLocation ) || ( m_Patient.Location != m_PatientStartLocation );
			}

		    private bool HealerDamaged
		    {
                get { return m_Context != null && m_Context.HealerDamaged; }
		    }

			protected override void OnTick()
			{
				m_Counter++;

				m_Healer.RevealingAction( true );
				m_Patient.RevealingAction( true );

				if( !CheckContinue() )
				{
					PrintMessages( 3 );
					m_Context.StopHeal();
					return;
				}

				if( HealerOrPatientMoved() )
					m_HealerOrPatientMoved = true;

				if( m_PoisonFirst )
				{
					if( m_OnSelf )
					{
						if( m_Counter == 3 )
						{
							if( m_Context.DoCure() )
							{
								m_Counter = 0;
								m_PoisonFirst = false;
							}
							else
								m_Context.StopHeal();
						}
					}
					else
					{
						if( m_Counter == 1 )
						{
							if( m_Context.DoCure() )
							{
								m_Counter = 0;
								m_PoisonFirst = false;
							}
							else
								m_Context.StopHeal();
						}
					}
				}
				else
				{
					if( m_OnSelf )
					{
						if( m_Counter == 10 )
						{
							PrintMessages( 3 );
                            m_Context.DoHeal( m_HealerOrPatientMoved, HealerDamaged );

							m_Context.StopHeal();
						}
					}
					else
					{
                        if( m_Counter == 6 )
                        {

                            PrintMessages( 3 );
                            m_Context.DoHeal( m_HealerOrPatientMoved, HealerDamaged );

                            m_Context.StopHeal();
                        }
					}
				}

				if( ( m_Counter % 3 ) == 0 && m_Counter != 0 )
				{
					PrintMessages( 2 );
					m_Patient.PlaySound( 0x57 );
				}

				if( m_Counter > 18 )
					m_Context.StopHeal();
			}
		}

		private static bool IsAnimal( Mobile m )
		{
			return !m.Player && ( m.Body.IsMonster || m.Body.IsAnimal );
		}

		private bool DoCure()
		{
			double healing = m_Healer.Skills[ SkillName.Healing ].Value;
			double anatomy = m_Healer.Skills[ SkillName.Anatomy ].Value;

			if( healing >= 60.0 && anatomy >= 60.0 )
			{
				int chance = (int)( 80.0 + ( ( healing + anatomy - 120.0 ) / 4.0 ) );

				if( Utility.Random( 100 ) < chance )
				{
					if( m_Patient.CurePoison( m_Healer ) )
					{
						CheckSkills( m_Healer, m_Patient, SkillName.Healing, SkillName.Anatomy, 0.5 );

						if( m_Patient != m_Healer )
							m_Healer.SendMessage( (m_Healer.Language == "ITA" ? "Hai curato il bersagio totalmente." : "You have cured the target of all poisons.") );
						m_Patient.SendMessage( (m_Healer.Language == "ITA" ? "Hai eliminato tutti i veleni dal tuo corpo."  : "You have been cured of all poisons." ) );
						return true;
					}
				}
				else
				{
					CheckSkills( m_Healer, m_Patient, SkillName.Healing, SkillName.Anatomy, 0.75 );
					m_Healer.SendMessage( (m_Healer.Language == "ITA" ? "Hai applicato le bende, con scarso successo." : "You apply the bandages, but they barely help.") );
				}
			}
			else
				m_Healer.SendMessage( (m_Healer.Language == "ITA" ? "Non sei riuscito a curarti." : "You could not cure.") );

			return false;
		}

		private void DoHeal( bool moved, bool damaged )
		{
			double healing = m_Healer.Skills[ SkillName.Healing ].Value;
			double anatomy = m_Healer.Skills[ SkillName.Anatomy ].Value;

			double min = ( anatomy / 5.0 ) + ( healing / 5.0 ) + 3.0;
			double max = ( anatomy / 5.0 ) + ( healing / 2.0 ) + 10.0;
			double toHeal = min + ( Utility.RandomDouble() * ( max - min ) );

			int difficulty = m_Patient.HitsMax - m_Patient.Hits;

			int chance = (int)( 50 + ( ( healing - ( difficulty ) ) * 2 ) );

			if( m_Patient.Poisoned )
			{
				toHeal = toHeal - ( 5 * m_Patient.Poison.RealLevel );
				if( toHeal < 0 )
					return;
			}

			if( moved || damaged )
				toHeal = toHeal * 0.5;
			else
			{
				if ( toHeal > 60 )
					toHeal = 60;
			}

			if( toHeal > difficulty )
				toHeal = difficulty;

			if( m_Patient != null && ( m_Patient.Body.IsMonster || m_Patient.Body.IsAnimal ) )
				toHeal += m_Patient.HitsMax / 100.0;

			double percent = ( toHeal < max ? (max - toHeal)*0.01 : 0.01 );

			if( Utility.RandomDouble() < healing / 100.0 )
			{
				CheckSkills( m_Healer, m_Patient, SkillName.Healing, SkillName.Anatomy, percent );
				if( m_Patient != null )
					m_Patient.Heal( (int)toHeal, m_Healer, false );

				if( m_Patient != m_Healer && m_Healer != null )
					m_Healer.SendMessage( string.Format( (m_Healer.Language == "ITA" ? "Hai curato {0} punti ferita." : "You healed {0} hit points." ) , (int)toHeal ) );

				if( m_Patient != null )
					m_Patient.SendMessage( (m_Healer.Language == "ITA" ? ( m_Patient == m_Healer ? "Hai curato le tue ferite." : "Sei stato curato.") : "You have been healed.") );
			}
			else
			{
				CheckSkills( m_Healer, m_Patient, SkillName.Healing, SkillName.Anatomy, (percent + 1)/2 );

				m_Healer.SendMessage( (m_Healer.Language == "ITA" ? "Hai applicato le bende, con scarso successo." : "You apply the bandages, but they barely help.") );
			}
		}

        /*
		private static void CheckSkills( Mobile healer, Mobile patient, SkillName primary, SkillName secondary )
		{
			double healing = healer.Skills[ primary ].Value;
			double anatomy = healer.Skills[ secondary ].Value;

			double primaryChance = ( 50.0 + ( ( healing - ( patient.HitsMax - patient.Hits ) ) * 2 ) ) / 100;
			double secondaryChance = ( 50.0 + ( ( anatomy - ( patient.HitsMax - patient.Hits ) ) * 2 ) ) / 100;

			healer.CheckSkill( primary, Math.Max( secondaryChance * 0.5, 0.1 ) );
			healer.CheckSkill( secondary, Math.Max( primaryChance, 0.1 ) );
		}
        */

		private static void CheckSkills( Mobile healer, Mobile patient, SkillName primary, SkillName secondary, double chance )
		{
			double healing = healer.Skills[ primary ].Value;
			double anatomy = healer.Skills[ secondary ].Value;

			//double primaryChance = ( 50.0 + ( ( healing - ( patient.HitsMax - patient.Hits ) ) * 2 ) ) / 100;
			double secondaryChance = ( 50.0 + ( ( anatomy - ( patient.HitsMax - patient.Hits ) ) * 2 ) ) / 100;
			//healer.SendMessage( chance.ToString());
			healer.CheckSkill( primary, chance );
			healer.CheckSkill( secondary, Math.Max( secondaryChance, 0.1 ) );
		}
		#endregion

		public static BandageContext BeginHeal( Mobile healer, Mobile patient )
		{
			return BeginHeal( healer, patient, false );
		}

		public static BandageContext BeginHeal( Mobile healer, Mobile patient, bool enhanced )
		{
			bool isDeadPet = ( patient is BaseCreature && ((BaseCreature)patient).IsDeadPet );

			if ( patient is Golem )
			{
				healer.SendLocalizedMessage( 500970 ); // Bandages cannot be used on that.
			}
			else if ( patient is BaseCreature && ((BaseCreature)patient).IsAnimatedDead )
			{
				healer.SendLocalizedMessage( 500951 ); // You cannot heal that.
			}
			else if ( !patient.Poisoned && patient.Hits == patient.HitsMax && !BleedAttack.IsBleeding( patient ) && !isDeadPet )
			{
				healer.SendLocalizedMessage( 500955 ); // That being is not damaged!
			}
			else if ( !patient.Alive && (patient.Map == null || !patient.Map.CanFit( patient.Location, 16, false, false )) )
			{
				healer.SendLocalizedMessage( 501042 ); // Target cannot be resurrected at that location.
			}
			else if ( healer.CanBeBeneficial( patient, true, true ) )
			{
				healer.DoBeneficial( patient );

				bool onSelf = ( healer == patient );
				int dex = healer.Dex;

				double seconds;
				double resDelay = ( patient.Alive ? 0.0 : 5.0 );

				if ( onSelf )
				{
					if ( Core.AOS )
						seconds = 5.0 + (0.5 * ((double)(120 - dex) / 10)); // TODO: Verify algorithm
					else
						seconds = 9.4 + ( 0.6 * ( (double)(/* 120 */ 100 - dex ) / 10 ) ); // mod by Dies Irae

					#region mod by Dies Irae
					if( Core.T2A )
						seconds = 15.0; // Healing yourself has a 15 second delay.

					//if( patient.Poisoned )
					//	seconds += 3.0; // Curing yourself has an 18 second delay.
					#endregion
				}
				else
				{
					if ( Core.AOS && GetPrimarySkill( patient ) == SkillName.Veterinary )
					{
							seconds = 2.0;
					}
					else if ( Core.AOS )
					{
						if (dex < 204)
						{		
							seconds = 3.2-(Math.Sin((double)dex/130)*2.5) + resDelay;
						}
						else
						{
							seconds = 0.7 + resDelay;
						}
					}
					else
					{
						if ( dex >= 100 )
							seconds = 3.0 + resDelay;
						else if ( dex >= 40 )
							seconds = 4.0 + resDelay;
						else
							seconds = 5.0 + resDelay;

						#region mod by Dies Irae
						if( Core.T2A )
							seconds = 5.0 + resDelay; // Healing others has a 5 second delay.

						//if( patient.Poisoned )
						//	seconds += 1.0; // Curing others has a 6 second delay
						#endregion
					}
				}

				BandageContext context = GetContext( healer );

				if ( context != null )
					context.StopHeal();
				seconds *= 1000;
				
				if( patient is BaseCreature || ( resDelay > 0.0 ))
					context = new BandageContext( healer, patient, TimeSpan.FromMilliseconds( seconds ), enhanced );
				else
					context = new BandageContext( healer, patient );//OldIternalTarget

				m_Table[healer] = context;

				if ( !onSelf )
					patient.SendLocalizedMessage( 1008078, false, healer.Name ); //  : Attempting to heal you.
				
				healer.SendLocalizedMessage( 500956 ); // You begin applying the bandages.
				return context;
			}

			return null;
		}
	}
}