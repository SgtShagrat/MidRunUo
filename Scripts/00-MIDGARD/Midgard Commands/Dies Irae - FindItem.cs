/***************************************************************************
 *                                      FindItem.cs
 *                            		---------------------
 *  begin               	: Settembre, 2006
 * 	version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 
 * 	TODO
 * 
 ***************************************************************************/
 
using System;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Commands
{
   	public class FindItem
   	{
   		#region Registrazione
   		public static void Initialize()
      	{
         	CommandSystem.Register( "FindItem", AccessLevel.Administrator, new CommandEventHandler( FindItem_OnCommand ) );
   		}
   		#endregion
   		
		#region Callback
      	[Usage( "FindItem" )]
   		[Description( "Cerca un seriale negli items e ne dice alcune props")]
      	public static void FindItem_OnCommand( CommandEventArgs e )
      	{
			Mobile From = e.Mobile;
			if( From == null  )
				return;  
			
			if( e.Length == 0 )
			{  			
				From.SendGump( new FindItemGump( From ) );
			}
			else
			{
				From.SendMessage( "Uso del comando: [FindItem" );
			}
      	}
      	#endregion
   	}

	public class FindItemGump : Server.Gumps.Gump
	{
		#region campi
		private Mobile m_From;
		#endregion
		
		#region contruttori
		public FindItemGump( Mobile from ) : base( 50, 50 )
		{
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			
			m_From = from;
			
			this.AddPage(0);
			this.AddBackground(0, 0, 300, 224, 83);
			this.AddHtml( 15, 50, 274, 100, @"Inserisci di seguito il seriale dell'oggetto o del mobile di cui vuoi conoscere:<br>Locazione<br>Contenitore.", (bool)true, (bool)true);
			this.AddLabel(40, 20, 0, @"Midgard Find Administration Tool:");
			this.AddTextEntry(22, 160, 273, 24, 0, 1, @"");
			this.AddButton(120, 180, 247, 248, 2, GumpButtonType.Reply, 0);
			//this.AddAlphaRegion(20, 160, 275, 23);
			this.AddBackground(10, 10, 300, 224, 2900);
		}
		#endregion

		#region metodi
		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if( info.ButtonID == 0 )
				return;
			
			TextRelay relay_1 = info.TextEntries[0];
			Serial s = new Serial();
			int SerialNumber = Server.Utility.ToInt32( relay_1.Text.Trim() );
			s = (Serial)SerialNumber; 
			
			if( !s.IsValid )
			{
				m_From.SendMessage( "Il seriale non e' valido" );
				return;
			}
			
			m_From.SendMessage( "Il seriale inserito è: {0}", s.ToString() );
			m_From.SendGump( new FindItemGump( m_From ) );
			
			if( s.IsItem )
			{
				Item i = World.FindItem( s );
				if( i != null )
				{
					m_From.SendMessage( "L'oggetto e' stato trovato nella locazione: {0} della Mappa {1}.", i.Location.ToString(), i.Map.ToString());
					m_From.SendMessage( "Il worldtop e' {0}", i.GetWorldTop().ToString() );	
					m_From.SendGump( new FindItemResponseGump( m_From, i ) );
				}
				else
				{
					m_From.SendMessage( "Nessun oggetto con quel seriale trovato" );
				}
			}
			else if( s.IsMobile )
			{
				Mobile m = World.FindMobile( s );
				if( m != null )
				{
					m_From.SendMessage( "Il Mobile e' stato trovato nella locazione: {0} della Mappa {1}.", m.Location.ToString(), m.Map.ToString());
					m_From.SendGump( new FindItemResponseGump( m_From, m ) );
				}
				else
				{
					m_From.SendMessage( "Nessun oggetto con quel seriale trovato" );
				}
			}
			else
			{
				m_From.SendMessage( "Il seriale non e' valido" );
			}
		}
		#endregion
	}

	public class FindItemResponseGump : Server.Gumps.Gump
	{
		#region campi
		private Mobile m_From;
		private Item m_Item;
		private Mobile m_Mobile;
		#endregion
		
		#region contruttori
		public FindItemResponseGump( Mobile from, Item item ) : base( 150, 150 )
		{
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			
			m_From = from;
			m_Item = item;
			
			this.AddPage(0);
			this.AddBackground(0, 0, 369, 223, 83);
			
			this.AddLabel(110, 20, 0, @"Midgard FindItem Results:");
			
			this.AddLabel(20, 50, 0, @"Seriale:");
			this.AddLabel(20, 70, 0, @"Type Oggetto:");
			this.AddLabel(20, 90, 0, @"Nome:");
			this.AddLabel(20, 110, 0, @"Colore:");
			this.AddLabel(20, 130, 0, @"Location:");
			this.AddLabel(20, 150, 0, @"Vera Posizione:");
			this.AddLabel(20, 170, 0, @"Mappa:");
			
			this.AddLabel(160, 50, 0, m_Item.Serial.ToString() );
			this.AddLabel(160, 70, 0, m_Item.GetType().Name );
			this.AddLabel(160, 90, 0, m_Item.Name );
			this.AddLabel(160, 110, 0, m_Item.Hue.ToString() );
			this.AddLabel(160, 130, 0, m_Item.Location.ToString() );
			this.AddLabel(160, 150, 0, m_Item.GetWorldTop().ToString() );
			this.AddLabel(160, 170, 0, m_Item.Map.ToString() );
			
			this.AddButton(290, 130, 247, 248, 1, GumpButtonType.Reply, 0);
			this.AddButton(290, 150, 247, 248, 2, GumpButtonType.Reply, 0);
		}
		
		public FindItemResponseGump( Mobile from, Mobile mobile ) : base( 150, 150 )
		{
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			
			m_From = from;
			m_Mobile = mobile;
			
			this.AddPage(0);
			this.AddBackground(0, 0, 369, 223, 83);
			
			this.AddLabel(110, 20, 0, @"Midgard Find Results:");
			
			this.AddLabel(20, 50, 0, @"Seriale:");
			this.AddLabel(20, 70, 0, @"Type Oggetto:");
			this.AddLabel(20, 90, 0, @"Nome:");
			this.AddLabel(20, 110, 0, @"Colore:");
			this.AddLabel(20, 130, 0, @"Location:");
			this.AddLabel(20, 170, 0, @"Mappa:");
			
			this.AddLabel(160, 50, 0, m_Mobile.Serial.ToString() );
			this.AddLabel(160, 70, 0, m_Mobile.GetType().Name );
			this.AddLabel(160, 90, 0, m_Mobile.Name );
			this.AddLabel(160, 110, 0, m_Mobile.Hue.ToString() );
			this.AddLabel(160, 130, 0, m_Mobile.Location.ToString() );
			this.AddLabel(160, 170, 0, m_Mobile.Map.ToString() );
			
			this.AddButton(290, 130, 247, 248, 1, GumpButtonType.Reply, 0);
			this.AddButton(290, 150, 247, 248, 2, GumpButtonType.Reply, 0);
		}
		#endregion
		
		#region metodi
		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if( info.ButtonID == 0 )
				return;
			
			IEntity EntityFound;
			if( m_Mobile != null )
				EntityFound = (IEntity)m_Mobile;
			else if( m_Item != null )
				EntityFound = (IEntity)m_Item;
			else
				return;
			
			if( info.ButtonID == 1 )
			{
				if( EntityFound.Map != null )
				{
					if( EntityFound.Map != Map.Internal ) 
					{
						m_From.Map = EntityFound.Map;
						m_From.Location = EntityFound.Location;
						if( m_Mobile != null )
							m_From.SendGump( new FindItemResponseGump( m_From, m_Mobile ) );
						else if( m_Item != null )
							m_From.SendGump( new FindItemResponseGump( m_From, m_Item ) );
						else
							return;
					}
				}				
			}
			
			if( info.ButtonID == 2 )
			{
				if( EntityFound.Map != null )
				{
					if( EntityFound.Map != Map.Internal ) 
					{
						m_From.Map = EntityFound.Map;
						if( m_Mobile != null )
						{
							m_From.Location = EntityFound.Location;
							m_From.SendGump( new FindItemResponseGump( m_From, m_Mobile ) );
						}
						else if( m_Item != null )
						{
							m_From.Location = m_Item.GetWorldTop();
							m_From.SendGump( new FindItemResponseGump( m_From, m_Item ) );
						}
						else
							return;
					}
				}				
			}
		}
		#endregion
	}
}
