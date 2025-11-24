/***************************************************************************
 *                               WarGump.cs
 *                            ----------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Gumps;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.OrderChaosWars
{
    public class WarGump : MidgardStandardGump
    {
        #region Buttons enum
        public enum Buttons
        {
            Close,

            GeneralObjectives,
            OrderObjectives,
            ChaosObjectives,

            Buildings
        }
        #endregion

        #region design variables
        public override int NumButtons
        {
            get { return 4; }
        }

        public override int MainWindowWidth
        {
            get { return 585; }
        }

        public override int NumLabels
        {
            get { return Core.Instance.CurrentPhase == WarPhase.Idle ? 2 : 7; }
        }
        #endregion

        private readonly Mobile m_From;

        public WarGump( Mobile from )
            : base( from, 50, 50 )
        {
            if( Core.Instance.CurrentBattle == null )
                return;

            m_From = from;

            Design();
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 200, string.Format( "War plan for {0} battle", Core.Instance.CurrentBattle.Definition.WarName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            string minRemaining = Core.Instance.CurrentPhase == WarPhase.Idle ? "-" : Core.Instance.NextStateTime.TotalMinutes.ToString( "F1" );

            AddLabel( labelOffsetX, 0 * LabelsOffset + labelOffsetY, OldGold32, "War state:" );
            AddLabel( labelOffsetX, 1 * LabelsOffset + labelOffsetY, LabelsHue, string.Format( "War phase: {0}", Core.Instance.CurrentPhaseName ) );

            if( Core.Instance.CurrentPhase > WarPhase.Idle )
            {
                AddLabel( labelOffsetX, 2 * LabelsOffset + labelOffsetY, LabelsHue, string.Format( "Mins remaining: {0}", minRemaining ) );

                AddLabel( labelOffsetX, 4 * LabelsOffset + labelOffsetY, OldGold32, "War scoring:" );
                AddLabel( labelOffsetX + 20, 5 * LabelsOffset + labelOffsetY, DefaultValueHue, string.Format( "order: {0}", Core.Instance.OrderScore ) );
                AddLabel( labelOffsetX + 20, 6 * LabelsOffset + labelOffsetY, DefaultValueHue, string.Format( "chaos: {0}", Core.Instance.ChaosScore ) );
            }

            AddActionButton( 1, "show building status", (int)Buttons.Buildings, m_From, true );
            AddActionButton( 2, "show general objectives", (int)Buttons.GeneralObjectives, m_From, true );
            AddActionButton( 3, "show order objectives", (int)Buttons.OrderObjectives, m_From, true );
            AddActionButton( 4, "show chaos objectives", (int)Buttons.ChaosObjectives, m_From, true );

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.Buildings:
                    from.SendGump( new WarBuildingsGump( Owner ) );
                    break;
                case (int)Buttons.GeneralObjectives:
                    from.SendGump( new WarObjectivesGump( Owner, WarObjectivesGump.GumpType.General, Core.Instance.CurrentBattle ) );
                    break;
                case (int)Buttons.OrderObjectives:
                    from.SendGump( new WarObjectivesGump( Owner, WarObjectivesGump.GumpType.Order, Core.Instance.CurrentBattle ) );
                    break;
                case (int)Buttons.ChaosObjectives:
                    from.SendGump( new WarObjectivesGump( Owner, WarObjectivesGump.GumpType.Chaos, Core.Instance.CurrentBattle ) );
                    break;
                default:
                    break;
            }
        }
    }
}