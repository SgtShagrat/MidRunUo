/***************************************************************************
 *                                  BodHelper.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Engines.BulkOrders;
using Server.Engines.Craft;
using Server.Commands;
using Server.Targeting;
using Server.Mobiles;
using System.IO;

namespace Midgard.Engines.Craft
{
    public static class BodHelper
    {
        public static void Initialize()
        {
            if( Misc.Midgard2Persistance.SmithBulksEnabled )
                CommandSystem.Register( "ValutaBOD", AccessLevel.Player, new CommandEventHandler( Evaluate_OnCommand ) );
        }

        [Usage( "ValutaBOD" )]
        [Description( "valuta un bulk order e ne restituisce il valore in punti." )]
        private static void Evaluate_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Seleziona il BOD da valutare" );
            e.Mobile.Target = new EvaluateBODTarget( e.Mobile );
        }

        private class EvaluateBODTarget : Target
        {
            private Mobile m_From;

            public EvaluateBODTarget( Mobile from )
                : base( -1, false, TargetFlags.None )
            {
                m_From = from;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( !( from is Midgard2PlayerMobile ) || !( targeted is Item ) )
                    return;

                if( targeted is SmallSmithBOD || targeted is LargeSmithBOD || targeted is SmallTailorBOD || targeted is LargeTailorBOD )
                {
                    int points = GetBulkValue( (Item)targeted );
                    int lastSmith = Math.Max( ( (Midgard2PlayerMobile)from ).LastSmithBulkOrderValue, 0 );
                    int lastTailor = Math.Max( ( (Midgard2PlayerMobile)from ).LastTailorBulkOrderValue, 0 );

                    if( points > 0 )
                    {
                        // int timeValue = (int)( points * 0.48 );
                        from.SendMessage( "Quest ordine vale {0} punti.", points );
                        from.SendMessage( "Il tuo ultimo ordine da sarto valeva {0} punti.", lastTailor );
                        from.SendMessage( "Il tuo ultimo ordine da fabbro valeva {0} punti.", lastSmith );
                    }
                    else
                        from.SendMessage( "Non riesco a valutare questo ordine." );
                }
                else
                    from.SendMessage( "Non hai scelto un bulk order valido." );
            }
        }

        public static int GetBulkValue( Item item )
        {
            int points = 0;

            if( item is SmallSmithBOD )
                points = SmithRewardCalculator.Instance.ComputePoints( (SmallSmithBOD)item );
            else if( item is LargeSmithBOD )
                points = SmithRewardCalculator.Instance.ComputePoints( (LargeSmithBOD)item );
            else if( item is SmallTailorBOD )
                points = TailorRewardCalculator.Instance.ComputePoints( (SmallTailorBOD)item );
            else if( item is LargeTailorBOD )
                points = TailorRewardCalculator.Instance.ComputePoints( (LargeTailorBOD)item );

            return points;
        }

        public enum BodMode
        {
            StandardRunUO,
            Midgard
        }

        public static readonly int SmithPointsLimit = 300;

        public static readonly int TailorPointsLimit = 200;

        /// <summary>
        /// Se impostata su StandardRunUO sia gli small sia i large sono generati secondo lo standard RunUO
        /// </summary>
        public static readonly BodMode GeneralBodMode = BodMode.Midgard;

        public class MaterialEntry
        {
            private double m_ReqSkill, m_MinSkill, m_MaxSkill;
            private BulkMaterialType m_Material;

            public double MaxSkill
            {
                get { return m_MaxSkill; }
            }

            public double MinSkill
            {
                get { return m_MinSkill; }
            }

            public double ReqSkill
            {
                get { return m_ReqSkill; }
            }

            public BulkMaterialType Material
            {
                get { return m_Material; }
            }

            public MaterialEntry( double reqSkill, double minSkill, double maxSkill, BulkMaterialType material )
            {
                m_ReqSkill = reqSkill;
                m_MinSkill = minSkill;
                m_MaxSkill = maxSkill;
                m_Material = material;
            }
        }

        /// <summary>
        /// Gestisce la scelta del materiale del bulk order
        /// </summary>
        public static BulkMaterialType GetRandomMaterial( double craftSkill, double itemidSkill, double armsloreSkill, MaterialEntry[] table )
        {
            for( int i = table.Length - 1; i > -1; i-- )
            {
                MaterialEntry entry = table[ i ];

                if( craftSkill >= entry.ReqSkill )
                {
                    double chance = ( craftSkill - entry.MinSkill ) / ( entry.MaxSkill - entry.MinSkill );

                    if( itemidSkill >= 100.0 )
                        chance *= 1.1;
                    if( armsloreSkill >= 100.0 )
                        chance *= 1.1;

                    double success = Utility.RandomDouble() * 100.0;

                    if( chance > success )
                        return entry.Material;
                }
            }

            return BulkMaterialType.None;
        }

        #region material type tables
        private static MaterialEntry[] m_BlacksmithyMaterialTable = new MaterialEntry[]
		{
			new MaterialEntry( 60.0, 	40.0, 41.60, 	BulkMaterialType.DullCopper ),
			new MaterialEntry( 70.0, 	50.0, 52.33, 	BulkMaterialType.ShadowIron ),
			new MaterialEntry( 80.0, 	60.0, 63.00,	BulkMaterialType.Copper ),
			new MaterialEntry( 90.0, 	70.0, 73.33, 	BulkMaterialType.Bronze ),
			new MaterialEntry( 100.0, 	80.0, 83.33, 	BulkMaterialType.Gold ),
			new MaterialEntry( 105.0, 	85.0, 90.00, 	BulkMaterialType.Agapite ),
			new MaterialEntry( 110.0, 	90.0, 96.00, 	BulkMaterialType.Verite ),
			new MaterialEntry( 115.0, 	95.0, 103.33, 	BulkMaterialType.Valorite )
		};

        private static List<int[]> m_BlacksmithyQuantityList;

        private static void InitBlacksmithyQuantityList()
        {
            if( m_BlacksmithyQuantityList == null )
                m_BlacksmithyQuantityList = new List<int[]>();

            m_BlacksmithyQuantityList.Add( new int[] { 10, 15, 15, 15, 20, 20, 20, 20, 20, 20 } ); // black 120 + 200 punti tra srmslore e itemid
            m_BlacksmithyQuantityList.Add( new int[] { 10, 10, 15, 15, 15, 20, 20, 20, 20, 20 } ); // black 120 + 100 punti tra srmslore e itemid
            m_BlacksmithyQuantityList.Add( new int[] { 10, 10, 10, 15, 15, 15, 20, 20, 20, 20 } ); // black <= 120
            m_BlacksmithyQuantityList.Add( new int[] { 10, 10, 10, 10, 15, 15, 15, 20, 20, 20 } ); // black <= 100
            m_BlacksmithyQuantityList.Add( new int[] { 10, 10, 10, 10, 10, 15, 15, 15, 20, 20 } ); // black <= 80
            m_BlacksmithyQuantityList.Add( new int[] { 10, 10, 10, 10, 10, 15, 15, 15, 15, 20 } ); // black <= 60
            m_BlacksmithyQuantityList.Add( new int[] { 10, 10, 10, 10, 10, 10, 15, 15, 15, 15 } ); // black <= 40
            m_BlacksmithyQuantityList.Add( new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 } ); // black <= 20
        }

        private static List<int[]> m_TailoringQuantityList;

        private static void InitTailoringQuantityList()
        {
            if( m_TailoringQuantityList == null )
                m_TailoringQuantityList = new List<int[]>();

            m_TailoringQuantityList.Add( new int[] { 10, 15, 15, 15, 20, 20, 20, 20, 20, 20 } ); // tailor 120 + 200 punti tra srmslore e itemid
            m_TailoringQuantityList.Add( new int[] { 10, 10, 15, 15, 15, 20, 20, 20, 20, 20 } ); // tailor 120 + 100 punti tra srmslore e itemid
            m_TailoringQuantityList.Add( new int[] { 10, 10, 10, 15, 15, 15, 20, 20, 20, 20 } ); // tailor <= 120
            m_TailoringQuantityList.Add( new int[] { 10, 10, 10, 10, 15, 15, 15, 20, 20, 20 } ); // tailor <= 100
            m_TailoringQuantityList.Add( new int[] { 10, 10, 10, 10, 10, 15, 15, 15, 20, 20 } ); // tailor <= 80
            m_TailoringQuantityList.Add( new int[] { 10, 10, 10, 10, 10, 15, 15, 15, 15, 20 } ); // tailor <= 60
            m_TailoringQuantityList.Add( new int[] { 10, 10, 10, 10, 10, 10, 15, 15, 15, 15 } ); // tailor <= 40
            m_TailoringQuantityList.Add( new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 } ); // tailor <= 20
        }

        private static MaterialEntry[] m_TailoringMaterialTable = new MaterialEntry[]
		{
			new MaterialEntry( 70.0, 	50.0, 53.50, 	BulkMaterialType.Spined ),
			new MaterialEntry( 90.0, 	70.0, 73.33, 	BulkMaterialType.Horned ),
			new MaterialEntry( 110.0, 	90.0, 93.00, 	BulkMaterialType.Barbed ),
		};
        #endregion

        /// <summary>
        /// Crea uno small bulk order da sarto in base alle caratteristiche del mobile m
        /// </summary>
        public static SmallTailorBOD CreateRandomSmallTailorFor( Mobile m )
        {
            SmallBulkEntry[] entries;

            double armsLore = m.Skills[ SkillName.ArmsLore ].Base;
            double itemId = m.Skills[ SkillName.ItemID ].Base;
            double tailoring = m.Skills[ SkillName.Tailoring ].Base;

            double sumBulkSkills = tailoring + armsLore + itemId;

            #region leather or cloth
            double leatherChance = 0.0;
            double minTailoringSkillToGetLeather = 70.0;

            if( tailoring >= minTailoringSkillToGetLeather )
                leatherChance = sumBulkSkills / 320.0;

            bool useMaterials = ( leatherChance > Utility.RandomDouble() && tailoring >= 6.2 ); // the easiest leather BOD is Leather Cap which requires at least 6.2 skill.

            if( useMaterials )
                entries = SmallBulkEntry.TailorLeather;
            else
                entries = SmallBulkEntry.TailorCloth;
            #endregion

            if( entries.Length > 0 )
            {
                #region quantity
                int amountMax;

                if( m_TailoringQuantityList == null || m_TailoringQuantityList.Count == 0 )
                    InitTailoringQuantityList();

                int index = 7 - (int)( tailoring / 20.0 );
                if( itemId >= 100.0 )
                    index--;
                else if( armsLore >= 100.0 )
                    index--;

                if( index < 0 )
                    index = 0;
                else if( m_TailoringQuantityList != null )
                    if( index > m_TailoringQuantityList.Count - 1 )
                        index = m_TailoringQuantityList.Count - 1;

                if( m_TailoringQuantityList != null )
                    amountMax = Utility.RandomList( m_TailoringQuantityList[ index ] );
                else
                    amountMax = 10;

                #endregion

                #region material
                BulkMaterialType material = BulkMaterialType.None;

                if( useMaterials )
                    material = GetRandomMaterial( tailoring, itemId, armsLore, m_TailoringMaterialTable );
                #endregion

                #region reqExceptional and validation
                double excChance = 0.0;
                double minTailoringSkillToGetExcs = 70.1;

                if( tailoring >= minTailoringSkillToGetExcs )
                    excChance = sumBulkSkills / 320.0;

                bool reqExceptional = ( excChance > Utility.RandomDouble() );

                CraftSystem system = DefTailoring.CraftSystem;

                List<SmallBulkEntry> validEntries = new List<SmallBulkEntry>();

                for( int i = 0; i < entries.Length; ++i )
                {
                    CraftItem item = system.CraftItems.SearchFor( entries[ i ].Type );

                    if( item != null )
                    {
                        bool allRequiredSkills = true;
                        double chance = item.GetSuccessChance( m, null, system, false, ref allRequiredSkills );

                        if( allRequiredSkills && chance >= 0.0 )
                        {
                            if( reqExceptional )
                                chance = item.GetExceptionalChance( system, chance, m );

                            if( chance > 0.0 )
                                validEntries.Add( entries[ i ] );
                        }
                    }
                }
                #endregion

                if( validEntries.Count > 0 )
                {
                    SmallBulkEntry entry = validEntries[ Utility.Random( validEntries.Count ) ];
                    return new SmallTailorBOD( entry, material, amountMax, reqExceptional );
                }
            }

            return null;
        }

        /// <summary>
        /// Crea uno small bulk order da fabbro in base alle caratteristiche del mobile m
        /// </summary>
        public static SmallSmithBOD CreateRandomSmallBlackFor( Mobile m )
        {
            SmallBulkEntry[] entries;

            double armsLore = m.Skills[ SkillName.ArmsLore ].Base;
            double itemId = m.Skills[ SkillName.ItemID ].Base;
            double blacksmithy = m.Skills[ SkillName.Blacksmith ].Base;

            double sumBulkSkills = blacksmithy + armsLore + itemId;

            #region Armor o Weapons
            double armorChance = 0.0;
            double minBlackSkillToGetArmors = 70.0;

            if( blacksmithy >= minBlackSkillToGetArmors )
                armorChance = sumBulkSkills / 320.0;

            bool useMaterials = ( armorChance > Utility.RandomDouble() );

            if( useMaterials )
                entries = SmallBulkEntry.BlacksmithArmor;
            else
                entries = SmallBulkEntry.BlacksmithWeapons;
            #endregion

            if( entries.Length > 0 )
            {
                #region quantity
                int amountMax;

                if( m_BlacksmithyQuantityList == null || m_BlacksmithyQuantityList.Count == 0 )
                    InitBlacksmithyQuantityList();

                int index = 7 - (int)( blacksmithy / 20.0 );
                if( itemId >= 100.0 )
                    index--;
                else if( armsLore >= 100.0 )
                    index--;

                if( index < 0 )
                    index = 0;
                else if( m_BlacksmithyQuantityList != null )
                    if( index > m_BlacksmithyQuantityList.Count - 1 )
                        index = m_BlacksmithyQuantityList.Count - 1;

                if( m_BlacksmithyQuantityList != null )
                    amountMax = Utility.RandomList( m_BlacksmithyQuantityList[ index ] );
                else
                    amountMax = 10;
                #endregion

                #region material
                BulkMaterialType material = BulkMaterialType.None;

                if( useMaterials )
                    material = GetRandomMaterial( blacksmithy, itemId, armsLore, m_BlacksmithyMaterialTable );
                #endregion

                #region reqExceptional and validation
                double excChance = 0.0;
                double minBlackSkillToGetExcs = 70.1;

                if( blacksmithy >= minBlackSkillToGetExcs )
                    excChance = sumBulkSkills / 320.0;

                bool reqExceptional = ( excChance > Utility.RandomDouble() );

                CraftSystem system = DefBlacksmithy.CraftSystem;

                List<SmallBulkEntry> validEntries = new List<SmallBulkEntry>();

                for( int i = 0; i < entries.Length; ++i )
                {
                    CraftItem item = system.CraftItems.SearchFor( entries[ i ].Type );

                    if( item != null )
                    {
                        bool allRequiredSkills = true;
                        double chance = item.GetSuccessChance( m, null, system, false, ref allRequiredSkills );

                        if( allRequiredSkills && chance >= 0.0 )
                        {
                            if( reqExceptional )
                                chance = item.GetExceptionalChance( system, chance, m );

                            if( chance > 0.0 )
                                validEntries.Add( entries[ i ] );
                        }
                    }
                }
                #endregion

                if( validEntries.Count > 0 )
                {
                    SmallBulkEntry entry = validEntries[ Utility.Random( validEntries.Count ) ];
                    return new SmallSmithBOD( entry, material, amountMax, reqExceptional );
                }
            }

            return null;
        }

        /// <summary>
        /// Crea uno large bulk order da fabbro in base alle caratteristiche del mobile m
        /// </summary>
        public static LargeSmithBOD CreateRandomLargeSmithBodFor( Mobile m )
        {
            bool shouldUseNormalMethod = false;

            if( GeneralBodMode == BodMode.StandardRunUO )
                shouldUseNormalMethod = true;
            else if( m.Skills[ SkillName.ArmsLore ].Value < 30 || m.Skills[ SkillName.ItemID ].Value < 30 )
                shouldUseNormalMethod = true;
            else if( m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).LastSmithBulkOrderValue < SmithPointsLimit )
                shouldUseNormalMethod = true;

            if( shouldUseNormalMethod )
                return new LargeSmithBOD();

            LargeBulkEntry[] entries;

            double armsLore = m.Skills[ SkillName.ArmsLore ].Base;
            double itemId = m.Skills[ SkillName.ItemID ].Base;
            double blacksmithy = m.Skills[ SkillName.Blacksmith ].Base;

            double sumBulkSkills = blacksmithy + armsLore + itemId;

            LargeSmithBOD bod = new LargeSmithBOD();

            #region Armor o Weapons
            double armorChance = 0.0;
            double minBlackSkillToGetArmors = 70.0;

            if( blacksmithy >= minBlackSkillToGetArmors )
                armorChance = sumBulkSkills / 320.0;

            bool useMaterials = ( armorChance > Utility.RandomDouble() );

            if( useMaterials )
            {
                switch( Utility.Random( 3 ) )
                {
                    default:
                    case 0: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.LargeRing ); break;
                    case 1: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.LargePlate ); break;
                    case 2: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.LargeChain ); break;
                }
            }
            else
            {
                switch( Utility.Random( 5 ) )
                {
                    default:
                    case 0: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.LargeAxes ); break;
                    case 1: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.LargeFencing ); break;
                    case 2: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.LargeMaces ); break;
                    case 3: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.LargePolearms ); break;
                    case 4: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.LargeSwords ); break;
                }
            }
            #endregion

            #region quantity
            int amountMax = 10;

            if( m_BlacksmithyQuantityList == null || m_BlacksmithyQuantityList.Count == 0 )
                InitBlacksmithyQuantityList();

            int index = 7 - (int)( blacksmithy / 20.0 );
            if( itemId >= 100.0 )
                index--;
            else if( armsLore >= 100.0 )
                index--;

            if( index < 0 )
                index = 0;
            else if( index > m_BlacksmithyQuantityList.Count - 1 )
                index = m_BlacksmithyQuantityList.Count - 1;

            amountMax = Utility.RandomList( m_BlacksmithyQuantityList[ index ] );
            #endregion

            #region material
            BulkMaterialType material = BulkMaterialType.None;

            if( useMaterials )
                material = GetRandomMaterial( blacksmithy, itemId, armsLore, m_BlacksmithyMaterialTable );
            #endregion

            #region reqExceptional
            double excChance = 0.0;
            double minBlackSkillToGetExcs = 70.1;

            if( blacksmithy >= minBlackSkillToGetExcs )
                excChance = sumBulkSkills / 320.0;

            bool reqExceptional = ( excChance > Utility.RandomDouble() );
            #endregion

            int hue = 0x44E;

            bod.Hue = hue;
            bod.AmountMax = amountMax;
            bod.Entries = entries;
            bod.RequireExceptional = reqExceptional;
            bod.Material = material;

            return bod;
        }

        /// <summary>
        /// Crea uno large bulk order da sarto in base alle caratteristiche del mobile m
        /// </summary>
        public static LargeTailorBOD CreateRandomLargeTailorBodFor( Mobile m )
        {
            bool shouldUseNormalMethod = false;

            if( GeneralBodMode == BodMode.StandardRunUO )
                shouldUseNormalMethod = true;
            else if( m.Skills[ SkillName.ArmsLore ].Value < 30 || m.Skills[ SkillName.ItemID ].Value < 30 )
                shouldUseNormalMethod = true;
            else if( m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).LastTailorBulkOrderValue < TailorPointsLimit )
                shouldUseNormalMethod = true;

            if( shouldUseNormalMethod )
                return new LargeTailorBOD();

            LargeBulkEntry[] entries;

            double armsLore = m.Skills[ SkillName.ArmsLore ].Base;
            double itemId = m.Skills[ SkillName.ItemID ].Base;
            double tailoring = m.Skills[ SkillName.Tailoring ].Base;

            double sumBulkSkills = tailoring + armsLore + itemId;

            LargeTailorBOD bod = new LargeTailorBOD();

            #region leather or cloth
            double leatherChance = 0.0;
            double minTailoringSkillToGetLeather = 70.0;

            if( tailoring >= minTailoringSkillToGetLeather )
                leatherChance = sumBulkSkills / 320.0;

            bool useMaterials = ( leatherChance > Utility.RandomDouble() && tailoring >= 6.2 );

            if( useMaterials )
            {
                switch( Utility.Random( 4 ) )
                {
                    default:
                    case 0: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.FemaleLeatherSet ); break;
                    case 1: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.MaleLeatherSet ); break;
                    case 2: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.StuddedSet ); break;
                    case 3: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.BoneSet ); break;
                }
            }
            else
            {
                switch( Utility.Random( 10 ) )
                {
                    default:
                    case 0: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.Farmer ); break;
                    case 1: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.FisherGirl ); break;
                    case 2: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.Gypsy ); break;
                    case 3: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.HatSet ); break;
                    case 4: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.Jester ); break;
                    case 5: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.Lady ); break;
                    case 6: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.Pirate ); break;
                    case 7: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.ShoeSet ); break;
                    case 8: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.TownCrier ); break;
                    case 9: entries = LargeBulkEntry.ConvertEntries( bod, LargeBulkEntry.Wizard ); break;
                }
            }
            #endregion

            #region quantity
            if( m_TailoringQuantityList == null || m_TailoringQuantityList.Count == 0 )
                InitTailoringQuantityList();

            int index = 7 - (int)( tailoring / 20.0 );
            if( itemId >= 100.0 )
                index--;
            else if( armsLore >= 100.0 )
                index--;

            if( index < 0 )
                index = 0;
            else if( index > m_TailoringQuantityList.Count - 1 )
                index = m_TailoringQuantityList.Count - 1;

            int amountMax = Utility.RandomList( m_TailoringQuantityList[ index ] );
            #endregion

            #region material
            BulkMaterialType material = BulkMaterialType.None;

            if( useMaterials )
                material = GetRandomMaterial( tailoring, itemId, armsLore, m_TailoringMaterialTable );
            #endregion

            #region reqExceptional
            double excChance = 0.0;
            double minTailoringSkillToGetExcs = 70.1;

            if( tailoring >= minTailoringSkillToGetExcs )
                excChance = sumBulkSkills / 320.0;

            bool reqExceptional = ( excChance > Utility.RandomDouble() );
            #endregion

            int hue = 0x483;

            bod.Hue = hue;
            bod.AmountMax = amountMax;
            bod.Entries = entries;
            bod.RequireExceptional = reqExceptional;
            bod.Material = material;

            return bod;
        }

        public static TimeSpan ScaleTime( TimeSpan oldDelay, LargeBOD bod )
        {
            return TimeSpan.Zero;
        }

        public static TimeSpan ScaleTime( TimeSpan oldDelay, SmallBOD bod )
        {
            int points = 0;

            if( bod is SmallSmithBOD )
                points = SmithRewardCalculator.Instance.ComputePoints( bod );
            else if( bod is SmallTailorBOD )
                points = TailorRewardCalculator.Instance.ComputePoints( bod );

            TimeSpan timeValue = TimeSpan.FromMinutes( points * 0.48 );

            if( oldDelay < timeValue )
                return TimeSpan.Zero;
            else
                return ( oldDelay - timeValue );
        }

        public static readonly string BulkPreRequestLog = "Logs/BulkPreRequest.log";

        public static void LogBulkPreRequest( Mobile m, bool isBlack )
        {
            try
            {
                string toLog = String.Format( "Mobile {0} (account {1}) - black {2} - itemid {3} - armslore {4} - " +
                                              "lastSmithPoints {5} - lastTailorPonts {6} - bulkRequested {7} - DateTime {8}",
                                             m.Name, m.Account,
                                             m.Skills[ SkillName.Blacksmith ].Base.ToString( "F2" ),
                                             m.Skills[ SkillName.ItemID ].Base.ToString( "F2" ),
                                             m.Skills[ SkillName.ArmsLore ].Base.ToString( "F2" ),
                                             ( (Midgard2PlayerMobile)m ).LastSmithBulkOrderValue,
                                             ( (Midgard2PlayerMobile)m ).LastTailorBulkOrderValue,
                                             isBlack ? "black" : "tailor",
                                             DateTime.Now );

                TextWriter tw = File.AppendText( BulkPreRequestLog );
                tw.WriteLine( toLog );
                tw.Close();
            }
            catch { }
        }

        public static readonly string BulkBulksGivenLog = "Logs/BulksGiven.log";

        public static void LogBulkGiven( Mobile m, SmallBOD bod )
        {
            bool isBlack = ( bod is SmallSmithBOD );
            int points = GetBulkValue( bod );

            try
            {
                string toLog = String.Format( "Mobile {0} (account {1}) - black {2} - itemid {3} - armslore {4} - " +
                                              "lastSmithPoints {5} - lastTailorPonts {6} - bulkRequested {7} - Points {8} - DateTime {9}",
                                             m.Name, m.Account,
                                             m.Skills[ SkillName.Blacksmith ].Base.ToString( "F2" ),
                                             m.Skills[ SkillName.ItemID ].Base.ToString( "F2" ),
                                             m.Skills[ SkillName.ArmsLore ].Base.ToString( "F2" ),
                                             ( (Midgard2PlayerMobile)m ).LastSmithBulkOrderValue,
                                             ( (Midgard2PlayerMobile)m ).LastTailorBulkOrderValue,
                                             isBlack ? "black" : "tailor",
                                             points,
                                             DateTime.Now );

                TextWriter tw = File.AppendText( BulkBulksGivenLog );
                tw.WriteLine( toLog );
                tw.Close();
            }
            catch { }
        }

        public static bool IsValidMidgardCandidate( Mobile m )
        {
            if( GeneralBodMode == BodMode.StandardRunUO )
                return false;
            else if( m.Skills[ SkillName.ArmsLore ].Value < 60 || m.Skills[ SkillName.ItemID ].Value < 60 )
                return false;

            return true;
        }
    }
}