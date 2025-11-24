using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class WeaponEngraver : BaseRepairableEngravingTool
    {
        public override int EngraverTypeLabelNumber
        {
            get { return 1065606; } // weapon
        }

        public override int EngraverTypeCapLabelNumber
        {
            get { return 1065614; } // Weapon
        }

        public override Type[] Engraves
        {
            get { return m_Engraves; }
        }

        private static readonly Type[] m_Engraves = new Type[]
                                                    {
                                                        typeof (BaseWeapon)
                                                    };

        public override int OSIMaxCharges
        {
            get { return 10; }
        }

        public override Type RechargeItemType
        {
            get { return typeof( BlueDiamond ); }
        }

        public override int TipTypeLabelNumber
        {
            get { return 1065628; } // blue diamond
        }

        public override int TipTypeCapLabelNumber
        {
            get { return 1065621; } // Blue Diamond
        }

        [Constructable]
        public WeaponEngraver()
            : base( 0x32F8, 10 )
        {
            Hue = 0;
        }

        #region serial-deserial
        public WeaponEngraver( Serial serial )
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