/***************************************************************************
 *                               VirtueChampion.cs
 *
 *   begin                : 14 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Midgard.Engines.MiniChampionSystem;

using Server;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public abstract class VirtueChampionInfo : ChampionSpawnInfo
    {
        public abstract Virtues Virtue { get; }

        public Mobile GenChampion()
        {
            return new Soulkeeper( Virtue );
        }

        public override void OnChampionRespawned( MiniChampionSpawn spawn, Mobile m )
        {
            switch( Virtue )
            {
                case Virtues.Honesty: m.Body = 146; break;
                case Virtues.Compassion: m.Body = 26; break;
                case Virtues.Valor: m.Body = 310; break;
                case Virtues.Justice: m.Body = 400; break;
                case Virtues.Sacrifice: m.Body = 24; break;
                case Virtues.Honor: m.Body = 970; break;
                case Virtues.Spirituality: m.Body = 3; break;
                case Virtues.Humility: m.Body = 50; break;
            }
        }
    }
}