using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class PearTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( PearTree1 ), typeof( PearTree2 ) }; } }
        public override string PlantName { get { return "pear tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public PearTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public PearTreeSeed( int amount )
            : base( amount )
        {
        }

        public PearTreeSeed( Serial serial )
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

    public class PearTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xDA4 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a pear tree sapling", "a pear tree", "a pear tree" }; } }
        public override double MinSkillToHarvest { get { return 70; } }
        public override Type TypeOfParentSeed { get { return typeof( PearTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return true; } }
        public override int ProduceTick { get { return 1; } }
        public override int Capacity { get { return 12; } }
        public override string CropName { get { return "pear"; } }
        public override string CropPluralName { get { return "pear"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xDA5 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { 0xDA6 }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xDA7 }; } }
        #endregion

        #region costruttori
        [Constructable]
        public PearTree1( Mobile owner )
            : base( owner )
        {
        }

        public PearTree1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Pear();
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

    public class PearTree2 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xDA8 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a pear tree sapling", "a pear tree", "a pear tree" }; } }
        public override double MinSkillToHarvest { get { return 70; } }
        public override Type TypeOfParentSeed { get { return typeof( PearTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return true; } }
        public override int ProduceTick { get { return 1; } }
        public override int Capacity { get { return 12; } }
        public override string CropName { get { return "pear"; } }
        public override string CropPluralName { get { return "pear"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xDA9 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { 0xDAA }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xDAB }; } }
        #endregion

        #region costruttori
        [Constructable]
        public PearTree2( Mobile owner )
            : base( owner )
        {
        }

        public PearTree2( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Pear();
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
