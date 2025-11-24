using System;

using Midgard.Engines.Races;

using Server;

namespace Server.Items
{
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class HeaterShield : BaseShield
	{
		public override int BasePhysicalResistance{ get{ return 0; } }
		public override int BaseFireResistance{ get{ return 1; } }
		public override int BaseColdResistance{ get{ return 0; } }
		public override int BasePoisonResistance{ get{ return 0; } }
		public override int BaseEnergyResistance{ get{ return 0; } }

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }
        
        public override string OldInitHits{ get{ return "1d7+30"; } } // mod by Dies Irae : pre-aos stuff

		public override int AosStrReq{ get{ return 90; } }

		public override int ArmorBase{ get{ return 23; } }

        #region mod by Dies Irae : preaos stuff
        public override int OldDexBonus { get { return -7; } }
        public override int OldStrReq { get { return 30; } } 
        #endregion

        public override int BlockCircle{ get{ return 1; } } // mod by Dies Irae : pre-aos stuff

        [Constructable]
		public HeaterShield() : base( 0x1B76 )
		{
			Weight = 8.0;
		}

		public HeaterShield( Serial serial ) : base(serial)
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
