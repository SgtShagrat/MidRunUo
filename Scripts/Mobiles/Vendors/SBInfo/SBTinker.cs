using System; 
using System.Collections.Generic;

using Midgard.Engines.SpellSystem;

using Server.Items; 

using Midgard.Engines.OldCraftSystem;
using Midgard.Items;

namespace Server.Mobiles 
{ 
	public class SBTinker: SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBTinker() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{ 

				Add( new GenericBuyInfo( typeof( Clock ), 22, 20, 0x104B, 0 ) );
				Add( new GenericBuyInfo( typeof( Nails ), 3, 20, 0x102E, 0 ) );
				Add( new GenericBuyInfo( typeof( ClockParts ), 3, 20, 0x104F, 0 ) );
				Add( new GenericBuyInfo( typeof( AxleGears ), 3, 20, 0x1051, 0 ) );
				Add( new GenericBuyInfo( typeof( Gears ), 2, 20, 0x1053, 0 ) );
				Add( new GenericBuyInfo( typeof( Hinge ), 2, 20, 0x1055, 0 ) );

				Add( new GenericBuyInfo( typeof( Sextant ), 13, 20, 0x1057, 0 ) );
				Add( new GenericBuyInfo( typeof( SextantParts ), 5, 20, 0x1059, 0 ) );
				Add( new GenericBuyInfo( typeof( Axle ), 2, 20, 0x105B, 0 ) );
				Add( new GenericBuyInfo( typeof( Springs ), 3, 20, 0x105D, 0 ) );

				Add( new GenericBuyInfo( "1024111", typeof( Key ), 8, 20, 0x100F, 0 ) );
				Add( new GenericBuyInfo( "1024112", typeof( Key ), 8, 20, 0x1010, 0 ) );
				Add( new GenericBuyInfo( "1024115", typeof( Key ), 8, 20, 0x1013, 0 ) );
				Add( new GenericBuyInfo( typeof( KeyRing ), 8, 20, /*0x1010*/ 0x1011, 0 ) ); // mod by Dies Irae
				Add( new GenericBuyInfo( typeof( Lockpick ), 12, 20, 0x14FC, 0 ) );

				Add( new GenericBuyInfo( typeof( TinkerTools ), 7, 20, 0x1EB8, 0 ) );
				Add( new GenericBuyInfo( typeof( Board ), 3, 20, 0x1BD7, 0 ) );
				Add( new GenericBuyInfo( typeof( IronIngot ), 5, 16, 0x1BF2, 0 ) );
				Add( new GenericBuyInfo( typeof( SewingKit ), 3, 20, 0xF9D, 0 ) );

				Add( new GenericBuyInfo( typeof( DrawKnife ), 10, 20, 0x10E4, 0 ) );
				Add( new GenericBuyInfo( typeof( Froe ), 10, 20, 0x10E5, 0 ) );
				Add( new GenericBuyInfo( typeof( Scorp ), 10, 20, 0x10E7, 0 ) );
				Add( new GenericBuyInfo( typeof( Inshave ), 10, 20, 0x10E6, 0 ) );

				Add( new GenericBuyInfo( typeof( ButcherKnife ), 13, 20, 0x13F6, 0 ) );

				Add( new GenericBuyInfo( typeof( Scissors ), 11, 20, 0xF9F, 0 ) );

				Add( new GenericBuyInfo( typeof( Tongs ), 13, 14, 0xFBB, 0 ) );

				Add( new GenericBuyInfo( typeof( DovetailSaw ), 12, 20, 0x1028, 0 ) );
				Add( new GenericBuyInfo( typeof( Saw ), 15, 20, 0x1034, 0 ) );

				Add( new GenericBuyInfo( typeof( Hammer ), 17, 20, 0x102A, 0 ) );
				Add( new GenericBuyInfo( typeof( SmithHammer ), 23, 20, 0x13E3, 0 ) );
				// TODO: Sledgehammer

				Add( new GenericBuyInfo( typeof( Shovel ), 12, 20, 0xF39, 0 ) );

				Add( new GenericBuyInfo( typeof( MouldingPlane ), 11, 20, 0x102C, 0 ) );
				Add( new GenericBuyInfo( typeof( JointingPlane ), 10, 20, 0x1030, 0 ) );
				Add( new GenericBuyInfo( typeof( SmoothingPlane ), 11, 20, 0x1032, 0 ) );

