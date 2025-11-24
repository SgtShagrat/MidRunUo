/***************************************************************************
 *                               EmailRegistrationGump.cs
 *
 *   begin                : 02 ottobre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Gumps;
using Server.Misc;
using Server.Network;

namespace Midgard.Gumps
{
    public class EmailRegistrationGump : Gump
    {
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler( OnLogin );
        }

        private static void OnLogin( LoginEventArgs e )
        {
            if( !e.Mobile.Player )
                return;

            if( e.Mobile.Account != null && string.IsNullOrEmpty( e.Mobile.Account.Email ) )
                Timer.DelayCall( TimeSpan.FromSeconds( 10.0 ), new TimerStateCallback( Notify ), e.Mobile );
        }

        private static void Notify( object state )
        {
            Mobile m = state as Mobile;
            if( m == null )
                return;

            m.SendGump( new EmailRegistrationGump( m ) );
        }

        public EmailRegistrationGump( Mobile m )
            : base( 50, 50 )
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage( 0 );
            AddBackground( 0, 0, 355, 160, 9200 );

            bool eng = m.TrueLanguage == LanguageType.Eng;

            if( eng )
            {
                AddLabel( 15, 15, 0, "Please register your account by email." );
                AddLabel( 15, 40, 0, "This will help us keep your account secure." );
            }
            else
            {
                AddLabel( 15, 15, 0, "Per favore registra l'email del tuo account." );
                AddLabel( 15, 40, 0, "Fallo per la sicurezza del tuo account." );
            }

            AddLabel( 15, 70, 0, "Email:" );
            AddLabel( 15, 95, 0, eng ? "Repeat:" : "Ripetila:" );

            // text entry background
            AddImage( 65, 65, 1141 );
            AddImage( 65, 95, 1141 );

            // email and verification
            AddTextEntry( 75, 70, 252, 20, 0, 2, "" );
            AddTextEntry( 75, 100, 252, 20, 0, 3, "" );

            // okay and cancel button
            AddButton( 115, 130, 247, 248, 1, GumpButtonType.Reply, 0 );
            AddButton( 190, 130, 241, 242, 0, GumpButtonType.Reply, 0 );

            // Midgard Logo
            AddImage( 288, 17, 174 );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( info.ButtonID != 1 )
                return;

            string email = info.GetTextEntry( 2 ).Text;
            string verification = info.GetTextEntry( 3 ).Text;

            Mobile m = sender.Mobile;

            if( email != verification )
            {
                sender.Mobile.SendMessage( m.TrueLanguage == LanguageType.Eng ? "Emails does not match." : "Le due email non coincidono." );
                return;
            }

            if( !Email.IsValid( email ) )
            {
                sender.Mobile.SendMessage( m.TrueLanguage == LanguageType.Eng ? "Email enetered is invalid." : "La mail inserita non e' valida." );
                return;
            }

            sender.Mobile.Account.Email = email;
            sender.Mobile.SendMessage( m.TrueLanguage == LanguageType.Eng ? "Email registered." : "Email registrata." );
        }
    }
}