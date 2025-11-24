// #define DebugMidgard2StatBall

/***************************************************************************
 *                                  Midgard2StatBall.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.1
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 			La Statball permette di aumentare le stats del pg nel backpack 
 * 			del quale è inserita.
 * 			Dopo la data di scadenza non è piu' usabile.
 * 			Se si hanno piu' statball nello zaino o se la skillball stessa 
 * 			è spostabile (Movable true) essa non permette di essere usata.
 * Changelog:
 * 			2.1
 * 			Messo decay a 1 gg.
 * 			Solo la prima volta che si apre il gump le stats tornano a 10.
 * 
 ***************************************************************************/
 
using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class Midgard2StatBall : Item
	{
		#region campi
#if DebugMidgard2StatBall
		private TimeSpan Durata = TimeSpan.FromSeconds( 1 );
#else
		private TimeSpan Durata = TimeSpan.FromDays( 1.0 );
#endif
		private DateTime m_CreationTime;
		private double m_Stats;
		private double m_StatsIniziali;
		#endregion
		
		#region proprieta
		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime CreationTime
		{
			get { return m_CreationTime; }
		}
		
		[CommandProperty( AccessLevel.Administrator )]
		public double Stats
		{ 
			get { return m_Stats; } 
			set { m_Stats = value; InvalidateProperties(); } 
		}
		
		[CommandProperty( AccessLevel.Administrator )]
		public double StatsIniziali
		{ 
			get { return m_StatsIniziali; } 
		}
		#endregion

		#region costruttori
		[Constructable]
		public Midgard2StatBall( double stats ) : base( 0x1870 )
		{
			m_Stats = stats; // Valore che si decrementa ad ogni up della stat
			m_StatsIniziali = stats; // Valore di reset della palla
			m_CreationTime = DateTime.Now;
			
			Movable = false;

			Name = "Midgard Stat Ball";
 			Hue = 2024;
 			LootType=LootType.Blessed;
			Weight = 1.0;
		}

		public Midgard2StatBall( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region metodi	
  		public override void GetProperties( ObjectPropertyList list )
		{
  			DateTime Scadenza = CreationTime + Durata;
  			TimeSpan Rimanenza = Scadenza - DateTime.Now;
  			
			base.GetProperties( list );
			
			list.Add( 1060658, "Punti stat nella gemma\t{0}" , m_Stats.ToString() );
			list.Add( 1060659, "Ore rimanenti\t{0}" , Rimanenza.TotalHours.ToString( "F0" ) );
		}
  		
  		public override void OnDoubleClick( Mobile from )
  		{		
  			PlayerMobile From = from as PlayerMobile;
  			
  			if( From == null )
  				return;
  			
  			// Controllo se la skillball è nello zaino
  			if( !IsChildOf( From.Backpack ) )
			{
				From.SendMessage( "La gemma deve essere nel tuo zaino per essere usata." );
				return;
			}
  			
  			// Controllo se la skillball non è muovibile
  			if( Movable == true )
			{
				From.SendMessage( "La gemma deve essere lockata nel tuo zaino. Contatta al piu' presto un Amministratore di Midgard." );
				return;
			}
  			
  			// Controllo se il pg è vivo
  			if( !From.CheckAlive() )
			{
				From.SendMessage( "I morti non imparano." );
				return;
			}
  			
  			// Controllo se ci sono piu' skillball nello zaino.
  			List<Item> items = From.Backpack.Items;
  			int NumGemmeInPack = 0;
  			foreach( Item i in items)
  			{
  				if( i is Midgard2SkillBall )
  				{
  					NumGemmeInPack++;
  				}
  			}	
  			if( NumGemmeInPack > 1 )
  			{
  				From.SendMessage( "Non puoi usare la gemma se ne hai un'altra nello zaino." );
				return;
  			}
  
  			// Controllo se la gemma è scaduta
  			if( DateTime.Now >= CreationTime + Durata )
  			{
   				From.SendMessage( "La gemma ha esaurito il suo potere." );
				return; 				
  			}
  				
  			// Controllo se si sta già usando la skillball
			if ( from.HasGump( typeof( Midgard2StatGump ) ) )
			{
				from.SendMessage( "La gemma è già in uso." );
			}
			else
			{
				from.SendGump( new Midgard2StatGump( From, this, true ) );
			}
  		}
		#endregion
		
		#region serialize deserialize
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			
			writer.Write( (double) m_StatsIniziali );
			writer.Write( (double) m_Stats );
			writer.Write( m_CreationTime );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			m_StatsIniziali = reader.ReadDouble();
			m_Stats = reader.ReadDouble();
			m_CreationTime = reader.ReadDateTime();
		}
		#endregion
	}
}

namespace Server.Gumps
{		
	public class Midgard2StatGump : Gump
	{
		#region Campi
		private Mobile m_From;
//		private Skill m_Stat;
		private Midgard2StatBall m_MiStBa;
   		private static double LimiteSup = 100.0;
//   		private static int m_Campi = 3;
		private bool m_First;
		#endregion

