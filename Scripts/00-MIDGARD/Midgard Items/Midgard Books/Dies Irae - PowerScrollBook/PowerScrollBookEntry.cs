using System;

namespace Server.Items
{
    public class PowerScrollBookEntry
    {
        private SkillName m_Skill;
        private int m_SkillValue;
        private int m_Price;

        public SkillName Skill
        {
            get { return m_Skill; }
            set { m_Skill = value; }
        }

        public int SkillValue
        {
            get { return m_SkillValue; }
            set { m_SkillValue = value; }
        }

        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        public Item Reconstruct()
        {
            return (new PowerScroll(m_Skill, m_SkillValue));
        }

        public PowerScrollBookEntry(PowerScroll ps)
        {
            m_Skill = ps.Skill;
            m_SkillValue = (int)ps.Value;
        }

        public PowerScrollBookEntry(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                    m_Skill = (SkillName)reader.ReadEncodedInt();
                    m_SkillValue = reader.ReadEncodedInt();
                    m_Price = reader.ReadEncodedInt();
                    break;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // Version

            writer.WriteEncodedInt((int)m_Skill);
            writer.WriteEncodedInt((int)m_SkillValue);
            writer.WriteEncodedInt((int)m_Price);
        }
    }
}
