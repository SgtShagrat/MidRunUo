using System;
using Midgard.Engines.OldCraftSystem;
using Midgard.Items;

using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Engines.Craft
{
    public class DefWaxCrafting : CraftSystem
    {
        public override string Name { get{ return "Wax Crafting"; } }

        public override bool SupportOldMenu { get { return true; } }

        public override CraftDefinitionTree DefinitionTree
        {
            get
            {
                if( m_CraftDefinitionTree == null )
                    m_CraftDefinitionTree = new CraftDefinitionTree( "WaxCrafting.xml", CraftSystem );

                return m_CraftDefinitionTree;
            }
        }

        private static CraftDefinitionTree m_CraftDefinitionTree;

        public override SkillName MainSkill
        {
            get { return SkillName.Alchemy; }
        }

        public override int GumpTitleNumber
        {
            get { return 1065280; } // <CENTER>Midgard Advanced Wax Crafting</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if( m_CraftSystem == null )
                    m_CraftSystem = new DefWaxCrafting();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA { get { return Core.AOS ? CraftECA.ChanceMinusSixtyToFourtyFive : CraftECA.ZeroToFourPercentPlusBonus; } }

        public override double GetChanceAtMin( CraftItem item )
        {
            return 0.0;
        }

        private DefWaxCrafting()
            : base( 2, 4, 1.25 ) // mod by Dies Irae 
        {
        }

        public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
        {
            if( tool.Deleted || tool.UsesRemaining < 0 )
                return 1044038; // You have worn out your tool!
            else if( !BaseTool.CheckAccessible( tool, from ) )
                return 1044263; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect( Mobile from )
        {
            from.PlaySound( 0x242 );
        }

        public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
        {
            if( toolBroken )
                from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

            if( failed )
            {
                if( lostMaterial )
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }
            else
            {
                if( quality == 0 )
                    return 502785; // You were barely able to make this item.  It's quality is below average.
                else if( makersMark && quality == 2 )
                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                else if( quality == 2 )
                    return 1044155; // You create an exceptional quality item.
                else
                    return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            #region Resources
            index = AddCraft( typeof( CandleWick ), 1065281, 1065282, 50.0, 80.0, typeof( Beeswax ), 1065310, 1, 1065311 );
            AddRes( index, typeof( Cloth ), 1065312, 1, 1065313 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( BlankCandle ), 1065281, 1065283, 50.0, 80.0, typeof( Beeswax ), 1065310, 2, 1065311 );
            SetNeedHeat( index, true );
            #endregion

            #region Candles
            index = AddCraft( typeof( CandleShort ), 1065290, 1065291, 80.0, 105.0, typeof( BlankCandle ), 1065314, 1, 1065315 );
            AddRes( index, typeof( CandleWick ), 1065316, 1, 1065317 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( CandleShortColor ), 1065290, 1065292, 80.0, 105.0, typeof( BlankCandle ), 1065314, 1, 1065315 );
            AddRes( index, typeof( Dyes ), 1065318, 1, 1065319 );
            AddRes( index, typeof( CandleWick ), 1065316, 1, 1065317 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( CandleLong ), 1065290, 1065293, 80.0, 110.0, typeof( BlankCandle ), 1065314, 1, 1065315 );
            AddRes( index, typeof( CandleWick ), 1065316, 1, 1065317 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( CandleLongColor ), 1065290, 1065294, 80.0, 110.0, typeof( BlankCandle ), 1065314, 1, 1065315 );
            AddRes( index, typeof( Dyes ), 1065318, 1, 1065319 );
            AddRes( index, typeof( CandleWick ), 1065316, 1, 1065317 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( CandleSkull ), 1065290, 1065295, 100.0, 100.0, typeof( BlankCandle ), 1065314, 1, 1065315 );
            AddRes( index, typeof( CandleWick ), 1065316, 1, 1065317 );
            AddRes( index, typeof( CandleFitSkull ), 1065320, 1, 1065321 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( CandleOfLove ), 1065290, "Copy of a Candle of Love", 100.0, 100.0, typeof( BlankCandle ), 1065314, 1, 1065315 );
            AddRes( index, typeof( CandleWick ), 1065316, 1, 1065317 );
            AddRes( index, typeof( EssenceOfLove ), 1065322, 1, 1065323 );
            SetNeedHeat( index, true );
            #endregion

            #region Decorative
            index = AddCraft( typeof( DippingStick ), 1065300, 1065301, 75.0, 115.0, typeof( BlankCandle ), 1065314, 3, 1065324 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( PileOfBlankCandles ), 1065300, 1065302, 75.0, 115.0, typeof( BlankCandle ), 1065314, 5, 1065324 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( SomeBlankCandles ), 1065300, 1065303, 75.0, 115.0, typeof( BlankCandle ), 1065314, 3, 1065324 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( RawWaxBust ), 1065300, 1065304, 50.0, 80.0, typeof( Beeswax ), 1065310, 4, 1065311 );
            SetNeedHeat( index, true );
            #endregion
        }
    }
}