using System;

using Midgard.Engines.Races;

using Server;

namespace Server.Items
{
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class MetalKiteShield : BaseShield, IDyable
	{
		public override int BasePhysicalResistance{ get{ return 0; } }
		public override int BaseFireResistance{ get{ return 0; } }
		public override int BaseColdResistance{ get{ return 0; } }
		public override int BasePoisonResistance{ get{ return 0; } }
		public override int BaseEnergyResistance{ get{ return 1; } }

		public override int InitMinHits{ get{ return 45; } }
		public override int InitMaxHits{ get{ return 60; } }

        public override string OldInitHits{ get{ return "1d13+45"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 45; } }

		public override int ArmorBase{ get{ return 16; } }

        #region mod by Dies Irae : preaos stuff
        public override int OldDexBonus { get { return -5; } }
        public override int OldStrReq { get { return 30; } } 
        #endregion

        public override int BlockCircle{ get{ return 1; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public MetalKiteShield() : base( 0x1B74 )
		{
			Weight = 7.0;
		}

		public MetalKiteShield( Serial serial ) : base(serial)
		{
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 5.0 )
				Weight = 7.0;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version
		}
	}
}
