/***************************************************************************
 *                               BiteCommand.cs
 *                            -------------------
 *   begin                : 21 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;
using Server;
using Server.Commands;
using Server.Items;
using Server.Targeting;

namespace Midgard.Engines.Races
{
    public class BiteCommand
    {
        public const double DelayBite = 5.0;

        public static void RegisterCommands()
        {
            CommandSystem.Register( "Morso", AccessLevel.Player, new CommandEventHandler( Bite_OnCommand ) );
        }

        [Usage( "Morso" )]
        [Description( "Morde la vittima" )]
        public static void Bite_OnCommand( CommandEventArgs e )
        {
            if( !Config.BiteEnabled )
            {
                e.Mobile.SendMessage( "Questo potere e' stato disabilitato." );
                return;
            }

            if( !e.Mobile.Alive )
            {
                e.Mobile.SendMessage( "Non puoi usare questo comando in questo stato." );
                return;
            }

            Mobile from = e.Mobile;

            if( from != null )
            {
                if( e.Length == 0 )
                {
                    if( from.Race == Core.Vampire )
                    {
                        if( from.CanBeginAction( typeof( BiteCommand ) ) )
                        {
                            from.Target = new BiteTarget( from );
                            from.SendMessage( "Vampiro: scegli chi mordere." );
                        }
                        else
                        {
                            from.SendMessage( "Non puoi usare ancora questo potere." );
                        }
                    }
                    else
                    {
                        from.SendMessage( "Non puoi usare questo potere. Solo i vampiri possono." );
                    }
                }
                else
                {
                    from.SendMessage( "Uso del comando: [Morso" );
                }
            }
        }

        public class BiteTarget : Target
        {
            private Mobile m_From;

            public BiteTarget( Mobile from )
                : base( 10, false, TargetFlags.Harmful )
            {
                m_From = from;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is Mobile )
                    DoBiteTarget( from, (Mobile)o );
            }
        }

        public static void DoBiteTarget( Mobile from, Mobile target )
        {
            try
            {
                if( !from.CanSee( target ) )
                {
                    from.SendLocalizedMessage( 500237 ); // Target can not be seen.
                }
                else if( !target.Alive || target.Race == Core.Vampire || target.Race == Core.Undead )
                {
                    from.SendMessage( "Non e' una creatura valida da mordere." );
                }
                else if( !Utility.InRange( target.Location, from.Location, 1 ) )
                {
                    from.SendMessage( "E' troppo lontano." );
                }
                else if( from.CanBeHarmful( target ) )
                {
                    from.BeginAction( typeof( BiteCommand ) );

                    if( from.Hidden )
                        from.RevealingAction();

                    // from.DoHarmful( target );
                    target.Paralyze( TimeSpan.FromSeconds( 10.0 ) );

                    target.PlaySound( 0x204 );
                    target.FixedEffect( 0x376A, 6, 1 );
                    target.PublicOverheadMessage( Server.Network.MessageType.Regular, 0x3B2, true, "* Vieni morso al collo *" );

                    Blood blood = new Blood();
                    blood.ItemID = Utility.Random( 0x122A, 5 );
                    blood.MoveToWorld( target.Location, target.Map );

                    from.Hunger = 20;
                    from.Thirst = 20;
                    from.SendMessage( "Ti sazi del sangue della tua vittima!" );

                    FoodDecaySystem.FoodDecayTimer.ComputeFoodStatLoss( from );

                    Timer.DelayCall( TimeSpan.FromSeconds( DelayBite ), new TimerStateCallback( ReleaseSpecialCommandsLock ), from );
                }
            }
            catch( Exception ex )
            {
                TextWriter tw = File.AppendText( "Logs/RaceErrors.log" );
                tw.WriteLine( "Warning: error in DoBiteTarget command." );
                tw.WriteLine( ex.ToString() );
                tw.WriteLine( "" );
                tw.Close();
            }
        }

        private static void ReleaseSpecialCommandsLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( BiteCommand ) );
            ( (Mobile)state ).SendMessage( "Puoi usare di nuovo il tuo potere razziale." );
        }
    }
}