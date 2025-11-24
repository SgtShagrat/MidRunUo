#if !DONOTCOMPILEDEBUGSCRIPT_

using System.Collections.Generic;
using System.Xml.Linq;

using Server;
using Server.Commands;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.RazorRpvRecorder
{
    internal class Commands
    {
        internal static void RegisterCommands()
        {
            CommandSystem.Register( "ToggleRecordingStates", AccessLevel.Developer, new CommandEventHandler( ToggleRecordingStates_OnCommand ) );
            CommandSystem.Register( "StartRecord", AccessLevel.GameMaster, new CommandEventHandler( StartRecord_OnCommand ) );
        }

        [Usage( "ToggleRecordingStates <true | false>" )]
        [Description( "Toggles the state of all recording mobiles on the shard." )]
        public static void ToggleRecordingStates_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 1 )
            {
                bool enabled = e.GetBoolean( 0 );

                RecorderMobile.ToggleRecordingStates( e.Mobile, enabled );
                e.Mobile.SendMessage( "All recording mobiles have been {0}.", enabled ? "enabled" : "disabled" );
            }
            else
            {
                e.Mobile.SendMessage( "Format: ToggleRecordingStates <true | false>" );
            }
        }

        [Usage( "StartRecord <optional description>" )]
        [Description( "Start Video Recording" )]
        public static void StartRecord_OnCommand( CommandEventArgs e )
        {
            if( e.Length > 1 )
            {
                e.Mobile.SendMessage( "Format: StartRecord <optional description>" );
                return;
            }

            string description = null;

            if( e.Length == 1 )
            {
                description = e.GetString( 0 );
            }

            e.Mobile.SendMessage( "Select cameramen" );
            e.Mobile.Target = new RecorderTarger( description );
        }

        private class RecorderTarger : Target
        {
            private readonly string m_Description;

            public RecorderTarger( string description )
                : base( 15, false, TargetFlags.None )
            {
                m_Description = description;
            }

            protected override void OnTarget( Mobile from, object targ )
            {
                if( ( !( targ is RecorderMobile ) ) )
                {
                    return;
                }

                RecorderMobile recorder = (RecorderMobile)targ;

                if( !recorder.Deleted )
                {
                    if( recorder.Recording )
                    {
                        recorder.PrivateOverheadMessage( MessageType.Regular, 0, true, "I'm already recording, sir.", from.NetState );
                        return;
                    }

                    string finaldesc = !string.IsNullOrEmpty( m_Description ) ? m_Description + " - " : "";
                    finaldesc += " Ready... CIACK!";

                    recorder.PrivateOverheadMessage( MessageType.Yell, 0, true, finaldesc, from.NetState );

                    List<XElement> info = new List<XElement>();
                    info.Add( new XElement( "description", m_Description ) );
                    recorder.StartRecording( from, info );
                }
            }
        }
    }
}

#endif