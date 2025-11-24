using System;

namespace Server.Items
{
	public class FireResistancePotionLesser : BaseResistancePotion
	{
		#region campi
		public override int Level{ get{ return 1;} }
		public override double PercProperFun{ get{ return 0.75;} }
		public override int DelayUse{ get{ return 10;} }
		public override ResistanceType ResType{ get{ return ResistanceType.Fire;} }
		#endregion
		
		#region costruttori
		[Constructable]
		public FireResistancePotionLesser( int amount ) : base( PotionEffect.FireResistanceLesser, amount )
		{
			// Name = "Lesser Fire Elemental Resistance Potion";
			Hue = 40;
		}
		
		[Constructable]
		public FireResistancePotionLesser() : this(1)
		{
		}
		
		public FireResistancePotionLesser( Serial serial ) : base( serial )
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
