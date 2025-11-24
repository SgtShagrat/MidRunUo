using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.ThirdCrownPorting
{
    public class GetSerial
    {
        public static void RegisterCommand()
        {
            CommandSystem.Register( "CodiceUnico", AccessLevel.Player, new CommandEventHandler( GetSerial_OnCommand ) );
        }

        [Usage( "CodiceUnico" )]
        [Description( "Ritorna il codice unico di un uggetto o di una creatura." )]
        public static void GetSerial_OnCommand( CommandEventArgs e )
        {
            if( e == null || e.Mobile == null )
                return;

            e.Mobile.Target = new GetSerialTarget();
        }

        public class GetSerialTarget : Target
        {
            public GetSerialTarget()
                : base( 30, true, TargetFlags.None )
            {
                CheckLOS = false;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( from == null || targeted == null )
                    return;

                if( targeted is Item )
                {
                    Item item = targeted as Item;
                    if( !item.IsChildOf( from.Backpack ) )
                    {
                        from.SendMessage( "L'oggetto deve essere nel tuo zaino per essere scelto." );
                    }
                    else if( !item.IsAccessibleTo( from ) )
                    {
                        from.SendMessage( "L'oggetto non e' accessibile." );
                    }
                    else
                    {
                        from.SendMessage( "L'oggetto ha tipo: {0}", item.GetType().Name );
                        from.SendMessage( "L'oggetto ha nome: {0}", item.Name ?? "nessuno" );
                        from.SendMessage( "Il codice identificativo unico dell'oggetto e': 0x{0:X4}", item.Serial.Value );
                    }
                }
                else if( targeted is BaseCreature )
                {
                    BaseCreature creature = targeted as BaseCreature;
                    if( !creature.Controlled )
                    {
                        from.SendMessage( "La creatura non e' tamata." );
                    }
                    else if( creature.ControlMaster != from )
                    {
                        from.SendMessage( "Non controlli la creatura." );
                    }
                    else
                    {
                        from.SendMessage( "La creatura ha tipo: {0}", creature.GetType().Name );
                        from.SendMessage( "La creatura ha nome: {0}", creature.Name ?? "nessuno" );
                        from.SendMessage( "Il codice identificativo unico dell'oggetto e': 0x{0:X4}", creature.Serial.Value );
                    }
                }
                else
                {
                    from.SendMessage( "Non hai scelto ne un oggetto ne una creatura valida." );
                }
            }
        }
    }
}