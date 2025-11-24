using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class ClothingEngraver : BaseRepairableEngravingTool
    {
        public override int EngraverTypeLabelNumber
        {
            get { return 1065680; } // clothing
        }

        public override int EngraverTypeCapLabelNumber
        {
            get { return 1065681; } // Clothing
        }

        public override Type[] Engraves
        {
            get { return m_Engraves; }
        }

        private static readonly Type[] m_Engraves = new Type[]
                       {
                           typeof (BaseClothing)
                       };

        public override int OSIMaxCharges
        {
            get { return 10; }
        }

        public override Type RechargeItemType
        {
            get { return typeof( EcruCitrine ); }
        }

        public override int TipTypeLabelNumber
        {
            get { return 1065625; } // ecru citrine
        }

        public override int TipTypeCapLabelNumber
        {
            get { return 1065618; } // Ecru Citrine
        }

        [Constructable]
        public ClothingEngraver()
            : base( 0xF9D, 10 )
        {
        }

        #region serial-deserial
        public ClothingEngraver( Serial serial )
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