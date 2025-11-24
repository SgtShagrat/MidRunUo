using Server.Gumps;

namespace Midgard.Engines.SpellSystem
{
    public class RightDock : BaseSkin
    {
        public RightDock()
        {
            OptionsButtonX = 725;
            OptionsButtonY = 0;
            OptionsButtonID = 5001;
            OptionsButtonPressedID = 5001;

            SchoolLabelX = 732;
            SchoolLabelY = 20;
            SchoolLabelHue = 1153;
            SchoolLabelSelectedHue = 7;

            SchoolOffsetX = 0;
            SchoolOffsetY = 35;

            SchoolButtonX = 715;
            SchoolButtonY = 13;
            SchoolButtonID = 83;
            SchoolButtonPressedID = 83;
            SchoolButtonW = 85;
            SchoolButtonH = 35;
            SchoolButtonIsBackground = true;

            SchoolShowIndicator = true;
            SchoolIndicatorX = 699;
            SchoolIndicatorY = 23;
            SchoolIndicatorID = 59;
            SchoolIndicatorSelectedID = 57;

            SpellOffsetX = 0;
            SpellOffsetY = 20;
            SpellsPerPage = 16;

            SpellBackgroundX = 515;
            SpellBackgroundY = 13;
            SpellBackgroundID = 83;
            SpellBackgroundIsBackground = true;
            SpellBackgroundW = 230;//200
            SpellBackgroundH = 350;

            SpellLabelX = 575;
            SpellLabelY = 30;
            SpellLabelHue = 1153;

            SpellCastButtonX = 555;
            SpellCastButtonY = 34;
            SpellCastButtonID = 10006;
            SpellCastButtonPressedID = 2361;

            SpellInfoButtonX = 535;
            SpellInfoButtonY = 30;
            SpellInfoButtonID = 55;
            SpellInfoButtonSelectedID = 56;

            SpellNextButtonX = 680;
            SpellNextButtonY = 355;
            SpellNextButtonID = 57;
            SpellNextButtonPressedID = 57;

            SpellNextShowLabel = false;
            SpellNextLabelX = 645;
            SpellNextLabelY = 353;
            SpellNextLabelHue = 1153;

            SpellPrevButtonX = 515;
            SpellPrevButtonY = 355;
            SpellPrevButtonID = 59;
            SpellPrevButtonPressedID = 59;

            SpellPrevShowLabel = false;
            SpellPrevLabelX = 550;
            SpellPrevLabelY = 353;
            SpellPrevLabelHue = 1153;

            SpellShowInfo = true;

            SpellInfoBackgroundX = 515;
            SpellInfoBackgroundY = 363;
            SpellInfoBackgroundID = 83;
            SpellInfoBackgroundIsBackground = true;
            SpellInfoBackgroundW = 285;
            SpellInfoBackgroundH = 140;

            SpellInfoIconX = 525;
            SpellInfoIconY = 374;
            SpellInfoBadIconID = 1220;

            SpellInfoNameX = 640;
            SpellInfoNameY = 371;
            SpellInfoNameHue = 1153;
            SpellInfoManaX = 570;
            SpellInfoManaY = 398;
            SpellInfoManaHue = 1153;
            SpellInfoSkillX = 570;
            SpellInfoSkillY = 378;
            SpellInfoSkillHue = 1153;

            SpellInfoReagentsX = 525;
            SpellInfoReagentsY = 417;
            SpellInfoReagentsOffsetX = 0;
            SpellInfoReagentsOffsetY = 15;
            SpellInfoReagentsHue = 1153;

            SpellInfoDescriptionX = 640;
            SpellInfoDescriptionY = 390;
            SpellInfoDescriptionW = 150;
            SpellInfoDescriptionH = 100;
            SpellInfoDescriptionShowBackground = false;
            SpellInfoDescriptionHue = 0xffffff;

            SpellInfoKeywordBackgroundX = 515;
            SpellInfoKeywordBackgroundY = 503;
            SpellInfoKeywordBackgroundID = 83;
            SpellInfoKeywordBackgroundIsBackground = true;
            SpellInfoKeywordBackgroundW = 285;
            SpellInfoKeywordBackgroundH = 40;

            SpellInfoKeywordLabelX = 527;
            SpellInfoKeywordLabelY = 513;
            SpellInfoKeywordLabelHue = 1153;

            SpellInfoKeywordTextX = 590;
            SpellInfoKeywordTextY = 513;
            SpellInfoKeywordTextW = 185;
            SpellInfoKeywordTextH = 20;
            SpellInfoKeywordTextHue = 68;

            SpellInfoKeywordButtonX = 780;
            SpellInfoKeywordButtonY = 517;
            SpellInfoKeywordButtonID = 10006;
            SpellInfoKeywordButtonPressedID = 2361;
        }

        public override void CreateSchoolButtons()
        {
            int csIndex = SchoolInfo.GetIndex( Book.MainBookSchool );

            SchoolInfo si = Book.MainBookSchool;

            int currentX = ( SchoolsAreFixed && SchoolStarts != null ? SchoolStarts[ 0 ].X : 0 * SchoolOffsetX );
            int currentY = ( SchoolsAreFixed && SchoolStarts != null ? SchoolStarts[ 0 ].Y : 0 * SchoolOffsetY );

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
                SpellbookGump.AddButton( currentX + SchoolButtonX, currentY + SchoolButtonY, SchoolButtonID, SchoolButtonPressedID, csIndex + 100,
                               GumpButtonType.Reply, 0 );

            if( SchoolShowIndicator )
                SpellbookGump.AddImage( currentX + SchoolIndicatorX, currentY + SchoolIndicatorY, SchoolIndicatorID );

            if( CustomSpellbookGump.OldStyle )
                SpellbookGump.AddOldHtmlHued( currentX + SchoolLabelX, currentY + SchoolLabelY, 200, 17, si.Name, CustomSpellbookGump.SchoolNameHue );
            else
                SpellbookGump.AddLabel( currentX + SchoolLabelX, currentY + SchoolLabelY, SchoolLabelHue, si.Name );
        }
    }
}