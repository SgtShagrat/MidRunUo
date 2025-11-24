using Server;
using Server.Items;

using Core = Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2634 barbarian helm - ( craftabile solo da NorthernHuman, vestibile solo da NorthernHuman )
    /// </summary>
    public class BarbarianPlateHelm : PlateHelm
    {
        [Constructable]
        public BarbarianPlateHelm()
            : this( 0 )
        {
        }

        [Constructable]
        public BarbarianPlateHelm( int hue )
        {
            ItemID = 0x2634;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "barbarian helm"; }
        }

        public override Race RequiredRace
        {
            get { return Core.NorthernHuman; }
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor
                   || from.Race == Core.NorthernHuman;
        }

        #region serialization
        public BarbarianPlateHelm( Serial serial )
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