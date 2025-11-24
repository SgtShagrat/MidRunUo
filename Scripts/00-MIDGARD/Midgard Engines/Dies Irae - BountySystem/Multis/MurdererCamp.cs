using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;

namespace Midgard.Engines.BountySystem
{
    public class MurdererCamp : BaseCamp
    {
        private Murderer m_Murderer;

        [Constructable]
        public MurdererCamp()
            : base( Utility.RandomList( 0x070, 0x72 ) )
        {
        }

        public override void AddComponents()
        {
            WoodenTreasureChest Chest = new WoodenTreasureChest();

            // Make the chest match the graphic of chest from the multi
            Chest.ItemID = 0xE43;

            TreasureMapChest.Fill( Chest, 1 );  // BRUJO, portato da level 2 a level 1
            AddItem( Chest, 0, -2, 0 );

            // Add the defending monsters (the murderer)
            m_Murderer = new Murderer( false );
            AddMobile( m_Murderer, 15, 0, 0, 0 );

            // Add some supporting brigands to aid him
            AddMobile( new Brigand(), 15, 0, 0, 0 );
            AddMobile( new Brigand(), 15, 0, 0, 0 );
            AddMobile( new Brigand(), 15, 0, 0, 0 );

            // Add the quest posting if possible
            string subject = "Bounty";
            string[] message = new string[]{
                                               m_Murderer.Name, 
                                               "has killed " + m_Murderer.Kills + " people!", 
                                               "The lords of the land have", 
                                               "placed a price on this vile", 
                                               "murderers head of", 
                                               m_Murderer.Bounty + " gold.", 
                                               "Return the severed head of", 
                                               m_Murderer.Name + " to any", 
                                               "town guard to collect your", 
                                               "reward. Beware, the villan",
                                               "has reportedly teamed up with",
                                               "other brigands in the area!",
            };

            BulletinMessage[] posts = BaseBulletinBoard.PostQuest( m_Murderer, 500, subject, message );

            // If the quest was posted to any bulletin boards, store them
            if( posts.Length > 0 )
                m_Murderer.Items.InsertRange( 0, posts );
        }

        public override void OnEnter( Mobile m )
        {
            base.OnEnter( m );

            if( m.Player && m_Murderer != null && m_Murderer.Alive )
            {
                int number;

                switch( Utility.Random( 3 ) )
                {
                    default:
                    case 0: number = 500201; break; // I can not die!
                    case 1: number = 502048; break; // I am no murderer!
                    case 2: number = 1007046; break; // Die, pathetic fool!
                }

                m_Murderer.Yell( number );
            }
        }

        public MurdererCamp( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_Murderer );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_Murderer = reader.ReadMobile() as Murderer;
                        break;
                    }
            }
        }
    }
}