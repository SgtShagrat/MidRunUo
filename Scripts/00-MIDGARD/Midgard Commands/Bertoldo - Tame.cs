using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Commands
{
    public class Tame
    {
        public static void Initialize()
        {
            CommandSystem.Register( "Tame", AccessLevel.GameMaster, new CommandEventHandler( Tame_OnCommand ) );
        }

        [Usage( "Tame" )]
        [Description( "Set the control master for a given creature to command user" )]
        private static void Tame_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Target the mobile you want to tame." );
            e.Mobile.Target = new TameTarget();
        }

        private static void EndTame( BaseCreature creature, Mobile from )
        {
            creature.SetControlMaster( from );
            creature.IsBonded = true;
        }

        private class TameTarget : Target
        {
            public TameTarget()
                : base( 15, false, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object targ )
            {
                if( ( targ is BaseCreature ) )
                    EndTame( (BaseCreature)targ, from );
                else
                    from.SendMessage( "That is not a creature." );
            }
        }
    }
}