using System;

using Midgard.Engines.SpellSystem;

using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;

namespace Server
{
	public class PoisonImpl : Poison
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
			if ( Core.AOS )
			{
				Register( new PoisonImpl( "Lesser", "Minore",		0,  4, 16,  7.5, 3.0, 2.25, 10, 4 ) );
				Register( new PoisonImpl( "Regular", "Normale",		1,  8, 18, 10.0, 3.0, 3.25, 10, 3 ) );
				Register( new PoisonImpl( "Greater", "Maggiore",	2, 12, 20, 15.0, 3.0, 4.25, 10, 2 ) );
				Register( new PoisonImpl( "Deadly", "Mortale",		3, 16, 30, 30.0, 3.0, 5.25, 15, 2 ) );
				Register( new PoisonImpl( "Lethal", "Letale",		4, 20, 50, 35.0, 3.0, 5.25, 20, 2 ) );
			}
			else
			{
				Register( new PoisonImpl( "Lesser", "Minore",		0, 4, 26,  2.500, 3.5, 3.0, 10, 4 ) );
				Register( new PoisonImpl( "Regular", "Normale",		1, 5, 26,  3.125, 3.5, 3.0, 10, 3 ) );
				Register( new PoisonImpl( "Greater", "Maggiore",	2, 6, 26,  6.250, 3.5, 3.0, 10, 2 ) );
				Register( new PoisonImpl( "Deadly", "Mortale",		3, 7, 26, 12.500, 3.5, 4.0, 10, 2 ) );
				Register( new PoisonImpl( "Lethal", "Letale",		4, 9, 26, 25.000, 3.5, 5.0, 10, 2 ) );
			}
			
			#region Mondain's Legacy
			if ( Core.ML )
			{
				Register( new PoisonImpl( "LesserDarkglow", "Darkglow Minore",		10,  4, 16,  7.5, 3.0, 2.25, 10, 4 ) );
				Register( new PoisonImpl( "RegularDarkglow", "Darkglow Normale",	11,  8, 18, 10.0, 3.0, 3.25, 10, 3 ) );
				Register( new PoisonImpl( "GreaterDarkglow", "Darkglow Maggiore",	12, 12, 20, 15.0, 3.0, 4.25, 10, 2 ) );
				Register( new PoisonImpl( "DeadlyDarkglow", "Darkglow Mortale",		13, 16, 30, 30.0, 3.0, 5.25, 15, 2 ) );
				
				Register( new PoisonImpl( "LesserParasitic", "Parasitic Minore",	14,  4, 16,  7.5, 3.0, 2.25, 10, 4 ) );
				Register( new PoisonImpl( "RegularParasitic", "Parasitic Normale",	15,  8, 18, 10.0, 3.0, 3.25, 10, 3 ) );
				Register( new PoisonImpl( "GreaterParasitic", "Parasitic Maggiore",	16, 12, 20, 15.0, 3.0, 4.25, 10, 2 ) );
				Register( new PoisonImpl( "DeadlyParasitic", "Parasitic Mortale",	17, 16, 30, 30.0, 3.0, 5.25, 15, 2 ) );
				Register( new PoisonImpl( "LethalParasitic", "Parasitic Letale",	18, 20, 50, 35.0, 3.0, 5.25, 20, 2 ) );
			}
			#endregion
			#region Poison Engine [Arlas]
			Register( new PoisonImpl( "MagiaLesser", "Magia Minore",	19, 4, 26,  2.500, 3.5, 3.0, 10, 4 ) );
			Register( new PoisonImpl( "MagiaRegular", "Magia Normale",	20, 5, 26,  3.125, 3.5, 3.0, 10, 3 ) );
			Register( new PoisonImpl( "MagiaGreater", "Magia Maggiore",	21, 6, 26,  6.250, 3.5, 3.0, 10, 2 ) );
			Register( new PoisonImpl( "MagiaDeadly", "Magia Mortale",	22, 7, 26, 12.500, 3.5, 4.0, 10, 2 ) );
			Register( new PoisonImpl( "MagiaLethal", "Magia Letale",	23, 9, 26, 25.000, 3.5, 5.0, 10, 2 ) );

