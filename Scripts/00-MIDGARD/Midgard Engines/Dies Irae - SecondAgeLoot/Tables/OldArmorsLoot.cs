/***************************************************************************
 *                               OldArmorsLoot.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server.Items;

namespace Midgard.Engines.SecondAgeLoot
{
    public static class OldArmorsLoot
    {
        #region [item list]

        /// <summary>
        /// 7034 30 // wooden shield 
        /// 7027 20 // buckler
        /// 7032 20 // kite
        /// 7028 30 // heater shield
        /// </summary>
        private static readonly LootItem[] GetShield = new LootItem[]
                                                       {
                                                           new LootItem( typeof(WoodenShield), 30 ),
                                                           new LootItem( typeof(Buckler), 20 ),
                                                           new LootItem( typeof(MetalKiteShield), 20 ),
                                                           new LootItem( typeof(HeaterShield), 30 ),
                                                       };

        /// <summary>
        /// 5055 10 // chain tunic 
        /// 5060 10 // chain tunic  
        /// 5068 30 // leather   
        /// 5083 20 // studded leather   
        /// 5100 15 // ring  
        /// 5133 40 // padded tunic  
        /// 5141 5  // plate   
        /// 5199 1  // bone   
        /// 7170 8  // Studded Xena Armor
        /// 7171 8  // Mirror Studded Xena Armor
        /// 7172 3  // Plate Xena Armor
        /// 7173 2  //  Mirror Plate Xena Armor
        /// 7174 9  // Leather Xena Armor
        /// 7175 9  // Mirror Leather Xena Armor
        /// </summary>
        private static readonly LootItem[] GetChestArmor = new LootItem[]
                                                           {
                                                               new LootItem( typeof(ChainChest), 10 ),
                                                               new LootItem( typeof(ChainChest), 10 ),
                                                               new LootItem( typeof(LeatherChest), 30 ),
                                                               new LootItem( typeof(StuddedChest), 20 ),
                                                               new LootItem( typeof(RingmailChest), 15 ),
                                                               new LootItem( typeof(RingmailChest), 40 ),
                                                               new LootItem( typeof(PlateChest), 5 ),
                                                               new LootItem( typeof(BoneChest), 1 ),
                                                               new LootItem( typeof(FemaleStuddedChest), 8 ),
                                                               new LootItem( typeof(FemaleStuddedChest), 8 ),
                                                               new LootItem( typeof(FemalePlateChest), 3 ),
                                                               new LootItem( typeof(FemalePlateChest), 2 ),
                                                               new LootItem( typeof(FemaleLeatherChest), 9 ),
                                                               new LootItem( typeof(FemaleLeatherChest), 9 )
                                                           };

        /// <summary>
        /// 5054 10 // chain   
        /// 5067 30 // leather   
        /// 5082 20 // studded leather   
        /// 5104 15 // ring  
        /// 5131 40 // padded   
        /// 5137 5  // plate   
        /// 5202 1  // bone
        /// 7168 5  // Xena Leather shorts
        /// 7176 5  // Xena Leather skirts
        /// 7178 5  // Xena Leather bustier
        /// 7180 5  // Xena Studded bustier
        /// </summary>
        private static readonly LootItem[] GetLegArmor = new LootItem[]
                                                         {
                                                             new LootItem( typeof(ChainLegs), 10 ),
                                                             new LootItem( typeof(LeatherLegs), 30 ),
                                                             new LootItem( typeof(StuddedLegs), 20 ),
                                                             new LootItem( typeof(RingmailLegs), 15 ),
                                                             new LootItem( typeof(RingmailLegs), 40 ),
                                                             new LootItem( typeof(PlateLegs), 5 ),
                                                             new LootItem( typeof(BoneLegs), 1 ),
                                                             new LootItem( typeof(LeatherShorts), 5 ),
                                                             new LootItem( typeof(LeatherSkirt), 5 ),
                                                             new LootItem( typeof(LeatherBustierArms), 5 ),   
                                                             new LootItem( typeof(StuddedBustierArms), 5 ),
                                                         };

        /// <summary>
        /// 5061 30 // leather 
        /// 5076 20 // studded leather 
        /// 5102 15 // ring 
        /// 5143 5  // plate 
        /// 5198 1  // bone
        /// </summary>
        private static readonly LootItem[] GetArmArmor = new LootItem[]
                                                         {
                                                             new LootItem( typeof(LeatherArms), 30 ),
                                                             new LootItem( typeof(StuddedArms), 20 ),
                                                             new LootItem( typeof(RingmailArms), 15 ),
                                                             new LootItem( typeof(PlateArms), 5 ),
                                                             new LootItem( typeof(BoneArms), 1 ),
                                                         };

        /// <summary>
        /// 5063 30 // leather 
        /// 5078 20 // studded leather 
        /// 5139 10 // plate 
        /// 5078 20 // gorget
        /// </summary>
        private static readonly LootItem[] GetNeckArmor = new LootItem[]
                                                          {
                                                              new LootItem( typeof(LeatherGorget), 30 ),
                                                              new LootItem( typeof(StuddedGorget), 20 ),
                                                              new LootItem( typeof(PlateGorget), 10 ),
                                                              new LootItem( typeof(StuddedGorget), 20 ),
                                                          };



        /// <summary>
        /// // 3731 40 // helmet 
        /// 5145 30 // bucket helm 
        /// 5201 10 // bone 
        /// 5138 20 // plate
        /// </summary>
        private static readonly LootItem[] GetHelmArmor = new LootItem[]
                                                          {
                                                              new LootItem( typeof(CloseHelm), 30 ),
                                                              new LootItem( typeof(BoneHelm), 10 ),
                                                              new LootItem( typeof(PlateHelm), 20 ),
                                                          };

        /// <summary>
        /// 5051 40 // chain 
        /// 5129 60 // padded cap
        /// </summary>
        private static readonly LootItem[] GetCoifArmor = new LootItem[]
                                                          {
                                                              new LootItem( typeof(ChainCoif), 40 ),
                                                              new LootItem( typeof(LeatherCap), 60 ),
                                                          };

        /// <summary>
        /// { helms 1 coifs 1 };
        /// </summary>
        private static readonly LootTable GetHeadArmor = new LootTable( new TableEntry[]
                                                                        {
                                                                            new ListEntry( GetHelmArmor, 1 ),
                                                                            new ListEntry( GetCoifArmor, 1 )
                                                                        } );

        /// <summary>
        /// 5062 50 // leather 
        /// 5077 30 // studded leather 
        /// 5099 10 // ring 
        /// 5140 7 // plate 
        /// 5205 3 // bone 
        /// </summary>
        private static readonly LootItem[] GetHandArmor = new LootItem[]
                                                          {
                                                              new LootItem( typeof(LeatherGloves), 50 ),
                                                              new LootItem( typeof(StuddedGloves), 30 ),
                                                              new LootItem( typeof(RingmailGloves), 10 ),
                                                              new LootItem( typeof(PlateGloves), 7 ),
                                                              new LootItem( typeof(BoneGloves), 3 ),
                                                          };

        #endregion
 
        /// <summary>
        /// "chest armor" 1 
        /// "leg armor" 1 
        /// "arm armor" 1 
        /// "neck armor" 1 
        /// "head armor" 1 
        /// "hand armor" 1
        /// </summary>
        public static readonly LootTable GetBodyArmor = new LootTable( new TableEntry[]
                                                                       {
                                                                           new ListEntry( GetChestArmor, 1 ),
                                                                           new ListEntry( GetLegArmor, 1 ),
                                                                           new ListEntry( GetArmArmor, 1 ),
                                                                           new ListEntry( GetNeckArmor, 1 ),
                                                                           new TableEntry( GetHeadArmor, 1 ),
                                                                           new ListEntry( GetHandArmor, 1 ),
                                                                       } );

        /// <summary>
        /// { shields 1 };
        /// </summary>
        public static readonly LootTable GetExtraArmor = new LootTable( new TableEntry[]
                                                                        {
                                                                            new ListEntry( GetShield, 1 ),
                                                                        } );

        /// <summary>
        /// { "body armor" 1	"extra armor" 1 };
        /// </summary>
        public static readonly LootTable Armors = new LootTable( new TableEntry[]
                                                                 {
                                                                     new TableEntry( GetBodyArmor, 1 ),
                                                                     new TableEntry( GetExtraArmor, 1 )
                                                                 } );
    }
}