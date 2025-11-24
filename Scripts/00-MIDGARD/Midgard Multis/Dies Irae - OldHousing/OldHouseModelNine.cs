using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelNine : BaseHouse
    {
        public static int AreaScalar = 210;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -7, 2, 14, 7 ), new Rectangle2D( -2, -10, 9, 12 ), new Rectangle2D( 7, 3, 1, 4 ) };

        public OldHouseModelNine( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( 6, 4, 5, keyValue );

            AddSouthDoor( 2, 2, 5 );

            AddSouthDoor( 3, 2, 25 );

            SetSign( 7, 7, 5 );
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
            get { return new Point3D( 7, 10, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x2b:
                    return new PlasterHallDeed();
                case 0x38:
                    return new SandstoneCareHomeDeed();
                case 0x45:
                    return new TreeFellerVillaModelTwoDeed();
                default:
                    return new PlasterHallDeed();
            }
        }

        #region serialization
        public OldHouseModelNine( Serial serial )
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
    public class PlasterHallDeed : HouseDeed
    {
        [Constructable]
        public PlasterHallDeed()
            : base( 0x2b, new Point3D( 0, 10, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster hall"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelNine.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelNine( owner, 0x2b );
        }

        #region serialization
        public PlasterHallDeed( Serial serial )
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

    public class SandstoneCareHomeDeed : HouseDeed
    {
        [Constructable]
        public SandstoneCareHomeDeed()
            : base( 0x38, new Point3D( 0, 10, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone care home"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelNine.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelNine( owner, 0x38 );
        }

        #region serialization
        public SandstoneCareHomeDeed( Serial serial )
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

    public class TreeFellerVillaModelTwoDeed : HouseDeed
    {
        [Constructable]
        public TreeFellerVillaModelTwoDeed()
            : base( 0x45, new Point3D( 0, 10, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller villa (two)"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelNine.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelNine( owner, 0x45 );
        }

        #region serialization
        public TreeFellerVillaModelTwoDeed( Serial serial )
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

    public class FillablePlasterHallDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterHallDeed()
            : base( 0x2b, new Point3D( 0, 10, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster hall"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelNine.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelNine( owner, 0x2b );
        }

        public override int Price
        {
            get { return 105000; }
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
        public FillablePlasterHallDeed( Serial serial )
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

    public class FillableSandstoneCareHomeDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneCareHomeDeed()
            : base( 0x38, new Point3D( 0, 10, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone care home"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelNine.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelNine( owner, 0x38 );
        }

        public override int Price
        {
            get { return 105000; }
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
        public FillableSandstoneCareHomeDeed( Serial serial )
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

    public class FillableTreeFellerVillaModelTwoDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableTreeFellerVillaModelTwoDeed()
            : base( 0x45, new Point3D( 0, 10, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller villa (two)"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelNine.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelNine( owner, 0x45 );
        }

        public override int Price
        {
            get { return 105000; }
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
        public FillableTreeFellerVillaModelTwoDeed( Serial serial )
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