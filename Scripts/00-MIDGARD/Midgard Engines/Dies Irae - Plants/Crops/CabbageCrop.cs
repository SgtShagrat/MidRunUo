using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class CabbageSeed : BaseSeed
    {
        #region proprietà da BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( CabbageCrop ) }; } }
        public override string PlantName { get { return "cabbage"; } }
        #endregion

        #region proprietà di ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public CabbageSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public CabbageSeed( int amount )
            : base( amount )
        {
        }

        public CabbageSeed( Serial serial )
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

    public class CabbageCrop : BaseCrop
    {
        #region proprietà da BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0xCB5, 0xC61, 0xC7C }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "cabbage seedlings", "young cabbages", "cabbage plants" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( CabbageSeed ); } }

        // Variables concerning produce action
        public override string CropName { get { return "cabbage"; } }
        public override string CropPluralName { get { return "cabbages"; } }
        public override int ProductPhaseID { get { return 0; } }
        #endregion

        #region costruttori
        [Constructable]
        public CabbageCrop( Mobile owner )
            : base( owner )
        {
        }

        public CabbageCrop( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            Cabbage cabbage = new Cabbage();

            cabbage.ItemID = Utility.Random( 0xC7B, 2 );

            return cabbage;
        }

        public override int GetPickedID()
        {
            return Utility.Random( 0xC61, 3 );
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
