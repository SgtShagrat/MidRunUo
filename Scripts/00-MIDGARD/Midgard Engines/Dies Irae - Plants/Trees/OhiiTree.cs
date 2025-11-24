using System;
using Server;

namespace Midgard.Engines.PlantSystem
{
    public class OhiiTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( OhiiTree1 ) }; } }
        public override string PlantName { get { return "o'hii tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public OhiiTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public OhiiTreeSeed( int amount )
            : base( amount )
        {
        }

        public OhiiTreeSeed( Serial serial )
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

    public class OhiiTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CEA, 0x0CE9, 0xC9E }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "an o'hii tree sapling", "a young o'hii tree", "an o'hii tree" }; } }
        public override double MinSkillToHarvest { get { return 30; } }
        public override Type TypeOfParentSeed { get { return typeof( OhiiTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return string.Empty; } }
        public override string CropPluralName { get { return string.Empty; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public OhiiTree1( Mobile owner )
            : base( owner )
        {
        }

        public OhiiTree1( Serial serial )
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
