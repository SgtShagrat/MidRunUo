/***************************************************************************
 *                               OldWeaponsLoot.cs
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
    public static class OldWeaponsLoot
    {
        #region [item list]

        /// <summary>
        /// 3919 40 // crossbow
        /// 5117 20 // big crossbow
        /// 5042 40 // bow 
        /// </summary>
        private static readonly LootItem[] GetRangedWeapon = new LootItem[]
                                                             {
                                                                 new LootItem( typeof(Crossbow), 40 ),
                                                                 new LootItem( typeof(HeavyCrossbow), 20 ),
                                                                 new LootItem( typeof(Bow), 40 )
                                                             };

        /// <summary>
        /// 3718 5 // pickaxe 
        /// 3719 1 // pitchfork 
        /// 3721 5 // quarterstaff 
        /// 3779 1 // meat cleaver
        /// 3780 1 // skinning knife
        /// 5109 2 // shepherd's crook
        /// 5110 1 // butcher knife
        /// </summary>
        private static readonly LootItem[] EveryDayWeapon = new LootItem[]
                                                            {
                                                                new LootItem( typeof(Pickaxe), 5 ),
                                                                new LootItem( typeof(Pitchfork), 1 ),
                                                                new LootItem( typeof(QuarterStaff), 5 ),
                                                                new LootItem( typeof(Cleaver), 1 ),
                                                                new LootItem( typeof(SkinningKnife), 1 ),
                                                                new LootItem( typeof(ShepherdsCrook), 2 ),
                                                                new LootItem( typeof(ButcherKnife), 1 ),
                                                            };

        /// <summary>
        /// 15 // quarterstaff
        /// 20 // club
        /// </summary>
        private static readonly LootItem[] GetCudgelWeapon = new LootItem[]
                                                             {
                                                                 new LootItem( typeof( QuarterStaff ), 15 ),
                                                                 new LootItem( typeof( Club ), 20 ),
                                                             };

        /// <summary>
        /// 70 // mace
        /// 25 // war mace 
        /// 5  // maul
        /// </summary>
        private static readonly LootItem[] GetMaceWeapon = new LootItem[]
                                                           {
                                                               new LootItem( typeof( Mace ), 70 ),
                                                               new LootItem( typeof( WarMace ), 25 ),
                                                               new LootItem( typeof( Maul ), 5 )
                                                           };

        /// <summary>
        /// 80 // war hammer 
        /// 20 // hammerpick
        /// </summary>
        private static readonly LootItem[] GetHammerWeapon = new LootItem[]
                                                             {
                                                                 new LootItem( typeof( WarHammer ), 80 ),
                                                                 new LootItem( typeof( HammerPick ), 20 )
                                                             };

        /// <summary>
        /// 3907 10 // double-bitted battle axe
        /// 3911 30 // battle axe
        /// 3913 40 // new axe
        /// 3915 15 // double-bitted new axe
        /// 5040 5  // war axe
        /// </summary>
        private static readonly LootItem[] GetAxeWeapon = new LootItem[]
                                                          {
                                                              new LootItem( typeof( WarAxe ), 10 ),
                                                              new LootItem( typeof( WarAxe ), 30 ),
                                                              new LootItem( typeof( WarAxe ), 40 ),
                                                              new LootItem( typeof( WarAxe ), 15 ),
                                                              new LootItem( typeof( WarAxe ), 5 )
                                                          };

        /// <summary>
        /// 3934 20 // broad sword
        /// 3937 20 // long sword
        /// 5046 20 // scimitar
        /// 5048 5  // magic sword
        /// 5049 15 // viking sword
        /// 5118 5 // katana
        /// 5119 5 // katana2
        /// 5185 10 // cutlass
        /// </summary>
        private static readonly LootItem[] GetSwordWeapon = new LootItem[]
                                                            {
                                                                new LootItem( typeof( Broadsword ), 20 ),
                                                                new LootItem( typeof( Longsword ), 20 ),
                                                                new LootItem( typeof( Scimitar ), 20 ),
                                                                new LootItem( typeof( Longsword ), 5 ),
                                                                new LootItem( typeof( VikingSword ), 15 ),
                                                                new LootItem( typeof( Katana ), 5 ),
                                                                new LootItem( typeof( Katana ), 5, 0x13ff ),
                                                                new LootItem( typeof( Cutlass ), 10 )
                                                            };

        /// <summary>
        /// 3917 10 // bardiche
        /// 3938 40 // spear
        /// 5123 20 // short spear
        /// 5115 10 // 2 handed axe/pole axe
        /// 5125 5  // war fork
        /// 5183 15 // halberd
        /// </summary>
        private static readonly LootItem[] GetPoleArmsWeapon = new LootItem[]
                                                               {
                                                                   new LootItem( typeof( Bardiche ), 10 ),
                                                                   new LootItem( typeof( Spear ), 40 ), 
                                                                   new LootItem( typeof( ShortSpear ), 20 ),
                                                                   new LootItem( typeof( TwoHandedAxe ), 10 ),
                                                                   new LootItem( typeof( WarFork ), 5 ),
                                                                   new LootItem( typeof( Halberd ), 15 ),
                                                               };

        /// <summary>
        /// 3920 40 // dagger 1
        /// 3921 40 // dagger 2
        /// 5121 20 // kryss
        /// </summary>
        private static readonly LootItem[] GetDaggerWeapon = new LootItem[]
                                                             {
                                                                 new LootItem( typeof( Dagger ), 40 ),
                                                                 new LootItem( typeof( Dagger ), 40, 0xf51 ),
                                                                 new LootItem( typeof( Kryss ), 20 )
                                                             };
        #endregion
        /// <summary>
        /// { cudgels 1 maces 1 hammers 1 };
        /// </summary>
        private static readonly LootTable GetBluntWeapon = new LootTable( new TableEntry[]
                                                                          {
                                                                              new ListEntry( GetCudgelWeapon, 1 ),
                                                                              new ListEntry( GetMaceWeapon, 1 ),
                                                                              new ListEntry( GetHammerWeapon, 1 )
                                                                          } );

        /// <summary>
        /// { axes 1 swords 1 pole_arms 1 daggers 1 };
        /// </summary>
        private static readonly LootTable GetEdgedWeapon = new LootTable( new TableEntry[]
                                                                          {
                                                                              new ListEntry( GetAxeWeapon, 1 ),
                                                                              new ListEntry( GetSwordWeapon, 1 ),
                                                                              new ListEntry( GetPoleArmsWeapon, 1 ),
                                                                              new ListEntry( GetDaggerWeapon, 1 )
                                                                          } );
        /// <summary>
        /// { "blunt weapons" 1 "edged weapons" 1 "ranged weapons" 1 };
        /// </summary>
        private static readonly LootTable RealWeapon = new LootTable( new TableEntry[]
                                                                      {
                                                                          new TableEntry( GetBluntWeapon, 1 ),
                                                                          new TableEntry( GetEdgedWeapon, 1 ),
                                                                          new ListEntry( GetRangedWeapon, 1 )
                                                                      } );

        /// <summary>
        /// { "everyday weapons" 1 "real weapons" 5 };
        /// </summary>
        public static readonly LootTable Weapons = new LootTable( new TableEntry[]
                                                                  {
                                                                      new ListEntry( EveryDayWeapon, 1 ),
                                                                      new TableEntry( RealWeapon, 5 )
                                                                  } );

        public static readonly LootTable RangedWeapon = new LootTable( GetRangedWeapon );
    }
}