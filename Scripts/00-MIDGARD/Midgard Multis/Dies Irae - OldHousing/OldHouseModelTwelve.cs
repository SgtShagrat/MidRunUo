using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelTwelve : BaseHouse
    {
        public static int AreaScalar = 136;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -6, -6, 11, 12 ), new Rectangle2D( 5, -4, 1, 4 ) };

        public OldHouseModelTwelve( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( 4, -3, 5, keyValue );

            AddEastDoor( -1, -4, 5 );

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
                case 0x2E:
                    return new PlasterParsonageDeed();
                case 0x3B:
                    return new SandstoneGateHouseDeed();
                case 0x48:
                    return new LargeTreeFellerHouseDeed();
                default:
                    return new PlasterParsonageDeed();
            }
        }

        #region serialization
        public OldHouseModelTwelve( Serial serial )
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
    public class PlasterParsonageDeed : HouseDeed
    {
        [Constructable]
        public PlasterParsonageDeed()
            : base( 0x2E, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster parsonage"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwelve.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwelve( owner, 0x2E );
        }

        #region serialization
        public PlasterParsonageDeed( Serial serial )
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

    public class SandstoneGateHouseDeed : HouseDeed
    {
        [Constructable]
        public SandstoneGateHouseDeed()
            : base( 0x3B, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone gate house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwelve.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwelve( owner, 0x3B );
        }

        #region serialization
        public SandstoneGateHouseDeed( Serial serial )
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

    public class LargeTreeFellerHouseDeed : HouseDeed
    {
        [Constructable]
        public LargeTreeFellerHouseDeed()
            : base( 0x48, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a large tree feller house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwelve.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwelve( owner, 0x48 );
        }

        #region serialization
        public LargeTreeFellerHouseDeed( Serial serial )
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

    public class FillablePlasterParsonageDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterParsonageDeed()
            : base( 0x2E, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster parsonage"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwelve.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwelve( owner, 0x2E );
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
        public FillablePlasterParsonageDeed( Serial serial )
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

    public class FillableSandstoneGateHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneGateHouseDeed()
            : base( 0x3B, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone gate house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwelve.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwelve( owner, 0x3B );
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
        public FillableSandstoneGateHouseDeed( Serial serial )
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

    public class FillableLargeTreeFellerHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableLargeTreeFellerHouseDeed()
            : base( 0x48, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a large tree feller house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelTwelve.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelTwelve( owner, 0x48 );
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
        public FillableLargeTreeFellerHouseDeed( Serial serial )
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