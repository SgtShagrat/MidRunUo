/***************************************************************************
 *                                  HoneycombProcessingKettleQuest.cs
 *                            		---------------------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Quest relativa al Apiculture system.
 * 			Richiede 5 HoneyComb.
 * 			Come premio da un HoneycombProcessingKettle.
 * 
 ***************************************************************************/

using System;

using Midgard.Items;

using Server;
using Server.Engines.Quests;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.Quests
{
	public class HoneycombProcessingKettleQuest : BaseQuest
	{	
		public override TimeSpan RestartDelay{ get{ return TimeSpan.FromDays( 1.0 ); } }
		
		/* I love Bears but honey more */
		public override object Title{ get{ return 1065405; } }
		
		/* The old man takes a good look at you. 'Hey, you there! You look like someone who's willing and able to help me!
		I am a candle crafter. Well, I used to be. I'm too old to roam the woods and collect honeycombs out of beehives.
		Get some for me! You find honeycombs in beehives, of course, but the bears that live in the close area around the 
		hives tend to steal them and have honeycombs, too! You wont regret helping me! Get me five honeycombs and I'll give
		you one of these kettles. You can use them to separate honey and wax out of a honeycomb, so you can process it further! */
		public override object Description{ get{ return 1065406; } }
		
		/* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
		public override object Refuse{ get{ return 1065407; } }
		
		/* You're not quite done yet.  Get back to work! */
		public override object Uncomplete{ get{ return 1065408; } }
		
		/* Thank you! Here, theres your Honeycomb Processing Kettle! */
		public override object Complete{ get{ return 1065409; } }
	
		public HoneycombProcessingKettleQuest() : base()
		{			
			AddObjective( new ObtainObjective( typeof( HoneyComb ), "honey comb", 5 ) );	
			
			AddReward( new BaseReward( typeof( HoneycombProcessingKettle ), 1065418 ) );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}		
	}
	
	public class Osvald : MondainQuester
	{		
		public override Type[] Quests
		{ 
			get{ return new Type[] 
			{ 
				typeof( HoneycombProcessingKettleQuest ),
			};} 
		}
		
		[Constructable]
		public Osvald() : base( "Elder Osvald", "the old friend of bees" )
		{
		}
		
		public Osvald( Serial serial ) : base( serial )
		{
		}		
		
		public override void InitBody()
		{
			InitStats( 100, 100, 25 );
			
			Female = false;
			Race = Race.Human;
			
			Hue = 0x840E;
			HairItemID = 0x203D;
			HairHue = 0x1;
			FacialHairItemID = 0x203F;
			FacialHairHue = 0x1;
		}
		
		public override void InitOutfit()
		{
			AddItem( new Backpack() );		
			AddItem( new Sandals( 0x753 ) );
			AddItem( new LeatherArms() );
			AddItem( new LeatherGloves() );
			AddItem( new LeatherLegs() );
			AddItem( new LeatherChest() );
			AddItem( new LeatherGorget() );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
	
	[CorpseName( "a bear corpse" )]
	public class HoneyBear : BaseCreature
	{
		[Constructable]
		public HoneyBear() : base(AIType.AI_Animal, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "a brown bear";
			Body = 167;
			BaseSoundID = 0xA3;
			
			SetStr( 76, 100 );
			SetDex( 26, 45 );
			SetInt( 23, 47 );
			
			SetHits( 46, 60 );
			SetMana( 0 );
			
			SetDamage( 6, 12 );
			
			SetDamageType( ResistanceType.Physical, 100 );
			
			SetResistance( ResistanceType.Physical, 20, 30 );
			SetResistance( ResistanceType.Cold, 15, 20 );
			SetResistance( ResistanceType.Poison, 10, 15 );
			
			SetSkill( SkillName.MagicResist, 25.1, 35.0 );
			SetSkill( SkillName.Tactics, 40.1, 60.0 );
			SetSkill( SkillName.Wrestling, 40.1, 60.0 );
			
			Fame = 450;
			Karma = 0;
			
			VirtualArmor = 24;
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 41.1;
			
			PackItem( new HoneyComb() );
			
			#region xmlattach
			// Attachment EnemyMastery
			XmlEnemyMastery WyrmMastery = new XmlEnemyMastery( "WhiteWyrm", 100, 1000);
			WyrmMastery.Name = "WyrmMastery";
			XmlAttach.AttachTo(this, WyrmMastery);
			
			XmlEnemyMastery DragonMastery = new XmlEnemyMastery( "Dragon", 100, 1000);
			DragonMastery.Name = "DragonMastery";
			XmlAttach.AttachTo(this, DragonMastery);
			
			XmlEnemyMastery NightmareMastery = new XmlEnemyMastery( "Nightmare", 100, 1000);
			NightmareMastery.Name = "NightmareMastery";
			XmlAttach.AttachTo(this, NightmareMastery);
			
			XmlEnemyMastery KirinMastery = new XmlEnemyMastery( "Kirin", 100, 1000);
			KirinMastery.Name = "KirinMastery";
			XmlAttach.AttachTo(this, KirinMastery);
			
			XmlEnemyMastery UnicornMastery = new XmlEnemyMastery( "Unicorn", 100, 1000);
			UnicornMastery.Name = "UnicornMastery";
			XmlAttach.AttachTo(this, UnicornMastery);
			#endregion	
		}
	
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 12; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.FruitsAndVegies | FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }
		
		public HoneyBear( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
	
	public class HoneyComb : Item
	{
		public override int LabelNumber{ get{ return 1065417; } } // Honey Comb
		
		[Constructable]
		public HoneyComb() : this( 1 )
		{
		}
		
		[Constructable]
		public HoneyComb( int amount ) : base( 0x1762 )
		{
			Stackable = true;
			Weight = 2.0;
			Amount = amount;
			Hue = 1177;
		}
		
		public HoneyComb( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
	
	public class HoneycombProcessingKettle : Item 
	{ 
		public override int LabelNumber{ get{ return 1065418; } } // Honeycomb Processing Kettle
		
		[Constructable] 
		public HoneycombProcessingKettle() : base( 0x9ED ) 
		{ 
			Weight = 10.0;             
		} 
	
		public override void OnDoubleClick( Mobile from ) 
		{ 
			Container pack = from.Backpack; 
			
			if( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				if (pack != null && pack.ConsumeTotal( typeof( HoneyComb ), 1 ) ) 
				{ 
					from.SendLocalizedMessage( 1065419 ); // You centrifuge the honeycomb and separate honey and wax.
					from.AddToBackpack( new RawBeeswax() ); 
					from.AddToBackpack( new JarHoney() );
				} 
				else 
				{ 
					from.SendLocalizedMessage( 1065420 ); // You need a honeycomb to use in this kettle.
					return; 
				} 
			}
			else 
			{ 
				from.SendLocalizedMessage( 1065421 ); //	You are too far away from this.
				return; 
			} 
		}
		
		public HoneycombProcessingKettle( Serial serial ) : base( serial )
		{
		}
	
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	} 

  	public class WildBeehive: Item
	{
  		public override int LabelNumber{ get{ return 1065423; } } // Wild Beehive
  		
		private int m_UsesRemaining = Utility.RandomMinMax( 1, 3 );
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		[Constructable]
		public WildBeehive() : base( 0x91A )
		{
			Movable = false;
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				if ( m_UsesRemaining == 1 )
				{
					from.AddToBackpack( new HoneyComb() );
					InvalidateProperties();
					from.SendLocalizedMessage( 1065424 ); //	As you pull out the last honeycomb the beehive collapses.
					Delete();
				}
				else
				{
					from.AddToBackpack( new HoneyComb() );
					m_UsesRemaining -= 1;
					InvalidateProperties();
					from.SendLocalizedMessage( 1065425 ); //	You take a honeycomb out of the beehive.
					
				}
			}
			else 
  			{ 
				from.SendLocalizedMessage( 1065426 ); // You are too far away from the beehive.
  			} 
		}
		
		public override bool HandlesOnMovement
		{
			get { return true; }
		}

		public WildBeehive( Serial serial ) : base( serial )
		{
		}		

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );

			writer.Write( (int) m_UsesRemaining ); 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			m_UsesRemaining = (int)reader.ReadInt(); 			
			
		}
	}
}
