/***************************************************************************
 *							   EnchantedGroveSpell.cs
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
using Server.Targeting;
using Server.Mobiles;
using Server.Engines.PartySystem;

namespace Midgard.Engines.SpellSystem
{
	public class EnchantedGroveSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo
			(
			"Enchanted Grove", "En Ante Ohm Sepa",
			266,
			9040,
			true,
			Reagent.MandrakeRoot,
			Reagent.PetrifiedWood,
			Reagent.SpringWater
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( EnchantedGroveSpell ),
			"Causes a grove of magical trees to grow.",
			"Crea un boschetto con proprietà straordinarie."+
			"Durata (10 + 10*FL).",
			0x4dc
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override SpellCircle Circle{get { return SpellCircle.Seventh; }}
		//public override double DelayOfReuse{get { return 30.0; }}

		public EnchantedGroveSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if( Caster.CanBeginAction( typeof( EnchantedGroveSpell ) ) )
			{
				Caster.SendMessage( Caster.Language == "ITA" ? "Scegli il punto in cui invocare la sacra pietra." : "Choose where you want to get the Sacred Stone." );
				Caster.Target = new InternalTarget( this );
			}
			else
				Caster.SendMessage( Caster.Language == "ITA" ? "Stai usando troppo i poteri della natura." : "You are using too much nature powers." );
		}

		private class InternalTarget : Target
		{
			private readonly EnchantedGroveSpell m_Spell;

			public InternalTarget( EnchantedGroveSpell spell ) : base( 12, true, TargetFlags.None )
			{
				m_Spell = spell;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if( o is IPoint3D )
					m_Spell.Target( (IPoint3D)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Spell.FinishSequence();
			}
		}

		public void Target( IPoint3D p )
		{
			if( Caster.CanSee( p ) )
			{
				if ( CheckSequence() )
				{
					if( MidgardSpellHelper.CheckBlockField( p, Caster ) && SpellHelper.CheckTown( p, Caster ) )
					{
						Point3D loc = new Point3D( p.X, p.Y, p.Z );
						Map map = Caster.Map;

						if( map != null && SpellHelper.AdjustField( ref loc, map, 22, true ) )
						{
							if( Scroll != null )
								Scroll.Consume();

							SpellHelper.Turn( Caster, p );
							SpellHelper.GetSurfaceTop( ref p );
							Effects.PlaySound( p, map, 0x2 );

							new SacredStone( Caster, loc, FocusLevel );
							Caster.BeginAction( typeof( EnchantedGroveSpell ) );
						}
						else
						{
							Caster.SendMessage( Caster.Language == "ITA" ? "Non puoi invocare questo potere proprio lì." : "That point cannot be the target of this power." );
						}
					}
					else
					{
						Caster.SendMessage( Caster.Language == "ITA" ? "Non puoi invocarlo qui, prova in un punto vicino a te ma libero da ostacoli." : "You cannot summon a grove here, try to target a point near you." );
					}
				}
			}
			else
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}

			FinishSequence();
		}

		private static void ReleaseGroveLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( EnchantedGroveSpell ) );
			( (Mobile)state ).SendMessage( ((Mobile)state).Language == "ITA" ? "Puoi evocare un nuovo bosco incantato." : "You can cast again your enchanted grove." );
		}

		private class SacredStone : Item
		{
			private static readonly int RareTrees = 1;
			private static readonly double RareGrooveChance = 0.05;

			private readonly Timer m_BlessTimer;
			private readonly Mobile m_Caster;
			private readonly List<DruidTree> m_GroveList = new List<DruidTree>();
			private readonly Point3D m_Location;

			public override bool BlocksFit
			{
				get { return true; }
			}

			public SacredStone( Mobile caster, Point3D loc, int level ) : base( 0x1776 )
			{
				m_Caster = caster;
				m_Location = loc;

				Visible = false;
				Movable = false;

				Name = "Sacred Stone";

				MoveToWorld( m_Location, m_Caster.Map );

				if( m_Caster.InLOS( this ) )
					Visible = true;
				else
					Delete();

				if( Deleted )
				{
					m_Caster.EndAction( typeof( EnchantedGroveSpell ) );
					return;
				}

				BuildGrove();

				Timer.DelayCall( TimeSpan.FromSeconds( 10 + 10 * level ), new TimerCallback( RemoveGrove ) );

				m_BlessTimer = new BlessTimer( this, m_Caster, level > 9 ? 9 : level );
				m_BlessTimer.Start();
			}

			public SacredStone( Serial serial ) : base( serial )
			{
			}

			public override void OnAfterDelete()
			{
				RemoveGrove();
				m_Caster.SendMessage( m_Caster.Language == "ITA" ? "Il boschetto scompare..." : "Time has gone: the Grove vanishes." );
			}

			private void RemoveGrove()
			{
				Delete();
				RemoveGroveObjects();
				if (m_Caster!= null)
				{
					m_Caster.EndAction( typeof( EnchantedGroveSpell ) );
					m_Caster.SendMessage( m_Caster.Language == "ITA" ? "Puoi evocare un nuovo bosco incantato." : "You can cast again your enchanted grove." );
				}
				if( m_BlessTimer != null )
					m_BlessTimer.Stop();
			}

			private void RemoveGroveObjects()
			{
				foreach( DruidTree dt in m_GroveList )
				{
					if( dt != null && !dt.Deleted )
						dt.Delete();
				}
			}

			private void BuildGrove()
			{
				bool rareGrove = Utility.RandomDouble() < RareGrooveChance;

				Point3D p = Location;

				for( int i = 0; i < m_Offsets.Length; i += 2 )
				{
					Point3D finalLoc = new Point3D( p.X + m_Offsets[ i ], p.Y + m_Offsets[ i + 1 ], p.Z );
					DruidTree tree = BuildRandomTree( finalLoc, m_Caster.Map, rareGrove );
					m_GroveList.Add( tree );
				}

				if( rareGrove )
					m_Caster.SendMessage( m_Caster.Language == "ITA" ? "E' nato un raro boschetto di prugne!" : "A rare Plum Grove has born around you!" );
				else
					m_Caster.SendMessage( m_Caster.Language == "ITA" ? "Chiami la natura in tuo soccorso!" : "You call Nature's aid around you!" );
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

			private class TreeEntry
			{
				public int[] BustIDs { get; private set; }

				public int[] LeavesIDs { get; private set; }

				public TreeEntry( int[] bustIDs, int[] leavesIDs )
				{
					BustIDs = bustIDs;
					LeavesIDs = leavesIDs;
				}
			}

			#region m_TreeEntrys
			private readonly TreeEntry[] m_Trees = new TreeEntry[]
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

			#region Circle Offset
			private static readonly int[] m_Offsets = new int[]
			{
				-4, 1,
				-3, 3,
				-1, 4,
				1, 4,
				3, 3,
				4, 1,
				4, -1,
				3, -3,
				1, -4,
				-1, -4,
				-3, -3,
				-4, -1
			};
			#endregion

			private class BlessTimer : Timer
			{
				private readonly SacredStone m_SacredStone;
				private readonly Mobile m_Caster;
				private readonly DateTime m_Expiration;
				private readonly int m_Level;

				public BlessTimer( SacredStone stone, Mobile caster, int level ) : base( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.0 ) )
				{
					Priority = TimerPriority.FiftyMS;

					m_SacredStone = stone;
					m_Caster = caster;
					m_Expiration = DateTime.Now + TimeSpan.FromSeconds( 10 + 10* level );
					m_Level = level;
				}

				protected override void OnTick()
				{
					//if( !m_Caster.InLOS( m_SacredStone ) )
					//{
						//m_SacredStone.Delete();
					//	m_Caster.SendMessage( m_Caster.Language == "ITA" ? "{0}, la tua presenza è necessaria per la sopravvivenza del boschetto!" : "{0}, your presence were needed to the grove existance!", m_Caster.Name );
					//}
					if( m_SacredStone == null || m_SacredStone.Deleted || DateTime.Now > m_Expiration )
						Stop();
					else
					{
						List<Mobile> targets = new List<Mobile>();
						Map map = m_Caster.Map;

						if( map != null )
						{
							Party party = Party.Get( m_Caster );
							IPooledEnumerable eable = map.GetMobilesInRange( m_SacredStone.Location, 5 );

							foreach( Mobile m in eable )
							{
								if (m != m_Caster)
								{
									if (m is NatureFury || m.Criminal)
										continue;

									int noto = Notoriety.Compute( m_Caster, m );
									if( noto == Notoriety.Enemy || noto == Notoriety.Murderer || noto == Notoriety.Criminal )
										continue;
								}
								Party partym = Party.Get( m );

								if( m is BaseCreature && ( (BaseCreature)m ).GetMaster() == m_Caster )
									targets.Add( m );
								else if( m_Caster == m || m_Caster.CanBeBeneficial( m, false ) || party == partym )
									targets.Add( m );
							}

							//foreach( Mobile m in eable )
							//{
							//	if( m.Alive && m.Karma >= 0 && !m.Criminal && m_Caster.CanBeBeneficial( m, false ) )
							//		targets.Add( m );
							//}

							eable.Free();
						}

						foreach( Mobile m in targets )
						{
							m_Caster.DoBeneficial( m );
							m.FixedEffect( 0x37C4, 1, 12, 1109, 3 );

							if( m.Poisoned )
								m.CurePoison( m );

							int lore = (int)m_Caster.Skills[ SkillName.AnimalLore ].Value;

							m.Hits += lore / ( 10 - m_Level ) + Utility.Dice( 1, 10, 0 );
							m.Mana += lore / ( 20 - m_Level ) + Utility.Dice( 1, 5, 0 );
						}
					}
				}
			}

			#region serial-deserial
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
				writer.Write( 0 ); // version
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
				int version = reader.ReadInt();
			}
			#endregion
		}
	}
}