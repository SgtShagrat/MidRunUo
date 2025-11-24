using System;

using Server;

namespace Midgard.Items
{
    public class ElvenLamp : BaseDecoLight
    {
        public override int LitItemID { get { return 0x38D7; } }
        public override int UnlitItemID { get { return 0x38D7; } }
        public override string DefaultName { get { return "elven lamp"; } }

        [Constructable]
        public ElvenLamp()
            : base( 0x38D7 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle300;
            Weight = 10.0;
        }

        public ElvenLamp( Serial serial )
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