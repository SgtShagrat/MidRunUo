using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class MandrakeSeed : BaseSeed
    {
        #region proprietà da BaseSeed

        public override Type[] PlantTypes
        {
            get { return new Type[] { typeof( MandrakeCrop ) }; }
        }

        public override string PlantName
        {
            get { return "mandrake"; }
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
        public MandrakeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public MandrakeSeed( int amount )
            : base( amount )
        {
        }

        public MandrakeSeed( Serial serial )
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

    public class MandrakeCrop : BaseCrop
    {
        #region proprietà da BasePlant

        // Variables concerning phases and ids
        public override int[] PhaseIDs
        {
            get { return new int[] { 0x913, 0xCB5, 0xCB0, 0x18DF }; }
        }

        public override string[] PhaseName
        {
            get { return new string[] { "a pile of dirt", "mandrake seedlings", "young mandrake", "mandrake plants" }; }
        }

        public override double MinSkillToHarvest
        {
            get { return 50; }
        }

        public override Type TypeOfParentSeed
        {
            get { return typeof( MandrakeSeed ); }
        }

        // Variables concerning produce action
        public override string CropName
        {
            get { return "mandrake"; }
        }

        public override string CropPluralName
        {
            get { return "mandrake plants"; }
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
        public MandrakeCrop( Mobile owner )
            : base( owner )
        {
        }

        public MandrakeCrop( Serial serial )
            : base( serial )
        {
        }

        #endregion

        #region metodi

        public override Item GetCropObject()
        {
            RawMandrakeRoot mandrake = new RawMandrakeRoot();

            mandrake.ItemID = Utility.Random( 0x18DD, 2 );

            return mandrake;
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

    [Flipable( 0x18DD, 0x18DE )]
    public class RawMandrakeRoot : Item
    {
        #region costruttori

        [Constructable]
        public RawMandrakeRoot()
            : this( 1 )
        {
        }

        [Constructable]
        public RawMandrakeRoot( int amount )
            : base( 0x18DD )
        {
            Stackable = true;
            Amount = amount;
            Weight = 1.0;
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, String.Format( "{0}raw mandrake root", ( Amount == 1 ) ? "" : Amount + " " ) );
        }

        public RawMandrakeRoot( Serial serial )
            : base( serial )
        {
        }

        #endregion

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

            if( ItemID == 0x18E3 )
                ItemID = 0x18DD;
        }

        #endregion
    }
}