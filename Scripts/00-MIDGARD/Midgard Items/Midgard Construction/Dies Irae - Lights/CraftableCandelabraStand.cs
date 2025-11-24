using System;

using Server;

namespace Midgard.Items
{
    public class CraftableCandelabraStand : BaseDecoLight
    {
        public override int LitItemID { get { return 0xB26; } }
        public override int UnlitItemID { get { return 0xA29; } }

        public override int LabelNumber
        {
            get { return 1064931; } // candelabra stand
        }

        [Constructable]
        public CraftableCandelabraStand()
            : base( 0xA29 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle225;
            Weight = 20.0;
        }

        public CraftableCandelabraStand( Serial serial )
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