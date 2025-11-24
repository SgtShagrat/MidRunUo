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
    public class CandidatesManagementGump : BaseAcademyGump
    {
        public enum Buttons
        {
            Close,

            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 10; } }
        protected override int NumButtons { get { return 0; } }
        protected override int MainWindowWidth { get { return 250; } }
        #endregion

        private readonly List<Mobile> m_Mobiles;

        public CandidatesManagementGump( AcademySystem academy, Mobile owner )
            : this( academy, owner, null )
        {
        }

        public CandidatesManagementGump( AcademySystem academy, Mobile owner, IEnumerable<Mobile> list )
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

            foreach( Mobile m in system.Candidates )
            {
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
            AddMainTitle( String.Format( "Candidate List" ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int hue = HuePrim;

            for( int i = 0; i < m_Mobiles.Count; ++i )
            {
                Mobile m = m_Mobiles[ i ];
                if( m == null || m.Deleted )
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

                AddMainWindowRedGreenButton( y + 3, i + 1, i + 1000 + 1, (int)AcademyAccessFlags.RemoveAcademic, false );
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

            bool remove = info.ButtonID > 1000;

            int realIndex = remove ? info.ButtonID - 1000 - 1 : info.ButtonID - 1;

            if( realIndex < 0 || realIndex > m_Mobiles.Count )
                return;

            Mobile candidate = m_Mobiles[ realIndex ];
            if( candidate == null || candidate.Deleted )
                return;

            if( remove )
            {
                from.SendMessage( "{0} has been rejected from {1} candidates.", candidate.Name, Academy.ToString() );

                if( Academy.Candidates.Contains( candidate ) )
                    Academy.Candidates.Remove( candidate );
            }
            else
            {
                from.SendMessage( "{0} is now a member of {1}.", candidate.Name, Academy.ToString() );

                AcademySystemCommands.DoSetAcademy( candidate, Academy );
            }
        }
    }
}