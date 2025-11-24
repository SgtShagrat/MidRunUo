/***************************************************************************
 *                               AcademyPlayerStateCollection.cs
 *
 *   begin                : 07 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System.Collections.ObjectModel;

using Server;

namespace Midgard.Engines.Academies
{
    [PropertyObject]
    public class AcademyPlayerStateCollection : Collection<AcademyPlayerState>
    {
        public override string ToString()
        {
            return "...";
        }
    }
}