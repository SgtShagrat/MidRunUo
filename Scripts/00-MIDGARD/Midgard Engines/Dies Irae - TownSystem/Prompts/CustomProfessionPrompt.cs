/***************************************************************************
 *                                  SetCustomProfessionPrompt.cs
 *                            		----------------------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;
using Server.Prompts;

namespace Midgard.Engines.MidgardTownSystem
{
    public class SetCustomProfessionPrompt : Prompt
    {
        private Mobile m_From;
        private Mobile m_Target;

        public SetCustomProfessionPrompt( Mobile from, Mobile target )
        {
            m_From = from;
            m_Target = target;
        }

        public override void OnCancel( Mobile from )
        {
            if( m_Target.Deleted )
                return;

            TownPlayerState tps = TownPlayerState.Find( m_Target );
            if( tps != null )
            {
                tps.CustomProfession = null;
                from.SendMessage( "You have reset that custom pofession." );
            }

            from.SendGump( new CitizenInfoGump( TownSystem.Find( m_Target ), m_Target, from ) );
        }

        public override void OnResponse( Mobile from, string text )
        {
            Midgard2PlayerMobile m2pm = m_Target as Midgard2PlayerMobile;
            if( m2pm == null )
                return;

            TownPlayerState tps = TownPlayerState.Find( m_Target );
            if( tps != null )
            {
                text = text.Trim();

                if( text.Length > 20 )
                    text = text.Substring( 0, 20 );

                if( text.Length > 0 )
                {
                    tps.CustomProfession = text;
                    from.SendMessage( "You have set custom profession for player {0} to '{1}'.", m_Target.Name, text );
                }
            }

            from.SendGump( new CitizenInfoGump( TownSystem.Find( m_Target ), m_Target, from ) );
        }
    }
}