/***************************************************************************
 *                               MidgardComplexGump.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Ultima;

namespace Midgard.Gumps
{
    public abstract class MidgardComplexGump : Gump
    {
        // Background
        protected virtual int MainBgId { get { return 9350; } }

        protected virtual int WindowsBgId { get { return 2620; } }

        // Hues
        protected virtual int TitleHue { get { return 662; } }
        protected virtual int GroupsHue { get { return 92; } }
        protected virtual int ColsHue { get { return 15; } }
        protected virtual int LabelsHue { get { return 87; } }
        protected virtual int DefaultValueHue { get { return 0x3F; } } // verde acido
        protected virtual int DoActionHue { get { return 414; } }
        protected virtual int HuePrim { get { return 92; } }
        protected virtual int HueSec { get { return 87; } }
        protected virtual int DisabledHue { get { return Colors.DarkGray; } } //999; // grigio

        // Buttons
        protected virtual int ListBtnIdNormal { get { return 2103; } }
        protected virtual int ListBtnIdPressed { get { return 2104; } }

        // bottone con la X
        protected virtual int DoActionBtnIdNormal { get { return 0xA50; } }
        protected virtual int DoActionBtnIdPressed { get { return 0xA51; } }

        // bottone con la -
        protected virtual int DoAction2BtnIdNormal { get { return 0xA54; } }
        protected virtual int DoAction2BtnIdPressed { get { return 0xA55; } }

        protected virtual int NextPageBtnIdNormal { get { return 0x15E1; } }
        protected virtual int NextPageBtnIdPressed { get { return 0x15E5; } }
        protected virtual int PrevPageBtnIdNormal { get { return 0x15E3; } }
        protected virtual int PrevPageBtnIdPressed { get { return 0x15E7; } }

        protected virtual int UpBtnIdNormal { get { return 5600; } }
        protected virtual int UpBtnIdPressed { get { return 5604; } }
        protected virtual int DownBtnIdNormal { get { return 5602; } }
        protected virtual int DownBtnIdPressed { get { return 5606; } }

        protected const int LabelColor32 = 0xFFFFFF;
        protected const int SelectedColor32 = 0x8080FF;
        protected const int DisabledColor32 = 0x808080;
        protected const int GreenYellow32 = 0xADFF2F;
        protected const int OldGold32 = 0xCFB53B;

        #region design constants
        protected virtual int VertOffset { get { return 15; } }                // spazio generico verticale
        protected virtual int BtnWidth { get { return 27; } }                  // larghezza del generico bottone di azione
        protected virtual int BtnHeight { get { return 27; } }                 // altezza del generico bottone di azione

        protected virtual int LabelBtnOffsetX { get { return 5; } }            // distanza tra fine del bottone e inizio della sua label in X
        protected virtual int LabelBtnOffsetY { get { return 5; } }            // distanza tra fine del bottone e inizio della sua label in Y

        protected virtual int LabelsOffset { get { return 20; } }              // distanza tra un label nella main window e la successiva
        protected virtual int BtnsOffset { get { return 30; } }                // distanza tra un bottone e il successivo
        protected virtual int TitleHeight { get { return 40; } }               // altezza della finestra del titolo
        protected virtual int SubTitlesHeight { get { return 20; } }           // offset verticale per i sottotitoli
        protected virtual int HorBorder { get { return 10; } }                 // bordo tra la mainwindow e la fine del background

        protected virtual int MainWindowExtraVertOffset { get { return 15; } } // distanza in X tra inizio della MW e prima label
        protected virtual int MainWindowExtraHorOffset { get { return 15; } }  // distanza in Y tra inizio della MW e prima label
        protected virtual int MainWindowWidth { get { return 0; } }            // larghezza della finestra principale

        protected virtual int NumLabels { get { return 0; } }                  // numero di labels sotto la MW
        protected virtual int NumButtons { get { return 0; } }                 // numero di bottoni sotto la MW

        protected virtual bool HasSubtitles { get { return false; } }          // se ha i sottotitoli
        protected virtual bool HasCloseBtn { get { return true; } }            // se ha il bottone 'close'
        protected virtual string CloseBtnText { get { return "close"; } }      // il testo del bottone close.
        #endregion

        #region props
        public Mobile Owner { get; private set; }
        #endregion

        protected MidgardComplexGump( Mobile owner, int offsetX, int offsetY )
            : base( offsetX, offsetY )
        {
            Owner = owner;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
        }

        #region background
        public virtual int GetBackGroundWidth()
        {
            return MainWindowWidth + 2 * HorBorder;
        }

        public virtual int GetBackGroundHeight()
        {
            int height = ( 4 * VertOffset ) + TitleHeight + GetMainWindowHeight() + ( NumButtons * BtnsOffset );

            if( HasCloseBtn && NumButtons < 1 )
                height += BtnsOffset; // se non ho bottoni di default ma voglio io bottone di chiusura

            if( HasSubtitles )
                height += SubTitlesHeight; // se ho i sottotitoli aggiunti la loro altezza

            return height;
        }

        public virtual void AddMainBackground()
        {
            AddBackground( 0, 0, GetBackGroundWidth(), GetBackGroundHeight(), MainBgId );
        }

        public virtual void AddMainBgPageBtn( bool next, int id )
        {
            int x = GetBackGroundWidth() - 25;
            const int y = 10;

            if( !next )
                x -= 20;

            if( next )
                AddButton( x, y, NextPageBtnIdNormal, NextPageBtnIdPressed, id, GumpButtonType.Reply, 0 );
            else
                AddButton( x, y, PrevPageBtnIdNormal, PrevPageBtnIdPressed, id, GumpButtonType.Reply, 0 );
        }
        #endregion

        public virtual int GetTitleX( int width, int bgWidth )
        {
            return ( bgWidth - width ) / 2;
        }

        public virtual void AddBlackAlpha( int x, int y, int width, int height )
        {
            AddImageTiled( x, y, width, height, 2624 );
            AddAlphaRegion( x, y, width, height );
        }

        public virtual void AddPrevPageButton( int page, int id )
        {
            AddButton( MainWindowWidth - 45, 10, 0x15E3, 0x15E7, id, GumpButtonType.Page, page ); // Back
        }

        public virtual void AddNextPageButton( int page, int id )
        {
            AddButton( MainWindowWidth - 25, 10, 0x15E1, 0x15E5, id, GumpButtonType.Page, page + 1 ); // Next
        }

        #region main windows
        public virtual int GetMainWindowsStartY()
        {
            int y = ( 2 * VertOffset ) + TitleHeight;

            if( HasSubtitles )
                y += SubTitlesHeight;

            return y;
        }

        public virtual int GetMainWindowHeight()
        {
            return ( NumLabels * LabelsOffset ) + ( 2 * MainWindowExtraVertOffset );
        }

        public virtual void AddMainWindow()
        {
            AddWindow( HorBorder, GetMainWindowsStartY(), MainWindowWidth, GetMainWindowHeight() );
            if( Owner.PlayerDebug )
            {
                for( int i = 0; i < MainWindowWidth; i = i + 50 )
                    AddButton( HorBorder + i * 50, GetMainWindowsStartY(), ListBtnIdNormal, ListBtnIdPressed, 1,
                               GumpButtonType.Reply, 0 );
                for( int j = 0; j < GetMainWindowHeight(); j = j + 50 )
                    AddButton( HorBorder, GetMainWindowsStartY() + j, ListBtnIdNormal, ListBtnIdPressed, 1,
                               GumpButtonType.Reply, 0 );
            }
        }

        public virtual void AddMainHtmlWindow( string text )
        {
            AddHtmlWindow( HorBorder, GetMainWindowsStartY(), MainWindowWidth, GetMainWindowHeight(), text );
        }

        public virtual int GetMainWindowFirstLabelY()
        {
            return GetMainWindowsStartY() + MainWindowExtraVertOffset;
        }

        public virtual int GetMainWindowLabelsX()
        {
            return HorBorder + MainWindowExtraHorOffset;
        }

        public virtual void AddMainWindowPageBtn( bool next, int id )
        {
            int x = HorBorder + MainWindowWidth - 25;
            int y = GetMainWindowsStartY() + 10;

            if( !next )
                x -= 20;

            if( next )
                AddButton( x, y, NextPageBtnIdNormal, NextPageBtnIdPressed, id, GumpButtonType.Reply, 0 );
            else
                AddButton( x, y, PrevPageBtnIdNormal, PrevPageBtnIdPressed, id, GumpButtonType.Reply, 0 );
        }

        public virtual void AddMainWindowButton( int y, int id, int flag, bool specialAllow )
        {
            int x = HorBorder + MainWindowWidth - 25;

            if( HasAccess( flag ) || specialAllow )
                AddButton( x, y, ListBtnIdNormal, ListBtnIdPressed, id, GumpButtonType.Reply, 0 );
        }

        public virtual void AddMainWindowButton( int x, int y, int id, int flag )
        {
            AddMainWindowButton( x, y, id, flag, false );
        }

        public virtual void AddMainWindowButton( int x, int y, int id, int flag, bool specialAllow )
        {
            AddMainWindowButton( x, y, ListBtnIdNormal, ListBtnIdPressed, id, flag, specialAllow );
        }

        public virtual void AddMainWindowButton( int x, int y, int normalId, int pressedId, int id, int flag, bool specialAllow )
        {
            if( HasAccess( flag ) || specialAllow )
                AddButton( x, y, normalId, pressedId, id, GumpButtonType.Reply, 0 );
        }

        protected virtual int GreenBtnNormal { get { return 0x2C88; } }
        protected virtual int GreenBtnPressed { get { return 0x2C8A; } }

        protected virtual int RedBtnNormal { get { return 0x2C92; } }
        protected virtual int RedBtnPressed { get { return 0x2C94; } }

        public virtual void AddMainWindowRedGreenButton( int y, int greenId, int redId, int flag, bool specialAllow )
        {
            int x = HorBorder + MainWindowWidth - 25;

            if( HasAccess( flag ) || specialAllow )
            {
                AddMainWindowButton( x - 15, y, GreenBtnPressed, GreenBtnPressed, greenId, flag, specialAllow );
                AddMainWindowButton( x, y, RedBtnNormal, RedBtnPressed, greenId, flag, specialAllow );
            }
        }
        #endregion

        public virtual void AddWindow( int x, int y, int width, int height )
        {
            AddBackground( x, y, width, height, WindowsBgId );
            AddBlackAlpha( x + 10, y + 10, width - 20, height - 20 );
        }

        public virtual void AddHtmlWindow( int x, int y, int width, int height, string text )
        {
            AddWindow( x, y, width, height );
            AddHtml( x + 10, y + 10, width - 20, height - 20, text, false, true );
        }

        #region titles
        public int ComputeWidth( string title )
        {
            int width = 0;

            foreach( char c in title )
            {
                if( Char.IsUpper( c ) ) // 'M' width
                    width += 15;
                else if( c == ' ' ) // ' ' width
                    width += 3;
                else if( Char.IsLower( c ) ) // 'm' width
                    width += 10;
            }

            width = (int)( width / 1.15 );

            return width;
        }

        private Dictionary<char, int> m_LengthDict = new Dictionary<char, int>();

        private int ComputeTextWidth( string text )
        {
            int width = 0;

            foreach ( char c in text )
                width += GetUoCharWidth( c, 3 );

            return width;
        }

        private int GetUoCharWidth( char c, int font )
        {
            if( m_LengthDict == null )
                m_LengthDict = new Dictionary<char, int>();

            if( !m_LengthDict.ContainsKey( c ) )
            {
                ASCIIFont asciiFont = ASCIIFont.GetFixed( font );
                m_LengthDict[ c ] = asciiFont.GetBitmap( c ).Width;
            }

            return m_LengthDict[ c ];
        }

        public virtual void AddMainTitle( string title )
        {
            int titleWidth = ComputeTextWidth( title ); // ComputeWidth( title );

            AddTitle( GetTitleX( titleWidth, GetBackGroundWidth() ), VertOffset, titleWidth, TitleHeight, title );
        }

        public virtual void AddMainTitle( int titleWidth, string title )
        {
            AddTitle( GetTitleX( titleWidth, GetBackGroundWidth() ), VertOffset, titleWidth, TitleHeight, title );
        }

        public virtual void AddTitle( int x, int y, int width, int height, string title )
        {
            AddWindow( x, y, width, height );

            AddHtml( x + 10, y + 10, width - 20, height - 20, Color( Center( Ascii( title ) ), OldGold32 ), false, false );
        }

        public virtual void AddSubTitle( int x, string title )
        {
            AddLabel( x, GetMainWindowsStartY() - 17, ColsHue, title );
        }

        public virtual void AddSubTitleImage( int x, int imageId )
        {
            AddImage( x, GetMainWindowsStartY() - 17, imageId );
        }
        #endregion

        #region action buttons
        public abstract bool CanUseActionButton( Mobile owner );

        public virtual void AddActionButton( int numBtn, string label, int btnId, Mobile owner, int flag, bool specialAllow )
        {
            int y = GetFirstBtnY() + ( numBtn - 1 ) * BtnsOffset;
            int x = HorBorder;

            if( CanUseActionButton( owner ) )
            {
                if( HasAccess( flag ) || specialAllow )
                {
                    AddButton( x, y, DoActionBtnIdNormal, DoActionBtnIdPressed, btnId, GumpButtonType.Reply, 0 );

                    AddHtml( x + BtnsOffset + LabelBtnOffsetX, y + LabelBtnOffsetY, 240, 20, Color( Ascii( label ), Colors.Indigo ), false, false );
                }
                else
                {
                    AddImage( x, y, DoAction2BtnIdNormal );

                    AddHtml( x + BtnsOffset + LabelBtnOffsetX, y + LabelBtnOffsetY, 240, 20, Ascii( label ), false, false );
                }
            }
            else if( specialAllow || HasAccess( flag ) )
            {
                AddButton( x, y, DoActionBtnIdNormal, DoActionBtnIdPressed, btnId, GumpButtonType.Reply, 0 );

                AddHtml( x + BtnsOffset + LabelBtnOffsetX, y + LabelBtnOffsetY, 240, 20, Color( Ascii( label ), Colors.Indigo ), false, false );
            }
        }

        public virtual void AddActionButton( int numBtn, string label, int btnId )
        {
            AddActionButton( numBtn, label, btnId, Owner, 0 );
        }

        public virtual void AddActionButton( int numBtn, string label, int btnId, Mobile owner, int flag )
        {
            AddActionButton( numBtn, label, btnId, owner, flag, false );
        }

        public virtual int GetFirstBtnY()
        {
            return GetMainWindowsStartY() + GetMainWindowHeight() + VertOffset;
        }
        #endregion

        #region close btn
        public virtual void AddCloseButton()
        {
            AddCloseButton( 0 );
        }

        public virtual void AddCloseButton( int buttonID )
        {
            int x = GetBackGroundWidth() - HorBorder - BtnWidth;
            int y = GetFirstBtnY() + ( Math.Max( NumButtons - 1, 0 ) * BtnsOffset );

            AddHtml( x - 240, y + LabelBtnOffsetY, 240, 20, Right( Color( Ascii( CloseBtnText ), Colors.Indigo ) ), false, false );

            AddButton( x, y, DoActionBtnIdNormal, DoActionBtnIdPressed, buttonID, GumpButtonType.Reply, 0 );
        }
        #endregion

        public void AddSelectedButton( int x, int y, int buttonID, string text, bool isSelection )
        {
            AddButton( x, y - 1, isSelection ? 4006 : 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
            AddHtml( x + 35, y, 200, 20, Color( text, isSelection ? SelectedColor32 : LabelColor32 ), false, false );
        }

        public void AddButtonLabeled( int x, int y, int buttonID, string text, int hue )
        {
            AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
            AddHtml( x + 35, y, 240, 20, Color( text, hue ), false, false );
        }

        public void AddTextField( int x, int y, int width, int height, int index )
        {
            AddBackground( x - 2, y - 2, width + 4, height + 4, 0x2486 );
            AddTextEntry( x + 2, y + 2, width - 4, height - 4, 0, index, "" );
        }

        public virtual void AddGuideButton( int buttonID )
        {
            int x = GetBackGroundWidth() - HorBorder - BtnWidth;
            int y = GetFirstBtnY() + ( Math.Max( NumButtons - 2, 0 ) * BtnsOffset );

            AddHtml( x - 240, y + LabelBtnOffsetY, 240, 20, Right( Color( Ascii( "open guide" ), Colors.Indigo ) ), false, false );

            AddButton( x, y, DoActionBtnIdNormal, DoActionBtnIdPressed, buttonID, GumpButtonType.Reply, 0 );
        }

        public void AddHtmlText( int x, int y, int width, int height, TextDefinition text, bool back, bool scroll )
        {
            if( text != null && text.Number > 0 )
                AddHtmlLocalized( x, y, width, height, text.Number, back, scroll );
            else if( text != null && text.String != null )
                AddHtml( x, y, width, height, text.String, back, scroll );
        }

        #region access
        public virtual void RegisterUse( Type type )
        {
        }

        public virtual bool HasAccess( int flag )
        {
            if( Owner != null && Owner.AccessLevel > AccessLevel.Counselor )
                return true;

            return false;
        }

        public virtual string FormatPrivateInfo( string info )
        {
            return "--private info--";
        }
        #endregion

        #region hue
        public virtual int GetHueFor( Mobile m, int hue )
        {
            return ( hue == HuePrim ? HueSec : HuePrim );
        }

        public virtual int GetHueFor( int hue )
        {
            return GetHueFor( null, hue );
        }
        #endregion

        #region format
        public string Color( string text, int color )
        {
            return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
        }

        public string Center( string text )
        {
            return String.Format( "<CENTER>{0}</CENTER>", text );
        }

        public string Ascii( string text )
        {
            return String.Format( "<BIG>{0}", text );
        }

        public string Right( string text )
        {
            return String.Format( "<DIV ALIGN=RIGHT>{0}</DIV>", text );
        }

        public string Left( string text )
        {
            return String.Format( "<DIV ALIGN=LEFT>{0}</DIV>", text );
        }

        public static string FormatTimeSpan( TimeSpan ts )
        {
            return String.Format( "{0:D2}:{1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours % 24, ts.Minutes % 60, ts.Seconds % 60 );
        }
        #endregion
    }
}