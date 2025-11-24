/***************************************************************************
 *                               MarbleForge.cs
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
    [Forge]
    public class MarbleForge : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064957; } // marble forge
        }

        [Constructable]
        public MarbleForge()
            : base( 0x3CBE )
        {
        }

        public MarbleForge( Serial serial )
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