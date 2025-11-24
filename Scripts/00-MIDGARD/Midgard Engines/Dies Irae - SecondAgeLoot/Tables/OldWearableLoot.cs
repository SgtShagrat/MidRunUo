/***************************************************************************
 *                               OldWearableLoot.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Midgard.Items;

using Server.Items;

namespace Midgard.Engines.SecondAgeLoot
{
    public static class OldWearableLoot
    {
        #region [item list]

        /// <summary>
        /// { 5397 1 5424 1 };
        /// </summary>
        private static readonly LootItem[] GetCloak = new LootItem[]
                                                      {
                                                          new LootItem( typeof( Cloak ), 1 ),
                                                          new LootItem( typeof( Cloak ), 1, 0x1530 ),
                                                      };

        /// <summary>
        /// 5905 1 // thigh boots
        /// 5906 1 // thigh boots
        /// 5899 1 // black boots
        /// 5900 1 // black boots
        /// </summary>
        private static readonly LootItem[] GetBoot = new LootItem[]
                                                     {
                                                         new LootItem( typeof( ThighBoots ), 1 ),
                                                         new LootItem( typeof( ThighBoots ), 1 ),
                                                         new LootItem( typeof( Boots ), 1 ),
                                                         new LootItem( typeof( Boots ), 1 ),
                                                     };

        /// <summary>
        /// { 5062 1 /* leather gloves */ };
        /// </summary>
        private static readonly LootItem[] GetGlove = new LootItem[]
                                                      {
                                                          new LootItem( typeof( LeatherGloves ), 1 )
                                                      };

        /// <summary>
        /// 5908 1 // Hats 1
        /// 5909 1 // Hats 2
        /// 5910 1 // Hats 3
        /// 5911 1 // Hats 4
        /// 5912 1 // Hats 5
        /// 5913 1 // Hats 6
        /// 5914 1 // Hats 7
        /// 5915 1 // Hats 8
        /// 5916 1 // Hats 9
        /// </summary>
        private static readonly LootItem[] GetHat = new LootItem[]
                                                    {
                                                        new LootItem( typeof( WideBrimHat ), 1 ),
                                                        new LootItem( typeof( Cap ), 1 ),
                                                        new LootItem( typeof( TallStrawHat ), 1 ),
                                                        new LootItem( typeof( StrawHat ), 1 ),
                                                        new LootItem( typeof( WizardsHat ), 1 ),
                                                        new LootItem( typeof( Bonnet ), 1 ),
                                                        new LootItem( typeof( FeatheredHat ), 1 ),
                                                        new LootItem( typeof( TricorneHat ), 1 ),
                                                        new LootItem( typeof( JesterHat ), 1 )
                                                    };

        /// <summary>
        /// 5441 1 // gold belt
        /// </summary>
        private static readonly LootItem[] GetBelt = new LootItem[]
                                                     {
                                                         new LootItem( typeof( Belt ), 1 )
                                                     };

        /// <summary>
        /// 5445 1 // bear mask 1
        /// 5446 1 // bear mask 2
        /// 5447 1 // deer mask 1
        /// 5448 1 // deer mask 2
        /// 5449 1 // tribal mask 1
        /// 5450 1 // tribal mask 2
        /// 5451 1 // tribal mask 3
        /// 5452 1 // tribal mask 4
        /// </summary>
        private static readonly LootItem[] GetMask = new LootItem[]
                                                     {
                                                         new LootItem( typeof( BearMask ), 1 ),
                                                         new LootItem( typeof( BearMask ), 1 ),
                                                         new LootItem( typeof( DeerMask ), 1 ),
                                                         new LootItem( typeof( DeerMask ), 1 ),
                                                         new LootItem( typeof( TribalMask ), 1 ),
                                                         new LootItem( typeof( TribalMask ), 1 ),
                                                         new LootItem( typeof( TribalMask ), 1 ),
                                                         new LootItem( typeof( TribalMask ), 1 )
                                                     };

        /// <summary>
        /// 4229 1 // necklace 1
        /// 4232 1 // necklace 2
        /// 4233 1 // necklace 3
        /// </summary>
        private static readonly LootItem[] GetNecklace = new LootItem[]
                                                         {
                                                             new LootItem( typeof( Necklace ), 1 ),
                                                             new LootItem( typeof( Necklace ), 1 ),
                                                             new LootItem( typeof( Necklace ), 1 )
                                                         };

        /// <summary>
        /// { 4230 1 };
        /// </summary>
        private static readonly LootItem[] GetBracelet = new LootItem[]
                                                         {
                                                             new LootItem( typeof( GoldBracelet ), 1 )
                                                         };

        /// <summary>
        /// { 4231 1 };
        /// </summary>
        private static readonly LootItem[] GetEaring = new LootItem[]
                                                       {
                                                           new LootItem( typeof( GoldEarrings ), 1 )
                                                       };


        /// <summary>
        /// { 4234 1 };
        /// </summary>
        private static readonly LootItem[] GetRing = new LootItem[]
                                                     {
                                                         new LootItem( typeof( GoldRing ), 1 )
                                                     };
        #endregion

        /// <summary>
        /// { cloaks 1 boots 1 gloves 1 hats 1 belts 1 masks 1 }
        /// </summary>
        public static readonly LootTable GetClothing = new LootTable( new TableEntry[]
                                                                      {
                                                                          new ListEntry( GetCloak, 1 ),
                                                                          new ListEntry( GetBoot, 1 ),
                                                                          new ListEntry( GetGlove, 1 ),
                                                                          new ListEntry( GetHat, 1 ),
                                                                          new ListEntry( GetBelt, 1 ),
                                                                          new ListEntry( GetMask, 1 ),
                                                                      } );

        /// <summary>
        /// { necklaces 1 bracelets 1 earings 1 rings 1 };
        /// </summary>
        public static readonly LootTable GetJewelry = new LootTable( new TableEntry[]
                                                                     {
                                                                         new ListEntry( GetNecklace, 1 ),
                                                                         new ListEntry( GetBracelet, 1 ),
                                                                         new ListEntry( GetEaring, 1 ),
                                                                         new ListEntry( GetRing, 1 )
                                                                     } );

        /// <summary>
        /// { clothing 1 jewelry 1 };
        /// </summary>
        public static readonly LootTable Wearable = new LootTable( new TableEntry[]
                                                                   {
                                                                       new TableEntry( GetClothing, 1 ),
                                                                       new TableEntry( GetJewelry, 1 )
                                                                   } );

        public static readonly LootTable RandomHat = new LootTable( GetHat );
    }
}