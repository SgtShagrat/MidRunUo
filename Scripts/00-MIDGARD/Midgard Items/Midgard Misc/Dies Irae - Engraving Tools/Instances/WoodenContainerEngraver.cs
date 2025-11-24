using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class WoodenContainerEngraver : BaseEngravingTool
    {
        public override int EngraverTypeLabelNumber
        {
            get { return 1065604; } // wooden container
        }

        public override int EngraverTypeCapLabelNumber
        {
            get { return 1065612; } // Wooden Container
        }

        public override Type[] Engraves
        {
            get { return m_Engraves; }
        }

        private static readonly Type[] m_Engraves = new Type[]
                       {
                           typeof (WoodenBox), typeof (LargeCrate), typeof (MediumCrate),
                           typeof (SmallCrate), typeof (WoodenChest), typeof (CraftableBookcase),
                           typeof (Armoire), typeof (FancyArmoire), typeof (PlainWoodenChest),
                           typeof (OrnateWoodenChest), typeof (GildedWoodenChest), typeof (WoodenFootLocker),
                           typeof (FinishedWoodenChest), typeof (TallCabinet), typeof (ShortCabinet),
                           typeof (RedArmoire), typeof (CherryArmoire), typeof (MapleArmoire),
                           typeof (ElegantArmoire), typeof (Keg), typeof (SimpleElvenArmoire),
                           typeof (DecorativeBox), typeof (FancyElvenArmoire), typeof (OrnateElvenChestEastAddon),
                           typeof (RarewoodChest), typeof (OrnateElvenChestSouthAddon),
                           typeof (WeaponBarrel1), typeof (WeaponBarrel2), typeof (WeaponBarrel3),
                           typeof (WeaponBarrel4), typeof (CraftableTrashBarrel)
                       };

        [Constructable]
        public WoodenContainerEngraver()
            : base( 0x1026, 1 )
        {
        }

        #region serial-deserial
        public WoodenContainerEngraver( Serial serial )
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