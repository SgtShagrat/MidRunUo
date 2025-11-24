using System;

using Server;
using Server.Network;

namespace Midgard.Engines.Events.Items
{
    public class ChristmasSnowyTree : Item, IEventItem
    {
        #region IEventItem
        [CommandProperty(AccessLevel.GameMaster)]
        public int Year { get; set; }

        public EventType Event
        {
            get { return EventType.Christmas; }
        }
        #endregion

        [Constructable]
        public ChristmasSnowyTree()
            : base( 0x2377 )
        {
            Weight = 1.0;
            LootType = LootType.Blessed;

            Year = ChristmasHelper.GetCurrentYear;
        }

        public ChristmasSnowyTree( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Christmas 2010" );
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            PublicOverheadMessage( MessageType.Regular, Utility.RandomRedHue(), true, ChristmasHelper.GetMidgardGreetings() );

            base.OnDoubleClick( from );
        }

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
    }
}