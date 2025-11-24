/***************************************************************************
 *                                      SetName.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Commands;
using Server.Targeting;

namespace Midgard.Commands
{
    public class SetName
    {
        public static void Initialize()
        {
            CommandSystem.Register( "SetName", AccessLevel.Counselor, new CommandEventHandler( SetName_OnCommand ) );
        }

        [Usage( "SetName \"<nome>\"" )]
        [Description( "Setta il nome ad un oggetto o a un pg" )]
        public static void SetName_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 1 )
            {
                string name = e.GetString( 0 );
                if( name.Length > 0 && name.Length <= 20 )
                {
                    from.SendMessage( "Selezione l'oggetto o il pg da rinominare" );
                    from.Target = new InternalTarget( name );
                }
                else
                {
                    from.SendMessage( "Il nome non e' valido. Deve contenere al messimo 20 caratteri." );
                }
            }
            else
            {
                from.SendMessage( "Uso del comando: SetName \"<nome>\" " );
            }
        }

        private class InternalTarget : Target
        {
            private string m_Name;

            public InternalTarget( string name )
                : base( 12, false, TargetFlags.None )
            {
                m_Name = name;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                try
                {
                    if( targeted is Mobile )
                    {
                        ( (Mobile)targeted ).Name = m_Name;
                    }
                    else if( targeted is Item )
                    {
                        ( (Mobile)targeted ).Name = m_Name;
                    }
                    else
                    {
                        from.SendMessage( "Devi targhettare o un Mobile o un Item" );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "Il rename non riuscito dal pg staff {0} ha creato la seguente eccezione: {1}", from.Name, ex );
                }
            }
        }
    }
}