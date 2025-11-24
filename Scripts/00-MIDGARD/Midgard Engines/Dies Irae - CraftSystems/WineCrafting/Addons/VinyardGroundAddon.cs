using Server;
using Server.Items;
using Server.Multis;

namespace Midgard.Engines.WineCrafting
{
    public class VinyardGroundAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new VinyardGroundAddonDeed(); } }

        #region Constructors
        [Constructable]
        public VinyardGroundAddon( VinyardGroundType type, int width, int height )
            : this( (int)type, width, height )
        {
        }

        public VinyardGroundAddon( int type, int width, int height )
        {
            VinyardGroundInfo info = VinyardGroundInfo.GetInfo( type );

            AddComponent( new AddonComponent( info.GetItemPart( VinyardGroundPosition.Top ).ItemID ), 0, 0, 0 );
            AddComponent( new AddonComponent( info.GetItemPart( VinyardGroundPosition.Right ).ItemID ), width, 0, 0 );
            AddComponent( new AddonComponent( info.GetItemPart( VinyardGroundPosition.Left ).ItemID ), 0, height, 0 );
            AddComponent( new AddonComponent( info.GetItemPart( VinyardGroundPosition.Bottom ).ItemID ), width, height, 0 );

            int w = width - 1;
            int h = height - 1;

            for( int y = 1; y <= h; ++y )
                AddComponent( new AddonComponent( info.GetItemPart( VinyardGroundPosition.West ).ItemID ), 0, y, 0 );

            for( int x = 1; x <= w; ++x )
                AddComponent( new AddonComponent( info.GetItemPart( VinyardGroundPosition.North ).ItemID ), x, 0, 0 );

            for( int y = 1; y <= h; ++y )
                AddComponent( new AddonComponent( info.GetItemPart( VinyardGroundPosition.East ).ItemID ), width, y, 0 );

            for( int x = 1; x <= w; ++x )
                AddComponent( new AddonComponent( info.GetItemPart( VinyardGroundPosition.South ).ItemID ), x, height, 0 );

            for( int x = 1; x <= w; ++x )
                for( int y = 1; y <= h; ++y )
                    AddComponent( new AddonComponent( info.GetItemPart( VinyardGroundPosition.Center ).ItemID ), x, y, 0 );
        }

        public VinyardGroundAddon( Serial serial )
            : base( serial )
        {
        }
        #endregion

        public override void OnDoubleClick( Mobile from )
        {
            BaseHouse house = BaseHouse.FindHouseAt( this );

            if( house != null && house.IsCoOwner( from ) )
            {
                if( from.InRange( GetWorldLocation(), 3 ) )
                {
                    from.SendGump( new ConfirmRemovalGump( this ) );
                }
                else
                {
                    from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
                }
            }
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
    }

    public class VinyardGroundAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return null; } }

        [Constructable]
        public VinyardGroundAddonDeed()
        {
            Name = "Vinyard Ground Addon Deed";
        }

        public VinyardGroundAddonDeed( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsChildOf( from.Backpack ) )
                BoundingBoxPicker.Begin( from, new BoundingBoxCallback( BoundingBox_Callback ), null );
            else
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
        }

        private void BoundingBox_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
        {
            IPoint3D p = start;

            if( p == null || map == null )
                return;

            int width = ( end.X - start.X ), height = ( end.Y - start.Y );

            if( width < 2 || height < 2 )
                from.SendMessage( "The bounding targets must be at least a 3x3 box." );
            else if( IsChildOf( from.Backpack ) )
                from.SendGump( new VinyardGroundGump( this, p, map, width, height ) );
            else
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
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
    }
}