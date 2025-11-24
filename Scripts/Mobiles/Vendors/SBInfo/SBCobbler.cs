using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBCobbler : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBCobbler() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo()
            {
                #region mod by Dies Irae
                // http://forum.uosecondage.com/viewtopic.php?f=7&t=22181
                // Cobblers no longer sell colored sandals: This was confirmed via OSI information. Thanks Mens Rea.
                Add( new GenericBuyInfo( typeof( ThighBoots ), 15, 20, 0x1711, Core.AOS ? Utility.RandomNeutralHue() : 0 ) );
                Add( new GenericBuyInfo( typeof( Shoes ), 8, 20, 0x170f, Core.AOS ? Utility.RandomNeutralHue() : 0 ) );
                Add( new GenericBuyInfo( typeof( Boots ), 10, 20, 0x170b, Core.AOS ? Utility.RandomNeutralHue() : 0 ) );
                Add( new GenericBuyInfo( typeof( Sandals ), 5, 20, 0x170d, Core.AOS ? Utility.RandomNeutralHue() : 0 ) );
                #endregion
            } 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( Shoes ), 4 ); 
				Add( typeof( Boots ), 5 ); 
				Add( typeof( ThighBoots ), 7 ); 
				Add( typeof( Sandals ), 2 ); 
			} 
		} 
	} 
}