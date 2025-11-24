using Server;
using Server.Mobiles;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class HowlingSoulOfRedemption : BaseVirtueChampionHowlingCreature
    {
        public HowlingSoulOfRedemption( AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed )
            : base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
        {
        }

        public override string GetHowl()
        {
            return Utility.RandomStringList( Config.RedemptionHowls );
        }

        #region serialization
        public HowlingSoulOfRedemption( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}