/***************************************************************************
 *                               SlayObjective.cs
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
    public class SlayAllObjective : BaseObjective
    {
        private readonly List<Mobile> m_ToKillList;

        public Region Region { get; private set; }

        public SlayAllObjective( string name, int amount, string region, int seconds, WarTeam ownerTeam )
            : base( amount, seconds, name, ownerTeam )
        {
            if( region != null )
                Region = Core.GetRegion( region );

            m_ToKillList = new List<Mobile>();
        }

        private void LogEnemyList()
        {
            Logger.Log( "Slay all enemy list for team: {0}", OwnerTeam.Name );

            foreach( Mobile mobile in m_ToKillList )
                Logger.Log( "\t{0}", mobile.Name );
        }

        private void RefreshList()
        {
            for( int i = 0; i < m_ToKillList.Count; i++ )
            {
                Mobile m = m_ToKillList[ i ];
                if( m == null )
                    continue;

                WarTeam team = Utility.Find( m );
                if( team == null || OwnerTeam.Enemy != team )
                    m_ToKillList.Remove( m );
            }
        }

        public override void OnMemberAdded( WarTeam warTeam, Mobile mobile )
        {
            if( warTeam != null && warTeam == OwnerTeam.Enemy )
            {
                if( !m_ToKillList.Contains( mobile ) )
                {
                    m_ToKillList.Add( mobile );

                    Logger.Log( "Mobile {0} added to slay-all objective for team {1}.", OwnerTeam.Name );

                    LogEnemyList();
                }
            }
        }

        public override void OnMemberRemoved( WarTeam warTeam, Mobile mobile )
        {
            if( warTeam != null && warTeam == OwnerTeam.Enemy )
            {
                if( m_ToKillList.Contains( mobile ) )
                {
                    m_ToKillList.Remove( mobile );

                    Logger.Log( "Mobile {0} removed to slay-all objective for team {1}.", mobile.Name, OwnerTeam.Name );

                    LogEnemyList();
                }
            }
        }

        public override void HandleDeath( Mobile killer, Mobile killed )
        {
            // if the killer is not in the owner member do nothing...
            if( !OwnerTeam.IsMember( killer ) )
                return;

            if( IsObjective( killed ) )
            {
                Logger.Log( "Mobile {0} killed by {1} for slay-all objective - team {2}.", killed.Name, killer.Name, OwnerTeam.Name );
                Utility.Broadcast( OwnerTeam, 0x37, "Enemy {0} has been killed and the slay-all objective has been updated.", killed.Name );

                m_ToKillList.Remove( killed );
            }
        }

        public override string StatusDescription()
        {
            return string.Format( "To kill {0}.", m_ToKillList.Count );
        }

        private bool IsObjective( Mobile killed )
        {
            if( killed == null )
                return false;

            // even if the killed mobile is not a valid enemy do nothing...
            if( !OwnerTeam.Enemy.IsMember( killed ) )
                return false;

            if( !m_ToKillList.Contains( killed ) )
                return false;

            if( Utility.Find( killed ) == OwnerTeam.Enemy && m_ToKillList.Contains( killed ) )
                return Region == null || Region.Contains( killed.Location );

            return false;
        }

        public override void Update()
        {
            RefreshList();
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
        #endregion
    }
}