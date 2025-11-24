/***************************************************************************
 *                               IRecyclable.cs
 *
 *   begin                : 23 aprile 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.FeedStockRecoverySystem
{
    public interface IRecyclable
    {
        bool CanBeRecycledBy( Mobile from );

        double GetDifficulty( Mobile from );

        bool Recycle( Mobile from, BaseRecyclingTool tool );
    }
}