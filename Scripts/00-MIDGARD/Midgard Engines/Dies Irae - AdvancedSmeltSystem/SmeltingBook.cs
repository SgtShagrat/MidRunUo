/***************************************************************************
 *                               SmeltingBook.cs
 *                            ----------------------
 *   begin                : 16 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;

namespace Midgard.Engines.AdvancedSmelting
{
    public class SmeltingBook : Item
    {
        private Mobile m_Owner;

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; InvalidateProperties(); }
        }

        public override string DefaultName
        {
            get { return "Smelting Book"; }
        }

        [Constructable]
        public SmeltingBook()
            : this( 0xEFA )
        {
        }

        [Constructable]
        public SmeltingBook( int itemID )
            : base( itemID )
        {
            Weight = 3.0;
            LootType = LootType.Blessed;
            Hue = 0x461;

            Layer = ( Core.AOS ? Layer.Invalid : Layer.OneHanded );
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            if( m_Owner != null )
                LabelTo( from, String.Format( "Written by: {0}", m_Owner.Name ) );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( GetWorldLocation(), 1 ) )
            {
                if( m_Owner == null )
                {
                    m_Owner = from;
                    m_Owner.SendMessage( "Now this book belongs to you." );
                }
                else if( m_Owner != from )
                {
                    m_Owner.SendMessage( "This book belongs to someone else." );
                }
                else
                {
                    from.CloseGump( typeof( SmeltingBookGump ) );
                    from.SendGump( new SmeltingBookGump(this ) );
                }
            }
        }

        #region serialization

        public SmeltingBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version

            writer.Write( m_Owner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Owner = reader.ReadMobile();
        }

        #endregion
    }
}