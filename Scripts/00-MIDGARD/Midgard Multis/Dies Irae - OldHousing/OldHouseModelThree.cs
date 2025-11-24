using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelThree : BaseHouse
    {
        public static int AreaScalar = 101;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -5, -7, 5, 13 ), new Rectangle2D( 0, -6, 1, 12 ), new Rectangle2D( 1, -2, 3, 8 ) };

        public OldHouseModelThree( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( -1, -5, 5, keyValue );

            AddSouthDoor( -3, -2, 5 );

            SetSign( 0, -6, 5 );
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
            get { return new Point3D( 4, 7, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x25:
                    return new PlasterSmallLabDeed();
                case 0x32:
                    return new SmallSandstoneWorkshopDeed();
                case 0x3F:
                    return new SmallWoodenWorkshopDeed();
                default:
                    return new PlasterSmallLabDeed();
            }
        }

        #region serialization
        public OldHouseModelThree( Serial serial )
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
    public class PlasterSmallLabDeed : HouseDeed
    {
        [Constructable]
        public PlasterSmallLabDeed()
            : base( 0x25, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster small lab"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThree.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThree( owner, 0x25 );
        }

        #region serialization
        public PlasterSmallLabDeed( Serial serial )
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

    public class SmallSandstoneWorkshopDeed : HouseDeed
    {
        [Constructable]
        public SmallSandstoneWorkshopDeed()
            : base( 0x32, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a small sandstone workshop"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThree.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThree( owner, 0x32 );
        }

        #region serialization
        public SmallSandstoneWorkshopDeed( Serial serial )
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

    public class SmallWoodenWorkshopDeed : HouseDeed
    {
        [Constructable]
        public SmallWoodenWorkshopDeed()
            : base( 0x3F, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "A deed for a house. (ID: 0x3F)"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThree.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThree( owner, 0x3F );
        }

        #region serialization
        public SmallWoodenWorkshopDeed( Serial serial )
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

    public class FillablePlasterSmallLabDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterSmallLabDeed()
            : base( 0x25, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster small lab"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThree.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThree( owner, 0x25 );
        }

        public override int Price
        {
            get { return 50500; }
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
        public FillablePlasterSmallLabDeed( Serial serial )
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

    public class FillableSmallSandstoneWorkshopDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSmallSandstoneWorkshopDeed()
            : base( 0x32, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a small sandstone workshop"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThree.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThree( owner, 0x32 );
        }

        public override int Price
        {
            get { return 50500; }
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
        public FillableSmallSandstoneWorkshopDeed( Serial serial )
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

    public class FillableSmallWoodenWorkshopDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSmallWoodenWorkshopDeed()
            : base( 0x3F, new Point3D( 0, 7, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "A deed for a house. (ID: 0x3F)"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelThree.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelThree( owner, 0x3F );
        }

        public override int Price
        {
            get { return 50500; }
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
        public FillableSmallWoodenWorkshopDeed( Serial serial )
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