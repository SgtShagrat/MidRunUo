using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;

namespace Midgard.Engines.BountySystem
{
    public class LizardCamp : BaseCamp
    {
        private Mobile m_Prisoner;
        private BaseDoor m_Gate;

        [Constructable]
        public LizardCamp() : base( 0x1D4C )
        {
        }

        public override void AddComponents()
        {
            IronGate gate = new IronGate( DoorFacing.EastCCW );
            m_Gate = gate;

            gate.KeyValue = Key.RandomValue();
            gate.Locked = true;

            AddItem( gate, -2, 1, 0 );

            MetalChest chest = new MetalChest();

            chest.ItemID = 0xE7C;
            chest.DropItem( new Key( KeyType.Iron, gate.KeyValue ) );

            TreasureMapChest.Fill( chest, 2 );

            AddItem( chest, 4, 4, 0 );

            AddMobile( new Lizardman(), 15, 0, -2, 0 );
            AddMobile( new Lizardman(), 15, 0,  1, 0 );
            AddMobile( new Lizardman(), 15, 0, -1, 0 );
            AddMobile( new Lizardman(), 15, 0,  0, 0 );

            switch ( Server.Utility.Random( 2 ) )
            {
                case 0: m_Prisoner = new Noble(); break;
                case 1: m_Prisoner = new SeekerOfAdventure(); break;
            }

            m_Prisoner.YellHue = Server.Utility.RandomList( 0x57, 0x67, 0x77, 0x87, 0x117 );

            AddMobile( m_Prisoner, 2, -2, 0, 0 );
		
            // Add the quest posting if possible
            string subject = "Hostage Taking";
            string[] message = new string[]{
                                               "Blasted lizards!",
                                               "My friend " + m_Prisoner.Name,
                                               "has been taken by the scum!",
                                               "Anyone who can help will be",
                                               "rewarded with gold.",
                                               " ",
                                               "The Duke"
                                           };

            BulletinMessage[] posts = BaseBulletinBoard.PostQuest( m_Prisoner, 500, subject, message );

            // If the quest was posted to any bulletin boards, store them
            if( posts.Length > 0 )
                m_Prisoner.Items.InsertRange( 0, posts );
        }

        public override void OnEnter( Mobile m )
        {
            base.OnEnter( m );

            if ( m.Player && m_Prisoner != null && m_Gate != null && m_Gate.Locked )
            {
                int number;

                switch( Server.Utility.Random( 4 ) )
                {
                    default:
                    case 0: number =  502264; break; // Help a poor prisoner!
                    case 1: number =  502266; break; // Aaah! Help me!
                    case 2: number = 1046000; break; // Help! These savages wish to end my life!
                    case 3: number = 1046003; break; // Quickly! Kill them for me! HELP!!
                }

                m_Prisoner.Yell( number );
            }
        }

        public LizardCamp( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version

            writer.Write( m_Prisoner );
            writer.Write( m_Gate );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                {
                    m_Prisoner = reader.ReadMobile();
                    m_Gate = reader.ReadItem() as BaseDoor;
                    break;
                }
            }
        }
    }
}