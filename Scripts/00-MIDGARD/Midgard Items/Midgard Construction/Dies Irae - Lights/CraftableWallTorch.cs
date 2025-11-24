using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    [Flipable]
    public class CraftableWallTorch : BaseDecoLight
    {
        public override int LitItemID { get { return ItemID == 0xA05 ? 0xA07 : 0xA0C; } }

        public override int UnlitItemID { get { return ItemID == 0xA07 ? 0xA05 : 0xA0A; } }

        public override int LabelNumber
        {
            get { return 1064935; } // wall torch
        }

        [Constructable]
        public CraftableWallTorch()
            : base( 0xA05 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.WestBig;
            Weight = 3.0;
        }

        public CraftableWallTorch( Serial serial )
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
                case 0xA05:
                    ItemID = 0xA0A;
                    break;
                case 0xA07:
                    ItemID = 0xA0C;
                    break;

                case 0xA0A:
                    ItemID = 0xA05;
                    break;
                case 0xA0C:
                    ItemID = 0xA07;
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