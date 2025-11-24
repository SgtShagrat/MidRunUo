using System;
using System.Collections.Generic;

using Server.Items;

using Midgard.Items;

namespace Server.Mobiles
{
	public class SBMage : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMage()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				// Add( new GenericBuyInfo( typeof( Spellbook ), 18, 10, 0xEFA, 0 ) );
				Add( new GenericBuyInfo( typeof( Spellbook ), 47, 10, 0xEFA, 0 ) );

				if ( Core.AOS )
					Add( new GenericBuyInfo( typeof( NecromancerSpellbook ), 115, 10, 0x2253, 0 ) );
				
				Add( new GenericBuyInfo( typeof( ScribesPen ), 8, 10, 0xFBF, 0 ) );

				Add( new GenericBuyInfo( typeof( BlankScroll ), 5, 20, 0x0E34, 0 ) );

				Add( new GenericBuyInfo( "1041072", typeof( MagicWizardsHat ), 11, 10, 0x1718, Utility.RandomDyedHue() ) );

				Add( new GenericBuyInfo( typeof( RecallRune ), 15, 10, 0x1F14, 0 ) );

                #region mod by Dies Irae
                Add( new GenericBuyInfo( "wand of identification",typeof( SellableIDWandLesser ), 3000, 10, Utility.RandomList( 0xDF2, 0xDF3, 0xDF4, 0xDF5 ), 0 ) );
                Add( new GenericBuyInfo( "wand of identification",typeof( SellableIDWandNormal ), 5000, 10, Utility.RandomList( 0xDF2, 0xDF3, 0xDF4, 0xDF5 ), 0 ) );
                Add( new GenericBuyInfo( "wand of identification",typeof( SellableIDWandGreater ), 10000, 10, Utility.RandomList( 0xDF2, 0xDF3, 0xDF4, 0xDF5 ), 0 ) );
                Add( new GenericBuyInfo( typeof( MageStaff ), 1000, 10, 0x13F8, 0 ) );
                #endregion

				Add( new GenericBuyInfo( typeof( RefreshPotion ), 15, 10, 0xF0B, 0 ) );
				Add( new GenericBuyInfo( typeof( AgilityPotion ), 15, 10, 0xF08, 0 ) );
				Add( new GenericBuyInfo( typeof( NightSightPotion ), 15, 10, 0xF06, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserHealPotion ), 15, 10, 0xF0C, 0 ) );
				Add( new GenericBuyInfo( typeof( StrengthPotion ), 15, 10, 0xF09, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserPoisonPotion ), 15, 10, 0xF0A, 0 ) );
 				Add( new GenericBuyInfo( typeof( LesserCurePotion ), 15, 10, 0xF07, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserExplosionPotion ), 21, 10, 0xF0D, 0 ) );

				Add( new GenericBuyInfo( typeof( BlackPearl ), 5 / 2, 50, 0xF7A, 0 ) );
				Add( new GenericBuyInfo( typeof( Bloodmoss ), 5 / 2, 50, 0xF7B, 0 ) );
				Add( new GenericBuyInfo( typeof( Garlic ), 3 / 2, 50, 0xF84, 0 ) );
				Add( new GenericBuyInfo( typeof( Ginseng ), 3 / 2, 50, 0xF85, 0 ) );
				Add( new GenericBuyInfo( typeof( MandrakeRoot ), 3 / 2, 50, 0xF86, 0 ) );
				Add( new GenericBuyInfo( typeof( Nightshade ), 3 / 2, 50, 0xF88, 0 ) );
				Add( new GenericBuyInfo( typeof( SpidersSilk ), 3 / 2, 50, 0xF8D, 0 ) );
				Add( new GenericBuyInfo( typeof( SulfurousAsh ), 3 / 2, 50, 0xF8C, 0 ) );

				if ( Core.AOS )
				{
					Add( new GenericBuyInfo( typeof( BatWing ), 3, 999, 0xF78, 0 ) );
					Add( new GenericBuyInfo( typeof( DaemonBlood ), 6, 999, 0xF7D, 0 ) );
					Add( new GenericBuyInfo( typeof( PigIron ), 5, 999, 0xF8A, 0 ) );
					Add( new GenericBuyInfo( typeof( NoxCrystal ), 6, 999, 0xF8E, 0 ) );
					Add( new GenericBuyInfo( typeof( GraveDust ), 3, 999, 0xF8F, 0 ) );
				}

				Type[] types = Loot.RegularScrollTypes;

				int circles = 3;

				for ( int i = 0; i < circles*8 && i < types.Length; ++i )
				{
					int itemID = 0x1F2E + i;

					if ( i == 6 )
						itemID = 0x1F2D;
					else if ( i > 6 )
						--itemID;

                    int price = 12 + 5 + ( i / 8 ) * 2; // mod by Dies Irae

                    Add( new GenericBuyInfo( types[ i ], (int)( price * 1.90 ) /* 12 + ((i / 8) * 10) */, 20, itemID, 0 ) );
				}
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( WizardsHat ), 15 );

                /*
				Add( typeof( BlackPearl ), 3 ); 
				Add( typeof( Bloodmoss ),4 ); 
				Add( typeof( MandrakeRoot ), 2 ); 
				Add( typeof( Garlic ), 2 ); 
				Add( typeof( Ginseng ), 2 ); 
				Add( typeof( Nightshade ), 2 ); 
				Add( typeof( SpidersSilk ), 2 ); 
				Add( typeof( SulfurousAsh ), 2 ); 
                */

				if ( Core.AOS )
				{
					Add( typeof( BatWing ), 1 );
					Add( typeof( DaemonBlood ), 3 );
					Add( typeof( PigIron ), 2 );
					Add( typeof( NoxCrystal ), 3 );
					Add( typeof( GraveDust ), 1 );
				}

				Add( typeof( RecallRune ), /* 13 */ 10 );
				Add( typeof( Spellbook ), 25 );

				Type[] types = Loot.RegularScrollTypes;

                for( int i = 0; i < types.Length; ++i )
                {
                    int price = 6 + ( i / 8 ); // 12 + 5 + ( i / 8 ); // i + 3 + (i / 4)
                    Type t = types[ i ];

                    Add( t, price ); // This is NOT 100% OSI accurate. Two spells per circle will be off by 1gp, as OSI's math is slightly different.}
                }

			    if ( Core.SE )
				{				
					Add( typeof( ExorcismScroll ), 1 );
					Add( typeof( AnimateDeadScroll ), 26 );
					Add( typeof( BloodOathScroll ), 26 );
					Add( typeof( CorpseSkinScroll ), 26 );
					Add( typeof( CurseWeaponScroll ), 26 );
					Add( typeof( EvilOmenScroll ), 26 );
					Add( typeof( PainSpikeScroll ), 26 );
					Add( typeof( SummonFamiliarScroll ), 26 );
					Add( typeof( HorrificBeastScroll ), 27 );
					Add( typeof( MindRotScroll ), 39 );
					Add( typeof( PoisonStrikeScroll ), 39 );
					Add( typeof( WraithFormScroll ), 51 );
					Add( typeof( LichFormScroll ), 64 );
					Add( typeof( StrangleScroll ), 64 );
					Add( typeof( WitherScroll ), 64 );
					Add( typeof( VampiricEmbraceScroll ), 101 );
					Add( typeof( VengefulSpiritScroll ), 114 );
			}

		}
	}
	}
}