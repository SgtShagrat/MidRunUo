using System;
using Server.Commands;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.XmlPoints
{
    public class ConfirmChallengeGump : Gump
    {
        private readonly DuelLocationEntry m_DuelLocation;
        private readonly Mobile m_From;
        private readonly Mobile m_Target;

        public ConfirmChallengeGump( Mobile from, Mobile target, DuelLocationEntry duelloc )
            : base( 0, 0 )
        {
            if( target == null || from == null )
                return;

            CommandLogging.WriteLine( from, "{0} {1} challenged {2}", from.AccessLevel, CommandLogging.Format( from ),
                                      CommandLogging.Format( target ) );

            m_From = from;
            m_Target = target;
            m_DuelLocation = duelloc;

            var a = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );

            var atarg = (XmlPointsAttach)XmlAttach.FindAttachment( target, typeof( XmlPointsAttach ) );

            Closable = false;
            Dragable = true;

            AddPage( 0 );
            AddBackground( 10, 200, 200, 150, 5054 );

            AddLabel( 20, 205, 68, String.Format( XmlPointsAttach.GetText( target, 200259 ) ) ); // "You have been challenged by"
            AddLabel( 20, 225, 68, String.Format( XmlPointsAttach.GetText( target, 200260 ), from.Name ) ); // "{0}. Accept?"

            int y = 250;
            if( m_DuelLocation != null )
            {
                AddLabel( 20, y, 0, String.Format( XmlPointsAttach.GetText( target, 200649 ), m_DuelLocation.Name ) );
                // "Location: {0}"
            }
            else
            {
                AddLabel( 20, y, 0,
                          String.Format( XmlPointsAttach.GetText( target, 200649 ), XmlPointsAttach.GetText( target, 200661 ) ) );
                // "Location: Duel Here"
            }
            y += 20;


            if( a == null || a.Deleted || atarg == null || atarg.Deleted ||
                !atarg.CanAffectPoints( target, target, from, true ) )
            {
                AddLabel( 20, y, 33, String.Format( XmlPointsAttach.GetText( target, 200256 ) ) );
                // "You will NOT gain points!"
            }

            AddRadio( 35, 290, 9721, 9724, false, 1 ); // accept/yes radio
            AddRadio( 135, 290, 9721, 9724, true, 2 ); // decline/no radio
            AddHtmlLocalized( 72, 290, 200, 30, 1049016, 0x7fff, false, false ); // Yes
            AddHtmlLocalized( 172, 290, 200, 30, 1049017, 0x7fff, false, false ); // No

            AddButton( 80, 320, 2130, 2129, 3, GumpButtonType.Reply, 0 ); // Okay button
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( info == null || state == null || state.Mobile == null || m_From == null || m_Target == null )
                return;

            int radiostate = -1;
            if( info.Switches.Length > 0 )
            {
                radiostate = info.Switches[ 0 ];
            }

            switch( info.ButtonID )
            {
                default:
                    {
                        if( radiostate == 1 )
                        {
                            // challenge accept
                            // check to make sure the duel location is available
                            if( m_DuelLocation != null && !XmlPointsAttach.DuelLocationAvailable( m_DuelLocation ) )
                            {
                                XmlPointsAttach.SendText( m_Target, 200650, m_DuelLocation.Name ); // "{0} is occupied."
                                return;
                            }

                            // make sure neither participant is in combat
                            if( XmlPointsAttach.CheckCombat( m_From ) )
                            {
                                XmlPointsAttach.SendText( m_From, 100214, m_Target.Name );
                                // "Challenge with {0} has been cancelled"
                                XmlPointsAttach.SendText( m_From, 100670, m_From.Name ); // "{0} is in combat."
                                XmlPointsAttach.SendText( m_Target, 100214, m_From.Name );
                                // "Challenge with {0} has been cancelled"
                                XmlPointsAttach.SendText( m_Target, 100670, m_From.Name ); // "{0} is in combat."
                                return;
                            }

                            // make sure neither participant is in combat
                            if( XmlPointsAttach.CheckCombat( m_Target ) )
                            {
                                XmlPointsAttach.SendText( m_From, 100214, m_Target.Name );
                                // "Challenge with {0} has been cancelled"
                                XmlPointsAttach.SendText( m_From, 100670, m_Target.Name ); // "{0} is in combat."
                                XmlPointsAttach.SendText( m_Target, 100214, m_From.Name );
                                // "Challenge with {0} has been cancelled"
                                XmlPointsAttach.SendText( m_Target, 100670, m_Target.Name ); // "{0} is in combat."
                                return;
                            }

                            var a = (XmlPointsAttach)XmlAttach.FindAttachment( m_From, typeof( XmlPointsAttach ) );

                            // first confirm that they dont already have a challenge going
                            if( a != null && !a.Deleted && ( a.Challenger != null || a.ChallengeGame != null ) )
                            {
                                XmlPointsAttach.SendText( m_Target, 100261, m_From.Name );
                                // "{0} has already been challenged."

                                XmlPointsAttach.SendText( m_From, 100262 ); // "You are already being challenged."
                                return;
                            }

                            var ta = (XmlPointsAttach)XmlAttach.FindAttachment( m_Target, typeof( XmlPointsAttach ) );

                            // first confirm that they dont already have a challenge going
                            if( ta != null && !ta.Deleted && ( ta.Challenger != null || ta.ChallengeGame != null ) )
                            {
                                XmlPointsAttach.SendText( m_Target, 100262 ); // "You are already being challenged."

                                XmlPointsAttach.SendText( m_From, 100261, m_Target.Name );
                                // "{0} has already been challenged."
                                return;
                            }

                            // if they accept then assign the challenger fields on their points attachments
                            if( a != null && !a.Deleted )
                            {
                                a.Challenger = m_Target;
                            }

                            // assign the challenger field on the target points attachment
                            if( ta != null && !ta.Deleted )
                            {
                                ta.Challenger = m_From;
                            }

                            // notify the challenger and set up noto
                            XmlPointsAttach.SendText( m_From, 100263, m_Target.Name ); // "{0} accepted your challenge!"
                            m_From.Send( new MobileMoving( m_Target, Notoriety.Compute( m_From, m_Target ) ) );

                            // update the points gump if it is open
                            if( m_From.HasGump( typeof( PointsGump ) ) )
                            {
                                // redisplay it with the new info
                                if( a != null && !a.Deleted )
                                    a.OnIdentify( m_From );
                            }

                            // notify the challenged and set up noto
                            XmlPointsAttach.SendText( m_Target, 100264, m_From.Name );
                            // "You have accepted the challenge from {0}!"
                            m_Target.Send( new MobileMoving( m_From, Notoriety.Compute( m_Target, m_From ) ) );

                            // update the points gump if it is open
                            if( m_Target.HasGump( typeof( PointsGump ) ) )
                            {
                                // redisplay it with the new info
                                if( ta != null && !ta.Deleted )
                                    ta.OnIdentify( m_Target );
                            }

                            // cancel any precast spells
                            m_Target.Spell = null;
                            m_Target.Target = null;
                            m_From.Spell = null;
                            m_From.Target = null;

                            // let the challenger pick the dueling site
                            if( XmlPointsAttach.TeleportOnDuel && m_DuelLocation != null )
                            {
                                if( a != null )
                                {
                                    a.StartingLoc = m_From.Location;
                                    a.StartingMap = m_From.Map;
                                }
                                if( ta != null )
                                {
                                    ta.StartingLoc = m_Target.Location;
                                    ta.StartingMap = m_Target.Map;
                                }

                                m_Target.MoveToWorld( m_DuelLocation.FirstChallenger, m_DuelLocation.Map );
                                m_From.MoveToWorld( m_DuelLocation.SecondChallenger, m_DuelLocation.Map );
                            }
                            else
                            {
                                if( a != null )
                                    a.StartingMap = null;
                                if( ta != null )
                                    ta.StartingMap = null;
                            }
                        }
                        else
                        {
                            XmlPointsAttach.SendText( m_From, 100265, m_Target.Name ); // "Your challenge to {0} was declined."
                            XmlPointsAttach.SendText( m_Target, 100266, m_From.Name ); // "You declined the challenge by {0}."
                        }
                        break;
                    }
            }
        }
    }
}