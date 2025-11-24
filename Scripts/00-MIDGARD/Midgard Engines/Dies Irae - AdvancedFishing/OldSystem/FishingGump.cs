/*
using System;
using Server;
using Server.Items;

namespace Midgard.Engines.AdvancedFishing
{
	public delegate void FishingGumpCallback( Mobile from, int button, SpecialFishingPole TheSpecialRod );

	public class FishingGump : Gump
	{
		private FishingGumpCallback m_Callback;
		private object m_State;
		SpecialFishingPole m_TheSpecialRod=null;
		
		public FishingGump( int header, int headerColor, object content, int contentColor, int width, int height,SpecialFishingPole TheSpecialRod, FishingGumpCallback callback) : base( (640 - width) / 2, (480 - height) / 2 )
		{
			m_Callback = callback;
			m_TheSpecialRod=TheSpecialRod;
			Closable = true;
			
			X=0;
			Y=0;
			
			AddPage( 0 );

			AddBackground( 0, 0, 200, 100, 394 );

			AddButton( 40, 40, 116, 117, 1, GumpButtonType.Reply, 0 );
			AddButton( 90, 40, 118, 119, 2, GumpButtonType.Reply, 0 );
			AddButton( 140, 40, 114, 115, 3, GumpButtonType.Reply, 0 );
			
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 1 && m_Callback != null )
				m_Callback( sender.Mobile, 1 ,m_TheSpecialRod);
			else if ( info.ButtonID == 2 && m_Callback != null )
				m_Callback( sender.Mobile, 2 ,m_TheSpecialRod);
			else if ( info.ButtonID == 3 && m_Callback != null )
				m_Callback( sender.Mobile, 3 ,m_TheSpecialRod);
			else
				m_Callback( sender.Mobile, 0 ,m_TheSpecialRod);
		}
	}
}
*/