/***************************************************************************
 *                              SmeltingBookGump.cs
 *                            -----------------------
 *   begin                : 02 gennaio, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.AdvancedSmelting
{
    public class SmeltingBookGump : Gump
    {
        private readonly Mobile m_Owner;
        private readonly List<SmeltInfo> m_Recipes;

        public SmeltingBook Book { get; private set; }

        public SmeltingBookGump( SmeltingBook book )
            : base( 150, 200 )
        {
            Book = book;
            if( Book != null )
                m_Owner = Book.Owner;

            m_Recipes = new List<SmeltInfo>( SmeltInfo.SmeltList );

            AddBackground();
            AddIndex();

            for( int page = 0; page < 8; ++page )
            {
                AddPage( 2 + page );

                AddButton( 125, 14, 2205, 2205, 0, GumpButtonType.Page, 1 + page );

                if( page < 7 )
                    AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 3 + page );

                for( int half = 0; half < 2; ++half )
                    AddDetails( ( page * 2 ) + half, half );
            }
        }

        private void AddBackground()
        {
            AddPage( 0 );

            AddImage( 100, 10, 2201 );                  // Background image of an open book

            // Two separators under first line
            for( int i = 0; i < 2; ++i )
            {
                int xOffset = 125 + ( i * 165 );

                AddImage( xOffset, 50, 57 );            // starting piece
                xOffset += 20;

                for( int j = 0; j < 6; ++j, xOffset += 15 )
                    AddImage( xOffset, 50, 58 );        // little hor. line

                AddImage( xOffset - 5, 50, 59 );        // ending piece
            }

            // First four page buttons
            for( int i = 0, xOffset = 130, gumpID = 2225; i < 4; ++i, xOffset += 35, ++gumpID )
                AddButton( xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 2 + i );

            // Next four page buttons
            for( int i = 0, xOffset = 300, gumpID = 2229; i < 4; ++i, xOffset += 35, ++gumpID )
                AddButton( xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 6 + i );
        }

        private void AddIndex()
        {
            // Index
            AddPage( 1 );

            if( m_Owner != null )
            {
                AddOldHtml( 140, 40, 80, 18, "Owner:" );
                AddOldHtml( 200, 40, 30, 18, m_Owner.Name ?? "-" );
            }

            // Recipes
            if( m_Recipes != null )
            {
                AddOldHtml( 300, 40, 100, 18, "Recipes:" );
                AddOldHtml( 400, 40, 30, 18, m_Recipes.Count.ToString() );
            }

            // List of entries
            List<SmeltInfo> entries = m_Recipes;

            for( int i = 0; i < 16; ++i )
            {
                if( entries != null && i < entries.Count )
                {
                    // Button
                    // AddButton( 130 + ( ( i / 8 ) * 160 ), 65 + ( ( i % 8 ) * 15 ), 2103, 2104, 2 + ( i * 6 ) + 0, GumpButtonType.Reply, 0 );

                    string desc = CraftResources.GetName( entries[ i ].ResultRes );

                    // Description label
                    AddOldHtml( 145 + ( ( i / 8 ) * 160 ), 60 + ( ( i % 8 ) * 15 ), 115, 17, desc );
                }
            }

            // Turn page button
            AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 2 );
        }

        private void AddDetails( int index, int half )
        {
            int aligned = 135;
            int tabbed = 150;

            // List of entries
            List<SmeltInfo> entries = m_Recipes;
            if( entries == null )
                return;

            if( index < entries.Count )
            {
                SmeltInfo e = entries[ index ];

                AddOldHtml( tabbed + ( half * 160 ), 40, 115, 17, CraftResources.GetName( e.ResultRes ) );
                AddOldHtml( aligned + ( half * 160 ), 60, 115, 17, "Required:" );
                AddOldHtml( tabbed + ( half * 160 ), 80, 115, 17, String.Format( "{0} {1}", e.ResAmount1, CraftResources.GetName( e.Info1 ) ) );
                AddOldHtml( tabbed + ( half * 160 ), 100, 115, 17, String.Format( "{0} {1}", e.ResAmount2, CraftResources.GetName( e.Info2 ) ) );
            }
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;

            if( Book.Deleted || !from.InRange( Book.GetWorldLocation(), 1 ) )
                return;
        }
    }
}