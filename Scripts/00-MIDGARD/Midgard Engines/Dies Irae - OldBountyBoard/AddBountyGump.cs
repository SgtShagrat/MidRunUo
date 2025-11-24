using System;

using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class AddBountyGump : Gump
    {
        private readonly Mobile m_Killer;
        private readonly Mobile m_Victim;

        public AddBountyGump( Mobile victim, Mobile killer ) : base( 100, 100 )
        {
            m_Killer = killer;
            m_Victim = victim;

            BuildGump();
        }

        private void BuildGump() 
        {
            Closable = false;
            Resizable = false;
			
            AddPage( 0 );

            AddImage( 0, 0, 1140 );
            AddHtml( 62, 47, 270, 20, @"<Center>Place Bounty", false, false);
            string text = String.Format( "<BASEFONT COLOR=BLACK>Would you like to place a bounty on {0}'s head?</FONT>", m_Killer.Name );
            AddHtml( 62, 74, 270, 100, text, false, false);
            AddHtml( 62, 120, 40, 35, "Amount", false, false );
            AddTextEntry( 112, 120, 120, 20, 0x480, 0, String.Format( "{0}", 1000 ) ); 
            AddButton( 125, 190, 1149, 1148, 1, GumpButtonType.Reply, 0 );
            AddButton( 200, 190, 1146, 1146, 2, GumpButtonType.Reply, 0 );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;

            switch ( info.ButtonID )
            {
                case 1: 
                    {    
                        try
                        {
                            TextRelay te = info.GetTextEntry( 0 );

                            if( te != null )
                            {
                                int price = Convert.ToInt32( te.Text, 10 );

                                if( price < 1000 )
                                {
                                    from.SendAsciiMessage( "The bounty must be at least {0} gold.", 1000 );
                                    from.SendGump( new AddBountyGump( from, m_Killer ) );
                                    return;
                                }

                                if( m_Killer != null && !m_Killer.Deleted )
                                {
                                    //remove bounty gold
							
                                    if ( !Banker.Withdraw( from, price ) )
                                    {
                                        from.SendAsciiMessage( "You cannot afford a bounty of {0}!", price );
                                        from.SendGump( new AddBountyGump( from, m_Killer ), false );
                                        return;
                                    }

                                    from.SendAsciiMessage( "It would not be wise to place a bounty in this land, but you report the knave for murderer anyway." );
                                    m_Killer.Kills++;
                                    m_Killer.ShortTermMurders++;

                                    if ( m_Killer is PlayerMobile )
                                        ((PlayerMobile)m_Killer).ResetKillTime();

                                    m_Killer.SendAsciiMessage( "You have been reported for murder!" );//You have been reported for murder!

                                    if ( m_Killer.Kills == 5 )
                                        m_Killer.SendAsciiMessage( "You are now known as a murderer!" );//You are now known as a murderer!
                                    else if ( SkillHandlers.Stealing.SuspendOnMurder && m_Killer.Kills == 1 && m_Killer is PlayerMobile && ((PlayerMobile)m_Killer).NpcGuild == NpcGuild.ThievesGuild )
                                        m_Killer.SendAsciiMessage( "You have been suspended by the Thieves Guild." ); // You have been suspended by the Thieves Guild.

                                    // m_Killer.Bounty += price;
                                    m_Killer.SendAsciiMessage( "A bounty hath been issued for thee!" );
                                }
                            }
                        }
                        catch
                        {
                            from.SendAsciiMessage( "Bad format. #### expected." );
                            from.SendGump( new AddBountyGump( from, m_Killer ) );
                        }
                        break; 
                    }
                case 2: 
                    {
                        from.SendAsciiMessage( "You have cancelled the bounty reporting process." );
                        break; 
                    }
            }
        }

        public string Color( string text, int color )
        {
            return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
        }
    }
}