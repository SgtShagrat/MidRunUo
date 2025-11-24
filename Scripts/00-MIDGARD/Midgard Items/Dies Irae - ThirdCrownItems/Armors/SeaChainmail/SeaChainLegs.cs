using Midgard.Engines.Races;
using Server;
using Server.Items;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    public class SeaChainLegs : ChainLegs
    {
        [Constructable]
        public SeaChainLegs()
            : this( 0 )
        {
        }

        [Constructable]
        public SeaChainLegs( int hue )
        {
            ItemID = 0x38CA;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "sea chainmail legs"; }
        }

        public override Race RequiredRace
        {
            get { return Core.Naglor; }
        }

        #region serialization

        public SeaChainLegs( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor || from.Race == Core.Naglor;
        }
    }
}