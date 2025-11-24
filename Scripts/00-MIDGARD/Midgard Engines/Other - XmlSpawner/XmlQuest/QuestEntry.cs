using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public class QuestEntry
    {
        public Mobile Quester;
        public string Name;
        public DateTime WhenCompleted;
        public DateTime WhenStarted;
        public int Difficulty;
        public bool PartyEnabled;
        public int TimesCompleted = 1;

        public QuestEntry()
        {
        }

        public QuestEntry( Mobile m, IXmlQuest quest )
        {
            Quester = m;
            if( quest != null )
            {
                WhenStarted = quest.TimeCreated;
                WhenCompleted = DateTime.Now;
                Difficulty = quest.Difficulty;
                Name = quest.Name;
            }
        }

        public virtual void Serialize( GenericWriter writer )
        {
            writer.Write( 0 ); // version

            writer.Write( Quester );
            writer.Write( Name );
            writer.Write( WhenCompleted );
            writer.Write( WhenStarted );
            writer.Write( Difficulty );
            writer.Write( TimesCompleted );
            writer.Write( PartyEnabled );
        }

        public virtual void Deserialize( GenericReader reader )
        {
            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    Quester = reader.ReadMobile();
                    Name = reader.ReadString();
                    WhenCompleted = reader.ReadDateTime();
                    WhenStarted = reader.ReadDateTime();
                    Difficulty = reader.ReadInt();
                    TimesCompleted = reader.ReadInt();
                    PartyEnabled = reader.ReadBool();
                    break;
            }
        }

        public static void AddQuestEntry( Mobile m, IXmlQuest quest )
        {
            if( m == null || quest == null )
                return;

            // get the XmlQuestPoints attachment from the mobile
            var p = (XmlQuestPoints)XmlAttach.FindAttachment( m, typeof( XmlQuestPoints ) );

            if( p == null )
                return;

            // look through the list of quests and see if it is one that has already been done
            if( p.QuestList == null )
                p.QuestList = new List<QuestEntry>();

            bool found = false;
            foreach( QuestEntry e in p.QuestList )
            {
                if( e.Name == quest.Name )
                {
                    // found a match, so just change the number and dates
                    e.TimesCompleted++;
                    e.WhenStarted = quest.TimeCreated;
                    e.WhenCompleted = DateTime.Now;
                    // and update the difficulty and party status
                    e.Difficulty = quest.Difficulty;
                    e.PartyEnabled = quest.PartyEnabled;
                    found = true;
                    break;
                }
            }

            if( !found )
            {
                // add a new entry
                p.QuestList.Add( new QuestEntry( m, quest ) );
            }
        }
    }
}