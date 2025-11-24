/***************************************************************************
 *                               ArtOfMining.cs
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
    public class ArtOfMining : TrainingState
    {
        public override double MinSkillLevel { get { return 30.0; } }
        public override double MaxkillLevel { get { return 70.0; } }
        public override SkillName Skill { get { return SkillName.Mining; } }

        public ArtOfMining()
        {
        }

        public override string Name
        {
            get { return "The Art of Mining"; }
        }

        public override Disciplines Discipline
        {
            get { return Disciplines.ArtOfMining; }
        }
    }
}