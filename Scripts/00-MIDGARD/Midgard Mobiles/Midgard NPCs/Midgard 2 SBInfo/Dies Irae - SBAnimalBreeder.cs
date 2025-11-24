/***************************************************************************
 *                                      SBAnimalBreeder.cs
 *                            		-------------------------
 *  begin                	: Marzo, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  		Definizione degli oggetti venduti e comprati per l'npc 
 *  		AnimalBreeder.
 *  
 ***************************************************************************/

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBAnimalBreeder : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBAnimalBreeder()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                // moved to DefTailoring
                // Add( new GenericBuyInfo( typeof( PetLeash ), 20000, 5, 0x1374, 0 ) );

                // moved to DefAlchemy
                // Add( new GenericBuyInfo( typeof( PetGreaterCurePotion ), 2000, 5, 0xF0E, 2482 ) );
                // Add( new GenericBuyInfo( typeof( PetGreaterHealPotion ), 2000, 5, 0xF0E, 2466 ) );
                
                // Add( new GenericBuyInfo( typeof( PetCurePotion ), 100, 5, 0xF0E, 2481 ) );
                // Add( new GenericBuyInfo( typeof( PetHealPotion ), 50, 5, 0xF0E, 2464 ) ); 
                // Add( new GenericBuyInfo( typeof( PetResurrectionPotion ), 2500, 5, 0xF0E, 1943 ) );
                Add( new GenericBuyInfo( typeof( PetShrinkPotion ), 5000, 5, 0xF0E, 1152 ) );
                
                // Add( new GenericBuyInfo( typeof( PetBondingDeed ), 50000, 5, 0x14F0, 1154 ) );

                // moved to DefTinkering
                // Add( new GenericBuyInfo( typeof( Midgard.Items.HitchingPostDeed ), 10000, 5, 0x14F0, 0 ) );
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add( typeof( PetCurePotion ), 10 );
                Add( typeof( PetHealPotion ), 5 );
                Add( typeof( PetResurrectionPotion ), 100 );
            }
        }
    }
}