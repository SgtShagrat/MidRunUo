/***************************************************************************
 *                               ConfirmJoinGump.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Gumps;

using Server;

namespace Midgard.Engines.Classes
{
    public class ConfirmJoinGump : SmallConfirmGump
    {
        public ConfirmJoinGump( Mobile from, ClassSystem system, BaseClassGiver giver )
            : base( giver.JoinQuestion, ConfirmJoin_Callback, giver )
        {
            from.CloseGump( typeof( ConfirmJoinGump ) );
        }

        private static void ConfirmJoin_Callback( Mobile from, bool okay, object state )
        {
            BaseClassGiver giver = state as BaseClassGiver;
            if( giver != null )
                giver.EndJoin( from, okay );
        }
    }
}