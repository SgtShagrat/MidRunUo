/***************************************************************************
 *                                     BaseFertilizerPotion.cs
 *                            		----------------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Base class for plant fertilize potions.
 * 
 ***************************************************************************/
 
using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
	public abstract class BaseFertilizerPotion : BasePlantPotion
	{
	    public override int LabelNumber { get { return 1065764; } } // plant fertilizer

	    public BaseFertilizerPotion( PotionEffect effect, int amount ) : base( effect, amount )
		{
		}
		
		public BaseFertilizerPotion( Serial serial ) : base( serial )
		{
		}

	    public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if( Level != PlantPotionLevel.None )
				list.Add( 1065765, Enum.GetName( typeof(PlantPotionLevel), Level) ); // compund level: ~1_LEVEL~
		}

		public override void PotionAreaEffect( Point3D p, Map map )
		{
			if ( map.CanFit( p, 12, true, false ) )
			{
				int renderMode = Utility.RandomList( 0, 2, 3, 4, 5, 7 );
				Effects.SendLocationEffect( p, map, 0x3779, 16, Utility.RandomRedHue(), renderMode );
			}
		}
		
		public override void DoPlantEffect( BasePlant plant )
		{
			plant.Fertilize( Level );
		}
		
		public static double GetFertilizerDuration( PlantPotionLevel level )
		{
			double duration;
			
			switch( level )
			{
				case PlantPotionLevel.Light: 	duration = 2.0; 	break;
				case PlantPotionLevel.Medium:	duration = 3.0; 	break;	
				case PlantPotionLevel.Heavy: 	duration = 4.0; 	break;
				default: 						duration = 2.0; 	break;
			}

			return duration;				
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