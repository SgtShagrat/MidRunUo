using System.Collections.Generic;

namespace Midgard.Engines.SpellSystem
{
    public class TopDock : BaseSkin
    {
        public TopDock()
        {
            OptionsButtonY = 7;
            OptionsButtonID = 5001;
            OptionsButtonPressedID = 5001;

            SchoolLabelX = 84;
            SchoolLabelY = 7;
            SchoolLabelHue = 1153;
            SchoolLabelSelectedHue = 7;

            SchoolOffsetX = 85;
            SchoolOffsetY = 0;

            SchoolsAreFixed = true;
            SchoolStarts = new List<SpellStart>();
            SchoolStarts.Add( new SpellStart( 0, 0 ) );
            SchoolStarts.Add( new SpellStart( 85, 0 ) );
            SchoolStarts.Add( new SpellStart( 170, 0 ) );
            SchoolStarts.Add( new SpellStart( 255, 0 ) );
            SchoolStarts.Add( new SpellStart( 340, 0 ) );
            SchoolStarts.Add( new SpellStart( 425, 0 ) );
            SchoolStarts.Add( new SpellStart( 170, 35 ) );
            SchoolStarts.Add( new SpellStart( 255, 35 ) );
            SchoolStarts.Add( new SpellStart( 340, 35 ) );
            SchoolStarts.Add( new SpellStart( 425, 35 ) );
            SchoolStarts.Add( new SpellStart( 170, 70 ) );
            SchoolStarts.Add( new SpellStart( 255, 70 ) );
            SchoolStarts.Add( new SpellStart( 340, 70 ) );
            SchoolStarts.Add( new SpellStart( 425, 70 ) );

            SchoolButtonX = 65;
            SchoolButtonY = 0;
            SchoolButtonID = 83;
            SchoolButtonPressedID = 83;
            SchoolButtonW = 85;
            SchoolButtonH = 35;
            SchoolButtonIsBackground = true;


            SpellOffsetY = 20;

            SpellsPerPage = 8;
            SpellBackgroundX = 0;
            SpellBackgroundY = 35;
            SpellBackgroundID = 83;
            SpellBackgroundW = 265;//235
            SpellBackgroundH = 180;
            SpellBackgroundIsBackground = true;

            SpellPrevButtonX = 215;
            SpellPrevButtonY = 45;
            SpellPrevButtonID = 2435;
            SpellPrevButtonPressedID = 2436;

            SpellNextButtonX = 215;
            SpellNextButtonY = 185;
            SpellNextButtonID = 2437;
            SpellNextButtonPressedID = 2438;

            SpellInfoButtonX = 21;
            SpellInfoButtonY = 46;
            SpellInfoButtonID = 55;
            SpellInfoButtonSelectedID = 56;

            SpellCastButtonX = 41;
            SpellCastButtonY = 50;
            SpellCastButtonID = 10006;
            SpellCastButtonPressedID = 10006;

            SpellLabelX = 59;
            SpellLabelY = 46;
            SpellLabelHue = 1153;

            SpellShowInfo = true;

            SpellInfoBackgroundX = 0;
            SpellInfoBackgroundY = 205;
            SpellInfoBackgroundID = 83;
            SpellInfoBackgroundW = 235;
            SpellInfoBackgroundH = 205;
            SpellInfoBackgroundIsBackground = true;

            SpellInfoIconX = 12;
            SpellInfoIconY = 250;
            SpellInfoBadIconID = 1220;

            SpellInfoNameX = 55;
            SpellInfoNameY = 222;
            SpellInfoNameHue = 1153;

            SpellInfoManaX = 60;
            SpellInfoManaY = 275;
            SpellInfoManaHue = 1153;

            SpellInfoSkillX = 60;
            SpellInfoSkillY = 250;
            SpellInfoSkillHue = 1153;

            SpellInfoReagentsX = 120;
            SpellInfoReagentsY = 240;
            SpellInfoReagentsOffsetY = 15;
            SpellInfoReagentsHue = 1153;

            SpellInfoDescriptionX = 12;
            SpellInfoDescriptionY = 315;
            SpellInfoDescriptionW = 210;
            SpellInfoDescriptionH = 80;
            SpellInfoDescriptionHue = 0xffffff;

            SpellInfoKeywordBackgroundX = 0;
            SpellInfoKeywordBackgroundY = 420;
            SpellInfoKeywordBackgroundID = 83;
            SpellInfoKeywordBackgroundW = 235;
            SpellInfoKeywordBackgroundH = 40;
            SpellInfoKeywordBackgroundIsBackground = true;

            SpellInfoKeywordLabelX = 15;
            SpellInfoKeywordLabelY = 430;
            SpellInfoKeywordLabelHue = 1153;
            SpellInfoKeywordLabel = "Keyword :";

            SpellInfoKeywordTextX = 80;
            SpellInfoKeywordTextY = 430;
            SpellInfoKeywordTextW = 145;
            SpellInfoKeywordTextH = 20;
            SpellInfoKeywordTextHue = 1153;

            SpellInfoKeywordButtonX = 213;
            SpellInfoKeywordButtonY = 432;
            SpellInfoKeywordButtonID = 59;
            SpellInfoKeywordButtonPressedID = 59;
        }
    }
}