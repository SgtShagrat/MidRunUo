using Server;
using Server.Mobiles;

namespace Midgard.Engines.Classes
{
    public class HonestyStone : RitualItem
    {
        public override string DefaultName
        {
            get { return "a honesty stone"; }
        }

        public override Classes Class
        {
            get { return Classes.Paladin; }
        }

        [Constructable]
        public HonestyStone()
            : base( 0x186C )
        {
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public HonestyStone( Serial serial )
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