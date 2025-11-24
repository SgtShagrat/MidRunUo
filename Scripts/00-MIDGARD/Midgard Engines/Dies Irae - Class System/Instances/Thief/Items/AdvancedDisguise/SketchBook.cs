/***************************************************************************
 *                                  SketchBook.cs
 *                            		-------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.Classes;

using Server;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Prompts;

namespace Midgard.Engines.AdvancedDisguise
{
    public class SketchBook : Item, ISecurable, ICraftable
    {
        private static readonly List<SketchBook> m_SketchBooks = new List<SketchBook>();
        public static List<SketchBook> SketchBooks { get { return m_SketchBooks; } }

        private BookQuality m_Quality;
        private Mobile m_Crafter;
        private int m_MaxEntries;
        private SecureLevel m_Level;
        private Mobile m_Owner;

        public List<DisguiseEntry> Entries { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public BookQuality Quality
        {
            get { return m_Quality; }
            set { m_Quality = value; InvalidateProperties(); }
        }

        /*
        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; InvalidateProperties(); }
        }
        */

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxEntries
        {
            get { return m_MaxEntries; }
            set { m_MaxEntries = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsEmptyBook
        {
            get { return Entries.Count == 0; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; InvalidateProperties(); }
        }

        [Constructable]
        public SketchBook()
            : base( 0xEFA )
        {
            Hue = 982;
            Name = "a Sketch Book";
            Layer = Layer.Invalid;

            if( Core.AOS )
                LootType = LootType.Blessed;

            Weight = 1.0;

            Entries = new List<DisguiseEntry>();
            m_MaxEntries = 10;
            m_Owner = null;

            m_SketchBooks.Add( this );
        }

        public SketchBook( Serial serial )
            : base( serial )
        {
            m_SketchBooks.Add( this );
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
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

            if( m_Owner != null )
                list.Add( 1041602, m_Owner.Name ); // Owner : ~1_NAME~

            if( Entries != null && Entries.Count > 0 )
                list.Add( "Pages: {0}", Entries.Count );
        }

        public static bool CheckIsThief( Mobile from, bool message )
        {
            if( from == null )
                return false;

            bool isThief = ClassSystem.IsThief( from );

            if( message && !isThief )
                from.SendMessage( "Not a good thing. Only thieves can do that." );

            return isThief;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from == null )
                return;

            if( IsChildOf( from.Backpack ) )
            {
                if( from.AccessLevel > AccessLevel.Player || CheckIsThief( from, false ) )
                {
                    if( m_Owner == null )
                    {
                        m_Owner = from;
                        from.SendMessage( "Now you own this book." );
                    }

                    if( m_Owner != from )
                    {
                        from.SendMessage( "Only the book owner can open it." );
                    }
                    else
                    {
                        from.CloseGump( typeof( SketchGump ) );
                        from.SendGump( new SketchGump( from, this ) );
                    }
                }
                else
                {
                    from.SendMessage( "You don't know how to use that book." );
                }
            }
            else
            {
                from.SendMessage( "You can use a sketchbook only from your pack." );
            }
        }

        public override void OnDelete()
        {
            m_SketchBooks.Remove( this );

            base.OnDelete();
        }

        public static void RevomeAliasFromBooksForMobile( Mobile alias, Mobile thief )
        {
            if( m_SketchBooks == null || m_SketchBooks.Count <= 0 || alias == null )
                return;

            foreach( SketchBook t in m_SketchBooks )
            {
                if( t == null )
                    continue;

                if( t.Owner == null || t.Owner != thief )
                    continue;

                if( !t.HasAlias( alias ) )
                    continue;

                DisguiseEntry entry = t.GetEntryFromMobile( alias );

                if( entry != null )
                    t.RemoveEntry( entry );
            }
        }

        public bool HasAlias( Mobile mobile )
        {
            if( Entries == null || Entries.Count == 0 )
                return false;

            foreach( DisguiseEntry d in Entries )
            {
                if( d != null && d.AliasSource != null && d.AliasSource == mobile )
                {
                    return true;
                }
            }

            return false;
        }

        public void AddEntry( DisguiseEntry entry )
        {
            if( entry == null )
                return;

            if( Entries == null )
                Entries = new List<DisguiseEntry>();

            Entries.Add( entry );
            InvalidateProperties();
        }

        public void RemoveEntry( DisguiseEntry entry )
        {
            if( entry == null )
                return;

            if( Entries == null )
                Entries = new List<DisguiseEntry>();

            if( Entries.Contains( entry ) )
                Entries.Remove( entry );

            InvalidateProperties();
        }

        public DisguiseEntry GetEntryFromMobile( Mobile mobile )
        {
            if( Entries == null || Entries.Count == 0 )
                return null;

            foreach( DisguiseEntry d in Entries )
            {
                if( d != null && d.AliasSource != null && d.AliasSource == mobile )
                    return d;
            }

            return null;
        }

        #region ICraftable Members
        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
        {
            m_Quality = (BookQuality)( quality - 1 );

            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            m_MaxEntries = 10 + (int)( from.Skills[ SkillName.Inscribe ].Value / 10 );
            if( m_Quality == BookQuality.Exceptional )
                m_MaxEntries += 10;

            return quality;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 );

            // version 1
            writer.Write( m_Owner );

            // version 0
            writer.Write( (byte)m_Quality );
            writer.Write( m_Crafter );
            writer.Write( m_MaxEntries );
            writer.Write( (int)m_Level );

            writer.Write( Entries.Count );
            foreach( DisguiseEntry t in Entries )
                t.Serialize( writer );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    m_Owner = reader.ReadMobile();
                    goto case 0;
                case 0:
                    {
                        m_Quality = (BookQuality)reader.ReadByte();
                        m_Crafter = reader.ReadMobile();
                        m_MaxEntries = reader.ReadInt();
                        m_Level = (SecureLevel)reader.ReadInt();

                        int count = reader.ReadInt();
                        Entries = new List<DisguiseEntry>( count );
                        for( int i = 0; i < count; ++i )
                        {
                            DisguiseEntry entry = new DisguiseEntry( reader );
                            if( entry.IsValid() )
                                Entries.Add( entry );
                        }

                        break;
                    }
            }

            if( !Core.AOS )
                LootType = LootType.Regular;
        }
        #endregion

        private class NameBookEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly SketchBook m_Book;

            public NameBookEntry( Mobile from, SketchBook book )
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

        private class NameBookPrompt : Prompt
        {
            private readonly SketchBook m_Book;

            public NameBookPrompt( SketchBook book )
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
    }
}