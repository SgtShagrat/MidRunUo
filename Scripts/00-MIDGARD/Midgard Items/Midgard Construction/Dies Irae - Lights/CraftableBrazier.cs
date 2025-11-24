using System;

using Server;

namespace Midgard.Items
{
    public class CraftableBrazier : BaseDecoLight
    {
        public override int LitItemID { get { return 0xE31; } }

        public override int LabelNumber
        {
            get { return 1064928; } // brazier
        }

        [Constructable]
        public CraftableBrazier()
            : base( 0xE31 )
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle225;
            Weight = 20.0;
        }

        public CraftableBrazier( Serial serial )
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