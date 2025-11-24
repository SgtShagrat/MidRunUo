/***************************************************************************
 *                                  MidgardExportPG.cs
 *                            		---------------------
 *  begin               	: Agosto, 2006
 *  version					: 1.9
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
  * 
  *		Info:
  * 	Il comando [monoaccount <nome_account> <password> <mail>
  * 	permette ai singoli pg di portare sul server 2.0 fino a 6 pg.
  * 	Il comando [MonoaccountCreateMaster <nome_account> 
  * 	setta un account come Master per il porting.
  * 	Il comando [MonoAccountSetPgEsportabile 
  * 	setta un pg di nuovo esportabile.
  * 
  *  	History:
  * 	-1.1	Aggiunti Notice e Warning Gump
  * 	-1.2 	Aggiunte CallBack opportune
  * 	-1.3	Aggiunto Attributo e verifica Mail
  * 	-1.4	Aggiunto Log degli usi
  * 	-1.5	Aggiunto controllo PgEsportabili
  * 	-1.6 	Aggiunto controllo MasterAccount
  * 	-1.7	Aggiungi comandi MonoAccountCreateMaster e 
  * 			MonoAccountSetPgEsportabile
  *		-1.8	Fix vari
  * 	-1.9	Aggiunto Skill.Cap
  * 	-1.10 	Aggiunto try/catch sul nome del file
  * 
 ***************************************************************************/
 
using System;
using System.IO;
using System.Text;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using System.Xml;

namespace Midgard.Commands
{
   	public class MidgardExportPG
   	{
   		#region Registrazione
   		public static void Initialize()
      	{
         	CommandSystem.Register( "MonoAccount", AccessLevel.Developer, new CommandEventHandler( Monoaccount_OnCommand ) );
         	CommandSystem.Register( "MonoAccountCreateMaster", AccessLevel.Developer, new CommandEventHandler( MonoaccountCreate_OnCommand ) );
         	CommandSystem.Register( "MonoAccountSetPgEsportabile", AccessLevel.Developer, new CommandEventHandler( MonoaccountSetPgEsportabile_OnCommand ) );
      	}
		#endregion
		
