using System;

using Midgard.Engines.Races;

using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2641, 0x2642 )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class DragonChest : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 3; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 3; } }

		public override int InitMinHits{ get{ return 55; } }
		public override int InitMaxHits{ get{ return 75; } }

		public override int AosStrReq{ get{ return 75; } }
		public override int OldStrReq{ get{ return 60; } }

		public override int OldDexBonus{ get{ return /* -8 */ -5; } }

		public override int ArmorBase{ get{ return /* 40 */ 33; } } // mod by Dies Irae : pre-aos stuff

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Dragon; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RedScales; } }

        public override int BlockCircle{ get{ return 1; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public DragonChest() : base( 0x2641 )
		{
			Weight = 10.0;
		}

		public DragonChest( Serial serial ) : base( serial )
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