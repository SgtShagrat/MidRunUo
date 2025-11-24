using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
	[FlipableAttribute( 0x13FD, 0x13FC )]
	public class LightCrossbow : BaseRanged
	{
		public override string DefaultName { get { return "Light Crossbow"; } }

		public override int EffectID { get { return 0x1BFE; } }
		public override Type AmmoType { get { return typeof( Bolt ); } }

		public override int OldStrengthReq { get { return 10; } }
		public override int OldSpeed { get { return 30; } }

		public override int DefMinRange{ get { return 1 + OldMaterialMinRangeBonus; } }//mod by Arlas
		public override int DefMaxRange { get { return 7 + OldMaterialMaxRangeBonus; } }

		public override int InitMinHits { get { return 31; } }
		public override int InitMaxHits { get { return 40; } }

		public override int NumDice { get { return 4; } }
		public override int NumSides { get { return 8; } }
		public override int DiceBonus { get { return 2; } }

		[Constructable]
		public LightCrossbow() : base( 0x13FD )
		{
			Weight = 8.0;
			Layer = Layer.TwoHanded;
		}

		#region serialization
		public LightCrossbow( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		#endregion
	}
}