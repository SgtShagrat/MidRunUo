using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class CottonSeed : BaseSeed
    {
        #region proprietà da BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( CottonCrop ) }; } }
        public override string PlantName { get { return "cotton"; } }
        #endregion

        #region proprietà di ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public CottonSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public CottonSeed( int amount )
            : base( amount )
        {
        }

        public CottonSeed( Serial serial )
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

    public class CottonCrop : BaseCrop
    {
        #region proprietà da BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0xC52, 0xC54, 0xC50 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "cotton seedlings", "young cottons", "cotton plants" }; } }
        public override double MinSkillToHarvest { get { return 50; } }
        public override Type TypeOfParentSeed { get { return typeof( CottonSeed ); } }

        // Variables concerning produce action
        public override string CropName { get { return "cotton"; } }
        public override string CropPluralName { get { return "cotton"; } }
        public override int ProductPhaseID { get { return 0; } }
        #endregion

        #region costruttori
        [Constructable]
        public CottonCrop( Mobile owner )
            : base( owner )
        {
        }

        public CottonCrop( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Cotton();
        }

        public override int GetPickedID()
        {
            return Utility.Random( 0xC53, 2 );
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
