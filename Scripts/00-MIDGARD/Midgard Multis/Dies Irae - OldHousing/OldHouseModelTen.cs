using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelTen : BaseHouse
    {
        public static int AreaScalar = 173;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -8, -8, 15, 7 ), new Rectangle2D( -1, -1, 8, 8 ), new Rectangle2D( 7, 2, 1, 4 ) };

        public OldHouseModelTen( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( 6, 3, 5, keyValue );

            AddEastDoor( -1, -5, 5 );

            SetSign( 7, 2, 4 );
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
            get { return new Point3D( 7, 8, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x2C:
                    return new PlasterCivilianHouseDeed();
                case 0x39:
                    return new SandstoneLabDeed();
                case 0x46:
                    return new WoodenLabDeed();
                default:
                    return new PlasterCivilianHouseDeed();
            }
        }

        #region serialization
        public OldHouseModelTen( Serial serial )
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
    public class PlasterCivilianHouseDeed : HouseDeed
    {
        [Constructable]
        public PlasterCivilianHouseDeed()
            : base( 0x2C, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster civilian house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTen( owner, 0x2C );
        }

        #region serialization
        public PlasterCivilianHouseDeed( Serial serial )
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

    public class SandstoneLabDeed : HouseDeed
    {
        [Constructable]
        public SandstoneLabDeed()
            : base( 0x39, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone lab"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTen( owner, 0x39 );
        }

        #region serialization
        public SandstoneLabDeed( Serial serial )
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

    public class WoodenLabDeed : HouseDeed
    {
        [Constructable]
        public WoodenLabDeed()
            : base( 0x46, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "A deed for a house. (ID: 0x46)"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTen( owner, 0x46 );
        }

        #region serialization
        public WoodenLabDeed( Serial serial )
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

    public class FillablePlasterCivilianHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterCivilianHouseDeed()
            : base( 0x2C, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster civilian house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTen( owner, 0x2C );
        }

        public override int Price
        {
            get { return 86500; }
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
        public FillablePlasterCivilianHouseDeed( Serial serial )
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

    public class FillableSandstoneLabDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneLabDeed()
            : base( 0x39, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone lab"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTen( owner, 0x39 );
        }

        public override int Price
        {
            get { return 86500; }
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
        public FillableSandstoneLabDeed( Serial serial )
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

    public class FillableWoodenLabDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableWoodenLabDeed()
            : base( 0x46, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "A deed for a house. (ID: 0x46)"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTen( owner, 0x46 );
        }

        public override int Price
        {
            get { return 86500; }
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
        public FillableWoodenLabDeed( Serial serial )
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