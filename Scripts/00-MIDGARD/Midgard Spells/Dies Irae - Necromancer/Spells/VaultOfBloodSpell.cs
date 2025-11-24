/***************************************************************************
 *								  VaultOfBlood.cs
 *									---------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 *			Questo maleficio crea una pozza di sangue che permette a chi
 * 			vi entra di essere resuscitato all'istante.
 * 
 * 			Per ogni anima resuscitata il necromante subisce 1d30 + 10 danni.
 * 
 * 			Se l'anima è di un paladino l'incantesimo termina all'istante,
 * 			egli non viene resuscitato e tutte le creature nel raggio 
 * 			( SpiritSpeak / 5 ) con karma negativo subiscono 1d30 + 30 danni.
 * 
 * 			I necromanti non possono resuscitare nelle pozze di sangue.
 ***************************************************************************/

using System;

using Server;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class VaultOfBloodSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Vault Of Blood", "Uus Flam An Corp",
			-1,
			9002,
			false,
			Reagent.DaemonBlood,
			Reagent.GraveDust,
			Reagent.BatWing
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( VaultOfBloodSpell ),
			"This powerfull curse create a pool of blood which resurrect each being passing on it.",
			"Questo maleficio crea una pozza di sangue che permette a chi vi entra di essere resuscitato all'istante.",
			0x5009
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override int RequiredMana{get { return 35; }}
		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 5.0 ); }}
		public override double DelayOfReuse{get { return 60.0; }}
		public override double RequiredSkill{get { return 85.0; }}

		public override bool BlocksMovement{get { return true; }}

		public VaultOfBloodSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if( Caster.CanBeginAction( typeof( VaultOfBloodSpell ) ) )
				Caster.Target = new InternalTarget( this );
			else
				Caster.SendMessage( Caster.Language == "ITA" ? "Non puoi ancora usare questo incantesimo." : "You cannot summon another vault yet!" );
		}

		public void Target( IPoint3D p )
		{
			if( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( MidgardSpellHelper.CheckBlockField( p, Caster ) && SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );
				SpellHelper.GetSurfaceTop( ref p );
				Caster.BeginAction( typeof( VaultOfBloodSpell ) );

				Map map = Caster.Map;
				if( map == null || map == Map.Internal )
					return;

				IEntity to;

				if( p is Mobile )
					to = (Mobile) p;
				else
					to = new Entity( Serial.Zero, new Point3D( p ), map );

				TimeSpan duration = TimeSpan.FromSeconds( GetPowerLevel() * 15 );

				Caster.MovingEffect( to, 0xECA, 10, 0, false, false, 0, 0 );

				Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( DoVault_Callback ), new object[] { Caster, p, map, duration } );
				Timer.DelayCall( GetDelayOfReuse(), new TimerStateCallback( ReleaseVaultOfBloodLock ), Caster );
			}

			FinishSequence();
		}

		private static void DoVault_Callback( object state )
		{
			object[] states = (object[]) state;

			Mobile caster = (Mobile) states[ 0 ];
			IPoint3D p = (IPoint3D) states[ 1 ];
			Map map = (Map) states[ 2 ];
			TimeSpan duration = (TimeSpan) states[ 3 ];

			if( map != Map.Internal )
			{
				BloodVault vault = new BloodVault( caster );
				vault.MoveToWorld( new Point3D( p.X - 3, p.Y - 3, p.Z ), map );
				Timer.DelayCall( duration, new TimerStateCallback( DeleteVault_Callback ), vault );
			}
		}

		private static void DeleteVault_Callback( object state )
		{
			BloodVault vault = (BloodVault) state;
			if( vault != null && !vault.Deleted )
				vault.Delete();
		}

		private static void ReleaseVaultOfBloodLock( object state )
		{
			( (Mobile) state ).EndAction( typeof( VaultOfBloodSpell ) );
			( (Mobile) state ).SendMessage( ((Mobile) state).Language == "ITA" ? "Puoi riutilizzare il tuo sangue liberamente." : "You can summon a new vault around you." );
		}

		private class InternalTarget : Target
		{
			private readonly VaultOfBloodSpell m_Owner;

			public InternalTarget( VaultOfBloodSpell owner ) : base( 10, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}