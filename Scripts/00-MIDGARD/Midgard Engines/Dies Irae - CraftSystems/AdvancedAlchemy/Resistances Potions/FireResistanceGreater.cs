using System;
namespace Server.Items
{
	public class FireResistancePotionGreater : BaseResistancePotion
	{
		#region campi
		public override int Level{ get{ return 3;} }
		public override double PercProperFun{ get{ return 0.75;} }
		public override int DelayUse{ get{ return 10;} }
		public override ResistanceType ResType{ get{ return ResistanceType.Fire;} }
		#endregion
		
		#region costruttori
		[Constructable]
		public FireResistancePotionGreater( int amount ) : base( PotionEffect.FireResistanceGreater, amount )
		{
			// Name = "Greater Fire Elemental Resistance Potion";
			Hue = 38;
		}
		
		[Constructable]
		public FireResistancePotionGreater() : this(1)
		{
		}
		
		public FireResistancePotionGreater( Serial serial ) : base( serial )
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
