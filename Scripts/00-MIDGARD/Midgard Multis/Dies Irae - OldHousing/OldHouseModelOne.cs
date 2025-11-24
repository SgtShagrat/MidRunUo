using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelOne : BaseHouse
    {
        public static int AreaScalar = 144;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -6, -8, 7, 15 ), new Rectangle2D( 1, -6, 1, 4 ), new Rectangle2D( 1, 0, 5, 7 ) };

        public OldHouseModelOne( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( 0, -5, 5, keyValue );

            AddSouthDoor( -3, 0, 5 );

            SetSign( 2, ItemID == 0x403D ? -5 : -6, 14 );
            ChangeSignType( 0xBD1 );
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
            get { return new Point3D( 6, 8, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x23:
                    return new PlasterWorkshopDeed();
                case 0x30:
                    return new LargeSandstoneWorkshopDeed();
                case 0x3D:
                    return new TreeFellerWorkshopDeed();
                default:
                    return new PlasterWorkshopDeed();
            }
        }

        #region serialization
        public OldHouseModelOne( Serial serial )
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
    public class PlasterWorkshopDeed : HouseDeed
    {
        [Constructable]
        public PlasterWorkshopDeed()
            : base( 0x23, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster workshop"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelOne.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelOne( owner, 0x23 );
        }

        #region serialization
        public PlasterWorkshopDeed( Serial serial )
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

    public class LargeSandstoneWorkshopDeed : HouseDeed
    {
        [Constructable]
        public LargeSandstoneWorkshopDeed()
            : base( 0x30, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a large sandstone workshop"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelOne.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelOne( owner, 0x30 );
        }

        #region serialization
        public LargeSandstoneWorkshopDeed( Serial serial )
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

    public class TreeFellerWorkshopDeed : HouseDeed
    {
        [Constructable]
        public TreeFellerWorkshopDeed()
            : base( 0x3D, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller workshop"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelOne.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelOne( owner, 0x3D );
        }

        #region serialization
        public TreeFellerWorkshopDeed( Serial serial )
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

    public class FillablePlasterWorkshopDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterWorkshopDeed()
            : base( 0x23, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster workshop"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelOne.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelOne( owner, 0x23 );
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
        public FillablePlasterWorkshopDeed( Serial serial )
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

    public class FillableLargeSandstoneWorkshopDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableLargeSandstoneWorkshopDeed()
            : base( 0x30, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a large sandstone workshop"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelOne.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelOne( owner, 0x30 );
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
        public FillableLargeSandstoneWorkshopDeed( Serial serial )
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

    public class FillableTreeFellerWorkshopDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableTreeFellerWorkshopDeed()
            : base( 0x3D, new Point3D( 0, 8, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller workshop"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelOne.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelOne( owner, 0x3D );
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
        public FillableTreeFellerWorkshopDeed( Serial serial )
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