using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
	public class MediumFertilizerPotion : BaseFertilizerPotion
	{
	    public override PlantPotionLevel Level{ get{ return PlantPotionLevel.Medium; } }
		public override double MinUseSkill{ get{ return 60.0; } }

	    [Constructable]
		public MediumFertilizerPotion( int amount ) : base( PotionEffect.FertilizerMedium, amount )
		{
			Hue = 0x74B;
		}
		
		[Constructable]
		public MediumFertilizerPotion() : this( 1 )
		{
		}
		
		public MediumFertilizerPotion( Serial serial ) : base( serial )
		{
		}

	    #region serial-deserial
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
