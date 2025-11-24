using Midgard.Engines.Races;
using Server;
using Server.Items;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2600 Barbarian Gorget ( craftabile solo da razza : Nordico )
    /// </summary>
    public class BarbarianGorget : PlateGorget
    {
        [Constructable]
        public BarbarianGorget()
            : this( 0 )
        {
        }

        [Constructable]
        public BarbarianGorget( int hue )
        {
            ItemID = 0x2600;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "barbarian gorget"; }
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor
                   || from.Race == Core.NorthernHuman;
        }

        #region serialization

        public BarbarianGorget( Serial serial )
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