using Midgard.Engines.Races;
using Server;
using Server.Items;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    /// <summary>
    /// 0x1414 barbarian gloves - ( craftabile solo da razza )
    /// </summary>
    public class BarbarianPlateGloves : PlateGloves
    {
        [Constructable]
        public BarbarianPlateGloves()
            : this( 0 )
        {
        }

        [Constructable]
        public BarbarianPlateGloves( int hue )
        {
            ItemID = 0x1414;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "barbarian gloves"; }
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor
                   || from.Race == Core.NorthernHuman;
        }

        #region serialization

        public BarbarianPlateGloves( Serial serial )
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