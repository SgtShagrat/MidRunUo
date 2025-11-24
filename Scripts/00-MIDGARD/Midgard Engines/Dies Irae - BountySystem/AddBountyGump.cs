/***************************************************************************
 *                               AddBountyGump.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Midgard.Gumps;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.StringQueries;

namespace Midgard.Engines.BountySystem
{
    public class AddBountyQuery : StringQuery
    {
        private const string QueryFormat = "Would you like to place a bounty on {0}'s head?";
        private const string MaxAmountFormat = "(Max is {0})";

        private readonly Mobile m_Killer;

        public AddBountyQuery( Mobile victim, Mobile killer ) :
            base( String.Format( QueryFormat, killer.Name ?? "his" ), true, StringQueryStyle.Numerical,
            GetMaxBounty( victim ), String.Format( MaxAmountFormat, GetMaxBounty( victim ) ) )
        {
            m_Killer = killer;
        }

        public override void OnResponse( NetState sender, bool okay, string text )
        {
            if( !okay )
                return;

            int price;
            if( !int.TryParse( text, out price ) )
                return;

            Mobile from = sender.Mobile;
            if( from == null || !from.Player )
                return;

            if( price < Config.DefaultMinBounty )
            {
                from.SendMessage( "The bounty must be at least {0} gold.", Config.DefaultMinBounty );
                from.SendStringQuery( new AddBountyQuery( from, m_Killer ) );
                return;
            }

            from.SendGump( new ConfirmBountyGump( from, m_Killer, price ) );
        }

        private static int GetMaxBounty( Mobile victim )
        {
            return Math.Max( Banker.GetBalance( victim ), Config.DefaultMinBounty );
        }

        public class ConfirmBountyGump : SmallConfirmGump
        {
            public ConfirmBountyGump( Mobile from, Mobile killer, int price )
                : base( "Are you sure?", Confirm_Callback, new object[] { killer, price }, true )
            {
                from.CloseGump( typeof( ConfirmBountyGump ) );
            }

            private static void Confirm_Callback( Mobile from, bool okay, object state )
            {
                object[] states = state as object[];
                if( states == null || states.Length != 2 )
                    return;

                Mobile killer = states[ 0 ] as Mobile;
                int price = (int)states[ 1 ];

                if( killer == null || killer.Deleted || price < 0 )
                    return;

                //remove bounty gold
                if( !Banker.Withdraw( from, price ) )
                {
                    from.SendMessage( "You cannot afford a bounty of {0}!", price );
                    from.SendStringQuery( new AddBountyQuery( from, killer ) );
                    return;
                }

                Core.AddEntry( @from, killer, price, DateTime.Now + Config.DefaultDecayRate );
                killer.SendMessage( "A bounty hath been issued for thee!" );
            }
        }
    }

    /*
    public class AddBountyGump : Gump
    {
        private Mobile m_Killer;
        private Mobile m_Victim;

        public AddBountyGump( Mobile victim, Mobile killer )
            : base( 100, 100 )
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

            AddBackground( 0, 0, 400, 250, 2600 );

            AddOldHtml( 0, 20, 400, 35, "<center>Place Bounty</center>" );

            string text = String.Format( "Would you like to place a bounty on {0}'s head?", m_Killer.Name );
            AddOldHtml( 50, 55, 300, 50, text );

            AddOldHtml( 50, 120, 40, 35, "Amount" );
            AddTextEntry( 100, 120, 120, 20, 0x480, 0, String.Format( "{0}", Config.DefaultMinBounty ) );

            AddButton( 200, 175, 4005, 4007, 0, GumpButtonType.Reply, 0 );
            AddHtmlLocalized( 235, 175, 110, 35, 1046363, false, false ); // CANCEL

            AddButton( 65, 175, 4005, 4007, 1, GumpButtonType.Reply, 0 );
            AddHtmlLocalized( 100, 175, 110, 35, 1046362, false, false ); // CONTINUE
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;

            switch( info.ButtonID )
            {
                case 1:
                    {
                        try
                        {
                            TextRelay te = info.GetTextEntry( 0 );

                            if( te != null )
                            {
                                int price = Convert.ToInt32( te.Text, 10 );

                                if( price < Config.DefaultMinBounty )
                                {
                                    from.SendMessage( "The bounty must be at least {0} gold.", Config.DefaultMinBounty );
                                    from.SendGump( new AddBountyGump( from, m_Killer ) );
                                    return;
                                }

                                if( m_Killer != null && !m_Killer.Deleted )
                                {
                                    //remove bounty gold
                                    if( !Banker.Withdraw( from, price ) )
                                    {
                                        from.SendMessage( "You cannot afford a bounty of {0}!", price );
                                        from.SendGump( new AddBountyGump( from, m_Killer ), false );
                                        return;
                                    }

                                    Core.AddEntry( from, m_Killer, price, DateTime.Now + Config.DefaultDecayRate );
                                    m_Killer.SendMessage( "A bounty hath been issued for thee!" );
                                }
                            }
                        }
                        catch
                        {
                            from.SendMessage( "Bad format. #### expected." );
                            from.SendGump( new AddBountyGump( from, m_Killer ) );
                        }
                        break;
                    }
                case 2:
                    {
                        from.SendLocalizedMessage( 500518 );
                        break;
                    }
            }
        }

        public string Color( string text, int color )
        {
            return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
        }
    }
    */
}