/***************************************************************************
 *                                	 HardLabourNotifierGump.cs
 *
 *  begin                	: Gennaio, 2007
 *  version					: 0.1
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server.Gumps;

namespace Midgard.Engines.HardLabour
{
    public class HardLabourNotifierGump : Gump
    {
        private const string Msg = "Sei stato condannato ai <b>Lavori Forzati</b> <br><br>" +
                                    "Per espiare la tua pena dovrai lavorare in queste miniere, " +
                                    "estraendo minerale di ferro, per un certo periodo di tempo. " +
                                    "Quanto piu' ti darai da fare col piccone tanto prima il carceriere stabilira' " +
                                    "che il tuo lavoro è sufficiente a fare ammenda dei crimini da te commessi.<br>" +
                                    "Se il tuo piccone si danneggia puoi chiederne uno nuovo al carceriere chiedendo: <br>" +
                                    "<i>Please Master Give Me a Pickaxe</i><br><br>" +
                                    "Se dovessi morire di stenti, puoi tornare in vita rivolgendoti al carceriere nel modo seguente:<br>" +
                                    "<i>Please Master Ress Me</i><br><br>" +
                                    "Se dovessi finire le razioni standard di cibo chiedine altre al carceriere dicendo: <br>" +
                                    "<i>Please Master Give Me Something To Eat</i><br><br>" +
                                    "In ogni momento potrai rivisualizzare questo decreto di condanna dicendo al carceriere: <br>" +
                                    "<i>Please Master Help Me</i><br><br>" +
                                    "<b>Attenzione</b><br>I beni che ti sono stati confiscati, " +
                                    "li potrai ritirare presso la tua cassetta in banca al terminde della pena.<br>" +
                                    "La tua eventuale cavalcature e' custodita presso le stalle. <br>" +
                                    "Se dovessi riuscire ad evadere, e se sarai nuovamente condannato dovrai espiare " +
                                    "anche la pensa residua.<br>" +
                                    "Se trovato a 'macrare away' durante lo sconatare della pena, essa verra' quadruplicata.<br>";

        private int m_Pena;

        public HardLabourNotifierGump( int pena )
            : base( 50, 50 )
        {
            m_Pena = pena;

            Closable = false;
            Disposable = false;
            Dragable = false; ;
            Resizable = false;

            AddPage( 0 );
            AddImageTiled( 54, 43, 386, 343, 2624 );
            AddAlphaRegion( 54, 43, 386, 343 );
            AddImage( 79, 71, 9781 );
            AddImageTiled( 64, 19, 363, 29, 9391 );
            AddImageTiled( 52, 34, 6, 340, 9203 );
            AddImageTiled( 37, 19, 102, 29, 9390 );
            AddImageTiled( 48, 374, 371, 29, 9391 );
            AddImageTiled( 36, 374, 105, 29, 9390 );
            AddImageTiled( 437, 34, 6, 344, 9203 );
            AddImageTiled( 345, 19, 110, 29, 9392 );
            AddImageTiled( 339, 374, 114, 29, 9392 );
            AddImageTiled( 66, 24, 367, 17, 10101 );
            AddImage( 425, 24, 10104 );
            AddImage( 2, 11, 10440 );
            AddHtml( 118, 75, 200, 30, "<BASEFONT COLOR=#C0C0EE>Lavori Forzati - (Hard Labour)</BASEFONT>", false, false );
            AddImageTiled( 82, 107, 190, 1, 9203 );
            AddImage( 369, 57, 223 );
            AddButton( 212, 342, 2313, 2312, 55, GumpButtonType.Reply, 0 );

            AddHtml( 74, 124, 349, 209, Msg, false, true );
        }
    }
}

#region TODO: Eventuale gump complesso
//	 public class LavoriForzatiGump : Gump
//	{
//		private Mobile m_Condannato;	// Il condannato
//		private Mobile m_Judge;			// Colui che condanna
//		private int m_page;
//		private bool m_return;
//		
//		public LavoriForzatiGump(Mobile guardia, Mobile ladro, int page, string error) : base( 100, 40 )
//		{
//			BuildGumpz( guardia,  ladro, page, error,  "0" ,true);
//		}
//		
//		public LavoriForzatiGump(Mobile guardia, Mobile ladro, int page, string error, string reason, string numero_minerali, bool fullreturn ) : base( 100, 40 )
//		{
//			BuildGumpz(guardia,ladro, page, error,  numero_minerali, fullreturn );
//		}
//		
//		public void BuildGumpz(Mobile guardia, Mobile ladro, int page, string error, string reason, string numero_minerali, bool fullreturn )
//		{
//		m_Judge			= guardia;
//	    m_Condannato	= ladro;
//		m_page			= page;
//		m_return		= fullreturn;
//		m_Numero		= numero_minerali;
//		
//		m_Judge.CloseGump( typeof ( LavoriForzatiGump ) );
//		Closable = false;
//		Dragable = false;
//		AddPage(0);
//		AddBackground( 0, 0, 300, 120, 5054);
//		AddImageTiled( 10, 10, 280, 100, 2624 );
//		AddLabel( 16, 98, 200, "Ammontare della Pena");
//		AddBackground( 14, 114, 270, 24, 0x2486 );
//		//AddTextEntry( 18, 116, 272, 20, 200, 0,m_Numero );
//		}
//	}
#endregion