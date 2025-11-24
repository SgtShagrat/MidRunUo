using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1c0c, 0x1c0d )]
	public class StuddedBustierArms : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 2; } }
		public override int BaseFireResistance{ get{ return 4; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 4; } }

		public override int InitMinHits{ get{ return 101; } } // mod by Dies Irae : pre-aos stuff
		public override int InitMaxHits{ get{ return 115; } } // mod by Dies Irae : pre-aos stuff
        public override string OldInitHits{ get{ return "1d9+35"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 35; } }
		public override int OldStrReq{ get{ return 25; } } // mod by Dies Irae : pre-aos stuff

		public override int ArmorBase{ get{ return 16; } } // mod by Dies Irae : pre-aos stuff

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Studded; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.Half; } }

		public override bool AllowMaleWearer{ get{ return false; } }

        public override int BlockCircle{ get{ return 5; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public StuddedBustierArms() : base( 0x1C0C )
		{
			Weight = 1.0;
		}

		public StuddedBustierArms( Serial serial ) : base( serial )
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