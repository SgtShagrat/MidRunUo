/***************************************************************************
 *                               MailScroll.cs
 *
 *   begin                : 31 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Mobiles;

namespace Midgard.Engines.MailSystem
{
    public class MailScroll : Item
    {
        [Constructable]
        public MailScroll()
            : base( 0x227B )
        {
            Message = new MailMessage();
            Weight = 1.0;
        }

        public override string DefaultName
        {
            get { return "a mail scroll"; }
        }

        public MailMessage Message { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public MidgardTowns Town { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsCompleted
        {
            get { return Message != null && Message.From != null && Message.To != null; }
        }

        public virtual bool CanBeUsedBy( Mobile from, bool message )
        {
            if( from == null )
                return false;          
            else if( !from.CheckAlive( false ) )
            {
                if( message )
                    from.SendMessage( "Thou cannot do this while dead." );
                return false;
            }
            else if( from.Backpack == null || !IsChildOf( from.Backpack ) )
            {
                if( message )
                    from.SendMessage( "The scroll must be in your backpack." );
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Send( Mobile from )
        {
            Core.Instance.RegisterMessage( Town, Message );
            from.SendMessage( "Your message has been sent to the mailbox of {0}.", TownHelper.FindTownName( Town ) );

            try
            {
                TextWriter tw = File.AppendText( "Logs/Mails.txt" );

                tw.WriteLine( "Mail mandata dal pg {0} (account {1}), al pg {2} (account {3}) in data {4} alle ore {5}. Contenuto: {6}",
                              Message.From.Name, Message.From.Account.Username, Message.To.Name, Message.To.Account.Username,
                              DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(), string.Format( "{0}: {1}", Message.Title, Message.Text ) );

                tw.Close();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogWarningLine( ex.ToString() );
            }

            if( !Deleted )
                Delete();
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( Message.From == null )
                Message.From = from;

            from.SendMessage( "Feel free to review this mail." );
            from.SendGump( new MailCompositionGump( from, this ) );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( IsOwner( from ) )
            {
                if( IsCompleted )
                    LabelTo( from, "a mail from {0} to {1}", Message.From.Name, Message.To.Name );
                else
                    LabelTo( from, "a mail" );
            }
            else
                LabelTo( from, "a mail" );
        }

        public bool IsOwner( Mobile from )
        {
            return Message != null && Message.From != null && Message.From == from;
        }

        #region serialization
        public MailScroll( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( (int) Town );
            Message.Serialize( writer );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Town = (MidgardTowns) reader.ReadInt();
            Message = new MailMessage( reader );
        }
        #endregion
    }
}