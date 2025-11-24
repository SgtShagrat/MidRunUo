using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class FlaxSeed : BaseSeed
    {
        #region proprietà da BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( FlaxCrop ) }; } }
        public override string PlantName { get { return "flax"; } }
        #endregion

        #region proprietà di ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public FlaxSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public FlaxSeed( int amount )
            : base( amount )
        {
        }

        public FlaxSeed( Serial serial )
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

    public class FlaxCrop : BaseCrop
    {
        #region proprietà da BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x1A99, 0x1A9A, 0x1A9B }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "flax seedlings", "young flaxes", "flax plants" }; } }
        public override double MinSkillToHarvest { get { return 50; } }
        public override Type TypeOfParentSeed { get { return typeof( FlaxSeed ); } }

        // Variables concerning produce action
        public override string CropName { get { return "flax"; } }
        public override string CropPluralName { get { return "flaxes"; } }
        public override int ProductPhaseID { get { return 0; } }
        #endregion

        #region costruttori
        [Constructable]
        public FlaxCrop( Mobile owner )
            : base( owner )
        {
        }

        public FlaxCrop( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            Flax flax = new Flax();

            flax.ItemID = Utility.Random( 0x1A9C, 2 );

            return flax;
        }

        public override int GetPickedID()
        {
            return 0x1A9A;
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
