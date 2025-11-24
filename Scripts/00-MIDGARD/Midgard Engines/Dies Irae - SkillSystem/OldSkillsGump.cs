/***************************************************************************
 *							   Dies Irae - OldSkillsGump.cs
 *
 *   begin				: 07 November, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Engines.SkillSystem;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

using Skills = Server.Skills;

namespace Midgard.Gumps
{
	public class OldSkillsGump : Gump
	{
		private const int ColumnsOffset = 200;
		private const int ColumnsValueOffset = 130;
		private const int FirstLabelX = 40;
		private const int FirstLabelY = 40;
		private const int GroupsHue = Colors.RoyalBlue;
		private const int SubCappedSkills = Colors.DarkGoldenRod;
		// private const int OutOfCapSkillsHue = Colors.Darkorange;
		private const int NumRows = 23;
		private const int RawsOffset = 15;
		private const int WindowMaxX = 640;
		private const int WindowsMaxY = 430;

		private readonly Mobile m_Owner;

		public OldSkillsGump( Mobile owner ) : base( 0, 0 )
		{
			m_Owner = owner;

			int count = m_Groups.Length;
			Skills skills = m_Owner.Skills;

			Closable = true;
			Disposable = true;
			Dragable = true;

			AddPage( 0 );

			AddBackground( 0, 0, WindowMaxX + 3, WindowsMaxY, 0x1432 );
			AddImage( WindowMaxX / 2 - 30, 6, 0x834 ); // Skills

			int x;
			int y;

			int index = 0;

			for( int i = 0; i < count; i++ )
			{
				SkillsGumpGroup group = m_Groups[ i ];
				GetOffsetsbyIndex( index, out x, out y );
				AddOldHtmlHued( x, y, 200, 30, group.Name, GroupsHue );
				index++;

				foreach( SkillName t in group.Skills )
				{
					SkillName skill = t;
					GetOffsetsbyIndex( index, out x, out y );

					bool rawVision = ( owner is Midgard2PlayerMobile ) && ( (Midgard2PlayerMobile)owner ).PolSkillRawPointsEnabled && ( (Midgard2PlayerMobile)owner ).NotificationPolRawPointsEnabled;

					string value = (rawVision ? ( (Midgard2PlayerMobile)owner ).RawSkill( skill ).ToString() : skills[ skill ].Base.ToString( "F1" ));

					if( SkillSubCap.IsUnderAnySubCap( skill ) )
					{
						AddOldHtmlHued( x, y, 200, 30, GetRealSkillName( skill ), SubCappedSkills );
						AddOldHtmlHued( x + ColumnsValueOffset - (rawVision ? 24 : 2), y, 100, 30, value, SubCappedSkills );
					}
					else
					{
						AddOldHtml( x, y, 200, 30, GetRealSkillName( skill ) );
						AddOldHtml( x + ColumnsValueOffset - (rawVision ? 24 : 2) , y, 100, 30, value );
					}

					Skill sk = skills[ t ];
					if( sk != null )
					{
						int buttonID1, buttonID2;
						int xOffset, yOffset;

						switch( sk.Lock )
						{
							default:
							case SkillLock.Up:
								buttonID1 = 0x984;
								buttonID2 = 0x983;
								xOffset = 1;
								yOffset = 4;
								break;
							case SkillLock.Down:
								buttonID1 = 0x986;
								buttonID2 = 0x985;
								xOffset = 1;
								yOffset = 4;
								break;
							case SkillLock.Locked:
								buttonID1 = 0x82C;
								buttonID2 = 0x82C;
								xOffset = 0;
								yOffset = 2;
								break;
						}

						AddButton( x + 170 + xOffset, y + yOffset, buttonID1, buttonID2, 100 + sk.SkillID, GumpButtonType.Reply, 0 );

						if( sk.Info.Callback != null )
							AddButton( x - 10, y + 3, 0x837, 0x837, 1000 + sk.SkillID, GumpButtonType.Reply, 0 );
					}

					index++;
				}
			}

			AddOldHtmlHued( WindowMaxX / 2 - 100, WindowsMaxY - 25, 300, 30,
			string.Format( "Total: {0}/{1}",
			( m_Owner.SkillsTotal / 10.0 ).ToString( "F1" ),
			( m_Owner.SkillsCap / 10.0 ).ToString( "F1" ) ), 100 );

			index = index + 4;

			GetOffsetsbyIndex( index, out x, out y );
			AddOldHtml( x, y, 200, 30, "Skills Color:" );
			AddOldHtml( x + 20, y + RawsOffset * 1, 200, 30, "Skill in Cap" );

			// AddOldHtml( x + 20, y + RawsOffset * 2, 200, 30, "Out of Cap" );
			// AddOldHtmlHued( x + 20, y + RawsOffset * 2, 200, 30, "Out of Cap", OutOfCapSkills );

			AddOldHtmlHued( x + 20, y + RawsOffset * 2, 200, 30, "SubCapped Skill", SubCappedSkills );
			// AddOldHtmlHued( x + 20, y + RawsOffset * 3, 200, 30, "SubCapped + OutofCap:", 24 );
		}

		private enum ActionType
		{
			Use,
			Lock
		}

		private static string GetRealSkillName( SkillName name )
		{
			if( name == SkillName.Chivalry )
				return "Faith";
			else if( name == SkillName.Discordance )
				return "Enticement";
			else
				return name.ToString();
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if( info.ButtonID == 0 )
				return;

			int buttonID = 0;

			ActionType action = ActionType.Lock;
			if( info.ButtonID >= 1000 )
			{
				buttonID = info.ButtonID - 1000;
				action = ActionType.Use;
			}
			else if( info.ButtonID >= 100 )
			{
				buttonID = info.ButtonID - 100;
				action = ActionType.Lock;
			}

			if( buttonID >= 0 && buttonID < m_Owner.Skills.Length )
			{
				Skill sk = m_Owner.Skills[ buttonID ];
				if( sk != null )
				{
					if( action == ActionType.Lock )
					{
						switch( sk.Lock )
						{
							case SkillLock.Up: sk.SetLockNoRelay( SkillLock.Down ); sk.Update(); break;
							case SkillLock.Down: sk.SetLockNoRelay( SkillLock.Locked ); sk.Update(); break;
							case SkillLock.Locked: sk.SetLockNoRelay( SkillLock.Up ); sk.Update(); break;
						}

						m_Owner.SendGump( new OldSkillsGump( m_Owner ) );
					}
					else if( action == ActionType.Use )
					{
						Skills.UseSkill( m_Owner, buttonID );
					}
				}
			}
		}

		public static void Initialize()
		{
			CommandSystem.Register( "Skill", AccessLevel.Player, new CommandEventHandler( SkillGump_OnCommand ) );
		}

		[Usage( "Skill" )]
		[Description( "Open the old fashioned skill gump" )]
		public static void SkillGump_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null )
				return;

			if( e.Length == 0 )
			{
				if( from.HasGump( typeof( OldSkillsGump ) ) )
					from.CloseGump( typeof( OldSkillsGump ) );

				from.SendGump( new OldSkillsGump( from ) );
			}
			else
				from.SendMessage( "Command Use: [Skill" );
		}

		private static void GetOffsetsbyIndex( int index, out int offsetX, out int offsetY )
		{
			offsetX = FirstLabelX + ColumnsOffset * ( index / NumRows );
			offsetY = FirstLabelY + RawsOffset * ( index % NumRows );
		}

		#region Groups
		private static readonly SkillsGumpGroup[] m_Groups = new SkillsGumpGroup[]
		{
			new SkillsGumpGroup( "Crafting",
				new SkillName[]
				{
					SkillName.Alchemy,
					SkillName.Blacksmith,
					SkillName.Cartography,
					SkillName.Carpentry,
					SkillName.Cooking,
					SkillName.Fletching,
					SkillName.Inscribe,
					SkillName.Tailoring,
					SkillName.Tinkering
				}
			),

			new SkillsGumpGroup( "Bardic",
				new SkillName[]
				{
					SkillName.Discordance,
					SkillName.Musicianship,
					SkillName.Peacemaking,
					SkillName.Provocation
				}
			),

			new SkillsGumpGroup( "Magical",
				new SkillName[]
				{
					SkillName.EvalInt,
					SkillName.Magery,
					SkillName.MagicResist,
					SkillName.Meditation,
				}
			),

			new SkillsGumpGroup( "Miscellaneous",
				new SkillName[]
				{
					SkillName.Camping,
					SkillName.Fishing,
					SkillName.Healing,
					SkillName.Herding,
					SkillName.Lockpicking,
					SkillName.Lumberjacking,
					SkillName.Mining,
					SkillName.Snooping,
					SkillName.Veterinary
				}
			),

			new SkillsGumpGroup( "Combat",
				new SkillName[]
				{
					SkillName.Archery,
					SkillName.Fencing,
					SkillName.Macing,
					SkillName.Parry,
					SkillName.Swords,
					SkillName.Tactics,
					SkillName.Wrestling
				}
			),

			new SkillsGumpGroup( "Actions",
				new SkillName[]
				{
					SkillName.AnimalTaming,
					SkillName.Begging,
					SkillName.DetectHidden,
					SkillName.Hiding,
					SkillName.RemoveTrap,
					SkillName.Poisoning,
					SkillName.Stealing,
					SkillName.Stealth,
					SkillName.Tracking
				}
			),

			new SkillsGumpGroup( "Lore & Knowledge",
				new SkillName[]
				{
					SkillName.Anatomy,
					SkillName.AnimalLore,
					SkillName.ArmsLore,
					SkillName.Forensics,
					SkillName.ItemID,
					SkillName.TasteID
				}
			),

			new SkillsGumpGroup( "Mystical",
				new SkillName[]
				{
					SkillName.SpiritSpeak,
					SkillName.Chivalry,
					SkillName.Necromancy,
					SkillName.Spellweaving
				}
			)

		};
		#endregion
	}
}