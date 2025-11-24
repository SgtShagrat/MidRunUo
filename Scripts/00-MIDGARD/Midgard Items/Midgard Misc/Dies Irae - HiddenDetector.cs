/***************************************************************************
 *                                  HiddenDetector.cs
 *                            		-------------------
 *  begin                	: February, 2006
 *  version					: 1.2
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 	HiddenDetector rivela i player hiddati in uncerto raggio ogni tot secondi.
 * 
 *  versionamento: 
 *  1.0 Fixati tutti i maggiori bachi
 *  1.1 Fixate le label
 *  1.2 Fixata la serializzazione/deserializzazione
 * 	1.3	Aggiunte regioni
 ***************************************************************************/
 
using System;

namespace Server.Items
{
	public class HiddenDetector : Item
	{			
		#region campi
		private int m_Delay;
		private bool m_Active;
		private int m_Range;
		private Timer m_Timer; 
		#endregion
		
		#region proprieta
		[CommandProperty( AccessLevel.GameMaster )]
		public int Delay
		{
			get{ return m_Delay; }
			set{ m_Delay = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get { return m_Active; }
			set 
			{ 
				m_Active = value;
				if ( m_Active )
					TimerActivate();
				InvalidateProperties(); 
			}
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Range
		{
			get { return m_Range; }
			set { m_Range = value; InvalidateProperties(); }
		}
		#endregion

		#region costruttori
		public HiddenDetector( Serial serial ) : base( serial )
		{
		}
		
		[Constructable]
		public HiddenDetector () : this ( 10 , 10 )
		{
		}
					
		[Constructable]
		public HiddenDetector ( int HidRange ) : this ( HidRange , 10 )
		{
		}
		                                                                                      
		[Constructable]
		public HiddenDetector ( int HidRange , int HidTimeSpan ) : base( 0x1B7B ) // ( 0x1BC3 ) // mod by Dies Irae
		{	
			Movable = false;
			Visible = false;
			
			m_Active = false; 
			m_Range = HidRange;
			m_Delay = HidTimeSpan;
			
			Hue = 1152;
			Name = "a Hidden Detector";		
		}
		#endregion
		
		#region metodi
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if ( m_Active )
				list.Add( 1060742 );
			else
				list.Add( 1060743 ); 
			list.Add( 1060658, "Range\t{0}" , m_Range );
			list.Add( 1060659, "Delay\t{0}" , m_Delay );
		}
				
		public void TimerActivate()
		{
			m_Timer = new InternalTimer( this, this.m_Delay );
			m_Timer.Start();
		}
		
		public override void OnDelete()
		{
			if ( m_Active == true )
				m_Timer.Stop();
			base.OnDelete();			
		}
		#endregion
		
		#region serialize-deserialize
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( (int)m_Delay );
			writer.Write( (bool)m_Active );
			writer.Write( (int)m_Range );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Delay = reader.ReadInt();
			m_Active = reader.ReadBool();
			m_Range = reader.ReadInt();
			if ( m_Active )
				TimerActivate();

            #region mod by Dies Irae
            if( ItemID == 0x1BC3 )
                ItemID = 0x1B7B;
            #endregion
		}
		#endregion
			
		private class InternalTimer : Timer
		{
			#region campi
			private HiddenDetector m_Hid;
			#endregion
			
			#region costruttori
			public InternalTimer( HiddenDetector hid, int delay ) : base( TimeSpan.FromSeconds( delay ),TimeSpan.FromSeconds( delay ) )
			{
				Priority = TimerPriority.OneSecond;
				m_Hid = hid;
			}
			#endregion
	
			#region metodi
	      	protected override void OnTick()
	      	{	      		
	      		if ( m_Hid != null && !m_Hid.Deleted && m_Hid.Active )
      			{
		      		// revealing action
					IPooledEnumerable inRange = m_Hid.Map.GetMobilesInRange( m_Hid.Location , m_Hid.Range );
					foreach ( Mobile trg in inRange )
					{
						if( trg.Hidden && trg.AccessLevel == AccessLevel.Player )
						{
							trg.RevealingAction();
							trg.SendLocalizedMessage( 500814 ); // You have been revealed! 
						}
					}	
	      		}
	      	}
	      	#endregion
		}
	}
}
