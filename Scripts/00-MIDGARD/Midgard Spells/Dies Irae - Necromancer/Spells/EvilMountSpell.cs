/***************************************************************************
 *								  EvilMountSpell.cs
 *									-------------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Questo potente maleficio evoca un destriero non morto per il
 * 			necormante. Esso sara' resistente agli attacchi dei paladini
 * 			e durerà Spirit Speak minuti.
 * 
 ***************************************************************************/

using System;
using System.Collections;

using Midgard.Engines.Classes;

using Server;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public class EvilMountSpell : RPGNecromancerSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
			"Evil Mount", "Uus Corp Xen",
			-1,
			9002,
			false,
			Reagent.DaemonBlood,
			Reagent.PigIron,
			Reagent.GraveDust
			);

		private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( EvilMountSpell ),
			"This curse summons the Mount of the Necromancer.",
			"Questo maleficio evoca un destriero non morto per il necormante.",
			0x5005
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override int RequiredMana{get { return 40; }}
		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 10.0 ); }}
		public override double DelayOfReuse{get { return 600.0; }}

		public override double RequiredSkill{get { return 90.0; }}
		public override bool BlocksMovement{get { return true; }}

		public EvilMountSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static Hashtable m_Table = new Hashtable();

		public static Hashtable Table
		{
			get { return m_Table; }
		}

		public static bool HasMount( Mobile m )
		{
			return m_Table != null && m_Table.ContainsKey( m ) && m_Table[ m ] != null;
		}

		public override bool CheckCast()
		{
			if( !Caster.CanBeginAction( typeof( EvilMountSpell ) ) )
			{
				Caster.SendMessage( "You are too weak to summon another minion!" );
				return false;
			}

			if( HasMount( Caster ) )
			{
				Caster.SendMessage( "Necromancer, your servant soul is already wandering on this world." );
				return false;
			}

			return base.CheckCast();
		}

		public override void OnCast()
		{
			if( CheckSequence() )
			{
				Caster.BeginAction( typeof( EvilMountSpell ) );

				TimeSpan duration = TimeSpan.FromMinutes( 5 + ( GetPowerLevel() * 2 ) );

				try
				{
					EvilMount familiar = new EvilMount();

					if( BaseCreature.Summon( familiar, Caster, Caster.Location, 0x217, duration ) )
					{
						Caster.SendMessage( "Necromancer, your you have now a new servant. Its flesh is yours." );

						Caster.FixedParticles( 0x3728, 1, 10, 9910, EffectLayer.Head );
						familiar.PlaySound( familiar.GetIdleSound() );
						Table[ Caster ] = familiar;
					}
				}
				catch( Exception ex )
				{
					Console.WriteLine( ex.ToString() );
				}
			}

			FinishSequence();
		}

		public static void Unregister( Mobile master )
		{
			if( master == null || !m_Table.ContainsKey( master ) )
				return;

			m_Table.Remove( master );
		}

		public static void Initialize()
		{
			EventSink.Speech += new SpeechEventHandler( EventSink_FreeMount );
		}

		public static void EventSink_FreeMount( SpeechEventArgs args )
		{
			Mobile caster = args.Mobile;
			if( caster == null || !caster.Player || !caster.Alive )
				return;

			if( ClassSystem.IsNecromancer( caster ) && Insensitive.Compare( args.Speech, "My servant is free" ) == 0 )
			{
				if( HasMount( caster ) )
				{
					EvilMount mount = ( (EvilMount)Table[ caster ] );
					if( mount != null && !mount.Deleted )
						mount.Delete();

					Unregister( caster );
				}

				caster.SendMessage( "Your Servant has been released!" );
			}
		}
	}
}