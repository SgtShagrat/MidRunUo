/***************************************************************************
 *                               BaseVirtueChampionBoss.cs
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
    public abstract class BaseVirtueChampionBoss : BaseCreature
    {
        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }

        protected BaseVirtueChampionBoss( AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed )
            : base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
        {
        }

        #region serialization
        public BaseVirtueChampionBoss( Serial serial )
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