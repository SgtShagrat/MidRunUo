using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;
using Midgard.Engines.OldCraftSystem;
using Server;
using Server.Items;

using Core = Midgard.Engines.MyXmlRPC.Core;

namespace Midgard.Items
{
    [Furniture]
    [Flipable( 0xa9d, 0xa9e )]
    public class CraftableBookcase : BaseContainer
    {
        public static void Initialize()
        {
            // http://93.63.153.178/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getLibraryStatus
            Core.Register( "getLibraryStatus", new MyXmlEventHandler( GetLibraryStatusOnCommand ), null );
        }

        public static void GetLibraryStatusOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "GetLibraryStatus command called..." );

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "status", from bookcase in AllCases
                                                                where bookcase != null && bookcase.CaseIsPublic
                                                                from book in bookcase.Books
                                                                where book != null
                                                                select book.ToXElement() ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        private static readonly List<CraftableBookcase> m_AllCases = new List<CraftableBookcase>();

        public static List<CraftableBookcase> AllCases
        {
            get { return m_AllCases; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CaseIsPublic { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasBooks
        {
            get { return Items.Count > 0; }
        }

        public List<BaseBook> Books
        {
            get
            {
                List<BaseBook> books = new List<BaseBook>();
                foreach( Item item in Items )
                {
                    if( item == null || !( item is BaseBook ) )
                        continue;

                    BaseBook book = (BaseBook)item;
                    if( !string.IsNullOrEmpty( book.Author ) && !string.IsNullOrEmpty( book.FullContent ) )
                        books.Add( book );
                }

                return books;
            }
        }

        [Constructable]
        public CraftableBookcase()
            : base( 0xA9D )
        {
            CaseIsPublic = false;

            AllCases.Add( this );
        }

        private void InvalidatetemID()
        {
            if( HasBooks )
            {
                if( ItemID == 0xA9D )
                    TurnToFull( true );
                else if( ItemID == 0xA9E )
                    TurnToFull( false );
            }
            else
                TurnToEmpty();
        }

        private static readonly int[] m_FullBookcasesSouth = new int[] { 0xA97, 0xA98, 0xA9B };

        private static readonly int[] m_FullBookcasesEast = new int[] { 0xA99, 0xA9A, 0xA9C };

        private void TurnToFull( bool south )
        {
            ItemID = Utility.RandomList( south ? m_FullBookcasesSouth : m_FullBookcasesEast );
        }

        private void TurnToEmpty()
        {
            ItemID = Array.IndexOf( m_FullBookcasesSouth, ItemID ) > -1 ? 0xA9D : 0xA9E;
        }

        public override void AddItem( Item dropped )
        {
            base.AddItem( dropped );

            InvalidatetemID();
        }

        public override void RemoveItem( Item dropped )
        {
            base.RemoveItem( dropped );

            InvalidatetemID();
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if( dropped == null || dropped.Deleted || from == null )
                return false;

            if( ( dropped is BaseBook || dropped is GenericCraftBook ) )
                return base.OnDragDrop( from, dropped );
            else
            {
                from.SendMessage( "You can only place book in this case!" );
                return false;
            }
        }

        public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
        {
            if( item == null || item.Deleted || from == null )
                return false;

            if( ( item is BaseBook || item is GenericCraftBook ) )
                return base.OnDragDropInto( from, item, p );
            else
            {
                from.SendMessage( "You can only place book in this case!" );
                return false;
            }
        }

        public override void OnAfterDelete()
        {
            AllCases.Remove( this );

            base.OnAfterDelete();
        }

        #region serialization
        public CraftableBookcase( Serial serial )
            : base( serial )
        {
            AllCases.Add( this );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}