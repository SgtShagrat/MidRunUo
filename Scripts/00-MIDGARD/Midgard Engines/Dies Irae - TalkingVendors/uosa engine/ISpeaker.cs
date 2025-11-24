// Created on 7/6/2009 1:46:59 PM
// Template by -=Derrick=-
// http://www.joinuo.com

using System;

namespace Server.Mobiles
{
    [Flags]
    public enum SophisticationLevel : byte
    {
        Low = 0x01,
        Medium = 0x02,
        High = 0x04
    } ;

    [Flags]
    public enum AttitudeLevel : byte
    {
        Goodhearted = 0x01,
        Kindly = 0x02,
        Neutral = 0x04,
        Belligerent = 0x08,
        Wicked = 0x10
    } ;

    [Flags]
    public enum NotorietyLevel : byte
    {
        Infamous = 0x01,
        Outlaw = 0x02,
        Anonymous = 0x04,
        Known = 0x08,
        Famous = 0x10
    } ;

    public interface ISpeaker
    {
        SpeechFragment PersonalFragmentObj { get; }
        SpeechFragment LocalFragmentObj { get; }
        SophisticationLevel Sophistication { get; }
        AttitudeLevel Attitude { get; }
        Mobile FocusMob { get; }
        Region Region { get; }
    }
}