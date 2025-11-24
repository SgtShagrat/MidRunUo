/***************************************************************************
 *                               BaseAcademyQuest.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Engines.Quests;

namespace Midgard.Engines.Academies
{
    public abstract class BaseAcademyQuest : BaseQuest
    {
        public abstract AcademySystem Academy { get; }
        public abstract Disciplines Discipline { get; }

        public virtual double MinSkillLevel { get { return 0.0; } }
        public virtual double MaxkillLevel { get { return MinSkillLevel + 30.0; } }
        public virtual SkillName Skill { get { return SkillName.Alchemy; } }

        #region serialization
        protected BaseAcademyQuest()
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();
        }
        #endregion
    }
}