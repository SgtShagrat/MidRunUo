using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelSeven : BaseHouse
    {
        public static int AreaScalar = 310;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -9, -9, 18, 17 ), new Rectangle2D( 9, -3, 1, 4 ) };

        public OldHouseModelSeven( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( 8, -2, 5, keyValue );

            if( ItemID != 0x4043 )
            {
                AddSouthDoor( -1, -1, 5 );

                AddEastDoor( -3, 3, 5 );
            }
            else
            {
                AddEastDoor( 0, 4, 5 );

                AddEastDoor( 0, -3, 5 );
            }

            SetSign( 9, 1, 4 );
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
            get { return new Point3D( 9, 9, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x29:
                    return new PlasterBourgeoisVillaDeed();
                case 0x36:
                    return new SandstoneVillaDeed();
                case 0x43:
                    return new TreeFellerVillaDeed();
                default:
                    return new PlasterBourgeoisVillaDeed();
            }
        }

        #region serialization
        public OldHouseModelSeven( Serial serial )
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
    public class PlasterBourgeoisVillaDeed : HouseDeed
    {
        [Constructable]
        public PlasterBourgeoisVillaDeed()
            : base( 0x29, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster bourgeois villa"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSeven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSeven( owner, 0x29 );
        }

        #region serialization
        public PlasterBourgeoisVillaDeed( Serial serial )
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

    public class SandstoneVillaDeed : HouseDeed
    {
        [Constructable]
        public SandstoneVillaDeed()
            : base( 0x36, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone villa"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSeven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSeven( owner, 0x36 );
        }

        #region serialization
        public SandstoneVillaDeed( Serial serial )
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

    public class TreeFellerVillaDeed : HouseDeed
    {
        [Constructable]
        public TreeFellerVillaDeed()
            : base( 0x43, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller villa"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSeven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSeven( owner, 0x43 );
        }

        #region serialization
        public TreeFellerVillaDeed( Serial serial )
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

    public class FillablePlasterBourgeoisVillaDeed : FillableHouseDeed
    {
        [Constructable]
        public FillablePlasterBourgeoisVillaDeed()
            : base( 0x29, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a plaster bourgeois villa"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSeven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSeven( owner, 0x29 );
        }

        public override int Price
        {
            get { return 155000; }
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
        public FillablePlasterBourgeoisVillaDeed( Serial serial )
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

    public class FillableSandstoneVillaDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneVillaDeed()
            : base( 0x36, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone villa"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSeven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSeven( owner, 0x36 );
        }

        public override int Price
        {
            get { return 155000; }
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
        public FillableSandstoneVillaDeed( Serial serial )
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

    public class FillableTreeFellerVillaDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableTreeFellerVillaDeed()
            : base( 0x43, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller villa"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSeven.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSeven( owner, 0x43 );
        }

        public override int Price
        {
            get { return 155000; }
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
        public FillableTreeFellerVillaDeed( Serial serial )
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
