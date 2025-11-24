using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Spells;
using Server.Mobiles;

namespace Midgard.Engines.TalkingVendors
{
    public class Core
    {
        internal static Dictionary<string, Dictionary<string, string>> SpeechTable { get; private set; }

        private static List<string> m_TalkToFriendsList;
        private static List<string> m_TalkAboutFriendsList;
        private static Dictionary<string, string> m_DefaultSpeechTable;
        private static Dictionary<string, string> m_KarmaGreetingsWorst;
        private static Dictionary<string, string> m_KarmaGreetingsNormal;
        private static Dictionary<string, string> m_KarmaGreetingsGood;
        private static Dictionary<string, string> m_KarmaGreetingsAwe;

        static Core()
        {
            TypeList = new Dictionary<Type, string>();
        }

        public static void ConfigSystem()
        {
            if( !Config.Enabled )
                return;

            var pkg = Packager.Core.Singleton[ typeof( Config ) ];

            pkg.LogInfo( "Configuring Speech system..." );

            if( !Directory.Exists( Config.SpeechPath ) )
            {
                Console.WriteLine();
                pkg.LogWarningLine( "Speech system is enabled but {0} has not been found.", Config.SpeechPath );
                return;
            }

            m_TalkToFriendsList = ReadListOfStrings( Config.TalkToFriendsFileName );
            m_TalkAboutFriendsList = ReadListOfStrings( Config.TalkingAboutFriendsFileName );
            m_KarmaGreetingsWorst = ReadKeyValueDictionary( Config.KarmaGreetingsWorstFileName );
            m_KarmaGreetingsNormal = ReadKeyValueDictionary( Config.KarmaGreetingsNormalFileName );
            m_KarmaGreetingsGood = ReadKeyValueDictionary( Config.KarmaGreetingsGoodFileName );
            m_KarmaGreetingsAwe = ReadKeyValueDictionary( Config.KarmaGreetingsAweFileName );
            m_DefaultSpeechTable = ReadKeyValueDictionary( Config.DefaultSpeechTableFileName );

            SpeechTable = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> dict;

            foreach( string info in Directory.GetFiles( Config.SpeechPath ) )
            {
                FileInfo finfo = new FileInfo( info );

                if( finfo.Name == Config.TalkToFriendsFileName ||
                    finfo.Name == Config.TalkingAboutFriendsFileName ||
                    finfo.Name == Config.DefaultSpeechTableFileName ||
                    finfo.Name == Config.KarmaGreetingsWorstFileName ||
                    finfo.Name == Config.KarmaGreetingsNormalFileName ||
                    finfo.Name == Config.KarmaGreetingsGoodFileName ||
                    finfo.Name == Config.KarmaGreetingsAweFileName )
                    continue;

                string type = finfo.Name.Replace( ".cfg", "" );
                Type t = ScriptCompiler.FindTypeByName( type, true );
                if( t != null )
                {
                    TypeList.Add( t, finfo.Name );
                    Debug( "Adding {0} for config file {1}", finfo.Name, t.Name );
                }
                else
                {
                    Console.WriteLine();
                    pkg.LogWarningLine( "Type not found for fileinfo {0}.", finfo.Name );
                    pkg.LogInfo( "Configuring Speech system..." );
                }

                dict = ReadKeyValueDictionary( info );

                Debug( "Adding {0} to SpeechTable with {1} entries", finfo.Name, dict.Count );
                SpeechTable.Add( finfo.Name, dict );
            }

            pkg.LogInfoLine( "done." );
        }

        internal static Dictionary<Type, string> TypeList { get; private set; }

        public static string GetSpeechForType( Type t )
        {
            string value = null;

            if( TypeList != null )
            {
                if( !TypeList.TryGetValue( t, out value ) )
                    Console.WriteLine( "Warning: Speech group for class {0} is required but not found.", t.Name );
            }
            else
                Console.WriteLine( "Warning: m_TypeList is null" );

            return value;
        }

        public static bool SupportSpeech( Mobile m )
        {
            return TypeList != null && TypeList.ContainsKey( m.GetType() );
        }

        /// <summary>
        /// This method return a dictionary of strings for a given dictionaryName.
        /// </summary>
        public static Dictionary<string, string> GetSpeech( string dictionaryName )
        {
            Dictionary<string, string> speech = null;

            if( SpeechTable != null && !SpeechTable.TryGetValue( dictionaryName, out speech ) )
            {
                Console.WriteLine( "Warning: Speech group {0} is required but not found.", dictionaryName );
                return null;
            }

            return speech;
        }

