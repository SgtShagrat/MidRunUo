/***************************************************************************
 *                               TrainingClassManagementGump.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.Academies
{
    public class TrainingClassManagementGump : BaseAcademyGump
    {
        public enum Buttons
        {
            Close,

            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 10; } }
        protected override int NumButtons { get { return 0; } }
        protected override int MainWindowWidth { get { return 195; } }
        #endregion

        public TrainingClassManagementGump( AcademySystem academy, Mobile owner )
            : base( academy, owner )
        {
            owner.CloseGump( typeof( TrainingClassManagementGump ) );

            Design();

            base.RegisterUse( typeof( TrainingClassManagementGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( "Academy Classes" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int hue = HuePrim;

            int pageX = HorBorder + MainWindowWidth - 25;
            int pageY = GetMainWindowsStartY() + 10;

            List<TrainingClass> classes = Academy.TrainingClasses;

            if( classes.Count == 0 )
            {
                AddLabel( labelOffsetX, labelOffsetY, TitleHue, "-empty-" );
                return;
            }

            for( int i = 0; i < classes.Count; i++ )
            {
                int page = i / NumLabels;
                int pos = i % NumLabels;

                if( pos == 0 )
                {
                    if( page == 0 )
                        AddLabel( labelOffsetX, labelOffsetY, TitleHue, "Choose from:" );

                    if( page > 0 )
                        AddButton( pageX, pageY, NextPageBtnIdNormal, NextPageBtnIdPressed, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                    AddPage( page + 1 );

                    if( page > 0 )
                        AddButton( pageX - 20, pageY, PrevPageBtnIdNormal, PrevPageBtnIdPressed, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                }

                int y = ( pos + 1 ) * LabelsOffset + labelOffsetY;

                hue = GetHueFor( hue );

                TrainingClass trainingClass = classes[ i ];

                AddMainWindowButton( y + 5, i + 1, (int)AcademyAccessFlags.Academic, false );
                AddLabel( labelOffsetX + 20, y, GetHueFor( hue ), trainingClass.Name );
            }

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == (int)Buttons.Close )
            {
                from.SendGump( new AcademySystemInfoGump( Academy, from ) );
                return;
            }

            int id = info.ButtonID - 1;

            if( id < 0 || id >= Academy.TrainingClasses.Count )
                return;

            from.SendGump( new TrainingClassInfoGump( Academy, Owner, Academy.TrainingClasses[ id ] ) );
        }
    }
}