/***************************************************************************
 *                               BaseClassAttributes.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.Academies
{
    [PropertyObject]
    public abstract class BaseAcademyAttributes
    {
        public AcademyPlayerState State { get; private set; }

        protected BaseAcademyAttributes( AcademyPlayerState state )
        {
            State = state;
        }

        public override string ToString()
        {
            return "...";
        }
    }
}