using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelFour : BaseHouse
    {
        public static int AreaScalar = 125;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -6, -6, 11, 11 ), new Rectangle2D( 5, 0, 1, 4 ) };

        public OldHouseModelFour( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( 4, 1, 5, keyValue );

            AddEastDoor( ItemID == 0x4040 ? -2 : 0, -4, 5 );

            if( ItemID != 0x4040 )
                AddSouthDoor( -4, -3, 5 );

            SetSign( 5, ItemID == 0x4040 ? 3 : -1, 5 );
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
            get { return new Point3D( 5, 6, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x26:
                    return new PlasterShopDeed();
                case 0x33:
                    return new SandstoneResidenceDeed();
                case 0x40:
                    return new WoodenWareHouseDeed();
                default:
                    return new PlasterShopDeed();
            }
        }

        #region serialization
        public OldHouseModelFour( Serial serial )
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
    public class PlasterShopDeed : HouseDeed
    {
        [Constructable]
        public PlasterShopDeed()
            : base( 0x26, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster shop"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFour.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFour( owner, 0x26 );
        }

        #region serialization
        public PlasterShopDeed( Serial serial )
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

    public class SandstoneResidenceDeed : HouseDeed
    {
        [Constructable]
        public SandstoneResidenceDeed()
            : base( 0x33, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone residence"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFour.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFour( owner, 0x33 );
        }

        #region serialization
        public SandstoneResidenceDeed( Serial serial )
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

    public class WoodenWareHouseDeed : HouseDeed
    {
        [Constructable]
        public WoodenWareHouseDeed()
            : base( 0x40, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a wooden warehouse"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFour.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFour( owner, 0x40 );
        }

        #region serialization
        public WoodenWareHouseDeed( Serial serial )
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

    public class FillablePlasterShopDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterShopDeed()
            : base( 0x26, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster shop"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFour.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFour( owner, 0x26 );
        }

        public override int Price
        {
            get { return 62500; }
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
        public FillablePlasterShopDeed( Serial serial )
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

    public class FillableSandstoneResidenceDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneResidenceDeed()
            : base( 0x33, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone residence"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFour.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFour( owner, 0x33 );
        }

        public override int Price
        {
            get { return 62500; }
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
        public FillableSandstoneResidenceDeed( Serial serial )
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

    public class FillableWoodenWareHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableWoodenWareHouseDeed()
            : base( 0x40, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a wooden warehouse"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFour.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFour( owner, 0x40 );
        }

        public override int Price
        {
            get { return 62500; }
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
        public FillableWoodenWareHouseDeed( Serial serial )
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