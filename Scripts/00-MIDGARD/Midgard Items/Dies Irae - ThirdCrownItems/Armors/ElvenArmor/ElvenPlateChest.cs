using Midgard.Engines.Races;
using Server;
using Server.Items;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
	public class ElvenPlateChest : PlateChest
	{
		[Constructable]
		public ElvenPlateChest() : this( 0 )
		{
		}

		[Constructable]
		public ElvenPlateChest( int hue )
		{
			ItemID = 0x286;
			Hue = hue;
		}

		public override string DefaultName
		{
			get { return "elven platemail"; }
		}

		public override bool CanBeCraftedBy( Mobile from )
		{
			return from.AccessLevel > AccessLevel.Counselor || Midgard.Engines.Races.Core.IsElfRace( from.Race );
		}

		public override bool CanEquip( Mobile from )
		{
			return from.Karma > 0 && base.CanEquip( from );
		}

		#region serialization

		public ElvenPlateChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		#endregion
	}
}