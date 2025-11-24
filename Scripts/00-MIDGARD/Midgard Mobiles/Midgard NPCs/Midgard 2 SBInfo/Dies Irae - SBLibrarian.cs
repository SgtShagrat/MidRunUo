// #define DebugSBLibrarian

/***************************************************************************
 *                                      SBLibrarian.cs
 *                            		--------------------
 *  begin                	: Marzo, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  		Definizione degli oggetti venduti e comprati per l'npc Librarian.
 *  
 ***************************************************************************/

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBLibrarian : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBLibrarian()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                // Libri per il cast
                // Add( new GenericBuyInfo( typeof( Spellbook ), 1000, 10, 0xEFA, 0 ) );
                //Add( new GenericBuyInfo( typeof( BookOfChivalry ), 10000, 5, 0x2252, 0 ) );
                //Add( new GenericBuyInfo( typeof( NecromancerSpellbook ), 10000, 5, 0x2253, 0 ) );
                // Add( new GenericBuyInfo( "1041267", typeof( Runebook ), 2500, 10, 0xEFA, 0x461 ) );
                //				Add( new GenericBuyInfo( typeof( BookOfBushido ), 50000, 5, 0x238C, 0 ) );
                //				Add( new GenericBuyInfo( typeof( BookOfNinjitsu ), 50000, 5, 0x23A0, 0 ) );

                // Alcuni libri random
                for( int i = 0; i < Loot.LibraryBookTypes.Length; i++ )
                {
                    if( Utility.RandomBool() )
                    {
                        BaseBook commonbook = (BaseBook)Loot.Construct( Loot.LibraryBookTypes[ i ] );

                        Add( new GenericBuyInfo( commonbook.GetType(), Utility.RandomMinMax( 1000, 5000 ), 1, commonbook.ItemID, Utility.RandomNeutralHue() ) );

                        if( !commonbook.Deleted )
                            commonbook.Delete();
                    }
                }

                // Alcuni libri random rari
                if( Utility.RandomDouble() < 0.1 )
                {
                    BaseBook book;
                    switch( Utility.RandomMinMax( 1, 3 ) )
                    {
                        case 1: book = Loot.RandomGrimmochJournal(); break;
                        case 2: book = Loot.RandomTavarasJournal(); break;
                        case 3: book = Loot.RandomLysanderNotebook(); break;
                        default: book = Loot.RandomLibraryBook(); break;
                    }

                    Add( new GenericBuyInfo( book.GetType(), Utility.RandomMinMax( 10000, 50000 ), 1, book.ItemID, Utility.RandomNeutralHue() ) );

                    if( !book.Deleted )
                        book.Delete();
                }
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }
}
