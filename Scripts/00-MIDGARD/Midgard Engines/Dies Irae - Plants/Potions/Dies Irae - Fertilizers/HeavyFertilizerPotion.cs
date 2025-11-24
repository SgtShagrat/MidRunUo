using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
	public class HeavyFertilizerPotion : BaseFertilizerPotion
	{
	    public override PlantPotionLevel Level{ get{ return PlantPotionLevel.Heavy; } }
		public override double MinUseSkill{ get{ return 90.0; } }

	    [Constructable]
		public HeavyFertilizerPotion( int amount ) : base( PotionEffect.FertilizerHeavy, amount )
		{
			Hue = 0x747;
		}
		
		[Constructable]
		public HeavyFertilizerPotion() : this( 1 )
		{
		}
		
		public HeavyFertilizerPotion( Serial serial ) : base( serial )
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
