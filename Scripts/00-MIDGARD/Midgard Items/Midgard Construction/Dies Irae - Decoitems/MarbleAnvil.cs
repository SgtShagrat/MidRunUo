/***************************************************************************
 *                               MarbleAnvil.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Items
{
    [Anvil, Flipable( 0x3CC0, 0x3CC1 )]
    public class MarbleAnvil : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064955; } // marble anvil
        }

        [Constructable]
        public MarbleAnvil()
            : base( 0x3CC0 )
        {
        }

        public MarbleAnvil( Serial serial )
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