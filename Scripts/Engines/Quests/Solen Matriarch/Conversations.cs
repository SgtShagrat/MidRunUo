using System;
using Server;

namespace Server.Engines.Quests.Matriarch
{
	public class DontOfferConversation : QuestConversation
	{
		private bool m_Friend;

		public override object Message
		{
			get
			{
				if ( m_Friend )
				{
					/* <I>The Solen Matriarch smiles as you greet her.</I><BR><BR>
					 * 
					 * It is good to see you again. I would offer to process some zoogi fungus for you,
					 * but you seem to be busy with another task at the moment. Perhaps you should
					 * finish whatever is occupying your attention at the moment and return to me once
					 * you're done.
					 */
					return 1054081;
				}
				else
				{
					/* <I>The Solen Matriarch smiles as she eats the seed you offered.</I><BR><BR>
					 * 
					 * Thank you for that seed. It was quite delicious.  <BR><BR>
					 * 
					 * I would offer to make you a friend of my colony, but you seem to be busy with
					 * another task at the moment. Perhaps you should finish whatever is occupying
					 * your attention at the moment and return to me once you're done.
					 */
					return 1054079;
				}
			}
		}

		#region mod by Magius(CHE)
		public override object MessageIta
		{
			get
			{
				if ( m_Friend )
				{
					return "<I>La Formica Madre ti sorride quando la saluti.</I><BR><BR>" +
						    "E' bello rivederti. Vorrei convertire qualche zoogi fungus per te. Ma al momento vedo che sei impegnato, forse dovresti terminare quello che stai facendo per poi tornare da me.";
				}
				else
				{
					return "<I>La Formica Madre ti sorride e mangia il seme che le hai offerto</I><BR><BR>" +
						    "Grazie per questo seme è stato delizioso.  <BR><BR>" +
							"Ti vorrei offrire la possibilità di diventare amico della mia colonia, ma sembri gia impegnato al momento in un altra missione. " +
							"Forse dovresti prima risolvere le questioni più urgenti e poi tornare da me una volta che le avrai terminate.";
				}
			}
		}
		#endregion

		public override bool Logged{ get{ return false; } }

		public DontOfferConversation( bool friend )
		{
			m_Friend = friend;
		}

		public DontOfferConversation()
		{
		}

		public override void ChildDeserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			m_Friend = reader.ReadBool();
		}

