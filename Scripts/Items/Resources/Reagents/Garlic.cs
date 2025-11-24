using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class Garlic : BaseReagent, ICommodity
	{
		public override int PotionType{get{ return 4; }}//1 Blu, 2 Rosso, 3 Giallo, 4 Bianco, 5 Verde
		public override int PotionStrenght{get{ return 1; }}//1-6

		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} garlic", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		[Constructable]
		public Garlic() : this( 1 )
		{
		}

		[Constructable]
		public Garlic( int amount ) : base( 0xF84, amount )
		{
		}

		public Garlic( Serial serial ) : base( serial )
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
}