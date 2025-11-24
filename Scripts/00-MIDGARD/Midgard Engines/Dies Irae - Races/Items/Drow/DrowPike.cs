using Server;
using Server.Items;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    /// <summary>
    /// 0x335C Drow Pike - ( craftabile solo da razza: drow )
    /// </summary>
    public class DrowPike : Pike
    {
        public override string DefaultName { get { return "drow pike"; } }

        public override int NumDice { get { return 2; } }
	    public override int NumSides { get { return 18; } }
	    public override int DiceBonus { get { return 0; } }

        public override int OldHitSound { get { return 572; } }
        public override int OldMissSound { get { return 569; } }

        [Constructable]
        public DrowPike()
        {
            ItemID = 0x335C;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor
                   || from.Race == Core.Drow;
        }

        #region serialization
        public DrowPike( Serial serial )
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