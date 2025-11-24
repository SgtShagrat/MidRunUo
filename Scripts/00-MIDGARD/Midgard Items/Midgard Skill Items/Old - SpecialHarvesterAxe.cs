using System;

namespace Server.Items
{
	public class SturdyHarvesterAxe : BaseAxe
	{
		public override int LabelNumber {get{	return 1064976;	}} // sturdy harvest axe
		
		public override SkillName DefSkill{ get{ return SkillName.Lumberjacking; } }
		
		public override int AosStrengthReq{ get{ return 35; } }
		public override int AosMinDamage{ get{ return 4; } }
		public override int AosMaxDamage{ get{ return 10; } }
		public override int AosSpeed{ get{ return 34; } }

		public override int OldStrengthReq{ get{ return 40; } }
		public override int OldMinDamage{ get{ return 6; } }
		public override int OldMaxDamage{ get{ return 38; } }
		public override int OldSpeed{ get{ return 30; } }

		public override int InitMinHits{ get{ return 45; } }
		public override int InitMaxHits{ get{ return 70; } }

		[Constructable]
		public SturdyHarvesterAxe() : base( 3914 )
		{
			UsesRemaining = 350;
			Weight = 4.0;
			Layer = Layer.TwoHanded;
		}

		public SturdyHarvesterAxe( Serial serial ) : base( serial )
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
			ItemID=3914;
		}
	}
}
