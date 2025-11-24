/***************************************************************************
 *                               Enums.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

namespace Midgard.Engines.WarSystem
{
    public enum BattleType
    {
        None,

        TestWarOne
    }

    public enum WarPhase
    {
        Idle = 0,

        PreBattle,
        BattlePending,
        PostBattle
    }
}