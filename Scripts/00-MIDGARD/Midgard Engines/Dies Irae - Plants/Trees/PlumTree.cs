using System;
using Server;

namespace Midgard.Engines.PlantSystem
{
    public class PlumTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( PlumTree1 ), typeof( PlumTree2 ) }; } }
        public override string PlantName { get { return "plum tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public PlumTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public PlumTreeSeed( int amount )
            : base( amount )
        {
        }

        public PlumTreeSeed( Serial serial )
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

    public class PlumTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0x26ED }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a plum tree sapling", "a young plum tree", "a plum tree" }; } }
        public override double MinSkillToHarvest { get { return 80; } }
        public override Type TypeOfParentSeed { get { return typeof( PlumTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return "plum"; } }
        public override string CropPluralName { get { return "plums"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0x26EF, 0x26F0, 0x26F1, 0x26F2, 0x26F3 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public PlumTree1( Mobile owner )
            : base( owner )
        {
        }

        public PlumTree1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            // TODO: implement plums
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

    public class PlumTree2 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0x26EE }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "an plum tree sapling", "a young plum tree", "a plum tree" }; } }
        public override double MinSkillToHarvest { get { return 80; } }
        public override Type TypeOfParentSeed { get { return typeof( PlumTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return "plum"; } }
        public override string CropPluralName { get { return "plums"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { 0x26EF, 0x26F0, 0x26F1, 0x26F2, 0x26F3 }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public PlumTree2( Mobile owner )
            : base( owner )
        {
        }

        public PlumTree2( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            // TODO: implement plums
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
