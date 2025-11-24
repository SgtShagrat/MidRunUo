/***************************************************************************
 *                               BaseWar.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;

namespace Midgard.Engines.WarSystem
{
    public abstract class BaseWar
    {
        public static readonly BaseWar TestWarOne = new TestWarOne();

        public static readonly BaseWar[] Wars = new BaseWar[]
                                                {
                                                    TestWarOne
                                                };

        public List<WarGate> WarGates { get; private set; }

        public WarDefinition Definition { get; set; }

        public List<BaseObjective> Objectives { get; private set; }

        public List<WarState> WarStates { get; private set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        ///     Timeout in seconds
        /// </summary>
        public int Seconds
        {
            get
            {
                if( EndTime > DateTime.Now )
                    return ( EndTime - DateTime.Now ).Seconds;
                else
                    return 0;
            }
        }

        /// <summary>
        ///     True if our war has a timeout
        /// </summary>
        public bool Timed { get; set; }

        protected BaseWar()
        {
            Objectives = new List<BaseObjective>();
            WarStates = new List<WarState>();

            // War event settings
            BattlePendingPeriod = Config.DefaultBattlePendingPeriod;
            PreBattlePeriod = Config.DefaultPreBattlePeriod;
            PostBattlePeriod = Config.DefaultPostBattlePeriod;

            RefreshDelay = Config.DefaultRefreshDelay;

            // Core settings
            DisableSavesDuringBattle = true;
            DisableTimersDuringBattle = true;

            // Death settings
            AutoRes = true;
            RefreshStatsAfterRes = true;
            RetrieveCorpseAfterRes = true;
            BroadcastDeath = true;
        }

        /// <summary>
        ///     Register an objective to our war plan
        /// </summary>
        public void AddObjective( BaseObjective objective )
        {
            if( objective == null )
                return;

            if( Objectives == null )
                Objectives = new List<BaseObjective>();

            Logger.Log( "AddObjective: {0}", objective.Name );

            objective.War = this;

            Objectives.Add( objective );
            objective.OwnerTeam.AddObjective( objective );
        }

        public virtual void AddWarState( WarTeam warTeam )
        {
            if( warTeam == null )
                return;

            if( WarStates == null )
                WarStates = new List<WarState>();

            WarState state = new WarState( this, warTeam );
            WarStates.Add( state );
            warTeam.State = state;
        }

        public virtual List<BaseObjective> GetCurrentObjectivesForTeam( WarTeam team )
        {
            Logger.Log( "GetCurrentObjectives: {0}", team );

            return team == null ? null : team.Objectives;
        }

        #region war settings
        public static TimeSpan BattlePendingPeriod = Config.DefaultBattlePendingPeriod;

        public static TimeSpan PostBattlePeriod = Config.DefaultPostBattlePeriod;

        public static TimeSpan PreBattlePeriod = Config.DefaultPreBattlePeriod;

        public static TimeSpan RefreshDelay = Config.DefaultRefreshDelay;

        public static bool DisableSavesDuringBattle { get; set; }

        public static bool DisableTimersDuringBattle { get; set; }

        public static bool AutoRes { get; set; }

        public static bool RefreshStatsAfterRes { get; set; }

        public static bool RetrieveCorpseAfterRes { get; set; }

        public static bool BroadcastDeath { get; set; }

        public static Map WarMap
        {
            get { return Map.Felucca; }
        }
        #endregion

        #region war events
        public virtual void DoStartPreWarActions()
        {
            Logger.Log( "DoStartPreWarActions" );

            foreach( BaseObjective objective in Objectives )
                objective.DoStartPreWarActions();

            Core.Instance.DisableSpawners();
        }

        public virtual void DoEndPreWarActions()
        {
            Logger.Log( "DoEndPreWarActions" );

            foreach( BaseObjective objective in Objectives )
                objective.DoEndPreWarActions();
        }

        public virtual void DoStartWarActions()
        {
            Logger.Log( "DoStartWarActions" );

            foreach( WarState state in WarStates )
                state.Score = 0;

            foreach( BaseObjective objective in Objectives )
                objective.DoStartWarActions();
        }

        public virtual void DoEndWarActions()
        {
            Logger.Log( "DoEndWarActions" );

            foreach( BaseObjective objective in Objectives )
                objective.DoEndWarActions();
        }

        public virtual void DoStartPostWarActions()
        {
            Logger.Log( "DoStartPostWarActions" );

            foreach( BaseObjective objective in Objectives )
                objective.DoStartPostWarActions();
        }

        public virtual void DoEndPostWarActions()
        {
            Logger.Log( "DoEndPostWarActions" );

            foreach( BaseObjective objective in Objectives )
                objective.DoEndPostWarActions();
        }

        public virtual void DoWarSliceActions()
        {
            Logger.Log( "DoWarSliceActions" );

            foreach( BaseObjective objective in Objectives )
                objective.DoWarSliceActions();

            ComputePoints();
        }

        public virtual void ComputePoints()
        {
            Logger.Log( "ComputePoints" );

            if( Objectives != null && Objectives.Count > 0 )
            {
                foreach( WarState warState in WarStates )
                {
                    foreach( BaseObjective objective in warState.StateTeam.Objectives )
                        warState.RegisterScoreIncrease( objective.GetPoints() );
                }
            }
        }

        public virtual bool EvaluateEndCriteria()
        {
            Logger.Log( "EvaluateEndCriteria" );

            foreach( WarState warState in WarStates )
            {
                if( warState.StateTeam.ObjectivesCompleted )
                    return true;
            }

            return false;
        }
        #endregion

        public void CreateWarStones()
        {
            foreach( WarTeam warTeam in Definition.WarTeams )
            {
                WarStone stone = new WarStone( warTeam );

                if( !Utility.FindItem( warTeam.WarStoneLocation, WarMap, stone ) )
                {
                    stone.MoveToWorld( warTeam.WarStoneLocation, WarMap );
                    warTeam.RegisterWarStone( stone );
                }
                else
                    stone.Delete();
            }
        }

        public void CreateWargates()
        {
            foreach( Point3D point3D in Definition.TravelGateLocations )
            {
                WarGate gate = new WarGate();

                if( !Utility.FindItem( point3D, WarMap, gate ) )
                {
                    gate.MoveToWorld( point3D, WarMap );
                    RegisterWarGate( gate );
                }
                else
                    gate.Delete();
            }
        }

        public void RegisterWarGate( WarGate gate )
        {
            if( gate == null )
                return;

            if( WarGates == null )
                WarGates = new List<WarGate>();

            Logger.Log( "RegisterWarGate: {0}", gate.Location );

            WarGates.Add( gate );
        }

        public virtual void GiveExtraPoints( WarTeam team, int value )
        {
            Logger.Log( "ComputePoints: {0} - {1}", team, value );

            team.State.RegisterScoreIncrease( value );
        }

        public virtual WarTeam ComputeWinners()
        {
            Logger.Log( "ComputeWinners." );

            Dictionary<WarState, int> dict = new Dictionary<WarState, int>();
            foreach( WarState state in WarStates )
                dict[ state ] = state.Score;

            // find the max score. This is a unique value. May be 0.
            int max = 0;
            foreach( KeyValuePair<WarState, int> keyValuePair in dict )
            {
                if( keyValuePair.Value > max )
                    max = keyValuePair.Value;
            }

            // find how many teams has the max value.
            int maxCount = 0;
            foreach( KeyValuePair<WarState, int> keyValuePair in dict )
            {
                if( keyValuePair.Value == max )
                    maxCount++;
            }

            // if the score is positive and we have only one leader assign the status
            if( max > 0 && maxCount == 1 )
            {
                foreach( KeyValuePair<WarState, int> keyValuePair in dict )
                {
                    if( keyValuePair.Value == max )
                        return keyValuePair.Key.StateTeam;
                }
            }

            return null;
        }

        public override string ToString()
        {
            return Definition.WarName;
        }

        public static BaseWar Parse( string name )
        {
            foreach( BaseWar war in Wars )
            {
                if( Insensitive.Equals( war.Definition.WarName, name ) )
                    return war;
            }

            return null;
        }

        public WarGateTravelDefinition[] TravelDefinitions
        {
            get { return Definition.TravelDefinitions; }
        }

        public Point3D[] TravelGateLocations
        {
            get { return Definition.TravelGateLocations; }
        }

        public void HandleDeath( Mobile killer, Mobile killed )
        {
            foreach( BaseObjective objective in Objectives )
                objective.HandleDeath( killer, killed );
        }

        public virtual void OnMemberAdded( WarTeam warTeam, Mobile mobile )
        {
            foreach( BaseObjective objective in Objectives )
                objective.OnMemberAdded( warTeam, mobile );
        }

        public virtual void OnMemberRemoved( WarTeam warTeam, Mobile mobile )
        {
            foreach( BaseObjective objective in Objectives )
                objective.OnMemberRemoved( warTeam, mobile );
        }

        #region serialization
        public virtual void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 ); // version

            writer.WriteEncodedInt( WarStates.Count );
            foreach( WarState warState in WarStates )
                warState.Serialize( writer );

            writer.WriteEncodedInt( Objectives.Count );
            foreach( BaseObjective objective in Objectives )
                objective.Serialize( writer );
        }

        public virtual void Deserialize( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    {
                        int teamCount = reader.ReadEncodedInt();

                        for( int i = 0; i < teamCount; i++ )
                        {
                            WarState state = new WarState( reader );

                            if( WarStates == null )
                                WarStates = new List<WarState>();

                            WarStates.Add( state );
                        }

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
    }
}