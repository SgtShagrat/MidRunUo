/***************************************************************************
 *							   EssenceOfWind.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Spells;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class EssenceOfWindSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Essence of Wind", "Ant Sepa Aeta",
			224,
			9011,
			Reagent.DestroyingAngel,
			Reagent.SpringWater
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( EssenceOfWindSpell ),
				"Deals cold damage and gives a swing speed and Faster Casting penalty to nearby enemies.",
				"Una folata di aria gelida investe tutti i nemici che circondano il Druido."+
				"Raggio (5 + FL); Durata (SK/24 + FL); Danno (7d8 + 10 + FL).",
				0x59e2
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override SpellCircle Circle{get { return SpellCircle.Seventh; }}

		public EssenceOfWindSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage { get { return true; } }

		private static readonly DiceRoll DamageDice = new DiceRoll( "7d8+10" );
		private const int MinDamageRange = 5;

		public void Target( IPoint3D p )
		{
			if( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				if( p is Item )
					p = ( (Item)p ).GetWorldLocation();

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;
				if( map == null )
				{
					FinishSequence();
					return;
				}

				int focus = FocusLevel;
				double skill = Caster.Skills[ SkillName.Spellweaving ].Value;

				int range = MinDamageRange + focus;
				int damage = DamageDice.Roll() + focus;
				TimeSpan duration = TimeSpan.FromSeconds( (int)( skill / 24 ) + focus );
				int fcMalus = focus + 1;
				int ssiMalus = 2 * ( focus + 1 );

				IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), range );

				foreach( Mobile m in eable )
				{
					if( m == Caster )
						continue;

					if( Caster != m && Caster.InLOS( m ) && SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) )
						targets.Add( m );
				}

				eable.Free();

				Caster.PlaySound( 0x5C6 );

				foreach( Mobile m in Caster.GetMobilesInRange( range ) )
				{
					if( Caster != m && Caster.InLOS( m ) && SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) )
						targets.Add( m );
				}

				if( targets.Count > 0 )
				{
					foreach( Mobile m in targets )
					{
						Caster.DoHarmful( m );
						if( m.Player )//edit by Arlas
							damage /= 4;
						else
							damage += focus;

						if ( CheckResisted( m ) )
						{
							damage = (int)(damage*( 1.0 - GetResistScalar( m ) ));//0.75;

							m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
							MidgardSpellHelper.Damage( m, Caster, damage, CustomResType.General );
							continue;
						}

						MidgardSpellHelper.Damage( m, Caster, damage, CustomResType.General );

						m_Table[ m ] = new EssenceOfWindInfo( m, fcMalus, ssiMalus, duration );

						if( m.NetState != null && !TransformationSpellHelper.UnderTransformation( m, typeof( AnimalFormSpell ) ) && m.AccessLevel < AccessLevel.GameMaster )
							m.Send( SpeedControl.WalkSpeed );

						m.SolidHueOverride = 0x482;
					}
				}
				else
				{
					Caster.PlaySound ( 0x29 );
				}
			}

			FinishSequence();
		}

		private static readonly Dictionary<Mobile, EssenceOfWindInfo> m_Table = new Dictionary<Mobile, EssenceOfWindInfo>();

		private class EssenceOfWindInfo
		{
			private Mobile Defender { get; set; }

			public int FCMalus { get; private set; }
			public int SSIMalus { get; private set; }
			public ExpireTimer Timer { get; private set; }

			public EssenceOfWindInfo( Mobile defender, int fcMalus, int ssiMalus, TimeSpan duration )
			{
				Defender = defender;

				FCMalus = fcMalus;
				SSIMalus = ssiMalus;

				Timer = new ExpireTimer( Defender, duration );
				Timer.Start();
			}
		}

		public static double GetFCMalus( Mobile m )
		{
			EssenceOfWindInfo info;

			if( m_Table.TryGetValue( m, out info ) )
				return info.FCMalus * 0.25;

			return 0.0;
		}

		public static int GetSSIMalus( Mobile m )
		{
			EssenceOfWindInfo info;

			if( m_Table.TryGetValue( m, out info ) )
				return info.SSIMalus;

			return 0;
		}

		public static bool IsDebuffed( Mobile m )
		{
			return m_Table.ContainsKey( m );
		}

		public static void StopDebuffing( Mobile m, bool message )
		{
			EssenceOfWindInfo info;

			if( m_Table.TryGetValue( m, out info ) )
				info.Timer.DoExpire();
		}

		private class ExpireTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public ExpireTimer( Mobile m, TimeSpan delay )
				: base( delay )
			{
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				DoExpire();
			}

			public void DoExpire()
			{
				Stop();

				if( m_Mobile == null )
					return;

				if( m_Mobile.NetState != null )
				{
					m_Mobile.Send( SpeedControl.Disable );
					m_Mobile.SolidHueOverride = -1;
				}

				m_Table.Remove( m_Mobile );
			}
		}

		public class InternalTarget : Target
		{
			private readonly EssenceOfWindSpell m_Owner;

			public InternalTarget( EssenceOfWindSpell owner )
				: base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				if( o != null && o is Mobile && o == m_Owner.Caster )
				{
					m_Owner.Caster.SendMessage( (m_Owner.Caster.Language == "ITA" ? "Non puoi farlo su te stesso." : "Thou cannot target yourself.") );
					return;
				}

				if( o is Mobile )
					m_Owner.Target( ( (Mobile)o ).Location );
				else if( m_Owner.Caster != null )
					m_Owner.Caster.SendMessage( (m_Owner.Caster.Language == "ITA" ? "Devi selezionare un essere vivente." : "Thou must target a valid living target.") );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}