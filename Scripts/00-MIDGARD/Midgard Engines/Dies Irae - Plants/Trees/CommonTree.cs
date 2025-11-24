using System;
using Server;

namespace Midgard.Engines.PlantSystem
{
    public class CommonTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( CommonTree1 ), typeof( CommonTree2 ), typeof( CommonTree3 ) }; } }
        public override string PlantName { get { return "common tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public CommonTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public CommonTreeSeed( int amount )
            : base( amount )
        {
        }

        public CommonTreeSeed( Serial serial )
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

    public class CommonTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xCCD }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a tree sapling", "a young tree", "a tree" }; } }
        public override double MinSkillToHarvest { get { return 10; } }
        public override Type TypeOfParentSeed { get { return typeof( CommonTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return string.Empty; } }
        public override string CropPluralName { get { return string.Empty; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xCCE }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xCCF }; } }
        #endregion

        #region costruttori
        [Constructable]
        public CommonTree1( Mobile owner )
            : base( owner )
        {
        }

        public CommonTree1( Serial serial )
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

    public class CommonTree2 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xCD0 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a tree sapling", "a young tree", "a tree" }; } }
        public override double MinSkillToHarvest { get { return 10; } }
        public override Type TypeOfParentSeed { get { return typeof( CommonTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return string.Empty; } }
        public override string CropPluralName { get { return string.Empty; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xCD1 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xCD2 }; } }
        #endregion

        #region costruttori
        [Constructable]
        public CommonTree2( Mobile owner )
            : base( owner )
        {
        }

        public CommonTree2( Serial serial )
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

    public class CommonTree3 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xCD3 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a tree sapling", "a young tree", "a tree" }; } }
        public override double MinSkillToHarvest { get { return 10; } }
        public override Type TypeOfParentSeed { get { return typeof( CommonTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return string.Empty; } }
        public override string CropPluralName { get { return string.Empty; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xCD4 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xCD5 }; } }
        #endregion

        #region costruttori
        [Constructable]
        public CommonTree3( Mobile owner )
            : base( owner )
        {
        }

        public CommonTree3( Serial serial )
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
