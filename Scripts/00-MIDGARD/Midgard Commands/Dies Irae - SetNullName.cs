/***************************************************************************
 *                                   SetNullName.cs
 *                            		----------------
 *  begin                	: Settembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Setta a null il nome di un target.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Commands;
using Server.Commands.Generic;

namespace Midgard.Commands
{
	public class SetNullName
	{
		public static void Initialize()
		{
			TargetCommands.Register( new SetNullNameCommand() );
			TargetCommands.Register( new FixCapitalizedNameCommand() );
            TargetCommands.Register( new GetSerialCommand() );
		}

		public class SetNullNameCommand : BaseCommand
		{
			public SetNullNameCommand()
			{
				AccessLevel = AccessLevel.Developer;
				Supports = CommandSupport.AllItems;
				ObjectTypes = ObjectTypes.Items;			
	
				Commands = new string[]{ "SetNullName" };
				Usage = "SetNullName";
				Description = "Set target name to null value.";
			}

			public override void Execute( CommandEventArgs e, object obj )
			{
				Item target = (Item)obj;

				if( target != null )
				{
					if( target.Name != null )
					{
						target.Name = null;
						AddResponse( "Their name is now -null-." );
					}
					else
					{
						LogFailure( "Their name is already -null-." );
					}
				}
				else
				{
					LogFailure( "Error in setting null name." );
				}
			}
		}

		public class FixCapitalizedNameCommand : BaseCommand
		{
			public FixCapitalizedNameCommand()
			{
				AccessLevel = AccessLevel.Developer;
				Supports = CommandSupport.AllItems;
				ObjectTypes = ObjectTypes.Items;			
	
				Commands = new string[]{ "FixCapitalizedName" };
				Usage = "FixCapitalizedName";
				Description = "Remove uppercase chars from an item name.";
			}

			public override void Execute( CommandEventArgs e, object obj )
			{
				Item target = (Item)obj;

				if( target != null )
				{
					if( target.Name != null )
					{
						string tmp = target.Name.ToLower();
						target.Name = tmp;
						AddResponse( "Their name is now all in lowercase." );
					}
					else
					{
						LogFailure( "Their name is -null-." );
					}
				}
				else
				{
					LogFailure( "Error fixing uppercase name." );
				}
			}
		}

		public class GetSerialCommand : BaseCommand
		{
			public GetSerialCommand()
			{
				AccessLevel = AccessLevel.Counselor;
				Supports = CommandSupport.All;
				ObjectTypes = ObjectTypes.Both;			
	
				Commands = new string[]{ "VerificaSeriale" };
				Usage = "VerificaSeriale";
				Description = "Ottiene il codice seriale di un oggetto o di un mobile.";
			}

			public override void Execute( CommandEventArgs e, object obj )
			{
                string serial = string.Empty;

                if( obj is Item )
                    serial = ( (Item)obj ).Serial.ToString();
                else if( obj is Mobile )
                    serial = ( (Mobile)obj ).Serial.ToString();

				if( !String.IsNullOrEmpty( serial ) )
				{
                    e.Mobile.SendMessage( "Il seriale cercato è {0}", serial );
				}
				else
				{
					LogFailure( "Errore: l'oggetto cercato non ha Seriale." );
				}
			}
		}
	}
}