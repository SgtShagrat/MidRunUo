using Server.Gumps;

namespace Midgard.Engines.SpellSystem
{
    public class LeftDock : BaseSkin
    {
        public LeftDock()
        {
            OptionsButtonX = 8;
            OptionsButtonY = 0;
            OptionsButtonID = 5001;
            OptionsButtonPressedID = 5001;

            SchoolLabelX = 12;
            SchoolLabelY = 20;
            SchoolLabelHue = 1153;
            SchoolLabelSelectedHue = 7;

            SchoolOffsetX = 0;
            SchoolOffsetY = 35;

            SchoolButtonX = 0;
            SchoolButtonY = 13;
            SchoolButtonID = 83;
            SchoolButtonPressedID = 83;
            SchoolButtonW = 85;
            SchoolButtonH = 35;
            SchoolButtonIsBackground = true;

            SchoolShowIndicator = true;
            SchoolIndicatorX = 68;
            SchoolIndicatorY = 22;
            SchoolIndicatorID = 57;
            SchoolIndicatorSelectedID = 59;

            SpellLabelX = 142;
            SpellLabelY = 29;
            SpellLabelHue = 1153;

            SpellOffsetX = 0;
            SpellOffsetY = 20;

            SpellCastButtonX = 124;
            SpellCastButtonY = 33;
            SpellCastButtonID = 10006;
            SpellCastButtonPressedID = 2361;

            SpellInfoButtonX = 104;
            SpellInfoButtonY = 29;
            SpellInfoButtonID = 55;
            SpellInfoButtonSelectedID = 56;

            SpellsPerPage = 16;
            SpellBackgroundX = 85;
            SpellBackgroundY = 13;
            SpellBackgroundID = 83;
            SpellBackgroundIsBackground = true;
            SpellBackgroundW = 230;//200
            SpellBackgroundH = 350;

            SpellNextButtonX = 250;
            SpellNextButtonY = 355;
            SpellNextButtonID = 57;
            SpellNextButtonPressedID = 57;

            SpellNextShowLabel = false;
            SpellNextLabelX = 219;
            SpellNextLabelY = 353;
            SpellNextLabelHue = 1153;

            SpellPrevButtonX = 85;
            SpellPrevButtonY = 355;
            SpellPrevButtonID = 59;
            SpellPrevButtonPressedID = 59;

            SpellPrevShowLabel = false;
            SpellPrevLabelX = 116;
            SpellPrevLabelY = 353;
            SpellPrevLabelHue = 1153;

            SpellShowInfo = true;

            SpellInfoBackgroundX = 0;
            SpellInfoBackgroundY = 363;
            SpellInfoBackgroundID = 83;
            SpellInfoBackgroundIsBackground = true;
            SpellInfoBackgroundW = 285;
            SpellInfoBackgroundH = 140;

            SpellInfoIconX = 10;
            SpellInfoIconY = 374;
            SpellInfoBadIconID = 1220;

            SpellInfoNameX = 120;
            SpellInfoNameY = 371;
            SpellInfoNameHue = 1153;
            SpellInfoManaX = 55;
            SpellInfoManaY = 398;
            SpellInfoManaHue = 1153;
            SpellInfoSkillX = 55;
            SpellInfoSkillY = 378;
            SpellInfoSkillHue = 1153;

            SpellInfoReagentsX = 10;
            SpellInfoReagentsY = 417;
            SpellInfoReagentsOffsetX = 0;
            SpellInfoReagentsOffsetY = 15;
            SpellInfoReagentsHue = 1153;

            SpellInfoDescriptionX = 120;
            SpellInfoDescriptionY = 390;
            SpellInfoDescriptionW = 150;
            SpellInfoDescriptionH = 100;
            SpellInfoDescriptionShowBackground = false;
            SpellInfoDescriptionHue = 0xffffff;

            SpellInfoKeywordBackgroundX = 0;
            SpellInfoKeywordBackgroundY = 503;
            SpellInfoKeywordBackgroundID = 83;
            SpellInfoKeywordBackgroundIsBackground = true;
            SpellInfoKeywordBackgroundW = 285;
            SpellInfoKeywordBackgroundH = 40;

            SpellInfoKeywordLabelX = 12;
            SpellInfoKeywordLabelY = 513;
            SpellInfoKeywordLabelHue = 1153;

            SpellInfoKeywordTextX = 75;
            SpellInfoKeywordTextY = 513;
            SpellInfoKeywordTextW = 185;
            SpellInfoKeywordTextH = 20;
            SpellInfoKeywordTextHue = 68;

            SpellInfoKeywordButtonX = 264;
            SpellInfoKeywordButtonY = 517;
            SpellInfoKeywordButtonID = 10006;
            SpellInfoKeywordButtonPressedID = 2361;
        }

        public override void CreateSchoolButtons()
        {
            int csIndex = SchoolInfo.GetIndex( Book.MainBookSchool );

            SchoolInfo si = Book.MainBookSchool;

            int currentX = ( SchoolsAreFixed && SchoolStarts != null ? SchoolStarts[ 0 ].X : SchoolOffsetX );
            int currentY = ( SchoolsAreFixed && SchoolStarts != null ? SchoolStarts[ 0 ].Y : SchoolOffsetY );

            if( SchoolButtonIsBackground )
                AddBackgroundButton( currentX + SchoolButtonX, currentY + SchoolButtonY, SchoolButtonW, SchoolButtonH, SchoolButtonID, SchoolButtonPressedID, csIndex + 100 );
            else if( SchoolButtonIsMulti )
            {
                if( SchoolButtonMultis != null )
                {
                    for( int i = 0; i < SchoolButtonMultis.Count; i++ )
                    {
                        GumpMulti gm = SchoolButtonMultis[ i ];
                        if( gm != null )
                            SpellbookGump.AddButton( currentX + gm.X, currentY + gm.Y, gm.ID, gm.IDPressed, csIndex + 100, GumpButtonType.Reply, 0 );
                    }
                }
            }
            else
            {
                SpellbookGump.AddButton( currentX + SchoolButtonX, currentY + SchoolButtonY, SchoolButtonID, SchoolButtonPressedID, csIndex + 100, GumpButtonType.Reply, 0 );
            }

            if( SchoolShowIndicator )
                SpellbookGump.AddImage( currentX + SchoolIndicatorX, currentY + SchoolIndicatorY, SchoolIndicatorID );

            if( CustomSpellbookGump.OldStyle )
                SpellbookGump.AddOldHtmlHued( currentX + SchoolLabelX, currentY + SchoolLabelY, 200, 17, si.Name, CustomSpellbookGump.SchoolNameHue );
            else
                SpellbookGump.AddLabel( currentX + SchoolLabelX, currentY + SchoolLabelY, SchoolLabelHue, si.Name );
        }
    }
}