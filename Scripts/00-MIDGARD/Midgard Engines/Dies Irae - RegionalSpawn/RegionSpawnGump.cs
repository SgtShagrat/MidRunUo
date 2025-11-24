/***************************************************************************
 *                                  RegionSpawnGroup.cs
 *                            		
 *  begin                	: Aprile, 2012
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Gumps;
using Server.Regions;

namespace Midgard.Engines.RegionalSpawningSystem
{
    public abstract class RegionSpawnGroup : Midgard.Gumps.MidgardComplexGump
    {
        // Background
        protected override int MainBgId { get { return 9350; } }
        protected override int WindowsBgId { get { return 2620; } }

        // Hues
        protected override int TitleHue { get { return 662; } }
        protected override int GroupsHue { get { return 92; } }
        protected override int ColsHue { get { return 15; } }
        protected override int LabelsHue { get { return 87; } }
        protected override int DefaultValueHue { get { return 0x3F; } } // verde acido
        protected override int DoActionHue { get { return 414; } }
        protected override int HuePrim { get { return 92; } }
        protected override int HueSec { get { return 87; } }
        protected override int DisabledHue { get { return Colors.DarkGray; } } //999; // grigio

        // Buttons
        protected override int ListBtnIdNormal { get { return 2103; } }
        protected override int ListBtnIdPressed { get { return 2104; } }

        // bottone con la X
        protected override int DoActionBtnIdNormal { get { return 0xA50; } }
        protected override int DoActionBtnIdPressed { get { return 0xA51; } }

        // bottone con la -
        protected override int DoAction2BtnIdNormal { get { return 0xA54; } }
        protected override int DoAction2BtnIdPressed { get { return 0xA55; } }

        protected override int NextPageBtnIdNormal { get { return 0x15E1; } }
        protected override int NextPageBtnIdPressed { get { return 0x15E5; } }
        protected override int PrevPageBtnIdNormal { get { return 0x15E3; } }
        protected override int PrevPageBtnIdPressed { get { return 0x15E7; } }

        protected override int UpBtnIdNormal { get { return 5600; } }
        protected override int UpBtnIdPressed { get { return 5604; } }
        protected override int DownBtnIdNormal { get { return 5602; } }
        protected override int DownBtnIdPressed { get { return 5606; } }

        #region design constants
        protected override int VertOffset { get { return 15; } }                // spazio generico verticale
        protected override int BtnWidth { get { return 27; } }                  // larghezza del generico bottone di azione
        protected override int BtnHeight { get { return 27; } }                 // altezza del generico bottone di azione

        protected override int LabelBtnOffsetX { get { return 5; } }            // distanza tra fine del bottone e inizio della sua label in X
        protected override int LabelBtnOffsetY { get { return 5; } }            // distanza tra fine del bottone e inizio della sua label in Y

        protected override int LabelsOffset { get { return 20; } }              // distanza tra un label nella main window e la successiva
        protected override int BtnsOffset { get { return 30; } }                // distanza tra un bottone e il successivo
        protected override int TitleHeight { get { return 40; } }               // altezza della finestra del titolo
        protected override int SubTitlesHeight { get { return 20; } }           // offset verticale per i sottotitoli
        protected override int HorBorder { get { return 10; } }                 // bordo tra la mainwindow e la fine del background

        protected override int MainWindowExtraVertOffset { get { return 15; } } // distanza in X tra inizio della MW e prima label
        protected override int MainWindowExtraHorOffset { get { return 15; } }  // distanza in Y tra inizio della MW e prima label
        protected override int MainWindowWidth { get { return 0; } }            // larghezza della finestra principale

        protected override int NumLabels { get { return 0; } }                  // numero di labels sotto la MW
        protected override int NumButtons { get { return 0; } }                 // numero di bottoni sotto la MW

        protected override bool HasSubtitles { get { return false; } }          // se ha i sottotitoli
        protected override bool HasCloseBtn { get { return true; } }            // se ha il bottone 'close'
        #endregion

        #region props
        public BaseRegion Region { get; private set; }
        #endregion

        protected RegionSpawnGroup( BaseRegion region, Mobile owner )
            : this( region, owner, 50, 50 )
        {
        }

        protected RegionSpawnGroup( BaseRegion region, Mobile owner, int offsetX, int offsetY )
            : base( owner, offsetX, offsetY )
        {
            Region = region;
        }

        #region action buttons
        public override bool CanUseActionButton( Mobile owner )
        {
            return true;
        }

        public override void AddActionButton( int numBtn, string label, int btnId )
        {
            AddActionButton( numBtn, label, btnId, Owner, 0 );
        }
        #endregion

        #region access
        public override bool HasAccess( int flag )
        {
            return Owner != null && Owner.AccessLevel > AccessLevel.Counselor;
        }
        #endregion
    }
}