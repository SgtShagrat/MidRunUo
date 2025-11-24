/***************************************************************************
 *                               ReceivedMailScroll.cs
 *
 *   begin                : 31 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.MailSystem
{
    public class ReceivedMailScroll : Item
    {
        private MailMessage m_Message;

        public ReceivedMailScroll( MailMessage message )
            : base( 0x227B )
        {
            m_Message = message;

            Weight = 1.0;
        }

        public override string DefaultName
        {
            get { return "a mail scroll"; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile From
        {
            get { return m_Message.From; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile To
        {
            get { return m_Message.To; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public string Text
        {
            get { return m_Message.Text; }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from == To || from.AccessLevel > AccessLevel.GameMaster )
                from.SendGump( new DisplayMailMessageGump( m_Message ) );
            else
                from.SendMessage( "You are not allowed to view its content." );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( From != null && To != null && from == To )
                LabelTo( from, "a mail from {0} to {1}", From.Name, To.Name );
            else
                LabelTo( from, "a mail" );
        }

        #region serialization
        public ReceivedMailScroll( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            m_Message.Serialize( writer );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Message = new MailMessage( reader );
        }
        #endregion
    }
}