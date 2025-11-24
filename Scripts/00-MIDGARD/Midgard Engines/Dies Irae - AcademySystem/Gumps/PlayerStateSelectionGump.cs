/***************************************************************************
 *                               PlayerStateSelectionGump.cs
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
    public delegate void ConfirmGumpCallback( Mobile from, AcademyPlayerState playerState, object state );

    public abstract class PlayerStateSelectionGump : BaseAcademyGump
    {
        private readonly AcademyAccessFlags m_Flag;
        private readonly List<AcademyPlayerState> m_List;
        private readonly ConfirmGumpCallback m_Callback;
        private readonly object m_State;

        public enum Buttons
        {
            Close,

            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 10; } }
        protected override int NumButtons { get { return 0; } }
        protected override int MainWindowWidth { get { return 460; } }
        #endregion

        public virtual string Title { get { return "Choose from:"; } }

        protected PlayerStateSelectionGump( AcademySystem academy, Mobile owner, AcademyAccessFlags flag, ConfirmGumpCallback callback )
            : this( academy, owner, null, flag, callback, null )
        {
        }

        protected PlayerStateSelectionGump( AcademySystem academy, Mobile owner, IEnumerable<AcademyPlayerState> list, AcademyAccessFlags flag, ConfirmGumpCallback callback )
            : this( academy, owner, list, flag, callback, null )
        {
        }

        protected PlayerStateSelectionGump( AcademySystem academy, Mobile owner, IEnumerable<AcademyPlayerState> list, AcademyAccessFlags flag, ConfirmGumpCallback callback, object state )
            : base( academy, owner )
        {
            m_Flag = flag;
            m_Callback = callback;
            m_State = state;

            owner.CloseGump( typeof( PlayerStateSelectionGump ) );

            if( list == null )
                list = Academy.Players;

            m_List = new List<AcademyPlayerState>( list );

            m_List.Sort( InternalComparer.Instance ); // mod by Dies Irae

            Design();

            base.RegisterUse( typeof( PlayerStateSelectionGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( Title );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Name" );

            int hue = HuePrim;

            if( m_List == null )
            {
                AddLabelCropped( labelOffsetX, labelOffsetY, 150, 21, hue, "-empty-" );
            }
            else
            {
                for( int i = 0; i < m_List.Count; ++i )
                {
                    AcademyPlayerState m = m_List[ i ];
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

                    AddMainWindowButton( y + 3, i + 1, (int)m_Flag, false );
                }
            }

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( info.ButtonID == (int)Buttons.Close )
                return;

            int realIndex = info.ButtonID - 1;

            if( realIndex < 0 || realIndex > m_List.Count )
                return;

            AcademyPlayerState playerState = m_List[ realIndex ];
            if( playerState == null || playerState.Mobile == null )
                return;

            if( m_Callback != null )
                m_Callback( sender.Mobile, playerState, m_State );
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
    }
}