/***************************************************************************
 *                               AcademySystemCommands.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.Academies
{
    public class AcademySystemCommands
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "SetAcademy", TestCenter.Enabled ? AccessLevel.Player : AccessLevel.Seer, SetAcademy_OnCommand );
            CommandSystem.Register( "KickAcademy", TestCenter.Enabled ? AccessLevel.Player : AccessLevel.Seer, KickAcademy_OnCommand );
        }

        [Usage( "SetAcademy <newAcademy>" )]
        [Description( "Set Academy of Target Player." )]
        public static void SetAcademy_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( e.Length == 1 )
            {
                AcademySystem system = AcademySystem.Parse( e.GetString( 0 ) );
                if( system != null )
                    from.Target = new InternalTarget( system, ActionType.Set );
                else
                    from.SendMessage( "That is not a valid Midgard Academy (\"SerpentsHoldAcademy\")." );
            }
            else
                from.SendMessage( "Command Use: [SetAcademy <newAcademy>" );
        }

        [Usage( "KickAcademy" )]
        [Description( "Remove AcademyState from Target Player." )]
        public static void KickAcademy_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( e.Length == 0 )
                from.Target = new InternalTarget( null, ActionType.Kick );
            else
                from.SendMessage( "Command Use: [KickAcademy" );
        }

        private enum ActionType
        {
            Kick,
            Set,
        }

        private class InternalTarget : Target
        {
            private readonly AcademySystem m_Academy;
            private readonly ActionType m_Action;

            public InternalTarget( AcademySystem newAcademy, ActionType action )
                : base( 10, false, TargetFlags.None )
            {
                m_Academy = newAcademy;
                m_Action = action;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted is Midgard2PlayerMobile )
                {
                    Midgard2PlayerMobile m = (Midgard2PlayerMobile)targeted;

                    switch( m_Action )
                    {
                        case ActionType.Kick:
                            DoPlayerReset( m );
                            break;
                        case ActionType.Set:
                            DoSetAcademy( m, m_Academy );
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static void DoPlayerReset( Mobile mobile )
        {
            if( mobile == null || mobile.Deleted )
                return;

            AcademyPlayerState tps = AcademyPlayerState.Find( mobile );
            if( tps != null )
                tps.Detach();

            mobile.InvalidateProperties();
        }

        public static void DoSetAcademy( Mobile mobile, AcademySystem system )
        {
            if( mobile == null || mobile.Deleted )
                return;

            if( system != null )
            {
                if( system.Candidates.Contains( mobile ) )
                    system.Candidates.Remove( mobile );

                AcademyPlayerState tps = AcademyPlayerState.Find( mobile );
                if( tps != null )
                    tps.Detach();

                new AcademyPlayerState( system, mobile );

                system.SetStartingSkills( mobile );
            }
        }
    }
}