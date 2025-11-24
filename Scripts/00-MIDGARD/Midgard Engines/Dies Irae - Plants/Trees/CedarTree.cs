using System;
using Server;

namespace Midgard.Engines.PlantSystem
{
    public class CedarTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( CedarTree1 ), typeof( CedarTree2 ) }; } }
        public override string PlantName { get { return "cedar tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public CedarTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public CedarTreeSeed( int amount )
            : base( amount )
        {
        }

        public CedarTreeSeed( Serial serial )
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

    public class CedarTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xCD6 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "an cedar tree sapling", "a young cedar tree", "a cedar tree" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( CedarTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return string.Empty; } }
        public override string CropPluralName { get { return string.Empty; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xCD7 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public CedarTree1( Mobile owner )
            : base( owner )
        {
        }

        public CedarTree1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return null;
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

    public class CedarTree2 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xCD8 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "an cedar tree sapling", "a young cedar tree", "a cedar tree" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( CedarTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return string.Empty; } }
        public override string CropPluralName { get { return string.Empty; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xCD9 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public CedarTree2( Mobile owner )
            : base( owner )
        {
        }

        public CedarTree2( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return null;
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
