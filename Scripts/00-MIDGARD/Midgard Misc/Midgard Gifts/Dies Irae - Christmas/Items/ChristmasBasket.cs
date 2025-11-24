using Server;
using Server.Items;

namespace Midgard.Engines.Events.Items
{
    public class ChristmasBasket : Container, IEventItem
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
        public ChristmasBasket()
            : base( 0x9AA )
        {
            Hue = Utility.RandomNeutralHue();
            Weight = 4.0;

            Year = ChristmasHelper.GetCurrentYear;
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, "Buon Natale {0} dallo Staff di Midgard!", Year );
        }

        #region serialization
        public ChristmasBasket( Serial serial )
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