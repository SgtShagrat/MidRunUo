/***************************************************************************
 *                               ConquerScoreRegionObjective.cs
 *                            -------------------
 *   begin                : 01 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;

namespace Midgard.Engines.OrderChaosWars
{
    public class ConquerScoreRegionObjective : BaseObjective
    {
        public ScoreRegion Region { get; private set; }

        public ConquerScoreRegionObjective( ScoreRegion region, string name, int seconds, Virtue virtue )
            : base( 1, seconds, name, virtue )
        {
            Region = region;
        }

        public override int GetPoints()
        {
            if( Region != null && OwnerVirtue != Virtue.None && Region.RegionVirtue == OwnerVirtue )
                return Region.PointScalar;

            return base.GetPoints();
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
            writer.WriteEncodedInt( (int)Region.RegionVirtue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            // TODO: check if scoreregions is already filled
            ScoreRegion r = Core.Instance.GetScoreRegionByIndex( reader.ReadEncodedInt() );
            Virtue v = (Virtue)reader.ReadEncodedInt();

            if( r != null )
                r.RegionVirtue = v;
            else
                Console.WriteLine( "Warning: null score region while deserializing." );
        }
    }
}