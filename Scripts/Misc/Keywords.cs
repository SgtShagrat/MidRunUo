using System;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Misc
{
	public class Keywords
	{
		public static void Initialize()
		{
			// Register our speech handler
			EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
		}

		public static void EventSink_Speech( SpeechEventArgs args )
		{
			Mobile from = args.Mobile;
			int[] keywords = args.Keywords;

			for ( int i = 0; i < keywords.Length; ++i )
			{
				switch ( keywords[i] )
				{
					case 0x002A: // *i resign from my guild*
					{
						if ( from.Guild != null )
							((Guild)from.Guild).RemoveMember( from );

						break;
					}
					case 0x0032: // *i must consider my sins*
					{
                        if( !Core.AOS )
                        {
                            //if( (longc==0) && (shortc==0) )
                            //    mesg := "Fear not, thou hast not slain the innocent.";
                            //elseif( (longc>0) && (shortc==0) )
                            //    mesg := "Fear not, thou hast not slain the innocent in some time.";
                            //elseif( (shortc>0) && (shortc<5) )
                            //    mesg := "Although thou hast slain the innocent, thy deeds shall not bring retribution upon thy return to the living.";
                            //elseif( (shortc>4) )
                            //    mesg := "If thou should return to the land of the living, the innocent shall wreak havoc upon thy soul.";
                            //endif
                            if( from is Midgard2PlayerMobile && ((Midgard2PlayerMobile)from).PermaRed )
                            {
                                from.SendMessage( 37, "You are known throughout the land as a murderous brigand." );
                            }
                            else if( from.ShortTermMurders == 0 && from.Kills == 0 )
                            {
                                from.SendMessage( 0x3B2, "Fear not, thou hast not slain the innocent." );
                            }
                            else if( from.ShortTermMurders == 0 && from.Kills > 0 )
                            {
                                from.SendMessage( 0x3B2, "Fear not, thou hast not slain the innocent in some time..." );
                            }
                            else if( from.ShortTermMurders > 0 && from.ShortTermMurders < 5 )
                            {
                                from.SendMessage( 0x3B2, "Although thou hast slain the innocent, thy deeds shall not bring retribution upon thy return to the living." );
                            }
                            else if( from.ShortTermMurders >= 5 )
                            {
                                from.SendMessage( 37, "If thou should return to the land of the living, the innocent shall wreak havoc upon thy soul." );
                            }
                        }
						else if( !Core.SE )
						{
							from.SendMessage( "Short Term Murders : {0}", from.ShortTermMurders );
							from.SendMessage( "Long Term Murders : {0}",  from.Kills );
						}
						else
						{
							from.SendMessage( 0x3B2, "Short Term Murders: {0} Long Term Murders: {1}", from.ShortTermMurders, from.Kills );
						}
						break;
					}
					case 0x0035: // i renounce my young player status*
					{
						if ( from is PlayerMobile && ((PlayerMobile)from).Young && !from.HasGump( typeof( RenounceYoungGump ) ) )
						{
							from.SendGump( new RenounceYoungGump() );
						}

						break;
					}
				}
			}
		}
	}
}