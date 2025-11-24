using System;
using Server;

namespace Midgard.Engines.PlantSystem
{
    public class MapleTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( MapleTree1 ), typeof( MapleTree2 ) }; } }
        public override string PlantName { get { return "maple tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public MapleTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public MapleTreeSeed( int amount )
            : base( amount )
        {
        }

        public MapleTreeSeed( Serial serial )
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

    public class MapleTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0x247D }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a maple tree sapling", "a young maple tree", "a maple tree" }; } }
        public override double MinSkillToHarvest { get { return 70; } }
        public override Type TypeOfParentSeed { get { return typeof( MapleTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return "maple"; } }
        public override string CropPluralName { get { return "maples"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0x2478, 0x2479, 0x247A, 0x247B, 0x247C }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        public override int FixLeavesAltitude { get { return 2; } }
        #endregion

        #region costruttori
        [Constructable]
        public MapleTree1( Mobile owner )
            : base( owner )
        {
        }

        public MapleTree1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            // TODO: implement maples
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

    public class MapleTree2 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0x247E }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a maple tree sapling", "a young maple tree", "a maple tree" }; } }
        public override double MinSkillToHarvest { get { return 70; } }
        public override Type TypeOfParentSeed { get { return typeof( MapleTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return "maple"; } }
        public override string CropPluralName { get { return "maples"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0x2478, 0x2479, 0x247A, 0x247B, 0x247C }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        public override int FixLeavesAltitude { get { return 2; } }
        #endregion

        #region costruttori
        [Constructable]
        public MapleTree2( Mobile owner )
            : base( owner )
        {
        }

        public MapleTree2( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            // TODO: implement maples
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
