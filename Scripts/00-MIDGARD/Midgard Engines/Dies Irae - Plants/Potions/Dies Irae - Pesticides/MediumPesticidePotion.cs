using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
	public class MediumPesticidePotion : BasePesticidePotion
	{
	    public override PlantPotionLevel Level{ get{ return PlantPotionLevel.Medium; } }
		public override double MinUseSkill{ get{ return 60.0; } }

	    [Constructable]
		public MediumPesticidePotion( int amount ) : base( PotionEffect.PesticideMedium, amount )
		{
			Hue = 0x849;
		}
		
		[Constructable]
		public MediumPesticidePotion() : this( 1 )
		{
		}
		
		public MediumPesticidePotion( Serial serial ) : base( serial )
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