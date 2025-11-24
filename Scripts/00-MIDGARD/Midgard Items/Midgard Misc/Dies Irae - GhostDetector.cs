/***************************************************************************
 *                                  GhostDetector.cs
 *                            		-------------------
 *  begin                	: July, 2006
 *  version					: 1.3
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 	1.3		Aggiunte regioni
 *  
 ***************************************************************************/
 
using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class GhostDetector : Item
	{		
		#region campi
		private int m_Delay;
		private bool m_Active;
		private int m_Range;
		private string m_Zone;
		private Timer m_Timer; 
		#endregion
		
		#region proprietà
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
		
		[CommandProperty( AccessLevel.GameMaster )]
		public string Zone
		{
			get{ return m_Zone; }
			set{ m_Zone = value; InvalidateProperties(); }
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
			list.Add( 1060658, "Range\t{0}" , m_Range.ToString() );
			list.Add( 1060659, "Delay\t{0}" , m_Delay.ToString() );
			list.Add( 1060660, "Zone\t{0}" , m_Zone );
			
		}
		#endregion
		
		#region costruttori
		public GhostDetector( Serial serial ) : base( serial )
		{
		}
		
		[Constructable]
		public GhostDetector () : this ( 10 , 10 )
		{
		}
					
		[Constructable]
		public GhostDetector ( int GhostRange ) : this ( GhostRange , 10 )
		{
		}
		                                                                                      
		[Constructable]
		public GhostDetector ( int GhostRange , int GhostTimeSpan ) : base( 0x1B7B ) // ( 0x1BC3 ) // mod by Dies Irae
		{	
			Movable = false;
			Visible = false;
			
			m_Active = false; 
			m_Range = GhostRange;
			m_Delay = GhostTimeSpan;
			m_Zone = this.GetWorldLocation().ToString();
			
			Hue = 1152;
			Name = "a Ghost Detector";		
		}
		#endregion
		
		#region metodi
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
		
		public static void BroadcastMessage(AccessLevel ac, int hue, string message)
        {
            foreach(NetState state in NetState.Instances)
            {
                Mobile m = state.Mobile;
                if (m != null && m.AccessLevel >= ac)
                    m.SendMessage(hue, message);
            }
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
			writer.Write( (string)m_Zone );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Delay = reader.ReadInt();
			m_Active = reader.ReadBool();
			m_Range = reader.ReadInt();
			m_Zone = reader.ReadString();
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
			private GhostDetector m_GhostD;
			#endregion
			
			#region cotruttori
			public InternalTimer( GhostDetector ghostD, int delay ) : base( TimeSpan.FromSeconds( delay ),TimeSpan.FromSeconds( delay ) )
			{
				Priority = TimerPriority.OneSecond;
				m_GhostD = ghostD;
			}
			#endregion
			
			#region metodi
	      	protected override void OnTick()
	      	{	      		
	      		if ( m_GhostD.Active )
      			{
		      		// revealing action
					IPooledEnumerable inRange = m_GhostD.Map.GetMobilesInRange( m_GhostD.Location , m_GhostD.Range );
					foreach ( Mobile trg in inRange )
					{
						if ( trg is PlayerMobile && !trg.Alive && trg.AccessLevel == AccessLevel.Player )
						{
							BroadcastMessage(AccessLevel.Counselor, 0x35, "Attenzione: Possibile fantasma lamer! " +
							                " NomePG: " + trg.Name + " Account: " + trg.Account + " Locazione: " + m_GhostD.Zone );
						}
					}	
	      		}
	      	}  
	      	#endregion
		}
	}
}
