/***************************************************************************
 *                               WarGump.cs
 *
 *   begin                : 27 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Midgard.Gumps;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.WarSystem
{
    public class WarGump : MidgardStandardGump
    {
        #region Buttons enum
        public enum Buttons
        {
            Close,

            Objectives,
            BattleConfiguration,

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
            get { return Core.Instance.CurrentPhase == WarPhase.Idle ? 2 : 5 + Core.Instance.CurrentBattle.WarStates.Count; }
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
                for( int i = 0; i < Core.Instance.CurrentBattle.WarStates.Count; i++ )
                {
                    WarState warState = Core.Instance.CurrentBattle.WarStates[ i ];
                    AddLabel( labelOffsetX + 20, 5 + i * LabelsOffset + labelOffsetY, DefaultValueHue, string.Format( "{0}: {1}", warState.StateTeam.Name, warState.Score ) );
                }
            }

            AddActionButton( 1, "show building status", (int)Buttons.Buildings, m_From, true );
            AddActionButton( 2, "show objectives", (int)Buttons.Objectives, m_From, true );
            AddActionButton( 3, "battle configuration", (int)Buttons.BattleConfiguration, m_From, m_From.AccessLevel > AccessLevel.Player );

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
                case (int)Buttons.Objectives:
                    from.SendGump( new WarObjectivesGump( Owner, Core.Instance.CurrentBattle ) );
                    break;
                case (int)Buttons.BattleConfiguration:
                    // from.SendGump( new BattleConfiguration( Owner ) );
                    break;
                default:
                    break;
            }
        }
    }
}