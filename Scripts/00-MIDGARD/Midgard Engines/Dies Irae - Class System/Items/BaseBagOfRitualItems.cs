/***************************************************************************
 *                               BaseBagOfRitualItems.cs
 *
 *   revision             : 03 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Engines.Classes;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseBagOfRitualItems : Bag
    {
        [Constructable]
        protected BaseBagOfRitualItems()
            : this( 10 )
        {
        }

        [Constructable]
        protected BaseBagOfRitualItems( int amount )
        {
            for( int i = 0; i < amount; i++ )
                DropItem( System.RandomRitualItem() );
        }

        #region serialization
        public BaseBagOfRitualItems( Serial serial )
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

        public abstract ClassSystem System { get; }
    }
}