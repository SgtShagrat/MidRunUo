/***************************************************************************
 *                               FarmablePlants.cs
 *
 *   begin                : 07-04-2012
 *   author               : Arlas		
 *
 *	Lavanda
 *	FioreBluDiMontagna
 *	FioreRossoDiMontagna
 *	FioreViolaDiMontagna
 *	FioreDiBelladonna
 *	Baccaneve
 *	Ranuncolo
 *	Cardo
 *	Ginepro
 *	CotoneDellaTundra
 *	CampanulaMortale
 ***************************************************************************/

using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class Lavanda : BaseReagent
	{
		public override int PotionType{get{ return 1; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 4; }}//1-6

		[Constructable]
		public Lavanda() : this( 1 )
		{
		}

		[Constructable]
		public Lavanda( int amount ) : base( 0xF88, amount )
		{
			Name = "Lavanda";
			Hue = 1658;
			Stackable = true;
		}

		public Lavanda( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "fior%ei% di lavanda" : "lavanda flower%s%", Amount, from.Language ) );
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

	public class FioreBluDiMontagna : BaseReagent
	{
		public override int PotionType{get{ return 1; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 2; }}//1-6

		[Constructable]
		public FioreBluDiMontagna() : this( 1 )
		{
		}

		[Constructable]
		public FioreBluDiMontagna( int amount ) : base( 0x386F, amount )
		{
			Name = "Fiore Blu di Montagna";
			Hue = 2519;
			Stackable = true;
		}

		public FioreBluDiMontagna( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "fior%ei% blu di montagna" : "blue mountain flower%s%", Amount, from.Language ) );
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

	public class FioreRossoDiMontagna : BaseReagent
	{
		public override int PotionType{get{ return 2; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 2; }}//1-6

		[Constructable]
		public FioreRossoDiMontagna() : this( 1 )
		{
		}

		[Constructable]
		public FioreRossoDiMontagna( int amount ) : base( 0x386F, amount )
		{
			Name = "Fiore Rosso di Montagna";
			Hue = 2949;
			Stackable = true;
		}

		public FioreRossoDiMontagna( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "fior%ei% ross%oi% di montagna" : "red mountain flower%s%", Amount, from.Language ) );
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

	public class FioreViolaDiMontagna : BaseReagent
	{
		public override int PotionType{get{ return 5; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 2; }}//1-6

		[Constructable]
		public FioreViolaDiMontagna() : this( 1 )
		{
		}

		[Constructable]
		public FioreViolaDiMontagna( int amount ) : base( 0x386F, amount )
		{
			Name = "Fiore Viola di Montagna";
			Hue = 2521;
			Stackable = true;
		}

		public FioreViolaDiMontagna( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "fior%ei% viola di montagna" : "purple mountain flower%s%", Amount, from.Language ) );
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

	public class FioreDiBelladonna : BaseReagent
	{
		public override int PotionType{get{ return 5; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 3; }}//1-6

		[Constructable]
		public FioreDiBelladonna() : this( 1 )
		{
		}

		[Constructable]
		public FioreDiBelladonna( int amount ) : base( 0x386E, amount )
		{
			Name = "Fiore di Belladonna";
			Hue = 1561;
			Stackable = true;
		}

		public FioreDiBelladonna( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "fior%ei% di belladonna" : "nightshade flower%s%", Amount, from.Language ) );
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

	public class Baccaneve : BaseReagent
	{
		public override int PotionType{get{ return 3; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 3; }}//1-6

		[Constructable]
		public Baccaneve() : this( 1 )
		{
		}

		[Constructable]
		public Baccaneve( int amount ) : base( 0x3893, amount )
		{
			Name = "Fiore di Baccaneve";
			Hue = 1380;
			Stackable = true;
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "fior%ei% di baccaneve" : "snowberry flower%s%", Amount, from.Language ) );
		}

		public Baccaneve( Serial serial ) : base( serial )
		{
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

	public class Ranuncolo : BaseReagent
	{
		public override int PotionType{get{ return 3; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 5; }}//1-6

		[Constructable]
		public Ranuncolo() : this( 1 )
		{
		}

		[Constructable]
		public Ranuncolo( int amount ) : base( 0x3893, amount )
		{
			Name = "Ranuncolo";
			//Hue = 1380;
			Stackable = true;
		}

		public Ranuncolo( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "ranuncol%oi%" : "blisterwort%s%", Amount, from.Language ) );
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

	public class Cardo : BaseReagent
	{
		public override int PotionType{get{ return 3; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 4; }}//1-6

		[Constructable]
		public Cardo() : this( 1 )
		{
		}

		[Constructable]
		public Cardo( int amount ) : base( 0x18E3, amount )
		{
			Name = "Cardo";
			Hue = 1784;
			Stackable = true;
		}

		public Cardo( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "card%oi%" : "thistle%s%", Amount, from.Language ) );
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

	public class Ginepro : BaseReagent
	{
		public override int PotionType{get{ return 1; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 3; }}//1-6

		[Constructable]
		public Ginepro() : this( 1 )
		{
		}

		[Constructable]
		public Ginepro( int amount ) : base( 0x1AA2, amount )
		{
			Name = "Bacca di Ginepro";
			Hue = 1295;
			Stackable = true;
		}

		public Ginepro( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "bacc%ahe% di ginepro" : "juniper berry%s%", Amount, from.Language ) );
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

	public class CotoneDellaTundra : BaseReagent
	{
		public override int PotionType{get{ return 4; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 5; }}//1-6

		[Constructable]
		public CotoneDellaTundra() : this( 1 )
		{
		}

		[Constructable]
		public CotoneDellaTundra( int amount ) : base( 0xDF9, amount )
		{
			Name = "Cotone della Tundra";
			//Hue = 1295;
			Stackable = true;
		}

		public CotoneDellaTundra( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "coton%ei% della tundra" : "tundra cotton%s%", Amount, from.Language ) );
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

	public class CampanulaMortale : BaseReagent
	{
		public override int PotionType{get{ return 2; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 3; }}//1-6

		[Constructable]
		public CampanulaMortale() : this( 1 )
		{
		}

		[Constructable]
		public CampanulaMortale( int amount ) : base( 0x3893, amount )
		{
			Name = "Fiore di Campanula Mortale";
			Hue = 2521;
			Stackable = true;
		}

		public CampanulaMortale( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "fior%ei% di campanula mortale" : "deathbell flower%s%", Amount, from.Language ) );
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