using System;

namespace Server.Items
{
	public class ResistancePotionsAlchemicalBag : Bag
	{
		#region metodi
		public override int LabelNumber { get { return 1064034; } } // resistance potions alchemical bag
		#endregion

		#region costruttori
		[Constructable]
		public ResistancePotionsAlchemicalBag() : this( 10 )
		{
			Movable = true;
			Hue = 1151;
		}

		[Constructable]
		public ResistancePotionsAlchemicalBag( int amount )
		{
			DropItem( new FireResistancePotionLesser( amount ) );
            DropItem( new ColdResistancePotionLesser( amount ) );
            DropItem( new PoisonResistancePotionLesser( amount ) );
            DropItem( new EnergyResistancePotionLesser( amount ) );
            
            DropItem( new FireResistancePotion( amount ) );
            DropItem( new ColdResistancePotion( amount ) );
            DropItem( new PoisonResistancePotion( amount ) );
            DropItem( new EnergyResistancePotion( amount ) );
            
            DropItem( new FireResistancePotionGreater( amount ) );
            DropItem( new ColdResistancePotionGreater( amount ) );
            DropItem( new PoisonResistancePotionGreater( amount ) );
            DropItem( new EnergyResistancePotionGreater( amount ) );
		}

		public ResistancePotionsAlchemicalBag( Serial serial ) : base( serial )
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
