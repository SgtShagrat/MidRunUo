using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class GinsengSeed : BaseSeed
    {
        #region proprietà da BaseSeed

        public override Type[] PlantTypes
        {
            get { return new Type[] { typeof( GinsengCrop ) }; }
        }

        public override string PlantName
        {
            get { return "ginseng"; }
        }

        public override double RequiredSkillToPlant
        {
            get { return 70.0; }
        }

        #endregion

        #region proprietà di ISowable

        public override bool CanGrowFarm
        {
            get { return true; }
        }

        #endregion

        #region costruttori

        [Constructable]
        public GinsengSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public GinsengSeed( int amount )
            : base( amount )
        {
        }

        public GinsengSeed( Serial serial )
            : base( serial )
        {
        }

        #endregion

        #region serial-deserial

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }

        #endregion
    }

    public class GinsengCrop : BaseCrop
    {
        #region proprietà da BasePlant

        // Variables concerning phases and ids
        public override int[] PhaseIDs
        {
            get { return new int[] { 0x913, 0xCB5, 0xCB0, 0x18E9 }; }
        }

        public override string[] PhaseName
        {
            get { return new string[] { "a pile of dirt", "ginseng seedlings", "young ginseng", "ginseng plants" }; }
        }

        public override double MinSkillToHarvest
        {
            get { return 50; }
        }

        public override Type TypeOfParentSeed
        {
            get { return typeof( GinsengSeed ); }
        }

        // Variables concerning produce action
        public override string CropName
        {
            get { return "ginseng"; }
        }

        public override string CropPluralName
        {
            get { return "ginseng plants"; }
        }

        public override int ProductPhaseID
        {
            get { return 0; }
        }

        public override double MinDiffSkillToCare
        {
            get { return 70; }
        }

        #endregion

        #region costruttori

        [Constructable]
        public GinsengCrop( Mobile owner )
            : base( owner )
        {
        }

        public GinsengCrop( Serial serial )
            : base( serial )
        {
        }

        #endregion

        #region metodi

        public override Item GetCropObject()
        {
            RawGinsengRoot ginseng = new RawGinsengRoot();

            ginseng.ItemID = Utility.Random( 0x18EB, 2 );

            return ginseng;
        }

        public override int GetPickedID()
        {
            return 0xCB0;
        }

        #endregion

        #region serial-deserial

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

    [Flipable( 0x18EB, 0x18EC )]
    public class RawGinsengRoot : Item
    {
        #region costruttori

        [Constructable]
        public RawGinsengRoot()
            : this( 1 )
        {
        }

        [Constructable]
        public RawGinsengRoot( int amount )
            : base( 0x18EB )
        {
            Stackable = true;
            Amount = amount;
            Weight = 1.0;
        }

        public RawGinsengRoot( Serial serial )
            : base( serial )
        {
        }

        #endregion

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, String.Format( "{0}raw ginseng root", ( Amount == 1 ) ? "" : Amount + " " ) );
        }

        #region Serial-Deserial

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