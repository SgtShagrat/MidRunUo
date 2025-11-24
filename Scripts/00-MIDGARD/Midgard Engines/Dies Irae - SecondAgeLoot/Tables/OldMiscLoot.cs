/***************************************************************************
 *                               OldMiscLoot.cs
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
    public static class OldMiscLoot
    {
        #region [item list]

        /// <summary>
        /// 3846 1 // Black Potion  
        /// 3847 1 // Orange Potion  
        /// 3848 1 // Blue Potion 
        /// 3849 1 // White potion
        /// 3850 1 // green potion 
        /// 3851 1 // Red Potion  
        /// 3852 1 // Yellow Potion  
        /// 3853 1 // Purple Potion
        /// </summary>
        private static readonly LootItem[] GetPotion = new LootItem[]
                                                       {
                                                           new LootItem( typeof( NightSightPotion ), 1 ),
                                                           new LootItem( typeof( CurePotion ), 1 ),
                                                           new LootItem( typeof( AgilityPotion ), 1 ),
                                                           new LootItem( typeof( StrengthPotion ), 1 ),
                                                           new LootItem( typeof( PoisonPotion ), 1 ),
                                                           new LootItem( typeof( StrengthPotion ), 1 ),
                                                           new LootItem( typeof( HealPotion ), 1 ),
                                                           new LootItem( typeof( ExplosionPotion ), 1 )
                                                       };

        /// <summary>
        /// 3637 1	// scroll	2
        /// 3638 1	// scroll	3
        /// 3638 1 	// scroll	4
        /// 3639 1	// scroll	5
        /// 3640 1	// scroll	6
        /// 3641 1	// scroll   7
        /// 3642 1 	// scroll	8
        /// 3828 1 	// scroll	10
        /// 3829 1 	// scroll	11																
        /// 3830 1 	// scroll	12
        /// 3831 1 	// scroll	13
        /// 3833 1 	// scroll	15
        /// </summary>
        public static readonly LootItem[] GetScroll = new LootItem[]
                                                      {
                                                          new LootItem( typeof( ClumsyScroll ), 1 ),
                                                          new LootItem( typeof( CreateFoodScroll ), 1 ),
                                                          new LootItem( typeof( FeeblemindScroll ), 1 ),
                                                          new LootItem( typeof( HealScroll ), 1 ),
                                                          new LootItem( typeof( MagicArrowScroll ), 1 ),
                                                          new LootItem( typeof( NightSightScroll ), 1 ),
                                                          new LootItem( typeof( WeakenScroll ), 1 ),
                                                          new LootItem( typeof( CunningScroll ), 1 ),
                                                          new LootItem( typeof( CureScroll ), 1 ),
                                                          new LootItem( typeof( HarmScroll ), 1 ),
                                                          new LootItem( typeof( MagicTrapScroll ), 1 ),
                                                          new LootItem( typeof( ProtectionScroll ), 1 )
                                                      };

        /// <summary>
        /// 3570 1	// magic wand 1
        /// 3571 1	// magic wand 2
        /// 3572 1	// magic wand 3
        /// 3573 1	// magic wand 4
        /// </summary>
        private static readonly LootItem[] GetWand = new LootItem[]
                                                     {
                                                         new LootItem( typeof( ClumsyWand ), 1 ),          
                                                         new LootItem( typeof( FeebleWand ), 1 ),
                                                         new LootItem( typeof( FireballWand ), 1 ),
                                                         new LootItem( typeof( GreaterHealWand ), 1 ),
                                                         new LootItem( typeof( HarmWand ), 1 ),
                                                         new LootItem( typeof( HealWand ), 1 ),
                                                         new LootItem( typeof( IDWand ), 1 ),
                                                         new LootItem( typeof( LightningWand ), 1 ),
                                                         new LootItem( typeof( MagicArrowWand ), 1 ),
                                                         new LootItem( typeof( ManaDrainWand ), 1 ),
                                                         new LootItem( typeof( WeaknessWand ), 1 )
                                                     };

        /// <summary>
        /// 5112 1 // gnarled staff 1
        /// 5113 1 // gnarled staff 2
        /// </summary>
        private static readonly LootItem[] GetStaff = new LootItem[]
                                                      {
                                                          new LootItem( typeof( GnarledStaff ), 1 ),
                                                          new LootItem( typeof( GnarledStaff ), 1, 0x13f9 )
                                                      };

        /// <summary>
        /// 3629 1 // crystal ball 1
        /// </summary>
        public static readonly LootItem[] GetCrystalBall = new LootItem[]
                                                           {
                                                               new LootItem( typeof( MagicCrystalBall ), 1 )
                                                           };

        /// <summary>
        /// 3633 1 // brazier 1
        /// 3634 1 // brazier 2
        /// 3635 1 // brazier 3
        /// 4644 1 // statue 1
        /// 4645 1 // statue 2
        /// 4646 1 // statue 3
        /// 4647 1 // statue 4
        /// 4648 1 // statue 5
        /// </summary>
        public static readonly LootItem[] GetStationary = new LootItem[]
                                                          {
                                                              new LootItem( typeof( MagicBrazier ), 1 ),
                                                              new LootItem( typeof( MagicBrazier ), 1, 3634 ),
                                                              new LootItem( typeof( MagicBrazier ), 1, 3635 ),
                                                              new LootItem( typeof( MagicStatue ), 1 ),
                                                              new LootItem( typeof( MagicStatue ), 1, 4645 ),
                                                              new LootItem( typeof( MagicStatue ), 1, 4646 ),
                                                              new LootItem( typeof( MagicStatue ), 1, 4647 ),
                                                              new LootItem( typeof( MagicStatue ), 1, 4648 )
                                                          };

        /// <summary>
        /// 3962 1 // BlackPearl	
        /// 3963 1 // BloodMoss	
        /// 3972 1 // Garlic		
        /// 3973 1 // Ginseng		
        /// 3974 1 // Mandrake		
        /// 3976 1 // Nightshade	
        /// 3980 1 // SulfurAsh	
        /// 3981 1 // SpiderSilk
        /// </summary>
        public static readonly LootItem[] GetReagent = new LootItem[]
                                                       {
                                                           new LootItem( typeof( BlackPearl ), 1 ),
                                                           new LootItem( typeof( Bloodmoss ), 1 ),
                                                           new LootItem( typeof( Garlic ), 1 ),
                                                           new LootItem( typeof( Ginseng ), 1 ),
                                                           new LootItem( typeof( MandrakeRoot ), 1 ),
                                                           new LootItem( typeof( Nightshade ), 1 ),
                                                           new LootItem( typeof( SulfurousAsh ), 1 ),
                                                           new LootItem( typeof( SpidersSilk ), 1 )
                                                       };
        #endregion

        /// <summary>
        /// { wands 2 staves 1 };
        /// </summary>
        public static readonly LootTable GetTargetable = new LootTable( new TableEntry[]
                                                                        {
                                                                            new ListEntry( GetWand, 2 ),
                                                                            new ListEntry( GetStaff, 1 )
                                                                        } );

        /// <summary>
        /// { "crystal balls" 1 "stationary" 1	};
        /// </summary>
        public static readonly LootTable GetActivator = new LootTable( new TableEntry[]
                                                                       {
                                                                           new ListEntry( GetCrystalBall, 1 ),
                                                                           new ListEntry( GetStationary, 1 )
                                                                       } );

        /// <summary>
        /// { reagents 2 scroll 3 "potions" 3 wearables 1 "targetables" 1  "activators" 1 }
        /// </summary>
        public static readonly LootTable MagicRelatedItem = new LootTable( new TableEntry[]
                                                                           {
                                                                               new ListEntry( GetReagent, 2 ),
                                                                               new ListEntry( GetScroll, 3 ),
                                                                               new ListEntry( GetPotion, 3 ),
                                                                               new TableEntry( OldWearableLoot.Wearable, 1 ),
                                                                               new TableEntry( GetTargetable, 1 ),
                                                                               new TableEntry( GetActivator, 1 )
                                                                           } );

        public static readonly LootTable Scrolls = new LootTable( new ListEntry[]
                                                                  {
                                                                      new ListEntry(GetScroll, 1)
                                                                  } );

        public static readonly LootTable Reagents = new LootTable( new ListEntry[]
                                                                   {
                                                                       new ListEntry(GetReagent, 1)
                                                                   } );
    }
}