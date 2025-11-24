using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Server;
using Server.Accounting;
using Server.Commands;
using Server.Engines.Help;
using Server.Network;
using Server.Regions;

namespace Server.Misc
{
	public enum PasswordProtection
	{
		None,
		Crypt,
		NewCrypt
	}

	public class AccountHandler
	{
		#region modifica by Berto
		private static int MaxAccountsPerIP = 10;
		private static bool AutoAccountCreation = TestCenter.Enabled;
		#endregion
		private static bool RestrictDeletion = !TestCenter.Enabled;
		private static TimeSpan DeleteDelay = TimeSpan.FromDays( 7.0 );

		public static PasswordProtection ProtectPasswords = PasswordProtection.NewCrypt;

		private static AccessLevel m_LockdownLevel;

		public static AccessLevel LockdownLevel
		{
			get{ return m_LockdownLevel; }
			set{ m_LockdownLevel = value; }
		}

		private static CityInfo[] StartingCities = new CityInfo[]
			{
				new CityInfo( "Yew",		"The Empath Abbey",			633,	858,	0  ),
				new CityInfo( "Minoc",		"The Barnacle",				2476,	413,	15 ),
				new CityInfo( "Britain",	"Sweet Dreams Inn",			1496,	1628,	10 ),
				new CityInfo( "Moonglow",	"The Scholars Inn",			4408,	1168,	0  ),
				new CityInfo( "Trinsic",	"The Traveler's Inn",		1845,	2745,	0  ),
				new CityInfo( "Magincia",	"The Great Horns Tavern",	3734,	2222,	20 ),
				new CityInfo( "Jhelom",		"The Mercenary Inn",		1374,	3826,	0  ),
				new CityInfo( "Skara Brae",	"The Falconer's Inn",		618,	2234,	0  ),
				new CityInfo( "Vesper",		"The Ironwood Inn",			2771,	976,	0  ),
				new CityInfo( "Haven",		"Buckler's Hideaway",		3667,	2625,	0  )
			};

        private static CityInfo[] OldStartingCities = new CityInfo[]
            {
                new CityInfo( "Darklore",       "Bloody Moon Inn",      1013,   364,    20, Map.Felucca ),
                new CityInfo( "Wolfsbane",      "Rusty Axe Inn",        1007,   1387,   0, Map.Felucca ),
                new CityInfo( "Britain",        "Sweet Dreams Inn",     1496,   1628,   10, Map.Felucca ),
                new CityInfo( "Serpent's Hold", "Knight's Rest Inn",    2977,   3407,   15, Map.Felucca ),
                new CityInfo( "Buccaneer's Den","Mute Parrot Inn",      2714,   2106,   0, Map.Felucca ),
                new CityInfo( "Calen Sul",      "Fallen Leaves Inn",    785,    1999,   0, Map.Felucca ),
                new CityInfo( "Dal Baratz",     "Golden Beer Inn",      1949,   282,    8, Map.Felucca ),
                new CityInfo( "Vinyamar",       "Crystal Keep",         3961,   405,    0, Map.Felucca ),
            };

		private static bool PasswordCommandEnabled = true;

        public static string LockDownFileName = "MidgardLockDownLevel.def"; // mod by Dies Irae

		public static void Initialize()
		{
			EventSink.DeleteRequest += new DeleteRequestEventHandler( EventSink_DeleteRequest );
			EventSink.AccountLogin += new AccountLoginEventHandler( EventSink_AccountLogin );
			EventSink.GameLogin += new GameLoginEventHandler( EventSink_GameLogin );

			if ( PasswordCommandEnabled )
				CommandSystem.Register( "Password", AccessLevel.Player, new CommandEventHandler( Password_OnCommand ) );

			if ( Core.AOS )
			{
				//CityInfo haven = new CityInfo( "Haven", "Uzeraan's Mansion", 3618, 2591, 0 );
				CityInfo haven = new CityInfo( "Haven", "Uzeraan's Mansion", 3503, 2574, 14 );
				StartingCities[StartingCities.Length - 1] = haven;
            }

            #region mod by Dies Irae
            if( File.Exists( LockDownFileName ) )
            {
                using( StreamReader op = new StreamReader( LockDownFileName ) )
                {
                    AccessLevel level = AccessLevel.Player;

                    try
                    {
                        level = (AccessLevel)Enum.Parse( typeof( AccessLevel ), op.ReadLine(), true );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }

                    if( level > AccessLevel.Player )
                    {
                        m_LockdownLevel = level;

                        Utility.PushColor( ConsoleColor.White );
                        Console.WriteLine( "Notice: Server is under this lockdown level: {0}", level );
                        Utility.PopColor();
                    }
                }
            }
            #endregion
        }

		[Usage( "Password <newPassword> <repeatPassword>" )]
		[Description( "Changes the password of the commanding players account. Requires the same C-class IP address as the account's creator." )]
		public static void Password_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			Account acct = from.Account as Account;

			if ( acct == null )
				return;

			IPAddress[] accessList = acct.LoginIPs;

			if ( accessList.Length == 0 )
				return;

			NetState ns = from.NetState;

