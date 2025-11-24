using System;

namespace Server.Items
{
	public class ColdResistancePotionLesser : BaseResistancePotion
	{
		#region campi
		public override int Level{ get{ return 1;} }
		public override double PercProperFun{ get{ return 0.75;} }
		public override int DelayUse{ get{ return 10;} }
		public override ResistanceType ResType{ get{ return ResistanceType.Cold;} }
		#endregion
		
		#region costruttori
		[Constructable]
		public ColdResistancePotionLesser( int amount ) : base( PotionEffect.ColdResistanceLesser, amount )
		{
			// Name = "Lesser Cold Elemental Resistance Potion";
			Hue = 195;
		}
		
		[Constructable]
		public ColdResistancePotionLesser() : this(1)
		{
		}
		
		public ColdResistancePotionLesser( Serial serial ) : base( serial )
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
