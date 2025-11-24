/***************************************************************************
 *								  BalanceOfDeathSpell.cs
 *									----------------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Con questo potente maleficio il necromante danneggia la sua vittima
 * 			in maniera che la percentuale della sua vita (rispetto la massima )
 * 			sia la stessa del necromante (rispetto la sua massima).
 * 
 * 			L'incantesimo funziona solo se il necromante e la vittima hanno 
 * 			almeno il 30% della loro forza vitale.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.Classes;

using Server;
using Server.Spells;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class BalanceOfDeathSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Balance Of Death", "Kal Rel Des Mani",
			-1,
			9002,
			true,
			Reagent.BatWing,
			Reagent.DaemonBlood
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( BalanceOfDeathSpell ),
			"This curse balance the Necromancer army resistance to dispel through the application of a blood rule of equity.",
			"Con questo potente maleficio il necromante bilancia la resistenza al dispel delle sue creature secondo un legge maledetta di equità di sangue.",
			0x5000
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override int RequiredMana{get { return 10; }}
		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 1.5 ); }}
		public override double DelayOfReuse{get { return 5.0; }}
		public override double RequiredSkill{get { return 60.0; }}
		public override bool BlocksMovement{get { return true; }}

		private const double MinPercOfUsage = 0.40;

		public BalanceOfDeathSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if( Caster.CanBeginAction( typeof( BalanceOfDeathSpell ) ) )
			{
				if( CheckSequence() )
				{
					//DoBloodOffering( Caster );
					Caster.BeginAction( typeof( BalanceOfDeathSpell ) );
					Caster.Target = new InternalTarget( this );
					Timer.DelayCall( GetDelayOfReuse(), new TimerStateCallback( ReleaseBalanceOfDeathLock ), Caster );
				}
				FinishSequence();
			}
			else
				Caster.SendMessage( Caster.Language == "ITA" ? "Non è passato abbastanza tempo dall'ultimo utilizzo." : "Not enough time has passed from last equality perversion." );
		}

		public void DoBalance( BaseCreature target )
		{
			//Caster.BeginAction( typeof( BalanceOfDeathSpell ) );
			int level = GetPowerLevel();
			double necro = Caster.Skills[SkillName.Necromancy].Value;

			if ( target.GetMaster() == Caster && target is BaseNecroFamiliar && ((BaseNecroFamiliar)target).CusDispelDifficulty < necro )
			{

				((BaseNecroFamiliar)target).CusDispelDifficulty = necro + level*3.5;
				//target.DispelFocus = 10*(6 - level);

				Caster.FixedParticles( 0x375A, 1, 17, 9919, 1157, 7, EffectLayer.Waist );
				Caster.FixedParticles( 0x3728, 1, 13, 9502, 1157, 7, (EffectLayer)255 );
				ClassSystem.Necromancer.SendOverheadMessage( Caster, Caster.Language == "ITA" ? "*uguaglianza è sangue*" : "*equality is blood*" );

				target.FixedParticles( 0x375A, 1, 17, 9919, 1157, 7, EffectLayer.Waist );
				target.FixedParticles( 0x3728, 1, 13, 9502, 1157, 7, (EffectLayer)255 );
				ClassSystem.Necromancer.SendOverheadMessage( target, Caster.Language == "ITA" ? "*uguaglianza è sangue*" : "*equality is blood*" );
			}
		}
		/*
		public void Target( Mobile m )
		{
			if( CheckHSequence( m ) )
			{
				if( m.Player )
				{
					try
					{
						Map map = Caster.Map;
						if( map == null || map == Map.Internal )
							return;

						double percNecroHits = Caster.Hits / (double)Caster.HitsMax;
						double percTargetHits = m.Hits / (double)m.HitsMax;

						if( percTargetHits > MinPercOfUsage && percNecroHits > MinPercOfUsage )
						{
							Caster.BeginAction( typeof( BalanceOfDeathSpell ) );

							SpellHelper.Turn( Caster, m );

							m.FixedParticles( 0x37C4, 1, 8, 9916, 39, 3, EffectLayer.Head );
							m.FixedParticles( 0x37C4, 1, 8, 9502, 39, 4, EffectLayer.Head );
							m.PlaySound( 0x210 );

							#region get party victims
							List<Mobile> leveled = new List<Mobile>();

							Party p = Party.Get( Caster );

							if( p != null )
							{
								foreach( PartyMemberInfo pmi in p.Members )
								{
									Mobile partyMember = pmi.Mobile;

									if( partyMember != null && Caster != partyMember && m != partyMember )
										leveled.Add( partyMember );
								}
							}
							#endregion

							#region get nearby victims
							foreach( Mobile mo in Caster.GetMobilesInRange( 1 + GetPowerLevel() * 2 ) )
							{
								if( Caster != mo && m != mo && ClassSystem.IsEvilOne( mo ) )
								{
									if( SpellHelper.ValidIndirectTarget( Caster, mo ) && Caster.CanBeHarmful( mo, false ) && Caster.InLOS( mo ) )
									{
										if( !leveled.Contains( mo ) )
											leveled.Add( mo );
									}
								}
							}
							#endregion

							#region damage victims (not target)
							for( int i = 0; i < leveled.Count; ++i )
							{
								Mobile lev = leveled[ i ];
								double percLevHits = lev.Hits / (double)lev.HitsMax;

								if( ClassSystem.IsEvilOne( lev ) )
									percLevHits -= GetPowerLevel() * 2;

								if( percLevHits > MinPercOfUsage )
								{
									int newHits = (int)( lev.HitsMax * percLevHits );

									if( newHits > lev.HitsMax )
										newHits = lev.HitsMax;

									if( newHits < 1 )
										newHits = 1;

									Caster.DoHarmful( lev );
									SpellHelper.Damage( this, lev, Math.Abs( lev.HitsMax - newHits ) );

									ClassSystem.Necromancer.SendOverheadMessage( lev, "* equality is blood *" );
								}
							}
							#endregion

							Caster.FixedParticles( 0x375A, 1, 17, 9919, 1157, 7, EffectLayer.Waist );
							Caster.FixedParticles( 0x3728, 1, 13, 9502, 1157, 7, (EffectLayer)255 );
							ClassSystem.Necromancer.SendOverheadMessage( Caster,"*equality is blood*" );

							m.FixedParticles( 0x375A, 1, 17, 9919, 1157, 7, EffectLayer.Waist );
							m.FixedParticles( 0x3728, 1, 13, 9502, 1157, 7, (EffectLayer)255 );
							ClassSystem.Necromancer.SendOverheadMessage( m, "*equality is blood*" );
						}
					}
					catch( Exception ex )
					{
						Console.WriteLine( ex.ToString() );
					}

					Timer.DelayCall( GetDelayOfReuse(), new TimerStateCallback( ReleaseBalanceOfDeathLock ), Caster );
				}
				else
				{
					Caster.SendMessage( "This necromantic power can be used only on other active players." );
				}
			}

			FinishSequence();
		}
		*/
		private static void ReleaseBalanceOfDeathLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( BalanceOfDeathSpell ) );
			( (Mobile)state ).SendMessage( ((Mobile)state).Language == "ITA" ? "Le trasfusioni di sangue non sono male.. vero?" : "Equality is a good thing... don't you think?" );
		}

		public class InternalTarget : Target
		{
			private readonly BalanceOfDeathSpell m_Owner;

			public InternalTarget( BalanceOfDeathSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				//if( o != null && o is Mobile && o == m_Owner.Caster )
				//{
				//	m_Owner.Caster.SendMessage( "Thou cannot target yourself." );
				//	return;
				//}

				if( o is BaseCreature )
					m_Owner.DoBalance( (BaseCreature)o );
				else if( m_Owner.Caster != null )
					m_Owner.Caster.SendMessage( m_Owner.Caster.Language == "ITA" ? "Devi selezionare una tua evocazione." : "Thou must target a valid living target." );
			}

			protected override void OnTargetFinish( Mobile m )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}