				Add( new GenericBuyInfo( typeof( Pickaxe ), 25, 20, 0xE86, 0 ) );


				Add( new GenericBuyInfo( typeof( Drums ), 21, 20, 0x0E9C, 0 ) );
				Add( new GenericBuyInfo( typeof( Tambourine ), 21, 20, 0x0E9E, 0 ) );
				Add( new GenericBuyInfo( typeof( LapHarp ), 21, 20, 0x0EB2, 0 ) );
				Add( new GenericBuyInfo( typeof( Lute ), 21, 20, 0x0EB3, 0 ) );

                #region mod by Dies Irae
                Add( new GenericBuyInfo( typeof( SiegeRepairTool ), 250, 20, 0x13E4, 0 ) );
                Add( new GenericBuyInfo( typeof( SiegeCommandTool ), 250, 20, 0x1EBA, 0 ) );

                Add( new GenericBuyInfo( typeof( SiegeCannonDeed ), 5000, 5, 0x14F0, 0x488 ) );
                Add( new GenericBuyInfo( typeof( LightCannonball ), 50, 20, 0xE74, 0 ) );
                Add( new GenericBuyInfo( typeof( IronCannonball ), 100, 20, 0xE74, 0 ) );
                Add( new GenericBuyInfo( typeof( ExplodingCannonball ), 150, 20, 0xE74, 0 ) );
                Add( new GenericBuyInfo( typeof( FieryCannonball ), 200, 20, 0xE74, 0 ) );
                Add( new GenericBuyInfo( typeof( GrapeShot ), 250, 20, 0xE74, 0 ) );

                Add( new GenericBuyInfo( typeof( SiegeCatapultDeed ), 2500, 20, 0x14F0, 0 ) );

                Add( new GenericBuyInfo( typeof( SiegeRamDeed ), 2000, 20, 0x14F0, 0x488 ) );
                Add( new GenericBuyInfo( typeof( LightSiegeLog ), 50, 20, 0x1BDE, 0 ) );
                Add( new GenericBuyInfo( typeof( HeavySiegeLog ), 100, 20, 0x1BDE, 0 ) );
                Add( new GenericBuyInfo( typeof( IronSiegeLog ), 150, 20, 0x1BDE, 0 ) );

                Add( new GenericBuyInfo( typeof( TinkeringBook ), 50, 10, 0xEFA, 0 ) );

                Add( new GenericBuyInfo( "crystal bottle", typeof( BlessedDrop ), 10, 20, 0xF0E, 0 ) );
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
				Add( typeof( Drums ), 10 );
				Add( typeof( Tambourine ), 10 );
				Add( typeof( LapHarp ), 10 );
				Add( typeof( Lute ), 10 );

				Add( typeof( Shovel ), 6 );
				Add( typeof( SewingKit ), 1 );
				Add( typeof( Scissors ), 6 );
				Add( typeof( Tongs ), 7 );
				Add( typeof( Key ), 1 );

				Add( typeof( DovetailSaw ), 6 );
				Add( typeof( MouldingPlane ), 6 );
				Add( typeof( Nails ), 1 );
				Add( typeof( JointingPlane ), 6 );
				Add( typeof( SmoothingPlane ), 6 );
				Add( typeof( Saw ), 7 );

				Add( typeof( Clock ), 11 );
				Add( typeof( ClockParts ), 1 );
				Add( typeof( AxleGears ), 1 );
				Add( typeof( Gears ), 1 );
				Add( typeof( Hinge ), 1 );
				Add( typeof( Sextant ), 6 );
				Add( typeof( SextantParts ), 2 );
				Add( typeof( Axle ), 1 );
				Add( typeof( Springs ), 1 );

				Add( typeof( DrawKnife ), 5 );
				Add( typeof( Froe ), 5 );
				Add( typeof( Inshave ), 5 );
				Add( typeof( Scorp ), 5 );

				Add( typeof( Lockpick ), /* 6 */ 2 );
				Add( typeof( TinkerTools ), 3 );

				Add( typeof( Board ), 1 );
				Add( typeof( Log ), 1 );

				Add( typeof( Pickaxe ), 16 );
				Add( typeof( Hammer ), 3 );
				Add( typeof( SmithHammer ), 11 );
				Add( typeof( ButcherKnife ), 6 );
			} 
		} 
	} 
}