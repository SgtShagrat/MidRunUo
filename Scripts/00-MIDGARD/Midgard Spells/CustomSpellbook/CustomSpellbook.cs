using System;
using System.Collections.Generic;

using Midgard.Engines.Classes;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.SpellSystem
{
    [Flags]
    public enum BookFlag
    {
        None = 0x00000000,
        IsFull = 0x00000001,
        Closable = 0x00000002
    }

    public abstract class CustomSpellbook : Item
    {
        private static List<CustomSpellbook> m_Books = new List<CustomSpellbook>();

        public static List<CustomSpellbook> Books
        {
            get { return m_Books; }
        }

        public void RegisterBook()
        {
            if( m_Books == null )
                m_Books = new List<CustomSpellbook>();

            if( !m_Books.Contains( this ) )
                m_Books.Add( this );
        }

        public static void RemoveBooksFor( Mobile from )
        {
            if( m_Books == null )
                return;

            for( int i = 0; i < m_Books.Count; i++ )
            {
                CustomSpellbook book = m_Books[ i ];
                if( book != null && !book.Deleted )
                {
                    if( book.Owner != null && book.Owner == from )
                        book.Delete();
                }
            }
        }

        private BookFlag m_BookFlags;

        [CommandProperty( AccessLevel.GameMaster )]
        public SchoolInfo CurrentSchool { get; set; }

        public List<IconInfo> DragableIcons { get; private set; }

        public Dictionary<int, bool> SpellContents { get; private set; }

        public List<SpellKeyword> Keywords { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int StartX { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int StartY { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Skin { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int RegX { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int RegY { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int SpellsOffset { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int OptionsOffset { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int CurrentSpell { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public PlayerMobile Owner { get; internal set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Closable
        {
            get { return GetFlag( BookFlag.Closable ); }
            set { SetFlag( BookFlag.Closable, value ); }
        }

        public override bool DisplayLootType
        {
            get { return true; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int TotalNumberOfSpells
        {
            get
            {
                if( SpellContents == null )
                    return 0;

                int count = 0;

                foreach( KeyValuePair<int, bool> kvp in SpellContents )
                {
                    if( kvp.Value )
                        count++;
                }

                return count;
            }
        }

        public abstract MidgardClasses BookClass { get; }

        public abstract SchoolInfo MainBookSchool { get; }

        protected CustomSpellbook()
            : this( 3834 )
        {
        }

        protected CustomSpellbook( int itemID )
            : base( itemID )
        {
            Weight = 1.0;
            Layer = Layer.OneHanded;

            Owner = null;
            Skin = RegX = RegY = 0;
            StartX = StartY = 50;
            Closable = true;

            DragableIcons = new List<IconInfo>();
            SpellContents = new Dictionary<int, bool>();
            Keywords = new List<SpellKeyword>();

            CurrentSchool = SchoolInfo.SchoolList[ 0 ];
            CurrentSpell = -1;

            AddSchool( MainBookSchool );
            RegisterBook();
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !ClassCheck( from ) )
            {
                from.SendMessage( from.Language == "ITA" ? "Non puoi usare questo potente artefatto." : "You cannot use this powerfull artifact." );
                return;
            }

            OwnerCheck( from );
        }

        public override bool HandlesOnSpeech
        {
            get { return true; }
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( e.Mobile != Owner )
                return;

            if( Keywords == null )
                return;

            foreach( SpellKeyword keyword in Keywords )
            {
                if( !Insensitive.Equals( keyword.Keyword, e.Speech ) )
                    continue;

                if( keyword.SpellID == -1 )
                {
                    Owner.SendMessage( "Somehow that keyword has a bad Spell Registry. Removing Keyword!" );
                    Keywords.Remove( keyword );
                    return;
                }
                else
                {
                    RPGSpellsSystem.CastSpellByID( keyword.SpellID, Owner );
                    e.Blocked = true;
                    e.Handled = true;
                }
            }

            base.OnSpeech( e );
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, 1042886, TotalNumberOfSpells.ToString() ); // ~1_NUMBERS_OF_SPELLS~ Spells
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if( Owner != null )
            {
                if( dropped is SpellScroll )
                {
                    if( AddScroll( from, dropped ) )
                    {
                        if( dropped.Amount > 1 )
                        {
                            dropped.Amount--;
                            return false;
                        }
                        else
                        {
                            dropped.Delete();
                            return true;
                        }
                    }

                    return false;
                }
                else
                {
                    PrivateOverheadMessage( MessageType.Whisper, 0, "That item cannot be placed in this book.", from.NetState );
                }
            }
            else
            {
                PrivateOverheadMessage( MessageType.Whisper, 0, "I can't accept anything till someone owns me!", from.NetState );
            }

            return false;
        }

        public override bool OnEquip( Mobile from )
        {
            if( Owner == null || Owner != from )
            {
                from.SendMessage( from.Language == "ITA" ? "Questo artefatto non ti appartiene!" : "This powerfull artifact does not belong to you!" );
                return false;
            }

            return true;
        }

        public override bool AllowEquipedCast( Mobile from )
        {
            return ( Owner != null && Owner == from );
        }

        public static List<CustomSpellbook> FindTomes( Mobile from )
        {
            if( from != null && from.Backpack != null )
            {
                List<CustomSpellbook> tomeList = from.Backpack.FindItemsByType<CustomSpellbook>( false );

                Item equippedTome = from.FindItemOnLayer( Layer.OneHanded );

                if( equippedTome != null && equippedTome is CustomSpellbook )
                {
                    if( tomeList == null )
                        tomeList = new List<CustomSpellbook>( 1 );
                    tomeList.Add( (CustomSpellbook)equippedTome );
                }
                if( tomeList == null )
                    return null;
                else if( tomeList.Count < 1 )
                    return null;
                else
                    return tomeList;
            }
            else
                return null;
        }

        public static bool TomeHasSpell( Mobile from, int spellID )
        {
            List<CustomSpellbook> tomeList = FindTomes( from );
            if( tomeList != null )
            {
                foreach( CustomSpellbook tome in tomeList )
                {
                    if( tome.ContainsSpell( spellID ) )
                        return true;
                }
            }
            return false;
        }

        private bool GetFlag( BookFlag flag )
        {
            return ( ( m_BookFlags & flag ) != 0 );
        }

        private void SetFlag( BookFlag flag, bool value )
        {
            if( value )
                m_BookFlags |= flag;
            else
                m_BookFlags &= ~flag;
        }

        public bool AddScroll( Mobile from, Item dropped )
        {
            if( SpellContents == null )
                SpellContents = new Dictionary<int, bool>();

            SpellScroll scroll = (SpellScroll)dropped;
            ExtendedSpellInfo si = SpellRegistry.GetExtendedSpellInfoByID( scroll.SpellID );

            if( si == null )
            {
                PrivateOverheadMessage( MessageType.Whisper, 0, "Unknown Spell.", from.NetState );
            }
            else if( si.School != MainBookSchool.School )
            {
                PrivateOverheadMessage( MessageType.Whisper, 0, "The book refuses this scroll.", from.NetState );
            }
            else if( ContainsSpell( scroll.SpellID ) )
            {
                from.SendLocalizedMessage( 500179 );
                // That spell is already present in that spellbook.
            }
            else
            {
                SpellContents[ scroll.SpellID ] = true;
                PrivateOverheadMessage( MessageType.Whisper, 0, "Added that spell to my memory.", from.NetState );
                InvalidateProperties();
                return true;
            }

            return false;
        }

        public void AddSchool( SchoolInfo info )
        {
            int[] ranges = info.Range;

            for( int x = 0; x < ranges.Length; x++ )
            {
                if( ContainsSpell( ranges[ x ] ) )
                    continue;

                SpellContents[ ranges[ x ] ] = true;
            }
        }

        public bool CanSpellBeCastBy( Mobile from, int spellID )
        {
            return ( Owner == from && ContainsSpell( spellID ) && RPGSpellsSystem.CanSpellBeCastBy( from, spellID, true ) );
        }

        public bool ContainsSpell( int spellID )
        {
            if( SpellContents == null )
                return false;

            ExtendedSpellInfo si = SpellRegistry.GetExtendedSpellInfoByID( spellID );
            if( si == null )
                return false;

            bool hasSpell;
            if( SpellContents.TryGetValue( spellID, out hasSpell ) )
                return hasSpell;
            else
                return false;
        }

        public void AddKeyword( string keyword, int spellID )
        {
            if( Keywords == null || Keywords.Count <= 0 )
            {
                Keywords = new List<SpellKeyword>();
                Keywords.Add( new SpellKeyword( spellID, keyword ) );
            }

            else
            {
                if( !CheckKeyword( keyword, spellID ) )
                {
                    Owner.SendMessage( Owner.Language == "ITA" ? "Quella Keyword è già in uso per un altro incantesimo." : "That Keyword is already assigned to another spell." );
                    return;
                }
                else
                {
                    if( String.IsNullOrEmpty( keyword ) )
                    {
                        Owner.SendMessage( Owner.Language == "ITA" ? "Keyword rimossa." : "Removed that keyword." );
                        return;
                    }
                    Keywords.Add( new SpellKeyword( spellID, keyword ) );
                    Owner.SendMessage( Owner.Language == "ITA" ? ("Keyword impostata : " + keyword) : ("Keyword has been set : " + keyword) );
                }
            }
        }

        public bool CheckKeyword( string keyword, int spellID )
        {
            if( Keywords == null )
                return true;

            for( int i = 0; i < Keywords.Count; i++ )
            {
                SpellKeyword key = Keywords[ i ];
                if( key == null )
                    continue;

                if( key.SpellID == spellID )
                {
                    Keywords.RemoveAt( i );
                    continue;
                }

                if( Insensitive.Equals( keyword, key.Keyword ) )
                    return false;
            }

            return true;
        }

        private void PrivateOverheadMessage( MessageType type, int hue, string text, NetState state )
        {
            if( state == null )
                return;

            state.Send( new AsciiMessage( Serial, ItemID, type, hue, 3, Name, text ) );
        }

        public bool OwnerCheck( Mobile from )
        {
            if( Owner == null && from is PlayerMobile )
            {
                Owner = from as PlayerMobile;
                from.PlaySound( 0x1F7 );
                BlessedFor = from;
                from.SendMessage( from.Language == "ITA" ? "Le scritte sull'artefatto cambiano..." : "The markings on the artifact change..." );
                return true;
            }
            else if( Owner == from )
            {
                from.CloseGump( typeof( SpellIconGump ) );
                from.CloseGump( typeof( CustomSpellbookGump ) );

                foreach( IconInfo ii in DragableIcons )
                {
                    if( ii != null )
                        from.SendGump( new SpellIconGump( this, ii.X, ii.Y, ii.Icon, ii.Spell, ii.Background ) );
                }

                from.SendGump( new CustomSpellbookGump( from, this ) );
                return true;
            }
            else
            {
                from.SendMessage( from.Language == "ITA" ? "L'artefatto non ti permette di vedere il suo contenuto." : "The artifact does not allow you to view its contents." );
                return false;
            }
        }

        public bool ClassCheck( Mobile from )
        {
            return BookClass != MidgardClasses.None && ClassSystem.Find( from ) == ClassSystem.Find( BookClass );
        }

        #region serialization

        public CustomSpellbook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 1 ); //version

            writer.WriteMobile( Owner );

            writer.WriteEncodedInt( StartX );
            writer.WriteEncodedInt( StartY );
            writer.WriteEncodedInt( Skin );
            writer.WriteEncodedInt( RegX );
            writer.WriteEncodedInt( RegY );
            writer.WriteEncodedInt( SpellsOffset );

            writer.WriteEncodedInt( DragableIcons.Count );
            foreach( IconInfo i in DragableIcons )
                i.Serialize( writer );

            // only serialize the true values
            Dictionary<int, bool> spellContents = new Dictionary<int, bool>();
            foreach( KeyValuePair<int, bool> kvp in SpellContents )
            {
                if( kvp.Value )
                    spellContents[ kvp.Key ] = kvp.Value;
            }

            writer.WriteEncodedInt( spellContents.Count );
            foreach( KeyValuePair<int, bool> kvp in spellContents )
                writer.WriteEncodedInt( kvp.Key );

            writer.WriteEncodedInt( Keywords.Count );
            foreach( SpellKeyword k in Keywords )
                k.Serialize( writer );

            writer.WriteEncodedInt( (int)m_BookFlags );

            SchoolInfo.WriteReference( writer, CurrentSchool );

            writer.WriteEncodedInt( CurrentSpell );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 1:
                    goto case 0;
                case 0:
                    OptionsOffset = 0;

                    Owner = reader.ReadMobile<PlayerMobile>();

                    if( version < 1 )
                    {
                        reader.ReadEncodedInt();
                        new AosAttributes( this, reader );
                        new AosSkillBonuses( this, reader );
                    }

                    StartX = reader.ReadEncodedInt();
                    StartY = reader.ReadEncodedInt();
                    Skin = reader.ReadEncodedInt();
                    RegX = reader.ReadEncodedInt();
                    RegY = reader.ReadEncodedInt();
                    SpellsOffset = reader.ReadEncodedInt();

                    int count = reader.ReadEncodedInt();
                    DragableIcons = new List<IconInfo>( count );
                    for( int i = 0; i < count; i++ )
                        DragableIcons.Add( new IconInfo( reader ) );

                    count = reader.ReadEncodedInt();
                    SpellContents = new Dictionary<int, bool>();
                    for( int i = 0; i < count; i++ )
                        SpellContents[ reader.ReadEncodedInt() ] = true;

                    count = reader.ReadEncodedInt();
                    Keywords = new List<SpellKeyword>( count );
                    for( int i = 0; i < count; i++ )
                        Keywords.Add( new SpellKeyword( reader ) );

                    if( version < 1 )
                    {
                        count = reader.ReadEncodedInt();
                        for( int i = 0; i < count; i++ )
                        {
                            reader.ReadString();
                            reader.ReadEncodedInt();
                        }
                    }

                    if( version < 1 )
                        reader.ReadEncodedInt();

                    m_BookFlags = (BookFlag)reader.ReadEncodedInt();

                    if( version < 1 )
                    {
                        reader.ReadInt(); // version

                        reader.ReadString();
                        reader.ReadString();
                        reader.ReadInt();
                        reader.ReadBool();
                        reader.ReadBool();
                        reader.ReadInt();
                    }
                    else
                    {
                        CurrentSchool = SchoolInfo.ReadReference( reader );
                    }

                    CurrentSpell = reader.ReadEncodedInt();
                    break;
            }

            RegisterBook();
        }

        #endregion
    }
}