using System;

using Midgard.Engines.Races;

using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13ee, 0x13ef )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class RingmailArms : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 3; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 1; } }
		public override int BasePoisonResistance{ get{ return 5; } }
		public override int BaseEnergyResistance{ get{ return 3; } }

		public override int InitMinHits{ get{ return 41; } } // mod by Dies Irae : pre-aos stuff
		public override int InitMaxHits{ get{ return 51; } } // mod by Dies Irae : pre-aos stuff
        
        public override string OldInitHits{ get{ return "1d11+40"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 40; } }
		public override int OldStrReq{ get{ return 20; } }

		public override int OldDexBonus{ get{ return -1; } }

        public override int ArmorBase { get { return 20; } } // mod by Dies Irae

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Ringmail; } }

        public override int BlockCircle{ get{ return 4; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public RingmailArms() : base( 0x13EE )
		{
			Weight = 15.0;
		}

		public RingmailArms( Serial serial ) : base( serial )
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
				Weight = 15.0;
		}
	}
}