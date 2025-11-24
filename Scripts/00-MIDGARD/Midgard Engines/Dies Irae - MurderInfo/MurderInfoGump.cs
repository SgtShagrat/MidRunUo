/***************************************************************************
 *                                  MurdersInfoGump.cs
 *                            		------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MurderInfo
{
    public class MurdersInfoGump : Gump
    {
        public enum GumpType
        {
            Killers,
            Victims
        }

        private enum Buttons
        {
            Close,

            PreviousPage,
            NextPage,

            Switch
        }

        #region fields
        private const int m_Fields = 20;
        private const int m_HueTit = 15;
        private Mobile m_Owner;
        private List<MurderInfo> m_Infoes;
        private int m_Page;
        private GumpType m_GumpType;
        #endregion

        #region constructors
        public MurdersInfoGump( Mobile owner )
            : this( owner, BuildList( owner, GumpType.Killers ), 1, GumpType.Killers )
        {
        }

        public MurdersInfoGump( Mobile owner, List<MurderInfo> infoes, int page, GumpType gumpType )
            : base( 50, 50 )
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            owner.CloseGump( typeof( MurdersInfoGump ) );

            m_Owner = owner;
            m_Infoes = infoes;
            m_GumpType = gumpType;

            Initialize( page );
        }
        #endregion

        #region members
        private void AddBlackAlpha( int x, int y, int width, int height )
        {
            AddImageTiled( x, y, width, height, 2624 );
            AddAlphaRegion( x, y, width, height );
        }

        private void Initialize( int page )
        {
            m_Page = page;

            AddPage( 0 );
            AddBackground( 0, 0, 310, 505, 9350 );
            AddBlackAlpha( 10, 10, 290, 485 );

            AddBlackAlpha( 20, 10, 270, 20 );
            AddLabel( 30, 10, m_HueTit, m_GumpType == GumpType.Killers ? "Killers list:" : "Victims list:" );

            AddBlackAlpha( 20, 470, 270, 20 );
            AddButton( 30, 475, 2103, 2103, (int)Buttons.Switch, GumpButtonType.Reply, 0 ); // switch gump
            AddLabel( 50, 470, m_HueTit, m_GumpType == GumpType.Killers ? "switch to victims info" : "switch to killers info" );

            if( m_Infoes == null || m_Infoes.Count == 0 )
            {
                AddLabelCropped( 13, 33, 150, 22, 37, "no info available" );
                return;
            }

            if( m_Page > 1 )
                AddButton( 250, 13, 0x15E3, 0x15E7, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0 );

            if( m_Page < Math.Ceiling( m_Infoes.Count / (double)m_Fields ) )
                AddButton( 270, 13, 0x15E1, 0x15E5, (int)Buttons.NextPage, GumpButtonType.Reply, 0 );

            int indMax = ( m_Page * m_Fields ) - 1;
            int indMin = ( m_Page * m_Fields ) - m_Fields;
            int indTemp = 0;

            for( int i = 0; i < m_Infoes.Count; ++i )
            {
                if( i >= indMin && i <= indMax )
                {
                    MurderInfo info = m_Infoes[ i ];

                    if( info != null )
                    {
                        int hue = GetHue( info );

                        if( m_GumpType == GumpType.Killers )
                            AddLabelCropped( 13, 33 + ( indTemp * 22 ), 150, 22, hue, info.Victim.Name );
                        else
                            AddLabelCropped( 13, 33 + ( indTemp * 22 ), 150, 22, hue, info.Killer.Name );

                        AddLabelCropped( 163, 33 + ( indTemp * 22 ), 150, 22, hue, info.TimeOfDeath.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" ) );
                        indTemp++;
                    }
                }
            }
        }

        private static int GetHue( MurderInfo info )
        {
            if( DateTime.Now - info.TimeOfDeath < Config.LamerDelay )
                return 0x3F; // green
            else
                return 0x22; // red
        }

        private static List<MurderInfo> BuildList( Mobile owner, GumpType type )
        {
            List<MurderInfo> list = new List<MurderInfo>();
            if( owner == null || !( owner is Midgard2PlayerMobile ) )
                return list;

            if( type == GumpType.Killers )
                list = MurderInfoPersistance.GetMurderInfoForKiller( owner );
            else
                list = MurderInfoPersistance.GetMurderInfoForVictim( owner );

            if( list == null || list.Count < 1 )
                return list;

            for( int i = 0; i < list.Count; i++ )
            {
                if( !MurderInfoHelper.IsValidInfo( list[ i ] ) )
                    list.Remove( list[ i ] );
            }

            try
            {
                list.Sort( MurderInfo.MurderInfoesComparer.Instance );
            }
            catch { }

            return list;
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.Close:
                    break;
                case (int)Buttons.NextPage:
                    m_Page++;
                    from.SendGump( new MurdersInfoGump( from, m_Infoes, m_Page, m_GumpType ) );
                    break;
                case (int)Buttons.PreviousPage:
                    m_Page--;
                    from.SendGump( new MurdersInfoGump( from, m_Infoes, m_Page, m_GumpType ) );
                    break;
                case (int)Buttons.Switch:
                    GumpType type = ( m_GumpType == GumpType.Killers ? GumpType.Victims : GumpType.Killers );
                    from.SendGump( new MurdersInfoGump( from, BuildList( m_Owner, type ), 1, type ) );
                    break;
                default:
                    break;

            }
        }
        #endregion
    }
}