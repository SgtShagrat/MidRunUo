// #define FACTIONS

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Server.Commands;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Midgard.Engines.SpellSystem;

namespace Server.Engines.XmlPoints
{
    public class XmlPointsAttach : XmlAttachment
    {
        public static readonly Point3D CoveLocation = new Point3D( 2230, 1224, 0 );
        private const int DefaultStartingPoints = 100; // 100 default starting points

        private List<KillEntry> m_KillList = new List<KillEntry>();
        private DateTime m_LastDecay;

        public DateTime CancelEnd;
        public CancelTimer MCancelTimer;

        public Point3D StartingLoc = CoveLocation;
        public Map StartingMap = Map.Felucca;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Points { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Rank { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DeltaRank { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime WhenRanked { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Credits { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Broadcast { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool ReceiveBroadcasts { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Kills { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Deaths { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastKill { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastDeath { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasChallenge
        {
            get { return ( ( Challenger != null && !Challenger.Deleted ) || ( ChallengeGame != null && !ChallengeGame.Deleted ) ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Challenger { get; set; }

        public BaseChallengeGame ChallengeGame { get; set; }

        public BaseChallengeGame ChallengeSetup { get; set; }

        public static bool TeleportOnDuel = true;
        // are players automatically teleported to and from the specified dueling location

        private static readonly TimeSpan m_DeathDelay = TimeSpan.FromSeconds( 60 );
        // 60 seconds default min time between deaths for point loss

        private static readonly TimeSpan m_KillDelay = TimeSpan.FromHours( 6 );
        // 6 hour default min interval between kills of the same player for point gain

        // set these scalings to determine points gained/lost based on the difference in points between the killer and the killed
        // default is set to 5% of the point difference (0.05).  regardless of the scaling at least 1 point will be gained/lost per kill
        private static double m_WinScale = 0.05;
        // set to zero for no scaling for points gained for killing (fixed 1 point per kill)

        private static double m_LoseScale = 0.05;
        // set to zero for no scaling for points lost when killed (fixed 1 point per death)

        private static double m_CreditScale = 0.05;

        // admin control of pvp-kill broadcasts. If false then no broadcasting. If true, then broadcasting is controlled by the player settings
        private static bool m_SystemBroadcast;

        private static TimeSpan m_LeaderboardSaveInterval = TimeSpan.FromMinutes( 15 );
        // default time interval between leaderboard saves

        private static string m_LeaderboardSaveDirectory = "Leaderboard";
        // default directory for saving leaderboard xml information

        private static int m_LeaderboardSaveRanks = 20;
        // number of ranked players to save to the leaderboard.  0 means save all players.

        private static readonly TimeSpan m_PointsDecayTime = TimeSpan.FromDays( 15 );
        // default time interval for automatic point loss for no pvp activity

        // set m_PointsDecay to zero to disable the automatic points decay feature
        private static int m_PointsDecay = 10; // default point loss if no kills are made within the PointsDecayTime

        // set m_CancelTimeout to determine how long it takes to cancel a challenge after it is requested
        public static TimeSpan CancelTimeout = TimeSpan.FromMinutes( 2 );

        public static bool AllowWithinGuildPoints = true; // allow within-guild challenge duels for points.

        public static bool UnrestrictedChallenges;

        // allows players to be autores'd following 1-on-1 duels
        // Team Challenge type matches handle their own autores behavior
        public static bool AutoResAfterDuel = true;

        public static bool GainHonorFromDuel = true;

        // log all kills that award points to the kills.log file
        public static bool LogKills = true;

        public static LeaderboardTimer m_LeaderboardTimer;
        public static string m_LeaderboardFile;
        public static List<RankEntry> RankList = new List<RankEntry>();
        public static bool m_Needsupdate = true;

        // this enum lists all supported languages
        public enum LanguageType
        {
            English
        }

        // there MUST be as many hashtable array entries as languages
        private static readonly Hashtable[] TextHash = new Hashtable[ 3 ];

        private LanguageType m_CurrentLanguage = LanguageType.English; // player selected language setting

        private static LanguageType m_SystemLanguage = LanguageType.English; // system default language setting

        public static DuelLocationEntry[] DuelLocations = new DuelLocationEntry[]
                                                              {
                                                                  //new DuelLocationEntry("Britain Arena", 1480, 1464, 17, Map.Felucca, 8), 
                                                                  //new DuelLocationEntry("Serpent's Hold Arena", 3030, 3386, 35, Map.Felucca, 7), 
                                                                  //new DuelLocationEntry("Luxor", 5395, 1246, 0, Map.Felucca, 12), 
                                                                  //new DuelLocationEntry("Prison Arena", 5476, 1254, 0, Map.Felucca, 18),	
		
                                                                  new DuelLocationEntry( "Mountain Arena", Map.Felucca, 
                                                                      new Point3D( 1055, 1543, 45 ), new Point3D( 1063, 1543, 45 ),
                                                                      new Rectangle2D( new Point2D( 1052, 1536 ), new Point2D( 1065, 1549 ) ) ),

                                                                  new DuelLocationEntry( "Sand Arena", Map.Felucca, 
                                                                      new Point3D( 1829, 955, 0 ), new Point3D( 1836, 955, 0 ),
                                                                      new Rectangle2D( new Point2D( 1826, 949 ), new Point2D( 1840, 962 ) ) ),

                                                                  new DuelLocationEntry( "Ice Arena", Map.Felucca, 
                                                                      new Point3D( 3963, 291, 35 ), new Point3D( 3972, 291, 35 ),
                                                                      new Rectangle2D( new Point2D( 3962, 284 ), new Point2D( 3974, 297 ) ) ),

                                                                  new DuelLocationEntry( "LostLands Arena", Map.Felucca, 
                                                                      new Point3D( 5139, 2904, 50 ), new Point3D( 5139, 2894, 50 ),
                                                                      new Rectangle2D( new Point2D( 5132, 2891 ), new Point2D( 5144, 2907 ) ) ),

                                                                 new DuelLocationEntry( "Lava Arena", Map.Felucca, 
                                                                      new Point3D( 4781, 3692, 100 ), new Point3D( 4791, 3682, 100 ),
                                                                      new Rectangle2D( new Point2D( 4779, 3680 ), new Point2D( 4793, 3694 ) ) ),

                                                                 new DuelLocationEntry( "Occlo Arena", Map.Felucca, 
                                                                      new Point3D( 3562, 2667, 10 ), new Point3D( 3574, 2669, 10 ),
                                                                      new Rectangle2D( new Point2D( 3565, 2677 ), new Point2D( 3575, 2680 ) ) ),

                                                                 new DuelLocationEntry( "Water Arena", Map.Felucca, 
                                                                      new Point3D( 6741, 1299, 0 ), new Point3D( 6752, 1299, 0 ),
                                                                      new Rectangle2D( new Point2D( 6739, 1291 ), new Point2D( 6755, 1306 ) ) ),

                                                                 new DuelLocationEntry( "Vesper Arena", Map.Felucca, 
                                                                      new Point3D( 2746, 1065, -10 ), new Point3D( 2753, 1056, -10 ),
                                                                      new Rectangle2D( new Point2D( 2741, 1050 ), new Point2D( 2757, 1069 ) ) )
                                                              };

        public static bool DuelLocationAvailable( DuelLocationEntry duelloc )
        {
            // check to see whether there are any players at the location
            if( duelloc == null || duelloc.Map == null )
                return true;

            foreach( Mobile m in duelloc.Map.GetMobilesInBounds( duelloc.Area ) )
            {
                if( m.Player )
                    return false;
            }

            return true;
        }

        public static bool CheckCombat( Mobile m )
        {
            return ( m != null && ( m.Aggressors.Count > 0 || m.Aggressed.Count > 0 ) );
        }

        #region [Text]

        [CommandProperty( AccessLevel.GameMaster )]
        public LanguageType CurrentLanguage
        {
            get { return m_CurrentLanguage; }
            set { m_CurrentLanguage = value; }
        }


        public static void AddText( LanguageType t, int index, string text )
        {
            var tindex = (int)t;

            if( tindex >= TextHash.Length )
            {
                Console.WriteLine( "XmlPoints: Invalid language type {0}: increase hashtable size", t );
                return;
            }


            if( TextHash[ tindex ] == null )
            {
                TextHash[ tindex ] = new Hashtable();
            }

            Hashtable h = TextHash[ tindex ];

            h.Add( index, text );
        }

        public static string SystemText( int index )
        {
            if( (int)m_SystemLanguage >= TextHash.Length )
            {
                // unsupported language
                // this should never happen
                return String.Format( "??Language {0}??", m_SystemLanguage );
            }

            Hashtable h = TextHash[ (int)m_SystemLanguage ];

            if( h == null || !h.Contains( index ) )
            {
                return String.Format( "??Entry {0}??", index );
            }

            return (string)h[ index ];
        }

        public string Text( int index )
        {
            if( (int)m_CurrentLanguage >= TextHash.Length )
            {
                // unsupported language
                // this should never happen
                Console.WriteLine( "XmlPoints: Unsupported language {0}", m_CurrentLanguage );
                return String.Format( "??Language {0}??", m_CurrentLanguage );
            }

            Hashtable h = TextHash[ (int)m_CurrentLanguage ];

            if( h == null || !h.Contains( index ) )
            {
                Console.WriteLine( "XmlPoints: Missing entry for {0} in language {1}", index, m_CurrentLanguage );
                // missing entry in the current language.  Try the default system language
                h = TextHash[ (int)m_SystemLanguage ];

                // still no entry, so make a final try in the default english
                if( h == null || !h.Contains( index ) )
                {
                    Console.WriteLine( "XmlPoints: Also missing entry for {0} in system language {1}", index, m_SystemLanguage );

                    // missing entry in the system language.  Try the default english
                    h = TextHash[ (int)LanguageType.English ];

                    // still no entry, so return null
                    if( h == null || !h.Contains( index ) )
                    {
                        Console.WriteLine( "XmlPoints: And finally missing entry for {0} in default language {1}", index, LanguageType.English );
                        return String.Format( "??Entry {0}??", index );
                    }
                }
            }

            return (string)h[ index ];
        }

        public static string GetText( Mobile from, int msgindex )
        {
            // go through the participant list and send all participants the message
            if( from == null )
                return "???";

            var a = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );

            if( a != null )
            {
                return a.Text( msgindex );
            }
            else
            {
                return SystemText( msgindex );
            }
        }

        public void SendText( int msgindex )
        {
            SendColorText( 0, msgindex );
        }

        public void SendText( int msgindex, object arg )
        {
            SendColorText( 0, msgindex, arg );
        }

        public void SendText( int msgindex, object arg, object arg2 )
        {
            SendColorText( 0, msgindex, arg, arg2 );
        }

        public void SendColorText( int color, int msgindex )
        {
            Mobile from = null;

            if( AttachedTo is Mobile )
            {
                from = (Mobile)AttachedTo;
            }
            // go through the participant list and send all participants the message
            if( from == null )
                return;

            from.SendMessage( color, String.Format( Text( msgindex ) ) );
        }

        public void SendColorText( int color, int msgindex, object arg, object arg2 )
        {
            Mobile from = null;

            if( AttachedTo is Mobile )
            {
                from = (Mobile)AttachedTo;
            }
            // go through the participant list and send all participants the message
            if( from == null )
                return;

            from.SendMessage( color, String.Format( Text( msgindex ), arg, arg2 ) );
        }

        public void SendColorText( int color, int msgindex, object arg )
        {
            Mobile from = null;

            if( AttachedTo is Mobile )
            {
                from = (Mobile)AttachedTo;
            }
            // go through the participant list and send all participants the message
            if( from == null )
                return;

            from.SendMessage( color, String.Format( Text( msgindex ), arg ) );
        }

        public static void SendText( Mobile from, int msgindex )
        {
            SendColorText( from, 0, msgindex );
        }

        public static void SendText( Mobile from, int msgindex, object arg )
        {
            SendColorText( from, 0, msgindex, arg );
        }

        public static void SendText( Mobile from, int msgindex, object arg, object arg2 )
        {
            SendColorText( from, 0, msgindex, arg, arg2 );
        }

        public static void SendColorText( Mobile from, int color, int msgindex )
        {
            // go through the participant list and send all participants the message
            if( from == null )
                return;

            var a = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );

            if( a != null )
            {
                from.SendMessage( color, a.Text( msgindex ) );
            }
        }

        public static void SendColorText( Mobile from, int color, int msgindex, object arg )
        {
            // go through the participant list and send all participants the message
            if( from == null )
                return;

            var a = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );

            if( a != null )
            {
                from.SendMessage( color, String.Format( a.Text( msgindex ), arg ) );
            }
        }

        public static void SendColorText( Mobile from, int color, int msgindex, object arg, object arg2 )
        {
            // go through the participant list and send all participants the message
            if( from == null )
                return;

            var a = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );

            if( a != null )
            {
                from.SendMessage( color, String.Format( a.Text( msgindex ), arg, arg2 ) );
            }
        }

        #endregion

        private static bool SameGuild( Mobile killed, Mobile killer )
        {
            return ( killer.Guild == killed.Guild && killer.Guild != null && killed.Guild != null );
        }

        public static void RefreshRankList()
        {
            if( m_Needsupdate && RankList != null )
            {
                RankList.Sort();

                int rank = 0;
                //int prevpoints = 0;
                foreach( RankEntry p in RankList )
                {
                    // bump the rank for every change in point level
                    // this means that people with the same points score will have the same rank
                    /*
					if(p.PointsAttachment.Points != prevpoints)
					{
						rank++;
					}

					prevpoints = p.PointsAttachment.Points;
					*/

                    // bump the rank for every successive player in the list.  Players with the same points total will be
                    // ordered by kills
                    rank++;

                    p.Rank = rank;
                }
                m_Needsupdate = false;
            }
        }

        public static int GetRanking( Mobile m )
        {
            if( RankList == null || m == null )
                return 0;

            RefreshRankList();

            // go through the sorted list and calculate rank

            foreach( RankEntry p in RankList )
            {
                // found the person?
                if( p.Killer == m )
                {
                    return p.Rank;
                }
            }

            // rank 0 means unranked
            return 0;
        }

        private static void UpdateRanking( Mobile m, XmlPointsAttach attachment )
        {
            if( RankList == null )
                RankList = new List<RankEntry>();

            // flag the rank list for updating on the next attempt to retrieve a rank
            m_Needsupdate = true;

            bool found = false;

            // rank the entries
            foreach( RankEntry p in RankList )
            {
                // found a match
                if( p != null && p.Killer == m )
                {
                    // update the entry with the new points value

                    p.PointsAttachment = attachment;
                    found = true;
                    break;
                }
            }

            // a new entry so add it
            if( !found )
            {
                RankList.Add( new RankEntry( m, attachment ) );
            }

            // if points statistics are being displayed in player name properties, then update them
            if( m != null )
                m.InvalidateProperties();
        }

        public static int GetCredits( Mobile m )
        {
            int val = 0;

            ArrayList list = XmlAttach.FindAttachments( m, typeof( XmlPointsAttach ) );
            if( list != null && list.Count > 0 )
            {
                val = ( (XmlPointsAttach)list[ 0 ] ).Credits;
            }

            return val;
        }

        public static int GetPoints( Mobile m )
        {
            int val = 0;

            ArrayList list = XmlAttach.FindAttachments( m, typeof( XmlPointsAttach ) );
            if( list != null && list.Count > 0 )
            {
                val = ( (XmlPointsAttach)list[ 0 ] ).Points;
            }

            return val;
        }

        public static bool HasCredits( Mobile m, int credits )
        {
            if( m == null || m.Deleted )
                return false;

            ArrayList list = XmlAttach.FindAttachments( m, typeof( XmlPointsAttach ) );
            if( list != null && list.Count > 0 )
            {
                var x = list[ 0 ] as XmlPointsAttach;

                if( x != null )
                    if( x.Credits >= credits )
                    {
                        return true;
                    }
            }

            return false;
        }

        public static bool TakeCredits( Mobile m, int credits )
        {
            if( m == null || m.Deleted )
                return false;

            ArrayList list = XmlAttach.FindAttachments( m, typeof( XmlPointsAttach ) );
            if( list != null && list.Count > 0 )
            {
                var x = list[ 0 ] as XmlPointsAttach;

                if( x != null )
                    if( x.Credits >= credits )
                    {
                        x.Credits -= credits;
                        return true;
                    }
            }

            return false;
        }

        public static void BroadcastMessage( AccessLevel ac, int hue, string message )
        {
            foreach( NetState state in NetState.Instances )
            {
                Mobile m = state.Mobile;

                if( m != null && m.AccessLevel >= ac )
                {
                    // check to see if they have a points attachment with ReceiveBroadcasts enabled
                    ArrayList list = XmlAttach.FindAttachments( XmlAttach.MobileAttachments, m, typeof( XmlPointsAttach ) );
                    if( list != null && list.Count > 0 )
                    {
                        var x = list[ 0 ] as XmlPointsAttach;

                        if( x != null )
                            if( !x.ReceiveBroadcasts )
                                return;
                    }

                    m.SendMessage( hue, message );
                }
            }
        }

        public static void EventSink_Speech( SpeechEventArgs args )
        {
            Mobile from = args.Mobile;
            if( from == null || !from.Player )
                return;

            if( Insensitive.Equals( args.Speech, "show my value" ) )
                ShowPointsOverhead( from );
            else if( Insensitive.Equals( args.Speech, "i wish to duel" ) )
                from.Target = new ChallengeTarget( from );
            else if( Insensitive.Equals( args.Speech, "i resign my duel" ) )
                ResignDuel( from );
        }

        private static void ResignDuel( Mobile from )
        {
            XmlPointsAttach a = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );
            if( a == null || a.Deleted )
                return;

            if( a.MCancelTimer != null && a.MCancelTimer.Running )
                a.SendText( 100208, a.CancelEnd - DateTime.Now ); // "{0} mins remaining until current challenge is cancelled."
            else
            {
                a.SendText( 100209, (int)CancelTimeout.TotalMinutes ); // "Canceling current challenge.  Please wait {0} minutes"

                if( a.Challenger != null )
                    SendText( a.Challenger, 100210, from.Name, (int)CancelTimeout.TotalMinutes ); // "{0} is canceling the current challenge. {1} minutes remain"

                // start up the cancel challenge timer
                a.DoTimer( CancelTimeout );

                // update the points gumps on the challenger if they are open
                if( from.HasGump( typeof( PointsGump ) ) )
                    a.OnIdentify( from );

                // update the points gumps on the challenge target if they are open
                if( a.Challenger != null && a.Challenger.HasGump( typeof( PointsGump ) ) )
                {
                    XmlPointsAttach ca = (XmlPointsAttach)XmlAttach.FindAttachment( a.Challenger, typeof( XmlPointsAttach ) );
                    if( ca != null && !ca.Deleted )
                        ca.OnIdentify( a.Challenger );
                }
            }
        }

        public static void ShowPointsOverhead( Mobile from )
        {
            if( from == null )
                return;

            from.PublicOverheadMessage( MessageType.Regular, 0x3B2, false, GetPoints( from ).ToString() );
        }

        public new static void Initialize()
        {
            // Register our speech handler
            EventSink.Speech += new SpeechEventHandler( EventSink_Speech );

            EventSink.Login += new LoginEventHandler( EventSink_Login );

            // CommandSystem.Register( "PointsLanguage", AccessLevel.Player, new CommandEventHandler( Language_OnCommand ) );
            // CommandSystem.Register( "Challenge", AccessLevel.Player, new CommandEventHandler( Challenge_OnCommand ) );
            // CommandSystem.Register( "LMSChallenge", AccessLevel.Player, new CommandEventHandler( LMSChallenge_OnCommand ) );
            // CommandSystem.Register( "TeamLMSChallenge", AccessLevel.Player, new CommandEventHandler( TeamLMSChallenge_OnCommand ) );
            // CommandSystem.Register( "Deathmatch", AccessLevel.Player, new CommandEventHandler( Deathmatch_OnCommand ) );
            // CommandSystem.Register( "TeamDeathmatch", AccessLevel.Player, new CommandEventHandler( TeamDeathmatch_OnCommand ) );
            // CommandSystem.Register( "DeathBall", AccessLevel.Player, new CommandEventHandler( DeathBall_OnCommand ) );
            // CommandSystem.Register( "KingOfTheHill", AccessLevel.Player, new CommandEventHandler( KingOfTheHill_OnCommand ) );
            // CommandSystem.Register( "TeamDeathBall", AccessLevel.Player, new CommandEventHandler( TeamDeathBall_OnCommand ) );
            // CommandSystem.Register( "TeamKotH", AccessLevel.Player, new CommandEventHandler( TeamKotH_OnCommand ) );
            // CommandSystem.Register( "CTFChallenge", AccessLevel.Player, new CommandEventHandler( CTFChallenge_OnCommand ) );
            CommandSystem.Register( "SystemBroadcastKills", AccessLevel.GameMaster, new CommandEventHandler( SystemBroadcastKills_OnCommand ) );
            CommandSystem.Register( "SeeKills", AccessLevel.GameMaster, new CommandEventHandler( SeeKills_OnCommand ) );
            CommandSystem.Register( "BroadcastKills", AccessLevel.GameMaster, new CommandEventHandler( BroadcastKills_OnCommand ) );
            CommandSystem.Register( "PuntiPvP", AccessLevel.GameMaster, new CommandEventHandler( CheckPoints_OnCommand ) );
            CommandSystem.Register( "ClassificaPvP", AccessLevel.GameMaster, new CommandEventHandler( TopPlayers_OnCommand ) );
            CommandSystem.Register( "AddAllPoints", AccessLevel.Developer, new CommandEventHandler( AddAllPoints_OnCommand ) );
            CommandSystem.Register( "RemoveAllPoints", AccessLevel.Developer, new CommandEventHandler( RemoveAllPoints_OnCommand ) );
            CommandSystem.Register( "LeaderboardSave", AccessLevel.Developer, new CommandEventHandler( LeaderboardSave_OnCommand ) );


            foreach( Item i in World.Items.Values )
            {
                if( i is BaseChallengeGame && !( (BaseChallengeGame)i ).GameCompleted )
                {
                    // find the region it is in
                    // is this in a challenge game region?
                    Region r = Region.Find( i.Location, i.Map );
                    if( r is ChallengeGameRegion )
                    {
                        var cgr = r as ChallengeGameRegion;

                        cgr.ChallengeGame = i as BaseChallengeGame;
                    }
                }
            }
        }

        private static void EventSink_Login( LoginEventArgs e )
        {
            // check the owner for existing challenges
            XmlPointsAttach a = (XmlPointsAttach)XmlAttach.FindAttachment( e.Mobile, typeof( XmlPointsAttach ) );
            if( a != null && a.Challenger != null )
            {
                if( a.StartingLoc != Point3D.Zero )
                    e.Mobile.MoveToWorld( a.StartingLoc, a.StartingMap );
            }
        }

        public static void WriteLeaderboardXml( string filename, int nranks )
        {
            string dirname = Path.Combine( m_LeaderboardSaveDirectory, filename );

            var sw = new StreamWriter( dirname );

            var xf = new XmlTextWriter( sw );

            xf.Formatting = Formatting.Indented;

            xf.WriteStartDocument( true );

            xf.WriteStartElement( "Leaderboard" );

            if( nranks > 0 )
                xf.WriteAttributeString( "nentries", nranks.ToString() );
            else
                xf.WriteAttributeString( "nentries", RankList.Count.ToString() );

            // go through the sorted list and display the top ranked players

            for( int i = 0; i < RankList.Count; i++ )
            {
                if( nranks > 0 && i >= nranks )
                    break;

                RankEntry r = RankList[ i ];
                XmlPointsAttach a = r.PointsAttachment;


                if( r.Killer != null && !r.Killer.Deleted && r.Rank > 0 && a != null && !a.Deleted )
                {
                    string guildname = null;

                    if( r.Killer.Guild != null )
                        guildname = r.Killer.Guild.Abbreviation;
#if(FACTIONS)
					string factionname = null;

					if(r.Killer is PlayerMobile && ((PlayerMobile)r.Killer).FactionPlayerState != null) 
						factionname = ((PlayerMobile)r.Killer).FactionPlayerState.Faction.ToString();
#endif
                    // check for any ranking change and update rank date
                    if( r.Rank != a.Rank )
                    {
                        a.WhenRanked = DateTime.Now;
                        if( a.Rank > 0 )
                            a.DeltaRank = a.Rank - r.Rank;
                        a.Rank = r.Rank;
                    }

                    TimeSpan timeranked = DateTime.Now - a.WhenRanked;

                    // write out the entry information

                    xf.WriteStartElement( "Entry" );
                    xf.WriteAttributeString( "number", i.ToString() );

                    xf.WriteStartElement( "Player" );
                    xf.WriteString( r.Killer.Name );
                    xf.WriteEndElement();

                    xf.WriteStartElement( "Guild" );
                    xf.WriteString( guildname );
                    xf.WriteEndElement();
#if(FACTIONS)
					xf.WriteStartElement( "Faction" );
					xf.WriteString( factionname );
					xf.WriteEndElement();
#endif
                    xf.WriteStartElement( "Points" );
                    xf.WriteString( a.Points.ToString() );
                    xf.WriteEndElement();

                    string kills = "???";
                    try
                    {
                        kills = a.Kills.ToString();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                    xf.WriteStartElement( "Kills" );
                    xf.WriteString( kills );
                    xf.WriteEndElement();

                    string deaths = "???";
                    try
                    {
                        deaths = a.Deaths.ToString();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                    xf.WriteStartElement( "Deaths" );
                    xf.WriteString( deaths );
                    xf.WriteEndElement();

                    xf.WriteStartElement( "Rank" );
                    xf.WriteString( a.Rank.ToString() );
                    xf.WriteEndElement();

                    xf.WriteStartElement( "Change" );
                    xf.WriteString( a.DeltaRank.ToString() );
                    xf.WriteEndElement();

                    xf.WriteStartElement( "Duration" );
                    xf.WriteString( timeranked.ToString() );
                    xf.WriteEndElement();

                    // end the entry
                    xf.WriteEndElement();
                }
            }

            xf.WriteEndElement();

            xf.Close();
        }

        public static string HtmlSpecialEncoding( string text )
        {
            if( text == null )
                return null;

            string encoded = "";

            // replace each char with the special ascii encoded equivalent
            foreach( char t in text )
            {
                encoded += String.Format( "&#{0};", (int)t );
            }

            return encoded;
        }

        public static void WriteLeaderboardHtml( string filename, int nranks )
        {
            string dirname = Path.Combine( m_LeaderboardSaveDirectory, filename );

            var sw = new StreamWriter( dirname );

            sw.WriteLine( "<TABLE border=\"1\" summary=\"This table gives leaderboard stats\"> " );
            sw.WriteLine( "<CAPTION><B>Leaderboard</B></CAPTION>" );
#if(FACTIONS)
			sw.WriteLine( "<TR><TH><TH>Player Name<TH>Guild<TH>Faction<TH>Points<TH>Kills<TH>Deaths<TH>Rank<TH>Change<TH>Time at current rank");
#else
            sw.WriteLine( "<TR><TH><TH>Player Name<TH>Guild<TH>Points<TH>Kills<TH>Deaths<TH>Rank<TH>Change<TH>Time at current rank" );
#endif
            // go through the sorted list and display the top ranked players

            for( int i = 0; i < RankList.Count; i++ )
            {
                if( nranks > 0 && i >= nranks )
                    break;

                RankEntry r = RankList[ i ];
                XmlPointsAttach a = r.PointsAttachment;

                if( r.Killer != null && !r.Killer.Deleted && r.Rank > 0 && a != null && !a.Deleted )
                {
                    string guildname = null;

                    if( r.Killer.Guild != null )
                        guildname = HtmlSpecialEncoding( r.Killer.Guild.Abbreviation );
#if(FACTIONS)
					string factionname = null;

					if(r.Killer is PlayerMobile && ((PlayerMobile)r.Killer).FactionPlayerState != null) 
						factionname = ((PlayerMobile)r.Killer).FactionPlayerState.Faction.ToString();
#endif
                    // check for any ranking change and update rank date
                    if( r.Rank != a.Rank )
                    {
                        a.WhenRanked = DateTime.Now;
                        if( a.Rank > 0 )
                            a.DeltaRank = a.Rank - r.Rank;
                        a.Rank = r.Rank;
                    }

                    TimeSpan tr = DateTime.Now - a.WhenRanked;
                    string timeranked;
                    var days = (int)tr.TotalDays;
                    var hours = (int)( tr - TimeSpan.FromDays( days ) ).TotalHours;
                    var minutes = (int)( tr - TimeSpan.FromHours( hours ) ).TotalMinutes;
                    var seconds = (int)( tr - TimeSpan.FromMinutes( minutes ) ).TotalSeconds;

                    if( days > 0 )
                    {
                        timeranked = String.Format( "{0} days {1} hrs", days, hours );
                    }
                    else if( hours > 0 )
                    {
                        timeranked = String.Format( "{0} hrs {1} mins", hours, minutes );
                    }
                    else
                    {
                        timeranked = String.Format( "{0} mins {1} secs", minutes, seconds );
                    }

                    string kills = "???";
                    try
                    {
                        kills = a.Kills.ToString();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }

                    string deaths = "???";
                    try
                    {
                        deaths = a.Deaths.ToString();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }

#if(FACTIONS)
    // write out the entry information
					sw.WriteLine( "<TR><TH><TD>{0}<TD>{1}<TD>{2}<TD>{3}<TD>{4}<TD>{5}<TD>{6}<TD>{7}<TD>{8}",
						r.Killer.Name,
						guildname,
						factionname,
						a.Points,
						kills,
						deaths,
						a.Rank,
						a.DeltaRank,
						timeranked
						);
#else
                    // write out the entry information
                    sw.WriteLine( "<TR><TH><TD>{0}<TD>{1}<TD>{2}<TD>{3}<TD>{4}<TD>{5}<TD>{6}<TD>{7}", r.Killer.Name, guildname, a.Points, kills, deaths, a.Rank, a.DeltaRank, timeranked );

#endif
                }
            }
            sw.WriteLine( "</TABLE>" );
            sw.Close();
        }

        public static void WriteLeaderboard( string filename, int nranks )
        {
            if( RankList == null )
                return;

            // force an update of the leaderboard rankings
            m_Needsupdate = true;
            RefreshRankList();

            if( !Directory.Exists( m_LeaderboardSaveDirectory ) )
                Directory.CreateDirectory( m_LeaderboardSaveDirectory );

            WriteLeaderboardXml( filename + ".xml", nranks );

            WriteLeaderboardHtml( filename + ".html", nranks );
        }

        [Usage( "LeaderboardSave [filename [minutes[nentries]]][off]" )]
        [Description( "Periodically save .xml leaderboard information to the specified file" )]
        public static void LeaderboardSave_OnCommand( CommandEventArgs e )
        {
            if( e.Arguments.Length > 0 )
            {
                if( m_LeaderboardTimer != null )
                    m_LeaderboardTimer.Stop();

                if( e.Arguments[ 0 ].ToLower() != "off" )
                {
                    m_LeaderboardFile = e.Arguments[ 0 ];

                    if( e.Arguments.Length > 1 )
                    {
                        try
                        {
                            m_LeaderboardSaveInterval = TimeSpan.FromMinutes( double.Parse( e.Arguments[ 1 ] ) );
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }
                    }

                    if( e.Arguments.Length > 2 )
                    {
                        try
                        {
                            m_LeaderboardSaveRanks = int.Parse( e.Arguments[ 2 ] );
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }
                    }


                    m_LeaderboardTimer = new LeaderboardTimer( m_LeaderboardFile, m_LeaderboardSaveInterval, m_LeaderboardSaveRanks );
                    m_LeaderboardTimer.Start();
                }
            }


            if( m_LeaderboardTimer != null && m_LeaderboardTimer.Running )
            {
                e.Mobile.SendMessage( "Leaderboard is saving to {0} every {1} minutes. Nranks = {2}", m_LeaderboardFile, m_LeaderboardSaveInterval.TotalMinutes, m_LeaderboardSaveRanks );
            }
            else
            {
                e.Mobile.SendMessage( "Leaderboard saving is off." );
            }
        }

        public static void LBSSerialize( GenericWriter writer )
        {
            // version
            writer.Write( 1 );

            // version 1
            writer.Write( m_SystemBroadcast );

            // version 0
            if( m_LeaderboardTimer != null && m_LeaderboardTimer.Running )
            {
                writer.Write( true );
            }
            else
                writer.Write( false );
            writer.Write( m_LeaderboardSaveInterval );
            writer.Write( m_LeaderboardSaveRanks );
            writer.Write( m_LeaderboardFile );
        }

        public static void LBSDeserialize( GenericReader reader )
        {
            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    m_SystemBroadcast = reader.ReadBool();
                    goto case 0;
                case 0:
                    bool running = reader.ReadBool();
                    m_LeaderboardSaveInterval = reader.ReadTimeSpan();
                    m_LeaderboardSaveRanks = reader.ReadInt();
                    m_LeaderboardFile = reader.ReadString();

                    if( running )
                    {
                        if( m_LeaderboardTimer != null )
                            m_LeaderboardTimer.Stop();
                        m_LeaderboardTimer = new LeaderboardTimer( m_LeaderboardFile, m_LeaderboardSaveInterval, m_LeaderboardSaveRanks );
                        m_LeaderboardTimer.Start();
                    }
                    break;
            }
        }

        [Usage( "PointsLanguage" )]
        [Description( "Displays or sets the language used by the points system" )]
        public static void Language_OnCommand( CommandEventArgs e )
        {
            var a = (XmlPointsAttach)XmlAttach.FindAttachment( e.Mobile, typeof( XmlPointsAttach ) );

            if( a == null || e.Mobile == null )
                return;

            if( e.Arguments.Length > 0 )
            {
                try
                {
                    a.CurrentLanguage = (LanguageType)Enum.Parse( typeof( LanguageType ), e.Arguments[ 0 ], true );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }

            e.Mobile.SendMessage( "Current language is {0}", a.CurrentLanguage );
        }

        [Usage( "PuntiPvP" )]
        [Description( "Visualizza il valore dei punti pvp e del livello." )]
        public static void CheckPoints_OnCommand( CommandEventArgs e )
        {
            string msg = null;

            ArrayList list = XmlAttach.FindAttachments( e.Mobile, typeof( XmlPointsAttach ) );
            if( list != null && list.Count > 0 )
            {
                msg = ( (XmlPointsAttach)list[ 0 ] ).OnIdentify( e.Mobile );
            }
            if( msg != null )
                e.Mobile.SendMessage( msg );
        }

        [Usage( "SeeKills [true/false]" )]
        [Description( "Determines whether a player sees others pvp broadcast results." )]
        public static void SeeKills_OnCommand( CommandEventArgs e )
        {
            ArrayList list = XmlAttach.FindAttachments( e.Mobile, typeof( XmlPointsAttach ) );
            if( list != null && list.Count > 0 )
            {
                if( e.Arguments.Length > 0 )
                {
                    try
                    {
                        ( (XmlPointsAttach)list[ 0 ] ).ReceiveBroadcasts = bool.Parse( e.Arguments[ 0 ] );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }

                e.Mobile.SendMessage( "SeeKills is {0}", ( (XmlPointsAttach)list[ 0 ] ).ReceiveBroadcasts );
            }
        }

        [Usage( "BroadcastKills [true/false]" )]
        [Description( "Determines whether pvp results will be broadcast.  The killers (winner) flag setting is used. " )]
        public static void BroadcastKills_OnCommand( CommandEventArgs e )
        {
            ArrayList list = XmlAttach.FindAttachments( e.Mobile, typeof( XmlPointsAttach ) );
            if( list != null && list.Count > 0 )
            {
                if( e.Arguments.Length > 0 )
                {
                    try
                    {
                        ( (XmlPointsAttach)list[ 0 ] ).Broadcast = bool.Parse( e.Arguments[ 0 ] );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }

                e.Mobile.SendMessage( "BroadcastKills is {0}", ( (XmlPointsAttach)list[ 0 ] ).Broadcast );
            }
        }

        [Usage( "SystemBroadcastKills [true/false]" )]
        [Description( "GM override of broadcasting of pvp results.  False means no broadcasting. True means players settings are used." )]
        public static void SystemBroadcastKills_OnCommand( CommandEventArgs e )
        {
            if( e.Arguments.Length > 0 )
            {
                try
                {
                    m_SystemBroadcast = bool.Parse( e.Arguments[ 0 ] );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }

            e.Mobile.SendMessage( "SystemBroadcastKills is {0}.", m_SystemBroadcast );
        }

        [Usage( "ClassificaPvP" )]
        [Description( "Visualizza la classifica pvp." )]
        public static void TopPlayers_OnCommand( CommandEventArgs e )
        {
            XmlPointsAttach attachment = null;
            // if this player has an XmlPoints attachment, find it
            ArrayList list = XmlAttach.FindAttachments( e.Mobile, typeof( XmlPointsAttach ) );
            if( list != null && list.Count > 0 )
            {
                attachment = (XmlPointsAttach)list[ 0 ];
            }

            e.Mobile.CloseGump( typeof( TopPlayersGump ) );
            e.Mobile.SendGump( new TopPlayersGump( attachment ) );
        }

        public static bool AreChallengers( Mobile from, Mobile target )
        {
            if( from == null || target == null )
                return false;

            #region mod by Dies Irae
            if( target is BaseCreature )
            {
                Mobile master = ( (BaseCreature)target ).GetMaster();
                if( master != null && master.Player )
                    target = master;
            }
            #endregion

            // both must be players
            if( !( from.Player && target.Player ) )
                return false;

            // check for points attachments on each
            XmlPointsAttach afrom = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );

            if( afrom == null || afrom.Deleted )
                return false;

            XmlPointsAttach atarget = (XmlPointsAttach)XmlAttach.FindAttachment( target, typeof( XmlPointsAttach ) );

            if( atarget == null || atarget.Deleted )
                return false;

            // check the individual challenger status on each
            if( afrom.Challenger == target && atarget.Challenger == from )
                return true;

            // check the team challenge status
            if( afrom.ChallengeGame != null && !afrom.ChallengeGame.Deleted && atarget.ChallengeGame == afrom.ChallengeGame )
                return afrom.ChallengeGame.AreChallengers( from, target );

            return false;
        }

        public static bool AreInAnyGame( Mobile target )
        {
            if( target == null )
                return false;

            // must be a player
            if( !target.Player )
                return false;

            // get the challenge game info from the points attachment
            var atarget = (XmlPointsAttach)XmlAttach.FindAttachment( target, typeof( XmlPointsAttach ) );

            if( atarget != null && !atarget.Deleted && atarget.ChallengeGame != null && !atarget.ChallengeGame.Deleted )
            {
                return atarget.ChallengeGame.AreInGame( target );
            }

            return false;
        }

        public static bool AreInSameGame( Mobile from, Mobile target )
        {
            if( from == null || target == null )
                return false;

            // both must be players
            if( !( from.Player && target.Player ) )
                return false;

            // check for points attachments on each
            var afrom = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );

            if( afrom == null || afrom.Deleted )
                return false;

            var atarget = (XmlPointsAttach)XmlAttach.FindAttachment( target, typeof( XmlPointsAttach ) );

            if( atarget == null || atarget.Deleted )
                return false;

            // check the team challenge status
            if( afrom.ChallengeGame != null && !afrom.ChallengeGame.Deleted && afrom.ChallengeGame == atarget.ChallengeGame )
            {
                return afrom.ChallengeGame.AreInGame( target ) && afrom.ChallengeGame.AreInGame( target );
            }

            return false;
        }

        public static bool AreTeamMembers( Mobile from, Mobile target )
        {
            if( from == null || target == null )
                return false;

            // both must be players
            if( !( from.Player && target.Player ) )
                return false;

            // check for points attachments on each
            var afrom = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );

            if( afrom == null || afrom.Deleted )
                return false;

            var atarget = (XmlPointsAttach)XmlAttach.FindAttachment( target, typeof( XmlPointsAttach ) );

            if( atarget == null || atarget.Deleted )
                return false;

            // check the team challenge status
            if( afrom.ChallengeGame != null && !afrom.ChallengeGame.Deleted && afrom.ChallengeGame == atarget.ChallengeGame )
            {
                return afrom.ChallengeGame.AreTeamMembers( from, target );
            }

            return false;
        }

        public static bool InsuranceIsFree( Mobile from, Mobile awardto )
        {
            if( from == null || awardto == null )
                return false;

            // both must be players
            if( !( from.Player && awardto.Player ) )
                return false;

            // check for points attachments on each
            var afrom = (XmlPointsAttach)XmlAttach.FindAttachment( from, typeof( XmlPointsAttach ) );

            if( afrom == null || afrom.Deleted )
                return false;

            var atarget = (XmlPointsAttach)XmlAttach.FindAttachment( awardto, typeof( XmlPointsAttach ) );

            if( atarget == null || atarget.Deleted )
                return false;

            // check the team challenge status
            if( afrom.ChallengeGame != null && !afrom.ChallengeGame.Deleted && afrom.ChallengeGame == atarget.ChallengeGame )
            {
                return afrom.ChallengeGame.InsuranceIsFree( from, awardto );
            }

            // uncomment the line below if you want to prevent insurance awards for normal 1on1 duels
            //if(atarget.Challenger == from) return true;

            return false;
        }

        public static bool YoungProtection( Mobile from, Mobile target )
        {
            // newbie protection
            if( ( ( target.SkillsTotal < 6000 && ( from.SkillsTotal - target.SkillsTotal ) > 1000 ) || ( target.RawStatTotal <= 200 && ( from.RawStatTotal - target.RawStatTotal ) > 20 ) ) )
                return true;

            // dont allow young players to be challenged by experienced players
            // this will allow young players to challenge other young players
            if( from is PlayerMobile && target is PlayerMobile )
            {
                if( ( (PlayerMobile)target ).Young && !( (PlayerMobile)from ).Young )
                    return true;
            }

            return false;
        }

        public static bool AllowChallengeGump( Mobile from, Mobile target )
        {
            if( from == null || target == null )
                return false;

            // uncomment the code below if you want to restrict challenges to towns only
            if( !from.Region.IsPartOf( typeof( TownRegion ) ) || !target.Region.IsPartOf( typeof( TownRegion ) ) )
            {
                from.SendMessage( "You must be in a town to issue a challenge" );
                return false;
            }


            if( from.Region.IsPartOf( typeof( Jail ) ) || target.Region.IsPartOf( typeof( Jail ) ) )
            {
                from.SendLocalizedMessage( 1042632 ); // You'll need a better jailbreak plan then that!
                return false;
            }

            return true;
        }

        public void DoTimer( TimeSpan delay )
        {
            if( MCancelTimer != null )
                MCancelTimer.Stop();

            MCancelTimer = new CancelTimer( this, delay );
            CancelEnd = DateTime.Now + delay;
            MCancelTimer.Start();
        }

        [Usage( "Challenge" )]
        [Description( "Challenge another player to a duel for points" )]
        public static void Challenge_OnCommand( CommandEventArgs e )
        {
            // target the player you wish to challenge
            e.Mobile.Target = new ChallengeTarget( e.Mobile );
        }

        [Usage( "LMSChallenge" )]
        [Description( "Creates a Last Man Standing challenge game" )]
        public static void LMSChallenge_OnCommand( CommandEventArgs e )
        {
            BaseChallengeGame.DoSetupChallenge( e.Mobile, 100302, typeof( LastManStandingGauntlet ) );
        }

        [Usage( "TeamLMSChallenge" )]
        [Description( "Creates a Team Last Man Standing challenge game" )]
        public static void TeamLMSChallenge_OnCommand( CommandEventArgs e )
        {
            BaseChallengeGame.DoSetupChallenge( e.Mobile, 100413, typeof( TeamLMSGauntlet ) );
        }

        [Usage( "Deathmatch" )]
        [Description( "Creates a Deathmatch challenge game" )]
        public static void Deathmatch_OnCommand( CommandEventArgs e )
        {
            BaseChallengeGame.DoSetupChallenge( e.Mobile, 100400, typeof( DeathmatchGauntlet ) );
        }

        [Usage( "TeamDeathmatch" )]
        [Description( "Creates a Team Deathmatch challenge game" )]
        public static void TeamDeathmatch_OnCommand( CommandEventArgs e )
        {
            BaseChallengeGame.DoSetupChallenge( e.Mobile, 100415, typeof( TeamDeathmatchGauntlet ) );
        }

        [Usage( "DeathBall" )]
        [Description( "Creates a DeathBall challenge game" )]
        public static void DeathBall_OnCommand( CommandEventArgs e )
        {
            BaseChallengeGame.DoSetupChallenge( e.Mobile, 100411, typeof( DeathBallGauntlet ) );
        }

        [Usage( "TeamDeathball" )]
        [Description( "Creates a Team Deathball challenge game" )]
        public static void TeamDeathBall_OnCommand( CommandEventArgs e )
        {
            BaseChallengeGame.DoSetupChallenge( e.Mobile, 100416, typeof( TeamDeathballGauntlet ) );
        }

        [Usage( "KingOfTheHill" )]
        [Description( "Creates a King of the Hill challenge game" )]
        public static void KingOfTheHill_OnCommand( CommandEventArgs e )
        {
            BaseChallengeGame.DoSetupChallenge( e.Mobile, 100410, typeof( KingOfTheHillGauntlet ) );
        }

        [Usage( "TeamKotH" )]
        [Description( "Creates a Team King of the Hill challenge game" )]
        public static void TeamKotH_OnCommand( CommandEventArgs e )
        {
            BaseChallengeGame.DoSetupChallenge( e.Mobile, 100417, typeof( TeamKotHGauntlet ) );
        }

        [Usage( "CTFChallenge" )]
        [Description( "Creates a CTF challenge game" )]
        public static void CTFChallenge_OnCommand( CommandEventArgs e )
        {
            BaseChallengeGame.DoSetupChallenge( e.Mobile, 100418, typeof( CTFGauntlet ) );
        }

        [Usage( "AddAllPoints" )]
        [Description( "Adds the XmlPoints attachment to all players" )]
        public static void AddAllPoints_OnCommand( CommandEventArgs e )
        {
            int count = 0;
            foreach( Mobile m in World.Mobiles.Values )
            {
                if( m.Player )
                {
                    // does this player already have a points attachment?
                    ArrayList list = XmlAttach.FindAttachments( m, typeof( XmlPointsAttach ) );
                    if( list == null || list.Count == 0 )
                    {
                        XmlAttachment x = new XmlPointsAttach();
                        XmlAttach.AttachTo( e.Mobile, m, x );
                        count++;
                    }
                }
            }
            e.Mobile.SendMessage( "Added XmlPoints attachments to {0} players", count );
        }

        [Usage( "RemoveAllPoints" )]
        [Description( "Removes the XmlPoints attachment from all players" )]
        public static void RemoveAllPoints_OnCommand( CommandEventArgs e )
        {
            int count = 0;
            foreach( Mobile m in World.Mobiles.Values )
            {
                if( m.Player )
                {
                    ArrayList list = XmlAttach.FindAttachments( XmlAttach.MobileAttachments, m, typeof( XmlPointsAttach ) );
                    if( list != null && list.Count > 0 )
                    {
                        foreach( XmlAttachment x in list )
                        {
                            x.Delete();
                        }
                    }
                    count++;
                }
            }
            e.Mobile.SendMessage( "Removed XmlPoints attachments from {0} players", count );
        }

        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        [Attachable]
        public XmlPointsAttach()
        {
            Points = DefaultStartingPoints;
            Broadcast = true;
            ReceiveBroadcasts = true;
            LastKill = DateTime.MinValue;
            LastDeath = DateTime.MinValue;
        }

        #region serialization
        public XmlPointsAttach( ASerial serial )
            : base( serial )
        {
            Points = DefaultStartingPoints;
            Broadcast = true;
            ReceiveBroadcasts = true;
            LastKill = DateTime.MinValue;
            LastDeath = DateTime.MinValue;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            // check for points decay
            if( Kills > 0 && m_PointsDecay > 0 && Points > DefaultStartingPoints && ( DateTime.Now - m_LastDecay ) > m_PointsDecayTime && ( DateTime.Now - LastKill ) > m_PointsDecayTime && ( DateTime.Now - LastDeath ) > m_PointsDecayTime )
            {
                Points -= m_PointsDecay;
                if( Points < DefaultStartingPoints )
                    Points = DefaultStartingPoints;
                m_LastDecay = DateTime.Now;
            }

            writer.Write( 8 );
            // version 8
            writer.Write( StartingLoc );

            if( StartingMap != null )
            {
                writer.Write( StartingMap.ToString() );
            }
            else
            {
                writer.Write( String.Empty );
            }
            // version 7
            writer.Write( m_CurrentLanguage.ToString() );
            // version 6
            writer.Write( ChallengeGame );
            writer.Write( ChallengeSetup );
            // version 5
            writer.Write( CancelEnd - DateTime.Now );
            // version 4
            writer.Write( ReceiveBroadcasts );
            // version 3
            writer.Write( Rank );
            writer.Write( DeltaRank );
            writer.Write( WhenRanked );
            writer.Write( m_LastDecay );
            // version 2
            writer.Write( Credits );
            // version 1
            writer.Write( Broadcast );
            // version 0
            writer.Write( Points );
            writer.Write( Kills );
            writer.Write( Deaths );
            writer.Write( Challenger );
            writer.Write( LastKill );
            writer.Write( LastDeath );
            // write out the kill list
            if( m_KillList != null )
            {
                writer.Write( m_KillList.Count );
                foreach( KillEntry k in m_KillList )
                {
                    writer.Write( k.Killed );
                    writer.Write( k.WhenKilled );
                }
            }
            else
            {
                writer.Write( 0 );
            }

            // need this in order to rebuild the rankings on deser
            if( AttachedTo is Mobile )
                writer.Write( AttachedTo as Mobile );
            else
                writer.Write( (Mobile)null );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 8:
                    {
                        StartingLoc = reader.ReadPoint3D();
                        string map = reader.ReadString();
                        try
                        {
                            StartingMap = string.IsNullOrEmpty( map ) ? Map.Felucca : Map.Parse( map );
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( "Error while parsing map in XmlPoints.Deserialize." );
                            Console.WriteLine( ex.ToString() );
                        }

                        if( StartingLoc == Point3D.Zero )
                        {
                            Console.WriteLine( "Warning: invalid starting location during xmlPointsattachment deserialize." );
                            StartingLoc = CoveLocation;
                        }
                        goto case 7;
                    }
                case 7:
                    {
                        string langstr = reader.ReadString();
                        try
                        {
                            m_CurrentLanguage = (LanguageType)Enum.Parse( typeof( LanguageType ), langstr );
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( "Error while parsing language in XmlPoints.Deserialize." );
                            Console.WriteLine( ex.ToString() );
                        }
                        goto case 6;
                    }
                case 6:
                    {
                        ChallengeGame = (BaseChallengeGame)reader.ReadItem();
                        ChallengeSetup = (BaseChallengeGame)reader.ReadItem();
                        goto case 5;
                    }
                case 5:
                    {
                        TimeSpan remaining = reader.ReadTimeSpan();
                        if( remaining > TimeSpan.Zero )
                        {
                            DoTimer( remaining );
                        }
                        goto case 4;
                    }
                case 4:
                    {
                        ReceiveBroadcasts = reader.ReadBool();
                        goto case 3;
                    }
                case 3:
                    {
                        Rank = reader.ReadInt();
                        DeltaRank = reader.ReadInt();
                        WhenRanked = reader.ReadDateTime();
                        m_LastDecay = reader.ReadDateTime();
                        goto case 2;
                    }
                case 2:
                    {
                        Credits = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        Broadcast = reader.ReadBool();
                        goto case 0;
                    }
                case 0:
                    {
                        Points = reader.ReadInt();
                        Kills = reader.ReadInt();
                        Deaths = reader.ReadInt();
                        Challenger = reader.ReadMobile();
                        LastKill = reader.ReadDateTime();
                        LastDeath = reader.ReadDateTime();

                        // read in the kill list
                        int count = reader.ReadInt();
                        m_KillList = new List<KillEntry>();
                        for( int i = 0; i < count; i++ )
                        {
                            Mobile m = reader.ReadMobile();
                            DateTime t = reader.ReadDateTime();
                            if( m != null && !m.Deleted )
                            {
                                m_KillList.Add( new KillEntry( m, t ) );
                            }
                        }

                        // get the owner of this in order to rebuild the rankings
                        Mobile killer = reader.ReadMobile();

                        // rebuild the ranking list
                        // if they have never made a kill, then dont rank
                        if( killer != null && Kills > 0 )
                        {
                            UpdateRanking( killer, this );
                        }
                        break;
                    }
            }
        }
        #endregion

        // updates the attachment kill list and removes expired entries
        private void RefreshKillList()
        {
            if( m_KillList != null )
            {
                var deletelist = new ArrayList();

                foreach( KillEntry k in m_KillList )
                {
                    if( k.WhenKilled + m_KillDelay <= DateTime.Now )
                    {
                        // expired so remove it from the list
                        deletelist.Add( k );
                    }
                }

                // clear out any expired entries
                if( deletelist.Count > 0 )
                {
                    foreach( KillEntry k in deletelist )
                    {
                        m_KillList.Remove( k );
                    }
                }
            }
        }

        public override bool HandlesOnKill
        {
            get { return true; }
        }

        // handles point gain when the player kills someone
        public override void OnKill( Mobile killed, Mobile killer )
        {
            bool isInDuel = Challenger != null;

            if( killer == null || killed == null || !( killed.Player ) || killer == killed )
                return;

            bool awardpoints = true;

            // if this was a team or challenge duel then clear agressor list
            if( killed == Challenger || killer == Challenger || AreInSameGame( killed, killer ) )
            {
                // and remove the challenger from the aggressor list so that the res noto is not affected
                ClearAggression( killed, killer );

                #region mod by Dies Irae

                try
                {
                    killer.Poison = null;
                    killer.Paralyzed = false;

                    EvilOmenSpell.CheckEffect( killer );
                    StrangleSpell.RemoveCurse( killer );
                    CorpseSkinSpell.RemoveCurse( killer );
                    CurseSpell.RemoveEffect( killer );
                    MortalStrike.EndWound( killer );
                    BloodOathSpell.RemoveCurse( killer );
                    MindRotSpell.ClearMindRotScalar( killer );

                    //DarkOmenSpell.CheckEffect( killer );
                    ChokingSpell.RemoveCurse( killer );
                    BloodConjunctionSpell.RemoveCurse( killer );
                    LobotomySpell.ClearMindRotScalar( killer );

                    BuffInfo.RemoveBuff( killer, BuffIcon.Clumsy );
                    BuffInfo.RemoveBuff( killer, BuffIcon.FeebleMind );
                    BuffInfo.RemoveBuff( killer, BuffIcon.Weaken );
                    BuffInfo.RemoveBuff( killer, BuffIcon.MassCurse );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }

                #endregion
            }

            // handle challenge team kills
            if( ChallengeGame != null && !ChallengeGame.Deleted )
            {
                ChallengeGame.OnKillPlayer( killer, killed );
            }

            // check to see whether points can be given
            if( !( AttachedTo is Mobile ) || !CanAffectPoints( (Mobile)AttachedTo, killer, killed, false ) )
            {
                awardpoints = false;
            }

            // if this was a challenge duel then clear the challenger field
            if( killed == Challenger || killer == Challenger || isInDuel ) // mod by Dies Irae
            {
                Challenger = null;

                killer.SendMessage( "Your duel ended." );

                if( killer.PlayerDebug )
                    killer.SendMessage( "Debug: OnKill -> Challenger = null" );
            }

            // begin the section to award points

            if( !awardpoints )
                return;

            if( LogKills )
            {
                try
                {
                    using( var op = new StreamWriter( "Logs/kills.log", true ) )
                    {
                        op.WriteLine( "{0}: {1} killed {2}", DateTime.Now, killer, killed );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }

            int killedpoints = 0;
            // give the killer his points, either a fixed amount or scaled by the difference with the points of the killed
            // if the killed has more points than the killed then gain more
            ArrayList list = XmlAttach.FindAttachments( killed, typeof( XmlPointsAttach ) );

            if( list != null && list.Count > 0 )
            {
                killedpoints = ( (XmlPointsAttach)list[ 0 ] ).Points;
            }

            var val = (int)( ( killedpoints - Points ) * m_WinScale );
            if( val <= 0 )
                val = 1;

            Points += val;

            var cval = (int)( ( killedpoints - Points ) * m_CreditScale );
            if( cval <= 0 )
                cval = 1;

            Credits += cval;

            LastKill = DateTime.Now;

            // killer.SendMessage(String.Format(Text(100215), val, killed.Name));  // "You receive {0} points for killing {1}"

            if( GainHonorFromDuel )
            {
                bool gainedPath = false;
                if( VirtueHelper.Award( killer, VirtueName.Honor, val, ref gainedPath ) )
                {
                    if( gainedPath )
                    {
                        killer.SendLocalizedMessage( 1063226 ); // You have gained a path in Honor!
                    }
                    else
                    {
                        killer.SendLocalizedMessage( 1063225 ); // You have gained in Honor.
                    }
                }
            }


            // add to the recently killed list
            //m_KillList.Add(new KillEntry(killed, DateTime.Now));

            // add to the cumulative death count
            Kills++;

            // update the overall ranking list
            UpdateRanking( killer, this );

            // if broadcast is enabled then announce it
            if( Broadcast && m_SystemBroadcast )
            {
                BroadcastMessage( AccessLevel.Player, 0x482, String.Format( SystemText( 100216 ), killer.Name, killed.Name ) );
                // "{0} has defeated {1} in combat."
            }

            // update the points gump if it is open
            if( killer.HasGump( typeof( PointsGump ) ) )
            {
                // redisplay it with the new info
                OnIdentify( killer );
            }

            // update the top players gump if it is open
            if( killer.HasGump( typeof( TopPlayersGump ) ) )
            {
                killer.CloseGump( typeof( TopPlayersGump ) );
                killer.SendGump( new TopPlayersGump( this ) );
            }
        }

        public void ReportPointLossCallback( object state )
        {
            var args = (object[])state;
            var points = (int)args[ 0 ];
            var name = (string)args[ 1 ];
            var m = (Mobile)args[ 2 ];

            if( m != null )
            {
                SendText( m, 100217, points, name ); // "You lost {0} point(s) for being killed by {1}"
            }
        }

        public static void AutoResCallback( object state )
        {
            object[] args = (object[])state;

            Mobile m = (Mobile)args[ 0 ];
            bool refresh = (bool)args[ 1 ];

            if( m != null )
            {
				//reset combatant
				m.Combatant = null;
		
				if (m.LastKiller != null)
					(m.LastKiller).Combatant = null;

                // auto tele ghosts to the corpse
                m.PlaySound( 0x214 );
                m.FixedEffect( 0x376A, 10, 16 );
                m.Resurrect();
                if( m.Corpse != null )
                {
                    m.MoveToWorld( m.Corpse.Location, m.Corpse.Map );

                    Corpse c = m.Corpse as Corpse;
                    if( c != null )
                        c.Open( m, true, true );

                    m.Corpse.LootType = LootType.Regular;
                }

                if( refresh )
                {
                    m.Hits = m.HitsMax;
                    m.Mana = int.MaxValue;
                    m.Stam = int.MaxValue;
                }
            }
        }

        public static void ReturnCallback( object state )
        {
            var args = (object[])state;

            var killer = (Mobile)args[ 0 ];
            var killed = (Mobile)args[ 1 ];
            var loc = (Point3D)args[ 2 ];
            var map = (Map)args[ 3 ];


            if( killer != null && killed != null && map != null && map != Map.Internal )
            {
                // auto tele players and corpses
                // if there were nearby pets/mounts then tele those as well

                var petlist = new ArrayList();
                foreach( Mobile m in killer.GetMobilesInRange( 16 ) )
                {
                    if( m is BaseCreature && ( (BaseCreature)m ).ControlMaster == killer )
                    {
                        petlist.Add( m );
                    }
                }
                foreach( Mobile m in killed.GetMobilesInRange( 16 ) )
                {
                    if( m is BaseCreature && ( (BaseCreature)m ).ControlMaster == killed )
                    {
                        petlist.Add( m );
                    }
                }

                // port the pets
                foreach( Mobile m in petlist )
                {
                    m.MoveToWorld( loc, map );
                }

                // port the killer and corpse
                killer.PlaySound( 0x214 );
                killer.FixedEffect( 0x376A, 10, 16 );
                killer.MoveToWorld( loc, map );
                if( killer.Corpse != null )
                {
                    killer.Corpse.MoveToWorld( loc, map );
                }

                // port the killed and corpse
                killed.PlaySound( 0x214 );
                killed.FixedEffect( 0x376A, 10, 16 );
                killed.MoveToWorld( loc, map );
                if( killed.Corpse != null )
                {
                    killed.Corpse.MoveToWorld( loc, map );
                }
            }
        }

        public bool CanAffectPoints( Mobile from, Mobile killer, Mobile killed, bool assumechallenge )
        {
            // uncomment this for newbie protection
            //if( ((killed.SkillsTotal < 6000 && (killer.SkillsTotal - killed.SkillsTotal ) > 1000) ||
            //(killed.RawStatTotal <= 200 && (killer.RawStatTotal - killed.RawStatTotal) > 20 ) ) && m_Challenger != killer && m_Challenger != killed) return false;

            // check for within guild kills and ignore them if this has been disabled
            if( !AllowWithinGuildPoints && SameGuild( killed, killer ) )
                return false;

            // check for within team kills and ignore them
            if( AreTeamMembers( killer, killed ) )
                return false;

            // are the players challengers?
            bool inchallenge = false;
            if( ( from == killer && Challenger == killed ) || ( from == killed && Challenger == killer ) )
            {
                inchallenge = true;
            }

            bool norestriction = UnrestrictedChallenges;

            // check for team challenges
            if( ChallengeGame != null && !ChallengeGame.Deleted )
            {
                // check to see if points have been disabled in this game
                if( !ChallengeGame.AllowPoints )
                    return false;

                inchallenge = true;

                // check for kill delay limitations on points awards
                norestriction = !ChallengeGame.UseKillDelay;
            }

            // if UnlimitedChallenges has been set then allow points
            // otherwise, challenges have to obey the same restrictions on minimum time between kills as normal pvp
            if( norestriction && ( inchallenge || assumechallenge ) )
                return true;

            // only allow guild kills to yield points if in a challenge
            if( !( assumechallenge || inchallenge ) && SameGuild( killed, killer ) )
                return false;

            // uncomment the line below to limit points to challenges. regular pvp will not give points
            //if(!inchallenge && !assumechallenge) return false;

            // check to see whether killing the target would yield points
            // get a point for killing if they havent been killed recent

            // get the points attachment on the killer if this isnt the killer
            XmlPointsAttach a = this;
            if( from != killer )
            {
                a = (XmlPointsAttach)XmlAttach.FindAttachment( killer, typeof( XmlPointsAttach ) );
            }
            if( a != null )
            {
                a.RefreshKillList();

                // check the kill list if there is one
                if( a.m_KillList != null )
                {
                    foreach( KillEntry k in a.m_KillList )
                    {
                        if( k.WhenKilled + m_KillDelay > DateTime.Now )
                        {
                            // found a match on the list so dont give any points
                            if( k.Killed == killed )
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            // check to see whether the killed target could yield points
            if( from == killed )
            {
                // is it still within the minimum delay for being killed?
                if( DateTime.Now < LastDeath + m_DeathDelay )
                    return false;
            }

            return true;
        }

        public void ClearAggression( Mobile source, Mobile target )
        {
            // and remove the challenger from the aggressor list so that the res noto is not affected
            List<AggressorInfo> klist = target.Aggressors;
            if( klist != null && klist.Count > 0 )
            {
                for( int j = 0; j < klist.Count; j++ )
                {
                    if( ( klist[ j ] ).Attacker == source || ( klist[ j ] ).Defender == source )
                    {
                        klist.Remove( klist[ j ] );
                        break;
                    }
                }
            }

            klist = target.Aggressed;
            if( klist != null && klist.Count > 0 )
            {
                for( int j = 0; j < klist.Count; j++ )
                {
                    if( ( klist[ j ] ).Attacker == source || ( klist[ j ] ).Defender == source )
                    {
                        klist.Remove( klist[ j ] );
                        break;
                    }
                }
            }
        }

        public override bool HandlesOnKilled
        {
            get { return true; }
        }

        // handles point loss when the player is killed
        public override void OnKilled( Mobile killed, Mobile killer )
        {
            bool isInDuel = Challenger != null; // mod by Dies Irae;

            if( killer == null || killed == null || !( killer.Player ) || killer == killed )
                return;

            bool awardpoints = true;

            // if this was a challenge duel then clear agression
            if( killed == Challenger || killer == Challenger || AreInSameGame( killer, killed ) )
            {
                ClearAggression( killer, killed );

                // and remove the challenger from the corpse aggressor list so that the corpse noto is not affected
                if( killed.Corpse is Corpse )
                {
                    List<Mobile> klist = ( (Corpse)killed.Corpse ).Aggressors;
                    if( klist != null && klist.Count > 0 )
                    {
                        for( int j = 0; j < klist.Count; j++ )
                        {
                            if( klist[ j ] == killer )
                            {
                                klist.Remove( killer );
                                break;
                            }
                        }
                    }
                }
            }

            // check to see whether points can be taken
            if( !( AttachedTo is Mobile ) || !CanAffectPoints( (Mobile)AttachedTo, killer, killed, false ) )
            {
                awardpoints = false;
            }

            // handle challenge team kills
            if( ChallengeGame != null && !ChallengeGame.Deleted && ChallengeGame.GetParticipant( killer ) != null && ChallengeGame.GetParticipant( killed ) != null )
            {
                ChallengeGame.OnPlayerKilled( killer, killed );
            }

            if( killed == Challenger || killer == Challenger || isInDuel )
            {
                EndDuel( killer, killed );
            }

            // begin the section to award points

            if( !awardpoints )
                return;

            int killerpoints = 0;

            // take points from the killed, either a fixed amount or scaled by the difference with the points of the killer
            // if the killer has fewer points than the killed then lose more
            XmlPointsAttach xp = XmlAttach.FindAttachment( killer, typeof( XmlPointsAttach ) ) as XmlPointsAttach;

            if( xp != null )
            {
                killerpoints = xp.Points;


                // add to the recently killed list
                xp.m_KillList.Add( new KillEntry( killed, DateTime.Now ) );
            }

            int val = (int)( ( Points - killerpoints ) * m_LoseScale );
            if( val <= 0 )
                val = 1;

            int startpoints = Points;

            Points -= val;

            // comment out this code if you dont want to have a zero floor and want to allow negative points
            if( Points < 0 )
                Points = 0;

            // add to the cumulative death count
            Deaths++;

            if( startpoints - Points > 0 )
            {
                // prepare the message to report the point loss.  Need the delay otherwise it wont show up due to the death sequence
                Timer.DelayCall( TimeSpan.FromSeconds( 5 ), new TimerStateCallback( ReportPointLossCallback ), new object[] { startpoints - Points, killer.Name, killed } );

                // update the overall ranking list
                UpdateRanking( killed, this );
            }

            LastDeath = DateTime.Now;
        }

        public virtual void EndDuel( Mobile killer, Mobile killed )
        {
		bool KilledInDuel = killer == Challenger;
            Challenger = null;
            killed.SendMessage( killed.Language == "ITA" ? "Il tuo duello è finito." : "Your duel ended." );

            if( killed.PlayerDebug )
                killed.SendMessage( "Debug: OnKilled -> Challenger = null" );

            BaseChallengeGame.ClearAggressors( killed );
            BaseChallengeGame.ClearAggressors( killer );

            BaseChallengeGame.ClearAggressed( killed );
            BaseChallengeGame.ClearAggressed( killer );

            if( killed.Criminal )
                killed.Criminal = false;

            if( killer.Criminal )
                killer.Criminal = false;

            if( AutoResAfterDuel && KilledInDuel )
            {
                // immediately bless the corpse to prevent looting
                if( killed.Corpse != null )
                    killed.Corpse.LootType = LootType.Blessed;

                // prepare the autores callback
                Timer.DelayCall( TimeSpan.FromSeconds( 2 ), new TimerStateCallback( AutoResCallback ), new object[] { killed, false } );
            }

            if( TeleportOnDuel )
            {
                // teleport back to original location
                if( StartingLoc != Point3D.Zero )
                    Timer.DelayCall( TimeSpan.FromSeconds( 30 ), new TimerStateCallback( ReturnCallback ), new object[] { killer, killed, StartingLoc, StartingMap } );
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            // only allow attachment to players
            if( !( AttachedTo is Mobile && ( (Mobile)AttachedTo ).Player ) )
                Delete();
        }

        public override string OnIdentify( Mobile from )
        {
            // uncomment this if you dont want players being able to check points/rank on other players
            //if((from != null) && (AttachedTo != from) && (from.AccessLevel == AccessLevel.Player)) return null;

            if( !( AttachedTo is Mobile ) )
                return null;

            int val = GetRanking( (Mobile)AttachedTo );

            var msg = new StringBuilder();

            if( val > 0 )
            {
                msg.AppendFormat( GetText( from, 100218 ), Points ); // "Current Points = {0}"
                msg.AppendFormat( "\n" );
                msg.AppendFormat( GetText( from, 100219 ), val ); // "Rank = {0}"
            }
            else
            {
                msg.AppendFormat( GetText( from, 100218 ), Points ); // "Current Points = {0}"
                msg.AppendFormat( "\n" );
                msg.AppendFormat( GetText( from, 100220 ) ); // "No ranking."
            }

            // report the number of Credits available if the player is checking.  Dont display this if others are checking (unless they are staff).
            if( ( from != null ) && ( ( AttachedTo == from ) || ( from.AccessLevel > AccessLevel.Player ) ) )
            {
                msg.AppendFormat( "\n" );
                msg.AppendFormat( GetText( from, 100221 ), Credits ); // "Available Credits = {0}"
            }

            msg.AppendFormat( "\n" );
            msg.AppendFormat( GetText( from, 100222 ), Kills, Deaths );
            // "Total Kills = {0}\nTotal Deaths = {1}\nRecent Kill List"

            RefreshKillList();

            if( m_KillList != null && m_KillList.Count > 0 )
            {
                foreach( KillEntry k in m_KillList )
                {
                    if( k.Killed != null && !k.Killed.Deleted )
                    {
                        msg.AppendFormat( "\n" );
                        msg.AppendFormat( GetText( from, 100223 ), k.Killed.Name, k.WhenKilled ); // "{0} killed at {1}"
                    }
                }
            }

            // display the points info gump
            if( from != null )
            {
                from.CloseGump( typeof( PointsGump ) );
                from.SendGump( new PointsGump( this, from, (Mobile)AttachedTo, msg.ToString() ) );
            }

            return null;
        }

        public string m_GuildFilter;
        public string m_NameFilter;
    }
}