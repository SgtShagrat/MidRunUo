/***************************************************************************
 *                                  CrystalHarvesting.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Definizioni per l'harvesting dei cristalli.
 * 
 ***************************************************************************/

using System;
using Midgard.Items;
using Midgard.Mobiles;
using Server;
using Server.Engines.Harvest;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines
{
	public class CrystalHarvesting : HarvestSystem
	{
		private static CrystalHarvesting m_System;

		public static CrystalHarvesting System
		{
			get
			{
				if ( m_System == null )
					m_System = new CrystalHarvesting();

				return m_System;
			}
		}

		private HarvestDefinition m_Crystals;

		public HarvestDefinition Crystals
		{
			get{ return m_Crystals; }
		}

		private CrystalHarvesting()
		{
			HarvestResource[] res;
			HarvestVein[] veins;

			HarvestDefinition crystals = m_Crystals = new HarvestDefinition();

			// Resource banks are every 8x8 tiles
			crystals.BankWidth = 4;
			crystals.BankHeight = 3;

			// Every bank holds from 6 to 12 crystals
			crystals.MinTotal = 6;
			crystals.MaxTotal = 12;

			// A resource bank will respawn its content every 10 to 20 minutes
			crystals.MinRespawn = TimeSpan.FromMinutes( 100.0 );
			crystals.MaxRespawn = TimeSpan.FromMinutes( 200.0 );

			// Skill checking is done on the Mining skill
			crystals.Skill = SkillName.Mining;

			// Set the list of harvestable tiles
			crystals.Tiles = m_CrystalTiles;

			// Players must be within 2 tiles to harvest
			crystals.MaxRange = 1;

			// One sand per harvest action
			crystals.ConsumedPerHarvest = 1;
			crystals.ConsumedPerFeluccaHarvest = 1;

			// The digging effect
			crystals.EffectActions = new int[]{ 11 };
			crystals.EffectSounds = new int[]{ 0x125, 0x126 };
			crystals.EffectCounts = new int[]{ 1, 2, 2, 2, 3 };
			crystals.EffectDelay = TimeSpan.FromSeconds( 1.6 );
			crystals.EffectSoundDelay = TimeSpan.FromSeconds( 0.9 );

			crystals.NoResourcesMessage = "This vein has been dryed off.";
			crystals.DoubleHarvestMessage = "Be patien while harvesting crystales";
			crystals.TimedOutOfRangeMessage = "Crystals require attention and patience";
			crystals.OutOfRangeMessage = "You are too far away";
			crystals.FailMessage = "You fail to obtain some usable crystals.";
			crystals.PackFullMessage = "Your pack is too full. That's empty it if you would more crystals!";
			crystals.ToolBrokeMessage = "You have broken the harvest tool!";

			res = new HarvestResource[]
				{
					new HarvestResource( 100.0, 90.0, 120.0, 1064889, 	typeof( CrystalOre ),	typeof( CrystalOreElemental ) ),
				};

			veins = new HarvestVein[]
				{
					new HarvestVein( 100.0, 0.0, res[0], null )
				};

			crystals.Resources = res;
			crystals.Veins = veins;

			if ( Core.ML )
			{
				crystals.BonusResources = new BonusHarvestResource[]
				{
					new BonusHarvestResource( 0.00,   99.8,    null, null ),

					new BonusHarvestResource( 100.0,  0.033, 1072562, typeof( BlueDiamond ) ),
					new BonusHarvestResource( 100.0,  0.033, 1072567, typeof( DarkSapphire ) ),
					new BonusHarvestResource( 100.0,  0.033, 1072570, typeof( EcruCitrine ) ),
					new BonusHarvestResource( 100.0,  0.033, 1072564, typeof( FireRuby ) ),
					new BonusHarvestResource( 100.0,  0.033, 1072566, typeof( PerfectEmerald ) ),
					new BonusHarvestResource( 100.0,  0.033, 1072568, typeof( Turquoise ) ),
				};
			}

			crystals.RaceBonus = Core.ML;
			crystals.RandomizeVeins = false;

			Definitions.Add( crystals );
		}

		public override bool CheckHarvest( Mobile from, Item tool )
		{
			if ( !base.CheckHarvest( from, tool ) )
			{
				return false;
			}
			else if ( from.Mounted )
			{
				from.SendMessage( "You cannot approach crystals while mounted!" );
				return false;
			}
			else if ( from.IsBodyMod && !from.Body.IsHuman )
			{
				from.SendMessage( "You cannot approach crystals while polymorphed!" );
				return false;
			}
			else
			{
				return true;
			}
		}

		public override bool CheckHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			if ( !base.CheckHarvest( from, tool, def, toHarvest ) )
				return false;

			if ( !(from is Midgard2PlayerMobile && from.Skills[SkillName.Mining].Base >= 100.0 && ((Midgard2PlayerMobile)from).CrystalHarvesting) )
			{
				OnBadHarvestTarget( from, tool, toHarvest );
				return false;
			}
			else if ( from.Mounted )
			{
				from.SendMessage( "You cannot approach crystals while mounted!" );
				return false;
			}
			else if ( from.IsBodyMod && !from.Body.IsHuman )
			{
				from.SendMessage( "You cannot approach crystals while polymorphed!" );
				return false;
			}
			else
			{
				return true;
			}
		}

		public override void SendSuccessTo( Mobile from, Item item, HarvestResource resource )
		{
			if ( item is CrystalOre )
				from.SendMessage( "You extract some workable crystal!" );
			else
				base.SendSuccessTo( from, item, resource );
		}

		private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				 0, -1,
				 0,  1,
				 1, -1,
				 1,  0,
				 1,  1
			};

		private static double ElementalChance = 0.1;

		public override void OnHarvestFinished( Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested )
		{
			if ( Utility.RandomDouble() < ElementalChance )
			{
				HarvestResource res = vein.PrimaryResource;

				if ( res == resource && res.Types.Length >= 2 )
				{
					try
					{
						Map map = from.Map;

						if ( map == null )
							return;

						BaseCreature spawned = Activator.CreateInstance( res.Types[1], new object[]{ 5 } ) as BaseCreature;

						if ( spawned != null )
						{
							int offset = Utility.Random( 8 ) * 2;

							for ( int i = 0; i < m_Offsets.Length; i += 2 )
							{
								int x = from.X + m_Offsets[(offset + i) % m_Offsets.Length];
								int y = from.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

								if ( map.CanSpawnMobile( x, y, from.Z ) )
								{
									spawned.MoveToWorld( new Point3D( x, y, from.Z ), map );
									spawned.Combatant = from;
									return;
								}
								else
								{
									int z = map.GetAverageZ( x, y );

									if ( map.CanSpawnMobile( x, y, z ) )
									{
										spawned.MoveToWorld( new Point3D( x, y, z ), map );
										spawned.Combatant = from;
										return;
									}
								}
							}

							spawned.MoveToWorld( from.Location, from.Map );
							spawned.Combatant = from;
						}
					}
					catch
					{
					}
				}
			}
		}

		public override bool BeginHarvesting( Mobile from, Item tool )
		{
			if ( !base.BeginHarvesting( from, tool ) )
				return false;

			from.SendLocalizedMessage( 503033 ); // Where do you wish to dig?
			return true;
		}

		public override void OnHarvestStarted( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			base.OnHarvestStarted( from, tool, def, toHarvest );

			from.RevealingAction( true );
		}

		public override void OnBadHarvestTarget( Mobile from, Item tool, object toHarvest )
		{
			if ( toHarvest is LandTarget )
				from.SendMessage( "You cannot extract any usable crystal from there!" );
			else
				from.SendMessage( "You cannot extract any usable crystal from that!" );
		}

		#region Tile lists
		private static int[] m_CrystalTiles = new int[]
			{
                0x6206, 0x6207, 0x6208, 0x6209, 0x620A, 0x620B, 0x620C, 0x620D,
                0x620E, 0x6210, 0x6211, 0x6212, 0x6213, 0x6214, 0x6215, 0x6216,
                0x6217, 0x6218, 0x621A, 0x621B, 0x621C, 0x621D, 0x621E, 0x621F,
                0x6220, 0x6221, 0x6222, 0x6224, 0x6225, 0x6226, 0x6227, 0x6228,
                0x6229, 0x622A, 0x622B, 0x622C
			};
		#endregion
	}
}
