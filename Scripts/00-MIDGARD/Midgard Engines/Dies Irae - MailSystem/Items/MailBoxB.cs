/***************************************************************************
 *                               MailBoxB.cs
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
    public class MailBoxB : BaseMailBox
    {
        [Constructable]
        public MailBoxB()
            : base( 0x187D )
        {
        }

        public override int EmptyItemID
        {
            get { return 0x187D; }
        }

        public override int FullItemID
        {
            get { return 0x188D; }
        }

        #region serialization
        public MailBoxB( Serial serial )
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