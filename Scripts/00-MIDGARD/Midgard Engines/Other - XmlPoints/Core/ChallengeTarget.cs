using System;
using Server.Engines.XmlSpawner2;
using Server.Spells;
using Server.Targeting;

namespace Server.Engines.XmlPoints
{
    public class ChallengeTarget : Target
    {
        private readonly Mobile m_Challenger;

        public ChallengeTarget( Mobile m )
            : base( 30, false, TargetFlags.None )
        {
            m_Challenger = m;
        }

        protected override void OnTarget( Mobile from, object targeted )
        {
            if( from == null || targeted == null )
                return;

            if( targeted is Mobile && ( (Mobile)targeted ).Player )
            {
                var pm = targeted as Mobile;

                // test them for young status
                if( XmlPointsAttach.YoungProtection( from, pm ) )
                {
                    XmlPointsAttach.SendText( from, 100207, pm.Name ); // "{0} is too inexperience to be challenged"
                    return;
                }

                // check the owner for existing challenges
                var a = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );

                if( a != null && !a.Deleted )
                {
                    // issuing a challenge when one is already in place will initiate cancellation of the current challenge
                    if( a.Challenger != null )
                    {
                        // this will initiate the challenge cancellation timer

                        if( a.MCancelTimer != null && a.MCancelTimer.Running )
                        {
                            // timer is running
                            a.SendText( 100208, a.CancelEnd - DateTime.Now );
                            // "{0} mins remaining until current challenge is cancelled."
                        }
                        else
                        {
                            a.SendText( 100209, XmlPointsAttach.CancelTimeout.TotalMinutes );
                            // "Canceling current challenge.  Please wait {0} minutes"

                            XmlPointsAttach.SendText( a.Challenger, 100210, from.Name, XmlPointsAttach.CancelTimeout.TotalMinutes );
                            // "{0} is canceling the current challenge. {1} minutes remain"

                            // start up the cancel challenge timer
                            a.DoTimer( XmlPointsAttach.CancelTimeout );

                            // update the points gumps on the challenger if they are open
                            if( from.HasGump( typeof( PointsGump ) ) )
                            {
                                a.OnIdentify( from );
                            }
                            // update the points gumps on the challenge target if they are open
                            if( a.Challenger.HasGump( typeof( PointsGump ) ) )
                            {
                                var ca =
                                    (XmlPointsAttach)XmlAttach.FindAttachment( a.Challenger, typeof( XmlPointsAttach ) );
                                if( ca != null && !ca.Deleted )
                                    ca.OnIdentify( a.Challenger );
                            }
                        }
                        return;
                    }

                    // check the target for existing challengers
                    var xa = (XmlPointsAttach)XmlAttach.FindAttachment( pm, typeof( XmlPointsAttach ) );

                    if( xa != null && !xa.Deleted )
                    {
                        if( xa.Challenger != null )
                        {
                            from.SendMessage( String.Format( a.Text( 100211 ), pm.Name ) );
                            // "{0} is already being challenged."
                            return;
                        }
                    }

                    if( from == targeted )
                    {
                        from.SendMessage( a.Text( 100212 ) ); // "You cannot challenge yourself."
                    }
                    else
                    {
                        bool enabled = XmlPointsAttach.TeleportOnDuel &&
                                       SpellHelper.CheckTravel( from.Map, from.Location, TravelCheckType.RecallFrom ) &&
                                       SpellHelper.CheckTravel( pm.Map, pm.Location, TravelCheckType.RecallFrom );

                        if( enabled )
                        {
                            // send the confirmation gump to the challenged player
                            from.SendGump( new IssueChallengeGump( m_Challenger, pm ) );
                        }
                        else
                            from.SendMessage( "You cannot challenge anyone here." );
                    }
                }
                else
                {
                    from.SendMessage( XmlPointsAttach.SystemText( 100213 ) ); // "No XmlPoints support."
                }
            }
        }
    }
}