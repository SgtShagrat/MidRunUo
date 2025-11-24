using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class CoconutPalmSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( CoconutPalm1 ) }; } }
        public override string PlantName { get { return "coconut palm"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public CoconutPalmSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public CoconutPalmSeed( int amount )
            : base( amount )
        {
        }

        public CoconutPalmSeed( Serial serial )
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

    public class CoconutPalm1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0C9C, 0x0C99, 0xD38, 0x0C95 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a coconut palm sapling", "a young coconut palm", "a coconut palm", "a great coconut palm" }; } }
        public override double MinSkillToHarvest { get { return 90; } }
        public override Type TypeOfParentSeed { get { return typeof( CoconutPalmSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return true; } }
        public override int ProduceTick { get { return 1; } }
        public override int Capacity { get { return 15; } }
        public override string CropName { get { return "coconut"; } }
        public override string CropPluralName { get { return "coconuts"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public CoconutPalm1( Mobile owner )
            : base( owner )
        {
        }

        public CoconutPalm1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Coconut();
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
