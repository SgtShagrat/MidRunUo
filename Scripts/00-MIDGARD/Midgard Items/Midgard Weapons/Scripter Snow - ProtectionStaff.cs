using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
    public class ProtectionStaff : BaseArmor
    {
        private ProtectionStaffLevel m_StaffLevel;
        private int m_PhysicalBonus;
        private int m_StaffHue;

        public enum ProtectionStaffLevel
        {
            Lesser,
            Normal,
            Greater
        }

        [Constructable]
        public ProtectionStaff() : base(0x13F8) // grafica staff
        {
            int chance = Utility.Random(100);

            if (chance < 60)
            {
                m_StaffLevel = ProtectionStaffLevel.Lesser;
                m_PhysicalBonus = Utility.RandomMinMax(1, 6);       // 1d6
            }
            else if (chance < 90)
            {
                m_StaffLevel = ProtectionStaffLevel.Normal;
                m_PhysicalBonus = Utility.RandomMinMax(1, 6) + 6;   // 1d6 + 6
            }
            else
            {
                m_StaffLevel = ProtectionStaffLevel.Greater;
                m_PhysicalBonus = Utility.RandomMinMax(1, 6) + 12;  // 1d6 + 12
            }

            // Colore random nel range 1554–1599
            m_StaffHue = Utility.RandomMinMax(1554, 1599);
            Hue = m_StaffHue;

            Weight = 5.0;
            Name = "Protection Staff";
        }

        // ✅ Override diretto dell'ArmorRating
        public override double ArmorRating
        {
            get { return m_PhysicalBonus; }
        }

        // Parametri richiesti da BaseArmor
        public override int InitMinHits { get { return 20; } }
        public override int InitMaxHits { get { return 40; } }
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }
        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

        public override void OnSingleClick(Mobile from)
        {
            string name;
            if (m_StaffLevel == ProtectionStaffLevel.Lesser)
                name = "a Lesser Protection Staff";
            else if (m_StaffLevel == ProtectionStaffLevel.Normal)
                name = "a Protection Staff";
            else
                name = "a Greater Protection Staff";

            LabelTo(from, name);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Protection Level: {0}", m_StaffLevel.ToString());
            list.Add("Armor Rating: {0}", m_PhysicalBonus);
            list.Add("Hue: {0}", m_StaffHue);
        }

        public ProtectionStaff(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
            writer.Write((int)m_StaffLevel);
            writer.Write(m_PhysicalBonus);
            writer.Write(m_StaffHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                m_StaffLevel = (ProtectionStaffLevel)reader.ReadInt();
                m_PhysicalBonus = reader.ReadInt();
                m_StaffHue = reader.ReadInt();
            }
        }
    }
}