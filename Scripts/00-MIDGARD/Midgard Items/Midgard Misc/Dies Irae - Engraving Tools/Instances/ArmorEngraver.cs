using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    [Flipable( 0x1EBA, 0x1EBB )]
    public class ArmorEngraver : BaseRepairableEngravingTool
    {
        public override int EngraverTypeLabelNumber
        {
            get { return 1065607; } // armor
        }

        public override int EngraverTypeCapLabelNumber
        {
            get { return 1065615; } // Armor
        }

        public override Type[] Engraves
        {
            get { return m_Engraves; }
        }

        private static readonly Type[] m_Engraves = new Type[]
                       {
                           typeof (BaseArmor)
                       };

        public override int OSIMaxCharges
        {
            get { return 10; }
        }

        public override Type RechargeItemType
        {
            get { return typeof( Turquoise ); }
        }

        public override int TipTypeLabelNumber
        {
            get { return 1065629; } // turquoise
        }

        public override int TipTypeCapLabelNumber
        {
            get { return 1065622; } // Turquoise
        }

        [Constructable]
        public ArmorEngraver()
            : base( 0x1EBA, 10 )
        {
        }

        #region serial-deserial
        public ArmorEngraver( Serial serial )
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