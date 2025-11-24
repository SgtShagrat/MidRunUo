using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fifth
{
	public class MagicReflectSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Magic Reflection", "In Jux Sanct",
				242,
				9012,
				Reagent.Garlic,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public MagicReflectSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( Core.AOS )
				return true;

			#region mod by Dies Irae
			if( Targetable )
				return true;
			#endregion

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				return false;
			}
			else if ( !Caster.CanBeginAction( typeof( DefensiveSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
				return false;
			}

			return true;
		}

		private static Hashtable m_Table = new Hashtable();

		public override void OnCast()
		{
			#region mod by Dies Irae
			if( Targetable )
			{
				Caster.Target = new InternalTarget( this );
				return;
			}
			#endregion

			if ( Core.AOS )
			{
				/* The magic reflection spell decreases the caster's physical resistance, while increasing the caster's elemental resistances.
				 * Physical decrease = 25 - (Inscription/20).
				 * Elemental resistance = +10 (-20 physical, +10 elemental at GM Inscription)
				 * The magic reflection spell has an indefinite duration, becoming active when cast, and deactivated when re-cast.
				 * Reactive Armor, Protection, and Magic Reflection will stay on—even after logging out, even after dying—until you “turn them off” by casting them again. 
				 */

				if ( CheckSequence() )
				{
					Mobile targ = Caster;

					ResistanceMod[] mods = (ResistanceMod[])m_Table[targ];

					if ( mods == null )
					{
						targ.PlaySound( 0x1E9 );
						targ.FixedParticles( 0x375A, 10, 15, 5037, EffectLayer.Waist );

						int physiMod = -25 + (int)(targ.Skills[SkillName.Inscribe].Value / 20);
						int otherMod = 10;

						mods = new ResistanceMod[5]
							{
								new ResistanceMod( ResistanceType.Physical, physiMod ),
								new ResistanceMod( ResistanceType.Fire,		otherMod ),
								new ResistanceMod( ResistanceType.Cold,		otherMod ),
								new ResistanceMod( ResistanceType.Poison,	otherMod ),
								new ResistanceMod( ResistanceType.Energy,	otherMod )
							};

						m_Table[targ] = mods;

						for ( int i = 0; i < mods.Length; ++i )
							targ.AddResistanceMod( mods[i] );

						string buffFormat = String.Format( "{0}\t+{1}\t+{1}\t+{1}\t+{1}", physiMod, otherMod );

						BuffInfo.AddBuff( targ, new BuffInfo( BuffIcon.MagicReflection, 1075817, buffFormat, true ) );
					}
					else
					{
						targ.PlaySound( 0x1ED );
						targ.FixedParticles( 0x375A, 10, 15, 5037, EffectLayer.Waist );

						m_Table.Remove( targ );

						for ( int i = 0; i < mods.Length; ++i )
							targ.RemoveResistanceMod( mods[i] );

						BuffInfo.RemoveBuff( targ, BuffIcon.MagicReflection );
					}
				}

				FinishSequence();
			}
			else
			{
				if ( Caster.MagicDamageAbsorb > 0 )
				{
					Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				}
				else if ( !Caster.CanBeginAction( typeof( DefensiveSpell ) ) )
				{
					Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
				}
				else if ( CheckSequence() )
				{
					if ( Caster.BeginAction( typeof( DefensiveSpell ) ) )
					{
						int value = (int)(Caster.Skills[SkillName.Magery].Value + Caster.Skills[SkillName.Inscribe].Value);
						value = (int)(8 + (value/200)*7.0);//absorb from 8 to 15 "circles"

						Caster.MagicDamageAbsorb = value;

						Caster.FixedParticles( 0x375A, 10, 15, 5037, EffectLayer.Waist );
						Caster.PlaySound( 0x1E9 );
					}
					else
					{
						Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
					}
				}

				FinishSequence();
			}
		}

		#region mod by Dies Irae
		private static bool Targetable = true;

		public void Target( Mobile m )
		{
			if( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( m.MagicDamageAbsorb > 0 )
			{
				Caster.SendMessage( "This target already has Magic Reflection." );
			}
			else if( !m.CanBeginAction( typeof( DefensiveSpell ) ) )
			{
				if( Caster != m )
					Caster.SendMessage( "The spell will not adhere to him at this time." );
				else
					Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
			}
			else if( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				/* demo code
				int value = 0;
				if( Caster.Skills[ CastSkill ].Value < 0x0A )
					value = 0x0A;
				else
					value = (int)( 0x0A * ( Caster.Skills[ CastSkill ].Value / 0x05 ) );
				*/

				/*
				int value = (int)( Caster.Skills[ SkillName.Magery ].Value + Caster.Skills[ SkillName.Inscribe ].Value );
				value = (int)( 8 + ( value / 200.0 ) * 7.0 ); //absorb from 8 to 15 "circles"
				*/

				m.MagicDamageAbsorb = 1 /* value */;
				m.FixedParticles( 0x376A, 0x0A, 0x0F, 5008, EffectLayer.Waist );
				// doMobAnimation(usedon, 0x375A, 0x0A, 0x0F, 0x00, 0x00);
				m.PlaySound( 0x01E9 );
			}

			FinishSequence();
		}

		protected class InternalTarget : Target
		{
			MagicReflectSpell m_Owner;

			public InternalTarget( MagicReflectSpell owner ) : base( 10, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
				else
				{
					from.SendMessage( "This target is not valid. It must be a being or person." );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
		#endregion

		#region Mondain's Legacy
		public static void EndReflect( Mobile m )
		{
			if ( m_Table.Contains( m ) )
			{
				ResistanceMod[] mods = (ResistanceMod[]) m_Table[ m ];

				if ( mods != null )
				{
					for ( int i = 0; i < mods.Length; ++i )
						m.RemoveResistanceMod( mods[ i ] );
				}

				m_Table.Remove( m );
				BuffInfo.RemoveBuff( m, BuffIcon.MagicReflection );
			}
		}
		#endregion
	}
}
