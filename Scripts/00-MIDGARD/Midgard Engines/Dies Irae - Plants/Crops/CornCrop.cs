using System;
using Midgard.Items;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class CornSeed : BaseSeed
    {
        #region proprietà da BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( CornCrop ) }; } }
        public override string PlantName { get { return "corn"; } }
        #endregion

        #region proprietà di ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public CornSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public CornSeed( int amount )
            : base( amount )
        {
        }

        public CornSeed( Serial serial )
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

    public class CornCrop : BaseCrop
    {
        #region proprietà da BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0xCB5, 0xC7E, 0xC7D }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "corn seedlings", "young corns", "corn plants" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( CornSeed ); } }

        // Variables concerning produce action
        public override string CropName { get { return "ear of corn"; } }
        public override string CropPluralName { get { return "ears of corn"; } }
        public override int ProductPhaseID { get { return 0; } }
        #endregion

        #region costruttori
        [Constructable]
        public CornCrop( Mobile owner )
            : base( owner )
        {
        }

        public CornCrop( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Corn();
        }

        public override int GetPickedID()
        {
            return 0xC7D;
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
