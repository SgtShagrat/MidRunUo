using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Commands
{
    public class Attack
    {
        public static void Initialize()
        {
            CommandSystem.Register( "Attack", AccessLevel.GameMaster, new CommandEventHandler( Attack_OnCommand ) );
        }

        [Usage( "Attack" )]
        [Description( "Set the control combatant for a given creature" )]
        private static void Attack_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Target the mobile who must attack." );
            e.Mobile.Target = new SelectAggressorTarget();
        }

        private static void EndSelect( Mobile from, BaseCreature aggressor )
        {
            from.SendMessage( "Target the mobile who must be aggressed." );
            from.Target = new SelectAggressedTarget( aggressor );
        }

        private class SelectAggressorTarget : Target
        {
            public SelectAggressorTarget()
                : base( 20, false, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object targ )
            {
                if( ( targ is BaseCreature ) )
                    EndSelect( from, (BaseCreature)targ );
                else
                    from.SendMessage( "That is not a creature." );
            }
        }

        private class SelectAggressedTarget : Target
        {
            private readonly BaseCreature m_Aggressor;

            public SelectAggressedTarget( BaseCreature aggressor )
                : base( 20, false, TargetFlags.None )
            {
                m_Aggressor = aggressor;
            }

            protected override void OnTarget( Mobile from, object targ )
            {
                if( targ is BaseCreature )
                {
                    m_Aggressor.Attack( (BaseCreature)targ );
                    m_Aggressor.DebugSay( "I will attack as commanded." );
                }
                else
                    from.SendMessage( "That is not a creature." );
            }
        }
    }
}