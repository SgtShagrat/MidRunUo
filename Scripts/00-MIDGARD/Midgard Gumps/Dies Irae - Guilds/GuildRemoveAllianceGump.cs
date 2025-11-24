using Server;
using Server.Guilds;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
	public class GuildRemoveAllianceGump : GuildListGump
	{
		public GuildRemoveAllianceGump( Mobile from, Guild guild ) : base( from, guild, true, guild.Allies )
		{
		}

		protected override void Design()
		{
			AddHtml( 20, 10, 400, 35, m_Mobile.Language == "ITA" ? "Seleziona la gilda da cui rimuovere l'alleanza." : "Select the guild you wish to remove alliance with.", false, false );

			AddButton( 20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 55, 400, 245, 30, 1011138, false, false ); // Send the olive branch.

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
							m_Guild.RemoveAlly( g );
							m_Guild.GuildTextMessage( m_Mobile.Language == "ITA" ? "Messaggio di Gilda: Non sei più alleato con questa gilda: {0} ({1})" : "Guild Message: You are now not in alliance with this guild: {0} ({1})", g.Name, g.Abbreviation );

							GuildGump.EnsureClosed( m_Mobile );

							if( m_Guild.Enemies.Count > 0 )
								m_Mobile.SendGump( new GuildRemoveAllianceGump( m_Mobile, m_Guild ) );
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