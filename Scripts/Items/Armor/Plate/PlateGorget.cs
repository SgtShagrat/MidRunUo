using System;

using Midgard.Engines.Races;

using Server.Items;

namespace Server.Items
{
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class PlateGorget : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 51; } } // mod by Dies Irae : pre-aos stuff
		public override int InitMaxHits{ get{ return 65; } }
        
        public override string OldInitHits{ get{ return "1d15+50"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 45; } }
		public override int OldStrReq{ get{ return 30; } }

		public override int OldDexBonus{ get{ return -1; } }

        public override int ArmorBase { get { return 30; } } // mod by Dies Irae : pre-aos stuff

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

        public override int BlockCircle{ get{ return 1; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public PlateGorget() : base( 0x1413 )
		{
			Weight = 2.0;
		}

		public PlateGorget( Serial serial ) : base( serial )
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
		}
	}
}