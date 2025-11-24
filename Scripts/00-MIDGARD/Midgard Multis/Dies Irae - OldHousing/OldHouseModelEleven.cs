using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelEleven : BaseHouse
    {
        public static int AreaScalar = 124;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -6, -5, 12, 10 ), new Rectangle2D( 1, 5, 4, 1 ) };

        public OldHouseModelEleven( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddSouthDoors( 2, 4, 5, keyValue );

            AddEastDoor( 0, -3, 5 );

            SetSign( 1, 5, 4 );
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
            get { return new Point3D( 6, 6, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x2D:
                    return new PlasterWarehouseDeed();
                case 0x3A:
                    return new SandstoneWareHouseDeed();
                case 0x47:
                    return new TreeFellerStockHouseDeed();
                default:
                    return new PlasterWarehouseDeed();
            }
        }

        #region serialization
        public OldHouseModelEleven( Serial serial )
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
    public class PlasterWarehouseDeed : HouseDeed
    {
        [Constructable]
        public PlasterWarehouseDeed()
            : base( 0x2D, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster warehouse"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEleven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEleven( owner, 0x2D );
        }

        #region serialization
        public PlasterWarehouseDeed( Serial serial )
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

    public class SandstoneWareHouseDeed : HouseDeed
    {
        [Constructable]
        public SandstoneWareHouseDeed()
            : base( 0x3A, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone warehouse"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEleven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEleven( owner, 0x3A );
        }

        #region serialization
        public SandstoneWareHouseDeed( Serial serial )
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

    public class TreeFellerStockHouseDeed : HouseDeed
    {
        [Constructable]
        public TreeFellerStockHouseDeed()
            : base( 0x47, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller stockhouse"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEleven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEleven( owner, 0x47 );
        }

        #region serialization
        public TreeFellerStockHouseDeed( Serial serial )
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

    public class FillablePlasterWarehouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterWarehouseDeed()
            : base( 0x2D, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster warehouse"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEleven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEleven( owner, 0x2D );
        }

        public override int Price
        {
            get { return 62000; }
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
        public FillablePlasterWarehouseDeed( Serial serial )
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

    public class FillableSandstoneWareHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneWareHouseDeed()
            : base( 0x3A, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone warehouse"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEleven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEleven( owner, 0x3A );
        }

        public override int Price
        {
            get { return 62000; }
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
        public FillableSandstoneWareHouseDeed( Serial serial )
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

    public class FillableTreeFellerStockHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableTreeFellerStockHouseDeed()
            : base( 0x47, new Point3D( 0, 6, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller stockhouse"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEleven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEleven( owner, 0x47 );
        }

        public override int Price
        {
            get { return 62000; }
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
        public FillableTreeFellerStockHouseDeed( Serial serial )
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