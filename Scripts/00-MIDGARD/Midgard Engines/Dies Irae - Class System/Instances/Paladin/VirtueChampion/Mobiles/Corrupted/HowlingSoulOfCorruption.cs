/***************************************************************************
 *                               HowlingSoulOfCorruption.cs
 *
 *   begin                : 13 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class HowlingSoulOfCorruption : BaseVirtueChampionHowlingCreature
    {
        public HowlingSoulOfCorruption( AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed )
            : base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
        {
        }

        public override string GetHowl()
        {
            return Utility.RandomStringList( Config.CorruptedHowls );
        }

        #region serialization
        public HowlingSoulOfCorruption( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}