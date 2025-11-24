using System;

using Midgard.Engines.Races;

using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1450, 0x1455 )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class BoneGloves : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 3; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 4; } }
		public override int BasePoisonResistance{ get{ return 2; } }
		public override int BaseEnergyResistance{ get{ return 4; } }

		public override int InitMinHits{ get{ return 25; } }
		public override int InitMaxHits{ get{ return 30; } }
        
        public override string OldInitHits{ get{ return "1d5+25"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 55; } }
		public override int OldStrReq{ get{ return 40; } }

		// public override int OldDexBonus{ get{ return -1; } } // mod by Dies Irae

		public override int ArmorBase{ get{ return 30; } }
		public override int RevertArmorBase{ get{ return 2; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Bone; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

        public override int BlockCircle{ get{ return 1; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public BoneGloves() : base( 0x1450 )
		{
			Weight = 2.0;
		}

		public BoneGloves( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			if ( Weight == 1.0 )
				Weight = 2.0;
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

        #region mod by Dies Irae
        public override int OnCraft( int quality, bool makersMark, Mobile from, Server.Engines.Craft.CraftSystem craftSystem, Type typeRes, BaseTool tool, Server.Engines.Craft.CraftItem craftItem, int resHue )
        {
            Quality = (ArmorQuality)quality;

            if( Quality == ArmorQuality.Exceptional )
                ArmorAttributes.MageArmor = 1;

            return base.OnCraft( quality, makersMark, from, craftSystem, typeRes, tool, craftItem, resHue );
        }
        #endregion
	}
}