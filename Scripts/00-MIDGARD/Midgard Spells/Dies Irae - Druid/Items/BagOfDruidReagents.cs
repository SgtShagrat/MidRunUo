/***************************************************************************
 *                               BagOfDruidReagents.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Items;

using Server;
using Server.Items;

namespace Midgard.Engines.SpellSystem
{
    public class BagOfDruidReagents : Bag
    {
        [Constructable]
        public BagOfDruidReagents()
            : this( 10 )
        {
            Hue = Utility.RandomNeutralHue();
        }

        [Constructable]
        public BagOfDruidReagents( int amount )
        {
            DropItem( new BlackPearl( amount ) );
            DropItem( new Bloodmoss( amount ) );
            DropItem( new Garlic( amount ) );
            DropItem( new Ginseng( amount ) );
            DropItem( new MandrakeRoot( amount ) );
            DropItem( new Nightshade( amount ) );
            DropItem( new SulfurousAsh( amount ) );
            DropItem( new SpidersSilk( amount ) );
            DropItem( new PetrifiedWood( amount ) );
            DropItem( new DestroyingAngel( amount ) );
            DropItem( new SpringWater( amount ) );
            DropItem( new FertileDirt( amount ) );
            DropItem( new Kindling( amount ) );
            DropItem( new PigIron( amount ) );
        }

        #region serialization
        public BagOfDruidReagents( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }
}