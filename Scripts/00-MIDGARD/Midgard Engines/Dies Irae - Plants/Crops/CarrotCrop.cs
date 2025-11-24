using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class CarrotSeed : BaseSeed
    {
        #region proprietà da BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( CarrotCrop ) }; } }
        public override string PlantName { get { return "carrot"; } }
        #endregion

        #region proprietà di ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public CarrotSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public CarrotSeed( int amount )
            : base( amount )
        {
        }

        public CarrotSeed( Serial serial )
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

    public class CarrotCrop : BaseCrop
    {
        #region proprietà da BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0xC68, 0xC69, 0xC76 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "carrot seedlings", "young carrots", "carrot plants" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( CarrotSeed ); } }

        // Variables concerning produce action
        public override string CropName { get { return "carrot"; } }
        public override string CropPluralName { get { return "carrots"; } }
        public override int ProductPhaseID { get { return 0; } }
        #endregion

        #region costruttori
        [Constructable]
        public CarrotCrop( Mobile owner )
            : base( owner )
        {
        }

        public CarrotCrop( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            Carrot carrot = new Carrot();

            carrot.ItemID = Utility.Random( 0xC77, 2 );

            return carrot;
        }

        public override int GetPickedID()
        {
            return 0xC69;
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
