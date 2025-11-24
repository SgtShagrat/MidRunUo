using System;

namespace Server.Items
{
	public class FireResistancePotion : BaseResistancePotion
	{
		#region campi
		public override int Level{ get{ return 2;} }
		public override double PercProperFun{ get{ return 0.75;} }
		public override int DelayUse{ get{ return 10;} }
		public override ResistanceType ResType{ get{ return ResistanceType.Fire;} }
		#endregion
		
		#region costruttori
		[Constructable]
		public FireResistancePotion( int amount ) : base( PotionEffect.FireResistance, amount )
		{
			// Name = "Fire Elemental Resistance Potion";
			Hue = 39;
		}
		
		[Constructable]
		public FireResistancePotion() : this(1)
		{
		}
		
		public FireResistancePotion( Serial serial ) : base( serial )
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
