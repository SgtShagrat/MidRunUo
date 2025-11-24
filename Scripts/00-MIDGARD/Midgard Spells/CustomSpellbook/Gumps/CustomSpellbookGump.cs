using System;
using Server;
using Server.Gumps;
using Server.Network;
using System.Collections.Generic;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class CustomSpellbookGump : Gump
    {
        public static readonly bool OldStyle = true;

        public enum Buttons
        {
            Close,

            OptionBtn = 1,
            OptionsApply = 5,
            RemoveIcons = 6,
            RemoveKeywords = 7,
            UpdateKeywords = 8,
            KeywordsPrevious = 9,
            KeywordsNext = 10,
            SpellsPrev = 12,
            SpellsNext = 13,
            InfoApplyKeyword = 15,
            SkinChange = 16
        }

        public enum TextRelays
        {
            Xrelay = 3,
            Yrelay = 4,
            KeywordRelay = 14
        }

        public enum Switches
        {
            Closable = 2,
        }

        #region hues
        public const int SchoolNameHue = Colors.Gold;
        public const int SpellPrevLabelHue = Colors.AntiqueWhite;
        public const int SpellNextLabelHue = Colors.AntiqueWhite;
        public const int SpellInfoShowNameHue = Colors.RoyalBlue;
        public const int SpellInfoShowManaHue = Colors.SteelBlue;
        public const int SpellInfoShowSkillHue = Colors.SteelBlue;
        public const int SpellInfoShowReagentsHue = Colors.SteelBlue;
        public const int SpellInfoShowDescriptionHue = Colors.AntiqueWhite;
        public const int SpellInfoKeywordLabelHue = Colors.RoyalBlue;
        public const int OptionsShowLabelHue = Colors.RoyalBlue;
        public const int ClosableLabelHue = Colors.Gold;
        public const int OptionsXLabelHue = Colors.SteelBlue;
        public const int OptionsYLabelHue = Colors.SteelBlue;
        public const int SkinLabelHue = Colors.SteelBlue;
        public const int RemIconsLabelHue = Colors.SteelBlue;
        public const int RemKeywordsLabelHue = Colors.SteelBlue;
        public const int KeywordsLabelHue = Colors.AntiqueWhite;
        public const int KeywordsApplyLabelHue = Colors.LightBlue;
        public const int KeywordsNameHue = Colors.AntiqueWhite;
        #endregion

        private Mobile m_From;
        private CustomSpellbook m_Book;

        public BaseSkin Skin;
        public BaseOptionSkin OptionSkin;

        public CustomSpellbookGump( Mobile from, CustomSpellbook book )
            : base( book.StartX, book.StartY )
        {
            m_From = from;
            m_Book = book;

            Closable = m_Book.Closable;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );

            Skin = SkinList.GetSkin( m_Book.Skin );
            OptionSkin = SkinList.GetOptionSkin( m_Book.Skin );

            Skin.CreateGump( m_Book, this );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( m_Book.Deleted )
                return;

            Mobile from = state.Mobile;
            int bid = info.ButtonID;

            if( bid == (int)Buttons.Close )
                from.CloseGump( typeof( CustomSpellbookGump ) );
            else if( bid == (int)Buttons.OptionBtn )
            {
                m_Book.CurrentSchool = m_Book.CurrentSchool == SchoolInfo.Options ? SchoolInfo.None : SchoolInfo.Options;

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid == (int)Buttons.OptionsApply )
            {
                m_Book.Closable = info.IsSwitched( (int)Switches.Closable );

                TextRelay xrelay = info.GetTextEntry( (int)TextRelays.Xrelay );
                TextRelay yrelay = info.GetTextEntry( (int)TextRelays.Yrelay );

                string xtext = ( xrelay == null ? null : xrelay.Text.Trim() );
                string ytext = ( yrelay == null ? null : yrelay.Text.Trim() );

                if( string.IsNullOrEmpty( xtext ) || string.IsNullOrEmpty( ytext ) )
                {
                    from.SendMessage( "You must enter a positive integer value in the box." );
                }
                else
                {
                    int x = 5; //m_Book.StartX;
                    int y = 75; //m_Book.StartY;
                    try
                    {
                        x = Int32.Parse( xtext );
                        y = Int32.Parse( ytext );

                        if( x >= 0 && y >= 0 )
                        {
                            m_Book.StartX = x;
                            m_Book.StartY = y;
                            m_Book.PublicOverheadMessage( MessageType.Whisper, 0, false, string.Format( "Updated X Coordinate to : {0}", xtext ) );
                            m_Book.PublicOverheadMessage( MessageType.Whisper, 0, false, string.Format( "Updated Y Coordinate to : {0}", ytext ) );
                        }
                        else
                            from.SendMessage( "You must enter a positive integer value in both boxes." );
                    }
                    catch
                    {
                        from.SendMessage( "You must enter a positive integer value in both boxes." );
                    }
                }

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid == (int)Buttons.RemoveIcons )
            {
                m_Book.DragableIcons.Clear();
                from.SendMessage( "All Icons have been deleted." );

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid == (int)Buttons.RemoveKeywords )
            {
                m_Book.Keywords.Clear();
                from.SendMessage( "All Keywords have been deleted." );

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid == (int)Buttons.UpdateKeywords )
            {
                for( int i = 0; i < OptionSkin.KeywordsPerPage; i++ )
                {
                    int index = ( i + m_Book.OptionsOffset - OptionSkin.KeywordsPerPage );

                    if( index >= m_Book.Keywords.Count || index < 0 )
                        continue;

                    TextRelay keywordRelay = info.GetTextEntry( 200 + i );
                    string keyword = ( keywordRelay == null ? null : keywordRelay.Text.Trim() );

                    if( string.IsNullOrEmpty( keyword ) )
                    {
                        from.SendMessage( "Removing keyword." );
                        SpellKeyword kw = new SpellKeyword( -1, "" );
                        m_Book.Keywords.RemoveAt( index );
                        m_Book.Keywords.Insert( index, kw );
                    }
                    else
                    {
                        from.SendMessage( "Changing keyword." );
                        SpellKeyword kw = new SpellKeyword( m_Book.Keywords[ index ].SpellID, keyword );
                        m_Book.Keywords.RemoveAt( index );
                        m_Book.Keywords.Insert( index, kw );
                    }
                }

                List<SpellKeyword> temp = new List<SpellKeyword>();
                for( int i = 0; i < m_Book.Keywords.Count; i++ )
                {
                    SpellKeyword kw = m_Book.Keywords[ i ];
                    if( kw.SpellID != -1 )
                        temp.Add( kw );
                }

                m_Book.Keywords.Clear();
                for( int i = 0; i < temp.Count; i++ )
                    m_Book.Keywords.Add( temp[ i ] );

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid == (int)Buttons.KeywordsPrevious )
            {
                m_Book.OptionsOffset -= OptionSkin.KeywordsPerPage;
                if( m_Book.OptionsOffset < OptionSkin.KeywordsPerPage )
                    m_Book.OptionsOffset = OptionSkin.KeywordsPerPage;

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid == (int)Buttons.KeywordsNext )
            {
                m_Book.OptionsOffset += OptionSkin.KeywordsPerPage;

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid == (int)Buttons.SpellsPrev )
            {
                m_Book.SpellsOffset -= Skin.SpellsPerPage;
                if( m_Book.SpellsOffset < Skin.SpellsPerPage )
                    m_Book.SpellsOffset = Skin.SpellsPerPage;

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid == (int)Buttons.SpellsNext )
            {
                m_Book.SpellsOffset += Skin.SpellsPerPage;

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid == (int)Buttons.InfoApplyKeyword )
            {
                TextRelay keywordRelay = info.GetTextEntry( 14 );
                string keyword = ( keywordRelay == null ? null : keywordRelay.Text.Trim() );

                m_Book.AddKeyword( keyword, m_Book.CurrentSpell );

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid == (int)Buttons.SkinChange )
            {
                if( m_Book.Skin + 1 > SkinList.HighestSkin )
                    m_Book.Skin = 0;
                else
                    m_Book.Skin++;

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid >= 1000 && bid < 2000 ) //Spell Casts
            {
                Spell spell = SpellRegistry.NewSpell( bid - 1000, from, null );
                if( spell != null )
                    spell.Cast();

                // Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), delegate { m_From.SendGump( new CustomSpellbookGump( m_From, m_Book ) ); } );
                // new GumpUpTimer( m_From, m_Book );
            }
            else if( bid >= 2000 && bid < 3000 ) //Spell Infos
            {
                if( bid - 2000 == m_Book.CurrentSpell )
                    m_Book.CurrentSpell = -1;
                else
                    m_Book.CurrentSpell = bid - 2000;

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else if( bid >= 3000 && bid < 4000 ) //Spell Icons
            {
                int spellID = bid - 3000;

                ExtendedSpellInfo si = SpellRegistry.GetExtendedSpellInfoByID( spellID );
                if( si == null )
                    return;

                int background = SchoolInfo.FindBackgroundInfo( si.School );
                if( background == -1 )
                    return;

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
                from.SendGump( new IconPlacementGump( m_Book, from, 375, 115, 5, si.SpellIcon, spellID, background ) );
            }
            else if( bid >= 100 && bid < 200 ) //School Selects
            {
                if( m_Book.CurrentSchool == SchoolInfo.SchoolList[ bid - 100 ] )
                    m_Book.CurrentSchool = SchoolInfo.None;
                else
                    m_Book.CurrentSchool = SchoolInfo.SchoolList[ bid - 100 ];

                from.SendGump( new CustomSpellbookGump( from, m_Book ) );
            }
            else
            {
                from.SendMessage( string.Format( "Button #{0} pressed", bid ) );
            }
        }
    }
}