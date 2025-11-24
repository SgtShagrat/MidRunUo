/***************************************************************************
 *                               Ingredienti.cs
 *
 *   begin                : 07-04-2012
 *   author               : Arlas		
 *	PVM BeccoDiFalco
 *	PVP CarneUmana
 *	PVM CodaDiTopo
 *	PVM ArtigliDiOrso
 *	PVP PolvereDiVampiro
 *	PVM DitoDiGigante
 *	- Lichene
 *	RadiceCanina
 *	UovoDiRagno
 *	PVP AliDiFata
 *	Granchio
 *	PVP Cuore
 *	PVP OrecchieDiElfo
 ***************************************************************************/

using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class PoisonIngredientsBag : Bag
	{
		public override string DefaultName
		{
			get { return "an ingredient bag"; }
		}

		[Constructable]
		public PoisonIngredientsBag() : this( 50 )
		{
			Movable = true;
			Hue = 0x250;
		}

		[Constructable]
		public PoisonIngredientsBag( int amount )
		{
			DropItem( new MortarPestle( 5 ) );
			DropItem( new BeccoDiFalco( amount ) );
			DropItem( new CarneUmana( amount ) );
			DropItem( new CodaDiTopo( amount ) );
			DropItem( new ArtigliDiOrso( amount ) );
			DropItem( new PolvereDiVampiro( amount ) );
			DropItem( new DitoDiGigante( amount ) );
			//DropItem( new Lichene());
			DropItem( new RadiceCanina( amount ) );
			DropItem( new UovoDiRagno( amount ) );
			DropItem( new AliDiFata( amount ) );
			DropItem( new Granchio( amount ) );
			DropItem( new Cuore( amount ) );
			DropItem( new Lavanda( amount ) );
			DropItem( new FioreBluDiMontagna( amount ) );
			DropItem( new FioreRossoDiMontagna( amount ) );
			DropItem( new FioreViolaDiMontagna( amount ) );
			DropItem( new FioreDiBelladonna( amount ) );
			DropItem( new Baccaneve( amount ) );
			DropItem( new Ranuncolo( amount ) );
			DropItem( new Cardo( amount ) );
			DropItem( new Ginepro( amount ) );
			DropItem( new CotoneDellaTundra( amount ) );
			DropItem( new CoronaInsanguinata( amount ) );
			DropItem( new AmanitaMuscaria( amount ) );
			DropItem( new CeppoDiFolletto( amount ) );
			DropItem( new FungoBianco( amount ) );
			DropItem( new FungoLuminoso( amount ) );
			DropItem( new MoraTapinella( amount ) );
			DropItem( new OrecchieDiElfo( amount ) );
			//
		}

		public PoisonIngredientsBag( Serial serial ) : base( serial )
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

	public class BeccoDiFalco : BaseReagent
	{
		public override int PotionType{get{ return 4; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 2; }}//1-6

		[Constructable]
		public BeccoDiFalco() : this( 1 )
		{
		}

		[Constructable]
		public BeccoDiFalco( int amount ) : base( 0x97C, amount )
		{
			Name = "Becco di Falco";
			//Hue = 2019;
			Stackable = true;
		}

		public BeccoDiFalco( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "becc%ohi% di falco" : "hawk beak%s%", Amount, from.Language ) );
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

	public class CarneUmana : BaseReagent
	{
		public override int PotionType{get{ return 3; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 1; }}//1-6

		[Constructable]
		public CarneUmana() : this( 1 )
		{
		}

		[Constructable]
		public CarneUmana( int amount ) : base( 0x1E1F, amount )
		{
			Name = "Carne Umana";
			//Hue = 2561;
			Stackable = true;
		}

		public CarneUmana( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "carn%ei% uman%ae%" : "human flesh%s%", Amount, from.Language ) );
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

	public class CodaDiTopo : BaseReagent
	{
		public override int PotionType{get{ return 1; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 1; }}//1-6

		[Constructable]
		public CodaDiTopo() : this( 1 )
		{
		}

		[Constructable]
		public CodaDiTopo( int amount ) : base( 0xEC1, amount )
		{
			Name = "Coda di Topo";
			Hue = 2148;
			Stackable = true;
		}

		public CodaDiTopo( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "cod%ae% di topo" : "rat tail%s%", Amount, from.Language ) );
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

	public class ArtigliDiOrso : BaseReagent
	{
		public override int PotionType{get{ return 5; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 1; }}//1-6

		[Constructable]
		public ArtigliDiOrso() : this( 1 )
		{
		}

		[Constructable]
		public ArtigliDiOrso( int amount ) : base( 0x1C20, amount )
		{
			Name = "Artigli di Orso";
			Hue = 2442;
			Stackable = true;
		}

		public ArtigliDiOrso( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "artigli%o% di orso" : "bear claw%s%", Amount, from.Language ) );
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

	public class PolvereDiVampiro : BaseReagent
	{
		public override int PotionType{get{ return 5; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 6; }}//1-6

		[Constructable]
		public PolvereDiVampiro() : this( 1 )
		{
		}

		[Constructable]
		public PolvereDiVampiro( int amount ) : base( 0x26B8, amount )
		{
			Name = "Polvere di Vampiro";
			Hue = 1150;
			Stackable = true;
		}

		public PolvereDiVampiro( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "polver%ei% di vampiro" : "vampire ash%s%", Amount, from.Language ) );
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

	public class DitoDiGigante : BaseReagent
	{
		public override int PotionType{get{ return 1; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 6; }}//1-6

		[Constructable]
		public DitoDiGigante() : this( 1 )
		{
		}

		[Constructable]
		public DitoDiGigante( int amount ) : base( 0x1B11, amount )
		{
			Name = "Dito di Gigante";
			Hue = 1005;
			Stackable = true;
		}

		public DitoDiGigante( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "dit%oa% di gigante" : "giant toe%s%", Amount, from.Language ) );
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

	public class Lichene : BaseReagent
	{
		[Constructable]
		public Lichene() : this( 1 )
		{
		}

		[Constructable]
		public Lichene( int amount ) : base( 0x1F0A, amount )
		{
			Name = "Lichene";
			//Hue = 1005;
			Stackable = false;
		}

		public Lichene( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "lichen%ei%" : "lichen%s%", Amount, from.Language ) );
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

	public class RadiceCanina : BaseReagent
	{
		public override int PotionType{get{ return 4; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 4; }}//1-6

		[Constructable]
		public RadiceCanina() : this( 1 )
		{
		}

		[Constructable]
		public RadiceCanina( int amount ) : base( 0x312A, amount )
		{
			Name = "Radice Canina";
			//Hue = 1005;
			Stackable = true;
		}

		public RadiceCanina( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "radic%ei% canin%ae%" : "canine root%s%", Amount, from.Language ) );
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

	public class UovoDiRagno : BaseReagent
	{
		public override int PotionType{get{ return 4; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 6; }}//1-6

		[Constructable]
		public UovoDiRagno() : this( 1 )
		{
		}

		[Constructable]
		public UovoDiRagno( int amount ) : base( 0x2809, amount )
		{
			Name = "Uovo di Ragno";
			//Hue = 1005;
			Stackable = true;
		}

		public UovoDiRagno( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "uov%oa% di ragno" : "spider egg%s%", Amount, from.Language ) );
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

	public class AliDiFata : BaseReagent
	{
		public override int PotionType{get{ return 3; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 6; }}//1-6

		[Constructable]
		public AliDiFata() : this( 1 )
		{
		}

		[Constructable]
		public AliDiFata( int amount ) : base( 0x2838, amount )
		{
			Name = "Ali di Fata";
			//Hue = 1005;
			Stackable = true;
		}

		public AliDiFata( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "ali di fata" : "fairy wings", Amount, from.Language ) );
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

	public class Granchio : BaseReagent
	{
		public override int PotionType{get{ return 2; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 4; }}//1-6

		[Constructable]
		public Granchio() : this( 1 )
		{
		}

		[Constructable]
		public Granchio( int amount ) : base( 0x3F23, amount )
		{
			Name = "Granchio";
			//Hue = 1005;
			Stackable = true;
		}

		public Granchio( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "granchi%o%" : "crab%s%", Amount, from.Language ) );
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

	public class Cuore : BaseReagent
	{
		public override int PotionType{get{ return 2; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 1; }}//1-6

		[Constructable]
		public Cuore() : this( 1 )
		{
		}

		[Constructable]
		public Cuore( int amount ) : base( 0x1CED, amount )
		{
			Name = "Cuore";
			//Hue = 1005;
			Stackable = true;
		}

		public Cuore( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "cuor%ei%" : "heart%s%", Amount, from.Language ) );
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

	public class OrecchieDiElfo : BaseReagent
	{
		public override int PotionType{get{ return 2; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 6; }}//1-6

		[Constructable]
		public OrecchieDiElfo() : this( 1 )
		{
		}
	
		[Constructable]
		public OrecchieDiElfo( int amount ) : base( 0x312D, amount )
		{
			Name = "Orecchie Di Elfo";
			Stackable = true;
			Amount = amount;
			Weight = 1;			
		}
		
		public OrecchieDiElfo( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "orecchie di elfo" : "elf ears", Amount, from.Language ) );
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