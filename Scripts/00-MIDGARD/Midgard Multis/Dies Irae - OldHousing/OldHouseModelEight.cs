using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelEight : BaseHouse
    {
        public static int AreaScalar = 144;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -5, -7, 10, 14 ), new Rectangle2D( -3, 7, 4, 1 ) };

        public OldHouseModelEight( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddSouthDoors( -2, 6, 5, keyValue );

            AddSouthDoor( 2, ItemID == 0x4044 ? -1 : -2, 5 );

            SetSign( -3, 7, 5 );
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
            get { return new Point3D( 5, 8, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x2A:
                    return new PlasterGateHouseDeed();
                case 0x37:
                    return new SandstoneRefugeDeed();
                case 0x44:
                    return new TreeFellerHouseDeed();
                default:
                    return new PlasterGateHouseDeed();
            }
        }

        #region serialization
        public OldHouseModelEight( Serial serial )
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
    public class PlasterGateHouseDeed : HouseDeed
    {
        [Constructable]
        public PlasterGateHouseDeed()
            : base( 0x2A, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster gate house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEight.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEight( owner, 0x2A );
        }

        #region serialization
        public PlasterGateHouseDeed( Serial serial )
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

    public class SandstoneRefugeDeed : HouseDeed
    {
        [Constructable]
        public SandstoneRefugeDeed()
            : base( 0x37, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone refuge"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEight.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEight( owner, 0x37 );
        }

        #region serialization
        public SandstoneRefugeDeed( Serial serial )
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

    public class TreeFellerHouseDeed : HouseDeed
    {
        [Constructable]
        public TreeFellerHouseDeed()
            : base( 0x44, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEight.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEight( owner, 0x44 );
        }

        #region serialization
        public TreeFellerHouseDeed( Serial serial )
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

    public class FillablePlasterGateHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterGateHouseDeed()
            : base( 0x2A, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster gate house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEight.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEight( owner, 0x2A );
        }

        public override int Price
        {
            get { return 72000; }
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
        public FillablePlasterGateHouseDeed( Serial serial )
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

    public class FillableSandstoneRefugeDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneRefugeDeed()
            : base( 0x37, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone refuge"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEight.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEight( owner, 0x37 );
        }

        public override int Price
        {
            get { return 72000; }
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
        public FillableSandstoneRefugeDeed( Serial serial )
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

    public class FillableTreeFellerHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableTreeFellerHouseDeed()
            : base( 0x44, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelEight.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelEight( owner, 0x44 );
        }

        public override int Price
        {
            get { return 72000; }
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
        public FillableTreeFellerHouseDeed( Serial serial )
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