using Server;
using Server.Items;

namespace Midgard.Items
{
    [Furniture]
    [Flipable( 0x1E34, 0x1E35 )]
    public class CraftableScareCrow : CraftableFurniture
    {
        public override int LabelNumber
        {
            get { return 1064927; } // scarecrow
        }

        [Constructable]
        public CraftableScareCrow()
            : base( 0x1E34 )
        {
        }

        public CraftableScareCrow( Serial serial )
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