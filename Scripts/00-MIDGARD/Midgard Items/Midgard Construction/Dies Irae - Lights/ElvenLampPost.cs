using System;

using Server;

namespace Midgard.Items
{
    public class ElvenLampPost : BaseDecoLight
    {
        public override int LitItemID { get { return 0x38D5; } }
        public override int UnlitItemID { get { return 0x38D5; } }
        public override string DefaultName { get { return "elven lampost"; } }

        [Constructable]
        public ElvenLampPost()
            : base( 0x38D5 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle300;
            Weight = 40.0;
        }

        public ElvenLampPost( Serial serial )
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