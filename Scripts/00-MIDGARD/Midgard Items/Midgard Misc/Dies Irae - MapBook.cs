/***************************************************************************
 *                                  MapBook.cs
 *                            		----------
 *  begin                	: Febbraio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

using Midgard.Gumps;
using Midgard.Items;

using Server;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Prompts;

namespace Midgard.Gumps
{
    public class MapBookGump : Gump
    {
        private MapBook m_Book;

        public MapBookGump( MapBook book )
            : base( 150, 200 )
        {
            m_Book = book;

            AddBackground();

            for ( int page = 0; page < 8; ++page )
            {
                AddPage( 1 + page );

                if( page > 0 ) //0
                    AddButton( 125, 14, 2205, 2205, 0, GumpButtonType.Page, page );

                if( page < 7 )
                    AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 2 + page );

                if( page < 4 )
                    AddImage( 135 + ( page * 35 ), 190, 36 );
                if( page > 3 )
                    AddImage( 305 + ( ( page - 4 ) * 35 ), 190, 36 );

                for ( int half = 0; half < 2; ++half )
                {
                    int tb = 0;
                    AddDetails( ( page * 4 ) + ( half * 2 ), half, tb );
                    tb = 1;
                    AddDetails( ( page * 4 ) + ( half * 2 ) + 1, half, tb );
                }
            }
        }

        public MapBook Book
        {
            get { return m_Book; }
        }

        public int GetMapHue( Map map )
        {
            if( map == Map.Trammel )
                return 10;
            else if( map == Map.Felucca )
                return 81;
            else if( map == Map.Ilshenar )
                return 1102;
            else if( map == Map.Malas )
                return 1102;

            return 0;
        }

        public string GetName( string name )
        {
            if( name == null || ( name = name.Trim() ).Length <= 0 )
                return "(indescript)";

            return name;
        }

        private void AddBackground()
        {
            AddPage( 0 );

            AddImage( 100, 10, 2200 ); // background 

            for ( int i = 0; i < 2; ++i ) // page separators
            {
                int xOffset = 125 + ( i * 165 );

                AddImage( xOffset, 105, 57 );
                xOffset += 20;

                for ( int j = 0; j < 6; ++j, xOffset += 15 )
                    AddImage( xOffset, 105, 58 );

                AddImage( xOffset - 5, 105, 59 );
            }

            for ( int i = 0, xOffset = 130, gumpID = 2225; i < 4; ++i, xOffset += 35, ++gumpID )
                AddButton( xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 1 + i );

            for ( int i = 0, xOffset = 300, gumpID = 2229; i < 4; ++i, xOffset += 35, ++gumpID )
                AddButton( xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 5 + i );
        }

        private void AddDetails( int index, int half, int tb )
        {
            int hue;

            if( index < m_Book.Entries.Count )
            {
                int btn;
                btn = ( index * 2 ) + 1;

                MapBookEntry e = (MapBookEntry) m_Book.Entries[ index ];
                hue = GetMapHue( e.Map );

                AddLabel( 135 + ( half * 160 ), 40 + ( tb * 80 ), hue, String.Format( "Level {0}  {1}", e.Level, e.Map ) );

                if( e.Decoder == null )
                    AddLabel( 135 + ( half * 160 ), 55 + ( tb * 80 ), hue, String.Format( "Not Decoded" ) );
                else
                {
                    AddLabel( 135 + ( half * 160 ), 55 + ( tb * 80 ), hue, String.Format( "Decoder  {0}", e.Decoder.Name ) );
                    AddButton( 135 + ( half * 160 ), 89 + ( tb * 80 ), 216, 216, btn + 1, GumpButtonType.Reply, 0 );

                    AddHtml( 150 + ( half * 160 ), 87 + ( tb * 80 ), 100, 18, "View", false, false );
                }

                AddButton( 135 + ( half * 160 ), 75 + ( tb * 80 ), 2437, 2438, btn, GumpButtonType.Reply, 0 );
                AddHtml( 150 + ( half * 160 ), 73 + ( tb * 80 ), 100, 18, "Drop TMap", false, false );
            }
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;

            if( m_Book.Deleted || !from.InRange( m_Book.GetWorldLocation(), 1 ) || !DesignContext.Check( from ) )
                return;

            int buttonID = info.ButtonID;
            int index = ( buttonID / 2 );
            int drp = buttonID % 2;

            if( index >= 0 && index < m_Book.Entries.Count && drp == 1 )
            {
                MapBookEntry e = (MapBookEntry) m_Book.Entries[ index ];

                if( m_Book.CheckAccess( from ) )
                {
                    m_Book.DropMap( from, e, index );
                    from.CloseGump( typeof( MapBookGump ) );
                    from.SendGump( new MapBookGump( m_Book ) );
                }
                else
                {
                    from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
                }
            }
            else if( index >= 1 && index < m_Book.Entries.Count + 1 && drp == 0 )
            {
                index = index - 1;

                MapBookEntry e = (MapBookEntry) m_Book.Entries[ index ];
                if( m_Book.CheckAccess( from ) )
                {
                    from.CloseGump( typeof( MapBookGump ) );
                    from.SendGump( new MapBookGump( m_Book ) );
                    m_Book.ViewMap( from, e, index );
                }
                else
                {
                    from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
                }
            }
        }
    }

    public class MapDisplayGump : Gump
    {
        public MapDisplayGump( int xx, int yy )
            : base( 25, 25 )
        {
            double dx = xx / 13.06;
            double dy = yy / 10.9;
            int x = (int) dx + 50;
            int y = (int) dy + 65;

            Resizable = true;
            Resizable = false;
            AddPage( 0 );
            AddBackground( 52, 25, 393, 430, 5120 );
            AddImage( 59, 63, 5528 );

            AddItem( x, y, 575 );
            AddImage( 60, 65, 2360 );
            AddImage( 430, 65, 2360 );
            AddImage( 430, 435, 2360 );
            AddImage( 60, 435, 2360 );
        }

        public string GetName( string name )
        {
            if( name == null || ( name = name.Trim() ).Length <= 0 )
                return "(indescript)";

            return name;
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
        }
    }
}

namespace Midgard.Items
{
    public class MapBook : Item, ISecurable, ICraftable
    {
        private Mobile m_Crafter;
        private int m_DefaultIndex;
        private ArrayList m_Entries;
        private SecureLevel m_Level;
        private int m_MaxMaps;
        private BookQuality m_Quality;

        [Constructable]
        public MapBook()
            : base( 0x2252 )
        {
            Weight = 1.0;
            Hue = Utility.RandomNeutralHue();
            Name = "Map Book";

            m_MaxMaps = 30;

            Layer = Layer.OneHanded;
            m_Entries = new ArrayList();
            m_DefaultIndex = -1;
        }

        public MapBook( Serial serial )
            : base( serial )
        {
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxMaps
        {
            get { return m_MaxMaps; }
            set
            {
                m_MaxMaps = value;
                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public BookQuality Quality
        {
            get { return m_Quality; }
            set
            {
                m_Quality = value;
                InvalidateProperties();
            }
        }

        /*
        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }
        */

        public ArrayList Entries
        {
            get { return m_Entries; }
        }

        public MapBookEntry Default
        {
            get
            {
                if( m_DefaultIndex >= 0 && m_DefaultIndex < m_Entries.Count )
                    return (MapBookEntry) m_Entries[ m_DefaultIndex ];

                return null;
            }
            set
            {
                if( value == null )
                    m_DefaultIndex = -1;
                else
                    m_DefaultIndex = m_Entries.IndexOf( value );
            }
        }

        #region ICraftable Members
        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
        {
            m_Quality = (BookQuality)( quality - 1 );

            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            m_MaxMaps = 30 + (int)( from.Skills[ SkillName.Inscribe ].Value / 10 );
            if( m_Quality == BookQuality.Exceptional )
                m_MaxMaps += 10;

            if( m_MaxMaps > 40 )
                m_MaxMaps = 40;

            return quality;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set
            {
                m_Crafter = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }
        #endregion

        #region ISecurable Members
        [CommandProperty( AccessLevel.GameMaster )]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
        #endregion

        public override bool AllowEquipedCast( Mobile from )
        {
            return true;
        }

        public override void GetContextMenuEntries( Mobile from, List< ContextMenuEntry > list )
        {
            base.GetContextMenuEntries( from, list );

            SetSecureLevelEntry.AddTo( from, this, list );

            if( from.CheckAlive() && IsChildOf( from.Backpack ) )
                list.Add( new NameBookEntry( from, this ) );
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( m_Quality == BookQuality.Exceptional )
                list.Add( 1063341 ); // exceptional

            if( m_Crafter != null )
                list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 );

            // version 1
            writer.Write( (byte) m_Quality );
            writer.Write( m_Crafter );
            writer.Write( m_MaxMaps );
            writer.Write( (int) m_Level );

            // version 0
            writer.Write( m_Entries.Count );
            for ( int i = 0; i < m_Entries.Count; ++i )
                ( (MapBookEntry) m_Entries[ i ] ).Serialize( writer );
            writer.Write( m_DefaultIndex );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        m_Quality = (BookQuality) reader.ReadByte();
                        m_Crafter = reader.ReadMobile();
                        m_MaxMaps = reader.ReadInt();
                        m_Level = (SecureLevel) reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        int count = reader.ReadInt();

                        m_Entries = new ArrayList( count );

                        for ( int i = 0; i < count; ++i )
                            m_Entries.Add( new MapBookEntry( reader ) );

                        m_DefaultIndex = reader.ReadInt();
                        break;
                    }
            }
        }

        public void DropMap( Mobile from, MapBookEntry e, int index )
        {
            if( m_DefaultIndex == index )
                m_DefaultIndex = -1;

            m_Entries.RemoveAt( index );

            TreasureMap tmap = new TreasureMap( e.Level, e.Map );

            tmap.Decoder = e.Decoder;
            tmap.ChestLocation = e.Location;
            tmap.ChestMap = e.Map;
            tmap.Bounds = e.Bounds;

            tmap.ClearPins();
            tmap.AddWorldPin( e.Location.X, e.Location.Y );

            from.AddToBackpack( tmap );

            from.SendMessage( "You have removed the Treasure Map" );
        }

        public void ViewMap( Mobile from, MapBookEntry e, int index )
        {
            if( m_DefaultIndex == index )
                m_DefaultIndex = -1;

            from.CloseGump( typeof( MapDisplayGump ) );
            from.SendGump( new MapDisplayGump( e.Location.X, e.Location.Y ) );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( GetWorldLocation(), 1 ) )
            {
                from.CloseGump( typeof( MapBookGump ) );
                from.SendGump( new MapBookGump( this ) );
            }
        }

        public bool CheckAccess( Mobile m )
        {
            if( !IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster )
                return true;

            BaseHouse house = BaseHouse.FindHouseAt( this );

            if( house != null && house.IsAosRules && ( house.Public ? house.IsBanned( m ) : !house.HasAccess( m ) ) )
                return false;

            return ( house != null && house.HasSecureAccess( m, m_Level ) );
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if( dropped is TreasureMap )
            {
                if( !CheckAccess( from ) )
                {
                    from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
                }
                else if( m_Entries.Count < m_MaxMaps )
                {
                    TreasureMap tmap = (TreasureMap) dropped;
                    if( tmap.Completed )
                    {
                        from.SendMessage( "This map is completed and can not be stored in this book" );
                        InvalidateProperties();
                        dropped.Delete();
                        return false;
                    }
                    if( tmap.ChestMap != null )
                    {
                        m_Entries.Add( new MapBookEntry( tmap.Level, tmap.Decoder, tmap.ChestMap, tmap.ChestLocation, tmap.Bounds ) );
                        InvalidateProperties();
                        dropped.Delete();
                        from.Send( new PlaySound( 0x42, GetWorldLocation() ) );
                        return true;
                    }
                    else
                    {
                        from.SendMessage( "This map is invalid" );
                    }
                }
                else
                {
                    from.SendMessage( "This TMap Book is full" );
                }
            }

            return false;
        }

        #region Nested type: NameBookEntry
        private class NameBookEntry : ContextMenuEntry
        {
            private MapBook m_Book;
            private Mobile m_From;

            public NameBookEntry( Mobile from, MapBook book )
                : base( 6216 )
            {
                m_From = from;
                m_Book = book;
            }

            public override void OnClick()
            {
                if( m_From.CheckAlive() && m_Book.IsChildOf( m_From.Backpack ) )
                {
                    m_From.Prompt = new NameBookPrompt( m_Book );
                    m_From.SendLocalizedMessage( 1062479 ); // Type in the new name of the book:
                }
            }
        }
        #endregion

        #region Nested type: NameBookPrompt
        private class NameBookPrompt : Prompt
        {
            private MapBook m_Book;

            public NameBookPrompt( MapBook book )
            {
                m_Book = book;
            }

            public override void OnResponse( Mobile from, string text )
            {
                if( text.Length > 40 )
                    text = text.Substring( 0, 40 );

                if( from.CheckAlive() && m_Book.IsChildOf( from.Backpack ) )
                {
                    m_Book.Name = Utility.FixHtml( text.Trim() );

                    from.SendMessage( "This Map Book name has been changed" );
                }
            }

            public override void OnCancel( Mobile from )
            {
            }
        }
        #endregion
    }

    public class MapBookEntry
    {
        private Rectangle2D m_Bounds;
        private Mobile m_Decoder;
        private int m_Level;
        private Point2D m_Location;
        private Map m_Map;

        public MapBookEntry( int level, Mobile decoder, Map map, Point2D loc, Rectangle2D bounds )
        {
            m_Level = level;
            m_Decoder = decoder;
            m_Map = map;
            m_Location = loc;
            m_Bounds = bounds;
        }

        public MapBookEntry( GenericReader reader )
        {
            int version = reader.ReadByte();

            m_Level = reader.ReadInt();
            m_Decoder = reader.ReadMobile();
            m_Map = reader.ReadMap();
            m_Location = reader.ReadPoint2D();
            m_Bounds = reader.ReadRect2D();
        }

        public int Level
        {
            get { return m_Level; }
        }

        public Mobile Decoder
        {
            get { return m_Decoder; }
        }

        public Map Map
        {
            get { return m_Map; }
        }

        public Point2D Location
        {
            get { return m_Location; }
        }

        public Rectangle2D Bounds
        {
            get { return m_Bounds; }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( (byte) 0 ); // version

            writer.Write( m_Level );
            writer.Write( m_Decoder );
            writer.Write( m_Map );
            writer.Write( m_Location );
            writer.Write( m_Bounds );
        }
    }
}