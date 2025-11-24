using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    /// <summary>
    /// Small Ship            	12	400 Stones	12,500 GP	
    /// </summary>
	public class SmallBoat : BaseBoat
	{
	    #region mod by Dies Irae
	    public override int HitsMax { get { return 10000; } }
	    public override int ResistFire { get { return 50; } }
	    public override int ResistPhysical { get { return 30; } }

        public override int BoatPrice { get { return 12500; } }

        public override string Description { get { return "This is the smallest ship available, it is highly manoeuvrable, especially in crowded ports." +
            "A great ship if you want to travel alone or with a few friends, but with any more than a few it is quite a tight fit." +
            "However if it is just you and your cargo this makes a fine ship."; } }

        public override int HoldSize { get { return 400; } }
	    #endregion

		public override int NorthID{ get{ return 0x4000; } }
		public override int  EastID{ get{ return 0x4001; } }
		public override int SouthID{ get{ return 0x4002; } }
		public override int  WestID{ get{ return 0x4003; } }

		public override int HoldDistance{ get{ return 4; } }
		public override int TillerManDistance{ get{ return -4; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, 0 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, 0 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 1, 3 ); } }

		public override BaseDockedBoat DockedBoat{ get{ return new SmallDockedBoat( this ); } }

		[Constructable]
		public SmallBoat()
		{
		}

		public SmallBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}

	public class SmallBoatDeed : BaseBoatDeed
	{
		public override int LabelNumber{ get{ return 1041205; } } // small ship deed
		public override BaseBoat Boat{ get{ return new SmallBoat(); } }

		[Constructable]
		public SmallBoatDeed() : base( 0x4000, Point3D.Zero )
		{
		}

		public SmallBoatDeed( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}

	public class SmallDockedBoat : BaseDockedBoat
	{
		public override BaseBoat Boat{ get{ return new SmallBoat(); } }

		public SmallDockedBoat( BaseBoat boat ) : base( 0x4000, Point3D.Zero, boat )
		{
		}

		public SmallDockedBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}
}