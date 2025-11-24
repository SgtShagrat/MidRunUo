using Server;

namespace Midgard.Engines.TownHouses
{
    public static class RegionHelper
    {
        public static void UpdateRegion( TownHouseSign sign )
        {
            if( sign == null )
                return;

            if( sign.House == null )
                return;

            if( sign.House.Region == null || sign.House.Region.Area == null )
                return;

            sign.House.UpdateRegion();

            Rectangle3D rect = new Rectangle3D( Point3D.Zero, Point3D.Zero );

            for( int i = 0; i < sign.House.Region.Area.Length; ++i )
            {
                rect = sign.House.Region.Area[ i ];
                rect = new Rectangle3D( new Point3D( rect.Start.X - sign.House.X, rect.Start.Y - sign.House.Y, sign.MinZ ), new Point3D( rect.End.X - sign.House.X, rect.End.Y - sign.House.Y, sign.MaxZ ) );
                sign.House.Region.Area[ i ] = rect;
            }

            sign.House.Region.Unregister();
            sign.House.Region.Register();
            sign.House.Region.GoLocation = sign.BanLoc;
        }

        public static bool RegionContains( Region region, Mobile m )
        {
            return region.GetMobiles().Contains( m );
        }

        public static Rectangle3D[] RegionArea( Region region )
        {
            return region.Area;
        }
    }
}