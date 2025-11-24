using System;

using Midgard.Engines.Classes;
using Midgard.Items;

using Server.Targeting;
using Server.Items;
using Server.Network;

namespace Server.SkillHandlers
{
	public class Poisoning
	{
		public static void Initialize()
		{
			SkillInfo.Table[ (int)SkillName.Poisoning ].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.Target = new InternalTargetPoison();

			m.SendLocalizedMessage( 502137 ); // Select the poison you wish to use

			return TimeSpan.FromSeconds( 10.0 ); // 10 second delay before beign able to re-use a skill
		}

		#region mod by Dies Irae
		private static readonly Type[] m_PoisonableWeaponsTable = new Type[]
		{
			typeof(ShortSpear),
			typeof(Kryss),
			typeof(Pitchfork),
			typeof(WarFork),
			typeof(Dagger),
			typeof(BackstabbingKnife)
		};

		public static bool IsPoisonableWeapon( Type type )
		{
			bool contains = false;

			for( int i = 0; !contains && i < m_PoisonableWeaponsTable.Length; ++i )
				contains = ( m_PoisonableWeaponsTable[ i ] == type );

			return contains;
		}
		#endregion

		private class InternalTargetPoison : Target
		{
			public InternalTargetPoison()
				: base( 2, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if( targeted is BasePoisonPotion )
				{
					if( ( (BasePoisonPotion)targeted ).MinPoisoningSkill > from.Skills[ SkillName.Poisoning ].Value )
					{
						from.SendMessage( (from.Language == "ITA" ? "Non sai come usare questa pozione.": "Thou don't know how to use that potion.") );
						return;
					}

					from.SendLocalizedMessage( 502142 ); // To what do you wish to apply the poison?
					from.Target = new InternalTarget( (BasePoisonPotion)targeted );
				}
				else // Not a Poison Potion
				{
					from.SendLocalizedMessage( 502139 ); // That is not a poison potion.
				}
			}

			private class InternalTarget : Target
			{
				private BasePoisonPotion m_Potion;

				public InternalTarget( BasePoisonPotion potion )
					: base( 2, false, TargetFlags.None )
				{
					m_Potion = potion;
				}

				protected override void OnTarget( Mobile from, object targeted )
				{
					if( m_Potion.Deleted )
						return;

					bool startTimer = false;

					if( targeted is Food || targeted is FukiyaDarts || targeted is Shuriken )
					{
						startTimer = true;
					}
					else if( targeted is BaseWeapon )
					{
						BaseWeapon weapon = (BaseWeapon)targeted;

						if( Core.AOS )
						{
							startTimer = ( weapon.PrimaryAbility == WeaponAbility.InfectiousStrike || weapon.SecondaryAbility == WeaponAbility.InfectiousStrike );
						}
						//else if ( weapon.Layer == Layer.OneHanded )
						//{
						// Only Bladed or Piercing weapon can be poisoned
						startTimer = IsPoisonableWeapon( weapon.GetType() ); // ( weapon.Type == WeaponType.Slashing || weapon.Type == WeaponType.Piercing );
						//}
					}
					#region mod by Dies Irae
					else if( targeted is Arrow )
					{
						if ( !ClassSystem.IsScout( from ) && m_Potion.Poison.Level >= 19 && m_Potion.Poison.Level <= 43 )
						{
							from.SendMessage( (from.Language == "ITA" ? "Pensi che questo veleno non durerebbe a lungo su di una freccia.": "Thou think that this poison wouldn't last long on that arrow.") );
							return;
						}

						startTimer = true;
					}
					else if( targeted is Bolt )
					{
						if ( !ClassSystem.IsScout( from ) && m_Potion.Poison.Level >= 19 && m_Potion.Poison.Level <= 43 )
						{
							from.SendMessage( (from.Language == "ITA" ? "Pensi che questo veleno non durerebbe a lungo su di un dardo.": "Thou think that this poison wouldn't last long on that bolt.") );
							return;
						}

						startTimer = true;
					}
					#endregion

					if( startTimer )
					{
						new InternalTimer( from, (Item)targeted, m_Potion ).Start();

						from.PlaySound( 0x4F );

						m_Potion.Consume();
						from.AddToBackpack( new Bottle() );
					}
					else // Target can't be poisoned
					{
						if( Core.AOS )
							from.SendLocalizedMessage( 1060204 ); // You cannot poison that! You can only poison infectious weapons, food or drink.
						else
							from.SendLocalizedMessage( 502145 ); // You cannot poison that! You can only poison bladed or piercing weapons, food or drink.
					}
				}

				private class InternalTimer : Timer
				{
					private Mobile m_From;
					private Item m_Target;
					private Poison m_Poison;
					private double m_MinSkill, m_MaxSkill;

					public InternalTimer( Mobile from, Item target, BasePoisonPotion potion )
						: base( TimeSpan.FromSeconds( 2.0 ) )
					{
						m_From = from;
						m_Target = target;
						m_Poison = potion.Poison;
						m_MinSkill = potion.MinPoisoningSkill;
						m_MaxSkill = potion.MaxPoisoningSkill;
						Priority = TimerPriority.TwoFiftyMS;
					}

					/// <summary>
					/// Possibilità di auto poisonarsi durante il prrocesso
					/// </summary>
					private const double Chance2AutoPoison = 0.005;

					private const double Chance2AutoPoisonForFood = 0.018;

					private const double Chance2AutoPoisonForWeapons = 0.03;

					private const double Chance2AutoPoisonForAmmo = 0.05;

					private const double Chance2AutoPoisonIfFailedCritical = 1.00;

					private const double Chance2AutoPoisonIfFailedNotCritical = 0.10;

					private const double Chance2AutoPoisonIfFailedOnWeapon = 0.12;

