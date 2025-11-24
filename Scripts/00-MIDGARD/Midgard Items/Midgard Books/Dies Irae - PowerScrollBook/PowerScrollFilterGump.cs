using System;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class PowerScrollFilterGump : Gump
    {
        private PlayerMobile m_From;
        private PowerScrollBook m_Book;

        private const int LabelColor = 0x7FFF;

        private static int[,] m_SkillFilters = new int[,]
			{
				{ 1062229,  0 }, // All
				{       0,  1 }, // Miscellaneous
				{       0,  2 }, // Combat Ratings
				{       0,  3 }, // Actions
                {       0,  0 }, // --blank--
				{       0,  4 }, // Lore & Knowledge
                {       0,  5 }, // Magical
                {       0,  6 }, // Crafting & Harvesting
			};

        private static int[,] m_ValueFilters = new int[,]
			{
				{ 1062229, 0 }, // All
				{       0, 1 }, // 105
				{       0, 2 }, // 110
                {       0, 3 }, // 115
                {       0, 4 }  // 120
			};

        private static int[][,] m_Filters = new int[][,]
			{
				m_SkillFilters,
                m_ValueFilters
			};

        private static int[] m_XOffsets_SkillValue = new int[] { 0, 75, 180, 275, 370 };
        private static int[] m_XOffsets_SkillCategory = new int[] { 0, 105, 255, 405 };

        private static int[] m_XWidths_Small = new int[] { 50, 50, 70, 50, 50 };
        private static int[] m_XWidths_Large = new int[] { 80, 80, 80, 80 };

        private void AddFilterList(int x, int y, int[] xOffsets, int yOffset, int[,] filters, int[] xWidths, int filterValue, int filterIndex)
        {
            for (int i = 0; i < filters.GetLength(0); ++i)
            {
                int number = filters[i, 0];

                bool isSelected = (filters[i, 1] == filterValue);

                if (!isSelected && (i % xOffsets.Length) == 0)
                    isSelected = (filterValue == 0);

                if (number == 0 && filters[i, 1] == 0)
                    continue;
                else if (filterIndex == 0)
                {
                    switch (filters[i, 1])
                    {
                        case 0:
                            AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>ALL" : "<basefont color=#FFFFFF>ALL", false, false);
                            AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                            continue;

                        case 1:
                            AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>Miscellaneous" : "<basefont color=#FFFFFF>Miscellaneous", false, false);
                            AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                            continue;

                        case 2:
                            AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>Combat Ratings" : "<basefont color=#FFFFFF>Combat Ratings", false, false);
                            AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                            continue;

                        case 3:
                            AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>Actions" : "<basefont color=#FFFFFF>Actions", false, false);
                            AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                            continue;

                        case 4:
                            AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>Lore & Knowledge" : "<basefont color=#FFFFFF>Lore & Knowledge", false, false);
                            AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                            continue;

                        case 5:
                            AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>Magical" : "<basefont color=#FFFFFF>Magical", false, false);
                            AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                            continue;

                        case 6:
                            AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>Crafting & Harvesting" : "<basefont color=#FFFFFF>Crafting & Harvesting", false, false);
                            AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                            continue;
                    }
                }
                else if (filterIndex == 1)
                {
                    if (filters[i, 1] >= 1 && filters[i, 1] <= 4)
                    {
                        switch (filters[i, 1])
                        {
                            case 1:
                                AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>105" : "<basefont color=#FFFFFF>105", false, false);
                                AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                                continue;
                            case 2:
                                AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>110" : "<basefont color=#FFFFFF>110", false, false);
                                AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                                continue;
                            case 3:
                                AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>115" : "<basefont color=#FFFFFF>115", false, false);
                                AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                                continue;
                            case 4:
                                AddHtml(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, isSelected ? "<basefont color=#8484FF>120" : "<basefont color=#FFFFFF>120", false, false);
                                AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
                                continue;
                        }
                    }
                }

                AddHtmlLocalized(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], yOffset, number, isSelected ? 16927 : LabelColor, false, false);
                AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            PowerScrollFilter f = (/*m_From.UseOwnFilter ? m_From.PowerScrollBookFilter : */m_Book.Filter);

            int index = info.ButtonID;

            switch (index)
            {
                case 0: // Apply
                    {
                        m_From.SendGump(new PowerScrollBookGump(m_From, m_Book));

                        break;
                    }
                case 1: // Set Book Filter
                    {
                        m_From.UseOwnFilter = false;
                        m_From.SendGump(new PowerScrollFilterGump(m_From, m_Book));

                        break;
                    }
                case 2: // Set Your Filter
                    {
                        m_From.UseOwnFilter = true;
                        m_From.SendGump(new PowerScrollFilterGump(m_From, m_Book));

                        break;
                    }
                case 3: // Clear Filter
                    {
                        f.Clear();
                        m_From.SendGump(new PowerScrollFilterGump(m_From, m_Book));

                        break;
                    }
                default:
                    {
                        index -= 4;

                        int type = index % 4;
                        index /= 4;

                        if (type >= 0 && type < m_Filters.Length)
                        {
                            int[,] filters = m_Filters[type];

                            if (index >= 0 && index < filters.GetLength(0))
                            {
                                if (filters[index, 0] == 0 && filters[index, 1] == 0)
                                    break;

                                switch (type)
                                {
                                    case 0:
                                        f.SkillCat = (SkillCategory)filters[index, 1];
                                        break;
                                    case 1:
                                        switch (filters[index, 1])
                                        {
                                            case 0: f.SkillValue = 0; break;
                                            case 1: f.SkillValue = 105; break;
                                            case 2: f.SkillValue = 110; break;
                                            case 3: f.SkillValue = 115; break;
                                            case 4: f.SkillValue = 120; break;
                                        }
                                        break;
                                }

                                m_From.SendGump(new PowerScrollFilterGump(m_From, m_Book));
                            }
                        }

                        break;
                    }
            }
        }

        public PowerScrollFilterGump(PlayerMobile from, PowerScrollBook book)
            : base(12, 24)
        {
            from.CloseGump(typeof(PowerScrollBookGump));
            from.CloseGump(typeof(PowerScrollFilterGump));

            m_From = from;
            m_Book = book;

            PowerScrollFilter f = (/*from.UseOwnFilter ? from.PowerScrollBookFilter : */book.Filter);

            AddPage(0);

            AddBackground(10, 10, 630, 439, 5054);

            AddImageTiled(18, 20, 613, 420, 2624);
            AddAlphaRegion(18, 20, 613, 420);

            AddImage(5, 5, 10460);
            AddImage(615, 5, 10460);
            AddImage(5, 424, 10460);
            AddImage(615, 424, 10460);

            AddHtmlLocalized(270, 20, 200, 32, 1062223, LabelColor, false, false); // Filter Preference

            //AddHtmlLocalized(26, 35, 120, 32, 1062228, LabelColor, false, false); // Bulk Order Type
            //AddFilterList(25, 61, m_XOffsets_Type, 40, m_TypeFilters, m_XWidths_Small, f.Type, 0);

            //AddHtmlLocalized(320, 35, 50, 32, 1062215, LabelColor, false, false); // Quality
            //AddFilterList(320, 61, m_XOffsets_Quality, 40, m_QualityFilters, m_XWidths_Small, f.Quality, 1);

            //AddHtmlLocalized(26, 100, 120, 32, 1062232, LabelColor, false, false); // Material Type
            AddLabel(26, 100, 1149, @"Skill Category");
            AddFilterList(25, 132, m_XOffsets_SkillCategory, 40, m_SkillFilters, m_XWidths_Large, (int)f.SkillCat, 0);

            //AddHtmlLocalized(26, 350, 120, 32, 1062217, LabelColor, false, false); // Amount
            AddLabel(26, 220, 1149, @"Skill Value");
            int ValueIndex = 0;
            switch (f.SkillValue)
            {
                case 105: ValueIndex = 1; break;
                case 110: ValueIndex = 2; break;
                case 115: ValueIndex = 3; break;
                case 120: ValueIndex = 4; break;
            }
            AddFilterList(25, 242, m_XOffsets_SkillValue, 40, m_ValueFilters, m_XWidths_Small, ValueIndex, 1);

            AddHtmlLocalized(75, 416, 120, 32, 1062477, (from.UseOwnFilter ? LabelColor : 16927), false, false); // Set Book Filter
            AddButton(40, 416, 4005, 4007, 1, GumpButtonType.Reply, 0);

            //AddHtmlLocalized(235, 416, 120, 32, 1062478, (from.UseOwnFilter ? 16927 : LabelColor), false, false); // Set Your Filter
            //AddButton(200, 416, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(405, 416, 120, 32, 1062231, LabelColor, false, false); // Clear Filter
            AddButton(370, 416, 4005, 4007, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(540, 416, 50, 32, 1011046, LabelColor, false, false); // APPLY
            AddButton(505, 416, 4017, 4018, 0, GumpButtonType.Reply, 0);
        }
    }
}
