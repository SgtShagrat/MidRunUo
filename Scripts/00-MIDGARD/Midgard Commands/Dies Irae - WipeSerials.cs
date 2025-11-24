/***************************************************************************
 *                                  .cs
 *                            		-------------------
 *  begin                	: Mese, 2000
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
using System.IO;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Commands
{
	public class WipeSerials
	{
		public static readonly string FileName = "Serials.txt";
		
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "WipeSerials" , AccessLevel.Developer, new CommandEventHandler( WipeSerials_OnCommand ) );
		}
		#endregion
	
		#region callback
		[Usage( "[WipeSerials" )]
		[Description( "Wipe all items which serial is in SerialsToWipe.txt" )]
		public static void WipeSerials_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null )
				return;
			
			string filePath = Path.Combine( Core.BaseDirectory, FileName );
			
			if( File.Exists( filePath ) )
			{
				using ( StreamWriter tw = new StreamWriter( "Logs/LogWipeSerialsSuccess.log", true ) )
				{
					tw.WriteLine( "<<< Wipe Serials initialized... >>>" );	

					using ( StreamWriter tw2 = new StreamWriter( "Logs/LogWipeSerialsUnSuccess.log", true ) )
					{
						tw2.WriteLine( "<<< Wipe Serials initialized... >>>" );	

						using ( StreamReader op = new StreamReader( filePath ) )
						{
							do
							{
								string lineRead = op.ReadLine();
								if( lineRead == null )
									break;

								if( lineRead.StartsWith( "#" ) )
									continue;
								else if( lineRead.Contains( "#" ) )
									lineRead = lineRead.Substring( lineRead.LastIndexOf( "#" ) );

								Serial s = new Serial();
								s = (Serial)Utility.ToInt32( lineRead ); 

								if( s.IsValid )
								{
									Item i = World.FindItem( s );

									if( i != null )
									{
										Type t = i.GetType();
										int itemId = i.ItemID;

										string root = string.Empty;
										object o = i.RootParent;
										if( o != null )
										{
											if( o is Item )
												root = ((Item)o).Name;
											else if( o is Mobile )
												root = ((Mobile)o).Name;

											if( o is PlayerMobile )
											{
												PlayerMobile p = (PlayerMobile)o;
												root = String.Format( "{0} ( Account {1} - Serial {2} - Location {3} - Map {4} )",
												                     p.Name, p.Account.Username, p.Serial.ToString(), p.Location.ToString(), p.Map.Name );
											}
										}

										if( !i.Movable )
										{
											tw2.WriteLine( "Object NOT deleted because NOT MOVABLE.\nType: {0} Serial: {1} Itemid: {2} Location {3}", t.Name, s.ToString(), itemId.ToString(), i.Location.ToString() );
											continue;
										}

										try
										{
											i.Delete();
										}	
										catch( Exception ex )
										{
											tw2.WriteLine( "Exception from WipeSerials : {0}", ex.ToString());
											Console.WriteLine( "Exception from WipeSerials : {0}", ex);
										}

										if( i.Deleted )
										{
											tw.WriteLine( "Object deleted successfully. Type: {0} Serial: {1} Itemid: {2} RootParent {3}", t.Name, s.ToString(), itemId.ToString(), root );
										}
										else
										{
											tw2.WriteLine( "Deleting object failed. Type: {0} Serial: {1}", t.Name, s.ToString(), itemId.ToString() );
										}
									}
									else
									{
										tw2.WriteLine( "Serial {0} is valid but item is not found. ", s.ToString());								
									}
								}
								else
								{
									tw2.WriteLine( "Serial not valid: {0}", s.ToString());							
								}
							}
							while( true );
						}
					}
				}
			}
			else
			{
				from.SendMessage( "File \"{0}\" not found in RunUO Core directory.", FileName );
			}
		}
		#endregion
	}
}