					private const int ArrowsForOnePotion = 8;

					protected override void OnTick()
					{
						double chance2AutoPoison = Chance2AutoPoison; // mod by magius(che)

						double skvalue = m_From.Skills[SkillName.Poisoning].Value;
						bool success = Utility.Random( 101 ) < skvalue;

						double rawPercent = (m_MinSkill > 0 ? (1 - (m_MinSkill/100)) : 0.90);

						if( skvalue < m_MaxSkill )
							m_From.CheckSkill( SkillName.Poisoning, (success? rawPercent : (rawPercent+1)/2) );

						//bool success = m_From.CheckTargetSkill( SkillName.Poisoning, m_Target, m_MinSkill - 30, m_MaxSkill - 30 );

						if( success /* Utility.Random( 100 ) < m_From.Skills[ SkillName.Poisoning ].Value */ )
						{
							// m_From.CheckTargetSkill( SkillName.Poisoning, m_Target, m_MinSkill, m_MaxSkill );

							if( m_Target is Food )
							{
								( (Food)m_Target ).Poison = m_Poison;
								chance2AutoPoison = Chance2AutoPoisonForFood; // mod by magius(che)	
							}
							else if( m_Target is BaseWeapon )
							{
								( (BaseWeapon)m_Target ).Poison = m_Poison;

								#region Mondain's Legacy mod
								( (BaseWeapon)m_Target ).PoisonCharges = 18 - ( m_Poison.RealLevel * 2 );
								#endregion

								( (BaseWeapon)m_Target ).PoisonerSkill = m_From.Skills[ SkillName.Poisoning ].Value; // mod by Dies Irae

								chance2AutoPoison = Chance2AutoPoisonForWeapons; // mod by magius(che)
							}
							#region mod by Dies Irae
							else if( m_Target is Arrow )
							{
								Arrow arrow = m_Target as Arrow;
								int toConsume = Math.Min( ArrowsForOnePotion, arrow.Amount );
								arrow.Consume( toConsume );
								m_From.AddToBackpack( BasePoisonedArrow.GetArrowByPoison( m_Poison, toConsume, m_From.Skills[ SkillName.Poisoning ].Value ) );
							}
							else if( m_Target is Bolt )
							{
								Bolt bolt = m_Target as Bolt;
								int toConsume = Math.Min( ArrowsForOnePotion, bolt.Amount );
								bolt.Consume( toConsume );
								m_From.AddToBackpack( BasePoisonedBolt.GetBoltByPoison( m_Poison, toConsume, m_From.Skills[ SkillName.Poisoning ].Value ) );
							}
							#endregion
							else if( m_Target is FukiyaDarts )
							{
								( (FukiyaDarts)m_Target ).Poison = m_Poison;

								#region Mondain's Legacy mod
								( (FukiyaDarts)m_Target ).PoisonCharges = Math.Min( 18 - ( m_Poison.RealLevel * 2 ), ( (FukiyaDarts)m_Target ).UsesRemaining );
								#endregion

								chance2AutoPoison = Chance2AutoPoisonForAmmo; // mod by magius(che)	
							}
							else if( m_Target is Shuriken )
							{
								( (Shuriken)m_Target ).Poison = m_Poison;

								#region Mondain's Legacy mod
								( (Shuriken)m_Target ).PoisonCharges = Math.Min( 18 - ( m_Poison.RealLevel * 2 ), ( (Shuriken)m_Target ).UsesRemaining );
								#endregion

								chance2AutoPoison = Chance2AutoPoisonForAmmo; // mod by magius(che)				
							}

							// m_From.SendLocalizedMessage( 1010517 ); // You apply the poison
							m_From.SendMessage( (m_From.Language == "ITA" ? "Hai applicato con successo una dose di veleno {0}." : "You apply a dose of {0} poison succesfully."), (m_From.Language == "ITA" ? m_Poison.NameIt.ToLower() : m_Poison.Name.ToLower()) );
							
							Misc.Titles.AwardKarma( m_From, -20 - (int)( m_MinSkill / 2 ), true ); // mod by Dies Irae
						}
						else // Failed
						{
							// 5% of chance of getting poisoned if failed
							if( m_From.Skills[ SkillName.Poisoning ].Base < 80.0 && Utility.Random( 20 ) == 0 )
							{
								#region mod by magius(che): autopoison *** moved down ***
								//m_From.SendLocalizedMessage( 502148 ); // You make a grave mistake while applying the poison.
								//m_From.ApplyPoison( m_From, m_Poison );
								chance2AutoPoison = Chance2AutoPoisonIfFailedCritical; //secure autopoisoning
								#endregion
							}
							else
							{
								if( m_Target is BaseWeapon )
								{
									BaseWeapon weapon = (BaseWeapon)m_Target;

									if( weapon.Type == WeaponType.Slashing )
										m_From.SendLocalizedMessage( 1010516 ); // You fail to apply a sufficient dose of poison on the blade
									else
										m_From.SendLocalizedMessage( 1010518 ); // You fail to apply a sufficient dose of poison

									chance2AutoPoison = Chance2AutoPoisonIfFailedOnWeapon; // mod by magius(che)
								}
								else
								{
									m_From.SendLocalizedMessage( 1010518 ); // You fail to apply a sufficient dose of poison
																
									chance2AutoPoison = Chance2AutoPoisonIfFailedNotCritical; // mod by magius(che)	
								}
							}
						}
						
						#region mod by magius(che): autopoison
						if ( Utility.RandomDouble() < chance2AutoPoison )
						{
							m_From.ApplyPoison( m_From, m_Poison );
							m_From.SendLocalizedMessage( 502148 );  // You make a grave mistake while applying the poison.
						}
						#endregion
					}
				}
			}
		}
	}
}