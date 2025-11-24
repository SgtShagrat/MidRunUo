using Midgard.Gumps;

using Server;

namespace Midgard.Engines.PermaredToBlue
{
	public class ConfirmJoinGump : SmallConfirmGump
	{
		public ConfirmJoinGump( Mobile from, BaseBlueGiver giver )
			: base( giver.JoinQuestion, ConfirmJoin_Callback, giver )
		{
			from.CloseGump( typeof( ConfirmJoinGump ) );
		}

		private static void ConfirmJoin_Callback( Mobile from, bool okay, object state )
		{
			BaseBlueGiver giver = state as BaseBlueGiver;
			if( giver != null )
				giver.EndJoin( from, okay );
		}
	}
}