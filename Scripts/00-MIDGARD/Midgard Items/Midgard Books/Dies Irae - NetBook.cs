/***************************************************************************
 *                               Old - NetBook.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using Server;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Prompts;

namespace Midgard.Items
{
    public class NetBook : Item, ISecurable, ICraftable
    {
        private BookQuality m_Quality;
        private Mobile m_Crafter;

        [CommandProperty( AccessLevel.GameMaster )]
        public string NetAddress { get; set; }

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

        [CommandProperty( AccessLevel.GameMaster )]
        public SecureLevel Level { get; set; }

        public override string DefaultName { get { return "a net book"; } }

        [Constructable]
        public NetBook()
            : this( 0xFF2 )
        {
        }

        [Constructable]
        public NetBook( int itemID )
            : base( itemID )
        {
            Weight = 1.0;
            LootType = Core.AOS ? LootType.Blessed : LootType.Regular;
            Hue = Utility.RandomNeutralHue();
        }

        public bool CheckAccess( Mobile m )
        {
            if( !IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster )
                return true;

            BaseHouse house = BaseHouse.FindHouseAt( this );

            if( house != null && house.IsAosRules && ( house.Public ? house.IsBanned( m ) : !house.HasAccess( m ) ) )
                return false;

            return ( house != null && house.HasSecureAccess( m, Level ) );
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
            {
                list.Add( 1063341 ); // exceptional

                if( m_Crafter != null )
                    list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            StringBuilder info = new StringBuilder();

            if( m_Quality == BookQuality.Exceptional )
                info.Append( "exceptional " ); // 'exceptional '

            info.Append( "net book" );

            if( Crafter != null && !String.IsNullOrEmpty( Crafter.Name ) )
                info.AppendFormat( " crafted by {0}", Crafter.Name ); // ' crafted by Dies Irae'

            LabelTo( from, info.ToString() );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from != null && ( IsChildOf( from.Backpack ) || from.InRange( this, 1 ) ) && CheckAccess( from ) )
            {
                if( !String.IsNullOrEmpty( NetAddress ) )
                    from.SendGump( new InternalGump( from, NetAddress ) );
                else
                {
                    from.Prompt = new AddressPrompt( this );
                    from.SendMessage( "Type the net address for this book." );
                }
            }
        }

        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
        {
            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            if( makersMark )
                Crafter = from;

            m_Quality = (BookQuality)( quality - 1 );

            return quality;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }

        #region serialization
        public NetBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 2 );

            // Version 2
            writer.Write( (byte)m_Quality );
            writer.Write( m_Crafter );
            writer.Write( (int)Level );

            // Version 1
            writer.Write( NetAddress );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 2:
                    {
                        m_Quality = (BookQuality)reader.ReadByte();
                        m_Crafter = reader.ReadMobile();
                        Level = (SecureLevel)reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        NetAddress = reader.ReadString();
                        break;
                    }
            }
        }
        #endregion

        private class NameBookEntry : ContextMenuEntry
        {
            private Mobile m_From;
            private NetBook m_Book;

            public NameBookEntry( Mobile from, NetBook book )
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
            private NetBook m_Book;

            public NameBookPrompt( NetBook book )
            {
                m_Book = book;
            }

            public override void OnResponse( Mobile from, string text )
            {
                if( text.Length > 40 )
                    text = text.Substring( 0, 40 );

                if( !from.CheckAlive() || !m_Book.IsChildOf( from.Backpack ) )
                    return;

                m_Book.Name = Utility.FixHtml( text.Trim() );

                from.SendMessage( "This Net Book name has been changed" );
            }

            public override void OnCancel( Mobile from )
            {
            }
        }

        private class AddressPrompt : Prompt
        {
            private NetBook m_Book;

            public AddressPrompt( NetBook book )
            {
                m_Book = book;
            }

            public override void OnResponse( Mobile from, string text )
            {
                if( text.Length > 40 )
                    text = text.Substring( 0, 40 );

                if( !from.CheckAlive() || !m_Book.IsChildOf( from.Backpack ) )
                    return;

                m_Book.NetAddress = Utility.FixHtml( text.Trim() );

                from.SendMessage( "This Net Book address has been changed" );
            }

            public override void OnCancel( Mobile from )
            {
            }
        }

        private class InternalGump : Gump
        {
            private string m_NetAddress;

            public InternalGump( Mobile owner, string address )
                : base( 25, 25 )
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                owner.CloseGump( typeof( InternalGump ) );

                m_NetAddress = address;

                AddPage( 0 );

                AddImage( 0, 0, 0x898 );
                AddButton( 64, 160, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0 ); // OK
                AddButton( 228, 160, 0xF2, 0xF1, 2, GumpButtonType.Reply, 0 ); // Cancel
                AddHtml( 36, 20, 112, 112, "This book is presented in a Web format. Do you wish to view it now?", false, false );
                AddHtml( 200, 20, 112, 112, m_NetAddress, false, false );
            }

            public override void OnResponse( NetState state, RelayInfo info )
            {
                if( info.ButtonID == 1 )
                    state.Mobile.LaunchBrowser( m_NetAddress );
            }
        }
    }
}