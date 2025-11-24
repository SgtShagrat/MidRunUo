using System;
using Server.Engines.XmlSpawner2;

namespace Server.Engines.XmlPoints
{
    public class CancelTimer : Timer
    {
        private readonly XmlPointsAttach m_Attachment;

        public CancelTimer( XmlPointsAttach a, TimeSpan delay )
            : base( delay )
        {
            Priority = TimerPriority.OneSecond;
            m_Attachment = a;
        }

        protected override void OnTick()
        {
            if( m_Attachment == null || m_Attachment.Deleted )
                return;

            var from = m_Attachment.AttachedTo as Mobile;

            if( from != null && m_Attachment.Challenger != null )
            {
                XmlPointsAttach.SendText( from, 100214, m_Attachment.Challenger.Name );
                // "Challenge with {0} has been cancelled"

                if( from.HasGump( typeof( PointsGump ) ) )
                {
                    m_Attachment.OnIdentify( from );
                }
            }

            // clear the challenger on the challengers attachment
            var xa = (XmlPointsAttach)XmlAttach.FindAttachment( m_Attachment.Challenger, typeof( XmlPointsAttach ) );

            if( xa != null && !xa.Deleted )
            {
                // check the challenger field to see if it matches the current
                if( xa.Challenger == from )
                {
                    if( m_Attachment.Challenger != null && from != null )
                    {
                        m_Attachment.Challenger.SendMessage( String.Format( xa.Text( 100214 ), from.Name ) );
                        // "Challenge with {0} has been cancelled"
                    }
                    // then clear it
                    xa.Challenger = null;
                }
            }
            // clear challenger on this attachment
            m_Attachment.Challenger = null;

            // refresh any open gumps
            if( from != null && xa != null && xa.AttachedTo is Mobile )
            {
                if( from.HasGump( typeof( PointsGump ) ) )
                {
                    m_Attachment.OnIdentify( from );
                }

                // and update the gumps if they are open
                if( ( (Mobile)xa.AttachedTo ).HasGump( typeof( PointsGump ) ) )
                {
                    xa.OnIdentify( (Mobile)xa.AttachedTo );
                }
            }
        }
    }
}