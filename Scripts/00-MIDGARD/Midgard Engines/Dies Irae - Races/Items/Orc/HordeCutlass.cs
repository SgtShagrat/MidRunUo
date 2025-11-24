using Server;
using Server.Items;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3359 Horde Cutlas - ( craftabile solo da razza: orco )
    /// </summary>
    public class HordeCutlass : Cutlass
    {
        public override string DefaultName { get { return "horde cutlass"; } }

        [Constructable]
        public HordeCutlass()
        {
            ItemID = 0x3359;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor
                   || from.Race == Core.HighOrc;
        }

        #region serialization
        public HordeCutlass( Serial serial )
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