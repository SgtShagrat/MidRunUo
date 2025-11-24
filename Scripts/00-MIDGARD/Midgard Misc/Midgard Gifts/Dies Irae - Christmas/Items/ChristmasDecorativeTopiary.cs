using System;

using Server;

namespace Midgard.Engines.Events.Items
{
    public class ChristmasDecorativeTopiary : Item, IEventItem
    {
        #region IEventItem
        [CommandProperty( AccessLevel.GameMaster )]
        public int Year { get; set; }

        public EventType Event
        {
            get { return EventType.Christmas; }
        }
        #endregion

        [Constructable]
        public ChristmasDecorativeTopiary()
            : base( 0x2378 )
        {
            Weight = 1.0;
            LootType = LootType.Blessed;

            Year = ChristmasHelper.GetCurrentYear;
        }

        public ChristmasDecorativeTopiary( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Christmas {0}", Year );
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

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
    }
}