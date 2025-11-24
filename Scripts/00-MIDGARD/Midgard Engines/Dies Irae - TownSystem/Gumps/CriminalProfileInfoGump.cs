using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class CriminalProfileInfoGump : TownGump
    {
        public enum Buttons
        {
            Close,
            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 10; } }
        protected override int NumButtons { get { return 0; } }
        protected override int MainWindowWidth { get { return 470; } }
        protected override bool HasSubtitles { get { return true; } }
        #endregion

        private readonly List<TownCrime> m_Crimes;

        public CriminalProfileInfoGump( TownSystem system, Mobile owner, CriminalProfile profile )
            : this( system, owner, profile, null )
        {
        }

        public CriminalProfileInfoGump( TownSystem system, Mobile owner, CriminalProfile profile, IEnumerable<TownCrime> list )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( CriminalProfileInfoGump ) );

            m_Crimes = list == null ? BuildList( profile ) : new List<TownCrime>( list );

            Design();

            base.RegisterUse( typeof( CriminalProfileInfoGump ) );
        }

        private static List<TownCrime> BuildList( CriminalProfile profile )
        {
            List<TownCrime> list = new List<TownCrime>();
            if( profile.Crimes == null )
                return list;

            foreach( TownCrime crime in profile.Crimes )
            {
                if( crime != null )
                    list.Add( crime );
            }

            list.Sort( AgeCrimeComparer.Instance );

            return list;
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 175, "Profile Info" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Crime" );
            AddSubTitle( labelOffsetX + 170, "Detected" );
            AddSubTitle( labelOffsetX + 270, "Status" );
            AddSubTitle( labelOffsetX + 370, "Until" );

            AddCloseButton();

            if( m_Crimes == null || m_Crimes.Count == 0 )
            {
                AddLabel( labelOffsetX, labelOffsetY, DefaultValueHue, "- no crime found -" );
                return;
            }

            for( int i = 0; i < m_Crimes.Count; i++ )
            {
                TownCrime townCrime = m_Crimes[ i ];
                if( townCrime == null )
                    continue;

                int page = i / NumLabels;
                int pos = i % NumLabels;

                if( pos == 0 )
                {
                    if( page > 0 )
                        AddButton( 460, 10, 0x15E1, 0x15E5, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                    AddPage( page + 1 );

                    if( page > 0 )
                        AddButton( 440, 10, 0x15E3, 0x15E7, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                }

                int y = pos * LabelsOffset + labelOffsetY;

                string status = "unknown";
                int hue = DisabledHue;
                string until = "unknown";
                if( townCrime.Condamned )
                {
                    status = "condemned";
                    hue = DisabledHue;
                    until = townCrime.Condemn.ExpirationTime.ToString( "dd'-'MM HH':'mm" );
                }
                else if( townCrime.ExpirationTime > DateTime.Now )
                {
                    status = "catchable";
                    hue = LabelsHue;
                    until = townCrime.ExpirationTime.ToString( "dd'-'MM HH':'mm" );
                }
                AddLabel( labelOffsetX, y, hue, townCrime.DefaultName );
                AddLabel( labelOffsetX + 170, y, hue, townCrime.DateOfCrime.ToString( "dd'-'MM HH':'mm" ) );
                AddLabel( labelOffsetX + 270, y, hue, status );
                AddLabel( labelOffsetX + 370, y, hue, until );
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            sender.Mobile.SendGump( new CriminalProfilesGump( Town, sender.Mobile ) );
        }
    }
}