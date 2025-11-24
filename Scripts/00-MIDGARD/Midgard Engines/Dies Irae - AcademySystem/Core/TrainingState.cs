/***************************************************************************
 *                               TrainingState.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.Academies
{
    public abstract class TrainingState
    {
        public abstract string Name { get; }
        public abstract Disciplines Discipline { get; }

        public bool IsLearning { get; set; }
        public bool IsTeaching { get; set; }

        public virtual double MinSkillLevel { get { return 0.0; } }
        public virtual double MaxkillLevel { get { return MinSkillLevel + 30.0; } }
        public virtual SkillName Skill { get { return SkillName.Alchemy; } }

        public static TrainingState GetInstance( Disciplines discipline )
        {
            switch( discipline )
            {
                case Disciplines.ArtOfMining: return new ArtOfMining();
                default: return null;
            }
        }
    }
}