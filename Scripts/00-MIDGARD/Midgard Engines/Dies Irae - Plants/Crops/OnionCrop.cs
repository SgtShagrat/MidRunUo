using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class OnionSeed : BaseSeed
    {
        #region proprietà da BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( OnionCrop ) }; } }
        public override string PlantName { get { return "onion"; } }
        #endregion

        #region proprietà di ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public OnionSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public OnionSeed( int amount )
            : base( amount )
        {
        }

        public OnionSeed( Serial serial )
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

    public class OnionCrop : BaseCrop
    {
        #region proprietà da BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0xC68, 0xC69, 0xC6F }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "onion seedlings", "young onions", "onion plants" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( OnionSeed ); } }

        // Variables concerning produce action
        public override string CropName { get { return "onion"; } }
        public override string CropPluralName { get { return "onions"; } }
        public override int ProductPhaseID { get { return 0; } }
        #endregion

        #region costruttori
        [Constructable]
        public OnionCrop( Mobile owner )
            : base( owner )
        {
        }

        public OnionCrop( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            Onion onion = new Onion();

            onion.ItemID = Utility.Random( 0xC6D, 2 );

            return onion;
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
