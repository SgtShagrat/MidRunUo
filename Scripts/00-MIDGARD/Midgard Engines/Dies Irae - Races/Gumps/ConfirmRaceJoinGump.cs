using Midgard.Gumps;

using Server;

namespace Midgard.Engines.Races
{
    public class ConfirmRaceJoinGump : SmallConfirmGump
    {
        public ConfirmRaceJoinGump( Mobile from, MidgardRace system, RaceGiver giver )
            : base( giver.JoinQuestion, ConfirmJoin_Callback, giver )
        {
            from.CloseGump( typeof( ConfirmRaceJoinGump ) );
        }

        private static void ConfirmJoin_Callback( Mobile from, bool okay, object state )
        {
            RaceGiver giver = state as RaceGiver;
            if( giver != null )
                giver.EndJoin( from, okay );
        }
    }
}