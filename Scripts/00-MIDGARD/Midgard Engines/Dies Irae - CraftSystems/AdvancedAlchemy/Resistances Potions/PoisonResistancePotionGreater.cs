using System;

namespace Server.Items
{
	public class PoisonResistancePotionGreater : BaseResistancePotion
	{
		#region campi
		public override int Level{ get{ return 3;} }
		public override double PercProperFun{ get{ return 0.75;} }
		public override int DelayUse{ get{ return 10;} }
		public override ResistanceType ResType{ get{ return ResistanceType.Poison;} }
		#endregion
		
		#region costruttori
		[Constructable]
		public PoisonResistancePotionGreater( int amount ) : base( PotionEffect.PoisonResistance, amount )
		{
			// Name = "Greater Poison Elemental Resistance Potion";
			Hue = 508;
		}
		
		[Constructable]
		public PoisonResistancePotionGreater() : this(1)
		{
		}
		
		public PoisonResistancePotionGreater( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region serial deserial
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
		#endregion
	}
}
