using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class LeatherContainerEngraver : BaseEngravingTool
    {
        public override int EngraverTypeLabelNumber
        {
            get { return 1065603; } // leather container
        }

        public override int EngraverTypeCapLabelNumber
        {
            get { return 1065611; } // Leather Container 
        }

        public override Type[] Engraves
        {
            get { return m_Engraves; }
        }

        private static readonly Type[] m_Engraves = new Type[]
                                                    {
                                                        typeof (Pouch), typeof (Backpack), typeof (Bag)
                                                    };

        [Constructable]
        public LeatherContainerEngraver()
            : base( 0xF9D, 1 )
        {
        }

        #region serial-deserial
        public LeatherContainerEngraver( Serial serial )
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