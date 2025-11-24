/***************************************************************************
 *                               BagOfAdvancedSmelting.cs
 *
 *   begin                : 06 agosto, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Engines.AdvancedSmelting
{
    public class BagOfAdvancedSmelting : Bag
    {
        [Constructable]
        public BagOfAdvancedSmelting( int amount )
        {
            DropItem( new AdvancedForge() );
            DropItem( new CoilOre( amount ) );
            DropItem( new PowderOfMineralSolution( amount ) );

            foreach( int i in Enum.GetValues( typeof( SmeltingRecipes ) ) )
                DropItem( new SmeltingRecipeScroll( i ) );

            DropItem( new SmeltingBook() );
        }

        #region serialization
        public BagOfAdvancedSmelting( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}