using System;

using Server;

namespace Midgard.Items
{
    public class CraftableLampPost2 : BaseDecoLight
    {
        public override int LitItemID { get { return 0xB22; } }
        public override int UnlitItemID { get { return 0xB23; } }

        public override int LabelNumber
        {
            get { return 1064933; } // round lamp post
        }

        [Constructable]
        public CraftableLampPost2()
            : base( 0xB23 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle300;
            Weight = 40.0;
        }

        public CraftableLampPost2( Serial serial )
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