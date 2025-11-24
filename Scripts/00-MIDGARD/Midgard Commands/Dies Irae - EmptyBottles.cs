/***************************************************************************
 *                                   EmptyBottles.cs
 *                            		-----------------
 *  begin                	: Maggio, 2008
 *  version					: 2.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Commands;
using Server.Targeting;
using Server.Items;

namespace Midgard.Commands
{
    public class EmptyBottles
    {
        #region registrazione
        public static void Initialize()
        {
            CommandSystem.Register( "SvuotaBottiglie", AccessLevel.Player, new CommandEventHandler( EmptyBottles_OnCommand ) );
        }
        #endregion

        #region callback
        [Usage( "SvuotaBottiglie" )]
        [Description( "Svuota dal loro contenuto una o piu' bottiglie" )]
        public static void EmptyBottles_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 0 )
            {
                from.Target = new InternalTarget();
            }
            else
            {
                from.SendMessage( "Uso del comando: SvuotaBottiglie" );
            }
        }
        #endregion

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base( 0, false, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                try
                {
                    if( targeted is BasePotion )
                    {
                        BasePotion potion = (BasePotion)targeted;
                        Container pack = from.Backpack;

                        if( potion.IsChildOf( pack ) )
                        {
                            Bottle bottle = new Bottle( potion.Amount );

                            if( pack == null || !pack.TryDropItem( from, bottle, true ) )
                            {
                                bottle.MoveToWorld( from.Location, from.Map );
                            }

                            potion.Delete();

                            from.SendMessage( "Hai svuotato con successo l{0} boccett{0}.", potion.Amount > 1 ? "e" : "a" );

                            if( bottle.Amount >= 10 )
                            {
                                Blood pool = new Blood( Utility.Random( 0x122A, 5 ) );
                                pool.Name = "a pool of liquid";
                                pool.Hue = potion.Hue != 0 ? potion.Hue : Utility.RandomDyedHue();
                                pool.MoveToWorld( from.Location, from.Map );
                            }
                        }
                        else
                        {
                            from.SendMessage( "Devi scegliere una o piu' pozioni nel tuo zaino." );
                        }
                    }
                    else
                    {
                        from.SendMessage( "Devi scegliere una o piu' pozioni nel tuo zaino." );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex );
                }
            }
        }
    }
}