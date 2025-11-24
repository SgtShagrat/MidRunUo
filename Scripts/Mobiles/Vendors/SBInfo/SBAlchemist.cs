using System;
using System.Collections.Generic;
using Server.Items;

using Midgard.Engines.OldCraftSystem;
using Midgard.Misc;

namespace Server.Mobiles
{
	public class SBAlchemist : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBAlchemist()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{  
				Add( new GenericBuyInfo( typeof( RefreshPotion ), 15, 10, 0xF0B, 0 ) );
				Add( new GenericBuyInfo( typeof( AgilityPotion ), 15, 10, 0xF08, 0 ) );
				Add( new GenericBuyInfo( typeof( NightSightPotion ), 15, 10, 0xF06, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserHealPotion ), 15, 10, 0xF0C, 0 ) );
				Add( new GenericBuyInfo( typeof( StrengthPotion ), 15, 10, 0xF09, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserPoisonPotion ), 15, 10, 0xF0A, 0 ) );
 				Add( new GenericBuyInfo( typeof( LesserCurePotion ), 15, 10, 0xF07, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserExplosionPotion ), 21, 10, 0xF0D, 0 ) );
				Add( new GenericBuyInfo( typeof( MortarPestle ), 8, 10, 0xE9B, 0 ) );

				Add( new GenericBuyInfo( typeof( BlackPearl ), 5 / 2, 50, 0xF7A, 0 ) );
				Add( new GenericBuyInfo( typeof( Bloodmoss ), 5 / 2, 50, 0xF7B, 0 ) );
				Add( new GenericBuyInfo( typeof( Garlic ), 3 / 2, 50, 0xF84, 0 ) );
				Add( new GenericBuyInfo( typeof( Ginseng ), 3 / 2, 50, 0xF85, 0 ) );
				Add( new GenericBuyInfo( typeof( MandrakeRoot ), 3 / 2, 50, 0xF86, 0 ) );
				Add( new GenericBuyInfo( typeof( Nightshade ), 3 / 2, 50, 0xF88, 0 ) );
				Add( new GenericBuyInfo( typeof( SpidersSilk ), 3 / 2, 50, 0xF8D, 0 ) );
				Add( new GenericBuyInfo( typeof( SulfurousAsh ), 3 / 2, 50, 0xF8C, 0 ) );

				Add( new GenericBuyInfo( typeof( Bottle ), 5, 100, 0xF0E, 0 ) ); 
				Add( new GenericBuyInfo( typeof( HeatingStand ), 2, 100, 0x1849, 0 ) ); 

				Add( new GenericBuyInfo( "1041060", typeof( HairDye ), 37, 10, 0xEFF, 0 ) );

				Add( new GenericBuyInfo( typeof( HeatingStand ), 2, 100, 0x1849, 0 ) ); // This is on OSI :-P

                #region mod by Dies Irae : pre-aos stuff
                if( Midgard2Persistance.PaganPotionsEnabled )
                {
                    Add( new GenericBuyInfo( typeof( BatWing ), 5, 20, 0xF78, 0 ) );
                    Add( new GenericBuyInfo( typeof( VolcaninAsh ), 5, 20, 0xF7F, 0 ) );
                    Add( new GenericBuyInfo( typeof( SerpentScale ), 5, 20, 0x26B6, 0 ) );
                    Add( new GenericBuyInfo( typeof( BlackMoor ), 5, 20, 0xF82, 0 ) );
                    Add( new GenericBuyInfo( typeof( DragonBlood ), 5, 20, 0xF82, 0 ) );
                    Add( new GenericBuyInfo( typeof( WyrmHeart ), 5, 20, 0xF91, 0 ) );
                    Add( new GenericBuyInfo( typeof( DeadWood ), 5, 20, 0xF90, 0 ) );
                    Add( new GenericBuyInfo( typeof( DaemonBone ), 5, 20, 0xF80, 0 ) );
                    Add( new GenericBuyInfo( typeof( Pumices ), 5, 20, 0xF8B, 0 ) );
                    Add( new GenericBuyInfo( typeof( Obsidian ), 5, 20, 0xF89, 0 ) );
                    Add( new GenericBuyInfo( typeof( ExecutionerCap ), 5, 20, 0xF83, 0 ) );
                    Add( new GenericBuyInfo( typeof( EyeOfNewt ), 5, 20, 0xF87, 0 ) );
                    Add( new GenericBuyInfo( typeof( BloodSpawn ), 5, 20, 0xF7C, 0 ) );	
                }

                Add( new GenericBuyInfo( typeof( AlchemyBook ), 50, 10, 0xEFA, 0 ) );
			    #endregion;

			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
            #region mod by Dies Irae
            public override double Demultiplicator
            {
                get { return 1.0; }
            }
            #endregion

			public InternalSellInfo()
			{
                /*
				Add( typeof( BlackPearl ), 3 ); 
				Add( typeof( Bloodmoss ), 3 ); 
				Add( typeof( MandrakeRoot ), 2 ); 
				Add( typeof( Garlic ), 2 ); 
				Add( typeof( Ginseng ), 2 ); 
				Add( typeof( Nightshade ), 2 ); 
				Add( typeof( SpidersSilk ), 2 ); 
				Add( typeof( SulfurousAsh ), 2 );
                */
               
				Add( typeof( Bottle ), 3 );
				Add( typeof( MortarPestle ), 4 );
				Add( typeof( HairDye ), 19 );

                /*
				Add( typeof( NightSightPotion ), 7 );
				Add( typeof( AgilityPotion ), 7 );
				Add( typeof( StrengthPotion ), 7 );
				Add( typeof( RefreshPotion ), 7 );
				Add( typeof( LesserCurePotion ), 7 );
				Add( typeof( LesserHealPotion ), 7 );
				Add( typeof( LesserPoisonPotion ), 7 );
				Add( typeof( LesserExplosionPotion ), 10 );
                */

                Add( typeof( RefreshPotion ), 7 );
                Add( typeof( AgilityPotion ), 7 );
                Add( typeof( NightSightPotion ), 7 );
                Add( typeof( LesserHealPotion ), 5 );
                Add( typeof( HealPotion ), 7 );
                Add( typeof( StrengthPotion ), 7 );
                Add( typeof( LesserPoisonPotion ), 5 );
                Add( typeof( PoisonPotion ), 7 );
                Add( typeof( LesserCurePotion ), 5 );
                Add( typeof( CurePotion ), 7 );
                Add( typeof( LesserExplosionPotion ), 7 );
                Add( typeof( ExplosionPotion ), 9 );
			}
		}
	}
}
