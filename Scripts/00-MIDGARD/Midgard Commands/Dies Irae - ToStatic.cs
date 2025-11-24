using System;
using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;

namespace Midgard.Commands
{
    public class ToStaticCommand : BaseCommand
    {
        public ToStaticCommand()
        {
            AccessLevel = AccessLevel.Seer;
            Supports = CommandSupport.Single | CommandSupport.Area | CommandSupport.Multi | CommandSupport.Region | CommandSupport.Global;
            Commands = new string[] { "ToStatic" };
            ObjectTypes = ObjectTypes.Items;
            Usage = "ToStatic";
            Description = "Converts items and add-ons to their static equivalents";
        }

        public static void Initialize()
        {
            TargetCommands.Register( new ToStaticCommand() );
        }

        public override void ExecuteList( CommandEventArgs args, System.Collections.ArrayList list )
        {
            for( int i = 0; i < list.Count; i++ )
            {
                if( list[ i ] is AddonComponent || list[ i ] is BaseAddon )
                    continue;

                Execute( args, list[ i ] );
            }
        }

        public override void Execute( CommandEventArgs args, object o )
        {
            try
            {
                if( o is AddonComponent )
                {
                    BaseAddon addon = ( (AddonComponent)o ).Addon;

                    if( addon.Components.Count > 0 )
                    {
                        foreach (AddonComponent component in addon.Components)
                        {
                            Static newStatic = new Static( component.ItemID );

                            newStatic.Hue = component.Hue;
                            newStatic.Name = component.Name;
                            newStatic.MoveToWorld( new Point3D( component.Location ), component.Map );
                        }
                    }

                    addon.Delete();
                    AddResponse( "The add-on has been converted to static objects." );
                }
                else if( o is Item && !( o is Static ) )
                {
                    Item i = (Item)o;
                    Static newItem = new Static( i.ItemID );

                    newItem.Hue = i.Hue;
                    newItem.Layer = i.Layer;
                    newItem.Light = i.Light;
                    newItem.Name = i.Name;
                    newItem.MoveToWorld( new Point3D( i.Location ), i.Map );

                    if( i.Parent == args.Mobile )
                        newItem.Bounce( args.Mobile );

                    if( i is Container )
                        ( (Container)i ).Destroy();
                    else
                        i.Delete();

                    AddResponse( "The item has been converted to a static." );
                }
                else
                {
                    LogFailure( "This command only works with non-static items or add-ons." );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( "ToStatic conversion failed: {0}", ex );
            }
        }
    }
}
