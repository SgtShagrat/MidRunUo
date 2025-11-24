using Midgard.Engines.Races;
using Server;
using Server.Items;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    /// <summary>
    /// 0x10A9 Barbarian Legs - ( craftabile solo da razza )
    /// </summary>
    public class BarbarianLegs : PlateLegs
    {
        [Constructable]
        public BarbarianLegs()
            : this( 0 )
        {
        }

        [Constructable]
        public BarbarianLegs( int hue )
        {
            ItemID = 0x10A9;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "barbarian legs"; }
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor
                   || from.Race == Core.NorthernHuman;
        }

        #region serialization

        public BarbarianLegs( Serial serial )
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
    }
}