using Server;
using Server.Guilds;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
	public class GuildAllianceAdminGump : Gump
	{
		private readonly Mobile m_Mobile;
		private readonly Guild m_Guild;

		public GuildAllianceAdminGump( Mobile from, Guild guild ) : base( 20, 30 )
		{
			m_Mobile = from;
			m_Guild = guild;

			Dragable = true;

			AddPage( 0 );
			AddBackground( 0, 0, 550, 440, 5054 );
			AddBackground( 10, 10, 530, 420, 3000 );

			AddHtml( 20, 10, 510, 35, m_Mobile.Language == "ITA" ? "<center>FUNZIONI ALLEANZA</center>" : "<center>ALLIANCE FUNCTIONS</center>", false, false );

			AddButton( 20, 40, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtml( 55, 40, 400, 30, m_Mobile.Language == "ITA" ? "Dichiara alleanza ricercando il nome gilda." : "Declare alliance through guild name search.", false, false );

			int count = 0;

			if( guild.Allies.Count > 0 )
			{
				AddButton( 20, 160 + ( count * 30 ), 4005, 4007, 2, GumpButtonType.Reply, 0 );
				AddHtml( 55, 160 + ( count++ * 30 ), 400, 30, m_Mobile.Language == "ITA" ? "Rimuovi l'alleanza." : "Remove alliance.", false, false );
			}
			else
			{
				AddHtml( 20, 160 + ( count++ * 30 ), 400, 30, m_Mobile.Language == "ITA" ? "Nessuna alleanza corrente" : "No current alliances", false, false );
			}

			if( guild.AllyInvitations.Count > 0 )
			{
				AddButton( 20, 160 + ( count * 30 ), 4005, 4007, 3, GumpButtonType.Reply, 0 );
				AddHtml( 55, 160 + ( count++ * 30 ), 400, 30, m_Mobile.Language == "ITA" ? "Accetta richieste alleanza." : "Accept alliance invitations.", false, false );

				AddButton( 20, 160 + ( count * 30 ), 4005, 4007, 4, GumpButtonType.Reply, 0 );
				AddHtml( 55, 160 + ( count++ * 30 ), 400, 30, m_Mobile.Language == "ITA" ? "Rifiuta richieste alleanza." : "Reject alliance invitations.", false, false );
			}
			else
			{
				AddHtml( 20, 160 + ( count++ * 30 ), 400, 30, m_Mobile.Language == "ITA" ? "Nessuna richiesta di alleanza ricevuta." : "No current alliance invitations received.", false, false );
			}

			if( guild.AllyDeclarations.Count > 0 )
			{
				AddButton( 20, 160 + ( count * 30 ), 4005, 4007, 5, GumpButtonType.Reply, 0 );
				AddHtml( 55, 160 + ( count++ * 30 ), 400, 30, m_Mobile.Language == "ITA" ? "Rescindi le tue richieste di alleanza." : "Rescind your alliance invitazions.", false, false );
			}
			else
			{
				AddHtml( 20, 160 + ( count++ * 30 ), 400, 30, m_Mobile.Language == "ITA" ? "Nessuna richiesta di alleanza in attesa." : "No current alliance invittions.", false, false );
			}

			AddButton( 20, 400, 4005, 4007, 6, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 55, 400, 400, 35, 1011104, false, false ); // Return to the previous menu.
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if( GuildGump.BadLeader( m_Mobile, m_Guild ) )
				return;

			switch( info.ButtonID )
			{
				case 1: // Declare alliance
					{
						m_Mobile.SendMessage( m_Mobile.Language == "ITA" ? "Dichiara alleanza tramite ricerca - Nome Gilda:"  : "Declare alliance through search - Enter Guild Name:" );
						m_Mobile.Prompt = new GuildDeclareAlliancePrompt( m_Mobile, m_Guild );

						break;
					}
				case 2: // Remove alliance
					{
						GuildGump.EnsureClosed( m_Mobile );
						m_Mobile.SendGump( new GuildRemoveAllianceGump( m_Mobile, m_Guild ) );

						break;
					}
				case 3: // Accept alliance
					{
						GuildGump.EnsureClosed( m_Mobile );
						m_Mobile.SendGump( new GuildAcceptAllianceGump( m_Mobile, m_Guild ) );

						break;
					}
				case 4: // Reject alliance
					{
						GuildGump.EnsureClosed( m_Mobile );
						m_Mobile.SendGump( new GuildRejectAllianceGump( m_Mobile, m_Guild ) );

						break;
					}
				case 5: // Rescind invitations
					{
						GuildGump.EnsureClosed( m_Mobile );
						m_Mobile.SendGump( new GuildRescindAllianceDeclarationGump( m_Mobile, m_Guild ) );

						break;
					}
				case 6: // Return
					{
						GuildGump.EnsureClosed( m_Mobile );
						m_Mobile.SendGump( new GuildmasterGump( m_Mobile, m_Guild ) );

						break;
					}
			}
		}
	}
}