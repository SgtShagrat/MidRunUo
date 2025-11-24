using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelTwo : BaseHouse
    {
        public static int AreaScalar = 231;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -12, -7, 23, 7 ), new Rectangle2D( -6, 0, 11, 6 ), new Rectangle2D( 0, 6, 4, 1 ) };

        public OldHouseModelTwo( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddSouthDoors( 1, 5, 5, keyValue );

            AddEastDoor( 4, -4, 5 );

            if( ItemID != 0x403E )
                AddEastDoor( -6, -4, 5 );

            SetSign( 0, 6, ItemID == 0x403E ? 10 : 5 );
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
            get { return new Point3D( 11, 7, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x24:
                    return new PlasterMerchantHouseDeed();
                case 0x31:
                    return new SandstoneMerchantHouseDeed();
                case 0x3E:
                    return new WoodenStockHouseDeed();
                default:
                    return new PlasterMerchantHouseDeed();
            }
        }

        #region serialization
        public OldHouseModelTwo( Serial serial )
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
    public class PlasterMerchantHouseDeed : HouseDeed
    {
        [Constructable]
        public PlasterMerchantHouseDeed()
            : base( 0x24, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster merchant's house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwo.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwo( owner, 0x24 );
        }

        #region serialization
        public PlasterMerchantHouseDeed( Serial serial )
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

    public class SandstoneMerchantHouseDeed : HouseDeed
    {
        [Constructable]
        public SandstoneMerchantHouseDeed()
            : base( 0x31, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone merchant's house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwo.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwo( owner, 0x31 );
        }

        #region serialization
        public SandstoneMerchantHouseDeed( Serial serial )
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

    public class WoodenStockHouseDeed : HouseDeed
    {
        [Constructable]
        public WoodenStockHouseDeed()
            : base( 0x3E, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a wooden stockhouse"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwo.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwo( owner, 0x3E );
        }

        #region serialization
        public WoodenStockHouseDeed( Serial serial )
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

    public class FillablePlasterMerchantHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterMerchantHouseDeed()
            : base( 0x24, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster merchant's house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwo.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwo( owner, 0x24 );
        }

        public override int Price
        {
            get { return 115500; }
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
        public FillablePlasterMerchantHouseDeed( Serial serial )
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

    public class FillableSandstoneMerchantHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneMerchantHouseDeed()
            : base( 0x31, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone merchant's house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwo.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwo( owner, 0x31 );
        }

        public override int Price
        {
            get { return 115500; }
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
        public FillableSandstoneMerchantHouseDeed( Serial serial )
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

    public class FillableWoodenStockHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableWoodenStockHouseDeed()
            : base( 0x3E, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a wooden stockhouse"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwo.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwo( owner, 0x3E );
        }

        public override int Price
        {
            get { return 115500; }
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
        public FillableWoodenStockHouseDeed( Serial serial )
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