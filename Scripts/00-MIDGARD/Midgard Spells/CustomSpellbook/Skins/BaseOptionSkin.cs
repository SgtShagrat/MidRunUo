using System.Collections.Generic;
using Server.Gumps;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class BaseOptionSkin
    {
        public bool ShowFieldA = true;
        public int FieldAX = 0;
        public int FieldAY = 0;
        public int FieldAW = 0;
        public int FieldAH = 0;
        public int FieldAID = 0;

        public bool ShowFieldB = true;
        public int FieldBX = 0;
        public int FieldBY = 0;
        public int FieldBW = 0;
        public int FieldBH = 0;
        public int FieldBID = 0;

        public bool ShowFieldC = true;
        public int FieldCX = 0;
        public int FieldCY = 0;
        public int FieldCW = 0;
        public int FieldCH = 0;
        public int FieldCID = 0;

        public int ClosableX = 0;
        public int ClosableY = 0;
        public int ClosableCheckID = 0;
        public int ClosableCheckSelectedID = 0;
        public int ClosableLabelX = 0;
        public int ClosableLabelY = 0;
        public int ClosableLabelHue = 0;
        public string ClosableLabel = "Closable?";

        public int RegsX = 0;
        public int RegsY = 0;
        public int RegsCheckID = 0;
        public int RegsCheckSelectedID = 0;
        public int RegsLabelX = 0;
        public int RegsLabelY = 0;
        public int RegsLabelHue = 0;
        public string RegsLabel = "Show Regs?";

        public int XTextX = 0;
        public int XTextY = 0;
        public int XTextW = 0;
        public int XTextH = 0;
        public int XTextHue = 0;
        public int XBackgroundX = 0;
        public int XBackgroundY = 0;
        public int XBackgroundID = 0;
        public bool XIsBackground = false;
        public bool XIsTiled = false;
        public int XBackgroundW = 0;
        public int XBackgroundH = 0;

        public int XLabelX = 0;
        public int XLabelY = 0;
        public int XLabelHue = 0;
        public string XLabel = "X :";

        public int YTextX = 0;
        public int YTextY = 0;
        public int YTextW = 0;
        public int YTextH = 0;
        public int YTextHue = 0;
        public int YBackgroundX = 0;
        public int YBackgroundY = 0;
        public int YBackgroundID = 0;
        public bool YIsBackground = false;
        public bool YIsTiled = false;
        public int YBackgroundW = 0;
        public int YBackgroundH = 0;

        public int YLabelX = 0;
        public int YLabelY = 0;
        public int YLabelHue = 0;
        public string YLabel = "Y :";

        public int ApplyX = 0;
        public int ApplyY = 0;
        public int ApplyID = 0;
        public int ApplyPressedID = 0;


        public int SkinX = 0;
        public int SkinY = 0;
        public int SkinID = 0;
        public int SkinPressedID = 0;

        public int SkinLabelX = 0;
        public int SkinLabelY = 0;
        public int SkinLabelHue = 0;
        public string SkinLabel = "Change Skin : ";

        public int RemIconsButtonX = 0;
        public int RemIconsButtonY = 0;
        public int RemIconsButtonID = 0;
        public int RemIconsButtonPressedID = 0;
        public bool RemIconsIsMulti = false;
        public List<GumpMulti> RemIconsMultis;

        public int RemIconsLabelX = 0;
        public int RemIconsLabelY = 0;
        public int RemIconsLabelHue = 0;
        public string RemIconsLabel = "All Icons";

        public int RemKeywordsButtonX = 0;
        public int RemKeywordsButtonY = 0;
        public int RemKeywordsButtonID = 0;
        public int RemKeywordsButtonPressedID = 0;
        public bool RemKeywordsIsMulti = false;
        public List<GumpMulti> RemKeywordsMultis;

        public int RemKeywordsLabelX = 0;
        public int RemKeywordsLabelY = 0;
        public int RemKeywordsLabelHue = 0;
        public string RemKeywordsLabel = "All Keywords";


        public int KeywordsLabelX = 0;
        public int KeywordsLabelY = 0;
        public int KeywordsLabelHue = 0;
        public string KeywordsLabel = "Keywords";

        public int KeywordsApplyX = 0;
        public int KeywordsApplyY = 0;
        public int KeywordsApplyID = 0;
        public int KeywordsApplyPressedID = 0;

        public int KeywordsApplyLabelX = 0;
        public int KeywordsApplyLabelY = 0;
        public int KeywordsApplyLabelHue = 0;
        public string KeywordsApplyLabel = "Apply";
        public bool KeywordsApplyShowLabel = false;

        public int KeywordsPerPage = 0;
        public int KeywordsOffsetX = 0;
        public int KeywordsOffsetY = 0;

        public int KeywordsNextX = 0;
        public int KeywordsNextY = 0;
        public int KeywordsNextID = 0;
        public int KeywordsNextPressedID = 0;

        public int KeywordsPrevX = 0;
        public int KeywordsPrevY = 0;
        public int KeywordsPrevID = 0;
        public int KeywordsPrevPressedID = 0;

        public int KeywordsNameX = 0;
        public int KeywordsNameY = 0;
        public int KeywordsNameHue = 0;

        public int KeywordsFieldX = 0;
        public int KeywordsFieldY = 0;
        public int KeywordsFieldW = 0;
        public int KeywordsFieldH = 0;
        public int KeywordsFieldID = 0;
        public bool KeywordsFieldIsBackground = false;
        public bool KeywordsFieldIsTiled = false;
        public bool KeywordsFieldIsMulti = false;
        public List<GumpMulti> KeywordsFieldMultis;

        public int KeywordsTextX = 0;
        public int KeywordsTextY = 0;
        public int KeywordsTextW = 0;
        public int KeywordsTextH = 0;
        public int KeywordsTextHue = 0;

        private CustomSpellbook m_Book;
        private CustomSpellbookGump m_Gump;

        public virtual void ShowOptions( CustomSpellbook book, CustomSpellbookGump gump )
        {
            m_Book = book;
            m_Gump = gump;

            #region Fields
            if( ShowFieldA )
            {
                m_Gump.AddBackground( FieldAX, FieldAY, FieldAW, FieldAH, FieldAID );
                AddBlackAlpha( FieldAX, FieldAY, FieldAW, FieldAH, m_Gump );
            }

            if( ShowFieldB )
            {
                m_Gump.AddBackground( FieldBX, FieldBY, FieldBW, FieldBH, FieldBID );
                AddBlackAlpha( FieldBX, FieldBY, FieldBW, FieldBH, m_Gump );
            }

            if( ShowFieldC )
            {
                m_Gump.AddBackground( FieldCX, FieldCY, FieldCW, FieldCH, FieldCID );
                AddBlackAlpha( FieldCX, FieldCY, FieldCW, FieldCH, m_Gump );
            }

            #endregion

            #region MiscOptions
            m_Gump.AddCheck( ClosableX, ClosableY, ClosableCheckID, ClosableCheckSelectedID, m_Book.Closable, 2 );

            // Close
            if( CustomSpellbookGump.OldStyle )
                m_Gump.AddOldHtmlHued( ClosableLabelX, ClosableLabelY, 200, 17, ClosableLabel, CustomSpellbookGump.ClosableLabelHue );
            else
                m_Gump.AddLabel( ClosableLabelX, ClosableLabelY, ClosableLabelHue, ClosableLabel );

            if( XIsBackground )
                m_Gump.AddBackground( XBackgroundX, XBackgroundY, XBackgroundW, XBackgroundH, XBackgroundID );
            else if( XIsTiled )
                m_Gump.AddImageTiled( XBackgroundX, XBackgroundY, XBackgroundW, XBackgroundH, XBackgroundID );
            else
                m_Gump.AddImage( XBackgroundX, XBackgroundY, XBackgroundID );

            m_Gump.AddTextEntry( XTextX + 10, XTextY, XTextW, XTextH, XTextHue, 3, m_Book.StartX.ToString() );

            if( CustomSpellbookGump.OldStyle )
                m_Gump.AddOldHtmlHued( XLabelX, XLabelY, 200, 17, XLabel, CustomSpellbookGump.OptionsXLabelHue );
            else
                m_Gump.AddLabel( XLabelX, XLabelY, XLabelHue, XLabel );

            if( YIsBackground )
                m_Gump.AddBackground( YBackgroundX, YBackgroundY, YBackgroundW, YBackgroundH, YBackgroundID );
            else if( YIsTiled )
                m_Gump.AddImageTiled( YBackgroundX, YBackgroundY, YBackgroundW, YBackgroundH, YBackgroundID );
            else
                m_Gump.AddImage( YBackgroundX, YBackgroundY, YBackgroundID );

            m_Gump.AddTextEntry( YTextX + 10, YTextY, YTextW, YTextH, YTextHue, 4, m_Book.StartY.ToString() );

            if( CustomSpellbookGump.OldStyle )
                m_Gump.AddOldHtmlHued( YLabelX, YLabelY, 200, 17, YLabel, CustomSpellbookGump.OptionsYLabelHue );
            else
                m_Gump.AddLabel( YLabelX, YLabelY, YLabelHue, YLabel );

            m_Gump.AddButton( ApplyX, ApplyY, ApplyID, ApplyPressedID, 5, GumpButtonType.Reply, 0 );

            m_Gump.AddButton( SkinX, SkinY, SkinID, SkinPressedID, 16, GumpButtonType.Reply, 0 );

            if( CustomSpellbookGump.OldStyle )
                m_Gump.AddOldHtmlHued( SkinLabelX, SkinLabelY, 200, 17, SkinLabel + m_Book.Skin, CustomSpellbookGump.SkinLabelHue );
            else
                m_Gump.AddLabel( SkinLabelX, SkinLabelY, SkinLabelHue, SkinLabel + m_Book.Skin );
            #endregion

            #region Removes
            if( RemIconsIsMulti )
            {
                if( RemIconsMultis != null )
                {
                    for( int i = 0; i < RemIconsMultis.Count; i++ )
                    {
                        GumpMulti gm = RemIconsMultis[ i ];
                        if( gm != null )
                            m_Gump.AddButton( gm.X, gm.Y, gm.ID, gm.IDPressed, 6, GumpButtonType.Reply, 0 );
                    }
                }
            }
            else
                m_Gump.AddButton( RemIconsButtonX, RemIconsButtonY, RemIconsButtonID, RemIconsButtonPressedID, 6, GumpButtonType.Reply, 0 );

            if( CustomSpellbookGump.OldStyle )
                m_Gump.AddOldHtmlHued( RemIconsLabelX, RemIconsLabelY, 200, 17, RemIconsLabel, CustomSpellbookGump.RemIconsLabelHue );
            else
                m_Gump.AddLabel( RemIconsLabelX, RemIconsLabelY, RemIconsLabelHue, RemIconsLabel );

            if( RemKeywordsIsMulti )
            {
                if( RemKeywordsMultis != null )
                {
                    for( int i = 0; i < RemKeywordsMultis.Count; i++ )
                    {
                        GumpMulti gm = RemKeywordsMultis[ i ];
                        if( gm != null )
                            m_Gump.AddButton( gm.X, gm.Y, gm.ID, gm.IDPressed, 7, GumpButtonType.Reply, 0 );
                    }
                }
            }
            else
                m_Gump.AddButton( RemKeywordsButtonX, RemKeywordsButtonY, RemKeywordsButtonID, RemKeywordsButtonPressedID, 7, GumpButtonType.Reply, 0 );

            if( CustomSpellbookGump.OldStyle )
                m_Gump.AddOldHtmlHued( RemKeywordsLabelX, RemKeywordsLabelY, 200, 17, RemKeywordsLabel, CustomSpellbookGump.RemKeywordsLabelHue );
            else
                m_Gump.AddLabel( RemKeywordsLabelX, RemKeywordsLabelY, RemKeywordsLabelHue, RemKeywordsLabel );
            #endregion

            #region Keywords
            if( CustomSpellbookGump.OldStyle )
                m_Gump.AddOldHtmlHued( KeywordsLabelX, KeywordsLabelY, 200, 17, KeywordsLabel, CustomSpellbookGump.KeywordsLabelHue );
            else
                m_Gump.AddLabel( KeywordsLabelX, KeywordsLabelY, KeywordsLabelHue, KeywordsLabel );

            if( KeywordsApplyShowLabel )
            {
                if( CustomSpellbookGump.OldStyle )
                    m_Gump.AddOldHtmlHued( KeywordsApplyLabelX, KeywordsApplyLabelY, 200, 17, KeywordsApplyLabel, CustomSpellbookGump.KeywordsApplyLabelHue );
                else
                    m_Gump.AddLabel( KeywordsApplyLabelX, KeywordsApplyLabelY, KeywordsApplyLabelHue, KeywordsApplyLabel );
            }

            m_Gump.AddButton( KeywordsApplyX, KeywordsApplyY, KeywordsApplyID, KeywordsApplyPressedID, 8, GumpButtonType.Reply, 0 );

            if( m_Book.Keywords != null && m_Book.Keywords.Count > 0 )
            {
                if( m_Book.OptionsOffset < KeywordsPerPage )
                    m_Book.OptionsOffset = KeywordsPerPage;

                if( m_Book.OptionsOffset > KeywordsPerPage )
                    m_Gump.AddButton( KeywordsPrevX, KeywordsPrevY, KeywordsPrevID, KeywordsPrevPressedID, 9, GumpButtonType.Reply, 0 );
                if( ( m_Book.Keywords.Count - m_Book.OptionsOffset ) > 0 )
                    m_Gump.AddButton( KeywordsNextX, KeywordsNextY, KeywordsNextID, KeywordsNextPressedID, 10, GumpButtonType.Reply, 0 );

                for( int i = 0; i < KeywordsPerPage; i++ )
                {
                    int index = ( i + m_Book.OptionsOffset - KeywordsPerPage );
                    if( index >= m_Book.Keywords.Count || index < 0 )
                        continue;
                    int currentX = i * KeywordsOffsetX;
                    int currentY = i * KeywordsOffsetY;

                    SpellKeyword kw = m_Book.Keywords[ index ];
                    if( kw == null )
                        continue;

                    ExtendedSpellInfo si = SpellRegistry.GetExtendedSpellInfoByID( kw.SpellID );
                    if( si == null )
                        continue;

                    if( KeywordsFieldIsBackground )
                        m_Gump.AddBackground( currentX + KeywordsFieldX, currentY + KeywordsFieldY, KeywordsFieldW, KeywordsFieldH, KeywordsFieldID );
                    else if( KeywordsFieldIsTiled )
                        m_Gump.AddImageTiled( currentX + KeywordsFieldX, currentY + KeywordsFieldY, KeywordsFieldW, KeywordsFieldH, KeywordsFieldID );
                    else if( KeywordsFieldIsMulti )
                    {
                        if( KeywordsFieldMultis == null )
                            continue;
                        for( int x = 0; x < KeywordsFieldMultis.Count; x++ )
                        {
                            GumpMulti gm = KeywordsFieldMultis[ x ];
                            if( gm != null )
                                m_Gump.AddImage( currentX + gm.X, currentY + gm.Y, gm.ID );
                        }
                    }
                    else
                        m_Gump.AddImage( currentX + KeywordsFieldX, currentY + KeywordsFieldY, KeywordsFieldID );

                    if( CustomSpellbookGump.OldStyle )
                        m_Gump.AddOldHtmlHued( currentX + KeywordsNameX, currentY + KeywordsNameY, 200, 17, si.Name, CustomSpellbookGump.KeywordsNameHue );
                    else
                        m_Gump.AddLabel( currentX + KeywordsNameX, currentY + KeywordsNameY, KeywordsNameHue, si.Name );
                    
                    m_Gump.AddTextEntry( currentX + KeywordsTextX, currentY + KeywordsTextY, KeywordsTextW, KeywordsTextH, KeywordsTextHue, 200 + i, ( kw.Keyword ?? "" ) );
                }
            }
            #endregion
        }

        private static void AddBlackAlpha( int x, int y, int width, int height, Gump g )
        {
            if( g == null )
                return;

            g.AddImageTiled( x + 10, y + 10, width - 20, height - 20, 2624 );
            g.AddAlphaRegion( x + 10, y + 10, width - 20, height - 20 );
        }
    }
}