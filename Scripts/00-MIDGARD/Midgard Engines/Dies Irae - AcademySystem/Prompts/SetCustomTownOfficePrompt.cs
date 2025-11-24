/***************************************************************************
 *                               SetCustomTownOfficePrompt.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;
using Server.Prompts;

namespace Midgard.Engines.Academies
{
    public class SetCustomAcademyOfficePrompt : Prompt
    {
        private readonly Mobile m_Target;

        public SetCustomAcademyOfficePrompt( Mobile target )
        {
            m_Target = target;
        }

        public override void OnCancel( Mobile from )
        {
            if( m_Target.Deleted )
                return;

            AcademyPlayerState tps = AcademyPlayerState.Find( m_Target );
            if( tps != null )
            {
                tps.CustomAcademyOffice = null;
                from.SendMessage( "You have reset that custom town office." );
            }

            from.SendGump( new AcademyInfoGump( AcademySystem.Find( m_Target ), m_Target, from ) );
        }

        public override void OnResponse( Mobile from, string text )
        {
            Midgard2PlayerMobile m2Pm = m_Target as Midgard2PlayerMobile;
            if( m2Pm == null )
                return;

            AcademyPlayerState tps = AcademyPlayerState.Find( m_Target );
            if( tps != null )
            {
                text = text.Trim();

                if( text.Length > 20 )
                    text = text.Substring( 0, 20 );

                if( text.Length > 0 )
                {
                    tps.CustomAcademyOffice = text;
                    from.SendMessage( "You have set custom town office for player {0} to '{1}'.", m_Target.Name, text );
                }
            }

            from.SendGump( new AcademyInfoGump( AcademySystem.Find( m_Target ), m_Target, from ) );
        }
    }
}