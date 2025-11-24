/***************************************************************************
 *                               FarmablePlants.cs
 *
 *   begin                : 07-04-2012
 *   author               : Arlas		
 *
 ***************************************************************************/

using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class LavandaPlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3210;
		}

		public override Item GetCropObject()
		{
			return new Lavanda();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public LavandaPlant() : base( GetCropID() )
		{
			Name = "Lavanda";
			Hue = 1658;
			//Stackable = true;
		}

		public LavandaPlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class FioreBluDiMontagnaPlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3212;
		}

		public override Item GetCropObject()
		{
			return new FioreBluDiMontagna();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public FioreBluDiMontagnaPlant() : base( GetCropID() )
		{
			Name = "Fiore Blu di Montagna";
			Hue = 2519;
			//Stackable = true;
		}

		public FioreBluDiMontagnaPlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class FioreRossoDiMontagnaPlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 9036;
		}

		public override Item GetCropObject()
		{
			return new FioreRossoDiMontagna();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public FioreRossoDiMontagnaPlant() : base( GetCropID() )
		{
			Name = "Fiore Rosso di Montagna";
			Hue = 2949;
			//Stackable = true;
		}

		public FioreRossoDiMontagnaPlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class FioreViolaDiMontagnaPlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3208;
		}

		public override Item GetCropObject()
		{
			return new FioreViolaDiMontagna();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public FioreViolaDiMontagnaPlant() : base( GetCropID() )
		{
			Name = "Fiore Viola di Montagna";
			Hue = 2521;
			//Stackable = true;
		}

		public FioreViolaDiMontagnaPlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class AtropaBelladonnaPlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 14489;
		}

		public override Item GetCropObject()
		{
			return new FioreDiBelladonna();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public AtropaBelladonnaPlant() : base( GetCropID() )
		{
			Name = "Atropa Belladonna";
			Hue = 1561;
			//Stackable = true;
		}

		public AtropaBelladonnaPlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class BaccanevePlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3213;
		}

		public override Item GetCropObject()
		{
			return new Baccaneve();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public BaccanevePlant() : base( GetCropID() )
		{
			Name = "Cespuglio di Baccaneve";
			Hue = 1380;
			//Stackable = true;
		}

		public BaccanevePlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class RanuncoloPlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 14486;
		}

		public override Item GetCropObject()
		{
			return new Ranuncolo();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public RanuncoloPlant() : base( GetCropID() )
		{
			Name = "Ranuncolo";
			//Hue = 1380;
			//Stackable = true;
		}

		public RanuncoloPlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class CardoPlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 731;
		}

		public override Item GetCropObject()
		{
			return new Cardo();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public CardoPlant() : base( GetCropID() )
		{
			Name = "Cardo";
			Hue = 1784;
			//Stackable = true;
		}

		public CardoPlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class GineproPlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 6811;
		}

		public override Item GetCropObject()
		{
			return new Ginepro();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public GineproPlant() : base( GetCropID() )
		{
			Name = "Cespuglio di Ginepro";
			Hue = 1295;
			//Stackable = true;
		}

		public GineproPlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class CotoneDellaTundraPlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 6377;
		}

		public override Item GetCropObject()
		{
			return new CotoneDellaTundra();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public CotoneDellaTundraPlant() : base( GetCropID() )
		{
			Name = "Cotone della Tundra";
			Hue = 2548;
			//Stackable = true;
		}

		public CotoneDellaTundraPlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class CampanulaMortalePlant : FarmableCrop
	{
		public static int GetCropID()
		{
			return 9036;
		}

		public override Item GetCropObject()
		{
			return new CampanulaMortale();
		}

		public override int GetPickedID()
		{
			return Utility.Random( 3502, 2 );
		}

		[Constructable]
		public CampanulaMortalePlant() : base( GetCropID() )
		{
			Name = "Campanula Mortale";
			//Hue = 2548;
			//Stackable = true;
		}

		public CampanulaMortalePlant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}