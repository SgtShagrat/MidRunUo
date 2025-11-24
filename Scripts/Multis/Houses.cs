using System;
using System.Collections;
using Midgard.Multis;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
    // http://wiki.uosecondage.com/index.php?title=House#Fortresses
    /* Building Name 	        Footprint  Stories  Rooms	                   Lockdowns	Secure 	Cost
     * Tower         	        16 x 14    3 + Roof	6 + 3 Stairway Rooms   	   244 	  	    12	    433,200 GP
     * Keep                     24 x 24    2        10 + 4 Small Balconies 	   375	 	    19	    665,200 GP 
     * Castle        	        31 x 31    2        11 + 4 Balc & a Courtyard  577	 	    29	    1,022,800 GP
     * Wood House         		7 x 7      1      	1            	            25           1	    43,800 GP 894gp/tile
     * Fieldstone House		    7 x 7      1      	1       	                25           1	    43,800 GP
     * Small Brick House		7 x 7      1      	1              	            25           1	    43,800 GP
     * Stone and Plaster House	7 x 7      1      	1              	            25           1      43,800 GP
     * Thatched Roof Cottage	7 x 7      1      	1              	            25           1	    43,800 GP
     * Wood and Plaster House	7 x 7      1      	1       	                25           1	    43,800 GP
     * Brick House (Large)		14 x 14    1      	3              	            86           4	    144,500 GP
     * Large Shop           	14 x 14    1      	3 + Porch	                86           4	    144,500 GP 737gp/tile
     * Stone and Plaster House	14 x 14    2      	4       	               108         	 5      192,400 GP 490 gp/tile
     * Wood and Plaster House	14 x 14    2      	4              	           108         	 5	    192,400 GP
     * Blue Tent	            11 x 11    1      	1              	             0         	 1	     22,800 GP
     * Green Tent	            11 x 11    1      	1              	             0         	 1	     22,800 GP
     */

    // Blue Tent, Green Tent
    public class OldTent : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -3, -3, 8, 8 ) };
        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override int DefaultPrice { get { return 22800; } }
        public override Point3D BaseBanLocation { get { return new Point3D( 1, 4, 0 ); } }
        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.TwoStoryFoundations[ 0 ]; } }

        public OldTent( Mobile owner, int id )
            : base( id, owner, 1, 0 )
        {
            uint keyValue = CreateKeys( owner );

            SetSign( -1, 5, 9 );

            ChangeSignType( 0x0bd1 );
        }

        public OldTent( Serial serial )
            : base( serial )
        {
        }

        public override bool IsInside( Point3D p, int height )
        {
            if( Deleted )
                return false;

            foreach( Rectangle2D rect in Area )
            {
                if( rect.Contains( new Point2D( p.X - X, p.Y - Y ) ) )
                    return true;
            }

            return false;
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x70:
                    return new BlueTentDeed();
                case 0x72:
                    return new GreenTentDeed();
                default:
                    return new BlueTentDeed();
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );//version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    // L / 17 S / 3
    // Wood House, Fieldstone House, Small Brick House, Stone and Plaster, Thatched Roof Cottage, Wood and Plaster House
	public class SmallOldHouse : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D(-3,-3,7,7 ), new Rectangle2D( -1, 4, 3, 1 ) };

		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 2, 4, 0 ); } }

		public override int DefaultPrice{ get{ return 43800; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[0]; } }

		public SmallOldHouse( Mobile owner, int id ) : base( id, owner, Core.AOS ? 425 : 25, Core.AOS ? 3 : 1 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoor( 0, 3, 7, keyValue );

			SetSign( 2, 4, 5 );
		}

		public SmallOldHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{
			switch ( ItemID ^ 0x4000 )
			{
				case 0x64: return new StonePlasterHouseDeed();
				case 0x66: return new FieldStoneHouseDeed();
				case 0x68: return new SmallBrickHouseDeed();
				case 0x6A: return new WoodHouseDeed();
				case 0x6C: return new WoodPlasterHouseDeed(); 
				case 0x6E: 
				default: return new ThatchedRoofCottageDeed();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 12 S / 2
    // Brick House (Large)
	public class GuildHouse : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -7, -7, 14, 14 ), new Rectangle2D( -2, 7, 4, 1 ) };

		public override int DefaultPrice{ get{ return 144500; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.ThreeStoryFoundations[20]; } }
		public override int ConvertOffsetX{ get{ return -1; } }
		public override int ConvertOffsetY{ get{ return -1; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 4, 8, 0 ); } }

		public GuildHouse( Mobile owner ) : base( 0x74, owner, Core.AOS ? 1100 : 86, Core.AOS ? 8 : 4 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( -1, 6, 7, keyValue );

			SetSign( 4, 8, 16 );

			AddSouthDoor( -3, -1, 7 );
			AddSouthDoor(  3, -1, 7 );
		}

		public GuildHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new BrickHouseDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 13 S / 2
    // Stone and Plaster House & Wood and Plaster House
	public class TwoStoryHouse : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -7, 0, 14, 7 ), new Rectangle2D( -7, -7, 9, 7 ), new Rectangle2D( -4, 7, 4, 1 ) };

		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 2, 8, 0 ); } }

		public override int DefaultPrice{ get{ return 192400; } }

		public TwoStoryHouse( Mobile owner, int id ) : base( id, owner, Core.AOS ? 1370 : 108, Core.AOS ? 10 : 5 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( -3, 6, 7, keyValue );

			SetSign( 2, 8, 16 );

			AddSouthDoor( -3, 0, 7 );
			AddSouthDoor( id == 0x76 ? -2 : -3, 0, 27 );
		}

		public TwoStoryHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{ 
			switch( ItemID ^ 0x4000 )
			{
				case 0x76: return new TwoStoryWoodPlasterHouseDeed();
				case 0x78:
				default: return new TwoStoryStonePlasterHouseDeed();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 9 S / 1.25
    // Tower
    [HouseRooms( "6" )]
    [HouseStoriesAttribute( "3 + Roof" )]
    [HouseDescription( "This building is quite large, and has plenty of room for an entire Guild. " + 
        "The addition of a useable roof makes this building quite unique." + 
        "It's one shortcoming is that from outside, you can see into Floor 3 if you stand under one of the " + 
        "Roof Supports (seen on the Third Floor Image)." )]
	public class Tower : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -7, -7, 16, 14 ), new Rectangle2D( -1, 7, 4, 2 ), new Rectangle2D( -11, 0, 4, 7 ), new Rectangle2D( 9, 0, 4, 7 ) };

		public override int DefaultPrice{ get{ return 433200; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.ThreeStoryFoundations[37]; } }
		public override int ConvertOffsetY{ get{ return -1; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 5, 8, 0 ); } }

		public Tower( Mobile owner ) : base( 0x7A, owner, Core.AOS ? 2119 : 244, Core.AOS ? 15 : 12 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( false, 0, 6, 6, keyValue );

			SetSign( 5, 8, 16 );

			AddSouthDoor( false, 3, -2, 6 );
			AddEastDoor( false, 1, 4, 26 );
			AddEastDoor( false, 1, 4, 46 );
		}

		public Tower( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new TowerDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 7 S / 0.94
    // Small Stone Keep
	public class Keep : BaseHouse//warning: ODD shape!
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -11, -11, 7, 8 ), new Rectangle2D( -11, 5, 7, 8 ), new Rectangle2D( 6, -11, 7, 8 ), new Rectangle2D( 6, 5, 7, 8 ), new Rectangle2D( -9, -3, 5, 8 ), new Rectangle2D( 6, -3, 5, 8 ), new Rectangle2D( -4, -9, 10, 20 ), new Rectangle2D( -1, 11, 4, 1 ) };

		public override int DefaultPrice{ get{ return 665200; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 5, 13, 0 ); } }

		public Keep( Mobile owner ) : base( 0x7C, owner, Core.AOS ? 2625 : 375, Core.AOS ? 18 : 19 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( false, 0, 10, 6, keyValue );
			
			SetSign( 5, 12, 16 );
		}

		public Keep( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new KeepDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 7 S / 0.97
    // Castle
	public class Castle : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{  new Rectangle2D( -15, -15, 31, 31 ), new Rectangle2D( -1, 16, 4, 1 ) };

		public override int DefaultPrice{ get{ return 1022800; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 5, 17, 0 ); } }

		public Castle( Mobile owner ) : base( 0x7E, owner, Core.AOS ? 4076 : 577, Core.AOS ? 28 : 29 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( false, 0, 15, 6, keyValue );

			SetSign( 5, 17, 16 );

			AddSouthDoors( false, 0, 11, 6, true );
			AddSouthDoors( false, 0, 5, 6, false );
			AddSouthDoors( false, -1, -11, 6, false );
		}

		public Castle( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new CastleDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 12 S / 2
    // Large Shop 
	public class LargePatioHouse : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -7, -7, 15, 14 ), new Rectangle2D( -5, 7, 4, 1 ) };

		public override int DefaultPrice{ get{ return Core.AOS ? 152800 : 144500; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.ThreeStoryFoundations[29]; } }
		public override int ConvertOffsetY{ get{ return -1; } }

		public override Rectangle2D[] Area { get { return AreaArray; } }
		public override Point3D BaseBanLocation { get { return new Point3D( 1, 8, 0 ); } }

		public LargePatioHouse( Mobile owner ) : base( 0x8C, owner, Core.AOS ? 1100 : 86, Core.AOS ? 8 : 4 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( -4, 6, 7, keyValue );
			
			SetSign( 1, 8, 16 );

			AddEastDoor( 1, 4, 7 );
			AddEastDoor( 1, -4, 7 );
			AddSouthDoor( 4, -1, 7 );
		}

		public LargePatioHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new LargePatioDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 10 S / 2
	public class LargeMarbleHouse : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -7, -7, 15, 14 ), new Rectangle2D( -6, 7, 6, 1 ) };

		public override int DefaultPrice{ get{ return 192000; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.ThreeStoryFoundations[29]; } }
		public override int ConvertOffsetY{ get{ return -1; } }

		public override Rectangle2D[] Area { get { return AreaArray; } }
		public override Point3D BaseBanLocation { get { return new Point3D( 1, 8, 0 ); } }

		public LargeMarbleHouse( Mobile owner ) : base( 0x96, owner, Core.AOS ? 1370 : 125, Core.AOS ? 10 : 5 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( false, -4, 3, 4, keyValue );

			SetSign( 1, 8, 11 );
		}

		public LargeMarbleHouse( Serial serial ) : base( serial )
		{
		}
        
		public override HouseDeed GetDeed() { return new LargeMarbleDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 10 S / 2
	public class SmallTower : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -3, -3, 8, 7 ), new Rectangle2D( 2, 4, 3, 1 ) };

		public override int DefaultPrice{ get{ return 88500; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[6]; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 1, 4, 0 ); } }

		public SmallTower( Mobile owner ) : base( 0x98, owner, Core.AOS ? 580 : 50, Core.AOS ? 4 : 2 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoor( false, 3, 3, 6, keyValue );

			SetSign( 1, 4, 5 );
		}

		public SmallTower( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new SmallTowerDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 10 S / 2
	public class LogCabin : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -3, -6, 8, 13 ) };

		public override int DefaultPrice{ get{ return 97800; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[12]; } }

		public override Rectangle2D[] Area { get { return AreaArray; } }
		public override Point3D BaseBanLocation { get { return new Point3D( 5, 8, 0 ); } }

		public LogCabin( Mobile owner ) : base( 0x9A, owner, Core.AOS ? 1100 : 86, Core.AOS ? 8 : 2 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoor( 1, 4, 8, keyValue );
			
			SetSign( 5, 8, 20 );

			AddSouthDoor( 1, 0, 29 );
		}

		public LogCabin( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new LogCabinDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
    
    // L / 10 S / 2
	public class SandStonePatio : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -5, -4, 12, 8 ), new Rectangle2D( -2, 4, 3, 1 ) };

		public override int DefaultPrice{ get{ return 90900; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[35]; } }
		public override int ConvertOffsetY{ get{ return -1; } }

		public override Rectangle2D[] Area { get { return AreaArray; } }
		public override Point3D BaseBanLocation { get { return new Point3D( 4, 6, 0 ); } }

		public SandStonePatio( Mobile owner ) : base( 0x9C, owner, Core.AOS ? 850 : 70, Core.AOS ? 6 : 3 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoor( -1, 3, 6, keyValue );
			
			SetSign( 4, 6, 24 );
		}

		public SandStonePatio( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new SandstonePatioDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 10 S / 2
	public class TwoStoryVilla : BaseHouse
	{
		public static Rectangle2D[] AreaArray = new Rectangle2D[]{ new Rectangle2D( -5, -5, 11, 11 ), new Rectangle2D( 2, 6, 4, 1 ) };

		public override int DefaultPrice{ get{ return 136500; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[31]; } }

		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 3, 8, 0 ); } }

		public TwoStoryVilla( Mobile owner ) : base( 0x9E, owner, Core.AOS ? 1100 : 86, Core.AOS ? 8 : 4 )
		{
			uint keyValue = CreateKeys( owner );

			AddSouthDoors( 3, 1, 5, keyValue );
			
			SetSign( 3, 8, 24 );

			AddEastDoor( 1, 0, 25 );
			AddSouthDoor( -3, -1, 25 );
		}

		public TwoStoryVilla( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() { return new VillaDeed(); }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    // L / 10 S / 2
	public class SmallShop : BaseHouse
	{
		public override Rectangle2D[] Area { get { return ( ItemID == 0x40A2 ? AreaArray1 : AreaArray2 ); } }
		public override Point3D BaseBanLocation { get { return new Point3D( 3, 4, 0 ); } }

		public override int DefaultPrice{ get{ return 63000; } }

		public override HousePlacementEntry ConvertEntry{ get{ return HousePlacementEntry.TwoStoryFoundations[0]; } }

		public static Rectangle2D[] AreaArray1 = new Rectangle2D[]{ new Rectangle2D(-3,-3,7,7), new Rectangle2D( -1, 4, 4, 1 ) };
		public static Rectangle2D[] AreaArray2 = new Rectangle2D[]{ new Rectangle2D(-3,-3,7,7), new Rectangle2D( -2, 4, 3, 1 ) };

		public SmallShop( Mobile owner, int id ) : base( id, owner, Core.AOS ? 425 : 25, Core.AOS ? 3 : 1 )
		{
			uint keyValue = CreateKeys( owner );

			BaseDoor door = MakeDoor( false, DoorFacing.EastCW );

			door.Locked = true;
			door.KeyValue = keyValue;

			if ( door is BaseHouseDoor )
				((BaseHouseDoor)door).Facing = DoorFacing.EastCCW;

			AddDoor( door, -2, 0, id == 0xA2 ? 24 : 27 );

			//AddSouthDoor( false, -2, 0, 27 - (id == 0xA2 ? 3 : 0), keyValue );
			
			SetSign( 3, 4, 7 - (id == 0xA2 ? 2 : 0) );
		}

		public SmallShop( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{ 
			switch ( ItemID ^ 0x4000 )
			{
				case 0xA0: return new StoneWorkshopDeed(); 
				case 0xA2:
				default: return new MarbleWorkshopDeed();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}