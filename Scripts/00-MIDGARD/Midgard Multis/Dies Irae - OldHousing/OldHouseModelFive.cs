using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelFive : BaseHouse
    {
        public static int AreaScalar = 158;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -6, -7, 11, 14 ), new Rectangle2D( 5, 0, 1, 4 ) };

        public OldHouseModelFive( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( 4, 1, 5, keyValue );

            AddEastDoor( -2, -4, 5 );

            if( ItemID == 0x4041 )
                AddSouthDoor( -4, -2, 25 );
            else
                AddEastDoor( -1, 0, 25 );

            SetSign( 5, 4, 5 );
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
            get { return new Point3D( 5, 8, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x27:
                    return new PlasterCareHomeDeed();
                case 0x34:
                    return new SandstoneTwoStoryHouseDeed();
                case 0x41:
                    return new TwoStoryTreeFellerHouseDeed();
                default:
                    return new PlasterCareHomeDeed();
            }
        }

        #region serialization
        public OldHouseModelFive( Serial serial )
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
    public class PlasterCareHomeDeed : HouseDeed
    {
        [Constructable]
        public PlasterCareHomeDeed()
            : base( 0x27, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster care home"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFive.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFive( owner, 0x27 );
        }

        #region serialization
        public PlasterCareHomeDeed( Serial serial )
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

    public class SandstoneTwoStoryHouseDeed : HouseDeed
    {
        [Constructable]
        public SandstoneTwoStoryHouseDeed()
            : base( 0x34, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone two story house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFive.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFive( owner, 0x34 );
        }

        #region serialization
        public SandstoneTwoStoryHouseDeed( Serial serial )
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

    public class TwoStoryTreeFellerHouseDeed : HouseDeed
    {
        [Constructable]
        public TwoStoryTreeFellerHouseDeed()
            : base( 0x41, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a two story tree feller house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFive.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFive( owner, 0x41 );
        }

        #region serialization
        public TwoStoryTreeFellerHouseDeed( Serial serial )
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

    public class FillablePlasterCareHomeDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterCareHomeDeed()
            : base( 0x27, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster care home"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFive.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFive( owner, 0x27 );
        }

        public override int Price
        {
            get { return 79000; }
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
        public FillablePlasterCareHomeDeed( Serial serial )
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

    public class FillableSandstoneTwoStoryHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneTwoStoryHouseDeed()
            : base( 0x34, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone two story house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFive.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFive( owner, 0x34 );
        }

        public override int Price
        {
            get { return 79000; }
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
        public FillableSandstoneTwoStoryHouseDeed( Serial serial )
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

    public class FillableTwoStoryTreeFellerHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableTwoStoryTreeFellerHouseDeed()
            : base( 0x41, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a two story tree feller house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFive.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFive( owner, 0x41 );
        }

        public override int Price
        {
            get { return 79000; }
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
        public FillableTwoStoryTreeFellerHouseDeed( Serial serial )
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