using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13B8, 0x13B7 )]
	public class ThinLongsword : BaseSword
	{
		public override int AosStrengthReq{ get{ return 35; } }
		public override int AosMinDamage{ get{ return 15; } }
		public override int AosMaxDamage{ get{ return 16; } }
		public override int AosSpeed{ get{ return 30; } }
		public override float MlSpeed{ get{ return 3.50f; } }
        #region mod by Dies Irae
        public override int LabelNumber
        {
            get
            {
                return 1025048; // Thin long sword
            }
        }
        #endregion

		public override int OldStrengthReq{ get{ return 25; } }
		public override int OldMinDamage{ get{ return 5; } }
		public override int OldMaxDamage{ get{ return 33; } }
		public override int OldSpeed{ get{ return 35; } }

		public override int DefHitSound{ get{ return 0x237; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 90; } } // mod by Dies Irae

	    #region mod by Dies Irae
	    public override int NumDice { get { return 2; } }
	    public override int NumSides { get { return 15; } }
	    public override int DiceBonus { get { return 3; } }
	    #endregion

	    [Constructable]
		public ThinLongsword() : base( 0x13B8 )
		{
			Weight = 1.0;
		}

		public ThinLongsword( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}