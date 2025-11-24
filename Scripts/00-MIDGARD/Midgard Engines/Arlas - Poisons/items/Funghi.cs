/***************************************************************************
 *                               FarmablePlants.cs
 *
 *   begin                : 07-04-2012
 *   author               : Arlas		
 *
 *	CoronaInsanguinata
 *	AmanitaMuscaria
 *	CeppoDiFolletto
 *	FungoBianco
 *	FungoLuminoso
 *	MoraTapinella
 *
 ***************************************************************************/

using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class CoronaInsanguinata : BaseReagent
	{
		public override int PotionType{get{ return 2; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 5; }}//1-6

		[Constructable]
		public CoronaInsanguinata() : this( 1 )
		{
		}

		[Constructable]
		public CoronaInsanguinata( int amount ) : base( 0xF25, amount )
		{
			Name = "Corona Insanguinata";
			Hue = 2019;
			Stackable = true;
		}

		public CoronaInsanguinata( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "coron%ae% insanguinat%ae%" : "bleeding crown%s%", Amount, from.Language ) );
		}

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
	}

	public class AmanitaMuscaria : BaseReagent
	{
		public override int PotionType{get{ return 3; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 2; }}//1-6

		[Constructable]
		public AmanitaMuscaria() : this( 1 )
		{
		}

		[Constructable]
		public AmanitaMuscaria( int amount ) : base( 0xD16, amount )
		{
			Name = "Amanita Muscaria";
			Hue = 2561;
			Stackable = true;
		}

		public AmanitaMuscaria( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "amanit%ae% muscari%ae%" : "amanita muscaria%s%", Amount, from.Language ) );
		}

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
	}

	public class CeppoDiFolletto : BaseReagent
	{
		public override int PotionType{get{ return 5; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 4; }}//1-6

		[Constructable]
		public CeppoDiFolletto() : this( 1 )
		{
		}

		[Constructable]
		public CeppoDiFolletto( int amount ) : base( 0xD17, amount )
		{
			Name = "Ceppo di Folletto";
			Hue = 1300;
			Stackable = true;
		}

		public CeppoDiFolletto( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "cepp%oi% di folletto" : "imp stool%s%", Amount, from.Language ) );
		}

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
	}

	public class FungoBianco : BaseReagent
	{
		public override int PotionType{get{ return 4; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 3; }}//1-6

		[Constructable]
		public FungoBianco() : this( 1 )
		{
		}

		[Constructable]
		public FungoBianco( int amount ) : base( 0xD19, amount )
		{
			Name = "Fungo Bianco";
			Hue = 2442;
			Stackable = true;
		}

		public FungoBianco( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "fung%ohi% bianc%ohi%" : "white cap%s%", Amount, from.Language ) );
		}

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
	}

	public class FungoLuminoso : BaseReagent
	{
		public override int PotionType{get{ return 5; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 5; }}//1-6

		[Constructable]
		public FungoLuminoso() : this( 1 )
		{
		}

		[Constructable]
		public FungoLuminoso( int amount ) : base( 0xD18, amount )
		{
			Name = "Fungo Luminoso";
			Hue = 1494;
			Stackable = true;
		}

		public FungoLuminoso( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "fung%ohi% luminos%oi%" : "glowing mushroom%s%", Amount, from.Language ) );
		}

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
	}

	public class MoraTapinella : BaseReagent
	{
		public override int PotionType{get{ return 1; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 5; }}//1-6

		[Constructable]
		public MoraTapinella() : this( 1 )
		{
		}

		[Constructable]
		public MoraTapinella( int amount ) : base( 0xD0F, amount )
		{
			Name = "Mora Tapinella";
			Hue = 1493;
			Stackable = true;
		}

		public MoraTapinella( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "mor%ae% tapinell%ae%" : "mora tapinella%s%", Amount, from.Language ) );
		}

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
	}
}