using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelSix : BaseHouse
    {
        public static int AreaScalar = 230;

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -9, -9, 17, 8 ), new Rectangle2D( -2, -1, 10, 9 ), new Rectangle2D( 8, 2, 1, 4 ) };

        public OldHouseModelSix( Mobile owner, int id )
            : base( id, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( 7, 3, 5, keyValue );

            AddEastDoor( -2, -5, 5 );

            SetSign( 8, 6, 5 );
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
            get { return new Point3D( 8, 9, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x28:
                    return new LargePlasterCivilianHouseDeed();
                case 0x35:
                    return new SandstoneCivilianHouseDeed();
                case 0x42:
                    return new TreeFellerRefugeDeed();
                default:
                    return new LargePlasterCivilianHouseDeed();
            }
        }

        #region serialization
        public OldHouseModelSix( Serial serial )
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
    public class LargePlasterCivilianHouseDeed : HouseDeed
    {
        [Constructable]
        public LargePlasterCivilianHouseDeed()
            : base( 0x28, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a large plaster civilian house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSix.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSix( owner, 0x28 );
        }

        #region serialization
        public LargePlasterCivilianHouseDeed( Serial serial )
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

    public class SandstoneCivilianHouseDeed : HouseDeed
    {
        [Constructable]
        public SandstoneCivilianHouseDeed()
            : base( 0x35, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone civilian house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSix.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSix( owner, 0x35 );
        }

        #region serialization
        public SandstoneCivilianHouseDeed( Serial serial )
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

    public class TreeFellerRefugeDeed : HouseDeed
    {
        [Constructable]
        public TreeFellerRefugeDeed()
            : base( 0x42, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller refuge"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSix.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSix( owner, 0x42 );
        }

        #region serialization
        public TreeFellerRefugeDeed( Serial serial )
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

    public class FillableLargePlasterCivilianHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableLargePlasterCivilianHouseDeed()
            : base( 0x28, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a large plaster civilian house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSix.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSix( owner, 0x28 );
        }

        public override int Price
        {
            get { return 115000; }
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
        public FillableLargePlasterCivilianHouseDeed( Serial serial )
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

    public class FillableSandstoneCivilianHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSandstoneCivilianHouseDeed()
            : base( 0x35, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a sandstone civilian house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSix.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSix( owner, 0x35 );
        }

        public override int Price
        {
            get { return 115000; }
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
        public FillableSandstoneCivilianHouseDeed( Serial serial )
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

    public class FillableTreeFellerRefugeDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableTreeFellerRefugeDeed()
            : base( 0x42, new Point3D( 0, 9, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a tree feller refuge"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelSix.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelSix( owner, 0x42 );
        }

        public override int Price
        {
            get { return 115000; }
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
        public FillableTreeFellerRefugeDeed( Serial serial )
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