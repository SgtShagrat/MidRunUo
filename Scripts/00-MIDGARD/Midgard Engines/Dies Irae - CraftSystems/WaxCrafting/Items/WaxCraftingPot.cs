using Midgard.Engines.Craft;

using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Items
{
    public class WaxCraftingPot : BaseTool
	{
		public override int LabelNumber{ get{ return 1065263; } } // Wax Crafting Pot
		
		public override CraftSystem CraftSystem{ get{ return DefWaxCrafting.CraftSystem; } }

		[Constructable]
		public WaxCraftingPot() : base( 0x142A )
		{
			Weight = 8.0;
		}

		public WaxCraftingPot( Serial serial ) : base( serial )
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