			Register( new PoisonImpl( "StanchezzaLesser", "Stanchezza Minore",	24, 4, 26,  2.500, 3.5, 3.0, 10, 4 ) );
			Register( new PoisonImpl( "StanchezzaRegular", "Stanchezza Normale",	25, 5, 26,  3.125, 3.5, 3.0, 10, 3 ) );
			Register( new PoisonImpl( "StanchezzaGreater", "Stanchezza Maggiore",	26, 6, 26,  6.250, 3.5, 3.0, 10, 2 ) );
			Register( new PoisonImpl( "StanchezzaDeadly", "Stanchezza Mortale",	27, 7, 26, 12.500, 3.5, 4.0, 10, 2 ) );
			Register( new PoisonImpl( "StanchezzaLethal", "Stanchezza Letale",	28, 9, 26, 25.000, 3.5, 5.0, 10, 2 ) );

			Register( new PoisonImpl( "ParalisiLesser", "Paralisi Minore",		29,  4, 16,  7.5, 2, 2.0, 2, 1 ) );
			Register( new PoisonImpl( "ParalisiRegular", "Paralisi Normale",	30,  8, 18, 10.0, 2, 3.0, 2, 1 ) );
			Register( new PoisonImpl( "ParalisiGreater", "Paralisi Maggiore",	31, 12, 20, 15.0, 2, 4.0, 2, 1 ) );
			Register( new PoisonImpl( "ParalisiDeadly", "Paralisi Mortale",		32, 16, 30, 30.0, 2, 5.0, 2, 1 ) );
			Register( new PoisonImpl( "ParalisiLethal", "Paralisi Letale",		33, 20, 50, 35.0, 2, 6.0, 2, 1 ) );

			Register( new PoisonImpl( "BloccoLesser", "Blocco Minore",	34,  4, 16,  7.5, 1, 2.0, 2, 1 ) );
			Register( new PoisonImpl( "BloccoRegular", "Blocco Normale",	35,  8, 18, 10.0, 1, 4.0, 2, 1 ) );
			Register( new PoisonImpl( "BloccoGreater", "Blocco Maggiore",	36, 12, 20, 15.0, 1, 6.0, 2, 1 ) );
			Register( new PoisonImpl( "BloccoDeadly", "Blocco Mortale",	37, 16, 30, 30.0, 1, 8.0, 2, 1 ) );
			Register( new PoisonImpl( "BloccoLethal", "Blocco Letale",	38, 20, 50, 35.0, 1, 10.0, 2, 1 ) );

