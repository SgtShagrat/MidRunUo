using System;
using System.Collections.Generic;
using Server;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class BaseSkin
    {
        public bool OptionsShowLabel = false;
        public int OptionsLabelX = 0;
        public int OptionsLabelY = 0;
        public int OptionsLabelHue = 0;
        public int OptionsLabelSelectedHue = 0;
        public string OptionsLabel = "Options";

        public int OptionsButtonX = 0;
        public int OptionsButtonY = 0;
        public int OptionsButtonID = 0;
        public int OptionsButtonPressedID = 0;
        public int OptionsButtonW = 0;
        public int OptionsButtonH = 0;
        public bool OptionsButtonIsBackground = false;
        public bool OptionsButtonIsMulti = false;
        public List<GumpMulti> OptionsButtonMultis;

        public bool OptionsShowIndicator = false;
        public int OptionsIndicatorID = 0;
        public int OptionsIndicatorSelectedID = 0;

        public int SchoolLabelX = 0;
        public int SchoolLabelY = 0;
        public int SchoolLabelHue = 0;
        public int SchoolLabelSelectedHue = -1;

        public int SchoolOffsetX = 0;
        public int SchoolOffsetY = 0;

        public bool SchoolsAreFixed = false;
        public List<SpellStart> SchoolStarts;

        public int SchoolButtonX = 0;
        public int SchoolButtonY = 0;
        public int SchoolButtonID = 0;
        public int SchoolButtonPressedID = 0;
        public int SchoolButtonW = 0;
        public int SchoolButtonH = 0;
        public bool SchoolButtonIsBackground = false;
        public bool SchoolButtonIsMulti = false;
        public List<GumpMulti> SchoolButtonMultis;

        public bool SchoolShowIndicator = false;
        public int SchoolIndicatorX = 0;
        public int SchoolIndicatorY = 0;
        public int SchoolIndicatorID = 0;
        public int SchoolIndicatorSelectedID = 0;

        public int SpellOffsetX = 0;
        public int SpellOffsetY = 0;

        public bool SpellsAreFixed = false;
        public List<SpellStart> SpellStarts;

        public int SpellsPerPage = 0;
        public int SpellBackgroundX = 0;
        public int SpellBackgroundY = 0;
        public int SpellBackgroundID = 0;
        public int SpellBackgroundW = 0;
        public int SpellBackgroundH = 0;
        public bool SpellBackgroundIsBackground = true;
        public bool SpellBackgroundIsTiled = false;
        public bool SpellBackgroundIsMulti = false;
        public List<GumpMulti> SpellBackgroundMultis;

        public int SpellPrevButtonX = 0;
        public int SpellPrevButtonY = 0;
        public int SpellPrevButtonID = 0;
        public int SpellPrevButtonPressedID = 0;

        public bool SpellPrevShowLabel = false;
        public int SpellPrevLabelX = 0;
        public int SpellPrevLabelY = 0;
        public int SpellPrevLabelHue = 0;
        public string SpellPrevLabel = "Prev";

        public int SpellNextButtonX = 0;
        public int SpellNextButtonY = 0;
        public int SpellNextButtonID = 0;
        public int SpellNextButtonPressedID = 0;

        public bool SpellNextShowLabel = false;
        public int SpellNextLabelX = 0;
        public int SpellNextLabelY = 0;
        public int SpellNextLabelHue = 0;
        public string SpellNextLabel = "Next";

        public int SpellInfoButtonX = 0;
        public int SpellInfoButtonY = 0;
        public int SpellInfoButtonID = 0;
        public int SpellInfoButtonSelectedID = 0;

        public int SpellCastButtonX = 0;
        public int SpellCastButtonY = 0;
        public int SpellCastButtonID = 0;
        public int SpellCastButtonPressedID = 0;

        public int SpellLabelX = 0;
        public int SpellLabelY = 0;
        public int SpellLabelHue = 0;

        public bool SpellShowInfo = false;

        public int SpellInfoBackgroundX = 0;
        public int SpellInfoBackgroundY = 0;
        public int SpellInfoBackgroundID = 0;
        public int SpellInfoBackgroundW = 0;
        public int SpellInfoBackgroundH = 0;
        public bool SpellInfoBackgroundIsBackground = true;
        public bool SpellInfoBackgroundIsTiled = false;
        public bool SpellInfoBackgroundIsMulti = false;
        public List<GumpMulti> SpellInfoBackgroundMultis;

        public int SpellInfoIconX = 0;
        public int SpellInfoIconY = 0;
        public int SpellInfoBadIconID = 1220;

        public bool SpellInfoShowName = true;
        public int SpellInfoNameX = 0;
        public int SpellInfoNameY = 0;
        public int SpellInfoNameHue = 0;

        public bool SpellInfoShowMana = true;
        public int SpellInfoManaX = 0;
        public int SpellInfoManaY = 0;
        public int SpellInfoManaHue = 0;
        public string SpellInfoManaLabel = "M: ";

        public bool SpellInfoShowSkill = true;
        public int SpellInfoSkillX = 0;
        public int SpellInfoSkillY = 0;
        public int SpellInfoSkillHue = 0;
        public string SpellInfoSkillLabel = "S: ";

        public bool SpellInfoShowReagents = true;
        public int SpellInfoReagentsX = 0;
        public int SpellInfoReagentsY = 0;
        public int SpellInfoReagentsOffsetX = 0;
        public int SpellInfoReagentsOffsetY = 0;
        public int SpellInfoReagentsHue = 0;

        public bool SpellInfoShowDescription = true;
        public int SpellInfoDescriptionX = 0;
        public int SpellInfoDescriptionY = 0;
        public int SpellInfoDescriptionW = 0;
        public int SpellInfoDescriptionH = 0;
        public bool SpellInfoDescriptionShowBackground = false;
        public int SpellInfoDescriptionHue = 0;

        public int SpellInfoKeywordBackgroundX = 0;
        public int SpellInfoKeywordBackgroundY = 0;
        public int SpellInfoKeywordBackgroundID = 0;
        public int SpellInfoKeywordBackgroundW = 0;
        public int SpellInfoKeywordBackgroundH = 0;
        public bool SpellInfoKeywordBackgroundIsBackground = false;
        public bool SpellInfoKeywordBackgroundIsTiled = false;
        public bool SpellInfoKeywordBackgroundIsMulti = false;
        public List<GumpMulti> SpellInfoKeywordBackgroundMultis;

        public int SpellInfoKeywordLabelX = 0;
        public int SpellInfoKeywordLabelY = 0;
        public int SpellInfoKeywordLabelHue = 0;
        public string SpellInfoKeywordLabel = "Keyword :";

        public int SpellInfoKeywordTextX = 0;
        public int SpellInfoKeywordTextY = 0;
        public int SpellInfoKeywordTextW = 0;
        public int SpellInfoKeywordTextH = 0;
        public int SpellInfoKeywordTextHue = 0;
        public string SpellInfoKeywordText = "";

        public int SpellInfoKeywordButtonX = 0;
        public int SpellInfoKeywordButtonY = 0;
        public int SpellInfoKeywordButtonID = 0;
        public int SpellInfoKeywordButtonPressedID = 0;

        public CustomSpellbook Book { get; private set; }

        public CustomSpellbookGump SpellbookGump { get; private set; }

        public virtual void CreateGump( CustomSpellbook book, CustomSpellbookGump gump )
        {
            Book = book;
            SpellbookGump = gump;

            CreateOptionsButton();
            CreateSchoolButtons();

            if( Book.CurrentSchool == null )
            {
                Book.CurrentSchool = SchoolInfo.SchoolList[ 0 ];
                Console.WriteLine( "Warning: BaseSkin with null CurrentSchool." );
            }

            int index = SchoolInfo.GetIndex( Book.CurrentSchool );

            if( index == 1 )
            {
                SpellbookGump.OptionSkin.ShowOptions( Book, SpellbookGump );
            }
            else if( index > 1 )
            {
                ShowSpells();

                if( Book.CurrentSpell != -1 && SpellShowInfo )
                    ShowSpellInfo();
            }
        }

        public virtual void CreateOptionsButton()
        {
            if( OptionsButtonIsBackground )
            {
                AddBackgroundButton( OptionsButtonX, OptionsButtonY, OptionsButtonW, OptionsButtonH, OptionsButtonID,
                                    OptionsButtonPressedID, (int)CustomSpellbookGump.Buttons.OptionBtn );
            }
            else if( OptionsButtonIsMulti )
            {
                if( OptionsButtonMultis != null )
                    foreach( GumpMulti gm in OptionsButtonMultis )
                    {
                        if( gm != null )
                            SpellbookGump.AddButton( gm.X, gm.Y, gm.ID, gm.IDPressed, (int)CustomSpellbookGump.Buttons.OptionBtn, GumpButtonType.Reply, 0 );
                    }
            }
            else
            {
                SpellbookGump.AddButton( OptionsButtonX, OptionsButtonY, OptionsButtonID, OptionsButtonPressedID, (int)CustomSpellbookGump.Buttons.OptionBtn, GumpButtonType.Reply, 0 );
            }

            if( OptionsShowLabel )
            {
                if( CustomSpellbookGump.OldStyle )
                    SpellbookGump.AddOldHtmlHued( OptionsLabelX, OptionsLabelY, 200, 17, OptionsLabel, CustomSpellbookGump.OptionsShowLabelHue );
                else
                    SpellbookGump.AddLabel( OptionsLabelX, OptionsLabelY, OptionsLabelHue, OptionsLabel );
            }
        }

        public virtual void CreateSchoolButtons()
        {
            int currentIndex = 0;

            SchoolInfo si = Book.MainBookSchool;

            // first two schools are reserved (none and options)
            int csIndex = SchoolInfo.GetIndex( Book.MainBookSchool );
            if( csIndex == -1 )
                return;

            int currentX = ( SchoolsAreFixed && SchoolStarts != null ? SchoolStarts[ currentIndex ].X : currentIndex * SchoolOffsetX );
            int currentY = ( SchoolsAreFixed && SchoolStarts != null ? SchoolStarts[ currentIndex ].Y : currentIndex * SchoolOffsetY );

            if( SchoolButtonIsBackground )
                AddBackgroundButton( currentX + SchoolButtonX, currentY + SchoolButtonY, SchoolButtonW, SchoolButtonH, SchoolButtonID, SchoolButtonPressedID, csIndex + 100 );
            else if( SchoolButtonIsMulti )
            {
                if( SchoolButtonMultis != null )
                {
                    foreach( GumpMulti gm in SchoolButtonMultis )
                    {
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

        public virtual void ShowSpells()
        {
            if( Book.CurrentSchool == null )
            {
                Console.WriteLine( "Warning: Book with null CurrentSchool in BaseSkin.ShowSpells ." );
                return;
            }

            if( Book.SpellsOffset < SpellsPerPage )
                Book.SpellsOffset = SpellsPerPage;

            int[] ranges = Book.CurrentSchool.Range;
            bool pages = ( ranges.Length > SpellsPerPage );

            if( Book.SpellsOffset >= ranges.Length + SpellsPerPage && pages )
                Book.SpellsOffset = ( ( ranges.Length / SpellsPerPage ) * SpellsPerPage );

            CreateSpellBackground();

            #region SpellAdvanceButtons
            if( Book.SpellsOffset > SpellsPerPage && pages )
            {
                SpellbookGump.AddButton( SpellPrevButtonX, SpellPrevButtonY, SpellPrevButtonID, SpellPrevButtonPressedID,
                    (int)CustomSpellbookGump.Buttons.SpellsPrev, GumpButtonType.Reply, 0 );

                if( SpellPrevShowLabel )
                {
                    //if( CustomSpellbookGump.OldStyle )
                    //    SpellbookGump.AddOldHtmlHued( SpellPrevLabelX, SpellPrevLabelY, 200, 17, SpellPrevLabel, SpellPrevLabelHue );
                    //else
                    SpellbookGump.AddLabel( SpellPrevLabelX, SpellPrevLabelY, SpellPrevLabelHue, SpellPrevLabel );
                }
            }

            if( ranges.Length - Book.SpellsOffset > 0 && pages )
            {
                SpellbookGump.AddButton( SpellNextButtonX, SpellNextButtonY, SpellNextButtonID, SpellNextButtonPressedID, (int)CustomSpellbookGump.Buttons.SpellsNext, GumpButtonType.Reply, 0 );
                if( SpellNextShowLabel )
                {
                    if( CustomSpellbookGump.OldStyle )
                        SpellbookGump.AddOldHtmlHued( SpellNextLabelX, SpellNextLabelY, 200, 17, SpellNextLabel, SpellNextLabelHue );
                    else
                        SpellbookGump.AddLabel( SpellNextLabelX, SpellNextLabelY, SpellNextLabelHue, SpellNextLabel );
                }
            }
            #endregion

            CreateSpellsOnPage( pages, ranges );
        }

        public virtual void CreateSpellBackground()
        {
            if( SpellBackgroundIsBackground )
            {
                SpellbookGump.AddBackground( SpellBackgroundX, SpellBackgroundY, SpellBackgroundW, SpellBackgroundH, SpellBackgroundID );
                SpellbookGump.AddBlackAlphaRegion( SpellBackgroundX, SpellBackgroundY, SpellBackgroundW, SpellBackgroundH );
            }
            else if( SpellBackgroundIsTiled )
                SpellbookGump.AddImageTiled( SpellBackgroundX, SpellBackgroundY, SpellBackgroundW, SpellBackgroundH, SpellBackgroundID );
            else if( SpellBackgroundIsMulti )
            {
                if( SpellBackgroundMultis != null )
                {
                    for( int i = 0; i < SchoolButtonMultis.Count; i++ )
                    {
                        GumpMulti gm = SpellBackgroundMultis[ i ];
                        if( gm != null )
                            SpellbookGump.AddImage( gm.X, gm.Y, gm.ID );
                    }
                }
            }
            else
                SpellbookGump.AddImage( SpellBackgroundX, SpellBackgroundY, SpellBackgroundID );
        }

        public virtual void CreateSpellsOnPage( bool pages, int[] ranges )
        {
            for( int i = 0; i < SpellsPerPage; i++ )
            {
                int index = ( i + ( pages ? Book.SpellsOffset : SpellsPerPage ) - SpellsPerPage );
                if( index >= ranges.Length || index < 0 )
                    continue;

                int currentX = ( SpellsAreFixed && SpellStarts != null ? SpellStarts[ i ].X : i * SpellOffsetX );
                int currentY = ( SpellsAreFixed && SpellStarts != null ? SpellStarts[ i ].Y : i * SpellOffsetY );

                int spellID = ranges[ index ];

                if( spellID != -1 && ( ( Book.ContainsSpell( spellID ) || !Book.CurrentSchool.ReqScrolls ) ) )
                {
                    bool enabled = RPGSpellsSystem.CanSpellBeCastBy( Book.Owner, spellID, false );
                    int hue = RPGSpellsSystem.GetSpellLabelHueBySpellID( Book.Owner, spellID );
			string level = " ("+RPGSpellsSystem.GetSpellLevelBySpellID( Book.Owner, spellID ).ToString()+")";
                    ExtendedSpellInfo si = SpellRegistry.GetExtendedSpellInfoByID( spellID );
                    if( si == null )
                        continue;

                    if( Book.CurrentSchool.School != si.School )
                        continue;

                    if( SpellShowInfo )
                    {
                        if( Book.CurrentSpell == spellID )
                            SpellbookGump.AddButton( currentX + SpellInfoButtonX, currentY + SpellInfoButtonY, SpellInfoButtonSelectedID, SpellInfoButtonID, 2000 + spellID, GumpButtonType.Reply, 0 );
                        else
                            SpellbookGump.AddButton( currentX + SpellInfoButtonX, currentY + SpellInfoButtonY, SpellInfoButtonID, SpellInfoButtonSelectedID, 2000 + spellID, GumpButtonType.Reply, 0 );
                    }

                    if( SpellRegistry.GetTypeFromRegNumber( spellID ) == null && !SpellRegistry.SpecialMoves.ContainsKey( spellID ) )
                        continue;

                    if( enabled )
                        SpellbookGump.AddButton( currentX + SpellCastButtonX, currentY + SpellCastButtonY, SpellCastButtonID, SpellCastButtonPressedID, 1000 + spellID, GumpButtonType.Reply, 0 );

                    if( CustomSpellbookGump.OldStyle )
                        SpellbookGump.AddOldHtmlHued( currentX + SpellLabelX, currentY + SpellLabelY, 200, 17, si.Name + level, hue );
                    else
                        SpellbookGump.AddLabel( currentX + SpellLabelX, currentY + SpellLabelY, SpellLabelHue, si.Name + level );
                }
            }
        }

        public virtual void ShowSpellInfo()
        {
            ExtendedSpellInfo si = SpellRegistry.GetExtendedSpellInfoByID( Book.CurrentSpell );
            if( si == null )
                return;

            if( Book.CurrentSchool.School != si.School )
                return;

            CreateInfoBackground();

            #region Info
            if( si.SpellIcon != -1 )
                SpellbookGump.AddButton( SpellInfoIconX, SpellInfoIconY, si.SpellIcon, si.SpellIcon, Book.CurrentSpell + 3000, GumpButtonType.Reply, 0 );
            else
                SpellbookGump.AddImage( SpellInfoIconX, SpellInfoIconY, SpellInfoBadIconID );

            if( SpellInfoShowName )
            {
                if( CustomSpellbookGump.OldStyle )
                    SpellbookGump.AddOldHtmlHued( SpellInfoNameX, SpellInfoNameY, 200, 17, si.Name, CustomSpellbookGump.SpellInfoShowNameHue );
                else
                    SpellbookGump.AddLabel( SpellInfoNameX, SpellInfoNameY, SpellInfoNameHue, si.Name );
            }

            if( SpellInfoShowMana )
            {
                if( CustomSpellbookGump.OldStyle )
                    SpellbookGump.AddOldHtmlHued( SpellInfoManaX, SpellInfoManaY, 200, 17, SpellInfoManaLabel + si.Mana, CustomSpellbookGump.SpellInfoShowManaHue );
                else
                    SpellbookGump.AddLabel( SpellInfoManaX, SpellInfoManaY, SpellInfoManaHue, SpellInfoManaLabel + si.Mana );
            }

            if( SpellInfoShowSkill )
            {
                if( CustomSpellbookGump.OldStyle )
                    SpellbookGump.AddOldHtmlHued( SpellInfoSkillX, SpellInfoSkillY, 200, 17, SpellInfoSkillLabel + si.Skill, CustomSpellbookGump.SpellInfoShowSkillHue );
                else
                    SpellbookGump.AddLabel( SpellInfoSkillX, SpellInfoSkillY, SpellInfoSkillHue, SpellInfoSkillLabel + si.Skill );
            }

            if( SpellInfoShowReagents )
            {
                List<string> regs = Reagent.ConvertRegs( si.Reagents );

                if( regs != null )
                {
                    for( int i = 0; i < regs.Count; i++ )
                    {
                        if( CustomSpellbookGump.OldStyle )
                            SpellbookGump.AddOldHtmlHued( SpellInfoReagentsX + SpellInfoReagentsOffsetX * i,
                                SpellInfoReagentsY + SpellInfoReagentsOffsetY * i, 200, 17, regs[ i ], CustomSpellbookGump.SpellInfoShowReagentsHue );
                        else
                        {
                            SpellbookGump.AddLabel( SpellInfoReagentsX + SpellInfoReagentsOffsetX * i,
                                SpellInfoReagentsY + SpellInfoReagentsOffsetY * i, SpellInfoReagentsHue, regs[ i ] );
                        }
                    }
                }
            }

            if( SpellInfoShowDescription )
            {
                string description;
                if( !string.IsNullOrEmpty( si.DescriptionIta ) && Book.Owner != null && Book.Owner.TrueLanguage == LanguageType.Ita )
                    description = si.DescriptionIta;
                else
                    description = si.Description;

                if( CustomSpellbookGump.OldStyle )
                    SpellbookGump.AddOldScrollableHtmlHued( SpellInfoDescriptionX, SpellInfoDescriptionY, SpellInfoDescriptionW, SpellInfoDescriptionH,
                        description, CustomSpellbookGump.SpellInfoShowDescriptionHue );
                else
                    TextDefinition.AddHtmlText( SpellbookGump, SpellInfoDescriptionX, SpellInfoDescriptionY, SpellInfoDescriptionW, SpellInfoDescriptionH,
                        description, SpellInfoDescriptionShowBackground, true, BaseQuestGump.C32216( SpellInfoDescriptionHue ), SpellInfoDescriptionHue );
            }
            #endregion

            CreateKeyword( si );
        }

        public virtual void CreateInfoBackground()
        {
            if( SpellInfoBackgroundIsBackground )
            {
                SpellbookGump.AddBackground( SpellInfoBackgroundX, SpellInfoBackgroundY, SpellInfoBackgroundW,
                                     SpellInfoBackgroundH, SpellInfoBackgroundID );

                SpellbookGump.AddBlackAlphaRegion( SpellInfoBackgroundX, SpellInfoBackgroundY, SpellInfoBackgroundW,
                    SpellInfoBackgroundH );
            }
            else if( SpellInfoBackgroundIsTiled )
            {
                SpellbookGump.AddImageTiled( SpellInfoBackgroundX, SpellInfoBackgroundY, SpellInfoBackgroundW,
                                             SpellInfoBackgroundH, SpellInfoBackgroundID );

                SpellbookGump.AddBlackAlphaRegion( SpellInfoBackgroundX, SpellInfoBackgroundY, SpellInfoBackgroundW,
                    SpellInfoBackgroundH );
            }
            else if( SpellInfoBackgroundIsMulti )
            {
                if( SpellInfoBackgroundMultis != null )
                {
                    foreach( GumpMulti gm in SpellInfoBackgroundMultis )
                    {
                        if( gm != null )
                            SpellbookGump.AddImage( gm.X, gm.Y, gm.ID );
                    }
                }
            }
            else
                SpellbookGump.AddImage( SpellInfoBackgroundX, SpellInfoBackgroundY, SpellInfoBackgroundID );
        }

        public virtual void CreateKeyword( ExtendedSpellInfo si )
        {
            Spell spell = SpellRegistry.GetSpellByType( si.SpellTypeName );
            if( spell == null )
            {
                Console.WriteLine( "Warning: null spell with type {0}", si.SpellTypeName );
                return;
            }

            int spellID = SpellRegistry.GetRegistryNumber( spell );
            if( spellID == -1 )
            {
                Console.WriteLine( "Warning: spellID -1 for type {0}", si.SpellTypeName );
                return;
            }

            if( SpellInfoKeywordBackgroundIsBackground )
            {
                SpellbookGump.AddBackground( SpellInfoKeywordBackgroundX, SpellInfoKeywordBackgroundY,
                                     SpellInfoKeywordBackgroundW, SpellInfoKeywordBackgroundH,
                                     SpellInfoKeywordBackgroundID );

                SpellbookGump.AddBlackAlphaRegion( SpellInfoKeywordBackgroundX, SpellInfoKeywordBackgroundY,
                    SpellInfoKeywordBackgroundW, SpellInfoKeywordBackgroundH );
            }
            else if( SpellInfoKeywordBackgroundIsTiled )
                SpellbookGump.AddImageTiled( SpellInfoKeywordBackgroundX, SpellInfoKeywordBackgroundY, SpellInfoKeywordBackgroundW, SpellInfoKeywordBackgroundH, SpellInfoKeywordBackgroundID );
            else if( SpellInfoKeywordBackgroundIsMulti )
            {
                if( SpellInfoKeywordBackgroundMultis != null )
                {
                    foreach( GumpMulti gm in SpellInfoKeywordBackgroundMultis )
                    {
                        if( gm != null )
                            SpellbookGump.AddImage( gm.X, gm.Y, gm.ID );
                    }
                }
            }
            else
                SpellbookGump.AddImage( SpellInfoKeywordBackgroundX, SpellInfoKeywordBackgroundY, SpellInfoKeywordBackgroundID );

            bool found = false;
            string keyword = "";

            for( int i = 0; i < Book.Keywords.Count && !found; i++ )
            {
                SpellKeyword key = Book.Keywords[ i ];
                if( key == null )
                    continue;

                if( spellID == key.SpellID )
                {
                    keyword = key.Keyword;
                    found = true;
                }
            }

            if( CustomSpellbookGump.OldStyle )
                SpellbookGump.AddOldHtmlHued( SpellInfoKeywordLabelX, SpellInfoKeywordLabelY, 200, 17, SpellInfoKeywordLabel, CustomSpellbookGump.SpellInfoKeywordLabelHue );
            else
                SpellbookGump.AddLabel( SpellInfoKeywordLabelX, SpellInfoKeywordLabelY,
                    SpellInfoKeywordLabelHue, SpellInfoKeywordLabel );

            SpellbookGump.AddTextEntry( SpellInfoKeywordTextX + 20, SpellInfoKeywordTextY, SpellInfoKeywordTextW,
                SpellInfoKeywordTextH, SpellInfoKeywordTextHue, 14, found ? keyword : "" );

            SpellbookGump.AddButton( SpellInfoKeywordButtonX, SpellInfoKeywordButtonY,
                SpellInfoKeywordButtonID, SpellInfoKeywordButtonPressedID, 15, GumpButtonType.Reply, 0 );
        }

        public void AddBackgroundButton( int x, int y, int w, int h, int id, int idp, int bid )
        {
            int xI = w / 16;
            int yI = h / 16;

            for( int mx = 1; mx < xI; mx++ )
            {
                for( int my = 1; my < yI; my++ )
                {
                    SpellbookGump.AddButton( x + 16 * mx, y + 16 * my, id + 4, idp + 4, bid, GumpButtonType.Reply, 0 );	//Middle
                }
            }

            for( int xx = 1; xx < xI; xx++ )
            {
                SpellbookGump.AddButton( x + 16 * xx, y, id + 1, idp + 1, bid, GumpButtonType.Reply, 0 ); //Top
                SpellbookGump.AddButton( x + 16 * xx, y - 16 + h, id + 7, idp + 7, bid, GumpButtonType.Reply, 0 ); //Bottom
            }

            for( int yy = 1; yy < yI; yy++ )
            {
                SpellbookGump.AddButton( x, y + 16 * yy, id + 3, idp + 3, bid, GumpButtonType.Reply, 0 ); //Left
                SpellbookGump.AddButton( x - 16 + w, y + 16 * yy, id + 5, idp + 5, bid, GumpButtonType.Reply, 0 ); //Right
            }

            SpellbookGump.AddButton( x, y, id, id, bid, GumpButtonType.Reply, 0 ); //Top Left
            SpellbookGump.AddButton( x + ( w - 16 ), y, id + 2, idp + 2, bid, GumpButtonType.Reply, 0 ); //Top Right
            SpellbookGump.AddButton( x, y + ( h - 16 ), id + 6, idp + 6, bid, GumpButtonType.Reply, 0 ); //Bottom Left
            SpellbookGump.AddButton( x + ( w - 16 ), y + ( h - 16 ), idp + 8, id + 8, bid, GumpButtonType.Reply, 0 ); //Bottom Right

            SpellbookGump.AddBlackAlphaRegion( x, y, w, h );
        }
    }
}