        public static bool HandlesOnSpeech( ITalkingMobile talkingMobile, Mobile from )
        {
            bool handle = ( Config.Enabled && from != null && talkingMobile != null &&
                from.Player && talkingMobile is Mobile && ( (Mobile)talkingMobile ).InRange( from, Config.RangePerception ) );

            if( from != null )
                Debug( "HandlesOnSpeech: {0} - {1}", from.Name, handle );

            return handle;
        }

        public static bool OnSpeech( BaseCreature creature, SpeechEventArgs e, bool checkFocus, bool checkNearby )
        {
            if( !Config.Enabled )
                return false;

            Debug( "OnSpeech 1" );
            if( Insensitive.Equals( e.Speech, "status" ) )
            {
                ExplainStatus( creature );
                e.Handled = true;
                return true;
            }

            Mobile speaker = e.Mobile;
            if( speaker == null || ( speaker.Hidden && speaker.AccessLevel > AccessLevel.Player ) )
            {
                DebugSay( creature, speaker, "My friend has gone or is hidden or is a GM." );
                return false;
            }

            if( e.Keywords != null && e.Keywords.Length > 0 )
            {
                Debug( "TalkinVendors OnSpeech returned false due to keywords in the speech argument." );
                return false;
            }

            Debug( "OnSpeech 2" );

            if( speaker.Player && speaker.Alive && creature.SupportSpeech && creature.IsSpeechEnabled )
            {
                Debug( "OnSpeech 3" );

                BaseAI ai = creature.AIObject;
                if( ai == null )
                    return false;

                if( !creature.InRange( speaker, Config.RangePerception ) )
                    return false;

                Debug( "OnSpeech 4" );

                // we don't handle any speech if someone else is talking to our mobile
                if( checkNearby && ai.SomeoneElseIsTalkingTo( speaker ) )
                    return false;

                if( !checkFocus || ai.SetFocunOn( e, false, true ) )
                {
                    bool check = CheckSpeech( creature, e );
                    Debug( "CheckSpeech() returned: " + check );
                    return check;
                }
            }

            return false;
        }

        /// <summary>
        /// General behavior of our ITalkingMobile to speech events
        /// </summary>
        private static bool CheckSpeech( BaseCreature creature, SpeechEventArgs e )
        {
            BaseAI ai = creature.AIObject;
            if( ai == null )
                return false;

            Mobile speaker = e.Mobile;
            string text = e.Speech;

            Debug( "CheckSpeech 1" );

            // if a keyword is found react
            if( BarkToKeyWord( creature, speaker, text, false ) )
            {
                Debug( "BarkToKeyWord( talkingMobile, speaker, text, false )" );
                return true;
            }

            // else if greet to speaker
            if( ai.WasNamed( e.Speech ) || e.HasKeyword( 0x003B ) ) // *greetings* *hail* *hello* *hey* *yo*
            {
                // TODO check annoying

                if( DefaultHello( creature, speaker ) )
                {
                    Debug( "DefaultHello( talkingMobile, speaker )" );
                    return true;
                }
            }

            // if our ITalkingMobile would not like to talk end conversation
            //if( Utility.RandomDouble() < FrequencyOfSpeech )
            //    return false;

            // if our ITalkingMobile is shy end conversation
            if( !creature.CanHaveFriends )
            {
                Debug( "!talkingMobile.CanHaveFriends" );
                return false;
            }

            if( creature.TalkingFriends == null )
                creature.TalkingFriends = new Dictionary<Mobile, int>();

            // if speaker is a friend of talkingMobile...
            if( creature.TalkingFriends.ContainsKey( speaker ) )
            {
                if( Utility.Random( 3 ) == 1 )
                    GreetFriend( creature, speaker );
                else
                    ChatAboutFriends( creature, speaker );

                Debug( "talkingMobile.TalkingFriends.ContainsKey( speaker )" );
                return true;
            }

            int loy = GetFriendLoyalty( speaker, creature );
            creature.Say( String.Format( "My loyalty to {0} is {1}", speaker.Name, loy ) );

            if( Utility.Random( 20 ) <= loy + 10 )
            {
                ChatAboutFriends( creature, speaker );

                Debug( "ChatAboutFriends( talkingMobile, speaker )" );
                return true;
            }

            if( DateTime.Now > creature.NextFriend )
                AddFriend( speaker, creature ); // it's time to make a new friend
            else
                ChatAboutFriends( creature, speaker );

            Debug( "end of CheckSpeech -> return true" );
            return true;
        }

        private static bool GetResponse( IDictionary<string, string> dict, string key, out string response )
        {
            if( dict != null )
            {
                if( dict.TryGetValue( key, out response ) )
                {
                    Debug( response );
                    return true;
                }
            }

            response = "";
            return false;
        }

