/***************************************************************************
 *                               Dies Irae - LargeCounter.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    [Furniture]
    [Flipable( 0xB3F, 0xB40 )]
    public class LargeCounter : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064510; } // Large Counter
        }

        [Constructable]
        public LargeCounter()
            : base( 0xB3F )
        {
            Weight = 2.0;
        }

        public LargeCounter( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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