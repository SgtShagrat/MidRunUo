using System;

using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.JailSystem
{
    public class UnattendedMacroGump : Gump
    {
        private static int[] m_Backgrounds = new int[]
                                             {
                                                 2600, 2620, 3500, 3600, 5100, 5120, 9200, 9250, 9300, 9350, 9400, 9450
                                             };

        private static string[] m_BadPhrases = new string[]
                                             {
                                                "Non ci sono...",
                                                "Lo confesso non ci sono.",
                                                "Lo confesso sono away.",
                                                "Hey, sono away.",
                                                "Sono assente.",
                                                "Sono lontano dal PC.",
                                                "Non sto giocando.",
                                                "Lo ammetto, voglio la Jail.",
                                                "Me ne frego, non ci sono.",
                                                "Clicca EasyUo per me.",
                                                "Tutto inutile, non ci sono.",
                                                "Sono momentaneamente assente."
                                             };

        private static string[] m_GoodPhrases = new string[]
                                             {
                                                "Ci sono.",
                                                "Sono presente!",
                                                "Eccomi!",
                                                "Sono qui.",
                                                "Eccomi qua.",
                                                "Presente!",
                                                "Visto, ci sono!",
                                                "Sto giocando.",
                                                "Ci sono..ci sono!",
                                                "Visto, sono in gioco!",
                                                "Eccomi qui, ci sono!",
                                                "Ti ho visto, e sono presente",
                                                "Sono attento e sto giocando"
                                             };

        private readonly Mobile m_BadBoy;

        private readonly DateTime m_Issued = DateTime.Now;
        private readonly Mobile m_Jailor;
        private readonly UAResponseTimer m_MyTimer;
        private readonly int m_Tbutton = 2;
        private bool m_CaughtFired;

        public UnattendedMacroGump( Mobile from, Mobile m )
            : base( 70, 40 )
        {
            string jailorString = ( from == null ? "Midgard Staff" : from.Name );

            m_Tbutton = Utility.Random( 6 ) + 1;

            ( (Account)m.Account ).Comments.Add( new AccountComment( JailSystem.JSName + "-warning", string.Format(
                "{0} checked to see if {1} was macroing unattended on: {2}", jailorString, m.Name, DateTime.Now ) ) );

            m_Jailor = from;
            m_BadBoy = m;

            Closable = false;
            Dragable = true;

            AddPage( 0 );
            AddBackground( 0, 0, 320, 320, Utility.RandomList( m_Backgrounds ) );
            AddOldHtml( 10, 10, 300, 50, string.Format( "<div align=center>Lo staff di Midgard sta verificando se sei presente.</div>" ) );

            int buttonID = Utility.RandomBool() ? 2472 : 1209;

            for( int i = 0; i < 6; i++ )
            {
                int offset = Utility.Random( 10 );

                AddButton( 20 + offset, 72 + ( 40 * i ), buttonID, buttonID + 1, ( i + 1 ), GumpButtonType.Reply, 0 );
                AddLabel( 50 + offset, 75 + ( 40 * i ), 200, m_Tbutton == ( i + 1 ) ?
                    m_GoodPhrases[ Utility.Random( m_GoodPhrases.Length ) ] :
                    m_BadPhrases[ Utility.Random( m_BadPhrases.Length ) ] );
            }

            m_MyTimer = new UAResponseTimer( this );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( m_MyTimer != null )
                m_MyTimer.Stop();

            if( m_Tbutton == info.ButtonID )
            {
                try
                {
                    string mtemp = string.Format( "{0} responded to the unattended macroing check in {1} seconds.",
                                                  from.Name,
                                                  DateTime.Now.Subtract( m_Issued ).Seconds );
                    // mod by Dies Irae
                    // ( (Account)m_BadBoy.Account ).Comments.Add( new AccountComment( JailSystem.JSName + "-warning", mtemp ) );

                    if( m_Jailor != null )
                        m_Jailor.SendMessage( mtemp );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
            else
            {
                CaughtInTheAct( false );
            }

            from.CloseGump( typeof( UnattendedMacroGump ) );
        }

        public void CaughtInTheAct( bool confessed )
        {
            if( m_CaughtFired )
                return;

            if( m_BadBoy == null || m_BadBoy.Deleted || m_BadBoy.Name == null )
                return;

            m_CaughtFired = true;

            string jailorString = ( m_Jailor == null ? "Midgard Staff" : m_Jailor.Name );

            if( !confessed )
            {
                JailSystem.Jail( m_BadBoy, 1, 0, 0, JailGump.Reasons[ 0 ], true, jailorString );

                if( m_Jailor != null )
                    m_Jailor.SendMessage( "{0} has been jailed for {1} from the warning you issued.", m_BadBoy.Name,
                                        JailGump.Reasons[ 0 ] );
            }
            else
            {
                JailSystem.Jail( m_BadBoy, 1, 0, 0, JailGump.Reasons[ 0 ], true, jailorString );

                if( m_Jailor != null )
                    m_Jailor.SendMessage( "{0} was been jailed for {1} when they confessed on the warning you issued.",
                                        m_BadBoy.Name, JailGump.Reasons[ 0 ] );
            }
            if( m_MyTimer != null )
            {
                m_MyTimer.Stop();
            }
        }

        public class UAResponseTimer : Timer
        {
            private int m_Counts = 300;
            public UnattendedMacroGump Gump;

            public UAResponseTimer( UnattendedMacroGump myGump )
                : base( TimeSpan.FromSeconds( 10 ), TimeSpan.FromSeconds( 10 ) )
            {
                Gump = myGump;
                Start();
            }

            protected override void OnTick()
            {
                m_Counts -= Interval.Seconds;

                switch( m_Counts )
                {
                    case 90:
                    case 80:
                    case 70:
                    case 60:
                    case 50:
                    case 40:
                    case 30:
                    case 20:
                        Interval = TimeSpan.FromSeconds( 1 );
                        goto case 10;
                    case 10:
                    case 9:
                    case 8:
                    case 7:
                    case 6:
                    case 5:
                    case 4:
                    case 3:
                    case 2:
                    case 1:
                        Gump.m_BadBoy.SendMessage( "Warning closing in {0} seconds", m_Counts );
                        break;
                    case 0:
                        Gump.CaughtInTheAct( false );
                        Gump.m_BadBoy.CloseGump( typeof( UnattendedMacroGump ) );
                        Stop();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}