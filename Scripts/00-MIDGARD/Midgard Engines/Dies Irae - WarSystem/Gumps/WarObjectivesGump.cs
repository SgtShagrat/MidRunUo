/***************************************************************************
 *                               WarObjectivesGump.cs
 *
 *   begin                : 27 febbraio 2011
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

namespace Midgard.Engines.WarSystem
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

        private readonly BaseWar m_War;

        public WarObjectivesGump( Mobile from, BaseWar war )
            : base( from, 0, 0 )
        {
            from.CloseGump( typeof( WarObjectivesGump ) );

            m_War = war;

            Design();
        }

        private void Design()
        {
            List<BaseObjective> objectives = m_War.Objectives;

            AddPage( 0 );

            AddMainBackground();

            AddMainTitle( 205, String.Format( "Objectives for {0} war", Core.Instance.CurrentBattle.Definition.WarName ) );
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
                        AddLabel( labelOffsetX, y, hue, string.Format( "{0} ({1})", objective.Name, objective.StatusDescription() ) );
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