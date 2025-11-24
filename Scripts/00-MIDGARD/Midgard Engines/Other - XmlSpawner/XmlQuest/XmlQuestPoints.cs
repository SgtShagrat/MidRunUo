using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Gumps;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public class XmlQuestPoints : XmlAttachment
    {
        public string GuildFilter;
        public string NameFilter;

        public List<QuestEntry> QuestList { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Rank { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DeltaRank { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime WhenRanked { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Points { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Credits { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int QuestsCompleted { get; set; }

        public XmlQuestPoints( ASerial serial )
            : base( serial )
        {
            QuestList = new List<QuestEntry>();
        }

        [Attachable]
        public XmlQuestPoints()
        {
            QuestList = new List<QuestEntry>();
        }

        public new static void Initialize()
        {
            // CommandSystem.Register( "PuntiQuest", AccessLevel.Player, new CommandEventHandler( CheckQuestPoints_OnCommand ) );

            CommandSystem.Register( "QuestLog", AccessLevel.Player, new CommandEventHandler( QuestLog_OnCommand ) );
        }

        [Usage( "PuntiQuest" )]
        [Description( "Restituisce il valore dei punti per le quest." )]
        public static void CheckQuestPoints_OnCommand( CommandEventArgs e )
        {
            if( e == null || e.Mobile == null )
                return;

            string msg = null;

            var p = (XmlQuestPoints)XmlAttach.FindAttachment( e.Mobile, typeof( XmlQuestPoints ) );
            if( p != null )
            {
                msg = p.OnIdentify( e.Mobile );
            }

            if( msg != null )
                e.Mobile.SendMessage( msg );
        }


        [Usage( "QuestLog" )]
        [Description( "Informa il giocatore della suo stato nelle quest statiche." )]
        public static void QuestLog_OnCommand( CommandEventArgs e )
        {
            if( e == null || e.Mobile == null )
                return;

            e.Mobile.CloseGump( typeof( QuestLogGump ) );
            e.Mobile.SendGump( new QuestLogGump( e.Mobile ) );
        }


        public static void GiveQuestPoints( Mobile from, IXmlQuest quest )
        {
            if( from == null || quest == null )
                return;

            // find the XmlQuestPoints attachment

            var p = (XmlQuestPoints)XmlAttach.FindAttachment( from, typeof( XmlQuestPoints ) );

            // if doesnt have one yet, then add it
            if( p == null )
            {
                p = new XmlQuestPoints();
                XmlAttach.AttachTo( from, p );
            }

            // if you wanted to scale the points given based on party size, karma, fame, etc.
            // this would be the place to do it
            int points = quest.Difficulty;

            // update the questpoints attachment information
            p.Points += points;
            p.Credits += points;
            p.QuestsCompleted++;

            if( from != null )
            {
                from.SendMessage( "You have received {0} quest points!", points );
            }

            // add the completed quest to the quest list
            QuestEntry.AddQuestEntry( from, quest );

            // update the overall ranking list
            XmlQuestLeaders.UpdateQuestRanking( from, p );
        }

        public static int GetCredits( Mobile m )
        {
            int val = 0;

            var p = (XmlQuestPoints)XmlAttach.FindAttachment( m, typeof( XmlQuestPoints ) );
            if( p != null )
            {
                val = p.Credits;
            }

            return val;
        }

        public static int GetPoints( Mobile m )
        {
            int val = 0;

            var p = (XmlQuestPoints)XmlAttach.FindAttachment( m, typeof( XmlQuestPoints ) );
            if( p != null )
            {
                val = p.Points;
            }

            return val;
        }

        public static bool HasCredits( Mobile m, int credits )
        {
            if( m == null || m.Deleted )
                return false;

            var p = (XmlQuestPoints)XmlAttach.FindAttachment( m, typeof( XmlQuestPoints ) );

            if( p != null )
            {
                if( p.Credits >= credits )
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

            var p = (XmlQuestPoints)XmlAttach.FindAttachment( m, typeof( XmlQuestPoints ) );

            if( p != null )
            {
                if( p.Credits >= credits )
                {
                    p.Credits -= credits;
                    return true;
                }
            }

            return false;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
            // version 0
            writer.Write( Points );
            writer.Write( Credits );
            writer.Write( QuestsCompleted );
            writer.Write( Rank );
            writer.Write( DeltaRank );
            writer.Write( WhenRanked );

            // save the quest history
            if( QuestList != null )
            {
                writer.Write( QuestList.Count );

                foreach( QuestEntry e in QuestList )
                {
                    e.Serialize( writer );
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
                case 0:

                    Points = reader.ReadInt();
                    Credits = reader.ReadInt();
                    QuestsCompleted = reader.ReadInt();
                    Rank = reader.ReadInt();
                    DeltaRank = reader.ReadInt();
                    WhenRanked = reader.ReadDateTime();

                    int nquests = reader.ReadInt();

                    if( nquests > 0 )
                    {
                        QuestList = new List<QuestEntry>( nquests );
                        for( int i = 0; i < nquests; i++ )
                        {
                            var e = new QuestEntry();
                            e.Deserialize( reader );

                            QuestList.Add( e );
                        }
                    }


                    // get the owner of this in order to rebuild the rankings
                    Mobile quester = reader.ReadMobile();

                    // rebuild the ranking list
                    // if they have never made a kill, then dont rank
                    if( quester != null && QuestsCompleted > 0 )
                    {
                        XmlQuestLeaders.UpdateQuestRanking( quester, this );
                    }
                    break;
            }
        }

        public override string OnIdentify( Mobile from )
        {
            return
                String.Format(
                    "Quest Points Status:\nTotal Quest Points = {0}\nTotal Quests Completed = {1}\nQuest Credits Available = {2}",
                    Points, QuestsCompleted, Credits );
        }
    }
}