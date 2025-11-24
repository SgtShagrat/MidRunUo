using System;

using Server;

namespace Midgard.Items
{
    public class ElvenLampPost2 : BaseDecoLight
    {
        public override int LitItemID { get { return 0x38D6; } }
        public override int UnlitItemID { get { return 0x38D6; } }
        public override string DefaultName { get { return "elven lampost"; } }

        [Constructable]
        public ElvenLampPost2()
            : base( 0x38D6 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle300;
            Weight = 40.0;
        }

        public ElvenLampPost2( Serial serial )
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