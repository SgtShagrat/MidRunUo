using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class NightshadeSeed : BaseSeed
    {
        #region proprietà da BaseSeed

        public override Type[] PlantTypes
        {
            get { return new Type[] { typeof( NightshadeCrop ) }; }
        }

        public override string PlantName
        {
            get { return "nightshade"; }
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
        public NightshadeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public NightshadeSeed( int amount )
            : base( amount )
        {
        }

        public NightshadeSeed( Serial serial )
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

    public class NightshadeCrop : BaseCrop
    {
        #region proprietà da BasePlant

        // Variables concerning phases and ids
        public override int[] PhaseIDs
        {
            get { return new int[] { 0x913, 0xCB5, 0xCB0, 0x18E5 }; }
        }

        public override string[] PhaseName
        {
            get { return new string[] { "a pile of dirt", "nightshade seedlings", "young nightshade", "nightshade plants" }; }
        }

        public override double MinSkillToHarvest
        {
            get { return 70; }
        }

        public override Type TypeOfParentSeed
        {
            get { return typeof( NightshadeSeed ); }
        }

        // Variables concerning produce action
        public override string CropName
        {
            get { return "nightshade"; }
        }

        public override string CropPluralName
        {
            get { return "nightshade plants"; }
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
        public NightshadeCrop( Mobile owner )
            : base( owner )
        {
        }

        public NightshadeCrop( Serial serial )
            : base( serial )
        {
        }

        #endregion

        #region metodi

        public override Item GetCropObject()
        {
            RawNightshade nightshade = new RawNightshade();

            nightshade.ItemID = Utility.Random( 0x18E7, 2 );

            return nightshade;
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

    [Flipable( 0x18E7, 0x18E8 )]
    public class RawNightshade : Item
    {
        [Constructable]
        public RawNightshade()
            : this( 1 )
        {
        }

        [Constructable]
        public RawNightshade( int amount )
            : base( 0x18E7 )
        {
            Stackable = true;
            Amount = amount;
            Weight = 1.0;
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, String.Format( "{0}raw nightshade", ( Amount == 1 ) ? "" : Amount + " " ) );
        }

        #region Serial-Deserial
        public RawNightshade( Serial serial )
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