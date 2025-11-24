/***************************************************************************
 *                               WarbuildingsGump.cs
 *                            -------------------------
 *   begin                : 07 novembre, 2008
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

namespace Midgard.Engines.WarSystem
{
    public class WarBuildingsGump : MidgardStandardGump
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

        public override bool HasSubtitles
        {
            get { return true; }
        }
        #endregion

        public WarBuildingsGump( Mobile from )
            : base( from, 0, 0 )
        {
            if( Core.Instance.CurrentBattle == null )
                return;

            Design();
        }

        private void Design()
        {
            List<ScoreRegion> scoreRegions = Core.Instance.ScoreRegions;

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 205, String.Format( "Building for {0} war", Core.Instance.CurrentBattle.Definition.WarName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Name" );
            AddSubTitle( labelOffsetX + 300, "Points" );
            AddSubTitle( labelOffsetX + 450, "Owner" );

            int hue = HuePrim;

            if( scoreRegions.Count > 0 )
            {
                ScoreRegion scoreRegion;

                for( int i = 0; i < scoreRegions.Count; ++i )
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

                    scoreRegion = scoreRegions[ i ];
                    if( scoreRegion == null )
                        continue;

                    hue = GetHueFor( hue );

                    AddLabel( labelOffsetX, y, hue, scoreRegion.Name );
                    AddLabel( labelOffsetX + 300, y, hue, scoreRegion.PointScalar.ToString() );
                    AddLabel( labelOffsetX + 450, y, hue, scoreRegion.OwnerTeam.Name );
                }
            }
            else
                AddLabel( labelOffsetX, labelOffsetY, hue, "- no building found -" );

            AddCloseButton();
        }
    }
}