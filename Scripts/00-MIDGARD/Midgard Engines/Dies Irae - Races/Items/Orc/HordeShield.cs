using Server;
using Server.Items;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    /// <summary>
    /// 0x335B Horde Shield - ( craftabile solo da razza: orco , non subisce la colorazione del metallo rimanendo sempre hue 0 )
    /// </summary>
    public class HordeShield : HeaterShield
    {
        public override string DefaultName { get { return "horde shield"; } }

        public override bool CannotBeHuedOnCraft { get { return true; } }

        public override int BlockCircle { get { return 1; } }

        public override int OldDexBonus { get { return -7; } }

        public override int OldStrReq { get { return 30; } }

        [Constructable]
        public HordeShield()
        {
            ItemID = 0x335B;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor
                   || from.Race == Core.HighOrc;
        }

        #region serialization
        public HordeShield( Serial serial )
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