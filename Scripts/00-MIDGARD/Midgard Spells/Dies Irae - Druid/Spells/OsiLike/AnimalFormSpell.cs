/***************************************************************************
 *							   AnimalFormSpell.cs
 *
 *   begin				: 29 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;

using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

namespace Midgard.Engines.SpellSystem
{
	public class AnimalFormSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Animal Form", "Lore Helma Kes",
			224,
			9011,
			Reagent.Garlic,
			Reagent.PetrifiedWood
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( AnimalFormSpell ),
			"This spell morph the druid into a natural being.",
			"Questo potere tramuta il druido in un animale.",
			0x4db
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler( OnLogin );
		}

		public static void OnLogin( LoginEventArgs e )
		{
			AnimalFormContext context = GetContext( e.Mobile );

			if( context != null && context.SpeedBoost )
				e.Mobile.Send( SpeedControl.MountSpeed );
		}

		public override SpellCircle Circle{get { return SpellCircle.Second; }}

		public override bool BlockedByAnimalForm{get { return false; }}

		public AnimalFormSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if( !BaseMount.CheckMountAllowed( Caster, true ) )
				return false;

			if( !Caster.CanBeginAction( typeof( PolymorphSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
				return false;
			}
			else if( TransformationSpellHelper.UnderTransformation( Caster ) )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Non puoi diventare un animale sotto questa forma." : "You cannot become an animal while in that form.") );
				return false;
			}

			return base.CheckCast();
		}

		public override void OnBeginCast()
		{
			base.OnBeginCast();

			Caster.FixedEffect( 0x37C4, 10, 14, 4, 3 );
		}

		public override bool CheckFizzle()
		{
			return true;
		}

		public override void OnCast()
		{
			if( !Caster.CanBeginAction( typeof( PolymorphSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
			}
			else if( TransformationSpellHelper.UnderTransformation( Caster ) )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Non puoi diventare un animale sotto questa forma." : "You cannot become an animal while in that form.") );
			}
			else if( !Caster.CanBeginAction( typeof( IncognitoSpell ) ) || ( Caster.IsBodyMod && GetContext( Caster ) == null ) )
			{
				DoFizzle();
			}
			else if( CheckSequence() )
			{
				AnimalFormContext context = GetContext( Caster );

				if( context != null )
				{
					RemoveContext( Caster, context, true );
				}
				else if( Caster is PlayerMobile )
				{
					if( GetLastAnimalForm( Caster ) == -1 || DateTime.Now - Caster.LastMoveTime > Caster.ComputeMovementSpeed( Caster.Direction ) )
					{
						Caster.CloseGump( typeof( AnimalFormGump ) );
						Caster.SendGump( new AnimalFormGump( Caster ) );
					}
					else
					{
						if( Morph( Caster, GetLastAnimalForm( Caster ) ) == MorphResult.Fail )
							DoFizzle();
					}
				}
				else
				{
					if( Morph( Caster, GetLastAnimalForm( Caster ) ) == MorphResult.Fail )
						DoFizzle();
				}
			}

			FinishSequence();
		}

		private static readonly Hashtable m_LastAnimalForms = new Hashtable();

		private static int GetLastAnimalForm( Mobile m )
		{
			if( m_LastAnimalForms.Contains( m ) )
				return (int)m_LastAnimalForms[ m ];

			return -1;
		}

		private enum MorphResult
		{
			Success,
			Fail,
			NoSkill
		}

		private static MorphResult Morph( Mobile m, int entryID )
		{
			if( entryID < 0 || entryID >= Entries.Length )
				return MorphResult.Fail;

			AnimalFormEntry entry = Entries[ entryID ];

			m_LastAnimalForms[ m ] = entryID;

			if( m.Skills.Spellweaving.Value < entry.ReqSkill )
			{
				m.SendMessage( (m.Language == "ITA" ? "Ti serve almeno {0} {1} per farlo." : "You need at least {0} {1} to do that.") , entry.ReqSkill.ToString( "F1" ), SkillName.Spellweaving );
				return MorphResult.NoSkill;
			}

			double d = m.Skills.Spellweaving.Value;

			if( d < entry.ReqSkill + 5 )
			{
				double chance = ( d - entry.ReqSkill ) / 5;

				if( chance < Utility.RandomDouble() )
					return MorphResult.Fail;
			}

			m.CheckSkill( SkillName.Spellweaving, 0.0, 37.5 );

			BaseMount.Dismount( m );

			m.BodyMod = entry.BodyMod;

			if( entry.HueMod > 0 )
				m.HueMod = entry.HueMod;

			if( entry.SpeedBoost )
				m.Send( SpeedControl.MountSpeed );

			foreach( SkillBonusInfo bonusInfo in entry.Skills )
				bonusInfo.ApplyTo( m );

			m.Target = null;

			Timer timer = new AnimalFormTimer( m, entry.BodyMod, entry.HueMod );
			timer.Start();

			AddContext( m, new AnimalFormContext( timer, entry.SpeedBoost, entry.Type, entry.Skills ) );
			return MorphResult.Success;
		}

		private static readonly Hashtable m_Table = new Hashtable();

		public static void AddContext( Mobile m, AnimalFormContext context )
		{
			m_Table[ m ] = context;

			if( context.Type == typeof( GreyWolf ) )
				m.CheckStatTimers();
		}

		public static void RemoveContext( Mobile m, bool resetGraphics )
		{
			AnimalFormContext context = GetContext( m );

			if( context != null )
				RemoveContext( m, context, resetGraphics );
		}

		public static void RemoveContext( Mobile m, AnimalFormContext context, bool resetGraphics )
		{
			m_Table.Remove( m );

			if( context.SpeedBoost )
			{
				if( m.Region is TwistedWealdDesert )
					m.Send( SpeedControl.WalkSpeed );
				else
					m.Send( SpeedControl.Disable );
			}

			foreach( SkillBonusInfo bonusInfo in context.Skills )
			{
				if( bonusInfo != null )
					bonusInfo.RemoveFrom( m );
			}

			if( resetGraphics )
			{
				m.HueMod = -1;
				m.BodyMod = 0;
			}

			context.Timer.Stop();
		}

		public static AnimalFormContext GetContext( Mobile m )
		{
			return ( m_Table[ m ] as AnimalFormContext );
		}

		public static bool UnderTransformation( Mobile m )
		{
			return ( GetContext( m ) != null );
		}

		public static bool UnderTransformation( Mobile m, Type type )
		{
			AnimalFormContext context = GetContext( m );

			return ( context != null && context.Type == type );
		}

		private class AnimalFormEntry
		{
			public Type Type { get; private set; }
			public string Name { get; private set; }
			private int Hue { get; set; }
			public double ReqSkill { get; private set; }
			public int BodyMod { get; private set; }
			public int HueMod { get; private set; }

			public bool SpeedBoost { get; private set; }

			public SkillBonusInfo[] Skills { get; private set; }

			public AnimalFormEntry( Type type, string name, int hue, double reqSkill, int bodyMod, bool speedBoost, params SkillBonusInfo[] skills ) :
				this( type, name, hue, reqSkill, bodyMod, 0, speedBoost, skills )
			{
			}

			public AnimalFormEntry( Type type, string name, int hue, double reqSkill, int bodyMod, int hueMod, bool speedBoost, params SkillBonusInfo[] skills )
			{
				Type = type;
				Name = name;
				Hue = hue;
				ReqSkill = reqSkill;
				BodyMod = bodyMod;
				HueMod = hueMod;
				SpeedBoost = speedBoost;
				Skills = skills;
			}
		}

		public class SkillBonusInfo
		{
			public SkillName Skill { get; set; }
			public double Value { get; set; }

			private DefaultSkillMod m_SkillMod;

			public SkillBonusInfo( SkillName skill, double value )
			{
				Skill = skill;
				Value = value;
			}

			public void ApplyTo( Mobile m )
			{
				if( m.PlayerDebug )
				{
					m.SendMessage( "Debug AnimalFormSpell: applying skill bonus: skill: {0}, baseValue: {1:F1}, bonus {2:F1}.",
						m_SkillMod.Skill, m.Skills[ m_SkillMod.Skill ].Value, m_SkillMod.Value );
				}

				m_SkillMod = new DefaultSkillMod( Skill, true, Value );
				m_SkillMod.ObeyCap = true;
				m.AddSkillMod( m_SkillMod );

				if( m.PlayerDebug )
					m.SendMessage( "Debug AnimalFormSpell: new skill value {0:F1}.", m.Skills[ m_SkillMod.Skill ].Value );
			}

			public void RemoveFrom( Mobile m )
			{
				if( m_SkillMod != null )
					m.RemoveSkillMod( m_SkillMod );
			}
		}

		private static readonly AnimalFormEntry[] Entries = new AnimalFormEntry[]
			{
				// +40% hiding
				new AnimalFormEntry( typeof( Rat ), "rat", 2309, 20.0, 0xEE, false,  new SkillBonusInfo( SkillName.Hiding, 40.0 ) ),
				 // +30% hiding
				new AnimalFormEntry( typeof( Rabbit ), "rabbit", 2309, 20.0, 0xCD, false,  new SkillBonusInfo( SkillName.Hiding, 30.0 ) ), 
				// +30% tracking
				new AnimalFormEntry( typeof( Dog ), "dog", 2309, 40.0, 0xD9, true, new SkillBonusInfo( SkillName.Tracking, 30.0 ) ),
				// +40% hiding and stealth
				 new AnimalFormEntry( typeof( Cat ), "cat", 2309, 40.0, 0xC9, true, new SkillBonusInfo( SkillName.Hiding, 40.0 ), new SkillBonusInfo( SkillName.Stealth, 40.0 )  ),
				// +30% detect hidden
				new AnimalFormEntry( typeof( GiantSerpent ), "giant serpent", 2009, 50.0, 0x15, true, new SkillBonusInfo( SkillName.DetectHidden, 30.0 ) ),

				// +30% anatomy
				new AnimalFormEntry( typeof( BullFrog ), "bullfrog", 2003, 50.0, 0x51, 0x5A3, false, new SkillBonusInfo( SkillName.Anatomy, 30.0 ) ),
				new AnimalFormEntry( typeof( Llama ), "llama", 0, 70.0, 0xDC, true, new SkillBonusInfo( SkillName.Anatomy, 30.0 ) ),
				new AnimalFormEntry( typeof( ForestOstard ), "forest ostard", 2212, 70.0, 0xDA, true, new SkillBonusInfo( SkillName.Anatomy, 30.0 ) ),

				// +50% tracking
				new AnimalFormEntry( typeof( GreyWolf ), "grey wolf", 2309, 82.5, 0x19, true, new SkillBonusInfo( SkillName.Tracking, 50.0 ) ),

				// speed
				//new AnimalFormEntry( typeof( Unicorn ), "unicorn", 0, 100.0, 0x7A, true ), //mod by Magius(CHE): midgard balance
			};

		private class AnimalFormGump : Gump
		{
			private readonly Mobile m_From;
			private readonly AnimalFormEntry[] m_Entries;

			private const int EnabledColor16 = 0x0F20;

			private const int EnabledColor32 = 0x18CD00;
			private const int DisabledColor32 = 0x4A8B52;

			public AnimalFormGump( Mobile from ) : base( 200, 100 )
			{
				m_From = from;
				m_Entries = AnimalFormSpell.Entries;

				AddPage( 0 );

				AddBackground( 10, 10, 250, 53 + ( m_Entries.Length * 21 ), 9270 );
				AddAlphaRegion( 20, 20, 230, 33 + ( m_Entries.Length * 21 ) );

				AddOldHtmlHued( 30, 26, 200, 20, (from.Language == "ITA" ? "Scegli la tua nuova forma" : "Choose thy new form"), EnabledColor16 );

				double weaving = from.Skills[ SkillName.Spellweaving ].Base;

				for( int i = 0; i < m_Entries.Length; ++i )
				{
					bool enabled = ( weaving >= m_Entries[ i ].ReqSkill );

					if( enabled )
						AddButton( 27, 33 + ( i * 21 ), 9702, 9703, i + 1, GumpButtonType.Reply, 0 );

					AddOldHtmlHued( 50, 31 + ( i * 21 ), 150, 20, m_Entries[ i ].Name, enabled ? EnabledColor32 : DisabledColor32 );
				}
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				int index = info.ButtonID - 1;

				if( index >= 0 && index < m_Entries.Length )
				{
					if( Morph( m_From, index ) == MorphResult.Fail )
					{
						m_From.SendMessage( m_From.Language == "ITA" ? "La trasformazione fallisce." : "The morph fails." );//MessageType.Regular, 0x3B2, 502632 ); // The spell fizzles.
						m_From.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );
						m_From.PlaySound( 0x5C );
					}
				}
			}
		}

		public class AnimalFormContext
		{
			public Timer Timer { get; private set; }
			public bool SpeedBoost { get; private set; }
			public Type Type { get; private set; }
			public SkillBonusInfo[] Skills { get; set; }

			public AnimalFormContext(Timer timer, bool speedBoost, Type type, SkillBonusInfo[] skills)
			{
				Timer = timer;
				SpeedBoost = speedBoost;
				Type = type;
				Skills = skills;
			}
		}

		private class AnimalFormTimer : Timer
		{
			private readonly Mobile m_Mobile;
			private readonly int m_Body;
			private readonly int m_Hue;

			public AnimalFormTimer( Mobile from, int body, int hue )
				: base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = from;
				m_Body = body;
				m_Hue = hue;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if( m_Mobile.Deleted || !m_Mobile.Alive || m_Mobile.Body != m_Body || ( m_Hue != 0 && m_Mobile.Hue != m_Hue ) )
				{
					RemoveContext( m_Mobile, true );
					Stop();
				}
			}
		}
	}
}