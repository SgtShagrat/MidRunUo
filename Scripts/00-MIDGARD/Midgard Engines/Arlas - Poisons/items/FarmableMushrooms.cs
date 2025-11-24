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
	public class CoronaInsanguinataMushroom : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3344;
		}

		public override Item GetCropObject()
		{
			return new CoronaInsanguinata();
		}

		public override int GetPickedID()
		{
			return 0x0913;
		}

		[Constructable]
		public CoronaInsanguinataMushroom() : base( GetCropID() )
		{
			Name = "Corona Insanguinata";
			Hue = 2019;
			//Stackable = true;
		}

		public CoronaInsanguinataMushroom( Serial serial ) : base( serial )
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

	public class AmanitaMuscariaMushroom : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3340;
		}

		public override Item GetCropObject()
		{
			return new AmanitaMuscaria();
		}

		public override int GetPickedID()
		{
			return 0x0913;
		}

		[Constructable]
		public AmanitaMuscariaMushroom() : base( GetCropID() )
		{
			Name = "Amanita Muscaria";
			Hue = 2561;
			//Stackable = true;
		}

		public AmanitaMuscariaMushroom( Serial serial ) : base( serial )
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

	public class CeppoDiFollettoMushroom : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3347;
		}

		public override Item GetCropObject()
		{
			return new CeppoDiFolletto();
		}

		public override int GetPickedID()
		{
			return 0x0913;
		}

		[Constructable]
		public CeppoDiFollettoMushroom() : base( GetCropID() )
		{
			Name = "Ceppo di Folletto";
			Hue = 1300;
			//Stackable = true;
		}

		public CeppoDiFollettoMushroom( Serial serial ) : base( serial )
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

	public class FungoBiancoMushroom : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3348;
		}

		public override Item GetCropObject()
		{
			return new FungoBianco();
		}

		public override int GetPickedID()
		{
			return 0x0913;
		}

		[Constructable]
		public FungoBiancoMushroom() : base( GetCropID() )
		{
			Name = "Fungo Bianco";
			Hue = 2442;
			//Stackable = true;
		}

		public FungoBiancoMushroom( Serial serial ) : base( serial )
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

	public class FungoLuminosoMushroom : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3346;
		}

		public override Item GetCropObject()
		{
			return new FungoLuminoso();
		}

		public override int GetPickedID()
		{
			return 0x0913;
		}

		[Constructable]
		public FungoLuminosoMushroom() : base( GetCropID() )
		{
			Name = "Fungo Luminoso";
			Hue = 1494;
			//Stackable = true;
		}

		public FungoLuminosoMushroom( Serial serial ) : base( serial )
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

	public class MoraTapinellaMushroom : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3342;
		}

		public override Item GetCropObject()
		{
			return new MoraTapinella();
		}

		public override int GetPickedID()
		{
			return 0x0913;
		}

		[Constructable]
		public MoraTapinellaMushroom() : base( GetCropID() )
		{
			Name = "Mora Tapinella";
			Hue = 1493;
			//Stackable = true;
		}

		public MoraTapinellaMushroom( Serial serial ) : base( serial )
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