/***************************************************************************
 *								  BloodOfferingSpell.cs
 *									---------------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Con questo potente spell il necromante trasferisce parte della
 *  		sua forza vitale alla sua armata non-morta.
 * 
 * 			Il raggio d'azione dello spell e' dato na SpiritSpeak / 12;
 * 	
 * 			La quantita' di forza vitale trasmessa è di 1 d ( SpiritSpeak / 4 ) + 10.
 * 			Il necromante subisce un danno pari a un decimo della vita trasferita.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Network;
using Server.Spells;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class BloodOfferingSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Blood Offering", "Uus Mani Corp Xen",
			-1,
			9002,
			false,
			Reagent.DaemonBlood,
			Reagent.GraveDust
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( BloodOfferingSpell ),
			"This power force some Necromancer's life force to be trasferred onto the unded creature targeted.",
			"Con questo potente maleficio il necromante trasferisce parte della sua forza vitale alla sua armata non-morta.",
			0x5002
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override int RequiredMana{get { return 10; }}
		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 1.5 ); }}
		public override double DelayOfReuse{get { return 5.0; }}
		public override double RequiredSkill{get { return 60.0; }}
		public override bool BlocksMovement{get { return true; }}

		public BloodOfferingSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if( Caster.CanBeginAction( typeof( BloodOfferingSpell ) ) )
			{
				if( CheckSequence() )
				{
					//DoBloodOffering( Caster );
					Caster.BeginAction( typeof( BloodOfferingSpell ) );
					Caster.Target = new InternalTarget( this );
					Timer.DelayCall( GetDelayOfReuse(), new TimerStateCallback( ReleaseBloodOfferingLock ), Caster );
				}

				FinishSequence();
			}
			else
				Caster.SendMessage( Caster.Language == "ITA" ? "Non sei ancora pronto per un'altra offerta di sangue." : "You are not ready to offer your life force." );
		}

		public void DoBloodOffering( BaseCreature target )
		{
			//Caster.BeginAction( typeof( BloodOfferingSpell ) );
			if ( target.GetMaster() == Caster && target.Summoned && target.Experience < 0 )
			{
				target.Experience = GetPowerLevel();
				target.RawInt += 10*GetPowerLevel();
				target.FixedParticles( 0x373A, 1, 17, 9903, 15, 4, EffectLayer.Head );
				target.PublicOverheadMessage( MessageType.Regular, 37, true, Caster.Language == "ITA"? "*la forza vitale della creatura aumenta*" : "*the Creature life force increase*" );
			}
		}

		public class InternalTarget : Target
		{
			private readonly BloodOfferingSpell m_Owner;

			public InternalTarget( BloodOfferingSpell owner ) : base( 12, false, TargetFlags.None )
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
					m_Owner.DoBloodOffering( (BaseCreature)o );
				else if( m_Owner.Caster != null )
					m_Owner.Caster.SendMessage( m_Owner.Caster.Language == "ITA" ? "Devi selezionare una tua evocazione." : "Thou must target a valid living target." );
			}

			protected override void OnTargetFinish( Mobile m )
			{
				m_Owner.FinishSequence();
			}
		}
		private static void ReleaseBloodOfferingLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( BloodOfferingSpell ) );
			( (Mobile)state ).SendMessage( ((Mobile)state).Language == "ITA" ? "Il tuo sangue è di nuovo normale." : "Your blood is now normal." );
		}
	}
}