        /// <summary>
        /// Reaction of our ITalkingMobile to a keyword
        /// </summary>
        private static bool BarkToKeyWord( ITalkingMobile talkingMobile, Mobile speaker, string text, bool ignoreKarmaLevel )
        {
            Debug( "BarkToKeyWord 1 " + text );

            if( talkingMobile == null )
                return false;

            string[] words = text.Split( ' ' );
            string response = string.Empty;
            bool foundResponse = false;

            foreach( string word in words )
            {
                string tmp = word.ToLower();

                if( GetResponse( GetDictionaryForKarmaLevel( GetKarmaForSpeaker( speaker ) ), tmp, out response ) )
                    foundResponse = true;
                else if( GetResponse( talkingMobile.Speech, tmp, out response ) )
                    foundResponse = true;
                else if( GetResponse( m_DefaultSpeechTable, tmp, out response ) )
                    foundResponse = true;

                if( foundResponse )
                    break;
            }

            if( foundResponse )
                Speak( talkingMobile, speaker, response, true );

            return foundResponse;
        }

        /// <summary>
        /// Default Reaction of our ITalkingMobile to ANY speech: A greeting.
        /// </summary>
        public static bool DefaultHello( ITalkingMobile talkingMobile, Mobile speaker )
        {
            Debug( "DefaultHello" );

            if( DateTime.Now > talkingMobile.LastGreet + Config.GreetingsDelay )
            {
                BarkToKeyWord( talkingMobile, speaker, "hi", false );
                talkingMobile.LastGreet = DateTime.Now;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Our ITalkingMobile wants to talk to speaker
        /// </summary>
        private static void GreetFriend( ITalkingMobile talkingMobile, Mobile speaker )
        {
            Debug( "GreetFriend" );

            string greetName = speaker.Name;

            int loy = GetFriendLoyalty( speaker, talkingMobile );
            if( loy >= 3 )
                greetName = string.Format( "My {0} {1}", GetFriendTitle( loy ), speaker.Name );

            talkingMobile.LastGreet = DateTime.Now;

            if( m_TalkToFriendsList != null && m_TalkToFriendsList.Count > 0 )
            {
                string response = m_TalkToFriendsList[ Utility.Random( m_TalkToFriendsList.Count ) ];
                response = response.Replace( "friendname", greetName );

                Speak( talkingMobile, speaker, response, true );

                if( Utility.Random( 8 ) == 1 )
                    IncreaseLoyalty( speaker, talkingMobile );
            }
        }

        /// <summary>
        /// Our ITalkingMobile wants to talk to speaker about its friends
        /// </summary>
        private static void ChatAboutFriends( ITalkingMobile talkingMobile, Mobile speaker )
        {
            Debug( "ChatAboutFriends" );

            List<Mobile> friends = new List<Mobile>();
            foreach( Mobile m in talkingMobile.TalkingFriends.Keys )
            {
                if( m != speaker )
                    friends.Add( m );
            }

            // our speaker is the only friend
            if( friends.Count < 1 )
                return;

            // pick a random friend to chat on
            Mobile friend = friends[ Utility.Random( friends.Count ) ];

            int loy = GetFriendLoyalty( friend, talkingMobile );

            // Our friend is a new one or our talker does not want to chat
            if( loy == 0 || Utility.Random( 3 ) == 1 )
            {
                BarkToKeyWord( talkingMobile, speaker, "default", false );
                return;
            }

            if( m_TalkAboutFriendsList != null && m_TalkAboutFriendsList.Count > 0 )
            {
                string chatName = friend.Name;
                if( loy >= 3 )
                    chatName = string.Format( "My {0} {1}", GetFriendTitle( loy ), friend.Name );

                string response = m_TalkAboutFriendsList[ Utility.Random( m_TalkAboutFriendsList.Count ) ];
                response = response.Replace( "charname", speaker.Name );
                response = response.Replace( "friendname", chatName );

                Speak( talkingMobile, speaker, response, true );
            }
        }

        /*
                /// <summary>
                /// A simple greet to ITalkingMobile loyal friend
                /// </summary>
                private static void YellToFriend( ITalkingMobile talkingMobile, Mobile speaker )
                {
                    if( talkingMobile.TalkingFriends != null && talkingMobile.TalkingFriends.ContainsKey( speaker ) )
                    {
                        int loy = GetFriendLoyalty( speaker, talkingMobile );

                        string yellName = string.Format( "{0}!", speaker.Name );
                        if( loy >= 3 )
                            yellName = string.Format( "My {0} {1}", GetFriendTitle( loy ), yellName );

                        Speak( talkingMobile, speaker, yellName, true );
                    }
                }
        */

        /// <summary>
        /// Get the loyalty level for our speaker
        /// </summary>
        private static int GetFriendLoyalty( Mobile speaker, ITalkingMobile talkingMobile )
        {
            int loyalty = 0;

            if( talkingMobile != null )
                talkingMobile.TalkingFriends.TryGetValue( speaker, out loyalty );

            DebugSay( talkingMobile, speaker, String.Format( "My loyalty to {0} is {1}", speaker.Name, loyalty ) );

            Debug( "GetFriendLoyalty: " + loyalty );

            return loyalty;
        }

        /// <summary>
        /// Add a friend to ITalkingMobile friend list. Default loyalty value is 1
        /// </summary>
        private static void AddFriend( Mobile speaker, ITalkingMobile talkingMobile )
        {
            Debug( "AddFriend" );

            if( talkingMobile == null || speaker == null )
                return;

            if( talkingMobile.TalkingFriends == null )
                talkingMobile.TalkingFriends = new Dictionary<Mobile, int>();

            if( !talkingMobile.TalkingFriends.ContainsKey( speaker ) )
            {
                DebugSay( talkingMobile, speaker, string.Format( "My new friend is {0}", speaker.Name ) );
                talkingMobile.TalkingFriends.Add( speaker, 1 );
            }

            talkingMobile.NextFriend = DateTime.Now + Config.RefreshFriendsDelay;
        }

        /// <summary>
        /// Increase loyalty of our speaker in the ITalkingMobile friend list
        /// </summary>
        private static void IncreaseLoyalty( Mobile speaker, ITalkingMobile talkingMobile )
        {
            Debug( "IncreaseLoyalty" );

            if( talkingMobile != null && talkingMobile.TalkingFriends != null && talkingMobile.TalkingFriends.ContainsKey( speaker ) )
            {
                if( talkingMobile.TalkingFriends[ speaker ] < Config.MaxLoyalty )
                {
                    DebugSay( talkingMobile, speaker, string.Format( "My loyalty is now {0}", talkingMobile.TalkingFriends[ speaker ] + 1 ) );
                    talkingMobile.TalkingFriends[ speaker ]++;
                }
            }
        }

        /// <summary>
        /// If our friend is loyal, get a better way to refer him
        /// </summary>
        private static string GetFriendTitle( int loyalty )
        {
            Debug( "GetFriendTitle" );

            switch( loyalty )
            {
                case 3:
                case 4:
                    return "Friend, ";
                case 5:
                    return "Good Friend, ";
                case 6:
                    return "Great Friend, ";
                case 7:
                    return "Old Friend, ";
                case 8:
                    return "Dear Friend, ";
                case 9:
                    return "Dearest Friend, ";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Say an ASCII message and turn to speaker
        /// </summary>
        private static void Speak( ITalkingMobile talkingMobile, Mobile to, string text, bool turn )
        {
            Debug( "Speak" );

            if( talkingMobile != null && talkingMobile is Mobile )
            {
                if( turn )
                    SpellHelper.Turn( (Mobile)talkingMobile, to );

                ( (Mobile)talkingMobile ).Say( true, text );
            }
        }

        private static KarmaLevel GetKarmaForSpeaker( Mobile m )
        {
            if( m.Criminal || m.Kills > 5 )
                return KarmaLevel.Worst;

            if( m.Karma < -2000 )
                return KarmaLevel.Worst;
            else if( m.Karma < 2000 )
                return KarmaLevel.Normal;
            else if( m.Karma < 7500 )
                return KarmaLevel.Good;
            else
                return KarmaLevel.Awesome;
        }

        public static void ExplainStatus( BaseCreature bc )
        {
            bc.Say( true, String.Format( "I'm {0}supporting advanced speech", bc.SupportSpeech ? "" : "not " ) );
            bc.Say( true, String.Format( "My speech dictionary is: {0}.", bc.SpeechName ?? "none" ) );
            bc.Say( true, String.Format( "I can {0}have a conversation", bc.IsSpeechEnabled ? "" : "not " ) );
            bc.Say( true, String.Format( "I can {0}have friends.", bc.CanHaveFriends ? "" : "not " ) );
        }

        private static Dictionary<string, string> GetDictionaryForKarmaLevel( KarmaLevel level )
        {
            switch( level )
            {
                case KarmaLevel.Worst:
                    return m_KarmaGreetingsWorst;
                case KarmaLevel.Normal:
                    return m_KarmaGreetingsNormal;
                case KarmaLevel.Good:
                    return m_KarmaGreetingsGood;
                case KarmaLevel.Awesome:
                    return m_KarmaGreetingsAwe;
            }

            return m_KarmaGreetingsNormal;
        }

        private static Dictionary<string, string> ReadKeyValueDictionary( string path )
        {
            if( !File.Exists( Path.Combine( Config.SpeechPath, path ) ) )
            {
                Console.WriteLine( "Warning: Speech system is enabled but {0} has not been found.", path );
                return null;
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();

            using( StreamReader ip = new StreamReader( Path.Combine( Config.SpeechPath, path ) ) )
            {
                string line;

                while( ( line = ip.ReadLine() ) != null )
                {
                    if( line.Length == 0 || line.StartsWith( "#" ) )
                        continue;

                    string[] split = line.Split( '\t' );

                    try
                    {
                        if( dict.ContainsKey( split[ 0 ] ) )
                            Log( line, path );
                        else
                            dict.Add( split[ 0 ].Trim().ToLower(), split[ 1 ] );
                    }
                    catch
                    {
                        Console.WriteLine( "Warning: Invalid speechTable entry:" );
                        Console.WriteLine( line );
                        Log( line, path );
                    }
                }
            }

            if( Server.Core.Debug ) // really too massive...
                Debug( "Dictionary {0} has {1} entries", path, dict.Count );

            return dict;
        }

        #region debugging and logging
        private static void DebugSay( ITalkingMobile talkingMobile, Mobile talker, string text )
        {
            if( talker.AccessLevel == AccessLevel.Developer )
            {
                if( talkingMobile is BaseCreature )
                    ( (BaseCreature)talkingMobile ).Say( text );
            }
        }

        private static void Debug( string format, params object[] args )
        {
            Debug( String.Format( format, args ) );
        }

        private static void Debug( string toCast )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( toCast );
        }

        private static void Log( string s, string group )
        {
            using ( StreamWriter w = new StreamWriter( "Logs/speech-system-errors.log", true ) )
                w.WriteLine( "String {0} for group {1} is redundant or invalid.", s, group );
        }
        #endregion

        private static List<string> ReadListOfStrings( string path )
        {
            if( !File.Exists( Path.Combine( Config.SpeechPath, path ) ) )
            {
                Console.WriteLine( "Warning: Speech system is enabled but {0} has not been found.", path );
                return null;
            }

            List<string> list = new List<string>();

            using( StreamReader ip = new StreamReader( Path.Combine( Config.SpeechPath, path ) ) )
            {
                string line;

                while( ( line = ip.ReadLine() ) != null )
                {
                    if( line.Length == 0 || line.StartsWith( "#" ) )
                        continue;

                    list.Add( line );
                }
            }

            if( Server.Core.Debug )
                Debug( "List {0} has {1} entries", path, list.Count );

            return list;
        }

        public static void InitializeTalkingMobile( ITalkingMobile talkingMobile )
        {
            if( talkingMobile == null )
                return;

            Debug( string.Format( "InitializeTalkingMobile: {0}", talkingMobile.GetType().Name ) );

            string speechName = GetSpeechForType( talkingMobile.GetType() );

            Debug( string.Format( "InitializeTalkingMobile Speech: {0}", ( speechName ?? "null" ) ) );

            talkingMobile.SpeechName = speechName;
            talkingMobile.NextFriend = DateTime.Now;
            talkingMobile.LastGreet = DateTime.Now;
        }

        private static string[] m_Endings = new string[]
        {
            "'Twas nice speaking with thee.",
            "I suppose I have other things to do.",
            "Thou seemst to be done speaking with me.",
            "Unless thou needest aught else, I am done with speaking.",
            "Unless thou needest aught else, I have my work to do.",
            "'A pleasure talking with thee.", "Farewell.",
            "Goodbye.", "Until later.", "Until we meet again.",
            "'Twas a pleasure.", "Farewell for now.", "Goodbye for now.",
            "Thou'rt done, and I have work to do.",
            "I have matters to attend to.", "Fare thee well."
        };

        private static string[] m_LowIntEndings = new string[]
        {
            "'Twas nice speakin' with ye.",
            "I's got other things to do, I reckon.",
            "Thou seemst to be done speakin' to me.",
            "'Less'n thou needst aught else, I's done.",
            "'Less'n thou needst aught else, I's got work to be doing.",
            "Nice talkin' with thee.", "Farewell.", "Bye!",
            "'Til later.", "'Til we meet again.", "Farewell for now.",
            "Goodbye for now.", "Thee's done, and I have work to do.",
            "I's got things to do.", "Fare thee well."
        };
    }
}