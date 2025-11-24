/***************************************************************************
 *                                  ShrinkCommands.cs
 *                            		--------------
 *  begin                	: Febbraio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Commands
{
    public class ShrinkCommands
    {
        #region registrazione
        public static void Initialize()
        {
            CommandSystem.Register( "Shrink", AccessLevel.GameMaster, new CommandEventHandler( Shrink_OnCommand ) );
            CommandSystem.Register( "ShrinkLockDown", AccessLevel.Administrator, new CommandEventHandler( ShrinkLockDown_OnCommand ) );
            CommandSystem.Register( "ShrinkRelease", AccessLevel.Administrator, new CommandEventHandler( ShrinkRelease_OnCommand ) );
        }
        #endregion

        #region callbacks
        [Usage( "ShrinkLockDown" )]
        [Description( "Disables all shrinkitems in the world." )]
        private static void ShrinkLockDown_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                int shrinkCount = 0;

                foreach( Item item in World.Items.Values )
                {
                    if( item is ShrinkItem )
                    {
                        shrinkCount += 1;
                    }
                }

                e.Mobile.SendMessage( "{0} ShrinkItems have been disabled.", shrinkCount );
                World.Broadcast( 0x35, true, "A server wide shrinkitem lockout has initiated" );
                World.Broadcast( 0x35, true, "All shrunken pets have been disabled untill further notice." );

                foreach( Item item in World.Items.Values )
                {
                    if( item is ShrinkItem )
                    {
                        ShrinkItem si = (ShrinkItem)item;
                        si.Disabled = true;
                    }
                }
            }
        }

        [Usage( "ShrinkRelease" )]
        [Description( "Re-enables all disabled shrink items in the world." )]
        private static void ShrinkRelease_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                int shrinkCount = 0;

                foreach( Item item in World.Items.Values )
                {
                    if( item is ShrinkItem )
                    {
                        shrinkCount += 1;
                    }
                }

                e.Mobile.SendMessage( "{0} ShrinkItems have been enabled.", shrinkCount );
                World.Broadcast( 0x35, true, "The server wide shrinkitem lockout has been lifted." );
                World.Broadcast( 0x35, true, "You may once again unshrink all shrunken pets." );

                foreach( Item item in World.Items.Values )
                {
                    if( item is ShrinkItem )
                    {
                        ShrinkItem si = (ShrinkItem)item;
                        si.Disabled = false;
                    }
                }
            }
        }

        [Usage( "Shrink" )]
        [Description( "Shrinks a creature." )]
        private static void Shrink_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                e.Mobile.Target = new ShrinkTarget();
                e.Mobile.SendLocalizedMessage( 1064342 ); // What do you wish to shrink?
            }
        }

        private class ShrinkTarget : Target
        {

            public ShrinkTarget()
                : base( -1, true, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is Item )
                    from.SendLocalizedMessage( 1064358 ); // That cannot be shrunken.
                else if( o is PlayerMobile )
                    from.SendLocalizedMessage( 1064358 ); // You must be near an animaltrainer to un-shrink your pet.
                else if( o is BaseCreature )
                {
                    BaseCreature c = (BaseCreature)o;
                    c.Shrink();
                }
                else
                {
                    from.SendLocalizedMessage( 1064358 ); // That cannot be shrunken.
                }
            }
        }
        #endregion
    }
}
