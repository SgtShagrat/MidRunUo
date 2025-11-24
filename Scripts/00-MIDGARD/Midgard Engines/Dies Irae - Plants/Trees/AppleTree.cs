using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class AppleTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( AppleTree1 ), typeof( AppleTree2 ) }; } }
        public override string PlantName { get { return "apple tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        public override bool CanGrowGarden { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public AppleTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public AppleTreeSeed( int amount )
            : base( amount )
        {
        }

        public AppleTreeSeed( Serial serial )
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

    public class AppleTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD94 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "an apple tree sapling", "a young apple tree", "an apple tree" }; } }

        // Variables concerning produce action
        public override bool CanProduce { get { return true; } }
        public override int ProduceTick { get { return 1; } }
        public override int Capacity { get { return 12; } }
        public override string CropName { get { return "apple"; } }
        public override string CropPluralName { get { return "apples"; } }
        public override double MinSkillToHarvest { get { return 70; } }
        public override Type TypeOfParentSeed { get { return typeof( AppleTreeSeed ); } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xD95 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { 0xD96 }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xD97 }; } }
        #endregion

        #region costruttori
        [Constructable]
        public AppleTree1( Mobile owner )
            : base( owner )
        {
        }

        public AppleTree1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Apple();
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

    public class AppleTree2 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xD98 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "an apple tree sapling", "a young apple tree", "an apple tree" }; } }

        // Variables concerning produce action
        public override bool CanProduce { get { return true; } }
        public override int ProduceTick { get { return 1; } }
        public override int Capacity { get { return 12; } }
        public override string CropName { get { return "apple"; } }
        public override string CropPluralName { get { return "apples"; } }
        public override double MinSkillToHarvest { get { return 70; } }
        public override Type TypeOfParentSeed { get { return typeof( AppleTreeSeed ); } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0xD99 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { 0xD9A }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { 0xD9B }; } }
        #endregion

        #region costruttori
        [Constructable]
        public AppleTree2( Mobile owner )
            : base( owner )
        {
        }

        public AppleTree2( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Apple();
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
