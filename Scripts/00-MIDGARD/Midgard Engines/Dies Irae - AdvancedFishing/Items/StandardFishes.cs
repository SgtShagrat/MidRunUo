/***************************************************************************
 *                               StandardFishes.cs
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
    public class StandardFishOne : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 50; }
        }

        public override string DefaultName
        {
            get { return "a fish"; }
        }

        [Constructable]
        public StandardFishOne( int weight )
            : base( weight, 0x09CC )
        {
        }

        #region serialization
        public StandardFishOne( Serial serial )
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

    public class StandardFishTwo : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 50; }
        }

        public override string DefaultName
        {
            get { return "a fish"; }
        }

        [Constructable]
        public StandardFishTwo( int weight )
            : base( weight, 0x09CD )
        {
        }

        #region serialization
        public StandardFishTwo( Serial serial )
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

    public class StandardFishThree : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 50; }
        }

        public override string DefaultName
        {
            get { return "a fish"; }
        }

        [Constructable]
        public StandardFishThree( int weight )
            : base( weight, 0x09CE )
        {
        }

        #region serialization
        public StandardFishThree( Serial serial )
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

    public class StandardFishFour : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 50; }
        }

        [Constructable]
        public StandardFishFour( int weight )
            : base( weight, 0x09CF )
        {
        }

        #region serialization
        public StandardFishFour( Serial serial )
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

    public class StandardSmallFishesOne : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 30; }
        }

        public override string DefaultName
        {
            get { return "some small fishes"; }
        }

        [Constructable]
        public StandardSmallFishesOne( int weight )
            : base( weight, 0x0DD6 )
        {
        }

        #region serialization
        public StandardSmallFishesOne( Serial serial )
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

    public class StandardSmallFishesTwo : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 30; }
        }

        public override string DefaultName
        {
            get { return "some small fishes"; }
        }

        [Constructable]
        public StandardSmallFishesTwo( int weight )
            : base( weight, 0x0DD7 )
        {
        }

        #region serialization
        public StandardSmallFishesTwo( Serial serial )
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

    public class StandardSmallFishesThree : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 30; }
        }

        public override string DefaultName
        {
            get { return "some small fishes"; }
        }

        [Constructable]
        public StandardSmallFishesThree( int weight )
            : base( weight, 0x0DD7 )
        {
        }

        #region serialization
        public StandardSmallFishesThree( Serial serial )
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

    public class StandardSmallFishesFour : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 30; }
        }

        public override string DefaultName
        {
            get { return "some small fishes"; }
        }

        [Constructable]
        public StandardSmallFishesFour( int weight )
            : base( weight, 0x0DD8 )
        {
        }

        #region serialization
        public StandardSmallFishesFour( Serial serial )
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

    public class AlbinoCourtesanFish : BaseAdvancedFish
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public override int CarveScalar
        {
            get { return 110; }
        }

        public override string DefaultName
        {
            get { return "albino courtesan fish"; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override FishHabitat Habitat
        {
            get { return FishHabitat.Sea; }
        }

        [Constructable]
        public AlbinoCourtesanFish()
            : base( 0x3B04 )
        {
        }

        #region serialization
        public AlbinoCourtesanFish( Serial serial )
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