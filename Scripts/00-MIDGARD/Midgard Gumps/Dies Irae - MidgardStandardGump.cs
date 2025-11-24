/***************************************************************************
 *                               Dies Irae - MidgardStandardGump.cs
 *
 *   begin                : 07 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Gumps;

namespace Midgard.Gumps
{
    public class MidgardStandardGump : Gump
    {
        // Background
        protected const int MainBgId = 9350;
        protected const int WindowsBgId = 2620;

        // Hues
        protected const int TitleHue = 662;
        protected const int GroupsHue = 92;
        protected const int ColsHue = 15;
        protected const int LabelsHue = 87;
        protected const int DefaultValueHue = 0x3F; // verde acido
        protected const int DoActionHue = 414;
        protected const int HuePrim = 92;
        protected const int HueSec = 87;
        protected const int DisabledHue = 999; // grigio

        // Buttons
        protected const int ListBtnIdNormal = 2103;
        protected const int ListBtnIdPressed = 2104;

        // bottone con la X
        protected const int DoActionBtnIdNormal = 0xA50;
        protected const int DoActionBtnIdPressed = 0xA51;

        // bottone con la -
        protected const int DoAction2BtnIdNormal = 0xA54;
        protected const int DoAction2BtnIdPressed = 0xA55;

        protected const int NextPageBtnIdNormal = 0x15E1;
        protected const int NextPageBtnIdPressed = 0x15E5;
        protected const int PrevPageBtnIdNormal = 0x15E3;
        protected const int PrevPageBtnIdPressed = 0x15E7;

        protected const int UpBtnIdNormal = 5600;
        protected const int UpBtnIdPressed = 5604;
        protected const int DownBtnIdNormal = 5602;
        protected const int DownBtnIdPressed = 5606;

        protected const int LabelColor32 = 0xFFFFFF;
        protected const int SelectedColor32 = 0x8080FF;
        protected const int DisabledColor32 = 0x808080;
        protected const int GreenYellow32 = 0xADFF2F;
        protected const int OldGold32 = 0xCFB53B;

        #region design constants
        protected const int VertOffset = 15;                                // spazio generico verticale
        protected const int BtnWidth = 27;                                  // larghezza del generico bottone di azione
        protected const int BtnHeight = 27;                                 // altezza del generico bottone di azione

        protected const int LabelBtnOffsetX = 5;                            // distanza tra fine del bottone e inizio della sua label in X
        protected const int LabelBtnOffsetY = 5;                            // distanza tra fine del bottone e inizio della sua label in Y

        public virtual int LabelsOffset { get { return 20; } }              // distanza tra un label nella main window e la successiva
        public virtual int BtnsOffset { get { return 30; } }                // distanza tra un bottone e il successivo
        public virtual int TitleHeight { get { return 40; } }               // altezza della finestra del titolo
        public virtual int SubTitlesHeight { get { return 20; } }           // offset verticale per i sottotitoli
        public virtual int HorBorder { get { return 10; } }                 // bordo tra la mainwindow e la fine del background

        public virtual int MainWindowExtraVertOffset { get { return 15; } } // distanza in X tra inizio della MW e prima label
        public virtual int MainWindowExtraHorOffset { get { return 15; } }  // distanza in Y tra inizio della MW e prima label
        public virtual int MainWindowWidth { get { return 0; } }            // larghezza della finestra principale

        public virtual int NumLabels { get { return 0; } }                  // numero di labels sotto la MW
        public virtual int NumButtons { get { return 0; } }                 // numero di bottoni sotto la MW

        public virtual bool HasSubtitles { get { return false; } }          // se ha i sottotitoli
        public virtual bool HasCloseBtn { get { return true; } }            // se ha il bottone 'close'
        #endregion

        #region fields
        private Mobile m_Owner;
        #endregion

        #region props
        public Mobile Owner
        {
            get { return m_Owner; }
        }
        #endregion

        public MidgardStandardGump( Mobile owner, int offsetX, int offsetY )
            : base( offsetX, offsetY )
        {
            m_Owner = owner;

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
            int y = 10;

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

        public virtual void AddMainWindowButton( int x, int y, int id )
        {
            AddMainWindowButton( x, y, id, false );
        }

        public virtual void AddMainWindowButton( int x, int y, int id, bool allow )
        {
            if( allow )
                AddButton( x, y, ListBtnIdNormal, ListBtnIdPressed, id, GumpButtonType.Reply, 0 );
        }
        #endregion

        public virtual void AddWindow( int x, int y, int width, int height )
        {
            AddBackground( x, y, width, height, WindowsBgId );
            AddBlackAlpha( x + 10, y + 10, width - 20, height - 20 );
        }

        #region titles
        public virtual void AddMainTitle( int titleWidth, string title )
        {
            AddTitle( GetTitleX( titleWidth, GetBackGroundWidth() ), VertOffset, titleWidth, TitleHeight, title );
        }

        public virtual void AddTitle( int x, int y, int width, int height, string title )
        {
            AddWindow( x, y, width, height );
            AddHtml( x + 10, y + 10, width - 20, height - 20, Color( Center( title ), OldGold32 ), false, false );
        }

        public virtual void AddSubTitle( int x, string title )
        {
            AddLabel( x, GetMainWindowsStartY() - 20, ColsHue, title );
        }

        public virtual void AddSubTitleImage( int x, int imageId )
        {
            AddImage( x, GetMainWindowsStartY() - 20, imageId );
        }
        #endregion

        #region action buttons
        public virtual void AddActionButton( int numBtn, string label, int btnId, Mobile owner, bool allow )
        {
            int y = GetFirstBtnY() + ( numBtn - 1 ) * BtnsOffset;
            int x = HorBorder;

            if( allow )
            {
                AddButton( x, y, DoActionBtnIdNormal, DoActionBtnIdPressed, btnId, GumpButtonType.Reply, 0 );
                AddLabel( x + BtnsOffset + LabelBtnOffsetX, y + LabelBtnOffsetY, DoActionHue, label );
            }
            else
            {
                AddImage( x, y, DoAction2BtnIdNormal );
                AddLabel( x + BtnsOffset + LabelBtnOffsetX, y + LabelBtnOffsetY, DisabledHue, label );
            }
        }

        public virtual void AddActionButton( int numBtn, string label, int btnId )
        {
            AddActionButton( numBtn, label, btnId, m_Owner );
        }

        public virtual void AddActionButton( int numBtn, string label, int btnId, Mobile owner )
        {
            AddActionButton( numBtn, label, btnId, owner, false );
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

            AddLabel( x - 37, y + LabelBtnOffsetY, DoActionHue, "close" ); // 37 e' l'offset orizzontale per la parola "close"
            AddButton( x, y, DoActionBtnIdNormal, DoActionBtnIdPressed, buttonID, GumpButtonType.Reply, 0 );
        }
        #endregion

        public virtual void AddGuideButton( int buttonID )
        {
            int x = GetBackGroundWidth() - HorBorder - BtnWidth;
            int y = GetFirstBtnY() + ( Math.Max( NumButtons - 2, 0 ) * BtnsOffset );

            AddLabel( x - 60, y + LabelBtnOffsetY, DoActionHue, "open guide" );
            AddButton( x, y, DoActionBtnIdNormal, DoActionBtnIdPressed, buttonID, GumpButtonType.Reply, 0 );
        }

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
        #endregion
    }
}