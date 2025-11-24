using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    /// <summary>
    /// Small Dragon Ship	12	400 Stones	12,500 GP	Similar to the small standard ship in size and performance, however the prominent dragon head on the prow shows that this ship is built for war.
    /// </summary>
	public class SmallDragonBoat : BaseBoat
	{
        #region mod by Dies Irae
        public override int HitsMax { get { return 10000; } }
        public override int ResistFire { get { return 40; } }
        public override int ResistPhysical { get { return 50; } }

        public override int BoatPrice { get { return 12500; } }

        public override string Description { get { return "Similar to the small standard ship in size and performance,"+
            "however the prominent dragon head on the prow shows that this ship is built for war."; } }

        public override int HoldSize { get { return 400; } }
        #endregion

		public override int NorthID{ get{ return 0x4004; } }
		public override int  EastID{ get{ return 0x4005; } }
		public override int SouthID{ get{ return 0x4006; } }
		public override int  WestID{ get{ return 0x4007; } }

		public override int HoldDistance{ get{ return 4; } }
		public override int TillerManDistance{ get{ return -4; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, 0 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, 0 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 1, 3 ); } }

		public override BaseDockedBoat DockedBoat{ get{ return new SmallDockedDragonBoat( this ); } }

		[Constructable]
		public SmallDragonBoat()
		{
		}

		public SmallDragonBoat( Serial serial ) : base( serial )
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

	public class SmallDragonBoatDeed : BaseBoatDeed
	{
		public override int LabelNumber{ get{ return 1041206; } } // small dragon ship deed
		public override BaseBoat Boat{ get{ return new SmallDragonBoat(); } }

		[Constructable]
		public SmallDragonBoatDeed() : base( 0x4004, Point3D.Zero )
		{
		}

		public SmallDragonBoatDeed( Serial serial ) : base( serial )
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

	public class SmallDockedDragonBoat : BaseDockedBoat
	{
		public override BaseBoat Boat{ get{ return new SmallDragonBoat(); } }

		public SmallDockedDragonBoat( BaseBoat boat ) : base( 0x4004, Point3D.Zero, boat )
		{
		}

		public SmallDockedDragonBoat( Serial serial ) : base( serial )
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