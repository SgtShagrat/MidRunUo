using System.Collections.Generic;

namespace Midgard.Engines.SpellSystem
{
    public class BottomDock : BaseSkin
    {
        public BottomDock()
        {
            OptionsShowLabel = true;
            OptionsLabelX = 7;
            OptionsLabelY = 556;
            OptionsLabelHue = 1153;
            OptionsLabelSelectedHue = 7;

            OptionsButtonX = 0;
            OptionsButtonY = 530;
            OptionsButtonID = 83;
            OptionsButtonPressedID = 83;
            OptionsButtonW = 65;
            OptionsButtonH = 70;
            OptionsButtonIsBackground = true;

            SchoolLabelX = 84;
            SchoolLabelY = 537;
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
            SchoolStarts.Add( new SpellStart( 510, 0 ) );
            SchoolStarts.Add( new SpellStart( 0, 35 ) );
            SchoolStarts.Add( new SpellStart( 85, 35 ) );
            SchoolStarts.Add( new SpellStart( 170, 35 ) );
            SchoolStarts.Add( new SpellStart( 255, 35 ) );
            SchoolStarts.Add( new SpellStart( 340, 35 ) );
            SchoolStarts.Add( new SpellStart( 425, 35 ) );
            SchoolStarts.Add( new SpellStart( 510, 35 ) );

            SchoolButtonX = 65;
            SchoolButtonY = 530;
            SchoolButtonID = 83;
            SchoolButtonPressedID = 83;
            SchoolButtonW = 85;
            SchoolButtonH = 35;
            SchoolButtonIsBackground = true;

            SpellOffsetX = 200;
            SpellOffsetY = 20;

            SpellsAreFixed = true;
            SpellStarts = new List<SpellStart>();
            SpellStarts.Add( new SpellStart( 0, 0 ) );
            SpellStarts.Add( new SpellStart( 0, 20 ) );
            SpellStarts.Add( new SpellStart( 0, 40 ) );
            SpellStarts.Add( new SpellStart( 0, 60 ) );
            SpellStarts.Add( new SpellStart( 200, 0 ) );
            SpellStarts.Add( new SpellStart( 200, 20 ) );
            SpellStarts.Add( new SpellStart( 200, 40 ) );
            SpellStarts.Add( new SpellStart( 200, 60 ) );

            SpellsPerPage = 8;

            SpellBackgroundX = 0;
            SpellBackgroundY = 400;
            SpellBackgroundID = 83;
            SpellBackgroundW = 435;//405
            SpellBackgroundH = 130;
            SpellBackgroundIsBackground = true;

            SpellPrevButtonX = 25;
            SpellPrevButtonY = 500;
            SpellPrevButtonID = 59;
            SpellPrevButtonPressedID = 59;

            SpellNextButtonX = 350;
            SpellNextButtonY = 500;
            SpellNextButtonID = 57;
            SpellNextButtonPressedID = 57;

            SpellInfoButtonX = 15;
            SpellInfoButtonY = 415;
            SpellInfoButtonID = 55;
            SpellInfoButtonSelectedID = 56;

            SpellCastButtonX = 35;
            SpellCastButtonY = 419;
            SpellCastButtonID = 10006;
            SpellCastButtonPressedID = 10006;

            SpellLabelX = 53;
            SpellLabelY = 415;
            SpellLabelHue = 1153;

            SpellShowInfo = true;

            SpellInfoIconX = 660;
            SpellInfoIconY = 445;
            SpellInfoBadIconID = 1220;

            SpellInfoNameX = 665;
            SpellInfoNameY = 410;
            SpellInfoNameHue = 1153;

            SpellInfoManaX = 710;
            SpellInfoManaY = 468;
            SpellInfoManaHue = 1153;

            SpellInfoSkillX = 710;
            SpellInfoSkillY = 448;
            SpellInfoSkillHue = 1153;

            SpellInfoReagentsX = 675;
            SpellInfoReagentsY = 515;
            SpellInfoReagentsOffsetY = 15;
            SpellInfoReagentsHue = 1153;

            SpellInfoDescriptionX = 420;
            SpellInfoDescriptionY = 415;
            SpellInfoDescriptionW = 235;
            SpellInfoDescriptionH = 72;
            SpellInfoDescriptionHue = 0xffffff;

            SpellInfoKeywordBackgroundX = 490;
            SpellInfoKeywordBackgroundY = 492;
            SpellInfoKeywordBackgroundID = 1141;

            SpellInfoKeywordLabelX = 420;
            SpellInfoKeywordLabelY = 495;
            SpellInfoKeywordLabelHue = 1153;
            SpellInfoKeywordLabel = "Keyword :";

            SpellInfoKeywordTextX = 500;
            SpellInfoKeywordTextY = 495;
            SpellInfoKeywordTextW = 255;
            SpellInfoKeywordTextH = 20;
            SpellInfoKeywordTextHue = 1153;

            SpellInfoKeywordButtonX = 770;
            SpellInfoKeywordButtonY = 500;
            SpellInfoKeywordButtonID = 10006;
            SpellInfoKeywordButtonPressedID = 2361;
        }

        public override void CreateInfoBackground()
        {
            SpellbookGump.AddImage( 644, 514, 91 );	//Corner
            SpellbookGump.AddImage( 660, 514, 89 );	//Corner
            SpellbookGump.AddImage( 660, 530, 83 );	//Corner
            SpellbookGump.AddImageTiled( 660, 546, 16, 38, 86 );	//Bottom Left Side
            SpellbookGump.AddImageTiled( 421, 514, 223, 16, 90 );	//Middle Bottom
            SpellbookGump.AddImageTiled( 421, 416, 364, 104, 87 );	//Top Middle
            SpellbookGump.AddImageTiled( 667, 520, 118, 64, 87 );	//Right Middle
            SpellbookGump.AddImage( 405, 400, 83 );				//Top Left
            SpellbookGump.AddImage( 405, 514, 89 );				//Top Right
            SpellbookGump.AddImage( 784, 400, 85 );				//Mid Left
            SpellbookGump.AddImage( 784, 584, 91 );				//Bottom Right
            SpellbookGump.AddImage( 660, 584, 89 );				//Bottom Left
            SpellbookGump.AddImageTiled( 421, 400, 364, 16, 84 );	//Top
            SpellbookGump.AddImageTiled( 405, 416, 16, 98, 86 );	//Left
            SpellbookGump.AddImageTiled( 784, 416, 16, 168, 88 );	//Right
            SpellbookGump.AddImageTiled( 676, 584, 108, 16, 90 );	//Bottom

            // TODO : alpharegion 
            //SpellbookGump.AddBlackAlphaRegion( SpellInfoBackgroundX, SpellInfoBackgroundY, SpellInfoBackgroundW,
            //    SpellInfoBackgroundH );
        }
    }
}