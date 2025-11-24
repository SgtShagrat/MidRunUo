using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class BananaTreeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( BananaTree1 ) }; } }
        public override string PlantName { get { return "banana tree"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public BananaTreeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public BananaTreeSeed( int amount )
            : base( amount )
        {
        }

        public BananaTreeSeed( Serial serial )
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

    public class BananaTree1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0CAB, 0x0CA8, 0x0CAA }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a banana tree sapling", "a young banana tree", "a banana tree" }; } }
        public override double MinSkillToHarvest { get { return 90; } }
        public override Type TypeOfParentSeed { get { return typeof( BananaTreeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return true; } }
        public override int ProduceTick { get { return 1; } }
        public override int Capacity { get { return 30; } }
        public override string CropName { get { return "banana"; } }
        public override string CropPluralName { get { return "bananas"; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public BananaTree1( Mobile owner )
            : base( owner )
        {
        }

        public BananaTree1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Banana();
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
