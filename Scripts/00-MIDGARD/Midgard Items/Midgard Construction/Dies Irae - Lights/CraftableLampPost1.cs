using System;

using Server;

namespace Midgard.Items
{
    public class CraftableLampPost1 : BaseDecoLight
    {
        public override int LitItemID { get { return 0xB20; } }
        public override int UnlitItemID { get { return 0xB21; } }

        public override int LabelNumber
        {
            get { return 1064932; } // classic lamp post
        }

        [Constructable]
        public CraftableLampPost1()
            : base( 0xB21 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle300;
            Weight = 40.0;
        }

        public CraftableLampPost1( Serial serial )
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