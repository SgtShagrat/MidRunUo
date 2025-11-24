/***************************************************************************
 *                               ScoreRegion.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;
using Server.Regions;

namespace Midgard.Engines.WarSystem
{
    public class ScoreRegion : BaseRegion
    {
        private WarTeam m_OwnerTeam;

        public ScoreRegion( ScoreRegionDefinition def )
            : base( def.Name, BaseWar.WarMap, DefaultPriority, def.Area )
        {
            Index = def.Index;
            PointScalar = def.PointScalar;

            if( Config.Enabled )
                Core.Instance.RegisterScoreRegion( this );
        }

        public bool IsConquerable
        {
            get { return Config.Enabled && m_OwnerTeam == null; }
        }

        public WarTeam OwnerTeam
        {
            get { return m_OwnerTeam; }
            set
            {
                WarTeam oldValue = m_OwnerTeam;
                if( oldValue != value )
                {
                    OnOwnerChanged( oldValue );

                    m_OwnerTeam = value;
                }
            }
        }

        public int PointScalar { get; private set; }

        public int Index { get; private set; }

        public override bool OnMoveInto( Mobile m, Direction d, Point3D newLocation, Point3D oldLocation )
        {
            if( !base.OnMoveInto( m, d, newLocation, oldLocation ) )
                return false;

            if( Core.Instance.Phase != WarPhase.Idle && Utility.Find( m ) == null )
            {
                m.SendMessage( "You cannot enter this building while a war is in progress." );
                return false;
            }

            return true;
        }

        public override bool AllowHousing( Mobile from, Point3D p )
        {
            return false;
        }

        public virtual void OnOwnerChanged( WarTeam oldValue )
        {
            List<DecorationItem> decos = GetDecos();

            foreach( DecorationItem deco in decos )
                deco.DecoTeam = m_OwnerTeam;

            Utility.Broadcast( "{0} team has conquered the {1} region.", m_OwnerTeam.Name, Name );
        }

        public List<DecorationItem> GetDecos()
        {
            List<DecorationItem> decos = new List<DecorationItem>();
            IPooledEnumerable eable = null;

            foreach( Rectangle3D rect in Area )
            {
                Rectangle2D rect2D = new Rectangle2D( new Point2D( rect.Start.X, rect.Start.Y ), new Point2D( rect.End.X, rect.End.Y ) );

                eable = Map.GetItemsInBounds( rect2D );

                foreach( Item item in eable )
                {
                    if( item is DecorationItem && Contains( item.Location ) )
                        decos.Add( (DecorationItem)item );
                }
            }

            if( eable != null )
                eable.Free();

            return decos;
        }

        public int GetPlayers( WarTeam team )
        {
            List<Mobile> list = GetPlayers();

            int count = 0;

            foreach( Mobile m in list )
            {
                if( m != null && m.Player && Utility.Find( m, false ) == team )
                    count++;
            }

            return count;
        }

        public void EvaluateStatus()
        {
            Dictionary<WarTeam, int> dict = new Dictionary<WarTeam, int>();

            foreach( WarTeam warTeam in Core.Instance.CurrentBattle.Definition.WarTeams )
                dict[ warTeam ] = GetPlayers( warTeam );

            // find the max score. This is a unique value. May be 0.
            int max = 0;
            foreach( KeyValuePair<WarTeam, int> keyValuePair in dict )
            {
                if( keyValuePair.Value > max )
                    max = keyValuePair.Value;
            }

            // find how many teams has the max value.
            int maxCount = 0;
            foreach( KeyValuePair<WarTeam, int> keyValuePair in dict )
            {
                if( keyValuePair.Value == max )
                    maxCount++;
            }

            // if the score is positive and we have only one leader assign the status
            if( max > 0 && maxCount == 1 )
            {
                foreach( KeyValuePair<WarTeam, int> keyValuePair in dict )
                {
                    if( keyValuePair.Value == max )
                        m_OwnerTeam = keyValuePair.Key;
                }
            }
        }
    }
}