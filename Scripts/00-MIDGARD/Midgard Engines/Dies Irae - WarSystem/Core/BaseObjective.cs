/***************************************************************************
 *                               BaseObjective.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.WarSystem
{
    public abstract class BaseObjective
    {
        private int m_CurProgress;
        private int m_Seconds;

        /// <summary>
        /// Our main war
        /// </summary>
        public BaseWar War { get; set; }

        /// <summary>
        /// Name used for description in gumps
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Number of sub-objective this objective is divided into
        /// </summary>
        public int MaxProgress { get; private set; }

        /// <summary>
        /// Slay and conquer objectives can be completed or not. Depends on the main war plan.
        /// </summary>
        public bool CanBeCompleted { get { return false; } }

        /// <summary>
        /// Current objective state count
        /// </summary>
        public int CurProgress
        {
            get { return m_CurProgress; }
            set
            {
                m_CurProgress = value;

                if( Completed )
                    OnCompleted();

                if( m_CurProgress == -1 )
                    OnFailed();

                if( m_CurProgress < -1 )
                    m_CurProgress = -1;
            }
        }

        /// <summary>
        /// Timeout in seconds
        /// </summary>
        public int Seconds
        {
            get { return m_Seconds; }
            set
            {
                m_Seconds = value;

                if( m_Seconds < 0 )
                    m_Seconds = 0;
            }
        }

        /// <summary>
        /// True if our objective has a timeout
        /// </summary>
        public bool Timed { get; set; }

        public bool Completed
        {
            get { return CurProgress >= MaxProgress; }
        }

        public bool Failed
        {
            get { return CurProgress == -1; }
        }

        /// <summary>
        /// The Virtue which this objective is assigned to
        /// </summary>
        public WarTeam OwnerTeam { get; private set; }

        protected BaseObjective()
            : this( 1, 0, string.Empty, null )
        {
        }

        protected BaseObjective( int maxProgress, string name )
            : this( maxProgress, 0, name, null )
        {
        }

        protected BaseObjective( int maxProgress, int seconds, string name, WarTeam ownerTeam )
        {
            MaxProgress = maxProgress;
            m_Seconds = seconds;
            Name = name;
            OwnerTeam = ownerTeam;

            Timed = seconds > 0;
        }

        /// <summary>
        /// Points obtained for completing our objective
        /// </summary>
        /// <returns>the points obtained</returns>
        public virtual int GetPoints()
        {
            return 0;
        }

        public virtual void Complete()
        {
            CurProgress = MaxProgress;
        }

        public virtual void Fail()
        {
            CurProgress = -1;
        }

        public virtual void OnCompleted()
        {
        }

        public virtual void OnFailed()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void UpdateTime()
        {
            if( !Timed || Failed )
                return;

            if( Seconds > 0 )
            {
                Seconds -= 1;
            }
            else if( !Completed )
            {
                Fail();
            }
        }

        public virtual string StatusDescription()
        {
            return string.Empty;
        }

        #region serialization
        public virtual void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 ); // version

            writer.Write( m_CurProgress );
            writer.Write( m_Seconds );
        }

        public virtual void Deserialize( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            m_CurProgress = reader.ReadInt();
            m_Seconds = reader.ReadInt();
        }
        #endregion

        #region war events
        public virtual void DoStartPreWarActions()
        {
        }

        public virtual void DoEndPreWarActions()
        {
        }

        public virtual void DoStartWarActions()
        {
        }

        public virtual void DoEndWarActions()
        {
        }

        public virtual void DoStartPostWarActions()
        {
        }

        public virtual void DoEndPostWarActions()
        {
        }

        public virtual void DoWarSliceActions()
        {
            Logger.Log( "DoEvaluateObjectiveStatus for objective: {0}", Name );

            Update();
            UpdateTime();
        }

        public virtual void HandleDeath( Mobile killer, Mobile killed )
        {
        }
        #endregion

        public virtual void OnMemberAdded( WarTeam warTeam, Mobile mobile )
        {
        }

        public virtual void OnMemberRemoved( WarTeam warTeam, Mobile mobile )
        {
        }
    }
}