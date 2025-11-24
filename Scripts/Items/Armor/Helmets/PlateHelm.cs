using System;

using Midgard.Engines.Races;

using Server;

namespace Server.Items
{
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class PlateHelm : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 46; } } // mod by Dies Irae : pre-aos stuff
		public override int InitMaxHits{ get{ return 58; } } // mod by Dies Irae : pre-aos stuff
        
        public override string OldInitHits{ get{ return "1d13+45"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 80; } }
		public override int OldStrReq{ get{ return 40; } }

		public override int OldDexBonus{ get{ return -1; } }

        public override int ArmorBase { get { return 30 ; } } // mod by Dies Irae : pre-aos stuff

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

        public override int BlockCircle{ get{ return 1; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public PlateHelm() : base( 0x1412 )
		{
			Weight = 5.0;
		}

		public PlateHelm( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 1.0 )
				Weight = 5.0;
		}
	}
}