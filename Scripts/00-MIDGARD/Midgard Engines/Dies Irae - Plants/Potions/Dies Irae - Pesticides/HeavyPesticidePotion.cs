using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
	public class HeavyPesticidePotion : BasePesticidePotion
	{
	    public override PlantPotionLevel Level{ get{ return PlantPotionLevel.Heavy; } }
		public override double MinUseSkill{ get{ return 90.0; } }

	    [Constructable]
		public HeavyPesticidePotion( int amount ) : base( PotionEffect.PesticideHeavy, amount )
		{
			Hue = 0x852;
		}
		
		[Constructable]
		public HeavyPesticidePotion() : this( 1 )
		{
		}
		
		public HeavyPesticidePotion( Serial serial ) : base( serial )
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