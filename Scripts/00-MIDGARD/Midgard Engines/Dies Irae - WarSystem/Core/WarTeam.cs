/***************************************************************************
 *                               WarTeam.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;

namespace Midgard.Engines.WarSystem
{
    public abstract class WarTeam
    {
        // public Mobile Leader { get; private set; } // may be usefull in future

        public string Name { get; private set; }
        public string Abbreviation { get; private set; }

        public WarTeam Enemy { get; private set; }

        public List<Mobile> Members { get; private set; }

        public abstract TeamHues TeamHue { get; }

        public virtual bool IsPermared
        {
            get { return false; }
        }

        public Item WarStone { get; private set; }

        public abstract Point3D WarStoneLocation { get; }

        public abstract HeadQuarterRegionDefinition HeadQuarterDefinition { get; }

        public List<BaseObjective> Objectives { get; private set; }

        public bool ObjectivesCompleted
        {
            get
            {
                foreach( BaseObjective objective in Objectives )
                {
                    if( objective.CanBeCompleted && !objective.Completed )
                        return false;
                }

                return true;
            }
        }

        public WarState State { get; set; }

        public WarTeam( string name, string abbreviation )
        {
            Name = name;
            Abbreviation = abbreviation;
        }

        public void AddMember( Mobile mobile, bool brodcastToTeam )
        {
            if( mobile == null )
                return;

            if( !IsElegible( mobile ) )
                return;

            if( Members == null )
                Members = new List<Mobile>();

            if( !Members.Contains( mobile ) )
            {
                if( brodcastToTeam )
                    Utility.Broadcast( this, 37, "{0} has joined the army!", mobile.Name );

                Utility.BroadcastToStaff( "{0} has joined the {1} army!", mobile.Name, Name );

                Members.Add( mobile );

                Core.Instance.CurrentBattle.OnMemberAdded( this, mobile );
            }
        }

        public void RemoveMember( Mobile mobile )
        {
            if( mobile == null )
                return;

            if( Members == null )
                return;

            if( Members.Contains( mobile ) )
            {
                Members.Remove( mobile );

                Logger.Log( "Member '{0}' leaved  team '{1}'.", mobile.Name, Name );

                Core.Instance.CurrentBattle.OnMemberRemoved( this, mobile );
            }
        }

        public void AddObjective( BaseObjective objective )
        {
            if( objective == null )
                return;

            if( Objectives == null )
                Objectives = new List<BaseObjective>();

            if( !Objectives.Contains( objective ) )
            {
                Objectives.Add( objective );

                Logger.Log( "Objective '{0}' registered for team {1}.", objective.Name, Name );
            }
        }

        public void AddEnemy( WarTeam team )
        {
            if( team == null )
                return;

            Enemy = team;

            Logger.Log( "Enemy '{0}' registered for team {1}.", Enemy.Name, Name );
        }

        public bool IsElegible( Mobile m )
        {
            if( IsMember( m ) )
                return false;

            return !IsEnemy( m );
        }

        public bool IsMember( Mobile m )
        {
            return Members != null && Members.Contains( m );
        }

        public bool IsEnemy( Mobile m )
        {
            return Enemy != null && Enemy.IsMember( m );
        }

        public void RegisterWarStone( WarStone stone )
        {
            if( WarStone != null && !WarStone.Deleted )
                WarStone.Delete();

            WarStone = stone;

            Logger.Log( "WarStone registered for team {0}.", Name );
        }
    }
}