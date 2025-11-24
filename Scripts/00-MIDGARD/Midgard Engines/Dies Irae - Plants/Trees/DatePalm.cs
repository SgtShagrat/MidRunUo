using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class DatePalmSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( DatePalm1 ) }; } }
        public override string PlantName { get { return "date palm"; } }
        public override int RootRadius { get { return 2; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public DatePalmSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public DatePalmSeed( int amount )
            : base( amount )
        {
        }

        public DatePalmSeed( Serial serial )
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

    public class DatePalm1 : BaseTree
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0x0C9B, 0x0C9D, 0xD37, 0x0C96 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a date palm sapling", "a young date palm", "a date palm", "a great date palm" }; } }
        public override double MinSkillToHarvest { get { return 90; } }
        public override Type TypeOfParentSeed { get { return typeof( DatePalmSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return true; } }
        public override int ProduceTick { get { return 1; } }
        public override int Capacity { get { return 15; } }
        public override string CropName { get { return "date"; } }
        public override string CropPluralName { get { return "dates"; } }
        #endregion

        #region properties from Base
        public override int[] LeavesIDs { get { return new int[] { }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public DatePalm1( Mobile owner )
            : base( owner )
        {
        }

        public DatePalm1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return new Dates();
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
