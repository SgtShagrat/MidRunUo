using System;

namespace Server.Items
{
    public class FlyingShadow : Item
    {
        [Constructable]
        public FlyingShadow( int itemID )
            : base( itemID )
        {
            Movable = false;

            Hue = 962;

            new InternalTimer( this ).Start();
        }

        public FlyingShadow( Serial serial )
            : base( serial )
        {
            new InternalTimer( this ).Start();
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
        }

        private class InternalTimer : Timer
        {
            private Item m_FlyingShadow;

            public InternalTimer( Item FlyingShadow )
                : base( TimeSpan.FromSeconds( 0.2 ) )
            {
                m_FlyingShadow = FlyingShadow;
            }

            protected override void OnTick()
            {
                m_FlyingShadow.Delete();
            }
        }
    }
}