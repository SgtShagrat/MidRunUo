/***************************************************************************
 *                                  ListSkillsStatCaps.cs
 *                            		---------------------
 *  begin                	: Ottobre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 
 ***************************************************************************/

using System;
using System.IO;
using Server;
using Server.Accounting;
using Server.Commands;

namespace Midgard.Commands
{
	public class ListSkillsStatCaps
	{
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "ListSkills", AccessLevel.Developer, new CommandEventHandler( ListSkills_OnCommand ) );
			CommandSystem.Register( "ListStats", AccessLevel.Developer, new CommandEventHandler( ListStats_OnCommand ) );
		}
		#endregion
	
		#region callback
		[Usage( "ListSkills" )]
		[Description( "List skills from players on our shard" )]
		public static void ListSkills_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 0 )
			{
	            try
	            {
					using( StreamWriter op = new StreamWriter( "Logs/MidgardSkills.log" ) )
					{
						op.WriteLine( "List generated on {0} in time {1}.", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
						op.WriteLine( "Total accounts processed {0}", Accounts.GetAccounts().Count );
						op.WriteLine( "" );
	
						foreach( Account a in Accounts.GetAccounts() )
						{
							if( a.AccessLevel > AccessLevel.Player )
								continue;

							op.WriteLine( "{0}", a.Username );

							for( int i = 0; i < a.Length; i++ )
							{
								Mobile m = a[i] as Mobile;
								bool isLamer;
								if( m != null && !m.Deleted )
								{
									op.WriteLine( "\tName: {0} - SkillCap {1} - SkillsTotal {2} {3}", m.Name, m.SkillsCap.ToString( "F2" ), m.SkillsTotal.ToString( "F2" ), 
									             ( m.SkillsCap < m.SkillsTotal ? "<<<<<<<<<<<<<<<<<<<<" : "" ) );

						            for( int s = 0; s < m.Skills.Length; s++ )
									{
						      			SkillName sn = (SkillName)s;
						      			double val = m.Skills[s].Base;
						      			double cap = m.Skills[s].Cap;

										isLamer = false;
										if( cap != 100.0 && cap != 105.0 && cap != 110.0 && cap != 115.0 && cap != 120.0 )
											isLamer = true;
										else if( val > 120.0 )
											isLamer = true;

						      			if( val > 0.0 || cap > 100.0 )
										{
						      				op.WriteLine( "\t\tSkill {0}: {1}/{2} {3}", sn.ToString(), val.ToString( "F2" ), cap.ToString( "F2" ), ( isLamer ? "<<<<<<<<<<<<<<<" : "" ) );
						      			}
						            }
								}
							}
						}
					}
	            }
				catch( Exception ex )
				{
					Console.WriteLine( ex.ToString() );
				}
			}
		}

		[Usage( "ListStats" )]
		[Description( "List all stats from player of our shard" )]
		public static void ListStats_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 0 )
			{
	            try
	            {
					using( StreamWriter op = new StreamWriter( "Logs/MidgardStats.log" ) )
					{
						op.WriteLine( "List generated on {0} in time {1}.", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
						op.WriteLine( "Total accounts processed {0}", Accounts.GetAccounts().Count );
						op.WriteLine( "" );
	
						foreach( Account a in Accounts.GetAccounts() )
						{
							if( a.AccessLevel > AccessLevel.Player )
								continue;

							op.WriteLine( "{0}", a.Username );

							for( int i = 0; i < a.Length; i++ )
							{
								Mobile m = a[i] as Mobile;
								bool isLamer;
								if( m != null && !m.Deleted )
								{
									isLamer = false;
									if( m.StatCap != 225 && m.StatCap != 230 && m.StatCap != 235 && m.StatCap != 240 && m.StatCap != 245 && m.StatCap != 250 )
										isLamer = true;
									else if( m.RawStr > 125 || m.RawDex > 125 ||  m.RawInt > 125 )
										isLamer = true;
									else if( m.StatCap < m.RawStatTotal )
										isLamer = true;

									op.WriteLine( "\tName: {0} - Str {1} - Dex {2} - Int {3} - StatCap {4} - SumStats {5} {6}",
									              m.Name, m.RawStr, m.RawDex, m.RawInt, m.StatCap, m.RawStatTotal, (isLamer ? "<<<<<<<<<<<<<<<<<<<<<<<<<<" : "") );
								}
							}
						}
					}
	            }
				catch( Exception ex )
				{
					Console.WriteLine( ex.ToString() );
				}
			}
		}
		#endregion
	}
}
