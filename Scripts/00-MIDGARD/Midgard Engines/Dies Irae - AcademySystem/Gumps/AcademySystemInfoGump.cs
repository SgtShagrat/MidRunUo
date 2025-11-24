/***************************************************************************
 *                               TownSystemInfoGump.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.Academies
{
    public class AcademySystemInfoGump : BaseAcademyGump
    {
        public enum Buttons
        {
            Close,

            CandidateList,
            AcademicList,
            News,
            AdvancedGump,
            Guide,
            Classes
        }

        #region design variables
        protected override int NumLabels { get { return Labels.Length; } }
        protected override int NumButtons { get { return 5; } }
        protected override int MainWindowWidth { get { return 380; } }
        #endregion

        private static readonly string[] Labels = new string[]
                                                  {
                                                      "Total Candidates",
                                                      "Total Academics",
                                                      "Total Classes"
                                                  };

        public AcademySystemInfoGump( AcademySystem system, Mobile owner )
            : base( system, owner )
        {
            if( Academy == null )
                return;

            Owner.CloseGump( typeof( AcademySystemInfoGump ) );

            Design();

            base.RegisterUse( typeof( AcademySystemInfoGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( string.Format( "{0}", Academy.Definition.AcademyName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            for( int i = 0; i < Labels.Length; i++ )
                AddLabel( labelOffsetX, labelOffsetY + ( LabelsOffset * i ), GroupsHue, String.Format( "{0}:", Labels[ i ] ) );

            List<string> values = new List<string>();
            values.Add( Academy.Candidates.Count.ToString() );
            values.Add( Academy.Players.Count.ToString() );
            values.Add( Academy.TrainingClasses.Count.ToString() );

            for( int i = 0; i < values.Count; i++ )
                AddLabel( labelOffsetX + 120, labelOffsetY + ( 20 * i ), DefaultValueHue, values[ i ] );

            bool isGM = Owner.AccessLevel > AccessLevel.Counselor;

            // Buttons
            AddActionButton( 1, "members", (int)Buttons.AcademicList, Owner, (int) AcademyAccessFlags.Academic, isGM );
            AddActionButton( 2, "candidates", (int)Buttons.CandidateList, Owner, (int) AcademyAccessFlags.SetAcademyAccess, isGM );
            AddActionButton( 3, "classes", (int)Buttons.Classes, Owner, (int) AcademyAccessFlags.Academic, isGM );
            AddActionButton( 4, "news", (int)Buttons.News, Owner, (int) AcademyAccessFlags.ClearNews, isGM );
            AddActionButton( 5, "advanced features", (int)Buttons.AdvancedGump, Owner, (int) AcademyAccessFlags.Academic, isGM );

            AddCloseButton();
            AddGuideButton( (int)Buttons.Guide );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( Academy == null )
                return;

            Mobile from = sender.Mobile;
            if( from == null )
                return;

            switch( info.ButtonID )
            {
                case (int)Buttons.CandidateList:
                    from.SendGump( new CandidatesManagementGump( Academy, from ) );
                    break;
                case (int)Buttons.AcademicList:
                    from.SendGump( new AcademyManagementGump( Academy, from ) );
                    break;
                case (int)Buttons.News:
                    // from.SendGump( new AcademyCrierGump( Owner, Academy ) );
                    from.SendMessage( "Not implemented yet...");
                    break;
                case (int)Buttons.AdvancedGump:
                    // from.SendGump( new AdvancedFeaturesGump( Academy, Owner ) );
                    from.SendMessage( "Not implemented yet...");
                    break;
                case (int)Buttons.Classes:
                    from.SendGump( new TrainingClassManagementGump( Academy, from ) );
                    break;
                case (int)Buttons.Guide:
                    from.LaunchBrowser( Config.SiteGuide );
                    break;
                default:
                    from.SendMessage( "You closed that info menu." );
                    break;
            }
        }
    }
}