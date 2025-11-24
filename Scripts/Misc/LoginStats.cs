using System;
using Server.Network;

namespace Server.Misc
{
	public class LoginStats
	{
		public static void Initialize()
		{
			// Register our event handler
			EventSink.Login += new LoginEventHandler( EventSink_Login );
		}

		private static void EventSink_Login( LoginEventArgs args )
		{
            if( !Core.AOS )
            {
                args.Mobile.SendMessage( "Benvenuto su Midgard avventuriero. Che la sorte assista le tue leggendarie avventure." );
                return;
            }

			int userCount = NetState.Instances.Count;
			int itemCount = World.Items.Count;
			int mobileCount = World.Mobiles.Count;

			Mobile m = args.Mobile;

			try
			{
				for( int i=0; i < NetState.Instances.Count; i++ )
				{
					Mobile staffOnline = NetState.Instances[i].Mobile;
					if( staffOnline != null && !staffOnline.Deleted && staffOnline.AccessLevel > AccessLevel.GameMaster  )
					{
						if( !((Server.Mobiles.Midgard2PlayerMobile)staffOnline).OnlineVisible )
						{
							userCount--;
						}
					}
				}
			}
			catch
			{
			}
			
			#region modifica by Dies Irae
			m.SendMessage( "Benvenuto su Midgard, {0}! Al momento {1} {2} giocator{3} online.\n" +
			               "Nel mondo di Midgard ci sono {4} oggetti e {5} mobiles.\n" +
			               "Lo staff Ti augura un buon gioco sul nostro Server.",
			               args.Mobile.Name,
			               userCount == 1 ? "c'e'" : "ci sono", userCount, userCount == 1 ? "e" : "i",
			               itemCount, mobileCount );

//			m.SendMessage( "Welcome, {0}! There {1} currently {2} user{3} online, with {4} item{5} and {6} mobile{7} in the world.",
//				args.Mobile.Name,
//				userCount == 1 ? "is" : "are",
//				userCount, userCount == 1 ? "" : "s",
//				itemCount, itemCount == 1 ? "" : "s",
//				mobileCount, mobileCount == 1 ? "" : "s" );
			#endregion
		}
	}
}