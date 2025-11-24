/***************************************************************************
 *                                  PlayerResume.cs
 *                            		-------------------
 *  begin                	: Novembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Resume a player to targe account from an older save
 * 
 ***************************************************************************/

using System;
using System.IO;
using Server;
using Server.Accounting;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Commands
{
	public class PlayerResume
	{
		public static void Initialize()
		{
			CommandSystem.Register( "PlayerResume" , AccessLevel.Developer, new CommandEventHandler( PlayerResume_OnCommand ) );
		}

		public readonly static string OlderMobileIndexPath = Path.Combine( "OlderSave/Mobiles/", "Mobiles.idx" );
		public readonly static string OlderMobileTypesPath = Path.Combine( "OlderSave/Mobiles/", "Mobiles.tdb" );
		public readonly static string OlderMobileDataPath = Path.Combine( "OlderSave/Mobiles/", "Mobiles.bin" );

		[Usage( "PlayerResume <serial> <account>" )]
		[Description( "Resume a player from an older save" )]
		public static void PlayerResume_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			if( from == null || from.Deleted )
				return;

			if( e.Arguments.Length != 2 )
			{
				from.SendMessage( "Command Use: PlayerResume <serial> <account>" );
				return;
			}

			Serial oldSerial = new Serial();
			int SerialNumber = Utility.ToInt32( e.Arguments[0].Trim() );
			oldSerial = (Serial)SerialNumber; 

			if( !oldSerial.IsValid )
			{
				from.SendMessage( "Serial entered is not a valid one." );
				return;
			}

			if( World.FindMobile( oldSerial ) != null )
			{
				from.SendMessage( "Serial entered is alredy present in NEWER save." );
				return;
			}

			Account acct = Accounts.GetAccount( e.Arguments[1].Trim() ) as Account;
			if( acct == null )
			{
				from.SendMessage( "Account entered is not a valid account or does not exist." );
				return;
			}

			for( int i = 0; i < acct.Length; i++ )
			{
				if( acct[i] != null && acct[i].Serial.Equals( oldSerial ) )
				{
					from.SendMessage( "Account entered is valid but has already a player with taht Serial number!" );
					return;
				}
			}

			if( acct.Count >= acct.Length )
			{
				from.SendMessage( "Account entered is valid but has already 6 characters." );
				return;
			}

			if( !File.Exists( OlderMobileIndexPath ) || !File.Exists( OlderMobileTypesPath ) || !File.Exists( OlderMobileDataPath ) ) 
			{
				from.SendMessage( "Older save not found. May be it's on a wrong path." );
				return;
			}

			from.SendMessage( "Serial is {0}, account user is {1}, and all saved files exist.", oldSerial.ToString(), acct.Username );
			Console.WriteLine( "Started player resuming:" );
			Console.WriteLine( "\tUserId: {0}", acct.Username );
			Console.WriteLine( "\tSerial: {0}", oldSerial.ToString());

			Midgard2PlayerMobile m2pm = null;
			int typeID;
			int serial;
			long pos = 0;
			int length = 0;
			int mobileCount;

			#region index finder
			using ( FileStream idx = new FileStream( OlderMobileIndexPath, FileMode.Open, FileAccess.Read, FileShare.Read ) ) 
			{
				BinaryReader idxReader = new BinaryReader( idx );
				mobileCount = idxReader.ReadInt32();

				Console.WriteLine( "\tStarted seeking serial..." );
				bool found = false;

				for( int i = 0; i < mobileCount && found == false; ++i ) 
				{
					typeID = idxReader.ReadInt32();
					serial = idxReader.ReadInt32();
					pos = idxReader.ReadInt64();
					length = idxReader.ReadInt32();

					if( oldSerial.Equals( (Serial)serial ) )
					{
						found = true;

						Console.WriteLine( "\tSerial {0} found on older save.", oldSerial.ToString() );
						Console.WriteLine( "\ttypeID {0} | serial {1} | pos {2} | length {3}", typeID, serial, pos.ToString(), length );

						try
						{
							m2pm = new Midgard2PlayerMobile( (Serial)serial );
						}
						catch( Exception ex )
						{
							Console.WriteLine( ex.ToString() );
						}
	
						if( m2pm != null )
						{
							Console.WriteLine( "\tMobile added to World" );
							World.AddMobile( m2pm );
						}
					}
				}

				if( !found )
				{
					Console.WriteLine( "\tSerial {0} NOT found on older save.", oldSerial.ToString() );
					return;
				}

				idxReader.Close();
			}
			#endregion

			#region data reader
			using ( FileStream bin = new FileStream( OlderMobileDataPath, FileMode.Open, FileAccess.Read, FileShare.Read ) ) 
			{
				BinaryFileReader reader = new BinaryFileReader( new BinaryReader( bin ) );

				Console.WriteLine( "\tStarted reading data..." );

				if( m2pm != null ) 
				{
					reader.Seek( pos, SeekOrigin.Begin );

					try 
					{
						m2pm.Deserialize( reader );

						if( reader.Position != ( pos + length ) )
							throw new Exception( String.Format( "***** Bad serialize on {0} *****", m2pm.GetType() ) );
					}
					catch( Exception ex )
					{
						Console.WriteLine( ex.ToString() );
					}
				}

				reader.Close();
			}
			#endregion

			#region account managment
			int slot = NewSlot( acct );

			if( slot > -1 )
			{
				Console.WriteLine( "\tCharacter {0} added to {1} account", m2pm.Name, acct.Username );
				acct[slot] = m2pm;
			}
			#endregion
		}

		public static int NewSlot( Account a )
		{
			for( int i = 0; i < a.Length; i++ )
			{
				if( a[i] == null )
					return i;
			}
			return -1;
		}
	}
}
