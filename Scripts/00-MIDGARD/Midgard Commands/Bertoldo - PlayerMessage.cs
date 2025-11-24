// #define DebugPM

/***************************************************************************
 *                                    PrivateMessageSystem.cs
 *                            		---------------------------
 *  begin                	: Luglio 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  original concept		: Bertoldo
 * 	rebuild					: Dies Irae
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:	
 * 			Il comando [PlayerMessage (alias [PM) permette di inviare un 
 * 			breve messaggio ad un altro player.
 * 			I messaggi vengono loggati in Logs\Midgard2PlayerMessagesLog.txt
 * 
 * 			Il comando [PlayerMessageIgnore (alias [PMIgnore) permette di 
 * 			mettere in ignore list un player online fino al logout o di rimuoverlo
 * 			dalla IgnoreList se vi e' presente.

 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Midgard.ContextMenus;

using Server;
using Server.Commands;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Midgard.Gumps
{
    public class PrivateMessageSystem
    {
        private static bool Enabled = false;

        public static void Initialize()
        {
            if( Enabled )
            {
                CommandSystem.Register( "PM", AccessLevel.Player, new CommandEventHandler( PlayerMessageOnCommand ) );
                CommandSystem.Register( "PlayerMessage", AccessLevel.Player, new CommandEventHandler( PlayerMessageOnCommand ) );
                CommandSystem.Register( "PMIgnore", AccessLevel.Player, new CommandEventHandler( PlayerMessageIgnoreOnCommand ) );
                CommandSystem.Register( "PlayerMessageIgnore", AccessLevel.Player, new CommandEventHandler( PlayerMessageIgnoreOnCommand ) );
                CommandSystem.Register( "PMIgnoreList", AccessLevel.Player, new CommandEventHandler( PlayerMessageIgnoreListOnCommand ) );
            }
        }

        [Usage( "PM or PlayerMessage" ), Description( "Apre il menu per mandare un messaggio personale." )]
        public static void PlayerMessageOnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( from == null || from.Deleted )
                return;

            if( from.Region.IsPartOf( typeof( Jail ) ) || from.Region.IsPartOf( "Hard Labour Penitentiary" ) )
                from.SendLocalizedMessage( 1065190 ); // Think about your sins instead of sending messages!
            else
            {
                try
                {
                    from.SendGump( new PlayerListGump( from ) );
                }
                catch( Exception ex )
                {
                    using( TextWriter t = new StreamWriter( "PlayerMessageSystemErrors.txt", true ) )
                    {
                        t.WriteLine( ex.ToString() );
                    }
                }
            }
        }

        [Usage( "PMIgnore <nomePG> or PlayerMessageIgnore <nomePG>" ), Description( "Aggiunge o rimuove un giocatore dalla IgnoreList" )]
        public static void PlayerMessageIgnoreOnCommand( CommandEventArgs e )
        {
            Midgard2PlayerMobile source = e.Mobile as Midgard2PlayerMobile;

            if( source == null || source.Deleted )
                return;

            if( string.IsNullOrEmpty( e.ArgString ) )
            {
                source.SendLocalizedMessage( 1065191 ); // Usage: PMIgnore <player name to ignore>
                return;
            }

            Mobile target = FindPlayerByName( e.ArgString );

            if( target == null || target.Deleted )
            {
                source.SendLocalizedMessage( 1065192 ); // No player online found with that name.
                return;
            }
            else if( target.AccessLevel > AccessLevel.Player )
            {
                source.SendLocalizedMessage( 1065193 ); // You cannot ignore Staff members.
                return;
            }
            else if( target == source )
            {
                source.SendLocalizedMessage( 1065194 ); // You cannot ignore yourself.
                return;
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                if( source.PlayerMessageIgnoreList.Contains( target ) )
                {
                    source.PlayerMessageIgnoreList.Remove( target );

                    sb.AppendFormat( "{0}, you have <em>removed</em> {1} from your Player Message IgnoreList.", source.Name, target.Name );

                    source.SendGump( new NoticeGump( 1060635, 30720, sb.ToString(), 0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[] { } ) );
                }
                else
                {
                    source.PlayerMessageIgnoreList.Add( target );

                    sb.AppendFormat( "{0}, you have <em>added</em> {1} to your PM IgnoreList.\n", source.Name, target.Name );
                    sb.AppendFormat( "No more messages will be sent to you from {0} neither you will be able to send messages to {0}.\n", target.Name );
                    sb.AppendFormat( "The effect of Player Message Ignore will last until your log-off or server restarts.\n" );
                    sb.AppendFormat( "You can remove {0} at any time from your Player Message IgnoreList using <em>[PMIgnore {0}</em> again.", target.Name );

                    source.SendGump( new NoticeGump( 1060635, 30720, sb.ToString(), 0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[] { } ) );
                }
            }
        }

        [Usage( "PMIgnoreList" ), Description( "Lista tutti i giocatori ignorati." )]
        public static void PlayerMessageIgnoreListOnCommand( CommandEventArgs e )
        {
            Midgard2PlayerMobile m2pm = e.Mobile as Midgard2PlayerMobile;

            if( m2pm == null || m2pm.Deleted )
                return;

            if( m2pm.PlayerMessageIgnoreList != null && m2pm.PlayerMessageIgnoreList.Count > 0 )
            {
                m2pm.SendMessage( "You have ignored:" );
                for( int i = 0; i < m2pm.PlayerMessageIgnoreList.Count; i++ )
                {
                    Mobile m = m2pm.PlayerMessageIgnoreList[ i ];
                    if( m != null && !String.IsNullOrEmpty( m.Name ) )
                    {
                        m2pm.SendMessage( m.Name );
                    }
                }
            }
            else
            {
                m2pm.SendMessage( "You have not ignored any player." );
            }
        }

        private static void CloseNoticeCallback( Mobile from, object state )
        {
        }

        #region context menus
        public static void GetSelfContextMenus( Mobile from, Mobile player, List<ContextMenuEntry> list )
        {
            if( from.CheckAlive() )
                list.Add( new CallbackPlayerEntry( 1038, new ContextPlayerCallback( ToggleAcceptPrivateMessages ), player ) ); // Toggle Accepting Private Messages
        }

        /// <summary>
        /// Metodo invocato quando un player vuole abilitare o disabilitare
        /// l'accettazione dei messaggi privati
        /// </summary>
        private static void ToggleAcceptPrivateMessages( Mobile from )
        {
            if( !from.CheckAlive() )
                return;

            Midgard2PlayerMobile player = from as Midgard2PlayerMobile;
            if( player == null )
                return;

            // 1064253 You have chosen to refuse private messages from other players.
            // 1064254 You have chosen to accept private messages from other players.
            player.SendLocalizedMessage( player.AcceptPrivateMessages ? 1064253 : 1064254 );

            player.AcceptPrivateMessages = !player.AcceptPrivateMessages;
        }
        #endregion

        private static void AddBlackAlpha( int x, int y, int width, int height, Gump g )
        {
            if( g == null )
                return;

            g.AddImageTiled( x, y, width, height, 2624 );
            g.AddAlphaRegion( x, y, width, height );
        }

        private static Mobile FindPlayerByName( string name )
        {
            if( string.IsNullOrEmpty( name ) )
                return null;

            try
            {
                List<NetState> states = NetState.Instances;

                for( int i = 0; i < states.Count; ++i )
                {
                    Mobile m = states[ i ].Mobile;
                    if( m != null && m.Name != null && Insensitive.Compare( m.Name, name ) == 0 )
                        return m;
                }
            }
            catch( Exception ex )
            {
                using( TextWriter t = new StreamWriter( "PlayerMessageSystemErrors.txt", true ) )
                {
                    t.WriteLine( ex.ToString() );
                }
            }

            return null;
        }

        private static bool IsDeletedOrOffline( Mobile from, Mobile target, bool warn )
        {
            PlayerMobile m_Target = target as PlayerMobile;

            if( m_Target == null )
                return true;

            if( from == null || from.Deleted )
                return true;

            if( m_Target.Deleted )
            {
                if( warn )
                    from.SendLocalizedMessage( 1065195 ); // That player has deleted their character.
                return true;
            }
            else if( m_Target.NetState == null )
            {
                if( warn )
                    from.SendLocalizedMessage( 1065196 ); // That player is no longer online.
                return true;
            }
            return false;
        }

        private static bool IsBusy( Mobile from )
        {
            return from != null && (
                from.HasGump( typeof( PlayerListGump ) ) ||
                from.HasGump( typeof( MessageComposerGump ) ) ||
                from.HasGump( typeof( MessageNotificationGump ) ) ||
                from.HasGump( typeof( MessageReceivedGump ) ) );
        }

        private static void LogError( Exception ex )
        {
            try
            {
                using( TextWriter t = new StreamWriter( "Logs/PlayerMessageSystemErrors.txt", true ) )
                {
                    t.WriteLine( ex.ToString() );
                }
            }
            catch
            {
            }
        }

        public class PlayerListGump : Gump
        {
            #region costanti
            private static readonly int m_Fields = 15;
            private static readonly int m_HueTit = 15;
            private static readonly int m_HuePrim = 92;
            private static readonly int m_HueSec = 87;
            private static readonly int m_DeltaBut = 2;
            #endregion

            #region campi
            private Mobile m_From;
            private List<Mobile> m_MobsTo;
            private int m_Page;
            #endregion

            #region costruttori
            public PlayerListGump( Mobile from )
                : this( from, BuildList( from ), 1 )
            {
            }

            public PlayerListGump( Mobile from, List<Mobile> mobsTo, int page )
                : base( 50, 50 )
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                from.CloseGump( typeof( PlayerListGump ) );

                m_From = from;
                m_MobsTo = mobsTo;
                m_Page = page;

                Design();
            }
            #endregion

            #region metodi
            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                try
                {
                    if( info.ButtonID == 0 )
                        return;							// Chiudi

                    if( info.ButtonID == 200 ) 		// Pagina precedente
                    {
                        m_Page--;
                        from.SendGump( new PlayerListGump( from, m_MobsTo, m_Page ) );
                    }
                    else if( info.ButtonID == 300 ) 	// Pagina successiva
                    {
                        m_Page++;
                        from.SendGump( new PlayerListGump( from, m_MobsTo, m_Page ) );
                    }
                    else
                    {
                        if( info.ButtonID - 1 > m_MobsTo.Count ) // sanity
                            return;

                        Mobile target = m_MobsTo[ info.ButtonID - 1 ];

                        if( target == null || target.Deleted ) // sanity
                        {
                            from.SendMessage( "That player is offline." );
                            return;
                        }

                        if( IsBusy( target ) )
                            from.SendMessage( "That player cannot receive a message because of a pending message. Try later..." );
                        else if( !IsDeletedOrOffline( from, target, true ) )
                            from.SendGump( new MessageComposerGump( from, target ) );
                    }
                }
                catch( Exception ex )
                {
                    LogError( ex );
                }
            }

            private static List<Mobile> BuildList( Mobile from )
            {
                List<Mobile> list = new List<Mobile>();
                List<NetState> states = NetState.Instances;

                Midgard2PlayerMobile source = from as Midgard2PlayerMobile;
                if( source == null || source.Deleted )
                    return list;

                for( int i = 0; i < states.Count; ++i )
                {
                    Midgard2PlayerMobile target = states[ i ].Mobile as Midgard2PlayerMobile;

                    // Se non è Online o è stato deletato non addarlo
                    if( target == null || target.Deleted )
                        continue;

                    // Se il target è se stesso non addarlo
                    else if( target == source )
                        continue;

                    // Se il target è celato allora non addarlo
                    else if( !target.OnlineVisible )
                        continue;

                    // Se non accetta PM allora non addarlo
                    else if( !target.AcceptPrivateMessages )
                        continue;

                    // Se e' nella propria IgnoreList allora non addarlo
                    else if( source.PlayerMessageIgnoreList.Contains( target ) )
                        continue;

                    // Se e' nella IgnoreList del target allora non addarlo
                    else if( target.PlayerMessageIgnoreList.Contains( source ) )
                        continue;

                    // Altrimenti addalo
                    list.Add( target );
                }

                list.Sort( InternalComparer.Instance );

                return list;
            }

            public void Design()
            {
                AddPage( 0 );

                AddBackground( 10, 10, 200, 390, 83 );
                AddBlackAlpha( 20, 20, 180, 370, this );
                AddLabel( 80, 20, m_HueTit, "Midgard" );
                AddLabel( 40, 40, m_HueTit, "Player Message System" );

                for( int i = 0; i < m_MobsTo.Count; i++ )
                {
                    Mobile m = m_MobsTo[ i ];
                    if( m == null || m.Deleted )
                        continue;

                    int page = i / m_Fields;
                    int pos = i % m_Fields;

                    int hue = m_HuePrim;

                    if( pos == 0 )
                    {
                        if( page > 0 )
                            AddButton( 180, 60, 5601, 5605, 20000, GumpButtonType.Page, page + 1 ); // Next

                        AddPage( page + 1 );

                        if( page > 0 )
                            AddButton( 160, 60, 5603, 5607, 20000, GumpButtonType.Page, page ); // Back
                    }

                    hue = ( hue == m_HuePrim ? m_HueSec : m_HuePrim );

                    int y = pos * 20 + 90;

                    if( m_MobsTo[ i ].AccessLevel > m_From.AccessLevel )
                        AddLabel( 30, y, 46, "Staff Player" );
                    else
                    {
                        AddLabel( 30, y, hue, m_MobsTo[ i ].Name );
                        AddButton( 180, y + m_DeltaBut, 9762, 9763, i + 1, GumpButtonType.Reply, 0 );
                    }
                }
            }
            #endregion

            private class InternalComparer : IComparer<Mobile>
            {
                public static readonly IComparer<Mobile> Instance = new InternalComparer();

                public int Compare( Mobile x, Mobile y )
                {
                    if( x == null || y == null )
                        throw new ArgumentException();

                    return Insensitive.Compare( x.Name, y.Name );
                }
            }
        }

        public class MessageComposerGump : Gump
        {
            #region campi
            private Mobile m_From;
            private Mobile m_To;
            #endregion

            #region costruttori
            public MessageComposerGump( Mobile from, Mobile to )
                : base( 50, 50 )
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_From = from;
                m_To = to;

                m_From.CloseGump( typeof( MessageComposerGump ) );

                AddPage( 0 );
                AddBackground( 0, 0, 390, 250, 83 );
                AddBlackAlpha( 10, 10, 370, 230, this );

                AddButton( 350, 22, 5601, 5605, 100, GumpButtonType.Reply, 0 );

                AddImageTiled( 20, 80, 350, 150, 2624 );
                AddTextEntry( 20, 80, 350, 150, 55, 2, string.Empty );

                AddLabel( 20, 60, 15, @"Type your message here:" );
                AddLabel( 310, 20, 15, @"Send" );

                AddLabel( 30, 20, 34, @"Message from: " );
                AddLabel( 30, 40, 65, @"Message to:" );
                AddLabel( 150, 20, 34, m_From.Name );
                AddLabel( 150, 40, 65, m_To.Name );
            }
            #endregion

            #region metodi
            public override void OnResponse( NetState state, RelayInfo info )
            {
                if( info.ButtonID != 100 )		// Chiudi
                    return;

                if( IsDeletedOrOffline( m_From, m_To, true ) )
                    return;

                if( IsBusy( m_To ) )
                    m_From.SendMessage( "That player cannot receive a message because of a pending message. Try later..." );

                TextRelay text = info.GetTextEntry( 2 );

                try
                {
                    TextWriter tw = File.AppendText( "Logs/Midgard2PlayerMessagesLog.txt" );
                    tw.WriteLine( "PM mandato dal pg {0} (account {1}), al pg {2} (account {3}) in data {4} alle ore {5}. Contenuto: {6}",
                                  m_From.Name, m_From.Account.Username,
                                  m_To.Name, m_To.Account.Username,
                                  DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(),
                                  text.Text );
                    tw.Close();
                }
                catch( Exception ex )
                {
                    Console.Write( "Player Message Log Failed due to Exception: {0}", ex );
                }

                try
                {
                    m_To.SendGump( new MessageNotificationGump( m_From, m_To, text.Text ) );
                }
                catch( Exception ex )
                {
                    LogError( ex );
                }
            }
            #endregion
        }

        public class MessageNotificationGump : Gump
        {
            #region campi
            private Mobile m_From;
            private Mobile m_To;
            private string m_Text;
            #endregion

            #region costruttori
            public MessageNotificationGump( Mobile from, Mobile to, string text )
                : base( 50, 50 )
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_From = from;
                m_To = to;
                m_Text = text;

                m_From.CloseGump( typeof( MessageNotificationGump ) );

                if( IsDeletedOrOffline( m_From, m_To, true ) )
                    return;

                if( string.IsNullOrEmpty( m_Text ) )
                    return;

                if( IsBusy( m_To ) )
                    return;

                AddPage( 0 );
                AddButton( 30, 30, 5517, 5518, 101, GumpButtonType.Reply, 0 );
            }
            #endregion

            #region metodi
            public override void OnResponse( NetState state, RelayInfo info )
            {
                try
                {
                    m_To.SendGump( new MessageReceivedGump( m_From, m_To, m_Text ) );
                }
                catch( Exception ex )
                {
                    LogError( ex );
                }
            }
            #endregion
        }

        public class MessageReceivedGump : Gump
        {
            #region campi
            private Mobile m_From;
            private Mobile m_To;
            private string m_Text;
            #endregion

            #region costruttori
            public MessageReceivedGump( Mobile from, Mobile to, string text )
                : base( 50, 50 )
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_From = from;
                m_To = to;
                m_Text = text;

                m_To.CloseGump( typeof( MessageReceivedGump ) );

                if( IsDeletedOrOffline( m_From, m_To, true ) )
                    return;

                if( string.IsNullOrEmpty( m_Text ) )
                    return;

                if( IsBusy( m_To ) )
                    return;

                AddPage( 0 );
                AddImage( 13, 168, 2080 );
                AddImage( 30, 204, 2081 );
                AddLabel( 48, 200, 593, "Messagge from:" );
                AddLabel( 140, 200, 488, m_From.Name );
                AddImage( 30, 274, 2081 );
                AddImage( 32, 344, 2083 );

                AddButton( 57, 302, 9004, 9004, 1, GumpButtonType.Reply, 0 );
                // AddImage(57, 302, 9004);

                AddHtml( 53, 225, 218, 81, @m_Text, false, true );
                AddLabel( 120, 315, 488, m_From.Name );
            }
            #endregion

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                try
                {
                    if( info.ButtonID == 1 )
                    {
                        if( m_From == null || m_From.Deleted ) // sanity
                        {
                            m_To.SendMessage( "That player is offline." );
                            return;
                        }

                        if( m_From.HasGump( typeof( MessageComposerGump ) ) ||
                            m_From.HasGump( typeof( MessageNotificationGump ) ) ||
                            m_From.HasGump( typeof( MessageReceivedGump ) ) )
                        {
                            m_To.SendMessage( "That player cannot receive a message because of a pending message. Try later..." );
                        }
                        else if( !IsDeletedOrOffline( m_To, m_From, true ) )
                        {
                            m_To.SendGump( new MessageComposerGump( m_To, m_From ) );
                        }
                    }
                }
                catch( Exception ex )
                {
                    LogError( ex );
                }
            }
        }
    }
}