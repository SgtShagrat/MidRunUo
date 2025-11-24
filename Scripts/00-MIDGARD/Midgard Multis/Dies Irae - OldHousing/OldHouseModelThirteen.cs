using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelThirteen : BaseHouse
    {
        public static int AreaScalar = 136;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -6, -6, 11, 12 ), new Rectangle2D( 5, -4, 1, 4 ) };

        public OldHouseModelThirteen( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( 4, -3, 5, keyValue );

            AddEastDoor( -2, -3, 5 );

            AddSouthDoor( 2, -1, 25 );

            SetSign( 5, -4, 4 );
            ChangeSignType( 3025 );
        }

        public override int DefaultPrice
        {
            get { return AreaScalar * 500; }
        }

        public override Rectangle2D[] Area
        {
            get { return AreaArray; }
        }

        public override Point3D BaseBanLocation
        {
            get { return new Point3D( 5, 7, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x2F:
                    return new PlasterHousewithBalconyDeed();
                case 0x3C:
                    return new SandstoneHousewithBalconyDeed();
                case 0x49:
                    return new WoodenHousewithBalconyDeed();
                default:
                    return new PlasterHousewithBalconyDeed();
            }
        }

        #region serialization
        public OldHouseModelThirteen( Serial serial )
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

namespace Midgard.Multis.Deeds
{
    public class PlasterHousewithBalconyDeed : HouseDeed
    {
        [Constructable]
        public PlasterHousewithBalconyDeed()
            : base( 0x2F, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster house with balcony"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThirteen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThirteen( owner, 0x2F );
        }

        #region serialization
        public PlasterHousewithBalconyDeed( Serial serial )
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

    public class SandstoneHousewithBalconyDeed : HouseDeed
    {
        [Constructable]
        public SandstoneHousewithBalconyDeed()
            : base( 0x3C, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone house with balcony"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThirteen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThirteen( owner, 0x3C );
        }

        #region serialization
        public SandstoneHousewithBalconyDeed( Serial serial )
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

    public class WoodenHousewithBalconyDeed : HouseDeed
    {
        [Constructable]
        public WoodenHousewithBalconyDeed()
            : base( 0x49, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a wooden house with balcony"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThirteen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThirteen( owner, 0x49 );
        }

        #region serialization
        public WoodenHousewithBalconyDeed( Serial serial )
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

    public class FillablePlasterHousewithBalconyDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterHousewithBalconyDeed()
            : base( 0x2F, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster house with balcony"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThirteen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThirteen( owner, 0x2F );
        }

        public override int Price
        {
            get { return 68000; }
        }

        public override int IronPerc
        {
            get { return 20; }
        }

        public override int StonePerc
        {
            get { return 60; }
        }

        public override int WoodPerc
        {
            get { return 20; }
        }

        #region serialization
        public FillablePlasterHousewithBalconyDeed( Serial serial )
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

    public class FillableSandstoneHousewithBalconyDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneHousewithBalconyDeed()
            : base( 0x3C, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone house with balcony"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThirteen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThirteen( owner, 0x3C );
        }

        public override int Price
        {
            get { return 68000; }
        }

        public override int IronPerc
        {
            get { return 20; }
        }

        public override int StonePerc
        {
            get { return 50; }
        }

        public override int WoodPerc
        {
            get { return 30; }
        }

        #region serialization
        public FillableSandstoneHousewithBalconyDeed( Serial serial )
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

    public class FillableWoodenHousewithBalconyDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableWoodenHousewithBalconyDeed()
            : base( 0x49, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a wooden house with balcony"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThirteen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThirteen( owner, 0x49 );
        }

        public override int Price
        {
            get { return 68000; }
        }

        public override int IronPerc
        {
            get { return 20; }
        }

        public override int StonePerc
        {
            get { return 20; }
        }

        public override int WoodPerc
        {
            get { return 60; }
        }

        #region serialization
        public FillableWoodenHousewithBalconyDeed( Serial serial )
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