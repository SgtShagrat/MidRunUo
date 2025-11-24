using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class PumpkinSeed : BaseSeed
    {
        #region proprietà da BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( PumpkinCrop ) }; } }
        public override string PlantName { get { return "pumpkin"; } }
        #endregion

        #region proprietà di ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public PumpkinSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public PumpkinSeed( int amount )
            : base( amount )
        {
        }

        public PumpkinSeed( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }

    public class PumpkinCrop : BaseCrop
    {
        #region proprietà da BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0xC5E, 0xC5F, 0xC6A }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "pumpkin seedlings", "young pumpkins", "pumpkin plants" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( PumpkinSeed ); } }

        // Variables concerning produce action
        public override string CropName { get { return "pumpkin"; } }
        public override string CropPluralName { get { return "pumpkins"; } }
        public override int ProductPhaseID { get { return 0; } }
        #endregion

        #region costruttori
        [Constructable]
        public PumpkinCrop( Mobile owner )
            : base( owner )
        {
        }

        public PumpkinCrop( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            Pumpkin pumpkin = new Pumpkin();

            pumpkin.ItemID = Utility.Random( 0xC6A, 3 );

            return pumpkin;
        }

        public override int GetPickedID()
        {
            return 0xC5F;
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }
}
