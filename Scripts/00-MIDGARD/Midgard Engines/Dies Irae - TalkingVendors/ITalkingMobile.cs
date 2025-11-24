using System;
using System.Collections.Generic;
using Server;

namespace Midgard.Engines.TalkingVendors
{
    public interface ITalkingMobile
    {
        /// <summary>
        /// True if our type supports advanced speech
        /// </summary>
        bool SupportSpeech { get; }

        string SpeechName { get; set; }

        /// <summary>
        /// Dictionary of key-value corresponging to ITalkingMobile reactions
        /// </summary>
        Dictionary<string, string> Speech { get; }

        /// <summary>
        /// Local ITalkingMobile parameter to speech
        /// </summary>
        bool IsSpeechEnabled { get; set; }

        /// <summary>
        /// If CanHaveFriends is false our ITalkingMobile cannot react to or add new friends
        /// </summary>
        bool CanHaveFriends { get; set; }

        /// <summary>
        /// Dictionary of friends mobile. Value is loyalty for that mobile
        /// </summary>
        Dictionary<Mobile, int> TalkingFriends { get; set; }

        /// <summary>
        /// New istant for friend searching
        /// </summary>
        DateTime NextFriend { get; set; }

        /// <summary>
        /// Instant of the last greeting
        /// </summary>
        DateTime LastGreet { get; set; }
    }
}