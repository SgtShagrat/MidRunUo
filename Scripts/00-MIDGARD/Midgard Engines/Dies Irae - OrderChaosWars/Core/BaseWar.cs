/***************************************************************************
 *                               BaseWar.cs
 *                            -------------------
 *   begin                : 01 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;

namespace Midgard.Engines.OrderChaosWars
{
    public abstract class BaseWar
    {
        // public static List<BaseWar> Wars { get { return Reflector.Wars; } }

        public static readonly BaseWar NujelmWar = new Nujelm();

        public static readonly BaseWar[] Wars = new BaseWar[]
                                                {
                                                    NujelmWar
                                                };

        private int m_ObjDefinitionStage;

        public WarDefinition Definition { get; set; }

        public List<BaseObjective> Objectives { get; private set; }

        public WarState OrderWarState { get; private set; }

        public WarState ChaosWarState { get; private set; }

        public List<BaseObjective> OrderObjectives { get; private set; }

        public List<BaseObjective> ChaosObjectives { get; private set; }

        protected BaseWar()
        {
            OrderWarState = new WarState( this, Virtue.Order );
            ChaosWarState = new WarState( this, Virtue.Chaos );

            Objectives = new List<BaseObjective>();
            m_ObjDefinitionStage = -1;
        }

        /// <summary>
        /// Register an objective to our war plan
        /// </summary>
        public void AddObjective( BaseObjective objective )
        {
            if( objective == null )
                return;

            if( Objectives == null )
                Objectives = new List<BaseObjective>();

            Logger.Log( "AddObjective: {0}", objective.Name );

            objective.War = this;
            objective.RightStage = m_ObjDefinitionStage;

            Objectives.Add( objective );
        }

        /// <summary>
        /// Add a stage to our war plan.
        /// Before adding any objective, a stage must be defined
        /// </summary>
        public void AddStage( int stage )
        {
            Logger.Log( "AddStage: {0}", stage );

            if( stage > 0 && stage > m_ObjDefinitionStage )
                m_ObjDefinitionStage = stage;
            else
                Logger.Log( "Warning: invalid stage definition in OrderChaos wars system." );
        }

        public virtual void InitStage( int stage, Virtue virtue )
        {
            Logger.Log( "InitStage: {0} Virtue: {1}", stage, virtue );

            switch( virtue )
            {
                case Virtue.Order:
                    OrderWarState.Stage = stage;
                    break;
                case Virtue.Chaos:
                    ChaosWarState.Stage = stage;
                    break;
                default:
                    break;
            }
        }

        public virtual bool IsLastStage( Virtue virtue )
        {
            Logger.Log( "IsLastStage: {0}", virtue );

            if( OrderWarState != null && OrderWarState.Stage >= m_ObjDefinitionStage )
                return true;

            if( ChaosWarState != null && ChaosWarState.Stage >= m_ObjDefinitionStage )
                return true;

            return false;
        }

        public virtual List<BaseObjective> GetCurrentObjectives( Virtue virtue )
        {
            Logger.Log( "GetCurrentObjectives: {0}", virtue );

            if( virtue == Virtue.None )
                return null;

            return virtue == Virtue.Order ? OrderObjectives : ChaosObjectives;
        }

        public virtual int GetStageForVirtue( Virtue virtue )
        {
            Logger.Log( "GetStageForVirtue: {0}", virtue );

            switch( virtue )
            {
                case Virtue.Order:
                    return OrderWarState.Stage;
                case Virtue.Chaos:
                    return ChaosWarState.Stage;
                default:
                    return 0;
            }
        }

        public virtual void OnAfterStageChanged( int newStage, Virtue virtue )
        {
            Logger.Log( "OnAfterStageChanged: {0} - {1}", virtue, newStage );

            InitStage( newStage, virtue );

            if( virtue != Virtue.Chaos )
            {
                if( OrderObjectives == null )
                    OrderObjectives = new List<BaseObjective>();
                else
                    OrderObjectives.Clear();
            }

            if( virtue != Virtue.Order )
            {
                if( ChaosObjectives == null )
                    ChaosObjectives = new List<BaseObjective>();
                else
                    ChaosObjectives.Clear();
            }

            if( Objectives == null )
            {
                Logger.Log( "Warning: m_Objectives is null in OnAfterStageChanged" );
                return;
            }

            foreach( BaseObjective objective in Objectives )
            {
                if( IsInRightStage( objective, virtue ) )
                {
                    if( virtue != Virtue.Chaos )
                        OrderObjectives.Add( objective );

                    if( virtue != Virtue.Order )
                        ChaosObjectives.Add( objective );
                }
            }

            Core.Broadcast( virtue, 37, "A new stage for current war has began." );
            Core.Broadcast( virtue, 37, "New war objectives are:" );

            foreach( BaseObjective objective in GetCurrentObjectives( virtue ) )
                Core.Broadcast( objective.Name );
        }

        public virtual bool IsInRightStage( BaseObjective objective, Virtue virtue )
        {
            return objective != null && virtue != Virtue.None && virtue == objective.OwnerVirtue && objective.RightStage == GetStageForVirtue( virtue );
        }

        public virtual void DoStartPreWarActions()
        {
            Logger.Log( "DoStartPreWarActions" );
        }

        public virtual void DoEndPreWarActions()
        {
            Logger.Log( "DoEndPreWarActions" );
        }

        public virtual void DoStartWarActions()
        {
            Logger.Log( "DoStartWarActions" );

            if( OrderWarState != null )
                OrderWarState.Score = 0;

            if( ChaosWarState != null )
                ChaosWarState.Score = 0;

            InitStage( 1, Virtue.Order );
            InitStage( 1, Virtue.Chaos );
        }

        public virtual void DoEndWarActions()
        {
            Logger.Log( "DoEndWarActions" );
        }

        public virtual void DoStartPostWarActions()
        {
            Logger.Log( "DoStartPostWarActions" );
        }

        public virtual void DoEndPostWarActions()
        {
            Logger.Log( "DoEndPostWarActions" );
        }

        public virtual void DoWarSliceActions()
        {
            Logger.Log( "DoWarSliceActions" );

            DoEvaluateObjectiveStatus();

            ComputePoints();
        }

        public virtual void DoEvaluateObjectiveStatus()
        {
            Logger.Log( "DoEvaluateObjectiveStatus." );

            if( OrderObjectives != null && ChaosObjectives != null )
            {
                Logger.Log( "\tOrders" );
                foreach( BaseObjective objective in OrderObjectives )
                {
                    Logger.Log( "\t{0}", objective.Name );

                    if( IsInRightStage( objective, Virtue.Order ) )
                    {
                        objective.Update();
                        objective.UpdateTime();
                    }
                }

                Logger.Log( "\tChaos" );
                foreach( BaseObjective objective in ChaosObjectives )
                {
                    if( IsInRightStage( objective, Virtue.Chaos ) )
                    {
                        objective.Update();
                        objective.UpdateTime();
                    }
                }

                if( !IsLastStage( Virtue.Order ) && OrderWarState.StageCompleted )
                    OrderWarState.Stage++;

                if( !IsLastStage( Virtue.Chaos ) && ChaosWarState.StageCompleted )
                    OrderWarState.Stage++;
            }
        }

        public virtual void ComputePoints()
        {
            Logger.Log( "ComputePoints" );

            if( OrderObjectives != null && ChaosObjectives != null )
            {
                foreach( BaseObjective objective in OrderObjectives )
                {
                    if( IsInRightStage( objective, Virtue.Order ) )
                        OrderWarState.RegisterScoreIncrease( objective.GetPoints() );
                }

                foreach( BaseObjective objective in ChaosObjectives )
                {
                    if( IsInRightStage( objective, Virtue.Chaos ) )
                        ChaosWarState.RegisterScoreIncrease( objective.GetPoints() );
                }
            }
        }

        public virtual void GiveExtraPoints( Virtue virtue, int value )
        {
            Logger.Log( "ComputePoints: {0} - {1}", virtue, value );

            if( virtue == Virtue.Order && OrderWarState != null )
                OrderWarState.RegisterScoreIncrease( value );
            else if( virtue == Virtue.Chaos && ChaosWarState != null )
                ChaosWarState.RegisterScoreIncrease( value );
        }

        public virtual Virtue ComputeWinners()
        {
            Logger.Log( "ComputePoints." );

            if( OrderWarState.Score > 0 || ChaosWarState.Score > 0 )
            {
                if( OrderWarState.Score > ChaosWarState.Score )
                    return Virtue.Order;
                else if( ChaosWarState.Score < OrderWarState.Score )
                    return Virtue.Chaos;
            }

            return Virtue.None;
        }

        public override string ToString()
        {
            return Definition.WarName;
        }

        #region serialization
        public virtual void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 ); // version

            OrderWarState.Serialize( writer );
            ChaosWarState.Serialize( writer );

            writer.WriteEncodedInt( Objectives.Count );
            foreach( BaseObjective objective in Objectives )
            {
                objective.Serialize( writer );
            }
        }

        public virtual void Deserialize( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    {
                        OrderWarState = new WarState( reader );
                        ChaosWarState = new WarState( reader );

                        int count = reader.ReadEncodedInt();

                        for( int i = 0; i < count; i++ )
                        {
                            BaseObjective objective = Objectives[ i ];

                            if( objective != null )
                                objective.Deserialize( reader );
                            else
                                Logger.Log( "Warning: wrong objective while deserializing a BaseWar" );
                        }

                        break;
                    }
            }
        }

        public static void WriteReference( GenericWriter writer, BaseWar war )
        {
            int idx = Array.IndexOf( Wars, war );

            writer.WriteEncodedInt( idx + 1 );
        }

        public static BaseWar ReadReference( GenericReader reader )
        {
            int idx = reader.ReadEncodedInt() - 1;

            if( idx >= 0 && idx < Wars.Length )
                return Wars[ idx ];

            return null;
        }
        #endregion

        public static BaseWar Parse( string name )
        {
            foreach( BaseWar war in Wars )
            {
                if( Insensitive.Equals( war.Definition.WarName, name ) )
                    return war;
            }

            return null;
        }
    }
}