			if ( ns == null )
				return;

			if ( e.Length == 0 )
			{
				from.SendMessage( "You must specify the new password." );
				return;
			}
			else if ( e.Length == 1 )
			{
				from.SendMessage( "To prevent potential typing mistakes, you must type the password twice. Use the format:" );
				from.SendMessage( "Password \"(newPassword)\" \"(repeated)\"" );
				return;
			}

			string pass = e.GetString( 0 );
			string pass2 = e.GetString( 1 );

			if ( pass != pass2 )
			{
				from.SendMessage( "The passwords do not match." );
				return;
			}

			bool isSafe = true;

			for ( int i = 0; isSafe && i < pass.Length; ++i )
				isSafe = ( pass[i] >= 0x20 && pass[i] < 0x80 );

			if ( !isSafe )
			{
				from.SendMessage( "That is not a valid password." );
				return;
			}

			try
			{
				IPAddress ipAddress = ns.Address;

				if ( Utility.IPMatchClassC( accessList[0], ipAddress ) )
				{
					acct.SetPassword( pass );
					from.SendMessage( "The password to your account has changed." );
				}
				else
				{
					PageEntry entry = PageQueue.GetEntry( from );

					if ( entry != null )
					{
						if ( entry.Message.StartsWith( "[Automated: Change Password]" ) )
							from.SendMessage( "You already have a password change request in the help system queue." );
						else
							from.SendMessage( "Your IP address does not match that which created this account." );
					}
					else if ( PageQueue.CheckAllowedToPage( from ) )
					{
						from.SendMessage( "Your IP address does not match that which created this account.  A page has been entered into the help system on your behalf." );

						from.SendLocalizedMessage( 501234, "", 0x35 ); /* The next available Counselor/Game Master will respond as soon as possible.
																	    * Please check your Journal for messages every few minutes.
																	    */

						PageQueue.Enqueue( new PageEntry( from, String.Format( "[Automated: Change Password]<br>Desired password: {0}<br>Current IP address: {1}<br>Account IP address: {2}", pass, ipAddress, accessList[0] ), PageType.Account ) );
					}

				}
			}
			catch
			{
			}
		}

