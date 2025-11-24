using Server;
using Server.Multis;

namespace Midgard.Engines.TownHouses
{
    public class VersionHouse : BaseHouse
    {
        public VersionHouse( int id, Mobile m, int locks, int secures )
            : base( id, m, locks, secures )
        {
        }

        public override Rectangle2D[] Area { get { return new Rectangle2D[ 5 ]; } }

        public override Point3D BaseBanLocation { get { return Point3D.Zero; } }

        #region serialization
        public VersionHouse( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
        }
        #endregion
    }
}