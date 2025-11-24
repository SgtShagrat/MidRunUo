/***************************************************************************
 *							   BlendWithForrestSpell.cs
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
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;

namespace Midgard.Engines.SpellSystem
{
	public class BlendWithForestSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo
			(
			"Blend With Forest", "Kes Ohm",
			206,
			9002,
			true,
			Reagent.Bloodmoss,
			Reagent.FertileDirt
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( BlendWithForestSpell ),
			"The Druid blends seamlessly with the background, becoming invisible to their foes.",
			"Il druido diventa tuttuno con la foresta che lo circonda.",
			0x4f3
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override SpellCircle Circle{get { return SpellCircle.Fifth; }}

		public override bool BlocksMovement{get { return false; }}

		private const int RareTrees = 1;
		private const double RareTreeChance = 0.05;

		private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();
		private static readonly Dictionary<Mobile, DruidTree> m_TreeTable = new Dictionary<Mobile, DruidTree>();

		public BlendWithForestSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public static bool IsImmune( Mobile m )
		{
			return m_Table.ContainsKey( m );
		}

		public static bool HasTree( Mobile m )
		{
			return m_TreeTable.ContainsKey( m ) && m_TreeTable[ m ] != null;
		}

		private void BeginImmune()
		{
			Timer t;

			if( IsImmune( Caster ) )
			{
				t = m_Table[ Caster ];
				if( t != null )
					t.Stop();

				m_Table.Remove( Caster );
			}

			int mean = (int)( ( Caster.Skills[ CastSkill ].Value + Caster.Skills[ DamageSkill ].Value ) / 2 );
			TimeSpan duration = TimeSpan.FromSeconds( mean + 5 );

			t = new BlessTimer( this, duration );

			if( IsImmune( Caster ) )
				return;

			m_Table.Add( Caster, t );
			t.Start();

			DeleteTree( Caster );

			DruidTree tree = BuildRandomTree( Caster.Location, Caster.Map, Utility.RandomDouble() < RareTreeChance );

			if( m_TreeTable.ContainsKey( Caster ) )
				m_TreeTable.Remove( Caster );

			m_TreeTable.Add( Caster, tree );

			Caster.SendMessage( (Caster.Language == "ITA" ? "Diventi un albero.. senti il potere della Natura!" : "You become a tree... feel the power of Nature!") );
			Caster.PlaySound( 0x19 );
			Caster.FixedParticles( 0x375A, 2, 10, 5027, 0x3D, 2, EffectLayer.Waist );

			Caster.Paralyze( duration );
			Caster.Hidden = true;
			Caster.Squelched = true;
			Caster.Blessed = true;

			PeaceInNature();
		}

		private void PeaceInNature()
		{
			StatMod mod = Caster.GetStatMod( "[Magic] Str Offset" );
			if( mod != null && mod.Offset < 0 )
				Caster.RemoveStatMod( "[Magic] Str Offset" );

			mod = Caster.GetStatMod( "[Magic] Dex Offset" );
			if( mod != null && mod.Offset < 0 )
				Caster.RemoveStatMod( "[Magic] Dex Offset" );

			mod = Caster.GetStatMod( "[Magic] Int Offset" );
			if( mod != null && mod.Offset < 0 )
				Caster.RemoveStatMod( "[Magic] Int Offset" );

			EvilOmenSpell.CheckEffect( Caster );
			StrangleSpell.RemoveCurse( Caster );
			CorpseSkinSpell.RemoveCurse( Caster );
			CurseSpell.RemoveEffect( Caster );
			MortalStrike.EndWound( Caster );
			BloodOathSpell.RemoveCurse( Caster );
			MindRotSpell.ClearMindRotScalar( Caster );

			//DarkOmenSpell.CheckEffect( Caster );
			ChokingSpell.RemoveCurse( Caster );
			BloodConjunctionSpell.RemoveCurse( Caster );
			LobotomySpell.ClearMindRotScalar( Caster );

			BuffInfo.RemoveBuff( Caster, BuffIcon.Clumsy );
			BuffInfo.RemoveBuff( Caster, BuffIcon.FeebleMind );
			BuffInfo.RemoveBuff( Caster, BuffIcon.Weaken );
			BuffInfo.RemoveBuff( Caster, BuffIcon.MassCurse );

			Caster.SendLocalizedMessage( 1072408 ); // Any curses on you have been lifted

			BleedAttack.EndBleed( Caster, false );
			MortalStrike.EndWound( Caster );

			BuffInfo.RemoveBuff( Caster, BuffIcon.Bleed );
			BuffInfo.RemoveBuff( Caster, BuffIcon.MortalStrike );

			Caster.SendLocalizedMessage( 1072405 ); // Your lasting damage effects have been removed!

			Caster.CurePoison( Caster );
		}

		private static void DeleteTree( Mobile m )
		{
			if( HasTree( m ) )
			{
				DruidTree tree = m_TreeTable[ m ];
				if( tree != null && !tree.Deleted )
					tree.Delete();
			}
		}

		public static void EndImmune( Mobile m )
		{
			Timer t = m_Table[ m ];
			if( t != null )
				t.Stop();

			m_Table.Remove( m );

			DeleteTree( m );

			if( m.Hidden )
				m.RevealingAction();
			if( m.Paralyzed )
				m.Paralyzed = false;
			if( m.Frozen )
				m.Frozen = false;
			if( m.Squelched )
				m.Squelched = false;
			if( m.Blessed )
				m.Blessed = false;

			m.SendMessage( (m.Language == "ITA" ? "Devi camminare di nuovo in questo mondo!" : "Thou have to walk again on this World!") );

			m.InvalidateProperties();
			m.Delta( MobileDelta.Noto );
		}

		public override void OnCast()
		{
			if( Caster.Mounted )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Non puoi diventare un albero montando un animale." :"Thou cannot become a tree while mounted.") );
				FinishSequence();
				return;
			}

			if( CheckSequence() )
				BeginImmune();

			FinishSequence();
		}

		private DruidTree BuildRandomTree( Point3D p, Map map, bool rare )
		{
			int from = ( rare ) ? ( m_Trees.Length - RareTrees ) : 0;
			int to = ( rare ) ? ( m_Trees.Length - 1 ) : m_Trees.Length - RareTrees - 1;
			int indexTree = Utility.RandomMinMax( from, to );

			int bust = Utility.RandomList( ( m_Trees[ indexTree ] ).BustIDs );
			int leaves = Utility.RandomList( ( m_Trees[ indexTree ] ).LeavesIDs );
			DruidTree tree = new DruidTree( bust, leaves );
			tree.MoveToWorld( p, map );

			return tree;
		}

		public static void Initialize()
		{
			EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
		}

		public static void EventSink_Speech( SpeechEventArgs args )
		{
			Mobile caster = args.Mobile;
			if( caster != null && caster.Player && caster.Alive && IsImmune( caster ) && Insensitive.Compare( args.Speech, "I am free" ) == 0 )
				EndImmune( caster );
		}

		internal class TreeEntry
		{
			public int[] BustIDs { get; set; }

			public int[] LeavesIDs { get; set; }

			public TreeEntry( int[] bustIDs, int[] leavesIDs )
			{
				BustIDs = bustIDs;
				LeavesIDs = leavesIDs;
			}
		}

		#region m_Trees
		private TreeEntry[] m_Trees = new TreeEntry[]
		{
			// 14 Commons
			new TreeEntry( new int[] { 0xCCD }, new int[] { 0xCCE, 0xCCF } ),
			new TreeEntry( new int[] { 0xCD0 }, new int[] { 0xCD1, 0xCD2 } ),
			new TreeEntry( new int[] { 0xCD3 }, new int[] { 0xCD4, 0xCD5 } ),
			new TreeEntry( new int[] { 0xCD6 }, new int[] { 0xCD7 } ),
			new TreeEntry( new int[] { 0xCD8 }, new int[] { 0xCD9 } ),
			new TreeEntry( new int[] { 0xCDA }, new int[] { 0xCDB, 0xCDC } ),
			new TreeEntry( new int[] { 0xCDD }, new int[] { 0xCDE, 0xCDF } ),
			new TreeEntry( new int[] { 0xCE0 }, new int[] { 0xCE1, 0xCE2 } ),
			new TreeEntry( new int[] { 0xCE3 }, new int[] { 0xCE4, 0xCE5 } ),
			new TreeEntry( new int[] { 0xCE6 }, new int[] { 0xCE7, 0xCE8 } ),
			new TreeEntry( new int[] { 0xCF8 }, new int[] { 0xCF9, 0xCFA } ),
			new TreeEntry( new int[] { 0xCFB }, new int[] { 0xCFC, 0xCFD } ),
			new TreeEntry( new int[] { 0xCFE }, new int[] { 0xCFF, 0xD00 } ),
			new TreeEntry( new int[] { 0xD01 }, new int[] { 0xD02, 0xD03 } ),
			// 1 Rare
			new TreeEntry( new int[] { 0x26ED, 0x26EE }, new int[] { 0x26EF, 0x26F0, 0x26F1, 0x26F2, 0x26F3 } )
		};
		#endregion

		internal class BlessTimer : Timer
		{
			private readonly BlendWithForestSpell m_Owner;
			private DateTime m_Expiration;
			private int m_Elapsed;

			public BlessTimer( BlendWithForestSpell owner, TimeSpan duration )
				: base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Owner = owner;
				m_Expiration = DateTime.Now + duration;
				m_Elapsed = 0;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if( DateTime.Now > m_Expiration )
				{
					EndImmune( m_Owner.Caster );
				}
				else
				{
					Mobile m = m_Owner.Caster;
					if( m == null )
					{
						Stop();
						return;
					}

					m_Elapsed++;

					int remains = (int)( m_Expiration - DateTime.Now ).TotalSeconds;

					if( remains % 10 == 0 )
					{
						m.SendMessage( (m.Language == "ITA" ? "{0} secondi rimasti alla fine." : "{0} seconds remains until commune end."), remains );
					}

					m.Mana += 1;
					m.Hits += 1;
					m.Stam += 1;
				}
			}
		}
	}
}