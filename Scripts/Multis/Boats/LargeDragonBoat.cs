using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    /// <summary>
    /// Large Dragon Ship	18	400 Stones	15,900 GP	Similar to the large standard ship in size and performance, however the prominent dragon head on the prow shows that this ship is built for war. If you want a ship to bring your army to bear on an enemy, this is the ship for you.
    /// </summary>
	public class LargeDragonBoat : BaseBoat
	{
        #region mod by Dies Irae
        public override int HitsMax { get { return 27500; } }
        public override int ResistFire { get { return 60; } }
        public override int ResistPhysical { get { return 70; } }

        public override int BoatPrice { get { return 14200; } }

        public override string Description
        {
            get
            {
                return "Similar to the large standard ship in size and performance, " +
                    "however the prominent dragon head on the prow shows that this ship is built for war." +
                    "If you want a ship to bring your army to bear on an enemy, this is the ship for you.";
            }
        }
        
        public override int HoldSize { get { return 400; } }
        #endregion

		public override int NorthID{ get{ return 0x4014; } }
		public override int  EastID{ get{ return 0x4015; } }
		public override int SouthID{ get{ return 0x4016; } }
		public override int  WestID{ get{ return 0x4017; } }

		public override int HoldDistance{ get{ return 5; } }
		public override int TillerManDistance{ get{ return -5; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, -1 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, -1 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 0, 3 ); } }

		public override BaseDockedBoat DockedBoat{ get{ return new LargeDockedDragonBoat( this ); } }

		[Constructable]
		public LargeDragonBoat()
		{
		}

		public LargeDragonBoat( Serial serial ) : base( serial )
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

	public class LargeDragonBoatDeed : BaseBoatDeed
	{
		public override int LabelNumber{ get{ return 1041210; } }// large dragon ship deed
		public override BaseBoat Boat{ get{ return new LargeDragonBoat(); } }

		[Constructable]
		public LargeDragonBoatDeed() : base( 0x4014, new Point3D( 0, -1, 0 ) )
		{
		}

		public LargeDragonBoatDeed( Serial serial ) : base( serial )
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

	public class LargeDockedDragonBoat : BaseDockedBoat
	{
		public override BaseBoat Boat{ get{ return new LargeDragonBoat(); } }

		public LargeDockedDragonBoat( BaseBoat boat ) : base( 0x4014, new Point3D( 0, -1, 0 ), boat )
		{
		}

		public LargeDockedDragonBoat( Serial serial ) : base( serial )
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