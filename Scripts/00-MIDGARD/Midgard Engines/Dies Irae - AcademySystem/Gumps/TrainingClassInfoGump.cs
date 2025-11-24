/***************************************************************************
 *                               TrainingClassInfoGump.cs
 *
 *   begin                : 05 novembre 2010
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
    public class TrainingClassInfoGump : BaseAcademyGump
    {
        public enum Buttons
        {
            Close,

            AddTeacher = 100,
            AddAdept,

            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 10; } }
        protected override int NumButtons { get { return 2; } }
        protected override int MainWindowWidth { get { return 460; } }
        protected override bool HasSubtitles { get { return true; } }
        #endregion

        private readonly List<AcademyPlayerState> m_Members;
        private readonly TrainingClass m_TrainingClass;

        public TrainingClassInfoGump( AcademySystem academy, Mobile owner, TrainingClass trainingClass )
            : this( academy, owner, trainingClass, null )
        {
        }

        public TrainingClassInfoGump( AcademySystem academy, Mobile owner, TrainingClass trainingClass, IEnumerable<AcademyPlayerState> list )
            : base( academy, owner )
        {
            m_TrainingClass = trainingClass;

            owner.CloseGump( typeof( TrainingClassInfoGump ) );

            m_Members = list == null ? BuildList( trainingClass, false ) : new List<AcademyPlayerState>( list );

            Design();

            base.RegisterUse( typeof( TrainingClassInfoGump ) );
        }

        private class InternalComparer : IComparer<AcademyPlayerState>
        {
            public static readonly IComparer<AcademyPlayerState> Instance = new InternalComparer();

            public int Compare( AcademyPlayerState x, AcademyPlayerState y )
            {
                if( x == null || y == null )
                    return 0;

                return Insensitive.Compare( x.Mobile.Name, y.Mobile.Name );
            }
        }

        private static List<AcademyPlayerState> BuildList( TrainingClass system, bool online )
        {
            List<AcademyPlayerState> list = new List<AcademyPlayerState>();

            foreach( AcademyPlayerState m in system.Members )
            {
                if( m == null || m.Mobile == null )
                    continue;

                if( !online || m.Mobile.NetState != null )
                    list.Add( m );
            }

            list.Sort( InternalComparer.Instance );

            return list;
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( string.Format( "{0} members:", m_TrainingClass.Name ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Name" );
            AddSubTitle( labelOffsetX + 150, "Status" );

            AddSubTitleImage( labelOffsetX + 410, 4033 );

            // buttons
            AddActionButton( 1, "add teacher", (int)Buttons.AddTeacher, Owner, (int)AcademyAccessFlags.SetAcademyOffice );
            AddActionButton( 2, "add adept", (int)Buttons.AddAdept, Owner, (int)AcademyAccessFlags.SetAcademyOffice );

            int hue = HuePrim;

            if( m_Members.Count == 0 )
            {
                AddLabel( labelOffsetX, labelOffsetY, TitleHue, "-empty-" );
                return;
            }

            for( int i = 0; i < m_Members.Count; ++i )
            {
                AcademyPlayerState m = m_Members[ i ];
                if( m == null || m.Mobile == null )
                    continue;

                int page = i / NumLabels;
                int pos = i % NumLabels;

                if( pos == 0 )
                {
                    if( page > 0 )
                        AddButton( 460, 10, 0x15E1, 0x15E5, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                    AddPage( page + 1 );

                    if( page > 0 )
                        AddButton( 440, 10, 0x15E3, 0x15E7, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                }

                int y = pos * LabelsOffset + labelOffsetY;

                hue = GetHueFor( m.Mobile, hue );

                AddLabelCropped( labelOffsetX, y, 150, 21, hue, m.Mobile.Name );

                AddLabelCropped( labelOffsetX + 150, y, 150, 21, hue, m_TrainingClass.IsTeacher( m ) ? "-teacher-" : "-adept-" );

                AddMainWindowButton( y + 3, i + 1, (int)AcademyAccessFlags.RemoveAcademic, false );
            }

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == (int)Buttons.Close )
            {
                from.SendGump( new TrainingClassManagementGump( Academy, from ) );
                return;
            }

            int realIndex = info.ButtonID - 1;

            if( realIndex >= 0 && realIndex < m_Members.Count )
            {
                AcademyPlayerState state = m_Members[ realIndex ];
                if( state == null )
                    return;

                m_TrainingClass.RemoveMember( state );
                from.SendMessage( "{0} has been removed from that class.", state.Mobile.Name );
            }

            switch( info.ButtonID )
            {
                case (int)Buttons.AddTeacher:
                    from.SendGump( new AddTeacherGump( Academy, from, m_TrainingClass, AcademyAccessFlags.SetAcademyOffice ) );
                    break;
                case (int)Buttons.AddAdept:
                    from.SendGump( new AddAdeptGump( Academy, from, m_TrainingClass, AcademyAccessFlags.SetAcademyOffice ) );
                    break;
                default:
                    from.SendGump( new TrainingClassManagementGump( Academy, from ) );
                    break;
            }
        }
    }
}