using System;
using Server;

namespace Midgard.Engines.PlantSystem
{
    public class OakTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( OakTree1 ), typeof( OakTree2 ) }; } }
        public override string PlantName { get { return "oak tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public OakTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public OakTreeSeed( int amount )
            : base( amount )
        {
        }

        public OakTreeSeed( Serial serial )
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

    public class OakTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xCDA }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "an oak tree sapling", "a young oak tree", "an oak tree" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( OakTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return string.Empty; } }
        public override string CropPluralName { get { return string.Empty; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xCDB }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xCDC }; } }
        #endregion

        #region costruttori
        [Constructable]
        public OakTree1( Mobile owner )
            : base( owner )
        {
        }

        public OakTree1( Serial serial )
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

    public class OakTree2 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xCDD }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "an oak tree sapling", "a young oak tree", "an oak tree" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( OakTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return string.Empty; } }
        public override string CropPluralName { get { return string.Empty; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xCDE }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xCDF }; } }
        #endregion

        #region costruttori
        [Constructable]
        public OakTree2( Mobile owner )
            : base( owner )
        {
        }

        public OakTree2( Serial serial )
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