		#region Costruttori	
		public Midgard2StatGump ( Mobile from, Midgard2StatBall msb, bool first ) : base ( 20, 20 )
		{
			m_MiStBa = msb;
			m_From = from;
			m_First = first;

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			
			m_From.CloseGump( typeof( Midgard2StatGump ) );
			
			if( m_First )
			{
				// Annulla tutte le stats.
				m_From.RawDex = 10;
				m_From.RawInt = 10;
				m_From.RawStr = 10;
				
				// E riempie la statball
				m_MiStBa.Stats = m_MiStBa.StatsIniziali;				
			}
			
			AddPage( 0 );
			AddBackground( 0, 0, 300, 129, 5054 );
			
			AddImageTiled( 10, 10, 280, 21, 3004 );
			AddLabel( 13, 11, 0, "Scegli la stat da alzare:" );
				
			AddImageTiled( (290-30-50+2), (119-21), 48, 21, 0xBBC );
			AddLabel( (290-30-50+3+2), (119-21+1), 0, "Chiudi:" );
			AddButton( (290-30), (119-21), 4018, 4019, 0, GumpButtonType.Reply, 0 ); // Chiudi
			
			AddImageTiled( 10, (119-21), 150, 21, 0xBBC );
			AddLabel( 13, (119-21+1), 0, "hai ancora " + m_MiStBa.Stats.ToString() + " punti." );
			
			
			// Forza
			AddImageTiled( 10, 32 + (0 * 22), 238, 21, 0xBBC );
			AddLabelCropped( 13, 33 + (0 * 22), 150, 21, 0, "Forza" );
			
			AddImageTiled( 181, 32 + (0 * 22), 48, 21, 0xBBC );
			AddLabelCropped( 182, 33 + (0 * 22), 234, 21, 0, m_From.RawStr.ToString() );
			if( m_From.RawStr < LimiteSup )
			{
				AddButton( 231, 33 + (0 * 22) + 3, 0x15E1, 0x15E5, 1, GumpButtonType.Reply, 0 );
			}			
			
			// Dex
			AddImageTiled( 10, 32 + (1 * 22), 238, 21, 0xBBC );
			AddLabelCropped( 13, 33 + (1 * 22), 150, 21, 0, "Destrezza" );
			
			AddImageTiled( 181, 32 + (1 * 22), 48, 21, 0xBBC );
			AddLabelCropped( 182, 33 + (1 * 22), 234, 21, 0, m_From.RawDex.ToString() );
			if( m_From.RawDex < LimiteSup )
			{
				AddButton( 231, 33 + (1 * 22) + 3, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0 );
			}	
			
			// Int
			AddImageTiled( 10, 32 + (2 * 22), 238, 21, 0xBBC );
			AddLabelCropped( 13, 33 + (2 * 22), 150, 21, 0, "Intelligenza" );
			
			AddImageTiled( 181, 32 + (2 * 22), 48, 21, 0xBBC );
			AddLabelCropped( 182, 33 + (2 * 22), 234, 21, 0, m_From.RawInt.ToString() );
			if( m_From.RawInt < LimiteSup )
			{
				AddButton( 231, 33 + (2 * 22) + 3, 0x15E1, 0x15E5, 3, GumpButtonType.Reply, 0 );
			}	
		}
		#endregion
			
		#region Metodi
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile From = state.Mobile;
				
			if( (From == null) || (m_MiStBa.Deleted) )
			{
				return;
			}
			
  			if( ! m_MiStBa.IsChildOf( From.Backpack ) )
			{
				From.SendMessage( "La gemma deve essere nel tuo zaino per essere usata." );
				return;
			}
  			
			if( info.ButtonID == 0 ) 
			{
				return;
			}
  			
			if( info.ButtonID > 0 ) 
			{
				if( m_MiStBa.Stats <= 0 )
				{
	  				From.SendMessage( "La gemma è vuota." );
					return;				
				}
				
				if( From.RawStatTotal >= 225 )
				{
					From.SendMessage("Avendo raggiunto lo StatCap questa gemma ti è inutile.");
				}
				
				if( info.ButtonID == 1 )
				{
					if( From.RawStr < 125 )
					{
						From.RawStr++;
						m_MiStBa.Stats--;
						From.SendGump( new Midgard2StatGump( From, m_MiStBa, false ) );
						return;
					}
				}
				
				if( info.ButtonID == 2 )
				{
					if( From.RawDex < 125 )
					{
						From.RawDex++;
						m_MiStBa.Stats--;
						From.SendGump( new Midgard2StatGump( From, m_MiStBa, false ) );
						return;
					}
				}
				
				if( info.ButtonID == 3 )
				{
					if( From.RawInt < 125 )
					{
						From.RawInt++;
						m_MiStBa.Stats--;
						From.SendGump( new Midgard2StatGump( From, m_MiStBa, false ) );
						return;
					}
				}

				From.SendMessage("La stat scelta non puo' essere alzata ulterioremente perchè il suo valore superebbe 125.");
			}
		}
		#endregion
	}
}
