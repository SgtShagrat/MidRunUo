using System;
using System.Collections;

using Midgard;

using Server.Targeting;
using Server.Network;

namespace Server.Spells.First
{
	public class ReactiveArmorSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Reactive Armor", "Flam Sanct",
				236,
				9011,
				Reagent.Garlic,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public ReactiveArmorSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

			if ( Caster.MeleeDamageAbsorb > 0 )
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
				/* The reactive armor spell increases the caster's physical resistance, while lowering the caster's elemental resistances.
				 * 15 + (Inscription/20) Physcial bonus
				 * -5 Elemental
				 * The reactive armor spell has an indefinite duration, becoming active when cast, and deactivated when re-cast. 
				 * Reactive Armor, Protection, and Magic Reflection will stay on—even after logging out, even after dying—until you “turn them off” by casting them again. 
				 * (+20 physical -5 elemental at 100 Inscription)
				 */

				if ( CheckSequence() )
				{
					Mobile targ = Caster;

					ResistanceMod[] mods = (ResistanceMod[])m_Table[targ];

					if ( mods == null )
					{
						targ.PlaySound( 0x1E9 );
						targ.FixedParticles( 0x376A, 9, 32, 5008, EffectLayer.Waist );

						mods = new ResistanceMod[5]
							{
								new ResistanceMod( ResistanceType.Physical, 15 + (int)(targ.Skills[SkillName.Inscribe].Value / 20) ),
								new ResistanceMod( ResistanceType.Fire, -5 ),
								new ResistanceMod( ResistanceType.Cold, -5 ),
								new ResistanceMod( ResistanceType.Poison, -5 ),
								new ResistanceMod( ResistanceType.Energy, -5 )
							};

						m_Table[targ] = mods;

						for ( int i = 0; i < mods.Length; ++i )
							targ.AddResistanceMod( mods[i] );

						int physresist = 15 + (int)(targ.Skills[SkillName.Inscribe].Value / 20);
						string args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", physresist, 5, 5, 5, 5);

						BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.ReactiveArmor, 1075812, 1075813, args.ToString()));
					}
					else
					{
						targ.PlaySound( 0x1ED );
						targ.FixedParticles( 0x376A, 9, 32, 5008, EffectLayer.Waist );

						m_Table.Remove( targ );

						for ( int i = 0; i < mods.Length; ++i )
							targ.RemoveResistanceMod( mods[i] );

						BuffInfo.RemoveBuff(Caster, BuffIcon.ReactiveArmor);
					}
				}

				FinishSequence();
			}
			else
			{
				if ( Caster.MeleeDamageAbsorb > 0 )
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
						int value = (int)(Caster.Skills[SkillName.Magery].Value + Caster.Skills[SkillName.Meditation].Value + Caster.Skills[SkillName.Inscribe].Value);
						value /= 3;

						if ( value < 0 )
							value = 1;
						else if ( value > 75 )
							value = 75;

						Caster.MeleeDamageAbsorb = value;

						Caster.FixedParticles( 0x376A, 9, 32, 5008, EffectLayer.Waist );
						Caster.PlaySound( 0x1F2 );
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
            /*
            else if( m.MeleeDamageAbsorb > 0 )
            {
                Caster.SendMessage( "This target already has Reactive Armor." );
            }*/
            else if( !m.CanBeginAction( typeof( DefensiveSpell ) ) )
            {
                if( Caster != m )
                    Caster.SendMessage( Caster.Language == "ITA" ? "L'incantesimo non aderisce al tuo bersaglio." : "The spell will not adhere to him at this time." );
                else
                    Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
            }
            else if( CheckBSequence( m ) )
            {
                SpellHelper.Turn( Caster, m );

                m.MeleeDamageAbsorb = (int)( DiceRoll.Roll( "1d8" ) + m.Skills[ CastSkill ].Value / 10 );
                // mod_amount := randomdiceroll("1d8")+cint(magery/10);

                m.FixedParticles( 0x376A, 0x09, 0x20, 5008, EffectLayer.Waist );
                // doMobAnimation(usedon, 0x376A, 0x09, 0x20, 0x00, 0x00);

                m.PlaySound( 0x1F2 );
            }
            else
            {
                Caster.SendMessage( Caster.Language == "ITA" ? "Bersaglio non valido. Devi lanciarlo su di un essere vivente." : "This target is not valid. It must be a being or person." );
            }

            FinishSequence();
        }

        protected class InternalTarget : Target
        {
            ReactiveArmorSpell m_Owner;

            public InternalTarget( ReactiveArmorSpell owner )
                : base( 10, false, TargetFlags.Beneficial )
            {
                m_Owner = owner;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is Mobile )
                {
                    m_Owner.Target( (Mobile)o );
                }
            }

            protected override void OnTargetFinish( Mobile from )
            {
                m_Owner.FinishSequence();
            }
        }
	    #endregion

		#region Mondain's Legacy
		public static void EndArmor( Mobile m )
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
				BuffInfo.RemoveBuff( m, BuffIcon.ReactiveArmor );
			}
		}
		#endregion
	}
}