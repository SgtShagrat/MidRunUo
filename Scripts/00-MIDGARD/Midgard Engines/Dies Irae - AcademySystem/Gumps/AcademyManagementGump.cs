/***************************************************************************
 *                               AcademyManagementGump.cs
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
    public class AcademyManagementGump : BaseAcademyGump
    {
        public enum Buttons
        {
            Close,

            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 20; } }
        protected override int NumButtons { get { return 0; } }
        protected override int MainWindowWidth { get { return 460; } }
        protected override bool HasSubtitles { get { return true; } }
        #endregion

        private readonly List<Mobile> m_Mobiles;

        public AcademyManagementGump( AcademySystem academy, Mobile owner )
            : this( academy, owner, null )
        {
        }

        public AcademyManagementGump( AcademySystem academy, Mobile owner, IEnumerable<Mobile> list )
            : base( academy, owner )
        {
            owner.CloseGump( typeof( AcademyManagementGump ) );

            if( list == null )
                m_Mobiles = BuildList( Academy, false );
            else
                m_Mobiles = new List<Mobile>( list );

            Design();

            base.RegisterUse( typeof( AcademyManagementGump ) );
        }

        private class InternalComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new InternalComparer();

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    return 0;

                return Insensitive.Compare( x.Name, y.Name );
            }
        }

        private static List<Mobile> BuildList( AcademySystem system, bool online )
        {
            List<Mobile> list = new List<Mobile>();

            foreach( AcademyPlayerState t in system.Players )
            {
                Mobile m = t.Mobile;
                if( m != null && !m.Deleted )
                {
                    if( list.Contains( m ) )
                        Console.WriteLine( "Warning: duplicate player {0} in townsystem {1}", m.Name, system.Definition.AcademyName );

                    if( !online || ( m.NetState != null ) )
                        list.Add( m );
                }
            }

            list.Sort( InternalComparer.Instance );

            return list;
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( String.Format( "{0} Members List", Academy.Definition.AcademyName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Name" );
            AddSubTitle( labelOffsetX + 150, "Office" );

            AddSubTitleImage( labelOffsetX + 410, 4033 );

            int hue = HuePrim;

            for( int i = 0; i < m_Mobiles.Count; ++i )
            {
                Mobile m = m_Mobiles[ i ];
                if( m == null || m.Deleted )
                    continue;

                AcademyPlayerState playerState = AcademyPlayerState.Find( m );
                if( playerState == null || playerState.Mobile == null )
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

                hue = GetHueFor( m, hue );

                AddLabelCropped( labelOffsetX, y, 150, 21, hue, m.Name );

                if( !String.IsNullOrEmpty( playerState.CustomAcademyOffice ) )
                    AddLabelCropped( labelOffsetX + 150, y, 150, 21, hue, playerState.CustomAcademyOffice );
                else if( playerState.AcademyOffice != AcademyOffices.None )
                    AddLabelCropped( labelOffsetX + 150, y, 150, 21, hue, MidgardUtility.GetFriendlyClassName( playerState.AcademyOffice.ToString() ) );

                AddMainWindowButton( y + 3, i + 1, (int)AcademyAccessFlags.Academic, false );
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

            if( info.ButtonID < 0 || info.ButtonID > m_Mobiles.Count )
                return;

            Mobile m = m_Mobiles[ info.ButtonID - 1 ];
            if( m == null || m.Deleted )
                return;

            from.SendGump( new AcademyInfoGump( Academy, m, from ) );
        }
    }
}