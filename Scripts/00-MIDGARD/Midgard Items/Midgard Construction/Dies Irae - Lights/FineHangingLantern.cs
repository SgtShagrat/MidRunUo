using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    [Flipable]
    public class FineHangingLantern : BaseDecoLight
    {
        public override int LitItemID { get { return ItemID == 0x3FFD ? 0x3FFB : 0x3FFA; } }

        public override int UnlitItemID { get { return ItemID == 0x3FFB ? 0x3FFD : 0x3FFC; } }

        public override int LabelNumber
        {
            get { return 1064952; } // fine hanging lantern
        }

        [Constructable]
        public FineHangingLantern()
            : base( 0x3FFD )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.WestBig;
            Weight = 10.0;
        }

        public FineHangingLantern( Serial serial )
            : base( serial )
        {
        }

        public void Flip()
        {
            switch( Light )
            {
                case LightType.WestBig:
                    Light = LightType.NorthBig;
                    break;
                case LightType.NorthBig:
                    Light = LightType.WestBig;
                    break;
            }

            switch( ItemID )
            {
                case 0x3FFA:
                    ItemID = 0x3FFB;
                    break;
                case 0x3FFB:
                    ItemID = 0x3FFA;
                    break;

                case 0x3FFC:
                    ItemID = 0x3FFD;
                    break;
                case 0x3FFD:
                    ItemID = 0x3FFC;
                    break;
            }
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }
}