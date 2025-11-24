using System;

using Midgard.Engines.Races;

using Server;

namespace Server.Items
{
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class MetalShield : BaseShield
	{
		public override int BasePhysicalResistance{ get{ return 0; } }
		public override int BaseFireResistance{ get{ return 1; } }
		public override int BaseColdResistance{ get{ return 0; } }
		public override int BasePoisonResistance{ get{ return 0; } }
		public override int BaseEnergyResistance{ get{ return 0; } }

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }
        
        public override string OldInitHits{ get{ return "1d9+35"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 45; } }

		public override int ArmorBase{ get{ return 9; } } // mod by Dies Irae

        #region mod by Dies Irae : preaos stuff
        public override int OldDexBonus { get { return -3; } } // ?

        public override int OldStrReq { get { return 10; } }

        public override int BlockCircle{ get{ return 1; } } // mod by Dies Irae : pre-aos stuff
        #endregion

		[Constructable]
		public MetalShield() : base( 0x1B7B )
		{
			Weight = 6.0;
		}

		public MetalShield( Serial serial ) : base(serial)
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
