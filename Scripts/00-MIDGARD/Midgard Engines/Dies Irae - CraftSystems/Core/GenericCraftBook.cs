/***************************************************************************
 *                               GenericCraftBook.cs
 *                            ----------------------
 *   begin                : 16 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;
using Server;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Network;
using System;

using Midgard.Engines.AdvancedCooking;
using Midgard.Engines.BrewCrafing;
using Midgard.Engines.Craft;

namespace Midgard.Engines.OldCraftSystem
{
    public abstract class GenericCraftBook : Item
    {
        public abstract CraftSystem BookSystem { get; }

        protected GenericCraftBook()
            : this( 0xEFA )
        {
        }

        protected GenericCraftBook( int itemID )
            : base( itemID )
        {
            Weight = 3.0;
            Hue = Utility.RandomNondyedHue();
            Layer = Layer.OneHanded;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( GetWorldLocation(), 1 ) )
            {
                from.CloseGump( typeof( GenericCraftBookGump ) );
                from.SendGump( new GenericCraftBookGump( this ) );
            }
        }

        #region serialization
        public GenericCraftBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        public class GenericCraftBookGump : Gump
        {
            private readonly List<CraftItem> m_Items;

            public GenericCraftBook Book { get; private set; }

            public GenericCraftBookGump( GenericCraftBook book )
                : base( 150, 200 )
            {
                Book = book;

                if( Book == null )
                    return;

                m_Items = new List<CraftItem>();
                foreach( CraftItem craftItem in Book.BookSystem.CraftItems )
                {
                    m_Items.Add( craftItem );
                }

                AddBackground();
                AddIndex();

                for( int page = 0; page < m_Items.Count; ++page )
                {
                    AddPage( 2 + page );

                    AddButton( 125, 14, 2205, 2205, 0, GumpButtonType.Page, 1 + page );

                    if( page < m_Items.Count - 1 )
                        AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 3 + page );

                    for( int half = 0; half < 2; ++half )
                        AddDetails( ( page * 2 ) + half, half );
                }
            }

            private void AddBackground()
            {
                AddPage( 0 );

                AddImage( 100, 10, 0x899 );                  // Background image of an open book

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
            }

            private int[] GetFistPageByGroupIndex()
            {
                CraftGroupCol entries = Book.BookSystem.CraftGroups;
                List<int> elementsForGroup = new List<int>();

                int counter = 0;

                elementsForGroup.Add( counter ); // first group start at index 0

                for( int i = 0; i < entries.Count; i++ )
                {
                    counter += entries.GetAt( i ).CraftItems.Count;
                    elementsForGroup.Add( counter );
                }

                return elementsForGroup.ToArray();
            }

            private void AddIndex()
            {
                CraftSystem system = Book.BookSystem;

                // Index
                AddPage( 1 );

                AddOldHtml( 140, 40, 80, 18, "System:" );
                AddFirstCharHuedHtml( 200, 40, 100, 18, system.Name, Colors.Indigo, true );

                // Groups
                AddOldHtml( 300, 40, 100, 18, "Groups:" );
                AddOldHtml( 400, 40, 30, 18, system.CraftGroups.Count.ToString() );

                int[] pageIndexes = GetFistPageByGroupIndex();

                // List of entries
                CraftGroupCol entries = system.CraftGroups;
                if( entries != null )
                {
                    for( int i = 0; i < 16 && i < entries.Count; ++i )
                    {
                        CraftGroup group = entries.GetAt( i );

                        string desc = StringList.GetClilocString( group.NameString, group.NameNumber );
                        int x = 140 + ( ( i / 8 ) * 160 );
                        int y = 60 + ( ( i % 8 ) * 17 );
                        int gumpId = ( i < 8 ) ? 0x8B0 : 0x8AF;
                        int xBtnOffset = ( ( i / 8 ) * 110 );

                        // Description label
                        AddButton( x - 4 + xBtnOffset, y + 2, gumpId, gumpId, 0, GumpButtonType.Page, 2 + pageIndexes[ i ] / 2 );
                        AddOldHtml( x + 15, y, 115, 17, desc.ToLower() );
                    }
                }

                // Turn page button
                AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 2 );
            }

            private void AddDetails( int index, int half )
            {
                int aligned = 135;
                int tabbed = 140;

                // List of entries
                List<CraftItem> entries = m_Items;
                if( entries == null )
                    return;

                if( index < entries.Count )
                {
                    CraftItem craftItem = entries[ index ];
                    int y = 40;

                    string name = StringList.GetClilocString( craftItem.NameString, craftItem.NameNumber, true );
                    AddOldHtml( 120 + ( half * 160 ), y, 155, 17, HtmlCenter( name ) );

                    if( Config.Debug )
                        Config.Pkg.LogInfoLine( "OldCraftBook, entryName: " + name );

                    y += 30;
                    AddOldHtml( aligned + ( half * 160 ), y, 150, 17, "Requirements:" );

                    y += 20;
                    for( int i = 0; i < craftItem.Resources.Count && i < 4; i++, y += 20 )
                    {
                        CraftRes craftResource = craftItem.Resources.GetAt( i );
                        string resName = StringList.GetClilocString( craftResource.NameString, craftResource.NameNumber );
                        AddOldHtml( tabbed + ( half * 160 ), y, 150, 17, String.Format( "{0} {1}", resName, craftResource.Amount ) );
                    }

                    y += 10;
                    double skill = Math.Max( craftItem.GetMinMainCraftSkill( Book.BookSystem ), 0.0 );
                    AddOldHtml( aligned + ( half * 160 ), y, 150, 17, String.Format( "Diff.: {0}", skill.ToString( "F2" ) ) );
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

    public class AlchemyBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "alchemist's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefAlchemy.CraftSystem; }
        }

        [Constructable]
        public AlchemyBook()
        {
        }

        #region serialization
        public AlchemyBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class BlacksmithyBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "blacksmith's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefBlacksmithy.CraftSystem; }
        }

        [Constructable]
        public BlacksmithyBook()
        {
        }

        #region serialization
        public BlacksmithyBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class BowFletchingBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "fletcher's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefBowFletching.CraftSystem; }
        }

        [Constructable]
        public BowFletchingBook()
        {
        }

        #region serialization
        public BowFletchingBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class CarpentryBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "carpenter's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefCarpentry.CraftSystem; }
        }

        [Constructable]
        public CarpentryBook()
        {
        }

        #region serialization
        public CarpentryBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class CartographyBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "map maker's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefCartography.CraftSystem; }
        }

        [Constructable]
        public CartographyBook()
        {
        }

        #region serialization
        public CartographyBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class CookingBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "cook's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefCooking.CraftSystem; }
        }

        [Constructable]
        public CookingBook()
        {
        }

        #region serialization
        public CookingBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class GlassblowingCraftGump : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "glass blower's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefGlassblowing.CraftSystem; }
        }

        [Constructable]
        public GlassblowingCraftGump()
        {
        }

        #region serialization
        public GlassblowingCraftGump( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class InscriptionBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "scribe's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefInscription.CraftSystem; }
        }

        [Constructable]
        public InscriptionBook()
        {
        }

        #region serialization
        public InscriptionBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class MasonryCraftBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "stone crafter's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefMasonry.CraftSystem; }
        }

        [Constructable]
        public MasonryCraftBook()
        {
        }

        #region serialization
        public MasonryCraftBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class TailoringBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "tailor's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefTailoring.CraftSystem; }
        }

        [Constructable]
        public TailoringBook()
        {
        }

        #region serialization
        public TailoringBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class TinkeringBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "tinker's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefTinkering.CraftSystem; }
        }

        [Constructable]
        public TinkeringBook()
        {
        }

        #region serialization
        public TinkeringBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class BakingBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "cook's handbook (baking recipes)"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefBaking.CraftSystem; }
        }

        [Constructable]
        public BakingBook()
        {
        }

        #region serialization
        public BakingBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class BoilingBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "cook's handbook (boiling recipes)"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefBoiling.CraftSystem; }
        }

        [Constructable]
        public BoilingBook()
        {
        }

        #region serialization
        public BoilingBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class GrillingBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "cook's handbook (grill recipes)"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefGrilling.CraftSystem; }
        }

        [Constructable]
        public GrillingBook()
        {
        }

        #region serialization
        public GrillingBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class BrewingBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "brewery handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefBrewing.CraftSystem; }
        }

        [Constructable]
        public BrewingBook()
        {
        }

        #region serialization
        public BrewingBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class CrystalCraftingBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "crystal forger's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefCrystalCrafting.CraftSystem; }
        }

        [Constructable]
        public CrystalCraftingBook()
        {
        }

        #region serialization
        public CrystalCraftingBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class WaxCraftingBook : GenericCraftBook
    {
        public override string DefaultName
        {
            get { return "wax crafter's handbook"; }
        }

        public override CraftSystem BookSystem
        {
            get { return DefWaxCrafting.CraftSystem; }
        }

        [Constructable]
        public WaxCraftingBook()
        {
        }

        #region serialization
        public WaxCraftingBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}