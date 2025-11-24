using System;
using System.Collections.Generic;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.XmlPoints
{
    public class IssueChallengeGump : Gump
    {
        #region constants

        private const int DeltaBut = 2;
        private const int Fields = 9;
        private const int FieldsDist = 25;
        private const int HuePrim = 92;
        private const int HueTit = 662;

        #endregion

        private readonly Mobile m_From;

        private readonly List<DuelLocationEntry> m_List;
        private readonly int m_Page;
        private readonly Mobile m_Target;

        public IssueChallengeGump( Mobile from, Mobile target )
            : this( from, target, 1 )
        {
        }

        private IssueChallengeGump( Mobile from, Mobile target, int page )
            : base( 50, 50 )
        {
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_From = from;
            m_Target = target;
            m_Page = page;

            m_List = BuildList( m_From );

            if( !XmlPointsAttach.AllowChallengeGump( m_From, m_Target ) )
            {
                m_From.SendMessage( XmlPointsAttach.SystemText( 100267 ) ); // "You cannot issue a challenge here."
                return;
            }

            var a = (XmlPointsAttach)XmlAttach.FindAttachment( m_From, typeof( XmlPointsAttach ) );
            var atarg = (XmlPointsAttach)XmlAttach.FindAttachment( m_Target, typeof( XmlPointsAttach ) );

            m_From.CloseGump( typeof( IssueChallengeGump ) );

            if( a == null || a.Deleted || atarg == null || atarg.Deleted )
            {
                m_From.SendMessage( XmlPointsAttach.SystemText( 100213 ) ); // "No XmlPoints support."
                return;
            }

            if( a.Deleted || atarg.Deleted || !a.CanAffectPoints( m_From, m_From, m_Target, true ) )
                m_From.SendMessage( "You will NOT gain any points from this challenge!" );

            Design();
        }

        private static List<DuelLocationEntry> BuildList( Mobile from )
        {
            var list = new List<DuelLocationEntry>();
            list.Add( new DuelLocationEntry( "Here", from.Map, from.Location, new Point3D( from.X + 2, from.Y, from.Z ),
                new Rectangle2D( new Point2D( from.X - 10, from.Y - 10 ), new Point2D( from.X + 10, from.Y + 10 ) ) ) );

            list.AddRange( XmlPointsAttach.DuelLocations );

            return list;
        }

        private void Design()
        {
            AddPage( 0 );

            AddBackground( 0, 0, 275, 325, 9200 );

            AddImageTiled( 10, 10, 255, 25, 2624 );
            AddImageTiled( 10, 45, 255, 240, 2624 );
            AddImageTiled( 40, 295, 225, 20, 2624 );

            AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
            AddHtmlLocalized( 45, 295, 75, 20, 1011012, HueTit, false, false ); // CANCEL

            AddAlphaRegion( 10, 10, 255, 285 );
            AddAlphaRegion( 40, 295, 225, 20 );

            AddLabelCropped( 14, 12, 255, 25, HueTit, String.Format( "Will you challenge {0}?", m_Target.Name ?? "him" ) );

            if( m_Page > 1 )
                AddButton( 225, 297, 5603, 5607, 200, GumpButtonType.Reply, 0 ); // Previous page

            if( m_Page < Math.Ceiling( m_List.Count / (double)Fields ) )
                AddButton( 245, 297, 5601, 5605, 300, GumpButtonType.Reply, 0 ); // Next Page

            int indMax = ( m_Page * Fields ) - 1;
            int indMin = ( m_Page * Fields ) - Fields;
            int indTemp = 0;

            for( int i = 0; i < m_List.Count; i++ )
            {
                if( i < indMin || i > indMax )
                    continue;

                DuelLocationEntry entry = m_List[ i ];
                bool enabled = XmlPointsAttach.DuelLocationAvailable( entry ) || i == 0;

                AddLabelCropped( 35, 52 + ( indTemp * FieldsDist ), 225, 20, HuePrim, entry.Name );

                if( enabled )
                    AddButton( 15, 52 + DeltaBut + ( indTemp * FieldsDist ), 1209, 1210, i + 1, GumpButtonType.Reply, 0 );

                indTemp++;
            }
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( info == null || state == null || state.Mobile == null )
                return;

            if( info.ButtonID == 0 )
            {
                m_From.SendMessage( XmlPointsAttach.GetText( m_From, 100258 ), m_Target.Name );
                // "You decided against challenging {0}."
                return;
            }

            int realIndex = info.ButtonID - 1;

            if( realIndex > -1 && realIndex < m_List.Count )
            {
                m_Target.SendGump( new ConfirmChallengeGump( m_From, m_Target, realIndex > 0 ? m_List[ realIndex ] : null ) );
                m_From.SendMessage( XmlPointsAttach.GetText( m_From, 100257 ), m_Target.Name );
                // "You have issued a challenge to {0}."
            }
        }
    }
}