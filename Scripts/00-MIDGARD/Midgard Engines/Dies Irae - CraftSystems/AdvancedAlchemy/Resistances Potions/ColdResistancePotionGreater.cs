using System;

namespace Server.Items
{
	public class ColdResistancePotionGreater : BaseResistancePotion
	{
		#region campi
		public override int Level{ get{ return 3;} }
		public override double PercProperFun{ get{ return 0.75;} }
		public override int DelayUse{ get{ return 10;} }
		public override ResistanceType ResType{ get{ return ResistanceType.Cold;} }
		#endregion
		
		#region costruttori
		[Constructable]
		public ColdResistancePotionGreater( int amount ) : base( PotionEffect.ColdResistanceGreater, amount )
		{
			// Name = "Greater Cold Elemental Resistance Potion";
			Hue = 193;
		}
		
		[Constructable]
		public ColdResistancePotionGreater() : this(1)
		{
		}
		
		public ColdResistancePotionGreater( Serial serial ) : base( serial )
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
