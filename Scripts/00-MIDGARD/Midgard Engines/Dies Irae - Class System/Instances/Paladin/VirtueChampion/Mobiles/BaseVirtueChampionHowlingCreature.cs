/***************************************************************************
 *                               BaseVirtueChampionHowlingCreature.cs
 *
 *   begin                : 13 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Mobiles;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public abstract class BaseVirtueChampionHowlingCreature : BaseCreature
    {
        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }

        protected BaseVirtueChampionHowlingCreature( AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed )
            : base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
        {
        }

        #region serialization
        public BaseVirtueChampionHowlingCreature( Serial serial )
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

        public override void OnThink()
        {
            base.OnThink();

            if( CanHowl() )
                Howl();
        }

        #region howl
        private DateTime m_NextHowl;

        public bool CanHowl()
        {
            if( m_NextHowl > DateTime.Now )
                return false;

            if( Combatant == null )
                return false;

            return Utility.Random( 20 ) < 5;
        }

        public void Howl()
        {
            Say( GetHowl() );

            m_NextHowl = DateTime.Now + TimeSpan.FromSeconds( 2 + Utility.RandomDouble() * 3 );
        }

        public abstract string GetHowl();
        #endregion
    }
}
