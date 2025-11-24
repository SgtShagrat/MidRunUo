/***************************************************************************
 *                               BaseClassPowersBook.cs
 *                            ----------------------
 *   begin                : 16 Aprile, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;

namespace Midgard.Engines.Classes
{
    public abstract class BaseClassPowersBook : Item
    {
        private static List<BaseClassPowersBook> m_Books = new List<BaseClassPowersBook>();

        // no longer needed...
        /*[CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        private Mobile m_Owner;*/

        protected BaseClassPowersBook()
            : this( 0xEFA )
        {
        }

        protected BaseClassPowersBook( int itemId )
            : base( itemId )
        {
            Weight = 3.0;
            Hue = 0x461;
            Layer = Layer.OneHanded;
        }

        public abstract ClassSystem BookSystem { get; }

        public override string DefaultName
        {
            get { return String.Format( "{0} Book", BookSystem.Definition.ClassName ?? "Class" ); }
        }

        public static List<BaseClassPowersBook> Books
        {
            get { return m_Books; }
        }

        public void RegisterBook()
        {
            if( m_Books == null )
                m_Books = new List<BaseClassPowersBook>();

            if( !m_Books.Contains( this ) )
                m_Books.Add( this );
        }

        public override void OnDoubleClick( Mobile from )
        {
            ClassPlayerState state = ClassPlayerState.Find( from );
            if( state == null || state.ClassSystem != BookSystem )
            {
                from.SendMessage( from.Language == "ITA" ? "Il tomo non ti permette di vedere il suo contenuto." : "The tome does not allow you to view its contents." );
                return;
            }

            /* if( m_Owner == null && from is PlayerMobile )
            {
                m_Owner = from;
                from.PlaySound( 0x1F7 );
                BlessedFor = from;
                from.SendMessage( "The markings on the tome change..." );
            } */

            //if( m_Owner == from )
            //{
            if( from.InRange( GetWorldLocation(), 1 ) )
            {
                from.CloseGump( typeof( ClassRitualGump ) );
                from.SendGump( new ClassRitualGump( state, this, BookSystem ) );
            }
            //}
            //else
            //    from.SendMessage( "The tome does not allow you to view its contents." );
        }

        #region serialization
        public BaseClassPowersBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version

            // writer.Write( m_Owner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( version < 1 )
                reader.ReadMobile();

            RegisterBook();
        }
        #endregion
    }
}