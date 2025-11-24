/***************************************************************************
 *                                  CommandsLeveling.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Sistema per la gestione dinamica dei livelli di accesso ai
 * 			comandi.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Gumps;

namespace Midgard.Engines
{
    public class CommandsLeveling
    {
        internal static bool Enabled = false;

        public static void Initialize()
        {
            if( Enabled )
            {
                CommandSystem.Register( "CommandsLeveling", AccessLevel.Developer, new CommandEventHandler( CommandsLeveling_OnCommand ) );
                EventSink.Login += new LoginEventHandler( OnLogin );
            }
        }

        [Usage( "[CommandsLeveling {help}{<command> info}{<command> restore}{<command> <oldAccess> <newAccess>}" )]
        [Description( "Manage accesslevel on command system" )]
        private static void CommandsLeveling_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            string msg;

            #region help
            if( e.Length == 1 && Utility.InsensitiveCompare( e.GetString( 0 ), "help" ) == 0 )
            {
                DoNotice( from, "Con questo comando puoi cambiare istantaneamente e pernamentemente il livello di accesso " +
                                 "di qualsiasi comando in game.<br>" +
                                 "L'uso del comando è [CommandsLeveling <em>comando</em> <em>vecchioAccesso</em> <em>nuovoAccesso</em>.<br> " +
                                 "Ad esempio: [CommandsLeveling xmlfind Seer Administrator " );
            }
            #endregion
            #region getActualLevel
            else if( e.Length == 2 && Utility.InsensitiveCompare( e.GetString( 1 ), "info" ) == 0 )
            {
                string commandToGetInfo = e.GetString( 0 );

                if( !IsValidCommand( commandToGetInfo ) )
                    DoNotice( from, "Il comando inserito non esiste." );
                else
                {
                    CommandEntry entry = CommandSystem.Entries[ commandToGetInfo ];

                    if( entry != null )
                        DoNotice( from, String.Format( "Il comando <em>{0}</em> ha livello di accesso {1}", entry.Command, entry.AccessLevel.ToString() ) );
                }
            }
            #endregion
            #region restore
            else if( e.Length == 2 && Utility.InsensitiveCompare( e.GetString( 1 ), "restore" ) == 0 )
            {
                string commandToRestore = e.GetString( 0 );

                if( !IsValidCommand( commandToRestore ) || !HasMod( commandToRestore ) )
                    DoNotice( from, "Il comando inserito non esiste o non e' stato mai modificato." );
                else
                {
                    DefaultInfo info = m_Defaults[ commandToRestore ];

                    if( info != null )
                    {
                        msg = String.Format( "Stai per ripristinare permanentemente il livello di accesso '{0}' al comando '{1}'.<br>" +
                                             "Sei sicuro di voler procedere?", info.OldAccess.ToString(), info.OldCommand );

                        from.SendGump( new WarningGump( 1060635, 30720, msg, 0xFFC000, 420, 280, new WarningGumpCallback( ConfirmRestoreCallBack ), new object[] { commandToRestore }, true ) );
                    }
                }
            }
            #endregion
            #region changeLevel
            else if( e.Length == 3 )
            {
                string command = e.GetString( 0 );
                string actualAccessLevel = e.GetString( 1 );
                string newAccessLevel = e.GetString( 2 );

                if( String.IsNullOrEmpty( actualAccessLevel ) || String.IsNullOrEmpty( newAccessLevel ) )
                    return;
                else if( !IsValidCommand( command ) )
                    DoNotice( from, "Il comando inserito non esiste." );
                else if( !Enum.IsDefined( typeof( AccessLevel ), actualAccessLevel ) || !Enum.IsDefined( typeof( AccessLevel ), newAccessLevel ) )
                {
                    DoNotice( from, "Hai inserito un valore non valido per il livello di accesso. I valori ammissibili sono:<br>" +
                                    "Owner<br>Developer<br>Administrator<br>Seer<br>GameMaster<br>Conselor<br>Player." );
                }
                else
                {
                    AccessLevel actualLevel = (AccessLevel)Enum.Parse( typeof( AccessLevel ), actualAccessLevel );
                    AccessLevel newLevel = (AccessLevel)Enum.Parse( typeof( AccessLevel ), newAccessLevel );
                    CommandEntry entry = CommandSystem.Entries[ command ];

                    if( entry.AccessLevel != actualLevel )
                        DoNotice( from, "Il comando esiste ma hai inserito un valore errato del suo attuale livello di accesso. Controlla che sia corretto." );
                    else
                    {
                        msg = String.Format( "Stai per cambiare permanentemente il livello di accesso al comando '{0}'.<br>" +
                                             "L'attuale livello di accesso è: {1}.<br>" +
                                             "Il nuovo livello di accesso sarà: {2}.<br>" +
                                             "Sei sicuro di voler procedere?",
                                             command, actualAccessLevel, newAccessLevel );

                        from.SendGump( new WarningGump( 1060635, 30720, msg, 0xFFC000, 420, 280, new WarningGumpCallback( ConfirmChangeLevelCallBack ), new object[] { command, newLevel }, true ) );
                    }
                }
            }
            #endregion
            else
            {
                DoNotice( from, "Uso del comando: [CommandsLeveling <em>comando</em> <em>vecchioAccesso</em> <em>nuovoAccesso</em><br>" +
                                "Oppure: [CommandsLeveling <em>help</em><br>" +
                                "Oppure: [CommandsLeveling <em>comando</em> info<br>" +
                                "Oppure: [CommandsLeveling <em>command</em> restore." );
            }
        }

        private static void DoNotice( Mobile to, string notice )
        {
            to.SendGump( new NoticeGump( 1060635, 30720, notice, 0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallBack ), null ) );
        }

        private static void CloseNoticeCallBack( Mobile from, object state )
        {
        }

        private static void ConfirmRestoreCallBack( Mobile from, bool okay, object state )
        {
            object[] states = (object[])state;

            string command = (string)states[ 0 ];

            if( IsValidCommand( command ) )
                RestoreCommand( command );
        }

        private static void ConfirmChangeLevelCallBack( Mobile from, bool okay, object state )
        {
            object[] states = (object[])state;

            string command = (string)states[ 0 ];
            AccessLevel level = (AccessLevel)states[ 1 ];

            if( IsValidCommand( command ) )
                Access( command, level );
        }

        private static Dictionary<string, DefaultInfo> m_Defaults = new Dictionary<string, DefaultInfo>();
        private static List<DefaultInfo> m_InitInfo = new List<DefaultInfo>();
        private static bool m_Inited;

        private static bool IsValidCommand( string command )
        {
            return !String.IsNullOrEmpty( command ) && CommandSystem.Entries.ContainsKey( command );
        }

        private static bool HasMod( string command )
        {
            return m_Defaults[ command ] != null && CommandSystem.Entries[ command ] != null;
        }

        private static void RestoreCommand( string command )
        {
            try
            {
                if( !HasMod( command ) )
                {
                    m_Defaults.Remove( command );
                    CommandSystem.Entries.Remove( command );
                    return;
                }

                DefaultInfo info = m_Defaults[ command ];
                CommandEntry entry = new CommandEntry( info.OldCommand, CommandSystem.Entries[ command ].Handler, info.OldAccess );
                CommandSystem.Entries.Remove( command );

                if( HasMod( info.OldCommand ) )
                    RestoreCommand( info.OldCommand );

                CommandSystem.Entries[ info.OldCommand ] = entry;

                m_Defaults.Remove( command );

                foreach( BaseCommandImplementor imp in BaseCommandImplementor.Implementors )
                {
                    foreach( string str in imp.Commands.Keys )
                    {
                        if( str == command )
                        {
                            imp.Commands[ info.OldCommand ] = imp.Commands[ str ];
                            imp.Commands[ str ].AccessLevel = info.OldAccess;

                            if( str != info.OldCommand )
                                imp.Commands.Remove( str );
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                LogError( ex );
            }
        }

        private static void Access( string command, AccessLevel level )
        {
            try
            {
                if( CommandSystem.Entries[ command ] == null )
                    return;

                DefaultInfo info;

                if( !HasMod( command ) )
                {
                    info = new DefaultInfo();
                    info.OldCommand = command;
                    info.NewCommand = command;
                    info.NewAccess = level;
                    info.OldAccess = CommandSystem.Entries[ command ].AccessLevel;
                    m_Defaults[ command ] = info;
                }
                else
                {
                    info = m_Defaults[ command ];
                    info.NewAccess = level;
                }

                CommandEntry entry = new CommandEntry( command, CommandSystem.Entries[ command ].Handler, info.NewAccess );
                CommandSystem.Entries[ command ] = entry;

                foreach( BaseCommandImplementor imp in BaseCommandImplementor.Implementors )
                {
                    foreach( string str in imp.Commands.Keys )
                    {
                        if( str == command )
                            imp.Commands[ str ].AccessLevel = info.NewAccess;
                    }
                }
            }
            catch( Exception ex )
            {
                LogError( ex );
            }
        }

        private static void ApplyCommand( string old, string txt )
        {
            try
            {
                if( CommandSystem.Entries[ txt ] != null || CommandSystem.Entries[ old ] == null )
                    return;

                if( HasMod( old ) )
                {
                    m_Defaults[ old ].NewCommand = txt;
                    m_Defaults[ txt ] = m_Defaults[ old ];
                    m_Defaults.Remove( old );
                }
                else
                {
                    DefaultInfo info = new DefaultInfo();
                    info.OldCommand = old;
                    info.NewCommand = txt;
                    info.NewAccess = CommandSystem.Entries[ old ].AccessLevel;
                    info.OldAccess = info.NewAccess;
                    m_Defaults[ txt ] = info;
                }

                CommandSystem.Entries[ txt ] = CommandSystem.Entries[ old ];
                CommandSystem.Entries.Remove( old );

                foreach( BaseCommandImplementor imp in BaseCommandImplementor.Implementors )
                {
                    foreach( string str in imp.Commands.Keys )
                    {
                        if( str == old )
                        {
                            imp.Commands[ txt ] = imp.Commands[ str ];
                            imp.Commands.Remove( str );
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                LogError( ex );
            }
        }

        public static void Configure()
        {
            if( Enabled )
            {
                EventSink.WorldLoad += new WorldLoadEventHandler( OnLoad );
                EventSink.WorldSave += new WorldSaveEventHandler( OnSave );
            }
        }

        private static void OnSave( WorldSaveEventArgs e )
        {
            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( OnSave );

                if( !Directory.Exists( "Saves/Commands/" ) )
                    Directory.CreateDirectory( "Saves/Commands/" );

                GenericWriter writer = new BinaryFileWriter( Path.Combine( "Saves/Commands/", "Commands.bin" ), true );

                writer.Write( 0 ); // version
                writer.Write( m_Defaults.Values.Count );

                foreach( DefaultInfo info in m_Defaults.Values )
                    info.Serialize( writer );

                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch( Exception ex )
            {
                LogError( ex );
            }
        }

        private static void OnLoad()
        {
            try
            {
                if( !File.Exists( Path.Combine( "Saves/Commands/", "Commands.bin" ) ) )
                    return;

                using( FileStream bin = new FileStream( Path.Combine( "Saves/Commands/", "Commands.bin" ), FileMode.Open, FileAccess.Read, FileShare.Read ) )
                {
                    GenericReader reader = new BinaryFileReader( new BinaryReader( bin ) );

                    int version = reader.ReadInt();
                    int count = reader.ReadInt();

                    for( int i = 0; i < count; ++i )
                        m_InitInfo.Add( new DefaultInfo( reader ) );
                }
            }
            catch( Exception ex )
            {
                LogError( ex );
            }
        }

        private static void OnLogin( LoginEventArgs e )
        {
            if( m_Inited )
                return;

            m_Inited = true;

            foreach( DefaultInfo defaultInfo in m_InitInfo )
            {
                ApplyCommand( defaultInfo.OldCommand, defaultInfo.NewCommand );
                Access( defaultInfo.NewCommand, defaultInfo.NewAccess );
            }
        }

        private static void LogError( Exception ex )
        {
            try
            {
                using( TextWriter t = new StreamWriter( "Logs/CommandsLevelingErrors.txt", true ) )
                {
                    t.WriteLine( ex.ToString() );
                }
            }
            catch
            {
            }
        }

        private sealed class DefaultInfo
        {
            public string NewCommand { get; set; }
            public string OldCommand { get; set; }
            public AccessLevel NewAccess { get; set; }
            public AccessLevel OldAccess { get; set; }

            #region serialization
            public DefaultInfo( GenericReader reader )
            {
                NewCommand = reader.ReadString();
                OldCommand = reader.ReadString();
                NewAccess = (AccessLevel)reader.ReadInt();
                OldAccess = (AccessLevel)reader.ReadInt();
            }

            public DefaultInfo()
            {
            }

            public void Serialize( GenericWriter writer )
            {
                writer.Write( NewCommand );
                writer.Write( OldCommand );
                writer.Write( (int)NewAccess );
                writer.Write( (int)OldAccess );
            }
            #endregion
        }
    }
}