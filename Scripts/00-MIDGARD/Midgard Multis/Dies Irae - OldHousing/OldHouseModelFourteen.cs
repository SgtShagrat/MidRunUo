using System;
using Server;
using Server.Multis;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Midgard.Multis
{
    public class OldHouseModelFourteen : BaseHouse
    {
        public static int AreaScalar = 76;

        // fix by Magius(CHE)
        // spero che questa classe venga usata solo per le cae con la porta ad EST, perchè altrimenti generare il baco opposto
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -5, -4, 8, 9 ), /*new Rectangle2D( 4, -2, 4, 1 ), };*/ new Rectangle2D( 4, -2, 1, 4 ), };

        public OldHouseModelFourteen( Mobile owner )
            : base( 0x4A, owner, Math.Max( AreaScalar / 3, 1 ), Math.Max( AreaScalar / 50, 1 ) )
        {
            uint keyValue = CreateKeys( owner );

            AddEastDoors( 3, -1, 5, keyValue );

            AddEastDoor( -1, -2, 5 );

            SetSign( 4, -2, 5 );
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
            get { return new Point3D( 4, 5, 0 ); }
        }

        public override HouseDeed GetDeed()
        {
            return new SmallTreeFellerHouseDeed();
        }

        #region serialization
        public OldHouseModelFourteen( Serial serial )
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
    public class SmallTreeFellerHouseDeed : HouseDeed
    {
        [Constructable]
        public SmallTreeFellerHouseDeed()
            : base( 0x4A, new Point3D( 0, 5, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a small tree feller house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFourteen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFourteen( owner );
        }

        #region serialization
        public SmallTreeFellerHouseDeed( Serial serial )
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

    public class FillableSmallTreeFellerHouseDeed : FillableHouseDeed
    {
        [Constructable]
        public FillableSmallTreeFellerHouseDeed()
            : base( 0x4A, new Point3D( 0, 5, 0 ) )
        {
        }

        public override string DefaultName
        {
            get { return "deed to a small tree feller house"; }
        }

        public override Rectangle2D[] Area
        {
            get { return OldHouseModelFourteen.AreaArray; }
        }

        public override BaseHouse GetHouse( Mobile owner )
        {
            return new OldHouseModelFourteen( owner );
        }

        public override int Price
        {
            get { return 38000; }
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
        public FillableSmallTreeFellerHouseDeed( Serial serial )
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