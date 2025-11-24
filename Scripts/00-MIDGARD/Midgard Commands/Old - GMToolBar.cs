/***************************************************************************
 *                                  	GMTool.cs
 *                            			---------
 *  begin                	: Marzo 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  original concept		: unkown
 * 	rebuild					: Dies Irae
 *
 ***************************************************************************/
 
using System;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Commands
{
	public class GMTool
	{
		public static void Initialize()
		{
			CommandSystem.Register( "GMTool", AccessLevel.Counselor, new CommandEventHandler( GMTool_OnCommand ) );
		}

		[Usage( "GMTool" )]
		[Description( "Hides/Unhides the staff gump" )]
		public static void GMTool_OnCommand( CommandEventArgs e )
		{
			e.Mobile.CloseGump( typeof( ToolBarGump ) );
			e.Mobile.SendGump( new ToolBarGump() );	
		}

		public class ToolBarGump : Gump
		{
			private string[] CommandList = new string[]
			{
				"Props", "Tele", "M Tele", "Who", "Go", "Goto", "Res", "Kill", "M Kill", "Delete", "M Delete", "Self Hide", "Self Unhide",
				"Pages", "Tame", "Possess", "UnPossess", "Move", "Home", "Stuck", "RefreshMe", "Light 100"
			};

			private string[] CommandListDisplay = new string[]
			{
				"Props", "Tele", "MTele", "Who", "Go", "Goto", "Res", "Kill", "MKill", "Delete", "MDelete", "Hide", "Unhide",
				"Pages", "Tame", "Possess", "UnPossess", "Move", "Home", "Stuck", "RefreshMe", "Light"
			};

			public ToolBarGump() : base( 0, 0 )
			{
				Closable = false;
				Disposable = true;
				Dragable = true;
				Resizable = false;
	
				AddPage(0);
				AddPage(1);
	
				AddImageTiled(-1, 33, 12, 34, 5057);
				AddImageTiled(5, 35, 695, 32, 5058);
				AddImageTiled(12, 65, 700, 12, 5061);
				AddImageTiled(10, 28, 700, 12, 5055);
				AddImageTiled(700, 38, 12, 27, 5059);
				AddImage(0, 28, 5054);
				AddImage(0, 65, 5060);
				AddImage(700, 65, 5062);
				AddImage(700, 28, 5056);
	
				for( int i = 0; i < 11 ; i++ )
				{
					AddButton( 30 + i * 60, 32,  2443, 2444, i + 1, GumpButtonType.Reply, 1 );
					AddLabel( 38 + i * 60, 32, 0, CommandListDisplay[ i ] );
				}
	
				for( int j = 11; j < CommandList.Length ; j++ )
				{
					AddButton( 30 + ( j - 11 ) * 60, 51,  2443, 2444, j + 1, GumpButtonType.Reply, 1 );
					AddLabel( 38 + ( j - 11 ) * 60, 51, 0, CommandListDisplay[ j ] );
				}
			
				AddButton(9, 32, 5540, 5540, 0, GumpButtonType.Page, 2);
				
				AddPage(2);
				AddImageTiled(-1, 33, 12, 34, 5057);
				AddImageTiled(5, 35, 25, 32, 5058);
				AddImageTiled(12, 65, 25, 12, 5061);
				AddImageTiled(10, 28, 25, 12, 5055);
				AddImage(0, 28, 5054);
				AddImage(0, 65, 5060);
				AddImage(32, 65, 5062);
				AddImage(32, 28, 5056);
	
				AddButton(9, 40, 5538, 5539, 0, GumpButtonType.Page, 1);
				AddImageTiled(30, 38, 12, 27, 5059);
			}
	
			public override void OnResponse( NetState state, RelayInfo info )
	        { 
				Mobile from = state.Mobile;
	
				if( info.ButtonID <= CommandList.Length && info.ButtonID > 0 )
					CommandSystem.Handle( from, String.Format( "{0}{1}", CommandSystem.Prefix, CommandList[ info.ButtonID - 1 ] ) );
	
				from.SendGump( new ToolBarGump() );
	        }	 
		} 
	}
}
