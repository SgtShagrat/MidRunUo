/***************************************************************************
 *                               Fishes.cs
 *
 *   begin                : 23 settembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.AdvancedFishing
{
    public class PiperFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 100; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Sea; }
        }

        public override string DefaultName
        {
            get { return "a piper fish"; }
        }

        [Constructable]
        public PiperFish( int weight )
            : base( weight, 0x3F1D )
        {
        }

        #region serialization
        public PiperFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class SalmonFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 100; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Sea; }
        }

        public override string DefaultName
        {
            get { return "a salmon fish"; }
        }

        [Constructable]
        public SalmonFish( int weight )
            : base( weight, 0x3F15 )
        {
        }

        #region serialization
        public SalmonFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class GranchioFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 500; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Sea; }
        }

        public override string DefaultName
        {
            get { return "a crab"; }
        }

        [Constructable]
        public GranchioFish( int weight )
            : base( weight, 0x3F23 )
        {
        }

        #region serialization
        public GranchioFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class MarlinFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 100; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Sea; }
        }

        public override string DefaultName
        {
            get { return "a blue marlin fish"; }
        }

        [Constructable]
        public MarlinFish( int weight )
            : base( weight, 0x3F25 )
        {
        }

        #region serialization
        public MarlinFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class MantaFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 300; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Sea; }
        }

        public override string DefaultName
        {
            get { return "a manta fish"; }
        }

        [Constructable]
        public MantaFish( int weight )
            : base( weight, 0x3F1E )
        {
        }

        #region serialization
        public MantaFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class CarpFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 700; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Lake; }
        }

        public override string DefaultName
        {
            get { return "a carp fish"; }
        }

        [Constructable]
        public CarpFish( int weight )
            : base( weight, 0x3F13 )
        {
        }

        #region serialization
        public CarpFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class CatFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 600; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Lake; }
        }

        public override string DefaultName
        {
            get { return "a cat fish"; }
        }

        [Constructable]
        public CatFish( int weight )
            : base( weight, 0x3F12 )
        {
        }

        #region serialization
        public CatFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class SmallMouthFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 400; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Lake; }
        }

        public override string DefaultName
        {
            get { return "a small mouth fish"; }
        }

        [Constructable]
        public SmallMouthFish( int weight )
            : base( weight, 0x3F14 )
        {
        }

        #region serialization
        public SmallMouthFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class TropicalFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 900; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Sea; }
        }

        public override string DefaultName
        {
            get { return "a tropical fish"; }
        }

        [Constructable]
        public TropicalFish( int weight )
            : base( weight, Utility.Random( 0x3F20, 3 ) )
        {
        }

        #region serialization
        public TropicalFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class TenchFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 550; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Lake; }
        }

        public override string DefaultName
        {
            get { return "a tench fish"; }
        }

        [Constructable]
        public TenchFish( int weight )
            : base( weight, 0x3F0A )
        {
        }

        #region serialization
        public TenchFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class BleakFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 900; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Lake; }
        }

        public override string DefaultName
        {
            get { return "a bleak fish"; }
        }

        [Constructable]
        public BleakFish( int weight )
            : base( weight, 0x3F18 )
        {
        }

        #region serialization
        public BleakFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class ChubFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 1000; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Lake; }
        }

        public override string DefaultName
        {
            get { return "a chub fish"; }
        }

        [Constructable]
        public ChubFish( int weight )
            : base( weight, 0x3F0C )
        {
        }

        #region serialization
        public ChubFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}