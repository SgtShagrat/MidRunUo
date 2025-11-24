/***************************************************************************
 *                               BaseMailBox.cs
 *
 *   begin                : 31 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MailSystem
{
    public abstract class BaseMailBox : Item
    {
        private MidgardTowns m_Town;

        public BaseMailBox( int itemID )
            : base( itemID )
        {
            Weight = 100.0;

            Core.Instance.RegisterBox( this );
        }

        public abstract int EmptyItemID { get; }
        public abstract int FullItemID { get; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsEmpty
        {
            get { return Town == MidgardTowns.None || Core.Instance.GetMailsByTown( Town ) == null; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public MidgardTowns Town
        {
            get { return m_Town; }
            set
            {
                MidgardTowns oldValue = m_Town;

                if( oldValue != value )
                {
                    m_Town = value;
                    OnTownChanged( oldValue );
                }
            }
        }

        public TownSystem System
        {
            get { return TownSystem.Find( m_Town ); }
        }

        public virtual void OnTownChanged( MidgardTowns oldValue )
        {
            InvalidateProperties();
        }

        public virtual void InvalidateItemId( bool full )
        {
            if( full && ItemID != FullItemID )
                ItemID = FullItemID;
            else if( !full && ItemID == FullItemID )
                ItemID = EmptyItemID;

            if( Config.Debug )
                PublicOverheadMessage( MessageType.Regular, 0x37, true, string.Format( "I have {0}messages.", full ? "" : "no " ) );
        }

        public override void OnDoubleClick( Mobile from )
        {
            base.OnDoubleClick( from );

            if( Town != MidgardTowns.None )
            {
                List<MailMessage> mails = Core.Instance.GetMailsByTownForMobile( Town, from );
                if( mails != null )
                {
                    foreach( MailMessage message in mails )
                    {
                        message.Deliver( from, Town );
                        Core.Instance.UnRegisterMessage( Town, message );
                    }
                }
                else
                    from.SendMessage( "There are no mails for you at the moment." );
            }
            else
                from.SendMessage( "This mailbox is not available at the moment." );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( IsEmpty )
                LabelTo( from, "an empty mailbox" );
            else
            {
                List<MailMessage> list = Core.Instance.GetMailsByTown( Town );
                if( list != null && list.Count > 0 )
                    LabelTo( from, "a mailbox with {0} messages within", list.Count );
            }
        }

        #region serialization
        public BaseMailBox( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( (int)m_Town );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Town = (MidgardTowns)reader.ReadInt();

            Core.Instance.RegisterBox( this );
        }
        #endregion
    }
}