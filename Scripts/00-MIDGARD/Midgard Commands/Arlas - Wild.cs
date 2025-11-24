using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Commands
{
	public class Wild
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Wild", AccessLevel.GameMaster, new CommandEventHandler( Wild_OnCommand ) );
		}

		[Usage( "Wild" )]
		[Description( "Stama la creatura selezionata" )]
		private static void Wild_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "Seleziona la creatura." );
			e.Mobile.Target = new SelectTarget();
		}

		private class SelectTarget : Target
		{
			public SelectTarget() : base( 20, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				if ( targ is BaseCreature )
					((BaseCreature)targ).MakeWild();
			}
		}
	}
}