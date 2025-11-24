using System.Collections.Generic;

using Server;
using Server.Guilds;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
	public class GuildDeclareAllianceGump : GuildListGump
	{
		public GuildDeclareAllianceGump( Mobile from, Guild guild, List<Guild> list )
			: base( from, guild, true, list )
		{
		}

		protected override void Design()
		{
			AddHtml( 20, 10, 400, 35, m_Mobile.Language == "ITA" ? "Seleziona la gilda con cui vorresti allearti." : "Select the guild you wish to declare alliance with.", false, false );

			AddButton( 20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtml( 55, 400, 245, 30, m_Mobile.Language == "ITA" ? "Conferma!" : "Confirm!", false, false );

			AddButton( 300, 400, 4005, 4007, 2, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 335, 400, 100, 35, 1011012, false, false ); // CANCEL
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if( GuildGump.BadLeader( m_Mobile, m_Guild ) )
				return;

			if( m_Guild == null )
				return;

			if( info.ButtonID == 1 )
			{
				int[] switches = info.Switches;

				if( switches.Length > 0 )
				{
					int index = switches[ 0 ];

					if( index >= 0 && index < m_List.Count )
					{
						Guild g = m_List[ index ];

						if( g != null )
						{
							if( g == m_Guild )
							{
								m_Mobile.SendMessage( m_Mobile.Language == "ITA" ? "Non puoi allearti con te stesso!" : "You cannot declare an alliance with yourself!" );
							}
							else if( ( g.AllyInvitations.Contains( m_Guild ) && m_Guild.AllyDeclarations.Contains( g ) ) || m_Guild.IsAlly( g ) )
							{
								m_Mobile.SendMessage( m_Mobile.Language == "ITA" ? "Siete già in una alleanza." : "You are already in alliance with that guild." );
							}
							else
							{
								if( !m_Guild.AllyDeclarations.Contains( g ) )
								{
									m_Guild.AllyDeclarations.Add( g );
									m_Guild.GuildTextMessage( m_Mobile.Language == "ITA" ? "Messaggio di Gilda: La tua gilda ha richiesto un'alleanza a: {0} ({1})" : "Guild Message: Your guild has sent an invitation for alliance to: {0} ({1})", g.Name, g.Abbreviation );
								}

								if( !g.AllyInvitations.Contains( m_Guild ) )
								{
									g.AllyInvitations.Add( m_Guild );
									m_Guild.GuildTextMessage( m_Mobile.Language == "ITA" ? "Messaggio di Gilda: La tua gilda ha ricevuto una richiesta di alleanza da: {0} ({1})" : "Guild Message: Your guild has received an invitation for alliance from: {0} ({1})", g.Name, g.Abbreviation );
								}
							}

							GuildGump.EnsureClosed( m_Mobile );
							m_Mobile.SendGump( new GuildAllianceAdminGump( m_Mobile, m_Guild ) );
						}
					}
				}
			}
			else if( info.ButtonID == 2 )
			{
				GuildGump.EnsureClosed( m_Mobile );
				m_Mobile.SendGump( new GuildmasterGump( m_Mobile, m_Guild ) );
			}
		}
	}
}