using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    [Flipable]
    public class CraftableWallSconce : BaseDecoLight
    {
        public override int LitItemID { get { return ItemID == 0x9FB ? 0x9FD : 0xA02; } }

        public override int UnlitItemID { get { return ItemID == 0x9FD ? 0x9FB : 0xA00; } }

        public override int LabelNumber
        {
            get { return 1064936; } // wall sconce
        }

        [Constructable]
        public CraftableWallSconce()
            : base( 0x9FB )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.WestBig;
            Weight = 3.0;
        }

        public CraftableWallSconce( Serial serial )
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
                case 0x9FB:
                    ItemID = 0xA00;
                    break;
                case 0x9FD:
                    ItemID = 0xA02;
                    break;
                case 0xA00:
                    ItemID = 0x9FB;
                    break;
                case 0xA02:
                    ItemID = 0x9FD;
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