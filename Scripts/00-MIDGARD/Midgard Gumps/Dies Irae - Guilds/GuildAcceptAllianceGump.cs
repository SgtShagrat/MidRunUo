using Server;
using Server.Guilds;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
	public class GuildAcceptAllianceGump : GuildListGump
	{
		public GuildAcceptAllianceGump( Mobile from, Guild guild ) : base( from, guild, true, guild.AllyInvitations )
		{
		}

		protected override void Design()
		{
			AddHtmlLocalized( 20, 10, 400, 35, 1011147, false, false ); // Select the guild to accept the invitations: 

			AddButton( 20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtml( 55, 400, 245, 30, m_Mobile.Language == "ITA" ? "Accetta richieste di alleanza." : "Accept alliance invitations.", false, false );

			AddButton( 300, 400, 4005, 4007, 2, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 335, 400, 100, 35, 1011012, false, false ); // CANCEL
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if( GuildGump.BadLeader( m_Mobile, m_Guild ) )
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
							m_Guild.AllyInvitations.Remove( g );
							g.AllyDeclarations.Remove( m_Guild );

							m_Guild.AddAlly( g );
							m_Guild.GuildTextMessage( m_Mobile.Language == "ITA" ? "Messaggio di Gilda: La tua gilda è ora alleata con: {0} ({1})." : "Guild Message: Your guild is now in alliance with: {0} ({1}).", g.Name, g.Abbreviation );

							GuildGump.EnsureClosed( m_Mobile );

							if( m_Guild.AllyInvitations.Count > 0 )
								m_Mobile.SendGump( new GuildAcceptAllianceGump( m_Mobile, m_Guild ) );
							else
								m_Mobile.SendGump( new GuildmasterGump( m_Mobile, m_Guild ) );
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