		public override void ChildSerialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (bool) m_Friend );
		}
	}

	public class AcceptConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>The Solen Matriarch looks pleased that you've accepted.</I><BR><BR>
				 * 
				 * Very good. Please start by hunting some infiltrators from the other solen
				 * colony and eliminating them. Slay 7 of them and then return to me.<BR><BR>
				 * 
				 * Farewell for now and good hunting.
				 */
				return 1054084;
			}
		}
		
		#region mod by Magius(CHE)
		public override object MessageIta
		{
			get
			{
				return "<I> La Formica Madre è enstusiasta che tu accetti.</I><BR><BR>" +
						"Benissimo. Inizia a cacciare gli \"Infiltrators\" nella mia colonia ed eliminali. " +
						"Uccidine 7 e poi torna da me." +
						"<BR><BR>" +
						"Addio, per adesso e buona caccia.";
			}
		}
		#endregion
		
		public AcceptConversation()
		{
		}

		public override void OnRead()
		{
			System.AddObjective( new KillInfiltratorsObjective() );
		}
	}

	public class DuringKillInfiltratorsConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>The Solen Matriarch looks up as you approach.</I><BR><BR>
				 * 
				 * You're back, but you have not yet eliminated 7 infiltrators from the enemy
				 * colony. Return when you have completed this task.<BR><BR>
				 * 
				 * Carry on. I'll be waiting for your return.
				 */
				return 1054089;
			}
		}
		
		#region mod by Magius(CHE)
		public override object MessageIta
		{
			get
			{
				return "<I>La Formica Madre finge di non vederti quando sei tornato.</I><BR><BR>" +
						"Sei tornato! Ma non hai eliminato 7 nemici (infiltrators) dalla mia colonia! Ritorna da me quando avrai completato la missione!<BR><BR>" +
						"Muoviti. Noi tutti aspettiamo il tuo ritorno.";
			}
		}
		#endregion

		public override bool Logged{ get{ return false; } }

		public DuringKillInfiltratorsConversation()
		{
		}
	}

	public class GatherWaterConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>The Solen Matriarch nods favorably as you approach her.</I><BR><BR>
				 * 
				 * Marvelous! I'm impressed at your ability to hunt and kill enemies for me.
				 * My colony is thankful.<BR><BR>
				 * 
				 * Now I must ask that you gather some water for me. A standard pitcher of water
				 * holds approximately one gallon. Please decant 8 gallons of fresh water
				 * into our water vats.<BR><BR>
				 * 
				 * Farewell for now.
				 */
				return 1054091;
			}
		}
		
		#region mod by Magius(CHE)
		public override object MessageIta
		{
			get
			{
				return "<I>La Formica Madre annuisce favorevolmente quando ti avvicini a lei.</I><BR><BR>" +
						"Meraviglioso! Sono molto impressionata dall'abilità che hai nell'uccidere i nemici che ci minacciano." +
						"La mia colonia ti ringrazia.<BR><BR>" +
						"Ora devo chiederti di raccogliere molta acqua per me. Hai presente le classiche brocche d'acqua?! Me ne servirebbero 8 da svuotare all'interno delle vasche che vedi di fronte a me." +
						"<BR><BR>" +
						"Addio per ora.";
			}
		}
		#endregion
		
		public GatherWaterConversation()
		{
		}

		public override void OnRead()
		{
			System.AddObjective( new GatherWaterObjective() );
		}
	}

	public class DuringWaterGatheringConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>The Solen Matriarch looks up as you approach.</I><BR><BR>
				 * 
				 * You're back, but you have not yet gathered 8 gallons of water. Return when
				 * you have completed this task.<BR><BR>
				 * 
				 * Carry on. I'll be waiting for your return.
				 */
				return 1054094;
			}
		}
		
		#region mod by Magius(CHE)
		public override object MessageIta
		{
			get
			{
				return "<I>La Formica Madre finge di non vederti quando sei tornato.</I><BR><BR>" +
					    "Sei gia di ritorno!?! Non hai completato quello che ti ho richiesto. Prendi 8 brocche piene d'acqua e svuotale all'interno delle vasche. Ritorna quando avrai completato questa missione.<BR><BR>" +
						"Muoviti! Noi tutti aspettiamo il tuo ritorno.";
			}
		}
		#endregion

		public override bool Logged{ get{ return false; } }

		public DuringWaterGatheringConversation()
		{
		}
	}

	public class ProcessFungiConversation : QuestConversation
	{
		private bool m_Friend;

		public override object Message
		{
			get
			{
				if ( m_Friend )
				{
					/* <I>The Solen Matriarch listens as you report the completion of your
					 * tasks to her.</I><BR><BR>
					 * 
					 * I give you my thanks for your help, and I will gladly process some zoogi
					 * fungus into powder of translocation for you. Two of the zoogi fungi are
					 * required for each measure of the powder. I will process up to 200 zoogi fungi
					 * into 100 measures of powder of translocation.<BR><BR>
					 * 
					 * I will also give you some gold for assisting me and my colony, but first let's
					 * take care of your zoogi fungus.
					 */
					return 1054097;
				}
				else
				{
					/* <I>The Solen Matriarch listens as you report the completion of your
					 * tasks to her.</I><BR><BR>
					 * 
					 * I give you my thanks for your help, and I will gladly make you a friend of my
					 * solen colony. My warriors, workers, and queens will not longer look at you
					 * as an intruder and attack you when you enter our lair.<BR><BR>
					 * 
					 * I will also process some zoogi fungus into powder of translocation for you.
					 * Two of the zoogi fungi are required for each measure of the powder. I will
					 * process up to 200 zoogi fungi into 100 measures of powder of translocation.<BR><BR>
					 * 
					 * I will also give you some gold for assisting me and my colony, but first let's
					 * take care of your zoogi fungus.
					 */
					return 1054096;
				}
			}
		}
		
		#region mod by Magius(CHE)
		public override object MessageIta
		{
			get
			{
				if ( m_Friend )
				{
					return "<I>La Formica Madre ti ascolta mentre racconti i dettagli del completamento della missione.</I><BR><BR>"+
							"Devo ringraziarti per il tuo aiuto, e sarò felice di convertirti i \"zoogi fungus\" in polvere (powder of translocation). "+
							"Il procedimento richiede 200 funghi per ottenere 100 cariche in polvere.<BR><BR>"+
							"Per aver assistito la mia colonia ti darò anche dei soldi per il disturbo. "+
							"Ora va! E prendi i \"zoogi fungus\".";
				}
				else
				{
					return "<I>La Formica Madre ti ascolta mentre racconti i dettagli del completamento della missione.</I><BR><BR>"+
							"Hmm...se ti fa piacere, Io potrei farti diventare amico della mia colonia. Questo ti consentirebbe di non essere più visto come un nemico da parte dei membri della mia colonia. "+
							"Oltre ad essere amico della mia colonia...io convertirà per te i \"zoogi fungus\" in polvere (powder of traslocation). "+
							"Ogni due funghi riceverai una carica in polvere per la traslocazione (Powder of traslocation). 200 funghi 100 cariche. Vi darò anche un po' di soldi per esservi preso cura della mia colonia. "+
							"Ora va! E prendi i \"zoogi fungus\".";
				}
			}
		}
		#endregion
		
		public ProcessFungiConversation( bool friend )
		{
			m_Friend = friend;
		}

		public override void OnRead()
		{
			System.AddObjective( new ProcessFungiObjective() );
		}

		public ProcessFungiConversation()
		{
		}

		public override void ChildDeserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			m_Friend = reader.ReadBool();
		}

		public override void ChildSerialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (bool) m_Friend );
		}
	}

	public class DuringFungiProcessConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>The Solen Matriarch smiles as you greet her.</I><BR><BR>
				 * 
				 * I will gladly process some zoogi fungus into powder of translocation for you.
				 * Two of the zoogi fungi are required for each measure of the powder.
				 * I will process up to 200 zoogi fungi into 100 measures of powder of translocation.
				 */
				return 1054099;
			}
		}
		
		#region mod by Magius(CHE)
		public override object MessageIta
		{
			get
			{
				return "<I>La Formica Madre sorride compiaciuta quando stai per salutarla.</I><BR><BR>" +
					"Sarò lieta di elaborare e convertire i \"zoogi fungus\" in cariche per la traslocazione. " +
					"Due zoogi fungus sono richiesti per ogni carica in polvere. " +
					"Potrei arrivare a 200 funghi per 100 cariche.";				
			}
		}
		#endregion

		public override bool Logged{ get{ return false; } }

		public DuringFungiProcessConversation()
		{
		}
	}

	public class FullBackpackConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>The Solen Matriarch looks at you with a smile.</I><BR><BR>
				 * 
				 * While I'd like to finish conducting our business, it seems that you're a
				 * bit overloaded with equipment at the moment.<BR><BR>
				 * 
				 * Perhaps you should free some room in your backpack before we proceed.
				 */
				return 1054102;
			}
		}
		
		#region mod by Magius(CHE)
		public override object MessageIta
		{
			get
			{
				return "<I>La Formica Madre sorride compiaciuta quando stai per salutarla</I><BR><BR>"+
					"Mi piacerebbe concludere lo scambio ma ho notato che il tuo zaino è pieno. Prova a fare un po' di spazio prima di procedere.<BR><BR>"+
					"Getta a terra qualche chincaglieria!";
			}
		}
		#endregion

		private bool m_Logged;

		public override bool Logged{ get{ return m_Logged; } }

		public FullBackpackConversation( bool logged )
		{
			m_Logged = logged;
		}

		public FullBackpackConversation()
		{
			m_Logged = true;
		}

		public override void OnRead()
		{
			if ( m_Logged )
				System.AddObjective( new GetRewardObjective() );
		}
	}

	public class EndConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				/* <I>The Solen Matriarch smiles as you greet her.</I><BR><BR>
				 * 
				 * Ah good, you've returned. I will conclude our business by giving you
				 * gold I owe you for aiding me.
				 */
				return 1054101;
			}
		}
		
		#region mod by Magius(CHE)
		public override object MessageIta
		{
			get
			{
				return "<I>TLa matriarca Madre sorride compiaciuta quando stai per salutarla.</I><BR><BR>"+
					"A bene, sei tornato! A conclusione del tuo lavoro eccoti i soldi che ti avevo promesso. ";
			}
		}
		#endregion		

		public EndConversation()
		{
		}

		public override void OnRead()
		{
			System.Complete();
		}
	}
}