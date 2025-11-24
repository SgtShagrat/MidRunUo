using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class WheatSeed : BaseSeed
    {
        #region proprietà da BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( WheatCrop ) }; } }
        public override string PlantName { get { return "wheat"; } }
        #endregion

        #region proprietà di ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public WheatSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public WheatSeed( int amount )
            : base( amount )
        {
        }

        public WheatSeed( Serial serial )
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

    public class WheatCrop : BaseCrop
    {
        #region proprietà da BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x1EBE, 0xC55, 0xC58, 0xC5B }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "wheat seedlings", "young wheats", "wheat plants", "adult wheat plants" }; } }
        public override double MinSkillToHarvest { get { return 50; } }
        public override Type TypeOfParentSeed { get { return typeof( WheatSeed ); } }

        // Variables concerning produce action
        public override string CropName { get { return "wheat"; } }
        public override string CropPluralName { get { return "wheats"; } }
        public override int ProductPhaseID { get { return 0; } }
        #endregion

        #region costruttori
        [Constructable]
        public WheatCrop( Mobile owner )
            : base( owner )
        {
        }

        public WheatCrop( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new WheatSheaf();
        }

        public override int GetPickedID()
        {
            return 0xC58;
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