		#region Callback
      	[Usage( "MonoAccountSetPgEsportabile <nome_account> <Seriale> " )]
   		[Description( "Consente di settare un pg esportabile conoscendone l'account e il seriale")]
      	public static void MonoaccountSetPgEsportabile_OnCommand( CommandEventArgs e )
      	{   			
      		string UserName = string.Empty;
      		string Seriale = string.Empty;
      		Mobile From = e.Mobile;
      		Account a = null;
      		
      		if( From == null | e.Length != 2 )
      		{
				From.SendGump( new NoticeGump( 1060635, 30720, "Almeno uno dei due parametri non è corretto." +
                               " Il formato corretto del comando è: " +
                               "\n [MonoAccountSetPgEsportabile nome_account Seriale.",
                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
      			return;
      		}
      		
      		UserName = e.GetString(0);
   			Seriale = e.GetString(1);
      		
   			foreach ( Account check in Accounts.GetAccounts() )
   			{
   				if( check.Username.ToLower() == UserName )
   					a = check;
   			}
   			
     		if( a == null )
   			{
     			From.SendGump( new NoticeGump( 1060635, 30720, "Questo account non esiste.",
                                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
   				return;
     		}
   			
 			for( int i = 0; i < a.Tags.Count; ++i )
			{
				AccountTag tag = (AccountTag)a.Tags[i];
				if( tag.Name == "PgEsportato" && tag.Value == Seriale )
				{
					a.RemoveTag(tag.Name);
					From.SendGump( new NoticeGump( 1060635, 30720, "Il pg " + From.Name + " è di nuovo esportabile.",
                                                   0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
					return;
				}
			}
			From.SendGump( new NoticeGump( 1060635, 30720, "Il seriale non è corretto.",
                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
      	}
      	
     	[Usage( "MonoaccountCreateMaster <nome_account> " )]
   		[Description( "Consente di settare un account come master per il porting")]
      	public static void MonoaccountCreate_OnCommand( CommandEventArgs e )
      	{ 	
      		string UserName = string.Empty;
      		Mobile From = e.Mobile;
      		Account a = null;
      		
      		if( From == null | e.Length != 1 )
  				return;
      		
  			UserName = e.GetString(0);
      		
  			foreach ( Account check in Accounts.GetAccounts() )
   			{
   				if( check.Username.ToLower() == UserName )
   					a = check;
   			}
   			
   			if( a == null )
   			{
				From.SendGump( new NoticeGump( 1060635, 30720, "Questo account non esiste.",
                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
   				return;
   			}
   			a.AddTag( "MonoAccountMaster" , "true" );
   			From.SendGump( new NoticeGump( 1060635, 30720, "Tag aggiunta all'account: " + a.ToString(),
                           0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );			
      	}
   		
   		[Usage( "monoaccount <nome_account> <password_nuova> <mail_nuova>" )]
   		[Description( "Consente di esportare i personaggi per il monoaccount")]
      	public static void Monoaccount_OnCommand( CommandEventArgs e )
      	{
  			string MonoAccount = string.Empty;
  			string MonoPassword = string.Empty;
  			string MonoMail = string.Empty;
  			string FileName = string.Empty;
  			string FilePath = string.Empty;
  			bool MonoAccountMaster = false;
  			
  			Mobile From = e.Mobile;  			
  			Account a = (Account)From.Account;
  			
			string NomeFileLog = @"monoaccount/" + "LogUsiMonoaccount.txt";
  			TextWriter tw = File.AppendText(NomeFileLog);
			try
			{
				tw.WriteLine( "L'utente: " + From.Name + " (Account: " + From.Account + " ) ha usato il comando [monoaccount" +
				           	  " alle ore: " + DateTime.Now.ToShortTimeString() + " del " +
				              DateTime.Now.Date.ToShortDateString() + "." );
			}
			finally
			{
				tw.Close();
			}

  			if( From != null && e.Length == 3 )
  			{
  				MonoAccount = e.GetString(0);
  				MonoPassword = e.GetString(1);
  				MonoMail = e.GetString(2);
  				
  				FileName = MonoAccount + ".xml";
  				try
  				{
  					FilePath = Path.Combine("monoaccount", FileName);
  				}
  				catch
  				{
   					From.SendGump( new NoticeGump( 1060635, 30720, @"Hai inserito almeno un carattere non valido (ad esempio" +
  					                               " . $ ^ { [ ( | ) * + ? \\  ). Tali caratteri sono proibiti nel nome dell'account. Se il tuo account " +
  					                               " contiene davvero un carattere speciale contatta lo staff per risolvere il problema manualmente. ",
  					                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
      				return; 					
  				}
  				
  				if( MonoAccount == string.Empty | MonoPassword == string.Empty | MonoMail == string.Empty ) 
  				{
  					From.SendGump( new NoticeGump( 1060635, 30720, "Almeno uno dei parametri non è corretto." +
  					                               " Il formato corretto del comando è: " +
  					                               "\n [monoaccount nome_account password_nuova mail_nuova.",
  					                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
      				return;
  				}
  			}
  			else
			{
				From.SendGump( new NoticeGump( 1060635, 30720, "Almeno uno dei parametri non è corretto." +
				                               " Il formato corretto del comando è: " +
				                               "\n [monoaccount nome_account password_nuova mail_nuova.",
				                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
				return;
			}
  			
  			// Controlla se il pg loggato e' esportabile
  			bool PgEsportabile = true;
			for( int i = 0; i < a.Tags.Count; ++i )
			{
				AccountTag tag = (AccountTag)a.Tags[i];
				if( tag.Name == "PgEsportato" && tag.Value == From.Serial.ToString() )
					PgEsportabile = false;
			}				
			if( !PgEsportabile )
			{
				From.SendGump( new NoticeGump( 1060635, 30720, "Questo personaggio è stato già esportato." +
				                               " Se non doveva esserlo contatta l'Amministratore di Midgard.",
				                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
				return;
			} 		
  			
			// Se il pg è esportabile crea un file Xml.
  			XmlDocument Doc = new XmlDocument();
  			
  			if ( File.Exists(FilePath) )
  			{
				Doc.Load(FilePath);
				XmlElement Root = Doc.DocumentElement;
				
				// Se il file esiste ma la password o la mail registrate non coincidono non procede.
				if( !(MonoPassword == Root.GetAttribute("password")) | !(MonoMail == Root.GetAttribute("mail")) )
				{
					From.SendGump( new NoticeGump( 1060635, 30720, "Il nuovo account esiste già ma la passwordo o" +
					                               " la mail inserite nel comando non sono valide. " +
              									   "\nSe ci sono errori nella creazione del nuovo account contattare" +
              									   " un Amministratore di Midgard.", 0xFFC000, 420, 280, 
              									   new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
					return;
				}
				   
				// Controlla se ha almeno una Slot libera
				if ( Root != null && Root.GetElementsByTagName("slot") != null )
				{
					int SlotOccupate = Root.GetElementsByTagName("slot").Count;
					if( SlotOccupate < 6 )
					{
						StringBuilder sb = new StringBuilder();
						sb.Append( "Il monoaccount ha già " );
						sb.Append( Root.GetElementsByTagName("slot").Count.ToString() );
						sb.Append( " slots occupate. " );
						for( int i=0; i<SlotOccupate; i++ )
						{
							sb.Append( "\nLa slot numero: " + (i+1).ToString() + " contiene il Pg di nome: " +
							           (Root.GetElementsByTagName("RawName")[i]).InnerText + "." );
						}
						sb.Append( "\nVuoi aggiungere <em><basefont color=red>permanentemente</basefont></em> questo player " );
						sb.Append( "(" + From.RawName + ") al nuovo account?" );
						From.SendGump( new WarningGump( 1060635, 30720, sb.ToString(),0xFFC000, 420, 280, 
						                                new WarningGumpCallback( ConfirmAddSlotCallBack ),
					                                    new object[]{ FilePath, Doc, SlotOccupate } ) );
					}
					else
					{
						From.SendGump( new NoticeGump( 1060635, 30720, "Il monoaccount ha già sei (6) slots occupate." +
						                               "\nNon possono essere aggiunte altre slots. Se ci sono errori nella " +
						                               "creazione del nuovo account contattare un Amministratore di Midgard.",
						                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
					}
				}
  			}
  			else
  			{    
  				// Se il file Xml non esiste controlla se c'e' la Tag MonoAccountMaster = true, altrimenti non procede.
  				for( int i = 0; i < a.Tags.Count; ++i )
				{
					AccountTag tag = (AccountTag)a.Tags[i];
					if( tag.Name == "MonoAccountMaster" && tag.Value == "true" )
						MonoAccountMaster = true;
				}
				if( !MonoAccountMaster )
				{
					From.SendGump( new NoticeGump( 1060635, 30720, "Questo account non e' un Account Master. " +
					                               " Se doveva esserlo contatta l'Amministratore di Midgard.",
					                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
					return;
				}  
				
				// Controlla se User e Password coincidono con quelle dell'account Master.
				if( MonoAccount != a.Username.ToLower() | !a.CheckPassword( MonoPassword ) )
			    {
   					From.SendGump( new NoticeGump( 1060635, 30720, "Almeno uno dei parametri non è corretto." +
	                               "\nDevi inserire l'ID dell'account Master, la sua attuale password e una mail "+
	                               "valida a cui assocuare il nuovo account ",
	                               0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
					return;
			    }
				
				// Se è andato tutto bene crea un file Xml per l'account Master.
				DateTime DataCreazione = a.Created;
				
				int NumeroRewPresi = 0;
				for( int i = 0; i < a.Tags.Count; ++i )
				{
					AccountTag RewTag = (AccountTag)a.Tags[i];
					if( RewTag.Name == "numRewardsChosen" )
						NumeroRewPresi = Convert.ToInt32( RewTag.Value );
				}
				
				From.SendGump( new WarningGump( 1060635, 30720, "Nessun account esiste con questo nome." +
  				                                "\nVuoi <em><basefont color=red>permanentemente</basefont></em>" +
  				                                " creare il monoaccount: " + MonoAccount +
  				                                " con password: " + MonoPassword + " e mail: " + MonoMail + "?",
                               				    0xFFC000, 420, 280, new WarningGumpCallback( CreateXmlCallBack ), 
                               				    new object[]{ FilePath, MonoAccount, MonoPassword, MonoMail, Doc, DataCreazione,  NumeroRewPresi } ) );
      		}
      	}
      	
      	public static void AddSlotElement( Mobile From, string FilePath, XmlDocument Doc, int UltimaSlotOccupata )
      	{
      		XmlDocument m_Doc = Doc;
      		Account a = (Account)From.Account;
      		PlayerMobile m_PlMo = (PlayerMobile)From;
      		
			// Crea l'elemento Slot e gli attacca l'atributo ID
      		XmlElement NewSlot = m_Doc.CreateElement("slot");
      		XmlAttribute IdSlot = m_Doc.CreateAttribute("ID");
      		IdSlot.Value = (UltimaSlotOccupata + 1).ToString();
      		NewSlot.SetAttributeNode(IdSlot);
      			
      		#region Props di PlayerMobile e da Mobile
      		
      		// Props sensibili di PlayerMobile
       		XmlElement Glassblowing = m_Doc.CreateElement("Glassblowing");
      		Glassblowing.InnerText = m_PlMo.Glassblowing.ToString();
      		NewSlot.AppendChild(Glassblowing);
 
      		XmlElement Masonry = m_Doc.CreateElement("Masonry");
      		Masonry.InnerText = m_PlMo.Glassblowing.ToString();
      		NewSlot.AppendChild(Masonry);
      		
       		XmlElement SandMining = m_Doc.CreateElement("SandMining");
      		SandMining.InnerText = m_PlMo.SandMining.ToString();
      		NewSlot.AppendChild(SandMining);     		
      		
       		XmlElement StoneMining = m_Doc.CreateElement("StoneMining");
      		StoneMining.InnerText = m_PlMo.StoneMining.ToString();
      		NewSlot.AppendChild(StoneMining);
      		
      		// Props sensibili di Mobile
      		XmlElement AccessLevel = m_Doc.CreateElement("AccessLevel");
      		AccessLevel.InnerText = From.AccessLevel.ToString();
      		NewSlot.AppendChild(AccessLevel);
      		
      		// ToShortTimeString()
      		
      		XmlElement BodyValue = m_Doc.CreateElement("BodyValue");
      		BodyValue.InnerText = From.BodyValue.ToString();
      		NewSlot.AppendChild(BodyValue);
   			
      		XmlElement CreationTime = m_Doc.CreateElement("CreationTime");
      		CreationTime.InnerText = From.CreationTime.ToString();
      		NewSlot.AppendChild(CreationTime);
      		
       		XmlElement Dex = m_Doc.CreateElement("Dex");
      		Dex.InnerText = From.Dex.ToString();
      		NewSlot.AppendChild(Dex);
       		
          		XmlElement Fame = m_Doc.CreateElement("Fame");
      		Fame.InnerText = From.Fame.ToString();
      		NewSlot.AppendChild(Fame);
      		
      		XmlElement Female = m_Doc.CreateElement("Female");
      		Female.InnerText = From.Female.ToString();
      		NewSlot.AppendChild(Female);
       		
      		XmlElement Followers = m_Doc.CreateElement("Followers");
      		Followers.InnerText = From.Followers.ToString();
      		NewSlot.AppendChild(Followers);
			
      		if( From.HairItemID > 0 )
      		{
	      		XmlElement HairHue = m_Doc.CreateElement("Hair.Hue");
	      		HairHue.InnerText = From.HairHue.ToString();
	      		NewSlot.AppendChild(HairHue);
      		}
			
      		XmlElement Hue = m_Doc.CreateElement("Hue");
      		Hue.InnerText = From.Hue.ToString();
      		NewSlot.AppendChild(Hue);
      	    		
       		XmlElement Karma = m_Doc.CreateElement("Karma");
      		Karma.InnerText = From.Karma.ToString();
      		NewSlot.AppendChild(Karma); 
      
      		XmlElement Kills = m_Doc.CreateElement("Kills");
      		Kills.InnerText = From.Kills.ToString();
      		NewSlot.AppendChild(Kills); 
      		      		
      		XmlElement RawDex = m_Doc.CreateElement("RawDex");
      		RawDex.InnerText = From.RawDex.ToString();
      		NewSlot.AppendChild(RawDex);
      		
       		XmlElement RawInt = m_Doc.CreateElement("RawInt");
      		RawInt.InnerText = From.RawInt.ToString();
      		NewSlot.AppendChild(RawInt);

      		XmlElement RawName = m_Doc.CreateElement("RawName");
      		RawName.InnerText = From.RawName.ToString();
      		NewSlot.AppendChild(RawName); 
      		
       		XmlElement RawStr = m_Doc.CreateElement("RawStr");
      		RawStr.InnerText = From.RawStr.ToString();
      		NewSlot.AppendChild(RawStr);     		
      		
       		XmlElement ShortTermMurders = m_Doc.CreateElement("ShortTermMurders");
      		ShortTermMurders.InnerText = From.ShortTermMurders.ToString();
      		NewSlot.AppendChild(ShortTermMurders);   
      		
      		XmlElement SkillsCap = m_Doc.CreateElement("SkillsCap");
      		SkillsCap.InnerText = From.SkillsCap.ToString();
      		NewSlot.AppendChild(SkillsCap);    		
      		 
      		for (int s=0; s<From.Skills.Length; s++)
			{
      			SkillName sn = (SkillName)s;
      			XmlElement XmlPropSk = m_Doc.CreateElement( sn.ToString());
      			XmlPropSk.InnerText = From.Skills[s].Base.ToString();
				NewSlot.AppendChild(XmlPropSk);
      			XmlElement XmlPropSkCap = m_Doc.CreateElement( sn.ToString() + ".Cap" );
      			XmlPropSkCap.InnerText = From.Skills[s].Cap.ToString();
				NewSlot.AppendChild(XmlPropSkCap);							
			}

      		XmlElement StatCap = m_Doc.CreateElement("StatCap");
      		StatCap.InnerText = From.StatCap.ToString();
      		NewSlot.AppendChild(StatCap);   
      		
      		XmlElement Title = m_Doc.CreateElement("Title");
      		Title.InnerText = From.Title;
      		NewSlot.AppendChild(Title);
      		
       		XmlElement TithingPoints = m_Doc.CreateElement("TithingPoints");
      		TithingPoints.InnerText = From.TithingPoints.ToString();
      		NewSlot.AppendChild(TithingPoints); 
      		
			for(int v=0; v<Enum.GetNames(typeof(VirtueName)).Length; v++)
			{
				VirtueName vn = (VirtueName)v;
				XmlElement XmlPropVn = m_Doc.CreateElement( vn.ToString() );
				XmlPropVn.InnerText = From.Virtues.GetValue(v).ToString();
				NewSlot.AppendChild(XmlPropVn);	
			}   
			#endregion e 
      		
      		// Aggiunta della nuova slot a DocumentElement
      		m_Doc.DocumentElement.InsertAfter( NewSlot, m_Doc.DocumentElement.LastChild );

      		FileStream FsXml = new FileStream( FilePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
      		m_Doc.Save(FsXml);
      		FsXml.Close();
      		
      		From.SendGump( new NoticeGump( 1060635, 30720, "Hai aggiunto con successo il personaggio (" + From.RawName + ")" +
      		                               " all' Account (" + m_Doc.DocumentElement.GetAttribute( "account" ) + ")" +
      		                               " che ha Password (" + m_Doc.DocumentElement.GetAttribute( "password" ) + ").",
                               			   0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeCallback ), new object[]{} ) );
      		
      		// Tagga il PG come non esportabile
      		a.AddTag( "PgEsportato" , From.Serial.ToString() );
      	}
      	
      	private static void CloseNoticeCallback( Mobile From, object state )
		{
      	}
      	
      	private static void CreateXmlCallBack( Mobile From, bool okay, object state )
      	{
      		object[] states = (object[])state;
      		
      		string FilePath = (string)states[0];
      		string MonoAccount = (string)states[1];
      		string MonoPassword = (string)states[2];
      		string MonoMail = (string)states[3];
      		XmlDocument Doc = (XmlDocument)states[4];
      		DateTime DataCreazione = (DateTime)states[5];
      		int NumRewPresi = (int)states[6];
      		
      		if( okay )
      		{
  				// Creazione dell'xml senza slots
				XmlTextWriter XmlTw = new XmlTextWriter(FilePath, null);
				XmlTw.Formatting = System.Xml.Formatting.Indented;
				XmlTw.WriteStartElement( "monoaccount" );
				XmlTw.WriteAttributeString( "account", MonoAccount );
				XmlTw.WriteAttributeString( "password", MonoPassword );
				XmlTw.WriteAttributeString( "mail", MonoMail );
				XmlTw.WriteAttributeString( "creazione", DataCreazione.ToShortDateString() );	
				XmlTw.WriteAttributeString( "rewardspresi", NumRewPresi.ToString() );
				XmlTw.WriteEndElement();
				XmlTw.Flush();
				XmlTw.Close();

				// Aggiunta della prima slot
				FileStream FsXml = new FileStream( FilePath,FileMode.Open,FileAccess.Read,FileShare.ReadWrite );
				Doc.Load(FsXml);
				FsXml.Close();
				AddSlotElement( From, FilePath, Doc, 0 );
      		}
      	}
      	
      	private static void ConfirmAddSlotCallBack( Mobile From, bool okay, object state )
      	{
      		object[] states = (object[])state;
      		
      		string FilePath = (string)states[0];
      		XmlDocument Doc = (XmlDocument)states[1];
      		int SlotOccupate = (int)states[2];
      		
      		if( okay )
      		{
      			AddSlotElement( From, FilePath, Doc, SlotOccupate );
      		}
      	}
      	#endregion
	}
}
