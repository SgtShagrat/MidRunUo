using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class PeachTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( PeachTree1 ), typeof( PeachTree2 ) }; } }
        public override string PlantName { get { return "peach tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public PeachTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public PeachTreeSeed( int amount )
            : base( amount )
        {
        }

        public PeachTreeSeed( Serial serial )
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

    public class PeachTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD9C }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a peach tree sapling", "a young peach tree", "a peach tree" }; } }
        public override double MinSkillToHarvest { get { return 70; } }
        public override Type TypeOfParentSeed { get { return typeof( PeachTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return true; } }
        public override int ProduceTick { get { return 1; } }
        public override int Capacity { get { return 12; } }
        public override string CropName { get { return "peach"; } }
        public override string CropPluralName { get { return "peach"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xD9D }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { 0xD9E }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xD9F }; } }
        #endregion

        #region costruttori
        [Constructable]
        public PeachTree1( Mobile owner )
            : base( owner )
        {
        }

        public PeachTree1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Peach();
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

    public class PeachTree2 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xDA0 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a peach tree sapling", "a young peach tree", "a peach tree" }; } }
        public override double MinSkillToHarvest { get { return 70; } }
        public override Type TypeOfParentSeed { get { return typeof( PeachTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return true; } }
        public override int ProduceTick { get { return 1; } }
        public override int Capacity { get { return 12; } }
        public override string CropName { get { return "peach"; } }
        public override string CropPluralName { get { return "peach"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xDA1 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { 0xDA2 }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xDA3 }; } }
        #endregion

        #region costruttori
        [Constructable]
        public PeachTree2( Mobile owner )
            : base( owner )
        {
        }

        public PeachTree2( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Peach();
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
