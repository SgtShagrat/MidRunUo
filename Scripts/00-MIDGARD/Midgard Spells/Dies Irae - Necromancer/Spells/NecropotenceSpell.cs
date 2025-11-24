/***************************************************************************
 *								  NecropotenceSpell.cs
 *									-------------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 *			Questo maleficio consente al necromante di avere al suo servizio
 * 			un'orda non morta.
 * 			Ogni non morto ha un costo di degenerazione che influisce sulla 
 * 			vita del necromante proporzionale alla sua potenza.
 * 
 * 			I non morti evocati durano SpiritSpeak secondi.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.Classes;

using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public class NecropotenceSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Necropotence", "Kal Corp Xen Bal",
			-1,
			9002,
			false,
			Reagent.PigIron,
			Reagent.BatWing
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo(
			typeof( NecropotenceSpell ),
			"This curse summons the Necromancers damned servants.",
			"Questo potente maleficio evoca i malvagi servitori del Necromante.",
			0x5006
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override int RequiredMana{get { return 20; }}
		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 4.0 ); }}
		public override double DelayOfReuse{get { return 5.0; }}
		public override double RequiredSkill{get { return 70.0; }}
		public override bool BlocksMovement{get { return true; }}

		public NecropotenceSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			//BaseCreature check = (BaseCreature)m_Table[ Caster ];

			//if( check != null && !check.Deleted && check.SummonMaster == Caster )
			//{
			//	Caster.SendLocalizedMessage( 1064848 ); // You already have a summoned creature.
			//	return false;
			//}
			if ( Caster.Followers >= Caster.FollowersMax )
			{
				Caster.SendMessage( Caster.Language == "ITA" ? "Non puoi evocare così tanti servitori!" : "You cannot summon so much minions!" );
				return false;
			}

			return base.CheckCast();
		}

		public override void OnCast()
		{
			if( CheckSequence() )
			{
				//if( Caster.CanBeginAction( typeof( NecropotenceSpell ) ) )
				//{
					Caster.CloseGump( typeof( NecropotenceGump ) );
					Caster.SendGump( new NecropotenceGump( Caster, this ) );
				//}
				//else
				//{
				//	Caster.SendMessage( "You cannot summon another damned soul!" );
				//}
			}

			FinishSequence();
		}

		private static readonly NecropotenceEntry[] m_Entries = new NecropotenceEntry[]
		{
			new NecropotenceEntry( typeof( SkeletonBuffer ), "Buffer Minion" ),
			new NecropotenceEntry( typeof( SkeletonDamager ), "Damager Minion"),
			new NecropotenceEntry( typeof( SkeletonDebuffer ), "Debuffer Minion"),
			new NecropotenceEntry( typeof( SkeletonHealer ), "Healer Minion"),
			new NecropotenceEntry( typeof( SkeletonSummoner ), "Summoner Minion")
		};

		public static NecropotenceEntry[] Entries { get { return m_Entries; } }

		public class NecropotenceEntry
		{
			public Type Type { get; private set; }
			public object Name { get; private set; }

			public NecropotenceEntry( Type type, object name)
			{
				Type = type;
				Name = name;
			}
		}

		private static readonly Dictionary<Mobile, List<Mobile>> m_Table = new Dictionary<Mobile, List<Mobile>>();

		public static void Unregister( Mobile master, Mobile summoned )
		{
			if( master == null )
				return;

			List<Mobile> list;
			m_Table.TryGetValue( master, out list );

			if( list == null )
				return;

			list.Remove( summoned );

			if( list.Count == 0 )
				m_Table.Remove( master );
		}

		public static List<Mobile> GetNecropotenceArmy( Mobile necromancer )
		{
			if( necromancer == null )
				return null;

			List<Mobile> list;
			m_Table.TryGetValue( necromancer, out list );

			return list;
		}

		private static void Register( Mobile master, Mobile summoned )
		{
			if( master == null )
				return;

			List<Mobile> list;
			m_Table.TryGetValue( master, out list );

			if( list == null )
				m_Table[ master ] = list = new List<Mobile>();

			for( int i = list.Count - 1; i >= 0; --i )
			{
				if( i >= list.Count )
					continue;

				Mobile mob = list[ i ];

				if( mob.Deleted )
					list.RemoveAt( i-- );
			}

			list.Add( summoned );

			//Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( Summoned_Damage ), summoned );
		}

		private static void SummonDelay_Callback( object state )
		{
			object[] states = (object[])state;

			Mobile caster = (Mobile)states[ 0 ];
			Type type = (Type)states[ 1 ];

			// Lo spell dura da a a 121 secondi
			int level = RPGSpellsSystem.GetPowerLevel( caster, typeof( NecropotenceSpell ) );
			TimeSpan duration = TimeSpan.FromSeconds( caster.Skills[ SkillName.SpiritSpeak ].Base * 10 + level * 1000 );

			BaseCreature summoned = null;

			try
			{
				summoned = Activator.CreateInstance( type ) as BaseCreature;
			}
			catch( Exception ex )
			{
				Console.WriteLine( ex.ToString() );
			}

			if( summoned == null )
				return;

			summoned.Tamable = false;
			summoned.ControlSlots = 0;
			summoned.Kills = 0;
			summoned.Fame = caster.Fame;

			Effects.PlaySound( caster.Location, caster.Map, summoned.GetAngerSound() );
			BaseCreature.Summon( summoned, true, caster, caster.Location, 0x28, duration );

			summoned.ControlOrder = OrderType.Guard;

			Register( caster, summoned );
		}

		public class NecropotenceGump : Gump
		{
			private readonly Mobile m_From;
			private readonly NecropotenceSpell m_Spell;

			private const int EnabledColor16 = 0x0F20;
			private const int DisabledColor16 = 0x262A;

			private const int EnabledColor32 = 0x18CD00;
			private const int DisabledColor32 = 0x4A8B52;

			public NecropotenceGump( Mobile from, NecropotenceSpell spell ) : base( 200, 100 )
			{
				m_From = from;
				m_Spell = spell;

				AddPage( 0 );

				AddBackground( 10, 10, 250, 53 + ( m_Entries.Length * 21 ), 9270 );
				AddAlphaRegion( 20, 20, 230, 33 + ( m_Entries.Length * 21 ) );

				AddHtmlLocalized( 30, 26, 200, 20, 1060147, EnabledColor16, false, false ); // Choose thy creature to summon...

				//double magery = from.Skills[ SkillName.Magery ].Base;

				for( int i = 0; i < m_Entries.Length; ++i )
				{
					object name = m_Entries[ i ].Name;

					bool enabled = true;//( magery >= m_Entries[ i ].ReqMagery );

					int level = RPGSpellsSystem.GetPowerLevel( m_From, typeof( NecropotenceSpell ) );

					if ( level == 1 )
					{
						AddButton( 27, 53 + ( i * 21 ), 2225, 2511, i + 1, GumpButtonType.Reply, 0 );
						AddHtml( 50 , 51 + ( i * 21 ), 150, 20, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", enabled ? EnabledColor32 : DisabledColor32, name ), false, false );
					}
					else if ( level == 2 )
					{
						AddButton( 27, 53 + ( i * 21 ), 2225, 2511, i + 1, GumpButtonType.Reply, 0 );
						AddButton( 49, 53 + ( i * 21 ), 2226, 2511, i + 10, GumpButtonType.Reply, 0 );
						AddHtml( 71 , 51 + ( i * 21 ), 150, 20, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", enabled ? EnabledColor32 : DisabledColor32, name ), false, false );
					}
					else if ( level == 3 )
					{
						AddButton( 27, 53 + ( i * 21 ), 2225, 2511, i + 1, GumpButtonType.Reply, 0 );
						AddButton( 49, 53 + ( i * 21 ), 2226, 2511, i + 11, GumpButtonType.Reply, 0 );
						AddButton( 71, 53 + ( i * 21 ), 2227, 2511, i + 21, GumpButtonType.Reply, 0 );
						AddHtml( 94 , 51 + ( i * 21 ), 150, 20, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", enabled ? EnabledColor32 : DisabledColor32, name ), false, false );
					}
					else if ( level == 4 )
					{
						AddButton( 27, 53 + ( i * 21 ), 2225, 2511, i + 1, GumpButtonType.Reply, 0 );
						AddButton( 49, 53 + ( i * 21 ), 2226, 2511, i + 11, GumpButtonType.Reply, 0 );
						AddButton( 71, 53 + ( i * 21 ), 2227, 2511, i + 21, GumpButtonType.Reply, 0 );
						AddButton( 94, 53 + ( i * 21 ), 2228, 2511, i + 31, GumpButtonType.Reply, 0 );
						AddHtml( 118 , 51 + ( i * 21 ), 150, 20, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", enabled ? EnabledColor32 : DisabledColor32, name ), false, false );
					}
					else
					{
						AddButton( 27, 53 + ( i * 21 ), 2225, 2511, i + 1, GumpButtonType.Reply, 0 );
						AddButton( 49, 53 + ( i * 21 ), 2226, 2511, i + 11, GumpButtonType.Reply, 0 );
						AddButton( 71, 53 + ( i * 21 ), 2227, 2511, i + 21, GumpButtonType.Reply, 0 );
						AddButton( 94, 53 + ( i * 21 ), 2228, 2511, i + 31, GumpButtonType.Reply, 0 );
						AddButton( 118, 53 + ( i * 21 ), 2229, 2511, i + 41, GumpButtonType.Reply, 0 );
						AddHtml( 140 , 51 + ( i * 21 ), 150, 20, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", enabled ? EnabledColor32 : DisabledColor32, name ), false, false );
					}
				}
			}

			private static void Scale( BaseCreature bc, Mobile from, double scalar, int level )
			{
				//from.SendMessage( level.ToString() );
				//from.SendMessage( scalar.ToString() );

				if( bc.HitsMaxSeed >= 0 )
					bc.HitsMaxSeed = (int)( level * scalar / 2 );

				bc.RawStr = (int)( level * scalar / 2 );
				bc.RawInt = (int)( level * scalar / 2 );
				bc.RawDex = (int)( level * scalar / 8 );

				bc.Hits = bc.HitsMax;
				bc.Mana = bc.ManaMax;
				bc.Stam = bc.StamMax;
				if ( level > 3 )
				{
					//bc.Name = "a skeletal dragon";
					bc.Body = 104;
				}
				else if ( level == 3 )
				{
					//bc.Name = "a lich lord";
					bc.Body = 79;
				}

				//bc.Skills.MagicResist = m_From.Skills.MagicResist;
				bc.Karma = from.Karma;
				bc.Kills = from.Kills;
				bc.Fame = from.Fame;
				bc.Tamable = false;
				bc.ControlSlots = level;
				if (level == 1 && bc is SkeletonSummoner)
					bc.ControlSlots = 2;

				bc.ControlOrder = OrderType.Guard;

				for( int i = 0; i < bc.Skills.Length; i++ )
				{
					Skill skill = bc.Skills[ i ];

					if( skill.Base > 0.0 )
						skill.Base = scalar / 2;
					if ( skill.Base < 80.0 )
						skill.Base += level;
				}

				//bc.PassiveSpeed /= level;
				bc.ActiveSpeed = 0.225 - 0.025*level;
				bc.PassiveSpeed = bc.ActiveSpeed*2;

				//bc.DamageMin = (int)( bc.DamageMin * scalar );
				//bc.DamageMax = (int)( bc.DamageMax * scalar );
			}

			private static void DoFlames( Mobile master, int level )
			{
				for( int i = 0; i < 10; i++ )
				{
					Point3D p = new Point3D( master.X + Utility.RandomMinMax( -level, level ), master.Y + Utility.RandomMinMax( -level, level ), master.Z + 20 );

					if( master.Map != Map.Internal )
					{
						Effects.SendLocationEffect( p, master.Map, 0x3709, 30, 10, Utility.RandomRedHue(), 0 );
						master.PlaySound( 0x208 );
					}
				}
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				int index = info.ButtonID - 1;
				int lvl = 1;
				if (index >= 10 )
					lvl = 2;
				if (index >= 20 )
					lvl = 3;
				if (index >= 30 )
					lvl = 4;
				if (index >= 40 )
					lvl = 5;

				if( index >= 0 )// && index < m_Entries.Length )
				{
					NecropotenceEntry entry = m_Entries[ index % 10 ];

					if( entry.Type == null )
					{
						m_From.SendLocalizedMessage( 1064847 ); // That Creature has not yet been defined.

						m_From.CloseGump( typeof( NecropotenceGump ) );
						m_From.SendGump( new NecropotenceGump( m_From, m_Spell ) );
					}
					else
					{
						if ( m_From.Followers + lvl > m_From.FollowersMax )
						{
							m_From.SendMessage( m_From.Language == "ITA" ? "Non puoi sostenere tanto potere!" : "You cannot substain so much power!" );
						}
						else
						{
							int level = RPGSpellsSystem.GetPowerLevel( m_From, typeof( NecropotenceSpell ) );
							TimeSpan duration = TimeSpan.FromSeconds( m_From.Skills[ SkillName.SpiritSpeak ].Value * 10 + level * 1000 );
							double skill = m_From.Skills[ SkillName.Necromancy ].Value + m_From.Skills[ SkillName.SpiritSpeak ].Value;

							try
							{
								BaseCreature bc = (BaseCreature)Activator.CreateInstance( entry.Type );
								Scale( bc, m_From, skill, lvl );

								Effects.PlaySound( m_From.Location, m_From.Map, bc.GetAngerSound() );

								if( BaseCreature.Summon( bc, m_From, m_From.Location, -1, duration ) )
								{
									m_From.FixedParticles( 0x3728, 1, 10, 9910, EffectLayer.Head );
									//bc.PlaySound( bc.GetIdleSound() );
									Register( m_From, bc );
									//Table[ m_From ] = bc;
								}
							}
							catch( Exception ex )
							{
								Console.WriteLine( ex.ToString() );
							}
							//m_From.BeginAction( typeof( NecropotenceSpell ) );

							DoFlames( m_From, 3 );
							//Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( SummonDelay_Callback ), new object[] { m_From, entry.Type } );
							//Timer.DelayCall( m_Spell.GetDelayOfReuse(), new TimerStateCallback( ReleaseNecropotenceLock ), m_Spell.Caster );
						}
					}
				}
				else
				{
					m_From.SendLocalizedMessage( 1061825 ); // You decide not to summon a Creature.
				}
			}
		}

		private static void ReleaseNecropotenceLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( NecropotenceSpell ) );
		}

		public static void Initialize()
		{
			EventSink.Speech += new SpeechEventHandler( EventSink_DisbandNecroArmy );
		}

		public static void EventSink_DisbandNecroArmy( SpeechEventArgs args )
		{
			Mobile caster = args.Mobile;
			if( caster == null || !caster.Player || !caster.Alive )
				return;

			if( ClassSystem.IsNecromancer( caster ) && Insensitive.Compare( args.Speech, "My army is disbanded" ) == 0 )
			{
				List<Mobile> toDisband = caster.AllFollowers;//GetNecropotenceArmy( caster );
				if( toDisband != null )
				{
					try
					{
						for( int i = 0; i < toDisband.Count; i++ )
						{
							if( i < toDisband.Count && toDisband[ i ] != null && toDisband[ i ] is BaseNecroFamiliar )
							{
								//Unregister( caster, toDisband[ i ] );
								toDisband[ i ].Kill();
							}
						}
					}
					catch( Exception ex )
					{
						Console.WriteLine( ex.ToString() );
					}
				}
			}
		}
	}
}