		private static void EventSink_DeleteRequest( DeleteRequestEventArgs e )
		{
			NetState state = e.State;
			int index = e.Index;

			Account acct = state.Account as Account;

			if ( acct == null )
			{
				state.Dispose();
			}
			else if ( index < 0 || index >= acct.Length )
			{
				state.Send( new DeleteResult( DeleteResultType.BadRequest ) );
				state.Send( new CharacterListUpdate( acct ) );
			}
			else
			{
				Mobile m = acct[index];

				if ( m == null )
				{
					state.Send( new DeleteResult( DeleteResultType.CharNotExist ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else if ( m.NetState != null )
				{
					state.Send( new DeleteResult( DeleteResultType.CharBeingPlayed ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else if ( RestrictDeletion && DateTime.Now < (m.CreationTime + DeleteDelay) )
				{
					state.Send( new DeleteResult( DeleteResultType.CharTooYoung ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else if ( m.AccessLevel == AccessLevel.Player && Region.Find( m.LogoutLocation, m.LogoutMap ).GetRegion( typeof( Jail ) ) != null )	//Don't need to check current location, if netstate is null, they're logged out
				{
					state.Send( new DeleteResult( DeleteResultType.BadRequest ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else
				{
					Console.WriteLine( "Client: {0}: Deleting character {1} (0x{2:X})", state, index, m.Serial.Value );

					acct.Comments.Add( new AccountComment( "System", String.Format( "Character #{0} {1} deleted by {2}", index + 1, m, state ) ) );

					m.Delete();
					state.Send( new CharacterListUpdate( acct ) );
				}
			}
		}

		public static bool CanCreate( IPAddress ip )
		{
			if ( !IPTable.ContainsKey( ip ) )
				return true;

			return ( IPTable[ip] < MaxAccountsPerIP );
		}

		private static Dictionary<IPAddress, Int32> m_IPTable;

		public static Dictionary<IPAddress, Int32> IPTable
		{
			get
			{
				if ( m_IPTable == null )
				{
					m_IPTable = new Dictionary<IPAddress, Int32>();

					foreach ( Account a in Accounts.GetAccounts() )
						if ( a.LoginIPs.Length > 0 )
						{
							IPAddress ip = a.LoginIPs[0];

							if ( m_IPTable.ContainsKey( ip ) )
								m_IPTable[ip]++;
							else
								m_IPTable[ip] = 1;
						}
				}

				return m_IPTable;
			}
		}	

		private static Account CreateAccount( NetState state, string un, string pw )
		{
			if ( un.Length == 0 || pw.Length == 0 )
				return null;

			bool isSafe = true;

			for ( int i = 0; isSafe && i < un.Length; ++i )
				isSafe = ( un[i] >= 0x20 && un[i] < 0x80 );

			for ( int i = 0; isSafe && i < pw.Length; ++i )
				isSafe = ( pw[i] >= 0x20 && pw[i] < 0x80 );

			if ( !isSafe )
				return null;

			if ( !CanCreate( state.Address ) )
			{
				Console.WriteLine( "Login: {0}: Account '{1}' not created, ip already has {2} account{3}.", state, un, MaxAccountsPerIP, MaxAccountsPerIP == 1 ? "" : "s" );
				return null;
			}

			Console.WriteLine( "Login: {0}: Creating new account '{1}'", state, un );

			#region modifica by Berto
			string em = null;
			Account a = new Account( un, pw ,em);
			#endregion

			return a;
		}

		#region modifica by Berto
		public static bool CanLogInLan( string  user )
		{
            if( TestCenter.Enabled )
                return true;

			Console.WriteLine( "LanCheck for account {0} ",  user );
			Account acct = Accounts.GetAccount( user ) as Account;

			if( acct != null && acct.GetTag( "LAN" ) == "1" )
			{
				Console.WriteLine("User {0} is logged in LAN status.", user );
				return true;				
			}

			return false;
		}
		#endregion
		
		public static void EventSink_AccountLogin( AccountLoginEventArgs e )
		{
			#region modifica by Berto
			if ( !IPLimiter.SocketBlock && !IPLimiter.Verify( e.State.Address ) && !CanLogInLan(e.Username) )
			{
			#endregion
				e.Accepted = false;
				e.RejectReason = ALRReason.InUse;

				Console.WriteLine( "Login: {0}: Past IP limit threshold", e.State );

				using ( StreamWriter op = new StreamWriter( "Logs/ipLimits.log", true ) )
					op.WriteLine( "{0}\tPast IP limit threshold\t{1}", e.State, DateTime.Now );

				return;
			}

			string un = e.Username;
			string pw = e.Password;

			e.Accepted = false;
			Account acct = Accounts.GetAccount( un ) as Account;

			if ( acct == null )
			{
				if ( AutoAccountCreation && un.Trim().Length > 0 )	//To prevent someone from making an account of just '' or a bunch of meaningless spaces 
				{
					e.State.Account = acct = CreateAccount( e.State, un, pw );
					e.Accepted = acct == null ? false : acct.CheckAccess( e.State );

					if ( !e.Accepted )
						e.RejectReason = ALRReason.BadComm;
				}
				else
				{
					Console.WriteLine( "Login: {0}: Invalid username '{1}'", e.State, un );
					e.RejectReason = ALRReason.Invalid;
				}
			}
			else if ( !acct.HasAccess( e.State ) )
			{
				Console.WriteLine( "Login: {0}: Access denied for '{1}'", e.State, un );
				e.RejectReason = ( m_LockdownLevel > AccessLevel.Player ? ALRReason.BadComm : ALRReason.BadPass );
			}
			else if ( !acct.CheckPassword( pw ) )
			{
				Console.WriteLine( "Login: {0}: Invalid password for '{1}'", e.State, un );
				e.RejectReason = ALRReason.BadPass;
			}
			else if ( acct.Banned )
			{
				Console.WriteLine( "Login: {0}: Banned account '{1}'", e.State, un );
				e.RejectReason = ALRReason.Blocked;
			}
			else
			{
				Console.WriteLine( "Login: {0}: Valid credentials for '{1}'", e.State, un );
				e.State.Account = acct;
				e.Accepted = true;

				acct.LogAccess( e.State );
            }

			if ( !e.Accepted )
				AccountAttackLimiter.RegisterInvalidAccess( e.State );
		}

		public static void EventSink_GameLogin( GameLoginEventArgs e )
		{
			#region modifica by Berto
			if ( !IPLimiter.SocketBlock && !IPLimiter.Verify( e.State.Address ) && !CanLogInLan(e.Username) )
			{
			#endregion	
				e.Accepted = false;

				Console.WriteLine( "Login: {0}: Past IP limit threshold", e.State );

				using ( StreamWriter op = new StreamWriter( "Logs/ipLimits.log", true ) )
					op.WriteLine( "{0}\tPast IP limit threshold\t{1}", e.State, DateTime.Now );

				return;
			}

			string un = e.Username;
			string pw = e.Password;

			Account acct = Accounts.GetAccount( un ) as Account;

			if ( acct == null )
			{
				e.Accepted = false;
			}
			else if ( !acct.HasAccess( e.State ) )
			{
				Console.WriteLine( "Login: {0}: Access denied for '{1}'", e.State, un );
				e.Accepted = false;
			}
			else if ( !acct.CheckPassword( pw ) )
			{
				Console.WriteLine( "Login: {0}: Invalid password for '{1}'", e.State, un );
				e.Accepted = false;
			}
			else if ( acct.Banned )
			{
				Console.WriteLine( "Login: {0}: Banned account '{1}'", e.State, un );
				e.Accepted = false;
			}
			else
			{
				acct.LogAccess( e.State );

				Console.WriteLine( "Login: {0}: Account '{1}' at character list", e.State, un );
				e.State.Account = acct;
				e.Accepted = true;
				e.CityInfo = Core.AOS ? StartingCities : OldStartingCities; // mod by Dies Irae
			}

			if ( !e.Accepted )
				AccountAttackLimiter.RegisterInvalidAccess( e.State );
		}
	}
}