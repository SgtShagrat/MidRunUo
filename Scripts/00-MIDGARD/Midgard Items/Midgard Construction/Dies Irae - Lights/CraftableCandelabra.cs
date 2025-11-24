using System;

using Server;

namespace Midgard.Items
{
    public class CraftableCandelabra : BaseDecoLight
    {
        public override int LitItemID { get { return 0xB1D; } }
        public override int UnlitItemID { get { return 0xA27; } }

        public override int LabelNumber
        {
            get { return 1064930; } // candelabra
        }

        [Constructable]
        public CraftableCandelabra()
            : base( 0xA27 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle225;
            Weight = 3.0;
        }

        public CraftableCandelabra( Serial serial )
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