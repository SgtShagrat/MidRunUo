/***************************************************************************
 *                               LockCommand.cs
 *
 *   begin                : 14 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
using Server.Network;

namespace Midgard.Commands
{
    public class LockCommand : BaseCommand
    {
        public LockCommand()
        {
            AccessLevel = AccessLevel.Seer;
            Supports = CommandSupport.Single | CommandSupport.Area | CommandSupport.Multi | CommandSupport.Region | CommandSupport.Global;
            Commands = new string[] { "Lock" };
            ObjectTypes = ObjectTypes.Items;
            Usage = "Lock";
            Description = "Lock an lockable item.";
        }

        public static void Initialize()
        {
            TargetCommands.Register( new LockCommand() );
        }

        public override void ExecuteList( CommandEventArgs args, System.Collections.ArrayList list )
        {
            foreach( object t in list )
            {
                if( t is ILockable )
                    Execute( args, t );
            }
        }

        public override void Execute( CommandEventArgs args, object o )
        {
            try
            {
                ILockable lockable = o as ILockable;
                if( lockable == null )
                    return;

                // get a random key value
                uint value = Key.RandomValue();

                Mobile from = args.Mobile;

                lockable.Locked = true;
                lockable.KeyValue = value;

                Key key;

                if( o is BaseDoor )
                {
                    BaseDoor door = (BaseDoor)o;

                    // set the linked door
                    if( door.Link != null )
                    {
                        door.Link.KeyValue = value;
                        door.Link.Locked = true;
                    }

                    key = new Key( KeyType.Gold, value, door.Link );
                    key.Name = String.Format( "key for a door at {0}", door.Location );
                }
                else
                {
                    Item i = (Item)o;

                    key = new Key( KeyType.Gold, value );

                    if( i != null )
                        key.Name = String.Format( "key for a {0} at {1}", MidgardUtility.GetFriendlyClassName( i.GetType().Name ), i.Location );
                    else
                        key.Name = String.Format( "key for an object at {0}", from.Location );
                }

                if( from.AddToBackpack( key ) )
                    from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "A key has been placed in your backpack" );
                else
                    from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "A key has been placed at your feet." );
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Lock command failed: {0}", ex );
            }
        }
    }
}