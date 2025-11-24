using System;
using Server;
using Server.Regions;

namespace Server.Items
{
	public abstract class BaseReagent : Item
	{
		public virtual int PotionType{get{ return 0; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public virtual int PotionStrenght{get{ return 0; }}//1-6

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		public BaseReagent( int itemID ) : this( itemID, 1 )
		{
		}

		public BaseReagent( int itemID, int amount ) : base( itemID )
		{
			Stackable = true;
			Amount = amount;
		}

		public BaseReagent( Serial serial ) : base( serial )
		{
		}

		public override bool Decays
		{
			get { return ( Spawner as SpawnEntry ) == null && base.Decays; }
		}

		#region mod by Dies Irae : pre-aos stuff
		public override void OnSingleClick( Mobile from )
		{
			//string name = string.IsNullOrEmpty( Name ) ? (from.Language == "ITA" ? StringList.LocalizationIta[ LabelNumber ] : StringList.Localization[ LabelNumber ]) : Name;
			string name = string.IsNullOrEmpty( Name ) ? StringUtility.ConvertItemName( LabelNumber, Amount, from.Language ) : StringUtility.ConvertItemName(Name, Amount, from.Language);
			//LabelTo( from, String.Format( "{0} {1}", Amount, name ) );
			LabelTo( from, name );
		}
		#endregion

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