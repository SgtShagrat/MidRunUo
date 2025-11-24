using System;

using Server;

namespace Midgard.Items
{
    public class CraftableLampPost3 : BaseDecoLight
    {
        public override int LitItemID { get { return 0xB24; } }
        public override int UnlitItemID { get { return 0xB25; } }

        public override int LabelNumber
        {
            get { return 1064934; } // modern lamp post
        }

        [Constructable]
        public CraftableLampPost3()
            : base( 0xb25 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle300;
            Weight = 40.0;
        }

        public CraftableLampPost3( Serial serial )
            : base( serial )
        {
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