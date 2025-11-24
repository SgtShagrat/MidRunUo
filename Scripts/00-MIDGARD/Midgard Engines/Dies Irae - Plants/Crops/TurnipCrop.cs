using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class TurnipSeed : BaseSeed
    {
        #region proprietà da BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( TurnipCrop ) }; } }
        public override string PlantName { get { return "turnip"; } }
        #endregion

        #region proprietà di ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public TurnipSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public TurnipSeed( int amount )
            : base( amount )
        {
        }

        public TurnipSeed( Serial serial )
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

    public class TurnipCrop : BaseCrop
    {
        #region proprietà da BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0xCB5, 0xC61, 0xC76 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "turnip seedlings", "young turnips", "turnip plants" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( TurnipSeed ); } }

        // Variables concerning produce action
        public override string CropName { get { return "turnip"; } }
        public override string CropPluralName { get { return "turnips"; } }
        public override int ProductPhaseID { get { return 0; } }
        #endregion

        #region costruttori
        [Constructable]
        public TurnipCrop( Mobile owner )
            : base( owner )
        {
        }

        public TurnipCrop( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            Turnip turnip = new Turnip();

            turnip.ItemID = Utility.Random( 0xD39, 2 );

            return turnip;
        }

        public override int GetPickedID()
        {
            return 0xC61;
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
