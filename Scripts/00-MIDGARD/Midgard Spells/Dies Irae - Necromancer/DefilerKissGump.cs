/***************************************************************************
 *							   Dies Irae - MidgardSaveGump.cs
 *
 *   begin				: 07 November, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

//#define Gump1
// #define Gump2
#define Gump3

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
	public class DefilerKissGump : Gump
	{
		public static void Initialize()
		{
			CommandSystem.Register( "DefilerKissGump", AccessLevel.Developer, new CommandEventHandler( DefilerKiss_OnCommand ) );
		}

		[Usage( "DefilerKissGump" )]
		[Description( "Open the DefilerKiss Gump" )]
		public static void DefilerKiss_OnCommand( CommandEventArgs e )
		{
			if( e.Length == 0 )
				e.Mobile.SendGump( new DefilerKissGump() );
		}

		public DefilerKissGump() : base( 30, 30 )
		{
			Closable = false;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImage(0, 0, 155);
		}

		#region members
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			from.SendMessage( "Defiler Kiss attivo." );
		}
		#endregion
	}
}
