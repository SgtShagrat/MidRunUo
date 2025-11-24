using System;

using Server;

namespace Midgard.Items
{
    public class CraftableBrazierTall : BaseDecoLight
    {
        public override int LitItemID { get { return 0x19AA; } }

        public override int LabelNumber
        {
            get { return 1064929; } // tall brazier
        }

        [Constructable]
        public CraftableBrazierTall()
            : base( 0x19AA )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle300;
            Weight = 25.0;
        }

        public CraftableBrazierTall( Serial serial )
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