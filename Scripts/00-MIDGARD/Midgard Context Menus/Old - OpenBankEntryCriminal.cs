using System;
using Server;
using Server.ContextMenus;
using Server.Items;

namespace Midgard.ContextMenus
{
	public class OpenBankEntryCriminal : ContextMenuEntry
	{
		private Mobile m_Banker;

		public OpenBankEntryCriminal( Mobile from, Mobile banker ) : base( 6105, 12 )
		{
			m_Banker = banker;
		}

		public override void OnClick()
		{
			if ( !Owner.From.CheckAlive() )
				return;

			BankBox box = this.Owner.From.BankBox;

			if ( box != null )
				box.Open();
		}
	}
}
