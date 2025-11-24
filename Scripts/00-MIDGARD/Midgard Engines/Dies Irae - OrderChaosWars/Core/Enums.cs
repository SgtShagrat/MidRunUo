/***************************************************************************
 *                               Enums.cs
 *                            --------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/
 
namespace Midgard.Engines.OrderChaosWars
{
    public enum BattleType
    {
        None,

        Nujelm,
        Moonglow
    }

    public enum WarPhase
    {
        Idle = 0,

        PreBattle,
        BattlePending,
        PostBattle
    }

    public enum Virtue
    {
        None = 0,

        Order,
        Chaos
    }
}