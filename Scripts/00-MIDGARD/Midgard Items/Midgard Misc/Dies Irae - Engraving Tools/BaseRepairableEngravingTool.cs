using System;
using System.Collections.Generic;

using Midgard.Engines.OldCraftSystem;

using Server;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.VeteranRewards;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    public abstract class BaseRepairableEngravingTool : BaseEngravingTool, IRewardItem, IRepairable
    {
        private int m_NumRepairs;

        public BaseRepairableEngravingTool( int itemID, int charges )
            : base( itemID, charges )
        {
            m_NumRepairs = 0;
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int NumRepairs
        {
            get { return m_NumRepairs; }
            set
            {
                int oldValue = m_NumRepairs;
                if( oldValue != value )
                {
                    if( value < 0 )
                    {
                        m_NumRepairs = 0;
                    }
                    else if( value > MaxRepairs )
                    {
                        m_NumRepairs = MaxRepairs;
                    }
                    else
                    {
                        m_NumRepairs = value;
                    }

                    OnNumRepairsChanged( oldValue );
                }
            }
        }

        public virtual int MaxRepairs
        {
            get { return 20; }
        }

        public virtual Type RechargeItemType
        {
            get { return null; }
        }

        public virtual int TipTypeLabelNumber
        {
            get { return 0; }
        }

        public virtual int TipTypeCapLabelNumber
        {
            get { return 0; }
        }

        #region IRewardItem Members
        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsRewardItem { get; set; }
        #endregion

        /// <summary>
        /// Event invoked when number of repairs of our engraving tool changes
        /// </summary>
        public virtual void OnNumRepairsChanged( int oldValue )
        {
            InvalidateProperties();
        }

        public override bool CheckAccess( Mobile from, bool sendFailureMessage )
        {
            if( from == null || from.Deleted || Deleted )
            {
                return false;
            }

            if( IsRewardItem && !RewardSystem.CheckIsUsableBy( from, this, null ) )
            {
                return false;
            }
            else
            {
                return base.CheckAccess( from, sendFailureMessage );
            }
        }

        public override void OnCurChargesChanged( int oldValue )
        {
            // do not delete on cur charges < 1
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries( from, list );

            if( from.Alive && CurCharges < 1 )
            {
                list.Add( new ReplaceTipEntry( this, CheckAccess( from, false ) ) );
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( IsRewardItem )
            {
                list.Add( 1065528 ); // Midgard Veteran Reward
            }

            list.Add( 1065653, string.Format( "#{0}", GetTipInfo() ) ); // Tip status: ~1_STATUS~

            if( IsInDebugMode )
            {
                list.Add( 1065688, string.Format( "{0}\t{1}", CurCharges, MaxCharges ) ); // ~1_CHAR~ charges left of ~2_MAXCHAR~
                list.Add( 1065687, string.Format( "{0}\t{1}", m_NumRepairs, MaxRepairs ) ); // ~1_REP~ num repairs left of ~2_MAXREP~
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            base.OnDoubleClick( from );

            if( CurCharges < 1 )
            {
                if( EngraverTypeLabelNumber > 0 )
                    from.SendLocalizedMessage( 1065637, string.Format( "#{0}", EngraverTypeLabelNumber ) );
                else
                    from.SendMessage( "The tip of this ~1_ENGRAVER~ engraving tool is cracked. It must be replaced to make the tool usable again.", EngraverTypeLabel );
            }
        }

        #region serial-deserial
        public BaseRepairableEngravingTool( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( IsRewardItem );
            writer.Write( m_NumRepairs );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        IsRewardItem = reader.ReadBool();
                        m_NumRepairs = reader.ReadInt();
                        break;
                    }
            }
        }
        #endregion

        private class ReplaceTipEntry : ContextMenuEntry
        {
            private readonly bool m_Enabled;
            private readonly BaseRepairableEngravingTool m_EngravingTool;

            public ReplaceTipEntry( BaseRepairableEngravingTool engravingTool, bool enabled )
                : base( 1061 )
            {
                m_EngravingTool = engravingTool;

                if( !enabled )
                {
                    Flags |= CMEFlags.Disabled;
                    m_Enabled = false;
                }
                else
                {
                    m_Enabled = true;
                }
            }

            public override void OnClick()
            {
                if( m_EngravingTool == null || m_EngravingTool.Deleted )
                {
                    return;
                }

                if( !m_Enabled )
                {
                    return;
                }

                Mobile from = Owner.From;

                if( !from.CheckAlive() )
                {
                    return;
                }
                else if( !m_EngravingTool.CheckAccess( from, true ) )
                {
                    return;
                }
                else if( from.Skills[ SkillName.Tinkering ].Value < 100.0 )
                {
                    from.SendLocalizedMessage( 1065648 ); // Only a Grand Master Tinker can try to replace the tip of this engraving tool.
                }
                else if( m_EngravingTool.NumRepairs >= m_EngravingTool.MaxRepairs )
                {
                    // This engraving tool has been repaired too many times and you'll not be able to replace it's tip.
                    from.SendLocalizedMessage( 1065684 );
                }
                else
                {
                    DefTinkering.CraftSystem.PlayCraftEffect( from );

                    Container pack = from.Backpack;

                    if( pack != null )
                    {
                        int gem = pack.ConsumeUpTo( m_EngravingTool.RechargeItemType, 1 );

                        if( gem > 0 )
                        {
                            if( CheckRepairDifficulty( from, SkillName.Tinkering, m_EngravingTool.CurCharges, m_EngravingTool.MaxCharges ) )
                            {
                                // You replaced successfully the ~1_TYPE~ tip of this engraving tool.
                                from.SendLocalizedMessage( 1065649, string.Format( "#{0}", m_EngravingTool.TipTypeLabelNumber ) );
                                m_EngravingTool.CurCharges = m_EngravingTool.MaxCharges;
                                m_EngravingTool.NumRepairs++;
                            }
                            else
                            {
                                if( m_EngravingTool.EngraverTypeLabelNumber > 0 )
                                    from.SendLocalizedMessage( 1065650, string.Format( "#{0}\t#{1}", m_EngravingTool.EngraverTypeLabelNumber, m_EngravingTool.TipTypeLabelNumber ) );
                                else
                                {
                                    from.SendMessage( "You failed to replace the tip of this {0} engraving tool.", m_EngravingTool.EngraverTypeLabel );
                                    from.SendMessage( "The {0} to mount was cracked.", m_EngravingTool.TipTypeLabel );
                                }
                            }
                        }
                        else
                        {
                            if( m_EngravingTool.EngraverTypeLabelNumber > 0 )
                                from.SendLocalizedMessage( 1065651, string.Format( "#{0}\t#{1}\t{2}", m_EngravingTool.TipTypeLabelNumber, m_EngravingTool.EngraverTypeLabelNumber, m_EngravingTool.MaxCharges ) );
                            else
                            {
                                from.SendMessage( "You need a {0} to repair the tip of the {1} engraver.", m_EngravingTool.TipTypeLabel, m_EngravingTool.EngraverTypeLabel );
                                from.SendMessage( "A successful replacement of the tip will give the engraver  abount {0} uses.", m_EngravingTool.MaxCharges );
                            }
                        }
                    }
                    else
                    {
                        if( m_EngravingTool.EngraverTypeLabelNumber > 0 )
                            from.SendLocalizedMessage( 1065651, string.Format( "#{0}\t#{1}\t{2}", m_EngravingTool.TipTypeLabelNumber, m_EngravingTool.EngraverTypeLabelNumber, m_EngravingTool.MaxCharges ) );
                        else
                        {
                            from.SendMessage( "You need a {0} to repair the tip of the {1} engraver.", m_EngravingTool.TipTypeLabel, m_EngravingTool.EngraverTypeLabel );
                            from.SendMessage( "A successful replacement of the tip will give the engraver  abount {0} uses.", m_EngravingTool.MaxCharges );
                        }
                    }
                }
            }

            private static int GetDifficulty( int curCharges, int maxCharges )
            {
                return ( ( ( maxCharges - curCharges ) * 1250 ) / Math.Max( maxCharges, 1 ) ) - 250;
            }

            private bool CheckRepairDifficulty( Mobile from, SkillName skill, int curCharges, int maxCharges )
            {
                double difficulty = GetDifficulty( curCharges, maxCharges ) * 0.1;
                if( m_EngravingTool.IsInDebugMode )
                {
                    from.SendLocalizedMessage( 1065678, difficulty.ToString( "F2" ) ); // The repair difficulty was: ~1_VAL~
                }
                return from.CheckSkill( skill, difficulty - 25.0, difficulty + 25.0 );
            }
        }

        public bool Repair( Mobile from, BaseTool tool )
        {
            if( tool == null || tool.CraftSystem != DefTinkering.CraftSystem )
                return false;

            if( !from.CheckAlive() )
            {
                return false;
            }
            else if( !CheckAccess( from, true ) )
            {
                return false;
            }
            else if( NumRepairs >= MaxRepairs )
            {
                // This engraving tool has been repaired too many times and will not be repaired any more.
                from.SendLocalizedMessage( 1065683 );
            }
            else if( from.Skills[ SkillName.Tinkering ].Base < 0.1 )
            {
                // Since you have no tinkering skill, you will need to find a skilled tinkerer to repair this for you.
                from.SendLocalizedMessage( 1065640 );
            }
            else if( from.Skills[ SkillName.Tinkering ].Base < 75.0 )
            {
                // Your tinkering skill is too low to fix this yourself.  A skilled tinkerer can help you repair this.
                from.SendLocalizedMessage( 1065641 );
            }
            else if( CurCharges < 1 )
            {
                // You need a ~1_TYPE~ to repair the tip of the ~2_ENGRAVER~ engraver.  A successful replacement of the tip will give the engraver  abount ~3_MAXCH~ uses.
                from.SendLocalizedMessage( 1065642, string.Format( "#{0}", TipTypeLabelNumber ) );
            }
            else
            {
                int toWeaken;
                double skillLevel = from.Skills[ SkillName.Tinkering ].Value;

                if( skillLevel >= 90.0 )
                {
                    toWeaken = 1;
                }
                else
                {
                    toWeaken = 2;
                }

                DefTinkering.CraftSystem.PlayCraftEffect( from );

                if( CheckWeaken( from, SkillName.Tinkering, CurCharges, MaxCharges ) )
                {
                    MaxCharges -= toWeaken;
                    CurCharges = Math.Max( 0, CurCharges - toWeaken );
                    if( CurCharges < 1 )
                    {
                        from.SendLocalizedMessage( 1065643, string.Format( "#{0}\t#{1}", TipTypeLabelNumber, EngraverTypeLabelNumber ) ); // You cracked the ~1_TYPE~ tip attempting to fix this ~2_ENGRAVER~ engraver.
                    }
                    else
                    {
                        from.SendLocalizedMessage( 1065644, string.Format( "#{0}\t#{1}", TipTypeLabelNumber, EngraverTypeLabelNumber ) ); // You failed to repair and damaged the ~1_TYPE~ tip of this ~2_ENGRAVER~ engraving tool.
                    }
                }
                else if( CheckRepairDifficulty( from, SkillName.Tinkering, CurCharges, MaxCharges ) )
                {
                    from.SendLocalizedMessage( 1065645 ); // You repaired the tip of this engraving tool.
                    CurCharges = MaxCharges;
                    NumRepairs++;
                    return true;
                }
                else
                {
                    from.SendLocalizedMessage( 1065646 ); // You failed to repair the tip of this engraving tool.
                }
            }
            return false;
        }

        private static int GetRepairDifficulty( int curCharges, int maxCharges )
        {
            return ( ( ( maxCharges - curCharges ) * 1250 ) / Math.Max( maxCharges, 1 ) ) - 250;
        }

        private bool CheckRepairDifficulty( Mobile from, SkillName skill, int curCharges, int maxCharges )
        {
            double difficulty = GetRepairDifficulty( curCharges, maxCharges ) * 0.1;
            if( IsInDebugMode )
            {
                from.SendLocalizedMessage( 1065678, difficulty.ToString( "F2" ) ); // The repair difficulty was: ~1_VAL~
            }
            return from.CheckSkill( skill, difficulty - 25.0, difficulty + 25.0 );
        }

        private static int GetWeakenChance( Mobile from, SkillName skill, int curCharges, int maxCharges )
        {
            return ( 40 + ( maxCharges - curCharges ) ) - (int)( from.Skills[ skill ].Value / 10 );
        }

        private bool CheckWeaken( Mobile from, SkillName skill, int curHits, int maxHits )
        {
            double chance = GetWeakenChance( from, skill, curHits, maxHits );
            if( IsInDebugMode )
            {
                from.SendLocalizedMessage( 1065679, chance.ToString( "F2" ) ); // The weaken chance was: ~1_VAL~
            }
            return ( chance > Utility.Random( 100 ) );
        }
    }
}