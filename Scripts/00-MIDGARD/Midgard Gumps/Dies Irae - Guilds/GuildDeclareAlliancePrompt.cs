using System.Collections.Generic;

using Server;
using Server.Guilds;
using Server.Gumps;
using Server.Prompts;

namespace Midgard.Gumps
{
	public class GuildDeclareAlliancePrompt : Prompt
	{
		private readonly Mobile m_Mobile;
		private readonly Guild m_Guild;

		public GuildDeclareAlliancePrompt( Mobile m, Guild g )
		{
			m_Mobile = m;
			m_Guild = g;
		}

		public override void OnCancel( Mobile from )
		{
			if( GuildGump.BadLeader( m_Mobile, m_Guild ) )
				return;

			GuildGump.EnsureClosed( m_Mobile );
			m_Mobile.SendGump( new GuildAllianceAdminGump( m_Mobile, m_Guild ) );
		}

		public override void OnResponse( Mobile from, string text )
		{
			if( GuildGump.BadLeader( m_Mobile, m_Guild ) )
				return;

			text = text.Trim();

			if( text.Length >= 3 )
			{
				List<Guild> guilds = Utility.CastConvertList<BaseGuild, Guild>( BaseGuild.Search( text ) );

				GuildGump.EnsureClosed( m_Mobile );

				if( guilds.Count > 0 )
				{
					m_Mobile.SendGump( new GuildDeclareAllianceGump( m_Mobile, m_Guild, guilds ) );
				}
				else
				{
					m_Mobile.SendGump( new GuildAllianceAdminGump( m_Mobile, m_Guild ) );
					m_Mobile.SendLocalizedMessage( 1018003 ); // No guilds found matching - try another name in the search
				}
			}
			else
			{
				m_Mobile.SendMessage( m_Mobile.Language == "ITA" ? "Per la ricerca devi inserire almeno 3 caratteri." : "Search string must be at least three letters in length." );
			}
		}
	}
}