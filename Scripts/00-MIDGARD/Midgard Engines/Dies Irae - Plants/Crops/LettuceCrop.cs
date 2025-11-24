using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class LettuceSeed : BaseSeed
    {
        #region proprietà da BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( LettuceCrop ) }; } }
        public override string PlantName { get { return "lettuce"; } }
        #endregion

        #region proprietà di ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public LettuceSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public LettuceSeed( int amount )
            : base( amount )
        {
        }

        public LettuceSeed( Serial serial )
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

    public class LettuceCrop : BaseCrop
    {
        #region proprietà da BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0xCB5, 0xC61, 0xC70 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "lettuce seedlings", "young lettuces", "lettuce plants" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( LettuceSeed ); } }

        // Variables concerning produce action
        public override string CropName { get { return "lettuce"; } }
        public override string CropPluralName { get { return "lettuce"; } }
        public override int ProductPhaseID { get { return 0; } }
        #endregion

        #region costruttori
        [Constructable]
        public LettuceCrop( Mobile owner )
            : base( owner )
        {
        }

        public LettuceCrop( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            Lettuce lettuce = new Lettuce();

            lettuce.ItemID = Utility.Random( 0xC70, 2 );

            return lettuce;
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
