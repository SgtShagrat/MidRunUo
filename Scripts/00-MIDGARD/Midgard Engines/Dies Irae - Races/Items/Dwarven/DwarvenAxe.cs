using Midgard.Engines.Races;

using Server;
using Server.Items;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
	/// <summary>
	/// 0x335F Dwarven Axe - ( craftabile solo da razza: nani, due mani )
	/// </summary>
	[RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class DwarvenAxe : DoubleAxe
	{
		public override string DefaultName { get { return "dwarven axe"; } }

		public override int BlockCircle{ get{ return -1; } }

		[Constructable]
		public DwarvenAxe()
		{
			ItemID = 0x335F;
			Layer = Layer.TwoHanded;
		}

		public override bool CanBeCraftedBy( Mobile from )
		{
			return from.AccessLevel > AccessLevel.Counselor || from.Race == Core.MountainDwarf;
		}

		#region serialization
		public DwarvenAxe( Serial serial ) : base( serial )
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