/***************************************************************************
 *                                     BaseCrop.cs
 *                            		-------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Base class for crops pants.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public abstract class BaseCrop : BasePlant
    {
        #region proprietà da BasePlant

        // General variables about plants
        public override bool IsDestroyable
        {
            get { return true; }
        }

        public override bool NeedWater
        {
            get { return true; }
        }

        // Variables concerning plant evolution and death
        public override bool CanGrow
        {
            get { return true; }
        }

        public override int GrowthTick
        {
            get { return 5; }
        }

        public override bool LimitedLifeSpan
        {
            get { return true; }
        }

        public override TimeSpan LifeSpan
        {
            get { return TimeSpan.FromDays( 60.0 ); }
        }

        public override TimeSpan LongDrought
        {
            get { return TimeSpan.FromDays( 3.0 ); }
        }

        public override TimeSpan DormantDrought
        {
            get { return TimeSpan.FromDays( 2.0 ); }
        }

        // Variables concerning phases and ids
        public abstract override int[] PhaseIDs { get; }
        public abstract override string[] PhaseName { get; }

        // Variables concerning produce action
        public override bool CanProduce
        {
            get { return true; }
        }

        public override int ProduceTick
        {
            get { return 1; }
        }

        public override int Capacity
        {
            get { return 5; }
        }

        public abstract override string CropName { get; }
        public abstract override string CropPluralName { get; }
        public abstract override int ProductPhaseID { get; }

        // Variables concerning harvesting
        public override bool HasParentSeed
        {
            get { return true; }
        }

        public override Type HarvestingTool
        {
            get { return typeof( Fists ); }
        }

        public override double HarvestDelay
        {
            get { return 2.0; }
        }

        public override bool HarvestInPack
        {
            get { return true; }
        }

        public override double MinDiffSkillToCare
        {
            get { return 30; }
        }

        #endregion

        #region costruttori

        public BaseCrop( Mobile owner )
            : base( 1, owner )
        {
        }

        public BaseCrop( Serial serial )
            : base( serial )
        {
        }

        #endregion

        #region metodi

        /// <summary>
        /// Abstract: Crops must give an ID for picked status.
        /// </summary>
        public abstract int GetPickedID();

        /// <summary>
        /// Override: Crops shift their ID according to GetPickedID if after being harvested they have no more products.
        /// </summary>
        public override void GotHarvested( Mobile harvester, bool harvestToPack )
        {
            base.GotHarvested( harvester, harvestToPack );

            if( Yield < 1 )
                ItemID = GetPickedID();
        }

        #endregion

        #region serial-deserial

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }

        #endregion
    }
}