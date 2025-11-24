/***************************************************************************
 *                               WarObjectivesGump.cs
 *                            -------------------
 *   begin                : 01 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Gumps;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.OrderChaosWars
{
    public class WarObjectivesGump : MidgardStandardGump
    {
        #region Buttons enum
        public enum Buttons
        {
            Close,
            Page = 10000
        }
        #endregion

        #region GumpType enum
        public enum GumpType
        {
            General,
            Order,
            Chaos
        }
        #endregion

        #region design variables
        public override int NumLabels
        {
            get { return 10; }
        }

        public override int NumButtons
        {
            get { return 0; }
        }

        public override int MainWindowWidth
        {
            get { return 585; }
        }
        #endregion

        private readonly GumpType m_Type;
        private readonly BaseWar m_War;

        public WarObjectivesGump( Mobile owner, BaseWar war )
            : this( owner, GumpType.General, war )
        {
        }

        public WarObjectivesGump( Mobile from, GumpType gumpType, BaseWar war )
            : base( from, 0, 0 )
        {
            from.CloseGump( typeof( WarObjectivesGump ) );

            m_Type = gumpType;
            m_War = war;

            Design();
        }

        private void Design()
        {
            List<BaseObjective> objectives = null;
            string title = string.Empty;

            switch( m_Type )
            {
                case GumpType.General:
                    objectives = m_War.Objectives;
                    title = "General";
                    break;
                case GumpType.Order:
                    objectives = m_War.OrderObjectives;
                    title = "Order";
                    break;
                case GumpType.Chaos:
                    objectives = m_War.ChaosObjectives;
                    title = "Chaos";
                    break;
            }

            AddPage( 0 );

            AddMainBackground();

            AddMainTitle( 205, String.Format( "{0} objectives for {1} war", title, Core.Instance.CurrentBattle.Definition.WarName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int hue = HuePrim;

            if( objectives != null && objectives.Count > 0 )
            {
                BaseObjective objective;

                for( int i = 0; i < objectives.Count; ++i )
                {
                    int page = i / NumLabels;
                    int pos = i % NumLabels;

                    if( pos == 0 )
                    {
                        if( page > 0 )
                            AddButton( 460, 10, NextPageBtnIdNormal, NextPageBtnIdPressed, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                        AddPage( page + 1 );

                        if( page > 0 )
                            AddButton( 440, 10, PrevPageBtnIdNormal, PrevPageBtnIdPressed, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                    }

                    int y = pos * LabelsOffset + labelOffsetY;

                    objective = objectives[ i ];
                    hue = GetHueFor( hue );

                    if( objective != null )
                        AddLabel( labelOffsetX, y, hue, string.Format( "{0} ({1})", objective.Name, objective.Completed ? "completed" : "not completed yet" ) );
                }
            }
            else
                AddLabel( labelOffsetX, labelOffsetY, hue, "- no objective found -" );

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            switch( info.ButtonID )
            {
                default:
                    break;
            }
        }
    }
}