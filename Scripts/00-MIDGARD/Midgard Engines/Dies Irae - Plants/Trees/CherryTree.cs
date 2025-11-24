using System;
using Server;

namespace Midgard.Engines.PlantSystem
{
    public class CherryTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( CherryTree1 ), typeof( CherryTree2 ) }; } }
        public override string PlantName { get { return "cherry tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public CherryTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public CherryTreeSeed( int amount )
            : base( amount )
        {
        }

        public CherryTreeSeed( Serial serial )
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

    public class CherryTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0x2476 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a cherry tree sapling", "a young cherry tree", "a cherry tree" }; } }
        public override double MinSkillToHarvest { get { return 80; } }
        public override Type TypeOfParentSeed { get { return typeof( CherryTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return "cherry"; } }
        public override string CropPluralName { get { return "cherries"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0x2471, 0x2472, 0x2473, 0x2474, 0x2475 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public CherryTree1( Mobile owner )
            : base( owner )
        {
        }

        public CherryTree1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            // TODO: implement cherryes
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

    public class CherryTree2 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0x2477 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a cherry tree sapling", "a young cherry tree", "a cherry tree" }; } }
        public override double MinSkillToHarvest { get { return 80; } }
        public override Type TypeOfParentSeed { get { return typeof( CherryTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return "cherry"; } }
        public override string CropPluralName { get { return "cherries"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0x2471, 0x2472, 0x2473, 0x2474, 0x2475 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public CherryTree2( Mobile owner )
            : base( owner )
        {
        }

        public CherryTree2( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            // TODO: implement cherryes
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