			Register( new PoisonImpl( "LentezzaLesser", "Lentezza Minore",		39,  4, 16,  7.5, 3.5, 10.0, 2, 1 ) );
			Register( new PoisonImpl( "LentezzaRegular", "Lentezza Normale",	40,  8, 18, 10.0, 3.5, 15.0, 2, 1 ) );
			Register( new PoisonImpl( "LentezzaGreater", "Lentezza Maggiore",	41, 12, 20, 15.0, 3.5, 20.0, 2, 1 ) );
			Register( new PoisonImpl( "LentezzaDeadly", "Lentezza Mortale",		42, 16, 30, 30.0, 3.5, 25.0, 2, 1 ) );
			Register( new PoisonImpl( "LentezzaLethal", "Lentezza Letale",		43, 20, 50, 35.0, 3.5, 30.0, 2, 1 ) );
			#endregion
		}

		public static Poison IncreaseLevel( Poison oldPoison )
		{
			Poison newPoison = ( oldPoison == null ? null : GetPoison( oldPoison.Level + 1 ) );

			return ( newPoison == null ? oldPoison : newPoison );
		}

		// Info
		private string m_Name;
		private string m_NameIt;
		private int m_Level;

		// Damage
		private int m_Minimum, m_Maximum;
		private double m_Scalar;

		// Timers
		private TimeSpan m_Delay;
		private TimeSpan m_Interval;
		private int m_Count, m_MessageInterval;

		public PoisonImpl( string name, string nameit, int level, int min, int max, double percent, double delay, double interval, int count, int messageInterval )
		{
			m_Name = name;
			m_NameIt = nameit;
			m_Level = level;
			m_Minimum = min;
			m_Maximum = max;
			m_Scalar = percent * 0.01;
			m_Delay = TimeSpan.FromSeconds( delay );
			m_Interval = TimeSpan.FromSeconds( interval );
			m_Count = count;
			m_MessageInterval = messageInterval;
		}

		public override string Name{ get{ return m_Name; } }
		public override string NameIt{ get{ return m_NameIt; } }
		public override int Level{ get{ return m_Level; } }

		#region Mondain's Legacy
		public override int RealLevel
		{
			get
			{
				if ( m_Level >= 39 )
					return m_Level - 39;
				else if ( m_Level >= 34 )
					return m_Level - 34;
				else if ( m_Level >= 29 )
					return m_Level - 29;
				else if ( m_Level >= 24 )
					return m_Level - 24;
				else if ( m_Level >= 19 )
					return m_Level - 19;
				else if ( m_Level >= 14 )
					return m_Level - 14;
				else if ( m_Level >= 10 )
					return m_Level - 10;

				return m_Level;
			}
		}

		public override int LabelNumber
		{
			get
			{
				if ( m_Level < 19 )
				{
					if ( m_Level >= 14 )
						return 1072852; // parasitic poison charges: ~1_val~
					else if ( m_Level >= 10 )
						return 1072853; // darkglow poison charges: ~1_val~
				}
				return 1062412 + RealLevel;//m_Level; // ~poison~ poison charges: ~1_val~
			}
		}
		#endregion

		public class PoisonTimer : Timer
		{
			private PoisonImpl m_Poison;
			private Mobile m_Mobile;
			private Mobile m_From;
			private int m_LastDamage;
			private int m_Index;

			public Mobile From{ get{ return m_From; } set{ m_From = value; } }

			public PoisonTimer( Mobile m, PoisonImpl p ) : base( p.m_Delay, p.m_Interval )
			{
				m_From = m;
				m_Mobile = m;
				m_Poison = p;
			}

			protected override void OnTick()
			{
				#region Mondain's Legacy mod
				if( /* (Core.AOS && m_Poison.RealLevel < 4 && TransformationSpellHelper.UnderTransformation( m_Mobile, typeof( VampiricEmbraceSpell ) )) || */
				( /* m_Poison.RealLevel */ m_Poison.Level < 3 && OrangePetals.UnderEffect( m_Mobile ) ) ||
				AnimalForm.UnderTransformation( m_Mobile, typeof( Unicorn ) ) ||
				CurePoisonSpell.UnderEffect( m_Mobile ) ) // mod by Dies Irae
				#endregion
				{
					if ( m_Mobile.CurePoison( m_Mobile ) )
					{
						m_Mobile.LocalOverheadMessage( MessageType.Emote, 0x3F, true,
							(m_Mobile.Language == "ITA" ? "*Resisti agli effetti del veleno*"
							: "*You feel yourself resisting the effects of the poison*") );

						m_Mobile.NonlocalOverheadMessage( MessageType.Emote, 0x3F, true,
							(m_Mobile.Language == "ITA" ? String.Format( "*{0} sembra resistente al veleno*", m_Mobile.Name )
							: String.Format( "*{0} seems resistant to the poison*", m_Mobile.Name )) );

						Stop();
						return;
					}
				}

				if ( m_Index++ == m_Poison.m_Count )
				{
					m_Mobile.SendLocalizedMessage( 502136 ); // The poison seems to have worn off.
					m_Mobile.Poison = null;

					if ( m_Poison.Level >= 39 )//Lentezza
						m_Mobile.Slowed = false;
					else if ( m_Poison.Level >= 34 )//Blocco
						m_Mobile.Frozen = false;
					else if ( m_Poison.Level >= 29 )//Paralisi
						m_Mobile.Paralyzed = false;

					Stop();
					return;
				}

				int damage;

				if ( !Core.AOS && m_LastDamage != 0 && Utility.RandomBool() )
				{
					damage = m_LastDamage;
				}
				else
				{
					damage = 1 + (int)(m_Mobile.Hits * m_Poison.m_Scalar);

					if ( m_Poison.Level >= 24 )//Stanchezza
						damage = 1 + (int)(m_Mobile.Stam * m_Poison.m_Scalar);
					else if ( m_Poison.Level >= 19 )//Magia
						damage = 1 + (int)(m_Mobile.Mana * m_Poison.m_Scalar);

					if ( damage < m_Poison.m_Minimum )
						damage = m_Poison.m_Minimum;
					else if ( damage > m_Poison.m_Maximum )
						damage = m_Poison.m_Maximum;

					#region mod by Dies Irae
					if( damage != m_LastDamage && ( m_Index % m_Poison.m_MessageInterval ) == 0 )
						m_Mobile.OnPoisoned( m_From, m_Poison, m_Poison );
					#endregion

					m_LastDamage = damage;
				}

				if ( m_From != null )
					m_From.DoHarmful( m_Mobile, true );

				IHonorTarget honorTarget = m_Mobile as IHonorTarget;
				if ( honorTarget != null && honorTarget.ReceivedHonorContext != null )
					honorTarget.ReceivedHonorContext.OnTargetPoisoned();
					
				#region Mondain's Legacy
				if ( Core.ML )
				{
					if ( m_From != null && m_Mobile != m_From && !m_From.InRange( m_Mobile.Location, 1 ) && m_Poison.m_Level >= 10 && m_Poison.m_Level <=13 ) // darkglow
					{
						m_From.SendLocalizedMessage( 1072850 ); // Darkglow poison increases your damage!
						damage = (int) Math.Floor( damage * 1.1 );
					}
					
					if ( m_From != null && m_Mobile != m_From && m_From.InRange( m_Mobile.Location, 1 ) && m_Poison.m_Level >= 14 && m_Poison.m_Level <= 18 ) // parasitic
					{
						int toHeal = Math.Min( m_From.HitsMax - m_From.Hits, damage );
												
						if ( toHeal > 0 )
						{
							m_From.SendLocalizedMessage( 1060203, toHeal.ToString() ); // You have had ~1_HEALED_AMOUNT~ hit points of damage healed.
							m_From.Heal( toHeal, m_Mobile, false );
						}
					}
				}
				#endregion

				#region Poison Engine [Arlas]
				/*
				* Magia - il pg perde lentamente mana
				* Stanchezza - il pg perde lentamente stamina
				* Paralisi - Paralize (max 1,5 secondi)
				* Blocco - Il pg si blocca nel posto ma può combattere e lanciare inc. (max 5 sec)
				* Lentezza - Swing più lento del normale
				* AOS.Damage( m_Mobile, m_From, damage, 0, 0, 0, 100, 0 );
				*/
				if ( m_Poison.Level >= 39 )//Lentezza
					m_Mobile.SlowSwing( m_Poison.m_Interval );
				else if ( m_Poison.Level >= 34 )//Blocco
					m_Mobile.Freeze( m_Poison.m_Interval );
				else if ( m_Poison.Level >= 29 )//Paralisi
					m_Mobile.Paralyze( m_Poison.m_Interval );
				else if ( m_Poison.Level >= 24 )//Stanchezza
					m_Mobile.DamageStam( damage, m_From );
				else if ( m_Poison.Level >= 19 )//Magia
					m_Mobile.DamageMana( damage, m_From );
				else
					AOS.Damage( m_Mobile, m_From, damage, 0, 0, 0, 100, 0 );
				#endregion

				m_Mobile.PlaySound( 0x246 ); // mod by Dies Irae

				//if ( (m_Index % m_Poison.m_MessageInterval) == 0 )
				//    m_Mobile.OnPoisoned( m_From, m_Poison, m_Poison );
			}
		}

		public override Timer ConstructTimer( Mobile m )
		{
			return new PoisonTimer( m, this );
		}

		#region mod by Dies Irae
		public override int GetResistDifficulty()
		{
			switch( RealLevel )//edit by Arlas
			{
				case 0: return 15;
				case 1: return 30;
				case 2: return 45;
				case 3: return 63;
				case 4: return 80;
				default: return 80;
			}
		}
		#endregion
	}
}