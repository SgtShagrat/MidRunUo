/***************************************************************************
 *                               BaseTroll.cs
 *
 *   begin                : 17 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    /// <summary>
    /// Troll family:
    /// 
    /// RunUO:
    ///     Troll
    ///     FrostTroll
    /// 
    /// POL:
    ///     TrollArcher
    ///     TrollMage
    ///     TrollPoisoner
    ///     TrollLord
    ///     TrollArcherElite
    ///     TrollAssassin
    ///     TrollGeneral
    ///     TrollKing
    ///     TrollMarksman
    ///     TrollShaman
    ///     TrollWarLord
    ///     TrollWarrior
    ///     TrollWarriorElite
    /// </summary>
    public abstract class BaseTroll : BaseCreature
    {
        protected BaseTroll( AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed,  double dPassiveSpeed ) : 
            base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
        {
        }

        public BaseTroll( AIType ai )
            : base( ai, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
        }

        public override bool CanRummageCorpses{ get{ return true; } }

        #region serialization
        public BaseTroll( Serial serial )
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