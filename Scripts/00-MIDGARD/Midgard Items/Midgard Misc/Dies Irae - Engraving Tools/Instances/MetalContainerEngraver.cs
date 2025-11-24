using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class MetalContainerEngraver : BaseEngravingTool
    {
        public override int EngraverTypeLabelNumber
        {
            get { return 1065605; } // metal container
        }

        public override int EngraverTypeCapLabelNumber
        {
            get { return 1065613; } // Metal Container
        }

        public override Type[] Engraves
        {
            get { return m_Engraves; }
        }

        private static readonly Type[] m_Engraves = new Type[]
                       {
                           typeof (ParagonChest), typeof (MetalChest), typeof (CraftableTrashChest),
                           typeof (MetalGoldenChest)
                       };

        [Constructable]
        public MetalContainerEngraver()
            : base( 0x1EB8, 1 )
        {
            Weight = 1;
        }

        #region serial-deserial
        public MetalContainerEngraver( Serial serial )
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