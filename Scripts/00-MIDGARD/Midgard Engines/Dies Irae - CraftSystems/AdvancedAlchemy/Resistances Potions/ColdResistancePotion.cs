using System;

namespace Server.Items
{
	public class ColdResistancePotion : BaseResistancePotion
	{
		#region campi
		public override int Level{ get{ return 2;} }
		public override double PercProperFun{ get{ return 0.75;} }
		public override int DelayUse{ get{ return 10;} }
		public override ResistanceType ResType{ get{ return ResistanceType.Cold;} }
		#endregion
		
		#region costruttori
		[Constructable]
		public ColdResistancePotion( int amount ) : base( PotionEffect.ColdResistance, amount )
		{
			// Name = "Cold Elemental Resistance Potion";
			Hue = 194;
		}
		
		[Constructable]
		public ColdResistancePotion() : this(1)
		{
		}
		
		public ColdResistancePotion( Serial serial ) : base( serial )
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
