using System;

using Midgard.Engines.SpellSystem;

using Server.Mobiles;
using Midgard.Engines.PlagueBeastLordPuzzle;
using Server.Targeting;
using Server.Network;
using Server.Spells.Chivalry;

namespace Server.Spells.Fifth
{
	public class ParalyzeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Paralyze", "An Ex Por",
				218,
				9012,
				Reagent.Garlic,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public ParalyzeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( Core.AOS && (m.Frozen || m.Paralyzed || (m.Spell != null && m.Spell.IsCasting && !(m.Spell is PaladinSpell))) )
			{
				Caster.SendLocalizedMessage( 1061923 ); // The target is already frozen.
			}
			#region mod by Dies Irae
            else if( !Core.AOS && ( m.Frozen || m.Paralyzed ) )
            {
                Caster.SendLocalizedMessage( 1061923 ); // The target is already frozen.
            }
		    #endregion
            else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );

                #region mod by Dies Irae
                if( HandleSelfMagicalAbsorption( m ) )
                    return;
                #endregion

				double duration;
				
				if ( Core.AOS )
				{
					int secs = (int)((GetDamageSkill( Caster ) / 10) - (GetResistSkill( m ) / 10));
					
					if( !Core.SE )
						secs += 2;

					if ( !m.Player )
						secs *= 3;

					if ( secs < 0 )
						secs = 0;

					duration = secs;
				}
				else
				{
					// Algorithm: ((20% of magery) + 7) seconds [- 50% if resisted]

					duration = Utility.Random(4) + (Caster.Skills[SkillName.Magery].Value /5.0); //7.0 + (Caster.Skills[SkillName.Magery].Value * 0.2);
					if (duration < 5.0 )
						duration = 5.0;
                    #region mod by Dies Irae
                    if( !Core.AOS && m.Player )
                        duration = MidgardSpellHelper.ScaleByCustomRes( duration, m, CustomResType.General );
                    #endregion

				    if ( CheckResisted( m ) )
					{
                        // duration *= 0.75;

                        m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.

					    return; // mod by Dies Irae
					}
				}

                /*
				if ( m is PlagueBeastLord )
				{
					( (PlagueBeastLord) m ).OnParalyzed( Caster );
					duration = 120;
				}
                */

				#region Modifica by Dies Irae
                if( m is PuzzlePlagueBeastLord )
                {
                	PuzzlePlagueBeastLord plaguebl = (PuzzlePlagueBeastLord)m;
					Caster.Combatant = null;
					if( !plaguebl.IsFrosted )
						plaguebl.IsFrosted = true;
                }
				#endregion

				m.Paralyze( TimeSpan.FromSeconds( duration ) );

                #region mod by Dies Irae
                if( m.Spell != null )
                    m.Spell.OnCasterHurt();
                #endregion

			    m.PlaySound( 0x204 );
				m.FixedEffect( 0x376A, 6, 1 );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private ParalyzeSpell m_Owner;

			public InternalTarget( ParalyzeSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}