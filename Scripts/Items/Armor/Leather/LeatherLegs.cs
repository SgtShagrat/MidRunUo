using System;

using Midgard.Engines.Races;

using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13cb, 0x13d2 )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class LeatherLegs : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 2; } }
		public override int BaseFireResistance{ get{ return 4; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 3; } }

		public override int InitMinHits{ get{ return 31; } } // mod by Dies Irae : pre-aos stuff
		public override int InitMaxHits{ get{ return 37; } } // mod by Dies Irae : pre-aos stuff
        
        public override string OldInitHits{ get{ return "1d7+30"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 20; } }
		public override int OldStrReq{ get{ return 10; } }

		public override int ArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

        public override int BlockCircle{ get{ return 7; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public LeatherLegs() : base( 0x13CB )
		{
			Weight = 4.0;
		}

		public LeatherLegs( Serial serial ) : base( serial )
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