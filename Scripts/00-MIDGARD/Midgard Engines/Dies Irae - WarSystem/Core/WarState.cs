/***************************************************************************
 *                               WarState.cs
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
    public class WarState
    {
        public BaseWar OwnerWar { get; set; }

        public WarTeam StateTeam { get; set; }

        public int Score { get; set; }

        public WarState( BaseWar war, WarTeam team )
        {
            OwnerWar = war;
            StateTeam = team;

            InitWarState();
        }

        public void InitWarState()
        {
            Score = 0;
        }

        public void RegisterScoreIncrease( int increase )
        {
            Score += increase;
        }

        public WarState( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    {
                        // Team = (WarTeam)reader.ReadEncodedInt(); // TODO write reference
                        Score = reader.ReadEncodedInt();

                        OwnerWar = BaseWar.ReadReference( reader );

                        break;
                    }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 ); // version

            // writer.WriteEncodedInt( (int)StateVirtue ); // TODO write reference
            writer.WriteEncodedInt( Score );

            BaseWar.WriteReference( writer, OwnerWar );
        }
    }
}