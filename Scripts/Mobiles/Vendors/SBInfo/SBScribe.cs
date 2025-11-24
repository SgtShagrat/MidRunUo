using System;
using System.Collections.Generic;
using Server.Items;

using Midgard.Engines.OldCraftSystem;

namespace Server.Mobiles
{
	public class SBScribe: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBScribe()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( ScribesPen ), 8,  20, 0xFBF, 0 ) );
				Add( new GenericBuyInfo( typeof( BlankScroll ), 5, 999, 0xEF3, 0 ) );
				Add( new GenericBuyInfo( typeof( ScribesPen ), 8,  20, 0xFC0, 0 ) );
				Add( new GenericBuyInfo( typeof( BrownBook ), 15, 10, 0xFEF, 0 ) );
				Add( new GenericBuyInfo( typeof( TanBook ), 15, 10, 0xFF0, 0 ) );
				Add( new GenericBuyInfo( typeof( BlueBook ), 15, 10, 0xFF2, 0 ) );
				#region modifica by Dies Irae
                Add( new GenericBuyInfo( "a mail scroll", typeof( Midgard.Engines.MailSystem.MailScroll ), 50, 99, 0x227B, 0 ) );
                Add( new GenericBuyInfo( typeof( InscriptionBook ), 50, 10, 0xEFA, 0 ) );
				#endregion
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
            #region mod by Dies Irae
            public override double Demultiplicator
            {
                get { return 0.1; }
            }
            #endregion

			public InternalSellInfo()
			{
				Add( typeof( ScribesPen ), 4 );
				Add( typeof( BrownBook ), 7 );
				Add( typeof( TanBook ), 7 );
				Add( typeof( BlueBook ), 7 );
				Add( typeof( BlankScroll ), 3 );
			}
		}
	}
}