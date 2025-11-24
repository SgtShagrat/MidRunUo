/***************************************************************************
 *                                     BasePesticidePotion.cs
 *                            		----------------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Base class for plant pesticide potions.
 * 
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
	public class LightPesticidePotion : BasePesticidePotion
	{
	    public override PlantPotionLevel Level{ get{ return PlantPotionLevel.Light; } }
		public override double MinUseSkill{ get{ return 0.0; } }

	    [Constructable]
		public LightPesticidePotion( int amount ) : base( PotionEffect.PesticideLight, amount )
		{
			Hue = 0x847;
		}
		
		[Constructable]
		public LightPesticidePotion() : this( 1 )
		{
		}
		
		public LightPesticidePotion( Serial serial ) : base( serial )
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