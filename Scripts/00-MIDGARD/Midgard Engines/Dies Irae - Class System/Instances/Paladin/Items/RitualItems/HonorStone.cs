using Server;
using Server.Mobiles;

namespace Midgard.Engines.Classes
{
    public class HonorStone : RitualItem
    {
        public override string DefaultName
        {
            get { return "a honor stone"; }
        }

        public override Classes Class
        {
            get { return Classes.Paladin; }
        }

        [Constructable]
        public HonorStone()
            : base( 0x186B )
        {
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public HonorStone( Serial serial )
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