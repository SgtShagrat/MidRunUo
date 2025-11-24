/***************************************************************************
 *                                    FindRacesCommand.cs
 *                            		-----------------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 * 			Comando per la gestione delle razze da parte dell'admin.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Commands
{
	public class FindRacesCommand
	{
		#region registration
		public static void Initialize()
		{
			CommandSystem.Register( "FindRaces", AccessLevel.Administrator, new CommandEventHandler( FindRaces_OnCommand ) );
		}
		#endregion

		#region IComparers
		public enum ComparerType { name, account, race };

		private class NameComparer : IComparer<Mobile>
		{
			public static readonly IComparer<Mobile> Instance = new NameComparer();

		    public int Compare( Mobile x, Mobile y )
			{
				if ( x == null || y == null )
					throw new ArgumentException();

				return Insensitive.Compare( x.Name, y.Name );
			}
		}

		private class AccountComparer : IComparer<Mobile>
		{
			public static readonly IComparer<Mobile> Instance = new AccountComparer();

		    public int Compare( Mobile x, Mobile y )
			{
				if ( x == null || y == null )
					throw new ArgumentException();

				return Insensitive.Compare( x.Account.Username, y.Account.Username );
			}
		}

		private class RaceComparer : IComparer<Mobile>
		{
			public static readonly IComparer<Mobile> Instance = new RaceComparer();

		    public int Compare( Mobile x, Mobile y )
			{
				if ( x == null || y == null )
					throw new ArgumentException();

				return Insensitive.Compare( x.Race.Name, y.Race.Name );
			}
		}
		#endregion

		#region callback
		[Usage( "FindRaces {name | account | race} {<list> <online>}" )]
		[Description( "List all raced pgs. If list argument is present a log is built. If online argument is present only active netstates are processed." )]
		public static void FindRaces_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length > 0 )
			{
				string args0 = e.GetString( 0 ).ToLower();

				ComparerType cryteria = ComparerType.race;
				if( Enum.IsDefined( typeof(ComparerType), args0 ) )
					cryteria = (ComparerType)Enum.Parse( typeof(ComparerType), args0 );
				
				bool online = false;
				bool list = false;

				for( int i = 0; i < e.Arguments.Length; i++ )
				{
					if( Utility.InsensitiveCompare( e.Arguments[i], "list" ) == 0 )
						list = true;
					if( Utility.InsensitiveCompare( e.Arguments[i], "online" ) == 0 )
						online = true;				   	
				}

				if( list )
				{
					ListRaced();
					from.SendMessage( "Raced table has been generated. See the file : <runuo root>/Logs/MidgardRacedPG.log" );
				}

				from.SendGump( new FindRacesGump( from, cryteria, online ) );
			}
			else
				from.SendMessage( "Command Use: FindRaces {name | account | race} {<list> <online>}" );
		}
		#endregion

		private static List<Mobile> BuildList( bool online, ComparerType cryteria )
		{
			List<Mobile> list = new List<Mobile>();

		    foreach( Account acct in Accounts.GetAccounts() )
			{
				for( int i = 0 ; i < acct.Count ; i++ )
				{
					Mobile m = acct[i];
					if( m == null || m.AccessLevel > AccessLevel.Player)
						continue;

					if( m.Race != Race.Human )
					{
						if( ( online && m.NetState != null ) || !online )
							list.Add( m );
					}
				}
			}

			switch( cryteria )
			{
				case ComparerType.name: list.Sort( NameComparer.Instance ); break;
				case ComparerType.account: list.Sort( AccountComparer.Instance ); break;
				case ComparerType.race: list.Sort( RaceComparer.Instance ); break;
				default: list.Sort( RaceComparer.Instance ); break;
			}

			return list;
		}

		private static void ListRaced()
		{
			using ( StreamWriter op = new StreamWriter( "Logs/MidgardRacedPG.log" ) )
			{
				try
				{
					List<Mobile> list = BuildList( false, ComparerType.account );

					op.WriteLine( "## Race List generated on {0} ##", DateTime.Now );
					op.WriteLine( "#################################################");
					op.WriteLine();
					op.WriteLine();

					op.WriteLine( "# Raced PGs:" );
					op.WriteLine();

					for( int i = 0 ; i < list.Count ; i++ )
						op.WriteLine("{0} - {1} - {2}", list[i].Name, list[i].Account.Username, list[i].Race );
				}
				catch( Exception ex )
				{
                    Console.WriteLine( ex.ToString() );
				}
			}
		}

		public class FindRacesGump : Gump
		{
			#region fields
			private static readonly int m_Fields = 20;
			private static readonly int m_HueTit = 15;
			private Mobile m_Owner;
			private List<Mobile> m_Mobiles;
			private int m_Page;
			private ComparerType m_Cryteria;
			private bool m_Online;
			#endregion

			#region constructors
			public FindRacesGump( Mobile owner ): this( owner, BuildList( false, ComparerType.race ), 1, ComparerType.race, false )
			{
			}

			public FindRacesGump( Mobile owner, bool online ): this( owner, BuildList( online, ComparerType.race ), 1, ComparerType.race, online )
			{
			}

			public FindRacesGump( Mobile owner, ComparerType cryteria, bool online ): this( owner, BuildList( online, cryteria ), 1, cryteria, online )
			{
			}

			public FindRacesGump( Mobile owner, List<Mobile> list, int page, ComparerType cryteria, bool online ): base( 50, 50 )
			{
				Closable = true;
				Disposable = true;
				Dragable = true;
				Resizable = false;

				owner.CloseGump( typeof(FindRacesGump) );

				m_Owner = owner;
				m_Mobiles = list;
				m_Cryteria = cryteria;
				m_Online = online;
			
				Initialize( page );
			}
			#endregion

			#region metodi
			private void AddBlackAlpha( int x, int y, int width, int height )
			{
				AddImageTiled( x, y, width, height, 2624 );
				AddAlphaRegion( x, y, width, height );
			}

			private void Initialize( int page )
			{	
					m_Page = page;
					
					AddPage( 0 );
					AddBackground( 0, 0, 405, 481, 83 );
					AddBlackAlpha(10, 10, 385, 461 );

					AddLabel( 13, 11, m_HueTit, "Race list:" );

					if( m_Page > 1 )
						AddButton( (381-20), 13, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0 ); 	// Previous Page

					if( m_Page < Math.Ceiling( m_Mobiles.Count/(double)m_Fields) )
						AddButton( 381, 13, 0x15E1, 0x15E5, 300, GumpButtonType.Reply, 0 ); 		// NextPage

					int IndMax = ( m_Page * m_Fields ) - 1;
					int IndMin = ( m_Page * m_Fields ) - m_Fields;
					int IndTemp = 0;

					for( int i = 0; i < m_Mobiles.Count; ++i )
					{	
						if( i >= IndMin && i <= IndMax )
						{
							Mobile m = m_Mobiles[i];
							if( m != null )
							{
								AddLabelCropped( 13, 33 + (IndTemp * 22), 150, 22, 0x58, m.Name );
								AddLabelCropped( 163, 33 + (IndTemp * 22), 150, 22 , 0x516, m.Account.Username );
								AddLabelCropped( 263, 33 + (IndTemp * 22), 150, 22, 0x144, m.Race.Name );

								// Goto if online
								if( m.NetState != null )
									AddButton( 363, 33 + (IndTemp * 22) + 3, 0x15E1, 0x15E5, 1000 + i + 1, GumpButtonType.Reply, 0 );

								// Remove race
								AddButton( 383, 33 + (IndTemp * 22) + 3, 0x15E1, 0x15E5, 2000 + i + 1, GumpButtonType.Reply, 0 );

								IndTemp++;
							}
						}	
					}
				}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				Mobile from = sender.Mobile;
	
				if( info.ButtonID == 0 )							// Close
					return;
	
				if( info.ButtonID == 200 ) 							// Previous Page
				{
					m_Page--;
					from.SendGump( new FindRacesGump( from, m_Mobiles, m_Page, m_Cryteria, m_Online ) );
				}
				else if( info.ButtonID == 300 ) 					// NextPage
				{
					m_Page++;
					from.SendGump( new FindRacesGump( from, m_Mobiles, m_Page, m_Cryteria, m_Online ) );
				}
				else if( info.ButtonID > 1000 && info.ButtonID < 2000 ) 
				{
					try
					{
						Mobile m = m_Mobiles[ info.ButtonID - 1000 - 1 ];
	
						if( m.Map != Map.Internal )
						{
							from.MoveToWorld( m.Location, m.Map );
							from.SendMessage( "You've gone to {0}", m.Name );
						}
						else
							from.SendMessage( "Player has logged-out." );
	
						from.SendGump( new FindRacesGump( from, m_Mobiles, m_Page, m_Cryteria, m_Online ) );
					}
					catch( Exception ex )
					{
						Console.WriteLine( "Error in [FindRacesGump: {0}", ex);
					}
				}
				else if( info.ButtonID > 2000 ) 
				{
					try
					{
						Mobile raced = m_Mobiles[ info.ButtonID - 2000 - 1 ];
						Race humans = Race.Human;
						raced.Race = humans;

						from.SendMessage( "You have setted human race to {0} (account {1}).", raced.Name, raced.Account.Username );
						
						// Validazione di Capelli e Barba
						if( humans.ValidateHair( raced, raced.HairItemID ) )
							raced.HairHue = humans.ClipHairHue( raced.HairHue );
						else
						{
							raced.HairItemID = humans.RandomHair( false );
							raced.HairHue = humans.RandomHairHue();
						}

						if( humans.ValidateFacialHair( raced, raced.FacialHairItemID ) )
							raced.FacialHairHue = humans.ClipHairHue( raced.FacialHairHue );
						else
						{
							raced.FacialHairItemID = humans.RandomFacialHair( false );
							raced.FacialHairHue = humans.RandomHairHue();
						}

						// Validazione del colore della pelle
						raced.Hue = humans.RandomSkinHue();

						from.SendGump( new FindRacesGump( from, m_Cryteria, m_Online ) );
					}
					catch( Exception ex )
					{
						Console.WriteLine( "Error in [FindRacesGump: {0}", ex);
					}
				}
			}
			#endregion
		}
	}
}
