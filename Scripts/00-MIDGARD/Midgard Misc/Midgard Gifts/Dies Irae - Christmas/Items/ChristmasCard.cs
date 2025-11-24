using Server;

namespace Midgard.Engines.Events.Items
{
    public class ChristmasCard : Item
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
        public ChristmasCard( string name )
            : base( 0x14EF )
        {
            Name = name;
            Hue = Utility.RandomList( 0x36, 0x17, 0x120 );
            Weight = 1.0;

            Year = ChristmasHelper.GetCurrentYear;
        }

        #region serialization
        public ChristmasCard( Serial serial )
            : base( serial )
        {
        }

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
        }
        #endregion
    }
}