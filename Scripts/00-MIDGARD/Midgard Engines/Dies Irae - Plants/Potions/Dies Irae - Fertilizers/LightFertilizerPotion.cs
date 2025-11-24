using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
	public class LightFertilizerPotion : BaseFertilizerPotion
	{
	    public override PlantPotionLevel Level{ get{ return PlantPotionLevel.Light; } }
		public override double MinUseSkill{ get{ return 0.0; } }

	    [Constructable]
		public LightFertilizerPotion( int amount ) : base( PotionEffect.FertilizerLight, amount )
		{
			Hue = 0x748;
		}
		
		[Constructable]
		public LightFertilizerPotion() : this( 1 )
		{
		}
		
		public LightFertilizerPotion( Serial serial ) : base( serial )
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
