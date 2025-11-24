using System;
using Server.Mobiles;

namespace Server.Items
{
    public class SporaFungis : Item, ICarvable
    {
        private SpawnTimer m_Timer;

        [Constructable]
        public SporaFungis()
            : base( 0x2231 )
        {
            ItemID = Utility.RandomList( 0x2231, 0x2230, 0x222F, 0x222E );
            Movable = false;
            Hue = 0x497;
            Name = "Spora Fungis";

            m_Timer = new SpawnTimer( this );
            m_Timer.Start();
        }

        public void Carve( Mobile from, Item item )
        {
            from.SendMessage( "You destroy the spora fungis,you are poisoned." );
            from.Poison = Poison.Lethal;

            Delete();

            m_Timer.Stop();
        }

        public SporaFungis( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Timer = new SpawnTimer( this );
            m_Timer.Start();
        }

        private class SpawnTimer : Timer
        {
            private Item m_Item;

            public SpawnTimer( Item item )
                : base( TimeSpan.FromSeconds( 4.5 + ( Utility.RandomDouble() * 2.5 ) ) )
            {
                Priority = TimerPriority.FiftyMS;

                m_Item = item;
            }

            protected override void OnTick()
            {
                if( m_Item.Deleted )
                    return;

                Mobile spawn;


                spawn = new Fungis();


                spawn.Map = m_Item.Map;
                spawn.Location = m_Item.Location;

                m_Item.Delete();
            }
        }
    }
}
