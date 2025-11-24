using System;

namespace Server.Items
{
	public class EnergyResistancePotion : BaseResistancePotion
	{
		#region campi
		public override int Level{ get{ return 2;} }
		public override double PercProperFun{ get{ return 0.75;} }
		public override int DelayUse{ get{ return 10;} }
		public override ResistanceType ResType{ get{ return ResistanceType.Energy;} }
		#endregion
		
		#region costruttori
		[Constructable]
		public EnergyResistancePotion( int amount ) : base( PotionEffect.EnergyResistance, amount )
		{
			// Name = "Energy Elemental Resistance Potion";
			Hue = 2140;
		}
		
		[Constructable]
		public EnergyResistancePotion() : this(1)
		{
		}
		
		public EnergyResistancePotion( Serial serial ) : base( serial )
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
