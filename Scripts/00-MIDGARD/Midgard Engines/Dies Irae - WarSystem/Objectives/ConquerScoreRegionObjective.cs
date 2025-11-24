/***************************************************************************
 *                               ConquerScoreRegionObjective.cs
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
    public class ConquerScoreRegionObjective : BaseObjective
    {
        public ScoreRegion Region { get; private set; }

        public ConquerScoreRegionObjective( ScoreRegion region, string name, int seconds, WarTeam warTeam )
            : base( 1, seconds, name, warTeam )
        {
            Region = region;
        }

        public bool IsConquered
        {
            get { return ( Region != null && OwnerTeam != null && Region.OwnerTeam == OwnerTeam ); }
        }

        public override int GetPoints()
        {
            if( Region != null && OwnerTeam != null && Region.OwnerTeam == OwnerTeam )
                return Region.PointScalar;

            return base.GetPoints();
        }

        public override string StatusDescription()
        {
            return string.Format( "Conquered: {0} (pts. {1})", IsConquered, Region == null ? 0 : Region.PointScalar );
        }

        public override void Update()
        {
            Region.EvaluateStatus();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            writer.WriteEncodedInt( Region.Index );
            writer.Write( Region.OwnerTeam == null ? string.Empty : Region.OwnerTeam.Name );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            ScoreRegion r = Core.Instance.GetScoreRegionByIndex( reader.ReadEncodedInt() );
            string teamName = reader.ReadString();
            if( teamName != string.Empty )
            {
                foreach( WarState warState in Core.Instance.CurrentBattle.WarStates )
                {
                    if( warState.StateTeam.Name == teamName )
                        r.OwnerTeam = warState.StateTeam;
                }
            }
        }
    }
}