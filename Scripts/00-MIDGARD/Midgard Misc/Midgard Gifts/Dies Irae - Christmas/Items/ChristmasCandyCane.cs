using System;

using Server;
using Server.Items;

namespace Midgard.Engines.Events.Items
{
    public class ChristmasCandyCane : Food, IEventItem
    {
        #region IEventItem
        [CommandProperty( AccessLevel.GameMaster )]
        public int Year { get; set; }

        public EventType Event
        {
            get { return EventType.Christmas; }
        }
        #endregion

        private InternalTimer m_Toothache;

        [Constructable]
        public ChristmasCandyCane()
            : base( Utility.Random( 0x2BDD, 4 ) )
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 0;
            LootType = LootType.Blessed;

            Year = ChristmasHelper.GetCurrentYear;
        }

        public ChristmasCandyCane( Serial serial )
            : base( serial )
        {
        }

        public override bool Eat( Mobile from )
        {
            from.PlaySound( Utility.Random( 0x3A, 3 ) );

            if( from.Body.IsHuman && !from.Mounted )
                from.Animate( 34, 5, 1, true, false, 0 );

            if( Poison != null )
                from.ApplyPoison( Poisoner, Poison );

            if( Utility.RandomDouble() < 0.05 )
                GiveToothAche( from, 0 );
            else
                from.SendLocalizedMessage( 1077387 );

            Consume();
            return true;
        }

        private void GiveToothAche( Mobile from, int seq )
        {
            if( m_Toothache != null )
                m_Toothache.Stop();

            from.SendLocalizedMessage( 1077388 + seq );

            if( seq < 5 )
            {
                m_Toothache = new InternalTimer( this, from, seq, TimeSpan.FromSeconds( 15 ) );
                m_Toothache.Start();
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, ChristmasHelper.GetMidgardGreetings() );
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.WriteEncodedInt( Year );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Year = reader.ReadEncodedInt();

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion

        private class InternalTimer : Timer
        {
            private readonly ChristmasCandyCane m_Candycane;
            private readonly int m_Sequencer;
            private readonly Mobile m_Mobile;

            public InternalTimer( ChristmasCandyCane candycane, Mobile m, int sequencer, TimeSpan delay )
                : base( delay )
            {
                Priority = TimerPriority.OneSecond;
                m_Candycane = candycane;
                m_Mobile = m;
                m_Sequencer = sequencer;
            }

            protected override void OnTick()
            {
                if( m_Mobile != null )
                {
                    m_Candycane.GiveToothAche( m_Mobile, m_Sequencer + 1 );
                }
            }
        }
    }
}