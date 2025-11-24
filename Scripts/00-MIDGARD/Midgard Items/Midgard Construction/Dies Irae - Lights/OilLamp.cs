using System;

using Server;

namespace Midgard.Items
{
    public class OilLamp : BaseDecoLight
    {
        public override int LitItemID { get { return 0x3FF4; } }
        public override int UnlitItemID { get { return 0x3FF3; } }

        public override int LabelNumber
        {
            get { return 1064953; } // oil lamp
        }

        [Constructable]
        public OilLamp()
            : base( 0x3FF3 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle225;
            Weight = 5.0;
        }

        public OilLamp( Serial serial )
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