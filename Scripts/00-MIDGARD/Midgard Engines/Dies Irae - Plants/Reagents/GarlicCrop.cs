using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class GarlicSeed : BaseSeed
    {
        #region proprietà da BaseSeed

        public override Type[] PlantTypes
        {
            get { return new Type[] { typeof( GarlicCrop ) }; }
        }

        public override string PlantName
        {
            get { return "garlic"; }
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
        public GarlicSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public GarlicSeed( int amount )
            : base( amount )
        {
        }

        public GarlicSeed( Serial serial )
            : base( serial )
        {
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

    public class GarlicCrop : BaseCrop
    {
        #region proprietà da BasePlant

        // Variables concerning phases and ids
        public override int[] PhaseIDs
        {
            get { return new int[] { 0x913, 0xCB5, 0xCB0, 0x18E1 }; }
        }

        public override string[] PhaseName
        {
            get { return new string[] { "a pile of dirt", "garlic seedlings", "young garlic", "garlic plants" }; }
        }

        public override double MinSkillToHarvest
        {
            get { return 70; }
        }

        public override Type TypeOfParentSeed
        {
            get { return typeof( GarlicSeed ); }
        }

        // Variables concerning produce action
        public override string CropName
        {
            get { return "garlic bulb"; }
        }

        public override string CropPluralName
        {
            get { return "garlic bulbs"; }
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
        public GarlicCrop( Mobile owner )
            : base( owner )
        {
        }

        public GarlicCrop( Serial serial )
            : base( serial )
        {
        }

        #endregion

        #region metodi

        public override Item GetCropObject()
        {
            GarlicBulb garlic = new GarlicBulb();

            garlic.ItemID = Utility.Random( 0x18E3, 2 );

            return garlic;
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

    [Flipable( 0x18E3, 0x18E4 )]
    public class GarlicBulb : Item
    {
        #region costruttori

        [Constructable]
        public GarlicBulb()
            : this( 1 )
        {
        }

        [Constructable]
        public GarlicBulb( int amount )
            : base( 0x18E3 )
        {
            Stackable = true;
            Amount = amount;
            Weight = 1.0;
        }

        public GarlicBulb( Serial serial )
            : base( serial )
        {
        }

        #endregion

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, String.Format( "{0}raw garlic", ( Amount == 1 ) ? "" : Amount + " " ) );
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