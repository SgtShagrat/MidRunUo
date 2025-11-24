/***************************************************************************
 *                               ScoreRegion.cs
 *                            --------------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/
 
using System.Collections.Generic;
using Server;
using Server.Regions;

namespace Midgard.Engines.OrderChaosWars
{
    public class ScoreRegion : BaseRegion
    {
        private Virtue m_RegionVirtue;

        public ScoreRegion( ScoreRegionDefinition def )
            : base( def.Name, Core.WarMap, DefaultPriority, def.Area )
        {
            Index = def.Index;
            PointScalar = def.PointScalar;

            if( Config.Enabled )
            {
                Core.Instance.RegisterScoreRegion( this );
            }
        }

        public bool IsConquerable
        {
            get { return Config.Enabled && RegionVirtue == Virtue.None; }
        }

        public Virtue RegionVirtue
        {
            get { return m_RegionVirtue; }
            set
            {
                Virtue oldValue = m_RegionVirtue;
                if( oldValue != value )
                {
                    OnVirtueChanged( oldValue );

                    m_RegionVirtue = value;
                }
            }
        }

        public int PointScalar { get; private set; }

        public int Index { get; private set; }

        public override bool OnMoveInto( Mobile m, Direction d, Point3D newLocation, Point3D oldLocation )
        {
            if( !base.OnMoveInto( m, d, newLocation, oldLocation ) )
                return false;

            if( Core.Instance.Phase != WarPhase.Idle && Core.Find( m ) == Virtue.None )
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

        public virtual void OnVirtueChanged( Virtue oldValue )
        {
            List<DecorationItem> decos = GetDecos();

            foreach( DecorationItem deco in decos )
                deco.DecoVirtue = m_RegionVirtue;

            Core.Broadcast( "{0} faction has conquered the {1} region.", Core.GetVirtueName( m_RegionVirtue ), Name );
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

        public int GetPlayers( Virtue virtue )
        {
            List<Mobile> list = GetPlayers();

            int count = 0;

            foreach( Mobile m in list )
            {
                if( m != null && m.Player && Core.Find( m, false ) == virtue )
                    count++;
            }

            return count;
        }

        public void EvaluateStatus()
        {
            int order = GetPlayers( Virtue.Order );
            int chaos = GetPlayers( Virtue.Chaos );

            if( order > 0 || chaos > 0 )
            {
                if( order > chaos )
                    m_RegionVirtue = Virtue.Order;
                else if( chaos > order )
                    m_RegionVirtue = Virtue.Chaos;
                else
                    m_RegionVirtue = Virtue.None;
            }
        }
    }
}