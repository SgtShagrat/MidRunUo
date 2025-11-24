using Server;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;

namespace Midgard.Engines.WineCrafting
{
    public class VinyardGroundGump : Gump
    {
        private const int EntryCount = 3;

        private BaseAddonDeed m_Deed;
        private IPoint3D m_P3D;
        private Map m_Map;
        private int m_Width;
        private int m_Height;

        public VinyardGroundGump( BaseAddonDeed deed, IPoint3D p, Map map, int width, int height )
            : base( 30, 30 )
        {
            m_Deed = deed;
            m_P3D = p;
            m_Map = map;
            m_Width = width;
            m_Height = height;

            AddPage( 0 );

            AddBackground( 0, 0, 450, 160, 9250 );

            AddAlphaRegion( 12, 12, 381, 22 );
            AddHtml( 13, 13, 379, 20, "<BASEFONT COLOR=WHITE>Choose a ground type</BASEFONT>", false, false );

            AddAlphaRegion( 398, 12, 40, 22 );
            AddAlphaRegion( 12, 39, 426, 109 );

            AddImage( 400, 16, 9766 );
            AddImage( 420, 16, 9762 );
            AddPage( 1 );

            int page = 1;

            for( int i = 0, index = 0; i < VinyardGroundInfo.Infos.Length; ++i, ++index )
            {
                if( index >= EntryCount )
                {
                    if( ( EntryCount * page ) == EntryCount )
                        AddImage( 400, 16, 0x2626 );

                    AddButton( 420, 16, 0x15E1, 0x15E5, 0, GumpButtonType.Page, page + 1 );

                    ++page;
                    index = 0;

                    AddPage( page );

                    AddButton( 400, 16, 0x15E3, 0x15E7, 0, GumpButtonType.Page, page - 1 );

                    if( ( VinyardGroundInfo.Infos.Length - ( EntryCount * page ) ) < EntryCount )
                        AddImage( 420, 16, 0x2622 );
                }

                VinyardGroundInfo info = VinyardGroundInfo.GetInfo( i );

                for( int j = 0; j < info.Entries.Length; ++j )
                {
                    if( info.Entries[ j ].OffsetX >= 0 && info.Entries[ j ].OffsetY >= 0 )
                        AddItem( 20 + ( index * 140 ) + info.Entries[ j ].OffsetX, 46 + info.Entries[ j ].OffsetY, info.Entries[ j ].ItemID );
                }

                AddButton( 20 + ( index * 140 ), 46, 1209, 1210, i + 1, GumpButtonType.Reply, 0 );
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID >= 1 )
            {
                BaseAddon addon = new VinyardGroundAddon( info.ButtonID - 1, m_Width, m_Height );

                Server.Spells.SpellHelper.GetSurfaceTop( ref m_P3D );
                BaseHouse houses = null;

                AddonFitResult res = addon.CouldFit( m_P3D, m_Map, from, ref houses );

                if( res == AddonFitResult.Valid )
                    addon.MoveToWorld( new Point3D( m_P3D ), m_Map );
                else if( res == AddonFitResult.Blocked )
                    from.SendLocalizedMessage( 500269 ); // You cannot build that there.
                else if( res == AddonFitResult.NotInHouse )
                    from.SendLocalizedMessage( 500274 ); // You can only place this in a house that you own!
                else if( res == AddonFitResult.DoorsNotClosed )
                    from.SendMessage( "You must close all house doors before placing this." );

                if( res == AddonFitResult.Valid )
                {
                    m_Deed.Delete();

                    if( houses != null )
                    {
                        houses.Addons.Add( addon );

                        from.SendGump( new VinyardGroundPlacedGump( m_Deed ) );
                    }
                }
                else
                {
                    addon.Delete();
                }
            }
        }
    }
}