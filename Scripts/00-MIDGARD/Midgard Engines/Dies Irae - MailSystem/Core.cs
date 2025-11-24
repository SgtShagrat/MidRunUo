/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 31 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Engines.MailSystem
{
    public sealed class Core
    {
        #region Singleton pattern
        private static Core m_Instance;

        private Core()
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Mail System singleton instanced." );
        }

        public static Core Instance
        {
            get
            {
                if( m_Instance == null )
                    m_Instance = new Core();
                return m_Instance;
            }
        }
        #endregion

        private List<BaseMailBox> m_MailBoxes;

        private Dictionary<MidgardTowns, List<MailMessage>> m_Mails;

        private Dictionary<string, Mobile> m_ValidRecipients;

        internal static void ConfigSystem()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( Load );
            EventSink.WorldSave += new WorldSaveEventHandler( Save );

            EventSink.Login += new LoginEventHandler( OnLogin );
        }

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "Lettere", AccessLevel.Player, new CommandEventHandler( Mails_OnCommand ) );
        }

        #region serialization
        public static void Save( WorldSaveEventArgs e )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "{0} Saving...", Config.Pkg.Title );

            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                string dir = Path.Combine( Path.GetPathRoot( Config.MailSavePath ), Path.GetDirectoryName( Config.MailSavePath ) );
                if( !Directory.Exists( dir ) )
                    Directory.CreateDirectory( dir );

                BinaryFileWriter writer = new BinaryFileWriter( Config.MailSavePath, true );
                Instance.Serialize( writer );
                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch
            {
                Config.Pkg.LogErrorLine( "Error serializing {0}.", Config.Pkg.Title );
            }

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        public static void Load()
        {
            if( Config.Debug )
                Console.Write( "{0}: Loading...", Config.Pkg.Title );

            while( !File.Exists( Config.MailSavePath ) )
            {
                Console.WriteLine( "Warning: {0} not found.", Config.MailSavePath );
                Console.WriteLine( " - Press return to continue, or R to try again." );
                string str = Console.ReadLine();

                if( str == null || str.ToLower() != "r" )
                    return;
            }

            try
            {
                BinaryReader bReader = new BinaryReader( File.OpenRead( Config.MailSavePath ) );
                BinaryFileReader reader = new BinaryFileReader( bReader );
                new Core( reader );

                bReader.Close();
            }
            catch
            {
                Console.WriteLine( "Error deserializing {0}.", Config.Pkg.Title );
            }

            if( Config.Debug )
                Console.WriteLine( "done." );
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 ); // version

            writer.Write( m_Mails != null ? m_Mails.Count : 0 );
            if( m_Mails == null )
                return;

            foreach( KeyValuePair<MidgardTowns, List<MailMessage>> keyValuePair in m_Mails )
            {
                writer.Write( (int)keyValuePair.Key );

                List<MailMessage> mailsByTown = keyValuePair.Value;
                writer.Write( mailsByTown.Count );

                foreach( MailMessage t in mailsByTown )
                    t.Serialize( writer );
            }
        }

        public Core( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    {
                        int towns = reader.ReadInt();

                        for( int i = 0; i < towns; i++ )
                        {
                            MidgardTowns town = (MidgardTowns)reader.ReadInt();

                            List<MailMessage> mailsByTown = new List<MailMessage>();
                            int count = reader.ReadInt();

                            for( int j = 0; j < count; j++ )
                                mailsByTown.Add( new MailMessage( reader ) );

                            if( mailsByTown.Count > 0 )
                                m_Mails.Add( town, mailsByTown );
                        }
                        break;
                    }
            }
        }
        #endregion

        private static void OnLogin( LoginEventArgs e )
        {
            Mobile m = e.Mobile;
            if( m == null )
                return;

            Dictionary<MidgardTowns, int> mails;
            if( Instance.GetPendingMailsCount( m, out mails ) )
            {
                int total = 0;
                foreach( KeyValuePair<MidgardTowns, int> i in mails )
                {
                    total += i.Value;
                }

                if( total > 0 )
                    m.SendMessage( "You have {0} pending mails. Digit [Mails to get notified about their location.", total );
            }
        }

        internal static void BuildRecipientsList()
        {
            try
            {
                List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );

                foreach( Mobile m in mobs )
                {
                    if( m != null && m is PlayerMobile )
                        Instance.RegisterRecipient( m );
                }
            }
            catch( Exception ex )
            {
                Config.Pkg.LogErrorLine( ex.ToString() );
            }
        }

        [Usage( "Lettere" )]
        [Description( "Notifica il numero e il luogo dove sono depositate le proprie lettere." )]
        public static void Mails_OnCommand( CommandEventArgs e )
        {
            Mobile m = e.Mobile;
            if( m == null )
                return;

            Dictionary<MidgardTowns, int> mails;
            if( Instance.GetPendingMailsCount( m, out mails ) )
            {
                int total = 0;
                foreach( KeyValuePair<MidgardTowns, int> i in mails )
                    total += i.Value;

                if( total > 0 )
                {
                    m.SendMessage( "You have {0} pending mail{1}.", total, total == 1 ? "" : "s" );
                    foreach( KeyValuePair<MidgardTowns, int> i in mails )
                        m.SendMessage( "Mails in {0}: {1}", TownHelper.FindTownName( i.Key ), i.Value );
                }
                else
                    m.SendMessage( "You have not pending mails." );
            }
            else
                m.SendMessage( "You have not pending mails." );
        }

        private void RegisterRecipient( Mobile m )
        {
            if( m_ValidRecipients == null )
                m_ValidRecipients = new Dictionary<string, Mobile>();

            if( string.IsNullOrEmpty( m.Name ) )
                Config.Pkg.LogWarningLine( "Warning: null player name (serial {0})", m.Serial.Value );
            else if( !m_ValidRecipients.ContainsKey( m.Name ) )
                m_ValidRecipients.Add( m.Name, m );
            else
                Config.Pkg.LogWarningLine( "Warning: duplicate player name \"{0}\" has been found by mailsystem.", m.Name );
        }

        internal bool IsValidRecipient( string name )
        {
            if( m_ValidRecipients == null )
                return false;

            return m_ValidRecipients.ContainsKey( name );
        }

        internal Mobile GetReceiptByName( string name )
        {
            if( !IsValidRecipient( name ) )
                return null;
            else
            {
                foreach( KeyValuePair<string, Mobile> keyValuePair in m_ValidRecipients )
                {
                    if( Insensitive.Compare( keyValuePair.Value.Name, name ) == 0 )
                        return keyValuePair.Value;
                }
            }

            return null;
        }

        internal void RegisterBox( BaseMailBox box )
        {
            if( m_MailBoxes == null )
                m_MailBoxes = new List<BaseMailBox>();

            if( !m_MailBoxes.Contains( box ) )
                m_MailBoxes.Add( box );
        }

        internal void UnRegisterMessage( MidgardTowns town, MailMessage message )
        {
            if( message == null || m_Mails == null )
                return;

            if( !m_Mails.ContainsKey( town ) )
                return;

            List<MailMessage> mails = m_Mails[ town ];
            if( mails == null )
                return;

            if( mails.Contains( message ) )
                mails.Remove( message );

            InvalidateBoxes( town );
        }

        internal void RegisterMessage( MidgardTowns town, MailMessage message )
        {
            if( message == null )
                return;

            if( m_Mails == null )
                m_Mails = new Dictionary<MidgardTowns, List<MailMessage>>();

            List<MailMessage> mails;
            if( m_Mails.ContainsKey( town ) )
            {
                mails = m_Mails[ town ];
                if( mails == null )
                    mails = new List<MailMessage>();

                if( !mails.Contains( message ) )
                    mails.Add( message );
            }
            else
            {
                mails = new List<MailMessage>();
                mails.Add( message );
                m_Mails.Add( town, mails );
            }

            InvalidateBoxes( town );
        }

        private void InvalidateBoxes( MidgardTowns town )
        {
            List<BaseMailBox> boxes = GetMailBoxesByTown( town );
            if( boxes == null || boxes.Count < 1 )
                return;

            foreach( BaseMailBox t in boxes )
                t.InvalidateItemId( TownHasMails( town ) );
        }

        private List<BaseMailBox> GetMailBoxesByTown( MidgardTowns town )
        {
            List<BaseMailBox> list = new List<BaseMailBox>();
            foreach( BaseMailBox mailBox in m_MailBoxes )
            {
                if( mailBox.Town == town )
                    list.Add( mailBox );
            }

            return list.Count > 0 ? list : null;
        }

        private bool GetPendingMailsCount( Mobile m, out Dictionary<MidgardTowns, int> result )
        {
            result = new Dictionary<MidgardTowns, int>();

            foreach( int town in Enum.GetValues( typeof( MidgardTowns ) ) )
            {
                if( town == 0 )
                    continue;

                List<MailMessage> mails = GetMailsByTownForMobile( (MidgardTowns)town, m );

                if( mails != null && mails.Count > 0 )
                    result.Add( (MidgardTowns)town, mails.Count );
            }

            if( result.Count > 0 )
                return true;
            else
            {
                result = null;
                return false;
            }
        }

        internal List<MailMessage> GetMailsByTown( MidgardTowns town )
        {
            if( m_Mails == null )
                return null;

            if( m_Mails.ContainsKey( town ) )
                return m_Mails[ town ];
            else
                return null;
        }

        private bool TownHasMails( MidgardTowns town )
        {
            if( m_Mails == null )
                return false;

            if( m_Mails.ContainsKey( town ) )
                return m_Mails[ town ].Count > 0;
            else
                return false;
        }

        internal List<MailMessage> GetMailsByTownForMobile( MidgardTowns town, Mobile m )
        {
            List<MailMessage> mails = GetMailsByTown( town );

            if( mails != null && mails.Count > 0 )
            {
                List<MailMessage> result = new List<MailMessage>();

                foreach( MailMessage message in mails )
                {
                    if( message.To == m )
                        result.Add( message );
                }

                return result;
            }

            return null;
        }

        internal bool IsMailBoxNear( Mobile m, bool message )
        {
            bool isNear = false;

            IPooledEnumerable eable = m.Map.GetItemsInRange( m.Location, 3 );
            foreach( Item i in eable )
            {
                if( i != null && !i.Deleted && i is BaseMailBox )
                    isNear = true;
            }

            eable.Free();

            if( message && !isNear )
                m.SendMessage( "You must be near a mailbox to do this." );

            return isNear;
        }
    }
}