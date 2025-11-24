using Server;
using Server.Mobiles;

namespace Midgard.Engines.Classes
{
    public class SpiritualityStone : RitualItem
    {
        public override string DefaultName
        {
            get { return "a spirituality stone"; }
        }

        public override Classes Class
        {
            get { return Classes.Paladin; }
        }

        [Constructable]
        public SpiritualityStone()
            : base( 0x1870 )
        {
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public SpiritualityStone( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}