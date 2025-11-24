using System;

using Midgard.Engines.Races;

using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13bf, 0x13c4 )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class ChainChest : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 4; } }
		public override int BaseFireResistance{ get{ return 4; } }
		public override int BaseColdResistance{ get{ return 4; } }
		public override int BasePoisonResistance{ get{ return 1; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 46; } } // mod by Dies Irae : pre-aos stuff
		public override int InitMaxHits{ get{ return 58; } } // mod by Dies Irae : pre-aos stuff
        
        public override string OldInitHits{ get{ return "1d13+45"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 60; } }
		public override int OldStrReq{ get{ return 20; } }

		public override int OldDexBonus{ get{ return -5; } }

        public override int ArmorBase { get { return 23; } }  // mod by Dies Irae : pre-aos stuff

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Chainmail; } }

        public override int BlockCircle{ get{ return 3; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public ChainChest() : base( 0x13BF )
		{
			Weight = 7.0;
		}

		public ChainChest( Serial serial ) : base( serial )
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