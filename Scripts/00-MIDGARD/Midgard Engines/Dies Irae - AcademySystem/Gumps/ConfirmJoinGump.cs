/***************************************************************************
 *                               ConfirmJoinGump.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Midgard.Gumps;

using Server;

namespace Midgard.Engines.Academies
{
    public class ConfirmJoinGump : SmallConfirmGump
    {
        public ConfirmJoinGump( Mobile from, AcademySystem system )
            : base( system.JoinQuestion, ConfirmJoin_Callback, system )
        {
            from.CloseGump( typeof( ConfirmJoinGump ) );
        }

        private static void ConfirmJoin_Callback( Mobile from, bool okay, object state )
        {
            ( (AcademySystem)state ).AddCandidate( from );
        }
    }
}