using System;

using Midgard.Engines.Races;

using Server;

namespace Server.Items
{
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class BronzeShield : BaseShield
	{
		public override int BasePhysicalResistance{ get{ return 0; } }
		public override int BaseFireResistance{ get{ return 0; } }
		public override int BaseColdResistance{ get{ return 1; } }
		public override int BasePoisonResistance{ get{ return 0; } }
		public override int BaseEnergyResistance{ get{ return 0; } }

		public override int InitMinHits{ get{ return 25; } }
		public override int InitMaxHits{ get{ return 30; } }
        
        public override string OldInitHits{ get{ return "1d5+25"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 35; } }

		public override int ArmorBase{ get{ return 10; } }

        public override int OldDexBonus { get { return -1; } } // mod by Dies Irae : pre-aos stuff
        public override int OldStrReq { get { return 20; } }  // mod by Dies Irae : pre-aos stuff

        public override int BlockCircle{ get{ return 1; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public BronzeShield() : base( 0x1B72 )
		{
			Weight = 6.0;
		}

		public BronzeShield( Serial serial ) : base(serial)
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

			writer.Write( (int)0 );//version
		}
	}
}
