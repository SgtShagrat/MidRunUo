using System;

namespace Server.Items
{
    public enum SkillCategory
    {
        None,
        Miscellaneous,
        CombatRatings,
        Actions,
        LoreKnowledge,
        Magical,
        CraftingHarvesting
    }

    public class PowerScrollFilter
    {
        private SkillCategory m_SkillCat;
        private int m_SkillValue;

        public bool IsDefault
        {
            get { return (m_SkillCat == SkillCategory.None && m_SkillValue == 0); }
        }

        public void Clear()
        {
            m_SkillCat = SkillCategory.None;
            m_SkillValue = 0;
        }

        public SkillCategory SkillCat
        {
            get { return m_SkillCat; }
            set { m_SkillCat = value; }
        }

        public int SkillValue
        {
            get { return m_SkillValue; }
            set { m_SkillValue = value; }
        }

        public PowerScrollFilter()
        {
        }

        public PowerScrollFilter(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    m_SkillCat = (SkillCategory)reader.ReadEncodedInt();
                    m_SkillValue = reader.ReadEncodedInt();
                    break;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            if (IsDefault)
            {
                writer.WriteEncodedInt(0); // version
            }
            else
            {
                writer.WriteEncodedInt(1); // version

                writer.WriteEncodedInt((int)m_SkillCat);
                writer.WriteEncodedInt((int)m_SkillValue);
            }
        }
    }
}
