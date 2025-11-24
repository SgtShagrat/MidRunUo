using System.Collections.Generic;

using Server;
using Server.Guilds;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
	public class GuildAllianceGump : Gump
	{
		private readonly Mobile m_Mobile;
		private readonly Guild m_Guild;

		public GuildAllianceGump( Mobile from, Guild guild ) : base( 20, 30 )
		{
			m_Mobile = from;
			m_Guild = guild;

			Dragable = true;

			AddPage( 0 );
			AddBackground( 0, 0, 550, 440, 5054 );
			AddBackground( 10, 10, 530, 420, 3000 );

			AddHtml( 20, 10, 500, 35, m_Mobile.Language == "ITA" ? "<center>STATO ALLEANZA</center>" : "<center>ALLIANCE STATUS</center>", false, false );

			AddButton( 20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 55, 400, 300, 35, 1011120, false, false ); // Return to the main menu.

			AddPage( 1 );

			AddButton( 375, 375, 5224, 5224, 0, GumpButtonType.Page, 2 );
			AddHtmlLocalized( 410, 373, 100, 25, 1011066, false, false ); // Next page

			AddHtml( 20, 45, 400, 20, m_Mobile.Language == "ITA" ? "Siamo alleati con:" : "We are allied with:", false, false );

			List<Guild> allies = guild.Allies;

			if( allies.Count == 0 )
			{
				AddHtml( 20, 65, 400, 20, m_Mobile.Language == "ITA" ? "Nessuna alleanza corrente" : "No current alliances", false, false );
			}
			else
			{
				for( int i = 0; i < allies.Count; ++i )
				{
					Guild g = allies[ i ];

					AddHtml( 20, 65 + ( i * 20 ), 300, 20, g.Name, false, false );
				}
			}

			AddPage( 2 );

			AddButton( 375, 375, 5224, 5224, 0, GumpButtonType.Page, 3 );
			AddHtmlLocalized( 410, 373, 100, 25, 1011066, false, false ); // Next page

			AddButton( 30, 375, 5223, 5223, 0, GumpButtonType.Page, 1 );
			AddHtmlLocalized( 65, 373, 150, 25, 1011067, false, false ); // Previous page

			AddHtml( 20, 45, 400, 20, m_Mobile.Language == "ITA" ? "Gilde a cui abbiamo richiesto alleanza:" : "Guilds that we invited for alliance:", false, false );

			List<Guild> declared = guild.AllyDeclarations;

			if( declared.Count == 0 )
			{
				AddHtml( 20, 65, 400, 20, m_Mobile.Language == "ITA" ? "Nessuna richiesta di alleanza ricevuta." : "No current invitations received for alliance.", false, false );
			}
			else
			{
				for( int i = 0; i < declared.Count; ++i )
				{
					Guild g = declared[ i ];

					AddHtml( 20, 65 + ( i * 20 ), 300, 20, g.Name, false, false );
				}
			}

			AddPage( 3 );

			AddButton( 30, 375, 5223, 5223, 0, GumpButtonType.Page, 2 );
			AddHtmlLocalized( 65, 373, 150, 25, 1011067, false, false ); // Previous page

			AddHtml( 20, 45, 400, 20, m_Mobile.Language == "ITA" ? "Gilde che hanno richiesto alleanza:" : "Guilds that have invited us for alliance:", false, false );

			List<Guild> invites = guild.AllyInvitations;

			if( invites.Count == 0 )
			{
				AddHtml( 20, 65, 400, 20, m_Mobile.Language == "ITA" ? "Nessuna richiesta di alleanza" : "No current alliance invitations", false, false );
			}
			else
			{
				for( int i = 0; i < invites.Count; ++i )
				{
					Guild g = invites[ i ];

					AddHtml( 20, 65 + ( i * 20 ), 300, 20, g.Name, false, false );
				}
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if( GuildGump.BadMember( m_Mobile, m_Guild ) )
				return;

			if( info.ButtonID == 1 )
			{
				GuildGump.EnsureClosed( m_Mobile );
				m_Mobile.SendGump( new GuildGump( m_Mobile, m_Guild ) );
			}
		}
	}
}