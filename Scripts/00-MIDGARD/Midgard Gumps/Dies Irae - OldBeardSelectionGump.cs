using System;

using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Gumps
{
    public class OldHairSelectionGump : Gump
    {
        private const int Fields = 10;

        #region tables
        private static HairStyleInfo[] m_HairTable = new HairStyleInfo[]
                                                 {
                                                     #region first page
                                                     new HairStyleInfo( "Short", "Short", 0x203B, 0x203B, 0xed1c, 0xC60C ),
                                                     new HairStyleInfo( "Long", "Long", 0x203C, 0x203C, 0xed1d, 0xc60d ),
                                                     new HairStyleInfo( "Ponytail", "Ponytail", 0x203D, 0x203D, 0xed1e, 0xc60e ),
                                                     new HairStyleInfo( "Mohawk", "Mohawk", 0x2044, 0x2044, 0xed27, 0xC60F ),
                                                     new HairStyleInfo( "Pageboy", "Pageboy", 0x2045, 0x2045, 0xED26, 0xED26 ),
                
                                                     new HairStyleInfo( "Buns", "Receding", 0x2046, 0x2048, 0xed28, 0xEDE5 ),
                                                     new HairStyleInfo( "2-tails", "2-tails", 0x2049, 0x2049, 0xede6, 0xede6 ),
                                                     new HairStyleInfo( "Topknot", "Topknot", 0x204A, 0x204A, 0xED29, 0xED29 ),
                                                     new HairStyleInfo( "Curly", "Curly", 0x2047, 0x2047, 0xed25, 0xc618 ),
                                                     new HairStyleInfo( "Style 1", 0x3DBB, 0xC630 ),
                                                     #endregion

                                                     #region second page
                                                     new HairStyleInfo( "Style 2", 0x3DBA, 0xC62F ),
                                                     new HairStyleInfo( "Style 3", 0x3DB9, 0xC62D ),
                                                     new HairStyleInfo( "Style 4", 0x3361, 0xC71B ),
                                                     new HairStyleInfo( "Style 5", 0x336B, 0xC6E4 ),
                                                     new HairStyleInfo( "Style 6", 0x336C, 0xC6E6 ),
                
                                                     new HairStyleInfo( "Style 7", 0x336D, 0xC6E5 ),
                                                     new HairStyleInfo( "Style 8", 0x336E, 0xC6E7 ),
                                                     new HairStyleInfo( "Style 9", 0x336F, 0xC62A ),
                                                     new HairStyleInfo( "Style 10", 0x3370, 0xC62B ),
                                                     new HairStyleInfo( "Style 11", 0x2637, 0xC59D ),
                                                     #endregion

                                                     #region third page
                                                     new HairStyleInfo( "Style 12", 0x2636, 0xC59E ),
                                                     new HairStyleInfo( "Style 13", 0x3fef, 0xC5B1 ),
                                                     new HairStyleInfo( "Style 14", 0x283, 0xC59F ),
                                                     new HairStyleInfo( "Style 15", 0x3fea, 0xC5AC ),
                                                     new HairStyleInfo( "Style 16", 0x10a7, 0xC72C ),
                                                     new HairStyleInfo( "Style 17", 0x3fec, 0xC6FF ),

                                                     new HairStyleInfo( "Bald", "Bald", 0, 0, 0, 0 ),
                                                     #endregion
                                                 };

        int[][] LayoutArray =
            {
                // bgX, bgY, htmlX, htmlY, imgX, imgY, butX, butY 
                #region first page
                new int[] { 235, 060, 150, 075, 168, 020, 118, 073 },
                new int[] { 235, 115, 150, 130, 168, 070, 118, 128 },
                new int[] { 235, 170, 150, 185, 168, 130, 118, 183 },
                new int[] { 235, 225, 150, 240, 168, 180, 118, 238 },
                new int[] { 235, 280, 150, 295, 168, 230, 118, 293 },

                new int[] { 425, 060, 342, 075, 358, 020, 310, 073 },
                new int[] { 425, 115, 342, 130, 358, 070, 310, 128 },
                new int[] { 425, 170, 342, 185, 358, 130, 310, 183 },
                new int[] { 425, 225, 342, 240, 358, 180, 310, 238 },
                new int[] { 425, 280, 342, 295, 358, 230, 310, 293 },
                #endregion
				
                #region second page
                new int[] { 235, 060, 150, 075, 168, 020, 118, 073 },
                new int[] { 235, 115, 150, 130, 168, 070, 118, 128 },
                new int[] { 235, 170, 150, 185, 168, 130, 118, 183 },
                new int[] { 235, 225, 150, 240, 168, 180, 118, 238 },
                new int[] { 235, 280, 150, 295, 168, 230, 118, 293 },

                new int[] { 425, 060, 342, 075, 358, 020, 310, 073 },
                new int[] { 425, 115, 342, 130, 358, 070, 310, 128 },
                new int[] { 425, 170, 342, 185, 358, 130, 310, 183 },
                new int[] { 425, 225, 342, 240, 358, 180, 310, 238 },
                new int[] { 425, 280, 342, 295, 358, 230, 310, 293 },
                #endregion

                #region third page
                new int[] { 235, 060, 150, 075, 168, 020, 118, 073 },
                new int[] { 235, 115, 150, 130, 168, 070, 118, 128 },
                new int[] { 235, 170, 150, 185, 168, 130, 118, 183 },
                new int[] { 235, 225, 150, 240, 168, 180, 118, 238 },
                new int[] { 235, 280, 150, 295, 168, 230, 118, 293 },

                new int[] { 425, 060, 342, 075, 358, 020, 310, 073 },
                new int[] { 425, 115, 342, 130, 358, 070, 310, 128 },
                new int[] { 425, 170, 342, 185, 358, 130, 310, 183 },
                new int[] { 425, 225, 342, 240, 358, 180, 310, 238 },
                new int[] { 425, 280, 342, 295, 358, 230, 310, 293 },
                #endregion
            };
        #endregion

        private readonly HairStylist m_Stylist;
        private readonly Mobile m_From;
        private int m_Page;

        public OldHairSelectionGump( Mobile from, HairStylist stylist )
            : this( from, stylist, 1 )
        {
        }

        private OldHairSelectionGump( Mobile from, HairStylist stylist, int page )
            : base( 50, 50 )
        {
            m_Stylist = stylist;
            m_From = from;
            m_Page = page;

            from.CloseGump( typeof( OldHairSelectionGump ) );

            Design();
        }

        private void Design()
        {
            AddPage( 0 );

            AddBackground( 100, 10, 400, 385, 0xA28 );

            if( m_Page > 1 )
                AddButton( 145, 340, 5603, 5607, 200, GumpButtonType.Reply, 0 ); // Previous page

            if( m_Page < Math.Ceiling( m_HairTable.Length / 10.0 ) )
                AddButton( 175, 340, 5601, 5605, 300, GumpButtonType.Reply, 0 ); // Next Page

            AddOldHtml( 100, 25, 400, 35, "<CENTER>HAIRSTYLE SELECTION MENU</center>" );

            int indMax = ( m_Page * Fields ) - 1;
            int indMin = ( m_Page * Fields ) - Fields;
            int indTemp = 0;

            for( int i = 0; i < m_HairTable.Length; i++ )
            {
                if( i >= indMin && i <= indMax )
                {
                    HairStyleInfo info = m_HairTable[ i ];
                    AddOldHtml( LayoutArray[ indTemp ][ 2 ], LayoutArray[ indTemp ][ 3 ], ( indTemp == 0 ) ? 125 : 80, ( indTemp == 0 ) ? 70 : 35, info.GetName( m_From ) );

                    int image = info.GetImage( m_From );
                    if( LayoutArray[ indTemp ][ 4 ] != 0 && image > 0 )
                    {
                        AddBackground( LayoutArray[ indTemp ][ 0 ], LayoutArray[ indTemp ][ 1 ], 50, 50, 0xA3C );
                        AddImage( LayoutArray[ indTemp ][ 4 ], LayoutArray[ indTemp ][ 5 ], image );
                    }

                    AddButton( LayoutArray[ indTemp ][ 6 ], LayoutArray[ indTemp ][ 7 ], 0xFA5, 0xFA7, i + 1, GumpButtonType.Reply, 0 );
                    indTemp++;
                }
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( info.ButtonID == 200 ) // Previous page
            {
                m_Page--;
                m_From.SendGump( new OldHairSelectionGump( m_From, m_Stylist, m_Page ) );
            }
            else if( info.ButtonID == 300 )  // Next Page
            {
                m_Page++;
                m_From.SendGump( new OldHairSelectionGump( m_From, m_Stylist, m_Page ) );
            }
            else if( info.ButtonID > 0 )
            {
                HairStyleInfo hairInfo = m_HairTable[ info.ButtonID - 1 ];
                m_Stylist.StartWorking( m_From, hairInfo );
            }
        }
    }

    public class OldBeardSelectionGump : Gump
    {
        private const int Fields = 10;

        #region tables
        private static BeardStyleInfo[] m_BeardTable = new BeardStyleInfo[]
                                                         {
                                                             new BeardStyleInfo( "Long", 0x203E, 50801 ),
                                                             new BeardStyleInfo( "Short", 0x203F, 50802 ),
                                                             new BeardStyleInfo( "Goatee", 0x2040, 50800 ),
                                                             new BeardStyleInfo( "Mustache", 0x2041, 50808 ),
                                                             new BeardStyleInfo( "Medium Short", 0x204B, 50904 ),
                                                             new BeardStyleInfo( "Medium Long", 0x204C, 50905 ),
                                                             new BeardStyleInfo( "Vandyke", 0x204D, 50906 ),
                                                             new BeardStyleInfo( "None", 0, 0)
                                                         };

        int[][] LayoutArray =
            {
                // bgX, bgY, htmlX, htmlY, imgX, imgY, butX, butY 
                #region first page
                new int[] { 235, 060, 150, 075, 168, 020, 118, 073 },
                new int[] { 235, 115, 150, 130, 168, 070, 118, 128 },
                new int[] { 235, 170, 150, 185, 168, 130, 118, 183 },
                new int[] { 235, 225, 150, 240, 168, 180, 118, 238 },
                new int[] { 235, 280, 150, 295, 168, 230, 118, 293 },

                new int[] { 425, 060, 342, 075, 358, 020, 310, 073 },
                new int[] { 425, 115, 342, 130, 358, 070, 310, 128 },
                new int[] { 425, 170, 342, 185, 358, 130, 310, 183 },
                new int[] { 425, 225, 342, 240, 358, 180, 310, 238 },
                new int[] { 425, 280, 342, 295, 358, 230, 310, 293 },
                #endregion
            };
        #endregion

        private readonly HairStylist m_Stylist;
        private readonly Mobile m_From;
        private int m_Page;

        public OldBeardSelectionGump( Mobile from, HairStylist stylist )
            : this( from, stylist, 1 )
        {
        }

        private OldBeardSelectionGump( Mobile from, HairStylist stylist, int page )
            : base( 50, 50 )
        {
            m_Stylist = stylist;
            m_From = from;
            m_Page = page;

            from.CloseGump( typeof( OldBeardSelectionGump ) );

            Design();
        }

        private void Design()
        {
            AddPage( 0 );

            AddBackground( 100, 10, 400, 385, 0xA28 );

            if( m_Page > 1 )
                AddButton( 145, 340, 5603, 5607, 200, GumpButtonType.Reply, 0 ); // Previous page

            if( m_Page < Math.Ceiling( m_BeardTable.Length / 10.0 ) )
                AddButton( 175, 340, 5601, 5605, 300, GumpButtonType.Reply, 0 ); // Next Page

            AddOldHtml( 100, 25, 400, 35, "<CENTER>BEARDSTYLE SELECTION MENU</center>" );

            int indMax = ( m_Page * Fields ) - 1;
            int indMin = ( m_Page * Fields ) - Fields;
            int indTemp = 0;

            for( int i = 0; i < m_BeardTable.Length; i++ )
            {
                if( i >= indMin && i <= indMax )
                {
                    BeardStyleInfo info = m_BeardTable[ i ];
                    AddOldHtml( LayoutArray[ indTemp ][ 2 ], LayoutArray[ indTemp ][ 3 ], ( indTemp == 0 ) ? 125 : 80, ( indTemp == 0 ) ? 70 : 35, info.MaleName );

                    if( LayoutArray[ indTemp ][ 4 ] != 0 && info.MaleImage > 0 )
                    {
                        AddBackground( LayoutArray[ indTemp ][ 0 ], LayoutArray[ indTemp ][ 1 ], 50, 50, 0xA3C );
                        AddImage( LayoutArray[ indTemp ][ 4 ], LayoutArray[ indTemp ][ 5 ], info.MaleImage );
                    }

                    AddButton( LayoutArray[ indTemp ][ 6 ], LayoutArray[ indTemp ][ 7 ], 0xFA5, 0xFA7, i + 1, GumpButtonType.Reply, 0 );
                    indTemp++;
                }
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( info.ButtonID == 200 ) // Previous page
            {
                m_Page--;
                m_From.SendGump( new OldBeardSelectionGump( m_From, m_Stylist, m_Page ) );
            }
            else if( info.ButtonID == 300 )  // Next Page
            {
                m_Page++;
                m_From.SendGump( new OldBeardSelectionGump( m_From, m_Stylist, m_Page ) );
            }
            else if( info.ButtonID > 0 )
            {
                BeardStyleInfo hairInfo = m_BeardTable[ info.ButtonID - 1 ];
                m_Stylist.StartWorking( m_From, hairInfo );
            }
        }
    }

    public class HairStyleInfo
    {
        private string MaleName { get; set; }
        private int FemaleItemID { get; set; }
        private int MaleItemID { get; set; }
        private string FemaleName { get; set; }
        private int MaleImage { get; set; }
        private int FemaleImage { get; set; }

        public HairStyleInfo( string femaleName, string maleName, int femaleItemID, int maleItemID, int femaleImage, int maleImage )
        {
            MaleName = maleName;
            FemaleItemID = femaleItemID;
            MaleItemID = maleItemID;
            FemaleName = femaleName;
            MaleImage = maleImage;
            FemaleImage = femaleImage;
        }

        public HairStyleInfo( string name, int itemID, int image )
            : this( name, name, itemID, itemID, image, image )
        {
        }

        public string GetName( Mobile m )
        {
            return m.Female ? FemaleName : MaleName;
        }

        public int GetImage( Mobile m )
        {
            return m.Female ? FemaleImage : MaleImage;
        }

        public int GetItemID( Mobile m )
        {
            return m.Female ? FemaleItemID : MaleItemID;
        }
    }

    public class BeardStyleInfo
    {
        public string MaleName { get; set; }
        public int MaleItemID { get; set; }
        public int MaleImage { get; set; }

        public BeardStyleInfo( string maleName, int maleItemID, int maleImage )
        {
            MaleName = maleName;
            MaleItemID = maleItemID;
            MaleImage = maleImage;
        }
    }

    public class CutHair : Item
    {
        [Constructable]
        public CutHair()
            : this( 0xDFE )
        {
        }

        [Constructable]
        public CutHair( int itemID )
            : base( itemID )
        {
            Movable = false;

            new InternalTimer( this ).Start();
        }

        public CutHair( Serial serial )
            : base( serial )
        {
            new InternalTimer( this ).Start();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        private class InternalTimer : Timer
        {
            private Item m_Item;

            public InternalTimer( Item item )
                : base( TimeSpan.FromSeconds( 15.0 ) )
            {
                Priority = TimerPriority.OneSecond;

                m_Item = item;
            }

            protected override void OnTick()
            {
                m_Item.Delete();
            }
        }
    }
}