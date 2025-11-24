/***************************************************************************
 *							   DruidFamiliarSpell.cs
 *							-------------------
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Nuovo sistema per il summon dei familiari dei druidi.
 * 			Permette di scegliere il tipo di familiare da evocare.
 * 			La lista di familiari e' la seguente con il rispettivo vaore di 
 * 			AnimLore necessaria al summon:
 * 
 * 			SkitteringHopperFamiliar 		30.0
 * 			PixieFamiliar 					40.0
 * 			FireflyFamiliar  				50.0
 * 			EagleFamiliar 					60.0
 * 			QuagmireFamiliar 				80.0
 * 			SummonedTreefellow 				100.0
 * 			DryadFamiliar 					115.0
 * 
 * 			La durata dello spell e' (Lore + Taming)*0.5 secondi.
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
	public class DruidFamiliarSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo
			(
			"Summon Familiar", "Lore Sec En Sepa Ohm",
			203,
			9031,
			Reagent.Bloodmoss,
			Reagent.DestroyingAngel
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( DruidFamiliarSpell ),
			"Summon Familiar",
			"Summon a familiar which aids the druid in troubles.",
			"Evoca un famiglio che combatte a fianco del Druido.",
			"Evoca un summon tra: skittering hopper, pixie, firefly, eagle, quagmire, treefellow e dryad. La durata è ((2+PL) * skill).",
			0x4f2
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override SpellCircle Circle{get { return SpellCircle.Fifth; }}
		public override double RequiredSkill{get { return 25.0; }}
		public DruidFamiliarSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static readonly Dictionary<Mobile, BaseCreature> m_Table = new Dictionary<Mobile, BaseCreature>();

		public static Dictionary<Mobile, BaseCreature> Table{get { return m_Table; }}

		public static bool HasFamiliar( Mobile from )
		{
			if( m_Table == null )
				return false;

			if( !m_Table.ContainsKey( from ) )
				return false;

			return m_Table[ from ] != null && !m_Table[ from ].Deleted;
		}

		public override bool CheckCast()
		{
			if( HasFamiliar( Caster ) )
			{
				Caster.SendLocalizedMessage( 1061605 ); // You already have a familiar.
				return false;
			}

			return base.CheckCast();
		}

		public override void OnCast()
		{
			if( CheckSequence() )
			{
				Caster.CloseGump( typeof( DruidFamiliarGump ) );
				Caster.SendGump( new DruidFamiliarGump( Caster, this ) );
			}

			FinishSequence();
		}

		public override TimeSpan GetCastDelay(){return TimeSpan.FromTicks( base.GetCastDelay().Ticks * 5 );}

		public override bool BlocksMovement{get { return true; }}

		private static readonly DruidFamiliarEntry[] m_Entries = new DruidFamiliarEntry[]
		{
			new DruidFamiliarEntry( typeof( SkitteringHopperFamiliar ), 1065510, 25.0 ), // Skittering Hopper
			new DruidFamiliarEntry( typeof( PixieFamiliar ), 1065511, 40.0 ), // Pixie
			new DruidFamiliarEntry( typeof( FireflyFamiliar ), 1065512, 60.0 ), // Firefly
			new DruidFamiliarEntry( typeof( EagleFamiliar ), 1065506, 70.0 ), // Spirit Eagle
			new DruidFamiliarEntry( typeof( QuagmireFamiliar ), 1065507, 80.0 ), // Quagmire
			new DruidFamiliarEntry( typeof( SummonedTreefellow ), 1065508, 90.0 ), // Treefellow
			new DruidFamiliarEntry( typeof( DryadFamiliar ), 1065509, 100.0 ) // Dryad
		};

		private class DruidFamiliarEntry
		{
			public Type Type { get; private set; }
			public object Name { get; private set; }
			public double ReqAnimalLore { get; private set; }
			public double ReqAnimalTaming { get; private set; }

			public DruidFamiliarEntry( Type type, object name, double reqAnimalLore ) :
				this( type, name, reqAnimalLore, reqAnimalLore )
			{
			}

			private DruidFamiliarEntry( Type type, object name, double reqAnimalLore, double reqAnimalTaming )
			{
				Type = type;
				Name = name;
				ReqAnimalLore = reqAnimalLore;
				ReqAnimalTaming = reqAnimalTaming;
			}
		}

		private class DruidFamiliarGump : Gump
		{
			private readonly Mobile m_From;

			private readonly DruidFamiliarSpell m_Spell;

			private const int EnabledColor16 = 0x0F20;
			private const int DisabledColor16 = 0x262A;

			private const int EnabledColor32 = 0x18CD00;
			private const int DisabledColor32 = 0x4A8B52;

			public DruidFamiliarGump( Mobile from, DruidFamiliarSpell spell ) : base( 200, 100 )
			{
				m_From = from;
				m_Spell = spell;

				AddPage( 0 );

				AddBackground( 10, 10, 250, 53 + ( m_Entries.Length * 21 ), 9270 );
				AddAlphaRegion( 20, 20, 230, 33 + ( m_Entries.Length * 21 ) );

				AddHtmlLocalized( 30, 26, 200, 20, 1060147, EnabledColor16, false, false ); // Choose thy familiar to summon...

				double lore = from.Skills[ SkillName.AnimalLore ].Base;
				double taming = from.Skills[ SkillName.Spellweaving ].Base;

				for( int i = 0; i < m_Entries.Length; ++i )
				{
					object name = m_Entries[ i ].Name;

					bool enabled = ( lore >= m_Entries[ i ].ReqAnimalLore && taming >= m_Entries[ i ].ReqAnimalTaming );

					if( enabled )
						AddButton( 27, 53 + ( i * 21 ), 9702, 9703, i + 1, GumpButtonType.Reply, 0 );

					if( name is int )
						AddHtmlLocalized( 50, 51 + ( i * 21 ), 150, 20, (int)name, enabled ? EnabledColor16 : DisabledColor16, false, false );
					else if( name is string )
						AddHtml( 50, 51 + ( i * 21 ), 150, 20, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", enabled ? EnabledColor32 : DisabledColor32, name ), false, false );
				}
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				int index = info.ButtonID - 1;

				if( index >= 0 && index < m_Entries.Length )
				{
					DruidFamiliarEntry entry = m_Entries[ index ];

					double lore = m_From.Skills[ SkillName.AnimalLore ].Base;
					double taming = m_From.Skills[ SkillName.Spellweaving ].Base;

					if( HasFamiliar( m_From ) )
					{
						m_From.SendLocalizedMessage( 1065504 ); // You already have a familiar.
					}
					else if( lore < entry.ReqAnimalLore || taming < entry.ReqAnimalTaming )
					{
						m_From.SendLocalizedMessage( 1061606, String.Format( "{0:F1}\t{1:F1}", entry.ReqAnimalLore, entry.ReqAnimalTaming ) ); // That familiar requires ~1_ANIMLORE~ AnimalLore and ~2_TAMING~ AnimalTaming skill to be summoned.
						m_From.CloseGump( typeof( DruidFamiliarGump ) );
						m_From.SendGump( new DruidFamiliarGump( m_From, m_Spell ) );
					}
					else if( entry.Type == null )
					{
						m_From.SendLocalizedMessage( 1064847 ); // That Creature has not yet been defined.

						m_From.CloseGump( typeof( DruidFamiliarGump ) );
						m_From.SendGump( new DruidFamiliarGump( m_From, m_Spell ) );
					}
					else
					{
						TimeSpan duration = TimeSpan.FromSeconds( ( 2.0 + m_Spell.GetPowerLevel() ) * m_From.Skills[ m_Spell.CastSkill ].Value );

						try
						{
							BaseCreature bc = (BaseCreature)Activator.CreateInstance( entry.Type );

							bc.Skills.MagicResist = m_From.Skills.MagicResist;
							bc.Karma = m_From.Karma;

							if( BaseCreature.Summon( bc, true, m_From, m_From.Location, -1, duration ) )
							{
								if( m_From.PlayerDebug )
								{
									m_From.SendMessage( "Debug DruidFamiliar: summoned: {0} - commandable {1} - controlled {2} - controller {3}.",
										bc.GetType().Name, bc.Commandable, bc.Controlled, bc.ControlMaster.Name ?? "" );
								}

								m_From.FixedParticles( 0x3728, 1, 10, 9910, EffectLayer.Head );
								bc.PlaySound( bc.GetIdleSound() );
								Table[ m_From ] = bc;
							}
						}
						catch( Exception ex )
						{
							Console.WriteLine( ex.ToString() );
						}
					}
				}
				else
				{
					m_From.SendLocalizedMessage( 1061825 ); // You decide not to summon a familiar.
				}
			}
		}

		public static void Unregister( Mobile master )
		{
			if( master == null || !m_Table.ContainsKey( master ) )
				return;

			m_Table.Remove( master );
		}

		public static void Initialize()
		{
			EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
		}

		public static void EventSink_Speech( SpeechEventArgs args )
		{
			Mobile caster = args.Mobile;
			if( caster == null || !caster.Player || !caster.Alive )
				return;

			if( ClassSystem.IsDruid( caster ) && Insensitive.Compare( args.Speech, "Free my friend" ) == 0 )
			{
				if( HasFamiliar( caster ) )
				{
					BaseCreature mount = Table[ caster ];
					if( mount != null && !mount.Deleted )
						mount.Kill();

					Unregister( caster );
				}

				caster.SendMessage( caster.Language == "ITA" ? "Il tuo amico ti ha lasciato..." : "Your friend has gone..." );
			}
		}
	}
}