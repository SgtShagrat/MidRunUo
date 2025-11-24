/***************************************************************************
 *                               Dies Irae - StartLocationGump.cs
 *
 *   begin                : 07 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
    public class StartLocationGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register( "TestLocationGump", AccessLevel.Counselor, new CommandEventHandler( SendLocationGump_OnCommand ) );
        }

        [Usage( "TestLocationGump" )]
        [Description( "Sends the start location gump to command user" )]
        private static void SendLocationGump_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new StartLocationGump() );
        }

        private class StartInfo
        {
            private static int PickerId = 2103;

            private string Name { get; set; }
            private int X { get; set; }
            private int Y { get; set; }
            private Point3D Location { get; set; }

            public static StartInfo[] List
            {
                get { return m_List; }
            }

            private StartInfo( string name, int x, int y, Point3D location )
            {
                Name = name;
                X = x;
                Y = y;
                Location = location;
            }

            public void Teleport( Mobile m )
            {
                m.MoveToWorld( Location, Map.Felucca );
                m.SendMessage( "Thou are welcome in {0}!", Name );
            }

            public void DrawButton( Gump g, int index )
            {
                g.AddButton( X, Y, PickerId, PickerId, index, GumpButtonType.Reply, 0 );
            }

           private static StartInfo[] m_List = new StartInfo[]
            {
                new StartInfo( "Yew",       198,  89, new Point3D(  535,  992,  0 ) ),   
                new StartInfo( "Minoc",     335, 176, new Point3D( 2477,  399, 15 ) ), 
                new StartInfo( "Britian",   206, 196, new Point3D( 1504, 1620, 21 ) ), 
                new StartInfo( "Moonglow",  400, 339, new Point3D( 4403, 1155,  0 ) ), 
                new StartInfo( "Trinsic",   137, 296, new Point3D( 1840, 2729,  0 ) ), 
                new StartInfo( "Magincia",  294, 343, new Point3D( 3703, 2194,  20 ) ), 
                new StartInfo( "Jhelom",     41, 334, new Point3D( 1364, 3822,  0 ) ), 
                new StartInfo( "Skara Brae",116, 166, new Point3D(  612, 2242,  0 ) ), 
                new StartInfo( "Vesper",    358, 219, new Point3D( 2778,  971,  0 ) ),

                new StartInfo( "Ocllo",     257, 373, new Point3D( 3655,  2541,  0 ) ), 
                new StartInfo( "Serpent's Hold",150, 389, new Point3D( 3014,  3460,  15 ) ), 
                new StartInfo( "Dal-Baraz", 319, 119, new Point3D( 1953,  283,  8 ) ), 
                new StartInfo( "Buccaneer's Den",228, 292, new Point3D( 2682,  2236,  2 ) ), 
                new StartInfo( "Calen Sul", 151, 157, new Point3D( 787,  1990,  0 ) ), 
                new StartInfo( "Vinyamar",  430, 251, new Point3D( 3910,  415,  0 ) ),
                new StartInfo( "Darklore",  249,  64, new Point3D( 1011,  366,  0 ) ),
                new StartInfo( "WolfsBane", 204, 138, new Point3D( 1014,  1387,  0 ) ), 
            };
        }

        public StartLocationGump()
            : base( 50, 50 )
        {
            Closable = false;
            Dragable = true;//bug allo start c'e' tutta la storia del nuovo pg che salta.

            AddPage( 0 );
            AddImage( 0, 0, 184 );

            for( int i = 0; i < StartInfo.List.Length; i++ )
                StartInfo.List[ i ].DrawButton( this, i + 1 );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;
            if( from == null )
                return;

            if( info.ButtonID == 0 )
            {
                from.SendMessage( "You cannot close this gumo without a valid choice." );
                from.SendGump( new StartLocationGump() );
                return;
            }

            int realIndex = info.ButtonID - 1;

            if( realIndex > -1 && realIndex < StartInfo.List.Length )
            {
                StartInfo.List[ realIndex ].Teleport( from );
            }
        }
    }
}