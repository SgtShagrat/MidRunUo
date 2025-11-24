/***************************************************************************
 *                                  MidgardTownJoinGump.cs
 *                            		----------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownJoinGump : TownGump
    {
        public enum Buttons
        {
            DoJoin = 1
        }

        #region design variables
        protected override int NumLabels { get { return 7; } }
        protected override int NumButtons { get { return 1; } }
        protected override int MainWindowWidth { get { return 500; } }
        protected override int HorBorder { get { return 20; } }
        #endregion

        #region costruttori
        public TownJoinGump( TownSystem system, Mobile owner )
            : base( system, owner, 100, 100 )
        {
            Owner.CloseGump( typeof( TownJoinGump ) );

            Design();

            base.RegisterUse( typeof( TownJoinGump ) );

            //AddPage( 0 );
            //AddBackground( 64, 45, 532, 384, 9250 );
            //AddBackground( 81, 60, 501, 353, 9350 );
            //AddBackground( 100, 140, 457, 200, 2620 );

            //AddImage( 12, 14, 10440 );
            //AddImage( 564, 14, 10441 );
            //AddImage( 213, 70, 83 );
            //AddImage( 250, 100, 96 );
            //AddImage( 450, 90, 97 );
            //AddImage( 220, 90, 95 );
            //AddImage( 450, 70, 85 );
            //AddImage( 214, 109, 89 );
            //AddImage( 450, 110, 91 );

            //AddLabel( 250, 80, 2224, "Midgard Town Join Stone" );

            //if( Town.Definition.LocalizedWelcomeMessage > 0 )
            //    AddHtmlLocalized( 106, 146, 444, 188, Town.Definition.LocalizedWelcomeMessage, 0, false, true );
            //else
            //    AddHtml( 106, 146, 444, 188, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", OldGold32, Town.Definition.WelcomeMessage ), false, true );

            //AddButton( 440, 350, 247, 248, (int)Buttons.DoJoin, GumpButtonType.Reply, 0 );
            //AddLabel( 160, 350, 37, "Are you sure you would to join this city?" );
        }
        #endregion

        #region metodi
        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 300, String.Format( "{0} Join Gump", Town.Definition.TownName ) );

            AddMainHtmlWindow( String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", OldGold32, Town.WelcomeMessage ) );

            //AddImage( -50, -60, 10440 );
            //AddImage( GetBackGroundWidth() - 35, -60, 10441 );

            // buttons
            AddActionButton( 1, "would you like to join this city?", (int)Buttons.DoJoin, Owner, (int) TownAccessFlags.None );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( info.ButtonID == (int)Buttons.DoJoin )
                TownHelper.DoAccountJoin( Owner, Town );
        }

        public override bool HasAccess( int flag )
        {
            return TownSystem.Find( Owner ) == null;
        }
        #endregion
    }
}