/***************************************************************************
 *                               FancyMarbleForge.cs
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
    [Forge, Flipable( 0x3CBB, 0x3CBC, 0x3CBD )]
    public class FancyMarbleForge : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064956; } // fancy marbe forge
        }

        [Constructable]
        public FancyMarbleForge()
            : base( 0x3CBB )
        {
        }

        public FancyMarbleForge( Serial serial )
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