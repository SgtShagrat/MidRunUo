/***************************************************************************
 *							   DryadAllure.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Spells;
using Server.Targeting;
using Server.Mobiles;

namespace Midgard.Engines.SpellSystem
{
	public class DryadAllureSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
		"Dryad Allure", "Lore Tia Kes",
			224,
			9011,
			Reagent.PetrifiedWood,
			Reagent.Kindling
		);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( DryadAllureSpell ),
				"Charms a target humanoid into doing the caster's bidding.",
				"Usando il fascino delle Driadi, il druido comanda i suoi avversari secondo la sua volontà.",
				0x59e3
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override SpellCircle Circle{get { return SpellCircle.Seventh; }}

		public DryadAllureSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( BaseCreature bc )
		{
			if( !Caster.CanSee( bc.Location ) || !Caster.InLOS( bc ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( !IsValidTarget( bc ) )
			{
				Caster.SendLocalizedMessage( 1074379 ); // You cannot charm that!
			}
			else if( Caster.Followers + 3 > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049607 ); // You have too many followers to control that creature.
			}
			else if( bc.Allured )
			{
				Caster.SendLocalizedMessage( 1074380 ); // This humanoid is already controlled by someone else.				
			}
			else if( CheckSequence() )
			{
				double skill = Caster.Skills[ CastSkill ].Value;
				double taming = Caster.Skills.AnimalTaming.Value;

				skill += taming > 90.0 ? ( taming - 90.0 ) : 0.0;

				double chance = skill / 200;

				if( chance > Utility.RandomDouble() )
				{
					bc.ControlSlots = 3;
					bc.Combatant = null;

					if( Caster.Combatant == bc )
					{
						Caster.Combatant = null;
						Caster.Warmode = false;
					}

					if( bc.SetControlMaster( Caster ) )
					{
						bc.PlaySound( 0x5C4 );
						bc.Allured = true;

						Container pack = bc.Backpack;

						if( pack != null )
						{
							for( int i = pack.Items.Count - 1; i >= 0; --i )
							{
								if( i >= pack.Items.Count )
									continue;

								pack.Items[ i ].Delete();
							}
						}

						if( bc is ArcticOgreLord )
							Scale( bc, 30 );

						Caster.SendLocalizedMessage( 1074377 ); // You allure the humanoid to follow and protect you.
						bc.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );

						TimeSpan duration = TimeSpan.FromSeconds( Math.Min( 60.0, skill ) );

						Timer.DelayCall( duration, new TimerStateCallback( FreeTheCreatureCallback ), bc );
					}
				}
				else
				{
					bc.PlaySound( 0x5C5 );
					bc.ControlTarget = Caster;
					bc.ControlOrder = OrderType.Attack;
					bc.Combatant = Caster;

					Caster.SendLocalizedMessage( 1074378 ); // The humanoid becomes enraged by your charming attempt and attacks you.
				}
			}

			FinishSequence();
		}

		private static void FreeTheCreatureCallback( object state )
		{
			BaseCreature bc = state as BaseCreature;

			if( bc != null )
			{
				Mobile master = bc.GetMaster();

				bc.SetControlMaster( null );

				if( master != null && Utility.Random( 10 ) == 1 )
				{
					bc.PlaySound( 0x5C5 );
					bc.ControlTarget = master;
					bc.ControlOrder = OrderType.Attack;
					bc.Combatant = master;

					master.SendMessage( (master.Language == "ITA"? "L'umanoide sembra arrabbiarsi, ti attacca!" : "The humanoid becomes enraged by your charming and attacks you.") );
				}
			}
			else
				Console.WriteLine( "Warning: null creature during dryad allure." );
		}

		private static void Scale( BaseCreature bc, int scalar )
		{
			int toScale = bc.RawStr;
			bc.RawStr = AOS.Scale( toScale, scalar );

			toScale = bc.HitsMaxSeed;

			if( toScale > 0 )
				bc.HitsMaxSeed = AOS.Scale( toScale, scalar );

			bc.Hits = bc.Hits; // refresh hits
		}

		public static bool IsValidTarget( BaseCreature bc )
		{
			if( bc == null || bc.IsParagon || ( bc.Controlled && !bc.Allured ) || bc.Summoned )
				return false;

			SlayerEntry slayer = SlayerGroup.GetEntryByName( SlayerName.Repond );

			if( slayer != null && slayer.Slays( bc ) )
				return true;

			return false;
		}

		public class InternalTarget : Target
		{
			private readonly DryadAllureSpell m_Owner;

			public InternalTarget( DryadAllureSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				if( o is BaseCreature )
				{
					m_Owner.Target( (BaseCreature)o );
				}
				else
				{
					m.SendLocalizedMessage( 1074379 ); // You cannot charm that!
				}
			}

			protected override void OnTargetFinish( Mobile m )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}