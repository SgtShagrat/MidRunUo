/***************************************************************************
 *                               WarState.cs
 *                            -------------------
 *   begin                : 01 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;
using Server;

namespace Midgard.Engines.OrderChaosWars
{
    public class WarState
    {
        private int m_Stage;

        public bool StageCompleted
        {
            get
            {
                if( War == null )
                    return true;

                bool completed = true;

                List<BaseObjective> list = War.GetCurrentObjectives( StateVirtue );

                if( list != null )
                {
                    foreach( BaseObjective objective in list )
                    {
                        if( !objective.Completed )
                            completed = false;
                    }
                }

                return completed;
            }
        }

        public BaseWar War { get; set; }

        public int Stage
        {
            get { return m_Stage; }
            set
            {
                int oldValue = m_Stage;
                if( oldValue != value )
                {
                    m_Stage = value;
                    OnStageChanged( oldValue );
                }
            }
        }

        public Virtue StateVirtue { get; private set; }

        public int Score { get; set; }

        public WarState( BaseWar war, Virtue virtue )
        {
            War = war;
            StateVirtue = virtue;

            InitWarState();
        }

        public void InitWarState()
        {
            m_Stage = 0;
            Score = 0;
        }

        public void RegisterScoreIncrease( int increase )
        {
            Score += increase;
        }

        public virtual void OnStageChanged( int oldValue )
        {
            Logger.Log( "Stage for virtue {0} is now {1}", StateVirtue, m_Stage );

            if( War != null )
                War.OnAfterStageChanged( m_Stage, StateVirtue );
        }

        public WarState( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    {
                        m_Stage = reader.ReadEncodedInt();
                        StateVirtue = (Virtue)reader.ReadEncodedInt();
                        Score = reader.ReadEncodedInt();

                        War = BaseWar.ReadReference( reader );

                        break;
                    }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 ); // version

            writer.WriteEncodedInt( m_Stage );
            writer.WriteEncodedInt( (int)StateVirtue );
            writer.WriteEncodedInt( Score );

            BaseWar.WriteReference( writer, War );
        }
    }
}