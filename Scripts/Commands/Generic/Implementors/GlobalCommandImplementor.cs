using System;
using System.Collections;
using Server;

namespace Server.Commands.Generic
{
	public class GlobalCommandImplementor : BaseCommandImplementor
	{
		public GlobalCommandImplementor()
		{
			Accessors = new string[]{ "Global" };
			SupportRequirement = CommandSupport.Global;
			SupportsConditionals = true;
			AccessLevel = AccessLevel.Administrator;
			Usage = "Global <command> [condition]";
			Description = "Invokes the command on all appropriate objects in the world. Optional condition arguments can further restrict the set of objects.";
		}

		public override void Compile( Mobile from, BaseCommand command, ref string[] args, ref object obj )
		{
			try
			{
				Extensions ext = Extensions.Parse( from, ref args );

				bool items, mobiles;

				if ( !CheckObjectTypes( command, ext, out items, out mobiles ) )
					return;

				ArrayList list = new ArrayList();

				if ( items )
				{
					foreach ( Item item in World.Items.Values )
					{
						#region modifica by Dies Irae
                        if ( ext.IsValid( item ) && BaseCommand.IsAccessible( from, item, command ) )
							list.Add( item );
						#endregion
					}
				}

				if ( mobiles )
				{
					foreach ( Mobile mob in World.Mobiles.Values )
					{
						#region modifica by Dies Irae
                        if ( ext.IsValid( mob ) && BaseCommand.IsAccessible( from, mob, command ) )
							list.Add( mob );
						#endregion
					}
				}

				ext.Filter( list );

				obj = list;
			}
			catch ( Exception ex )
			{
				from.SendMessage( ex.Message );
			}
		}
	}
}