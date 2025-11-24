using System;
using System.Collections.Generic;
using System.IO;

using Midgard.Engines.Classes;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

using Skills = Server.Skills;

namespace Midgard.Commands
{
    public class AdminUty
    {
        public static void Initialize()
        {
            CommandSystem.Register( "FindLamer", AccessLevel.Developer, new CommandEventHandler( FindLamer_OnCommand ) );
            CommandSystem.Register( "FindRaces", AccessLevel.Developer, new CommandEventHandler( FindRaces_OnCommand ) );
            CommandSystem.Register( "FindClass", AccessLevel.Developer, new CommandEventHandler( FindClass_OnCommand ) );
            CommandSystem.Register( "DeleteInactiveAccounts", AccessLevel.Developer, new CommandEventHandler( DeleteInactiveAccounts_OnCommand ) );
            CommandSystem.Register( "GotoSerial", AccessLevel.Developer, new CommandEventHandler( GotoSerial_OnCommand ) );
            CommandSystem.Register( "FindMobileByName", AccessLevel.Developer, new CommandEventHandler( FindMobileByName_OnCommand ) );
            CommandSystem.Register( "FindItemByName", AccessLevel.Developer, new CommandEventHandler( FindItemByName_OnCommand ) );
            CommandSystem.Register( "FindMobileByType", AccessLevel.Developer, new CommandEventHandler( FindMobileByType_OnCommand ) );
            CommandSystem.Register( "FindItemByType", AccessLevel.Developer, new CommandEventHandler( FindItemByType_OnCommand ) );
            CommandSystem.Register( "InternalItems", AccessLevel.Developer, new CommandEventHandler( InternalItems_OnCommand ) );
            CommandSystem.Register( "InternalMobiles", AccessLevel.Developer, new CommandEventHandler( InternalMobiles_OnCommand ) );
        }

