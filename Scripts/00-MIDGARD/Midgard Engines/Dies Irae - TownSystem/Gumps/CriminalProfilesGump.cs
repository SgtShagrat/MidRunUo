/***************************************************************************
 *                                  CriminalProfilesGump.cs
 *
 *  begin                	: Maggio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class CriminalProfilesGump : TownGump
    {
        public enum Buttons
        {
            Close,
            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 20; } }
        protected override int NumButtons { get { return 0; } }
        protected override int MainWindowWidth { get { return 500; } }
        protected override bool HasSubtitles { get { return true; } }
        #endregion

        private readonly List<CriminalProfile> m_Profiles;

        public CriminalProfilesGump( TownSystem system, Mobile owner )
            : this( system, owner, null )
        {
        }

        public CriminalProfilesGump( TownSystem system, Mobile owner, IEnumerable<CriminalProfile> list )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( CriminalProfilesGump ) );

            m_Profiles = list == null ? BuildList( system ) : new List<CriminalProfile>( list );

            Design();

            base.RegisterUse( typeof( CriminalProfilesGump ) );
        }

        private class InternalComparer : IComparer<CriminalProfile>
        {
            public static readonly IComparer<CriminalProfile> Instance = new InternalComparer();

            public int Compare( CriminalProfile x, CriminalProfile y )
            {
                if( x == null || y == null )
                    return 0;

                if( x.Criminal == null || y.Criminal == null )
                    return 0;

                return Insensitive.Compare( x.Criminal.Name, y.Criminal.Name );
            }
        }

        private static List<CriminalProfile> BuildList( TownSystem system )
        {
            List<CriminalProfile> list = new List<CriminalProfile>();
            if( system.CriminalProfiles == null )
                return list;

            foreach( CriminalProfile profile in system.CriminalProfiles )
            {
                if( profile != null && profile.Criminal != null )
                    list.Add( profile );
            }

            list.Sort( InternalComparer.Instance );

            return list;
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 315, String.Format( "{0} Criminal Profiles", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Name" );
            AddSubTitle( labelOffsetX + 170, "Stealing" );
            AddSubTitle( labelOffsetX + 250, "Murders" );
            AddSubTitle( labelOffsetX + 330, "Wanted Until" );

            AddSubTitleImage( labelOffsetX + 450, 4033 ); // mini scroll button

            AddCloseButton();

            if( m_Profiles == null || m_Profiles.Count == 0 )
            {
                AddLabel( labelOffsetX, labelOffsetY, DefaultValueHue, "- no criminal profile found -" );
                return;
            }

            for( int i = 0; i < m_Profiles.Count; i++ )
            {
                CriminalProfile criminalProfile = m_Profiles[ i ];
                if( criminalProfile == null )
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

                Mobile criminal = criminalProfile.Criminal;

                int hue = criminalProfile.CatchableUntil > DateTime.Now ? LabelsHue : DisabledHue;

                AddLabel( labelOffsetX, y, hue, criminal.Name );
                AddLabel( labelOffsetX + 170, y, hue, criminalProfile.GetCrimesByType( CrimeType.StealAction ).ToString() );
                AddLabel( labelOffsetX + 250, y, hue, criminalProfile.GetCrimesByType( CrimeType.ReportedAsMurderer ).ToString() );

                if( criminalProfile.IsUnderCondamn )
                    AddLabel( labelOffsetX + 330, y, 37, "-jailed-" );
                else if( criminalProfile.IsCatchable && criminalProfile.CatchableUntil > DateTime.MinValue )
                    AddLabel( labelOffsetX + 330, y, hue, criminalProfile.CatchableUntil.ToString( "dd'-'MM HH':'mm" ) );

                AddMainWindowButton( labelOffsetX + 450, y + 3, i + 1, (int) TownAccessFlags.BanCitizen );
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == (int)Buttons.Close )
            {
                from.SendGump( new TownSystemInfoGump( Town, from ) );
                return;
            }

            if( info.ButtonID < 0 || info.ButtonID > m_Profiles.Count )
                return;

            from.SendGump( new CriminalProfileInfoGump( Town, Owner, m_Profiles[ info.ButtonID - 1 ] ) );
        }
    }
}