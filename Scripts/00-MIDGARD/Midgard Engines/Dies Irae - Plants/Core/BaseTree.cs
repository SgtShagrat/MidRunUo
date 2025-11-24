/***************************************************************************
 *                                     BaseTree.cs
 *                            		-------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Base class for trees.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Items;
using Map = Server.Map;

namespace Midgard.Engines.PlantSystem
{
    public abstract class BaseTree : BasePlant
    {
        private TreeLeaves m_Leaves;

        #region proprietà da BasePlant
        // General variables about plants
        public override bool IsDestroyable { get { return true; } }
        public override bool NeedWater { get { return true; } }

        // Variables concerning plant evolution and death
        public override bool CanGrow { get { return true; } }
        public override int GrowthTick { get { return 10; } }
        public override bool LimitedLifeSpan { get { return true; } }
        public override TimeSpan LifeSpan { get { return TimeSpan.FromDays( 300 ); } }
        public override TimeSpan LongDrought { get { return TimeSpan.FromDays( 14 ); } }
        public override TimeSpan DormantDrought { get { return TimeSpan.FromDays( 2 ); } }

        // Variables concerning phases and ids
        public abstract override int[] PhaseIDs { get; }
        public abstract override string[] PhaseName { get; }

        // Variables concerning produce action
        public abstract override bool CanProduce { get; }
        public abstract override int ProduceTick { get; }
        public abstract override int Capacity { get; }
        public abstract override string CropName { get; }
        public abstract override string CropPluralName { get; }
        public override int ProductPhaseID { get { return 0; } }				// Trees doesn't shift main ID on product phase. See ProductLeavesID       

        // Variables concerning harvesting
        public override bool HasParentSeed { get { return true; } }
        public override Type HarvestingTool { get { return typeof( Fists ); } }
        public override double HarvestDelay { get { return 5.0; } }
        public override bool HarvestInPack { get { return true; } }

        public override double MinDiffSkillToCare
        {
            get { return 50; }
        }
        #endregion

        #region proprietà virtuali
        public abstract int[] LeavesIDs { get; }							// Ids of leavs. NB: Tree has leaves only at full grown level
        public abstract int[] ProductLeavesIDs { get; }						// Ids of leaves when there are fruicts on the tree; 
        public abstract int[] DriedLeavesIDs { get; }						// Ids of leaves when care level is bad.
        public virtual int FixLeavesAltitude { get { return 0; } }			// Some particular trees has Leaves higher then trunk
        #endregion

        #region altre proprietà
        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasLeaves
        {
            get { return m_Leaves != null; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool ShouldHaveLeaves
        {
            get { return IsFullGrown && LeavesIDs.Length > 0; }				// Tree has leaves only at full grown level
        }
        #endregion

        public BaseTree( Mobile owner )
            : base( 1, owner )
        {
        }

        #region metodi
        /// <summary>
        /// Event occurring when a tree is created.
        /// </summary>
        public override void InitPlant()
        {
            base.InitPlant();

            if( ShouldHaveLeaves && !HasLeaves )
                MakeLeaves();
        }

        #region Life action from e to our plant
        /// <summary>
        /// Override: Tree change leaves not trunk on grow if phase has leaves
        /// </summary>		
        public override void Grow()
        {
            base.Grow();

            CheckLeavesID();
        }

        /// <summary>
        /// Overridable: Event invoked when a tree have to grow leaves. (Only at full grown level!)
        /// </summary>	
        public virtual void MakeLeaves()
        {
            if( Deleted || Map == Map.Internal )
                return;

            if( !IsFullGrown || !CanGrow )
                return;

            int leavesID = Utility.RandomList( LeavesIDs );
            if( CareLevel <= PlantHelper.CareLevelBad )
            {
                if( DriedLeavesIDs.Length > 0 )
                    leavesID = Utility.RandomList( DriedLeavesIDs );
            }
            else if( Yield > 0 )
            {
                if( ProductLeavesIDs.Length > 0 )
                    leavesID = Utility.RandomList( ProductLeavesIDs );
            }

            m_Leaves = new TreeLeaves( this, leavesID );

            if( m_Leaves == null )
            {
                try
                {
                    Console.WriteLine( "Warning: leaves null on a tree after their creation." );
                    Console.WriteLine( "\tTree type - {0}.", GetType().Name );
                    Console.WriteLine( "\tCurrentPhase - {0}", CurrentPhase );
                    if( LeavesIDs.Length > 0 )
                        Console.WriteLine( "\t LeavesIDs[ CurrentPhase ] - {0}", LeavesIDs[ CurrentPhase ] );
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
        }
        #endregion

        #region OnChanged
        /// <summary>
        /// Event invoked when a plant care level changes.
        /// </summary>
        public override void OnCareLevelChanged( int oldValue )
        {
            base.OnCareLevelChanged( oldValue );

            CheckLeavesID();
            CheckPlantLeavesHue();
            CheckLeavesAltitude();
        }
        #endregion

        #region checks
        /// <summary>
        /// Override: Tree change leaves not trunk on produce
        /// </summary>
        public override void CheckPlantID()
        {
            if( Deleted )
                return;

            int currentID = PhaseIDs[ CurrentPhase ];
            if( ItemID != currentID )
                ItemID = currentID;

            CheckLeavesID();
            CheckPlantLeavesHue();
            CheckLeavesAltitude();
        }

        /// <summary>
        /// Overridable: Tree could have to shift leaves id on product
        /// </summary>
        public virtual void CheckLeavesID()
        {
            if( Deleted )
                return;

            if( !ShouldHaveLeaves && HasLeaves )								// remove leaves if tree should no have them
            {
                m_Leaves.Delete();
                DebugMessage( IsInDebugMode, "My leaves say hallo dude!", this );
            }
            else if( ShouldHaveLeaves && !HasLeaves )							// add leaves if tree should have and have not
            {
                MakeLeaves();
                DebugMessage( IsInDebugMode, "Hey! I should have a pair of leaves, don't I?", this );
            }

            if( HasLeaves )
            {
                if( Yield > 0 )
                {
                    if( HasWrongLeaves( ProductLeavesIDs ) )					// In any case if tree has fruits ID should be a random id in ProductLeavesIDs
                    {
                        m_Leaves.ItemID = Utility.RandomList( ProductLeavesIDs ); // if tree has wrong leaves put on a random leaves from ProductLeavesIDs
                        DebugMessage( IsInDebugMode, string.Format( "My leaves changed to dried due to a level of care of {0}.", CareLevel ), this );
                    }
                }
                else if( CareLevel <= PlantHelper.CareLevelBad )				// change leaves hue if carelevel lower than CareLevelBad					
                {
                    if( HasWrongLeaves( DriedLeavesIDs ) )						// check only for tree with defined DriedLeavesID
                    {
                        m_Leaves.ItemID = Utility.RandomList( DriedLeavesIDs );	// if tree has wrong leaves put on a random leaves from DriedLeavesIDs
                        DebugMessage( IsInDebugMode, string.Format( "My leaves >>ID<< changed to dried due to a level of care of {0}.", CareLevel ), this );
                    }
                }
                else if( HasWrongLeaves( LeavesIDs ) )
                {
                    m_Leaves.ItemID = Utility.RandomList( LeavesIDs );
                }
            }
        }

        /// <summary>
        /// Overridable: Tree could have to change leaves hue in ther life
        /// </summary>
        public virtual void CheckPlantLeavesHue()
        {
            if( m_Leaves == null || m_Leaves.Deleted )
                return;

            int rightHue = 0;													// normally trees has leaves with hue 0

            if( CareLevel <= PlantHelper.CareLevelBad )							// but if their carelevel is Bad
            {
                if( DriedLeavesIDs.Length == 0 )								// and they don't implement any DriedLeavesIDs
                {																// if their color is not DriedLeavesHue
                    rightHue = PlantHelper.DriedLeavesHue;						// set it to DriedLeavesHue
                }
            }

            if( m_Leaves.Hue != rightHue )										// if color is not the rightone
                m_Leaves.Hue = rightHue;										// set it to.
        }

        /// <summary>
        /// Overridable: Check if tree has leaves in right Z position
        /// </summary>
        public virtual void CheckLeavesAltitude()
        {
            if( m_Leaves == null || m_Leaves.Deleted )
                return;

            if( m_Leaves.Z != Z + FixLeavesAltitude )
                m_Leaves.Location = new Point3D( X, Y, Z + FixLeavesAltitude );
        }
        #endregion

        #region general methods
        /// <summary>
        /// Override: Tree OnLocationChange has to move its leaves location if there are ones.
        /// </summary>
        /// <param name="oldLocation">not used</param>      
        public override void OnLocationChange( Point3D oldLocation )
        {
            if( m_Leaves != null && !m_Leaves.Deleted )
            {
                Console.WriteLine( "OK" );
                //				m_Leaves.Delete();
                //				MakeLeaves();
                if( m_Leaves != null )
                    m_Leaves.Location = new Point3D( X, Y, Z + FixLeavesAltitude );
            }
        }

        /// <summary>
        /// Override: Tree OnMapChange has to move its leaves map if there are ones.
        /// </summary>  
        public override void OnMapChange()
        {
            if( m_Leaves != null && !m_Leaves.Deleted )
                m_Leaves.Map = Map;
        }

        /// <summary>
        /// Override: Tree OnAfterDelete has to remove its leaves if there are ones.
        /// </summary>  
        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if( m_Leaves != null && !m_Leaves.Deleted )
                m_Leaves.Delete();
        }

        /// <summary>
        /// Overridable: Check if our tre has leavesID in leavesGroup
        /// </summary>
        /// <param name="leavesGroup">array of integers with ids to check</param> 
        public virtual bool HasWrongLeaves( int[] leavesGroup )
        {
            if( leavesGroup.Length == 0 )											// if groups has no values is always right
                return false;

            if( Array.IndexOf( leavesGroup, m_Leaves.ItemID ) == -1 )
                return true;
            else
                return false;
        }
        #endregion
        #endregion

        #region serial-deserial
        public BaseTree( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );

            bool leaves = ( m_Leaves != null );
            writer.Write( leaves );
            if( leaves )
                writer.Write( m_Leaves );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            bool leaves = reader.ReadBool();
            if( leaves )
                m_Leaves = reader.ReadItem() as TreeLeaves;
        }
        #endregion

        #region Leaves
        private class TreeLeaves : Item
        {
            #region campi
            private BaseTree m_Tree;
            #endregion

            #region costruttori
            public TreeLeaves( BaseTree tree, int leavesID )
                : base( leavesID )
            {
                Movable = false;
                m_Tree = tree;
                if( m_Tree != null )
                {
                    if( m_Tree.Map != Map.Internal )
                        MoveToWorld( new Point3D( m_Tree.X, m_Tree.Y, m_Tree.Z + m_Tree.FixLeavesAltitude ), m_Tree.Map );
                    else
                        Console.WriteLine( "Warning: BaseTree on Map Internal on leaves creation!" );
                }
                else
                {
                    Console.WriteLine( "Warning: BaseTree not found on leaves creation!" );
                }
            }


            #endregion

            #region metodi
            /// <summary>
            /// Override: Leaves OnLocationChange have to move their trunk location.
            /// </summary>
            /// <param name="oldLocation">not used</param>  
            public override void OnLocationChange( Point3D oldLocation )
            {
                if( m_Tree != null )
                    m_Tree.Location = new Point3D( X, Y, Z - m_Tree.FixLeavesAltitude );
            }

            /// <summary>
            /// Override: Leaves OnMapChange have to move their trunk map.
            /// </summary>  
            public override void OnMapChange()
            {
                if( m_Tree != null )
                    m_Tree.Map = Map;
            }

            /// <summary>
            /// Override: Tree OnAfterDelete have to remove their trunk.
            /// </summary>
            /*
            public override void OnAfterDelete()
            {
                base.OnAfterDelete();
	
                if( m_Tree != null && !m_Tree.Deleted )
                {
                    if( m_Tree.IsFullGrown )
                        m_Tree.Delete();
                }
            }
            */
            #endregion

            #region serial-deserial
            public TreeLeaves( Serial serial )
                : base( serial )
            {
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( (int)0 ); // version

                writer.Write( m_Tree );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();

                m_Tree = reader.ReadItem() as BaseTree;
            }
            #endregion
        }
        #endregion
    }
}