        [Usage( "InternalMobiles" )]
        [Description( "Lists all mobiles on Internal map." )]
        private static void InternalMobiles_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new InternalMobilesGump( e.Mobile ) );
        }

        [Usage( "InternalItems" )]
        [Description( "Lists all items on Internal map." )]
        private static void InternalItems_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new InternalItemGump( e.Mobile ) );
        }

        [Usage( "FindLamer" )]
        [Description( "Trova tutti i pg con SkllCap > 7200, Skill > 700, Stat > nn StatCap >" )]
        public static void FindLamer_OnCommand( CommandEventArgs e )
        {
            using( StreamWriter op = new StreamWriter( "Logs/lamer.log" ) )
            {
                op.WriteLine( "## Lamer List generated on {0} ##", DateTime.Now );
                op.WriteLine( "#################################################" );
                op.WriteLine();
                op.WriteLine();

                op.WriteLine( "# Lamer:" );
                op.WriteLine();

                foreach( Account acct in Accounts.GetAccounts() )
                {
                    for( int i = 0; i < 6; i++ )
                    {
                        try
                        {
                            Mobile m = acct[ i ];
                            if( m == null )
                                continue;

                            if( m.AccessLevel > AccessLevel.Player )
                                continue;

                            if( m.RawStatTotal > 250 )
                            {
                                op.WriteLine( "#######################################################" );
                                op.WriteLine( "#### Account:	{0} - AccessLevel: {1}", acct.Username, acct.AccessLevel );
                                op.WriteLine( "#===========================================" );
                                op.WriteLine( "# Creato: {0} - Email: {1} - Ultimo Login {2}", acct.Created.ToLongDateString(), acct.Email, acct.LastLogin.ToLongDateString() );
                                op.WriteLine( "#===========================================" );
                                op.WriteLine( "#----------------------------------" );
                                op.WriteLine( "PG: {0} - AccessLevel: {1}", m.Name, m.AccessLevel );
                                op.WriteLine( "StatCap (RAW): {0}", m.RawStatTotal );
                            }
                            if( m.SkillsCap > 7200 )
                            {
                                op.WriteLine( "#######################################################" );
                                op.WriteLine( "#### Account:	{0} - AccessLevel: {1}", acct.Username, acct.AccessLevel );
                                op.WriteLine( "#===========================================" );
                                op.WriteLine( "# Creato: {0} - Email: {1} - Ultimo Login {2}", acct.Created.ToLongDateString(), acct.Email, acct.LastLogin.ToLongDateString() );
                                op.WriteLine( "#===========================================" );
                                op.WriteLine( "#----------------------------------" );
                                op.WriteLine( "PG: {0} - AccessLevel: {1}", m.Name, m.AccessLevel );
                                op.WriteLine( "SkillCap : {0}", m.SkillsCap );
                            }

                            Skills skills = m.Skills;
                            for( int j = 0; j < skills.Length; ++j )
                            {
                                if( skills[ j ].Base > skills[ j ].Cap )
                                {
                                    op.WriteLine( "#######################################################" );
                                    op.WriteLine( "#### Account:	{0} - AccessLevel: {1}", acct.Username, acct.AccessLevel );
                                    op.WriteLine( "#===========================================" );
                                    op.WriteLine( "# Creato: {0} - Email: {1} - Ultimo Login  {2}", acct.Created.ToLongDateString(), acct.Email, acct.LastLogin.ToLongDateString() );
                                    op.WriteLine( "#===========================================" );
                                    op.WriteLine( "#----------------------------------" );
                                    op.WriteLine( "PG: {0} - AccessLevel: {1}", m.Name, m.AccessLevel );
                                    op.WriteLine( "Skill {0} --> Valore: {1} Cap: {2}", skills[ j ].Name, skills[ j ].Base, skills[ j ].Cap );
                                }
                                if( skills[ j ].Cap > 720 )
                                {
                                    op.WriteLine( "#######################################################" );
                                    op.WriteLine( "#### Account:	{0} - AccessLevel: {1}", acct.Username, acct.AccessLevel );
                                    op.WriteLine( "#===========================================" );
                                    op.WriteLine( "# Creato: {0} - Email: {1} - Ultimo Login  {2}", acct.Created.ToLongDateString(), acct.Email, acct.LastLogin.ToLongDateString() );
                                    op.WriteLine( "#===========================================" );
                                    op.WriteLine( "#----------------------------------" );
                                    op.WriteLine( "PG: {0} - AccessLevel: {1}", m.Name, m.AccessLevel );
                                    op.WriteLine( "Skill {0} --> Cap: {1}", skills[ j ].Name, skills[ j ].Cap );
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            e.Mobile.SendMessage( "Object table has been generated. See the file : <runuo root>/Logs/lamer.log" );
        }

        [Usage( "FindRaces" )]
        [Description( "Genera un report con tutti i PG Razziati" )]
        public static void FindRaces_OnCommand( CommandEventArgs e )
        {
            using( StreamWriter op = new StreamWriter( "Logs/RacedPG.log" ) )
            {
                op.WriteLine( "## Race List generated on {0} ##", DateTime.Now );
                op.WriteLine( "#################################################" );
                op.WriteLine();
                op.WriteLine();

                op.WriteLine( "# Raced PGs:" );
                op.WriteLine();
                foreach( Account acct in Accounts.GetAccounts() )
                {
                    for( int i = 0; i < 6; i++ )
                    {
                        try
                        {
                            Mobile m = acct[ i ];
                            if( m == null )
                                continue;

                            if( m.AccessLevel > AccessLevel.Player )
                                continue;

                            if( m.Race != Race.Human )
                                op.WriteLine( "{0} - {1} ( Account: {2} )", m.Race, m.Name, acct.Username );
                        }
                        catch
                        {
                        }
                    }
                }
            }
            e.Mobile.SendMessage( "Object table has been generated. See the file : <runuo root>/Logs/RacedPG.log" );
        }

        [Usage( "FindClass" )]
        [Description( "Genera un report con tutti i PG Classati (eccetto paladini)" )]
        public static void FindClass_OnCommand( CommandEventArgs e )
        {
            using( StreamWriter op = new StreamWriter( "Logs/ClassedPG.log" ) )
            {
                op.WriteLine( "## Class List generated on {0} ##", DateTime.Now );
                op.WriteLine( "#################################################" );
                op.WriteLine();
                op.WriteLine();

                op.WriteLine( "# Classed PG:" );
                op.WriteLine();

                foreach( Account acct in Accounts.GetAccounts() )
                {
                    for( int i = 0; i < 6; i++ )
                    {
                        try
                        {
                            Mobile m = acct[ i ];

                            if( m == null )
                                continue;

                            if( m.AccessLevel > AccessLevel.Player )
                                continue;

                            ClassPlayerState state = ClassPlayerState.Find( m );
                            if( state != null )
                                op.WriteLine( "{0} - {1} ( Account: {2} )", state.ClassSystem.Definition.Class, m.Name, acct.Username );
                        }
                        catch
                        {
                        }
                    }
                }
            }
            e.Mobile.SendMessage( "Object table has been generated. See the file : <runuo root>/Logs/ClassedPG.log" );
        }

        [Usage( "DeleteInactiveAccounts" )]
        [Description( "Cancella gli account inattivi da 180 giorni in su" )]
        private static void DeleteInactiveAccounts_OnCommand( CommandEventArgs e )
        {
            List<Account> results = new List<Account>();
            DateTime minTime = DateTime.Now - TimeSpan.FromDays( 180.0 );

            foreach( Account acct in Accounts.GetAccounts() )
            {
                if( acct.LastLogin <= minTime )
                    results.Add( acct );
            }

            if( results.Count > 0 )
            {
                using( StreamWriter op = new StreamWriter( "Logs/ClassedPG.log" ) )
                {
                    op.WriteLine( "## Account delection List generated on {0} ##", DateTime.Now );
                    op.WriteLine( "#################################################" );
                    op.WriteLine();
                    op.WriteLine();

                    op.WriteLine( "# Accounts Deleted Successfully:" );
                    op.WriteLine();
                    foreach( Account t in results )
                    {
                        Account a = t;
                        a.Delete();
                        op.WriteLine( "Account: {0} - Created: {1}", a.Username, a.LastLogin.ToLongDateString() );
                    }
                    e.Mobile.SendMessage( "Inactive accounts deleted successfully." );
                }
            }
            else
                e.Mobile.SendMessage( "Ther are not inactive accounts." );
        }

        [Usage( "GotoSerial <serial>" )]
        [Description( "Brings you to the object with given serial." )]
        private static void GotoSerial_OnCommand( CommandEventArgs e )
        {
            Serial s = e.GetInt32( 0 );
            IEntity ie = World.FindEntity( s );
            if( ie == null )
                return;
            if( ie is Item )
                e.Mobile.Location = ( (Item)ie ).GetWorldLocation();
            else
                e.Mobile.Location = ie.Location;
        }

        [Usage( "FindMobileByName <name>" )]
        [Description( "Looks for a pc/npc with the given name." )]
        public static void FindMobileByName_OnCommand( CommandEventArgs e )
        {
            try
            {
                List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );

                foreach( Mobile m in mobs )
                {
                    if( m != null && m.Name != null )
                    {
                        if( m.Name.IndexOf( e.ArgString ) >= 0 )
                        {
                            if( m.Map != Map.Internal )
                                e.Mobile.Map = m.Map;
                            e.Mobile.Location = m.Location;
                            return;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        [Usage( "FindItemByName <name>" )]
        [Description( "Looks for an object with the given name." )]
        public static void FindItemByName_OnCommand( CommandEventArgs e )
        {
            try
            {
                foreach( Item i in World.Items.Values )
                {
                    if( i != null && i.Name != null )
                    {
                        if( i.Name.IndexOf( e.ArgString ) >= 0 )
                        {
                            if( i.Map != Map.Internal )
                                e.Mobile.Map = i.Map;
                            e.Mobile.Location = i.GetWorldLocation();
                            return;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        [Usage( "FindMobileByType <type>" )]
        [Description( "Looks for a mobile of the given type." )]
        public static void FindMobileByType_OnCommand( CommandEventArgs e )
        {
            try
            {
                Type t = ScriptCompiler.FindTypeByName( e.ArgString, true );
                List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );

                foreach( Mobile m in mobs )
                {
                    if( m != null && m.GetType() == t )
                    {
                        e.Mobile.Location = m.Location;
                        if( m.Map != Map.Internal )
                            e.Mobile.Map = m.Map;
                        return;
                    }
                }
            }
            catch
            {
            }
        }

        [Usage( "FindItemByType <type>" )]
        [Description( "Looks for an object of the given type." )]
        public static void FindItemByType_OnCommand( CommandEventArgs e )
        {
            Type t = ScriptCompiler.FindTypeByName( e.ArgString, true );
            try
            {
                foreach( Item i in World.Items.Values )
                {
                    if( i != null && i.GetType() == t )
                    {
                        if( i.Map != Map.Internal )
                            e.Mobile.Map = i.Map;
                        e.Mobile.Location = i.GetWorldLocation();
                        return;
                    }
                }
            }
            catch
            {
            }
        }

        #region Nested type: InternalItemGump
        private class InternalItemGump : Gump
        {
            private const int GumpOffsetX = 30;
            private const int GumpOffsetY = 30;

            private const int TextHue = 0;
            private const int TextOffsetX = 2;

            private const int OffsetGumpID = 0x0052; // Pure black 
            private const int HeaderGumpID = 0x0E14; // Dark navy blue, textured 
            private const int EntryGumpID = 0x0BBC; // Light offwhite, textured 
            private const int BackGumpID = 0x13BE; // Gray slate/stoney 
            private const int ButtonGumpID = 0x0E14; // Dark navy blue, textured 

            private const int ButtonWidth = 20;

            // public const int GoOffsetX = 2, GoOffsetY = 2;
            // public const int GoButtonID1 = 0x15E1; // Arrow pointing right 
            // public const int GoButtonID2 = 0x15E5; // " pressed 

            private const int DeleteOffsetX = 1;
            private const int DeleteOffsetY = 1;
            private const int DeleteButtonID1 = 0x0A94; // 'X' Button 
            private const int DeleteButtonID2 = 0x0A95; // " pressed 

            private const int PrevWidth = 20;
            private const int PrevOffsetX = 2;
            private const int PrevOffsetY = 2;
            private const int PrevButtonID1 = 0x15E3; // Arrow pointing left 
            private const int PrevButtonID2 = 0x15E7; // " pressed 

            private const int NextWidth = 20;
            private const int NextOffsetX = 2;
            private const int NextOffsetY = 2;
            private const int NextButtonID1 = 0x15E1; // Arrow pointing right 
            private const int NextButtonID2 = 0x15E5; // " pressed 

            private const int WipeWidth = 20;
            private const int WipeOffsetX = 0;
            private const int WipeOffsetY = 1;
            private const int WipeButtonID1 = 0x0A94; // 'X' Button 
            private const int WipeButtonID2 = 0x0A95; // " pressed 

            private const int OffsetSize = 1;

            private const int EntryHeight = 20;
            private const int BorderSize = 10;

            private const int PrevLabelOffsetX = PrevWidth + 1;
            private const int PrevLabelOffsetY = 0;

            private const int NextLabelOffsetX = -29;
            private const int NextLabelOffsetY = 0;

            private const int WipeLabelOffsetX = WipeWidth + 1;
            private const int WipeLabelOffsetY = 0;

            private const int EntryWidth = 500;
            private const int EntryCount = 15;

            private const int TotalWidth = OffsetSize + EntryWidth + OffsetSize + ( ButtonWidth * 2 ) + OffsetSize;
            // private const int TotalHeight = OffsetSize + ( ( EntryHeight + OffsetSize ) * ( EntryCount + 2 ) );

            private const int BackWidth = BorderSize + TotalWidth + BorderSize;
            // private const int BackHeight = BorderSize + TotalHeight + BorderSize;

            private static bool PrevLabel = true;
            private static bool NextLabel = true;
            private static bool WipeLabel = true;

            private readonly string m_Args = string.Empty;
            private readonly List<Item> m_Names;
            private int m_Page;

            public InternalItemGump( Mobile owner )
                : this( owner, BuildList(), 0 )
            {
            }

            private InternalItemGump( Mobile owner, List<Item> list, int page )
                : base( GumpOffsetX, GumpOffsetY )
            {
                owner.CloseGump( typeof( InternalItemGump ) );

                m_Names = list;

                Design( page );
            }

            private static List<Item> BuildList()
            {
                List<Item> list = new List<Item>();
                foreach( Item i in World.Items.Values )
                {
                    if( i.Map == Map.Internal && i.Parent == null && !( i is Fists ) && !( i is MountItem ) && !( i is EffectItem ) && !( i is ICommodity ) )
                        list.Add( i );
                }
                return list;
            }

            private void Design( int page )
            {
                m_Page = page;

                int count = m_Names.Count - ( page * EntryCount );

                if( count < 0 )
                    count = 0;
                else if( count > EntryCount )
                    count = EntryCount;

                int totalHeight = OffsetSize + ( ( EntryHeight + OffsetSize ) * ( count + 2 ) );

                AddPage( 0 );

                AddBackground( 0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID );
                AddImageTiled( BorderSize, BorderSize, TotalWidth, totalHeight, OffsetGumpID );

                int x = BorderSize + OffsetSize;
                int y = BorderSize + OffsetSize;

                //int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4); 
                int emptyWidth = EntryWidth;

                AddImageTiled( x, y, emptyWidth, EntryHeight, EntryGumpID );

                AddLabel( x + TextOffsetX, y, TextHue, string.Format( "Page {0} of {1} ({2}) - Matches for: {3}", page + 1, ( m_Names.Count + EntryCount - 1 ) / EntryCount, m_Names.Count, m_Args ) );

                x += emptyWidth + OffsetSize;

                AddImageTiled( x, y, PrevWidth, EntryHeight, HeaderGumpID );

                if( page > 0 )
                {
                    AddButton( x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0 );

                    if( PrevLabel )
                        AddLabel( x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous" );
                }

                x += PrevWidth + OffsetSize;

                AddImageTiled( x, y, NextWidth, EntryHeight, HeaderGumpID );

                if( ( page + 1 ) * EntryCount < m_Names.Count )
                {
                    AddButton( x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1 );

                    if( NextLabel )
                        AddLabel( x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next" );
                }

                for( int i = 0, index = page * EntryCount; i < EntryCount && index < m_Names.Count; ++i, ++index )
                {
                    x = BorderSize + OffsetSize;
                    y += EntryHeight + OffsetSize;

                    Item item = m_Names[ index ];

                    AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
                    AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, 33, item.Deleted ? "(deleted)" : ( string.Format( "{0}:{1}:{2}:{3}", item.GetType().Name, item.ItemID, item.Map, item.Location ) ) );

                    if( !item.Deleted )
                    {
                        x += EntryWidth + OffsetSize;
                        AddImageTiled( x, y, ButtonWidth, EntryHeight, ButtonGumpID );
                        AddButton( x + DeleteOffsetX, y + DeleteOffsetY, DeleteButtonID1, DeleteButtonID2, i + 4, GumpButtonType.Reply, 0 );

                        x += ButtonWidth + OffsetSize;
                        AddImageTiled( x, y, ButtonWidth, EntryHeight, ButtonGumpID );
                    }
                }

                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                AddImageTiled( x, y, TotalWidth, EntryHeight, EntryGumpID );

                AddImageTiled( x, y, WipeWidth, EntryHeight, HeaderGumpID );

                AddButton( x + WipeOffsetX, y + WipeOffsetY, WipeButtonID1, WipeButtonID2, 3, GumpButtonType.Reply, 1 );

                if( WipeLabel )
                    AddLabel( x + WipeLabelOffsetX, y + WipeLabelOffsetY, TextHue, "Wipe All Items Not Attached to Players" );
            }

            public override void OnResponse( NetState state, RelayInfo info )
            {
                Mobile from = state.Mobile;

                switch( info.ButtonID )
                {
                    case 0: // Closed 
                        {
                            return;
                        }
                    case 1: // Previous 
                        {
                            if( m_Page > 0 )
                                from.SendGump( new InternalItemGump( from, m_Names, m_Page - 1 ) );

                            break;
                        }
                    case 2: // Next 
                        {
                            if( ( m_Page + 1 ) * EntryCount < m_Names.Count )
                            {
                                if( from != null )
                                    from.SendGump( new InternalItemGump( from, m_Names, m_Page + 1 ) );
                            }
                            break;
                        }
                    case 3: // Wipe All Listed 
                        {
                            from.SendGump( new WipeAllGump( from, m_Names ) );
                            break;
                        }
                    default:
                        {
                            int index = ( m_Page * EntryCount ) + ( info.ButtonID - 4 );
                            bool deleting = index < 1000;
                            if( !deleting )
                                index -= 1000;

                            if( index >= 0 && index < m_Names.Count )
                            {
                                Item s = m_Names[ index ];

                                if( s.Deleted )
                                {
                                    from.SendMessage( "That item no longer exists." );
                                    from.SendGump( new InternalItemGump( from, m_Names, m_Page ) );
                                }
                                else
                                {
                                    if( deleting )
                                        from.SendGump( new DeleteItemGump( from, m_Names, index ) );
                                }
                            }

                            break;
                        }
                }
            }

            #region Nested type: DeleteItemGump
            private class DeleteItemGump : Gump
            {
                private readonly Mobile m_From;
                private readonly int m_Index;
                private readonly List<Item> m_Names;

                public DeleteItemGump( Mobile from, List<Item> spawners, int index )
                    : base( 50, 50 )
                {
                    m_From = from;
                    m_Names = spawners;
                    m_Index = index;

                    AddPage( 0 );

                    AddBackground( 0, 0, 270, 120, 5054 );
                    AddBackground( 10, 10, 250, 100, 3000 );

                    AddHtml( 20, 15, 230, 60, "Are you sure you wish to delete this item?", true, true );

                    AddButton( 20, 80, 4005, 4007, 2, GumpButtonType.Reply, 0 );
                    AddHtmlLocalized( 55, 80, 75, 20, 1011011, false, false ); // CONTINUE 

                    AddButton( 135, 80, 4005, 4007, 1, GumpButtonType.Reply, 0 );
                    AddHtmlLocalized( 170, 80, 75, 20, 1011012, false, false ); // CANCEL 
                }

                public override void OnResponse( NetState sender, RelayInfo info )
                {
                    if( info.ButtonID == 2 )
                    {
                        m_Names[ m_Index ].Delete();
                        m_Names.RemoveAt( m_Index );
                        m_From.SendLocalizedMessage( 1010303 ); // deleted object 
                    }
                    m_From.SendGump( new InternalItemGump( m_From, m_Names, 0 ) );
                }
            }
            #endregion

            #region Nested type: WipeAllGump
            private class WipeAllGump : Gump
            {
                private readonly Mobile m_From;
                private readonly List<Item> m_Names;

                public WipeAllGump( Mobile from, List<Item> spawners )
                    : base( 50, 50 )
                {
                    m_From = from;
                    m_Names = spawners;

                    AddPage( 0 );

                    AddBackground( 0, 0, 270, 120, 5054 );
                    AddBackground( 10, 10, 250, 100, 3000 );

                    AddHtml( 20, 15, 230, 60, "Are you sure you wish to delete all listed items?", true, true );

                    AddButton( 20, 80, 4005, 4007, 2, GumpButtonType.Reply, 0 );
                    AddHtmlLocalized( 55, 80, 75, 20, 1011011, false, false ); // CONTINUE 

                    AddButton( 135, 80, 4005, 4007, 1, GumpButtonType.Reply, 0 );
                    AddHtmlLocalized( 170, 80, 75, 20, 1011012, false, false ); // CANCEL 
                }

                public override void OnResponse( NetState sender, RelayInfo info )
                {
                    if( info.ButtonID == 2 )
                    {
                        foreach( Item item in m_Names )
                            item.Delete();

                        m_From.SendMessage( "Deleted items." );
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Nested type: InternalMobilesGump
        private class InternalMobilesGump : Gump
        {
            private const int GumpOffsetX = 30;
            private const int GumpOffsetY = 30;

            private const int TextHue = 0;
            private const int TextOffsetX = 2;

            private const int OffsetGumpID = 0x0052; // Pure black 
            private const int HeaderGumpID = 0x0E14; // Dark navy blue, textured 
            private const int EntryGumpID = 0x0BBC; // Light offwhite, textured 
            private const int BackGumpID = 0x13BE; // Gray slate/stoney 
            private const int ButtonGumpID = 0x0E14; // Dark navy blue, textured 

            private const int ButtonWidth = 20;

            // private const int GoOffsetX = 2, GoOffsetY = 2;
            // private const int GoButtonID1 = 0x15E1; // Arrow pointing right 
            // private const int GoButtonID2 = 0x15E5; // " pressed 

            private const int DeleteOffsetX = 1, DeleteOffsetY = 1;
            private const int DeleteButtonID1 = 0x0A94; // 'X' Button 
            private const int DeleteButtonID2 = 0x0A95; // " pressed 

            private const int PrevWidth = 20;
            private const int PrevOffsetX = 2, PrevOffsetY = 2;
            private const int PrevButtonID1 = 0x15E3; // Arrow pointing left 
            private const int PrevButtonID2 = 0x15E7; // " pressed 

            private const int NextWidth = 20;
            private const int NextOffsetX = 2, NextOffsetY = 2;
            private const int NextButtonID1 = 0x15E1; // Arrow pointing right 
            private const int NextButtonID2 = 0x15E5; // " pressed 

            private const int WipeWidth = 20;
            private const int WipeOffsetX = 0, WipeOffsetY = 1;
            private const int WipeButtonID1 = 0x0A94; // 'X' Button 
            private const int WipeButtonID2 = 0x0A95; // " pressed 

            private const int OffsetSize = 1;

            private const int EntryHeight = 20;
            private const int BorderSize = 10;

            private const int PrevLabelOffsetX = PrevWidth + 1;
            private const int PrevLabelOffsetY = 0;

            private const int NextLabelOffsetX = -29;
            private const int NextLabelOffsetY = 0;

            private const int WipeLabelOffsetX = WipeWidth + 1;
            private const int WipeLabelOffsetY = 0;

            private const int EntryWidth = 500;
            private const int EntryCount = 15;

            private const int TotalWidth = OffsetSize + EntryWidth + OffsetSize + ( ButtonWidth * 2 ) + OffsetSize;
            // private const int TotalHeight = OffsetSize + ( ( EntryHeight + OffsetSize ) * ( EntryCount + 2 ) );

            private const int BackWidth = BorderSize + TotalWidth + BorderSize;
            // private const int BackHeight = BorderSize + TotalHeight + BorderSize;

            private static bool PrevLabel = true;
            private static bool NextLabel = true;
            private static bool WipeLabel = true;

            private readonly string m_Args = string.Empty;
            private readonly List<Mobile> m_Names;
            private int m_Page;

            public InternalMobilesGump( Mobile owner )
                : this( owner, BuildList(), 0 )
            {
            }

            private InternalMobilesGump( Mobile owner, List<Mobile> list, int page )
                : base( GumpOffsetX, GumpOffsetY )
            {
                owner.CloseGump( typeof( InternalMobilesGump ) );

                m_Names = list;

                Design( page );
            }

            private static List<Mobile> BuildList()
            {
                List<Mobile> list = new List<Mobile>();
                foreach( Mobile i in World.Mobiles.Values )
                {
                    if( i.Map == Map.Internal && i.Account == null )
                    {
                        if( i is BaseMount )
                        {
                            BaseMount m = i as BaseMount;
                            if( m.Rider == null )
                                list.Add( m );
                        }
                        else
                            list.Add( i );
                    }
                }
                return list;
            }

            private void Design( int page )
            {
                m_Page = page;

                int count = m_Names.Count - ( page * EntryCount );

                if( count < 0 )
                    count = 0;
                else if( count > EntryCount )
                    count = EntryCount;

                int totalHeight = OffsetSize + ( ( EntryHeight + OffsetSize ) * ( count + 2 ) );

                AddPage( 0 );

                AddBackground( 0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID );
                AddImageTiled( BorderSize, BorderSize, TotalWidth, totalHeight, OffsetGumpID );

                int x = BorderSize + OffsetSize;
                int y = BorderSize + OffsetSize;

                //int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4); 
                int emptyWidth = EntryWidth;

                AddImageTiled( x, y, emptyWidth, EntryHeight, EntryGumpID );

                AddLabel( x + TextOffsetX, y, TextHue, string.Format( "Page {0} of {1} ({2}) - Matches for: {3}", page + 1, ( m_Names.Count + EntryCount - 1 ) / EntryCount, m_Names.Count, m_Args ) );

                x += emptyWidth + OffsetSize;

                AddImageTiled( x, y, PrevWidth, EntryHeight, HeaderGumpID );

                if( page > 0 )
                {
                    AddButton( x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0 );

                    if( PrevLabel )
                        AddLabel( x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous" );
                }

                x += PrevWidth + OffsetSize;

                AddImageTiled( x, y, NextWidth, EntryHeight, HeaderGumpID );

                if( ( page + 1 ) * EntryCount < m_Names.Count )
                {
                    AddButton( x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1 );

                    if( NextLabel )
                        AddLabel( x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next" );
                }

                for( int i = 0, index = page * EntryCount; i < EntryCount && index < m_Names.Count; ++i, ++index )
                {
                    x = BorderSize + OffsetSize;
                    y += EntryHeight + OffsetSize;

                    Mobile item = m_Names[ index ];

                    AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
                    try
                    {
                        AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, item is BaseCreature && ( (BaseCreature)item ).IsStabled ? 33 : 0x58 /*hue*/, item.Deleted ? "(deleted)" : ( string.Format( "{0}:{1}:{2}:{3} Stabled:{4}", item.GetType().Name, item is PlayerMobile ? ( (Account)item.Account ).Username : "Not Player", item.Map, item.Location, item is BaseCreature && ( (BaseCreature)item ).IsStabled ) ) );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( "Ex. generated from [InternalMobiles: {0}", ex );
                    }
                    if( !item.Deleted )
                    {
                        x += EntryWidth + OffsetSize;
                        AddImageTiled( x, y, ButtonWidth, EntryHeight, ButtonGumpID );
                        AddButton( x + DeleteOffsetX, y + DeleteOffsetY, DeleteButtonID1, DeleteButtonID2, i + 4, GumpButtonType.Reply, 0 );

                        x += ButtonWidth + OffsetSize;
                        AddImageTiled( x, y, ButtonWidth, EntryHeight, ButtonGumpID );
                    }
                }

                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                AddImageTiled( x, y, TotalWidth, EntryHeight, EntryGumpID );

                AddImageTiled( x, y, WipeWidth, EntryHeight, HeaderGumpID );

                AddButton( x + WipeOffsetX, y + WipeOffsetY, WipeButtonID1, WipeButtonID2, 3, GumpButtonType.Reply, 1 );

                if( WipeLabel )
                    AddLabel( x + WipeLabelOffsetX, y + WipeLabelOffsetY, TextHue, "Wipe All Non-Stabled" );
            }

            public override void OnResponse( NetState state, RelayInfo info )
            {
                Mobile from = state.Mobile;

                switch( info.ButtonID )
                {
                    case 0: // Closed 
                        {
                            return;
                        }
                    case 1: // Previous 
                        {
                            if( m_Page > 0 )
                                from.SendGump( new InternalMobilesGump( from, m_Names, m_Page - 1 ) );

                            break;
                        }
                    case 2: // Next 
                        {
                            if( ( m_Page + 1 ) * EntryCount < m_Names.Count )
                            {
                                if( from != null )
                                    from.SendGump( new InternalMobilesGump( from, m_Names, m_Page + 1 ) );
                            }
                            break;
                        }
                    case 3: // Wipe All Listed 
                        {
                            from.SendGump( new WipeAllGump( from, m_Names ) );
                            break;
                        }
                    default:
                        {
                            int index = ( m_Page * EntryCount ) + ( info.ButtonID - 4 );
                            bool deleting = index < 1000;
                            if( !deleting )
                                index -= 1000;

                            if( index >= 0 && index < m_Names.Count )
                            {
                                Mobile s = m_Names[ index ];

                                if( s.Deleted )
                                {
                                    from.SendMessage( "That mobile no longer exists." );
                                    from.SendGump( new InternalMobilesGump( from, m_Names, m_Page ) );
                                }
                                else
                                {
                                    if( deleting )
                                        from.SendGump( new DeleteGump( from, m_Names, index ) );
                                }
                            }

                            break;
                        }
                }
            }

            #region Nested type: DeleteGump
            private class DeleteGump : Gump
            {
                private readonly Mobile m_From;
                private readonly int m_Index;
                private readonly List<Mobile> m_Names;

                public DeleteGump( Mobile from, List<Mobile> spawners, int index )
                    : base( 50, 50 )
                {
                    m_From = from;
                    m_Names = spawners;
                    m_Index = index;

                    AddPage( 0 );

                    AddBackground( 0, 0, 270, 120, 5054 );
                    AddBackground( 10, 10, 250, 100, 3000 );

                    AddHtml( 20, 15, 230, 60, "Are you sure you wish to delete this mobile?", true, true );

                    AddButton( 20, 80, 4005, 4007, 2, GumpButtonType.Reply, 0 );
                    AddHtmlLocalized( 55, 80, 75, 20, 1011011, false, false ); // CONTINUE 

                    AddButton( 135, 80, 4005, 4007, 1, GumpButtonType.Reply, 0 );
                    AddHtmlLocalized( 170, 80, 75, 20, 1011012, false, false ); // CANCEL 
                }

                public override void OnResponse( NetState sender, RelayInfo info )
                {
                    if( info.ButtonID == 2 )
                    {
                        m_Names[ m_Index ].Delete();
                        m_Names.RemoveAt( m_Index );
                        m_From.SendLocalizedMessage( 1010303 ); // deleted object 
                    }
                    m_From.SendGump( new InternalMobilesGump( m_From, m_Names, 0 ) );
                }
            }
            #endregion

            #region Nested type: WipeAllGump
            private class WipeAllGump : Gump
            {
                private readonly Mobile m_From;
                private readonly List<Mobile> m_Names;

                public WipeAllGump( Mobile from, List<Mobile> spawners )
                    : base( 50, 50 )
                {
                    m_From = from;
                    m_Names = spawners;

                    AddPage( 0 );

                    AddBackground( 0, 0, 270, 120, 5054 );
                    AddBackground( 10, 10, 250, 100, 3000 );

                    AddHtml( 20, 15, 230, 60, "Are you sure you wish to delete all of the non-stabled listed mobiles?", true, true );

                    AddButton( 20, 80, 4005, 4007, 2, GumpButtonType.Reply, 0 );
                    AddHtmlLocalized( 55, 80, 75, 20, 1011011, false, false ); // CONTINUE 

                    AddButton( 135, 80, 4005, 4007, 1, GumpButtonType.Reply, 0 );
                    AddHtmlLocalized( 170, 80, 75, 20, 1011012, false, false ); // CANCEL 
                }

                public override void OnResponse( NetState sender, RelayInfo info )
                {
                    if( info.ButtonID == 2 )
                    {
                        foreach( Mobile item in m_Names )
                        {
                            if( item is BaseCreature )
                            {
                                if( ( (BaseCreature)item ).IsStabled == false )
                                    item.Delete();
                            }
                            else
                                item.Delete();
                        }

                        m_From.SendMessage( "Deleted non-stabled mobiles." );
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}