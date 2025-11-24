/***************************************************************************
 *                                  AdvancedSay.cs
 *                            		-------------------
 *  begin                	: Ottobre, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 * 		Il comando [AdvSay [<Colore> <Font>] <Stringa>
 * 		fa apparire un messaggio (<Stringa>) sull'oggetto o mobile selezionato.
 * 		Possono essere specificati anche colore e font.
 * 
 * 		Il comando [AdvBCArea <Raggio> [<Colore> <Font>] <Stringa>
 * 		bcasta un messaggio (<Stringa>) a tutti i mobiles nel raggio <Raggio>.
 * 		Possono essere specificati anche colore e font.
 *  
 ***************************************************************************/

using System;
using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Network;

namespace Midgard.Commands
{
	public class MidgardUtilities
	{
		#region registrazione
		public static void Initialize()
		{
			TargetCommands.Register( new AdvancedSayCommand() );
			TargetCommands.Register( new AdvancedBroadcastAreaCommand() );
		}
		#endregion

		public class AdvancedSayCommand : BaseCommand
		{
			public AdvancedSayCommand()
			{
				AccessLevel = AccessLevel.Counselor;
				Supports = CommandSupport.Area | CommandSupport.Region | CommandSupport.Global | CommandSupport.Multi | CommandSupport.Single;
				Commands = new string[]{ "AdvancedSay", "as" };
				ObjectTypes = ObjectTypes.Both;
				Usage = "AdvSay [<Colore> <Font>] <Stringa>";
				Description = "Pronuncia un messaggio su un oggetto o mobile.";
			}
			
			public override void Execute( CommandEventArgs e, object obj )
			{
				int Colore = 0x3b2;		// Valore di default
				int Font = 3; 			// Valore di default
				string Frase = String.Empty;

				Mobile From = e.Mobile;
				if( From == null  )
					return;
				
				if( e.Length == 1 )
  				{
					Frase = e.GetString( 0 );	  			
				}
				else if( e.Length == 3 )
				{
					Colore = e.GetInt32( 0 );
					Font = e.GetInt32( 1 );
					Frase = e.GetString( 2 );
				}
				
				CommandLogging.WriteLine( From, "{0} {1} ha usato AdvSay con la frase \"{2}\".", From.AccessLevel, CommandLogging.Format( From ), e.ArgString );
				AddResponse( String.Format( "Hai usato AdvancedSay. Colore: {0}; Font: {1}; Frase: \"{2}\"", Colore, Font, Frase ) );
				
				if( obj is Mobile )
               		Server.Mobiles.BaseXmlSpawner.PublicOverheadMobileMessage( (Mobile)obj, MessageType.Regular, Colore, Font, Frase, true );
                else if( obj is Item )
                	Server.Mobiles.BaseXmlSpawner.PublicOverheadItemMessage( (Item)obj, MessageType.Regular, Colore, Font, Frase );
			}
			
			public override bool ValidateArgs( BaseCommandImplementor impl, CommandEventArgs e )
			{
				if ( e.Arguments.Length == 1 || e.Arguments.Length == 3 )
					return true;
				
				e.Mobile.SendMessage( "Usage: " + Usage );
				return false;
			}
		}
		
		public class AdvancedBroadcastAreaCommand : BaseCommand
		{
			public AdvancedBroadcastAreaCommand()
			{
				AccessLevel = AccessLevel.GameMaster;
				Supports = CommandSupport.Multi | CommandSupport.Single;
				Commands = new string[]{ "AdvBCArea", "abca" };
				ObjectTypes = ObjectTypes.Both;
				Usage = "AdvBCArea <Raggio> [<Colore> <Font>] <Stringa>";
				Description = "Pronuncia un messaggio nel raggio dato a partire dall'oggetto o mobile selezionato";
			}
			
			public override void Execute( CommandEventArgs e, object obj )
			{
				int Raggio = 10;
				int Colore = 0x3b2;		// Valore di default
				int Font = 3; 			// Valore di default
				string Frase = String.Empty;

				Mobile From = e.Mobile;
				if( From == null  )
					return;
				
				if( e.Length == 2 )
  				{
					Raggio = e.GetInt32( 0 );
					Frase = e.GetString( 1 );	  			
				}
				else if( e.Length == 4 )
				{
					Raggio = e.GetInt32( 0 );
					Colore = e.GetInt32( 1 );
					Font = e.GetInt32( 2 );
					Frase = e.GetString( 3 );
				}
				
				CommandLogging.WriteLine( From, "{0} {1} ha usato AdvBCArea con la frase \"{2}\" in un raggio di {3}.", From.AccessLevel, CommandLogging.Format( From ), e.ArgString, Raggio );
				AddResponse( String.Format( "Hai usato AdvBCArea. Raggio: {0}; Colore: {1}; Font: {2}; Frase: \"{3}\"", Raggio, Colore, Font, Frase ) );
				
				IPooledEnumerable eable;
				if( obj is Mobile )
				{
					eable = ((Mobile)obj).Map.GetMobilesInRange( ((Mobile)obj).Location, Raggio);
				}
                else if( obj is Item )
                {
                	eable = ((Item)obj).Map.GetMobilesInRange( ((Item)obj).Location, Raggio);
                }
                else
                {
					eable = From.Map.GetMobilesInRange( From.Location, Raggio);
                	AddResponse( "Errore nel selezionare il centro del bcast: prendo te come centro." );
                }
	
	            foreach( Mobile trg in eable )
	            {
	             	if( trg != null )
	            	{
	            		if( trg != From )
	            		{  
	            			trg.Send(new AsciiMessage(Serial.MinusOne, -1, MessageType.Regular, Colore, Font, "System", Frase));
	            		}
	             	}
                }
			}
			
			public override bool ValidateArgs( BaseCommandImplementor impl, CommandEventArgs e )
			{
				if ( e.Arguments.Length == 2 || e.Arguments.Length == 4 )
					return true;
				
				e.Mobile.SendMessage( "Usage: " + Usage );
				return false;
			}
		}
	}
}

