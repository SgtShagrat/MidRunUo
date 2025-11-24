/***************************************************************************
 *                                  SetInfoPrompt.cs
 *                            		----------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Midgard.Misc;
using Server;
using Server.Mobiles;
using Server.Prompts;

namespace Midgard.Engines.MidgardTownSystem
{
    public class SetInfoPrompt : Prompt
    {
        private Mobile m_From;
        private Mobile m_Target;
        private InfoType m_Type;

        public SetInfoPrompt( Mobile from, Mobile target, InfoType type )
        {
            m_From = from;
            m_Target = target;
            m_Type = type;
        }

        public override void OnCancel( Mobile from )
        {
            if( m_Target.Deleted )
                return;

            Midgard2PlayerMobile m2pm = m_Target as Midgard2PlayerMobile;

            if( m2pm != null )
            {
                PersonalInfo info = m2pm.Info;

                if( m2pm.Info == null )
                    m2pm.Info = new PersonalInfo( m2pm );

                string type = string.Empty;

                switch( m_Type )
                {
                    case InfoType.Email:
                        type = "e-mail";
                        info.Email = String.Empty;
                        break;
                    case InfoType.ICQ:
                        type = "ICQ contact";
                        info.IcqContact = String.Empty;
                        break;
                    case InfoType.MSN:
                        type = "MSN contact";
                        info.MsnContact = String.Empty;
                        break;
                    default:
                        break;
                }

                if( !String.IsNullOrEmpty( type ) )
                    from.SendMessage( "You have reset {0} info of player {1}.", type, m_Target.Name );
            }

            from.SendGump( new CitizenInfoGump( TownSystem.Find( m_Target ), m_Target, from ) );
        }

        public override void OnResponse( Mobile from, string text )
        {
            Midgard2PlayerMobile m2pm = m_Target as Midgard2PlayerMobile;
            if( m2pm == null )
                return;

            PersonalInfo info = m2pm.Info;
            if( info == null )
                m2pm.Info = new PersonalInfo( m2pm );

            text = text.Trim();

            if( text.Length > 20 )
                text = text.Substring( 0, 20 );

            if( text.Length > 0 )
            {
                string type = String.Empty;

                switch( m_Type )
                {
                    case InfoType.Email:
                        type = "e-mail";
                        info.Email = text;
                        break;
                    case InfoType.ICQ:
                        type = "ICQ contact";
                        info.IcqContact = text;
                        break;
                    case InfoType.MSN:
                        type = "MSN contact";
                        info.MsnContact = text;
                        break;
                    default:
                        break;
                }

                if( !String.IsNullOrEmpty( type ) )
                    from.SendMessage( "You have set {0} info of player {1} to {2}.", type, m_Target.Name, text );
            }

            from.SendGump( new CitizenInfoGump( TownSystem.Find( m_Target ), m_Target, from ) );
        }
    }
}