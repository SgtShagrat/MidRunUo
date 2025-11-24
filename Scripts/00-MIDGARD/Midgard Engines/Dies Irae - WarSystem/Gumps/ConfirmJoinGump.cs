/***************************************************************************
 *                               ConfirmJoinGump.cs
 *
 *   begin                : 28 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Midgard.Gumps;

using Server;

namespace Midgard.Engines.WarSystem
{
    public class ConfirmJoinGump : SmallConfirmGump
    {
        public ConfirmJoinGump( Mobile from, WarStone stone )
            : base( "Will you join this war?", ConfirmJoin_Callback, stone )
        {
            from.CloseGump( typeof( ConfirmJoinGump ) );
        }

        private static void ConfirmJoin_Callback( Mobile from, bool okay, object state )
        {
            WarStone stone = state as WarStone;
            if( stone != null )
                stone.EndJoin( from, okay );
        }
    }
}