using System;
using System.Collections.Generic;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Network;

/*
** QuestRewardGump
** ArteGordon
** updated 9/18/05
**
** Gives out rewards based on the XmlQuestReward reward list entries and the players Credits that are accumulated through quests with the XmlQuestPoints attachment.
** The Gump supports Item, Mobile, and Attachment type rewards.
*/

namespace Server.Gumps
{
    public class QuestRewardGump : Gump
    {
        private List<XmlQuestPointsRewards> Rewards;

        private const int YInc = 35;
        private const int XCreditoffset = 350;
        private const int XPointsoffset = 480;
        private const int MaxItemsPerPage = 9;
        private int m_Viewpage;

        public QuestRewardGump( Mobile from, int page )
            : base( 20, 30 )
        {
            from.CloseGump( typeof( QuestRewardGump ) );

            // determine the gump size based on the number of rewards
            Rewards = XmlQuestPointsRewards.RewardsList;

            m_Viewpage = page;

            int height = MaxItemsPerPage * YInc + 120;
            int width = XPointsoffset + 110;

            /*
            if(Rewards != null && Rewards.Count > 0)
            {
                height = Rewards.Count*YInc + 120;
            }
            */

            AddBackground( 0, 0, width, height, 0xDAC );

            AddHtml( 40, 20, 350, 50, "Rewards Available for Purchase with QuestPoints Credits", false, false );

            AddLabel( 400, 20, 0, String.Format( "Available Credits: {0}", XmlQuestPoints.GetCredits( from ) ) );

            //AddButton( 30, height - 35, 0xFB7, 0xFB9, 0, GumpButtonType.Reply, 0 );
            //AddLabel( 70, height - 35, 0, "Close" );

            // put the page buttons in the lower right corner
            if( Rewards != null && Rewards.Count > 0 )
            {
                AddLabel( width - 165, height - 35, 0,
                         String.Format( "Page: {0}/{1}", m_Viewpage + 1, Rewards.Count / MaxItemsPerPage + 1 ) );

                // page up and down buttons
                AddButton( width - 55, height - 35, 0x15E0, 0x15E4, 13, GumpButtonType.Reply, 0 );
                AddButton( width - 35, height - 35, 0x15E2, 0x15E6, 12, GumpButtonType.Reply, 0 );
            }

            AddLabel( 70, 50, 40, "Reward" );
            AddLabel( XCreditoffset, 50, 40, "Credits" );
            AddLabel( XPointsoffset, 50, 40, "Min Points" );

            // display the items with their selection buttons
            if( Rewards != null )
            {
                int y = 50;
                for( int i = 0; i < Rewards.Count; i++ )
                {
                    if( i / MaxItemsPerPage != m_Viewpage )
                        continue;

                    XmlQuestPointsRewards r = Rewards[ i ];
                    if( r == null )
                        continue;

                    y += YInc;

                    int texthue = 0;

                    // display the item
                    if( r.MinPoints > XmlQuestPoints.GetPoints( from ) )
                    {
                        texthue = 33;
                    }
                    else
                    {
                        // add the selection button
                        AddButton( 30, y, 0xFA5, 0xFA7, 1000 + i, GumpButtonType.Reply, 0 );
                    }

                    // display the name
                    AddLabel( 70, y + 3, texthue, r.Name );

                    // display the cost
                    AddLabel( XCreditoffset, y + 3, texthue, r.Cost.ToString() );


                    // display the item
                    if( r.ItemID > 0 )
                        AddItem( XCreditoffset + 60, y, r.ItemID );

                    // display the min points requirement
                    AddLabel( XPointsoffset, y + 3, texthue, r.MinPoints.ToString() );
                }
            }
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( info == null || state == null || state.Mobile == null || Rewards == null )
                return;

            Mobile from = state.Mobile;

            switch( info.ButtonID )
            {
                case 12:
                    // page up
                    int nitems = 0;
                    if( Rewards != null )
                        nitems = Rewards.Count;

                    int page = m_Viewpage + 1;
                    if( page > nitems / MaxItemsPerPage )
                    {
                        page = nitems / MaxItemsPerPage;
                    }
                    state.Mobile.SendGump( new QuestRewardGump( state.Mobile, page ) );
                    break;
                case 13:
                    // page down
                    page = m_Viewpage - 1;
                    if( page < 0 )
                    {
                        page = 0;
                    }
                    state.Mobile.SendGump( new QuestRewardGump( state.Mobile, page ) );
                    break;
                default:
                    {
                        if( info.ButtonID >= 1000 )
                        {
                            int selection = info.ButtonID - 1000;
                            if( selection < Rewards.Count )
                            {
                                XmlQuestPointsRewards r = Rewards[ selection ];

                                // check the price
                                if( XmlQuestPoints.HasCredits( from, r.Cost ) )
                                {
                                    // create an instance of the reward type
                                    object o = null;

                                    try
                                    {
                                        o = Activator.CreateInstance( r.RewardType, r.RewardArgs );
                                    }
                                    catch
                                    {
                                    }

                                    bool received = true;

                                    if( o is Item )
                                    {
                                        // and give them the item
                                        from.AddToBackpack( (Item)o );
                                    }
                                    else if( o is Mobile )
                                    {
                                        // if it is controllable then set the buyer as master.  Note this does not check for control slot limits.
                                        if( o is BaseCreature )
                                        {
                                            var b = o as BaseCreature;
                                            b.Controlled = true;
                                            b.ControlMaster = from;
                                        }

                                        ( (Mobile)o ).MoveToWorld( from.Location, from.Map );
                                    }
                                    else if( o is XmlAttachment )
                                    {
                                        var a = o as XmlAttachment;

                                        XmlAttach.AttachTo( from, a );
                                    }
                                    else
                                    {
                                        from.SendMessage( 33, "unable to create {0}.", r.RewardType.Name );
                                        received = false;
                                    }

                                    // complete the transaction
                                    if( received )
                                    {
                                        // charge them
                                        XmlQuestPoints.TakeCredits( from, r.Cost );
                                        from.SendMessage( "You have purchased {0} for {1} credits.", r.Name, r.Cost );
                                    }
                                }
                                else
                                {
                                    from.SendMessage( "Insufficient Credits for {0}.", r.Name );
                                }
                                from.SendGump( new QuestRewardGump( from, m_Viewpage ) );
                            }
                        }
                        break;
                    }
            }
        }
    }
}