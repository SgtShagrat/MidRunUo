using System;

namespace Server.Engines.Harvest
{
	public class HarvestBank
	{
		private int m_Current;
		private int m_Maximum;
		private DateTime m_NextRespawn;
		private HarvestVein m_Vein, m_DefaultVein;

		HarvestDefinition m_Definition;

		public HarvestDefinition Definition
		{
			get { return m_Definition; }
		}

		public int Current
		{
			get
			{
				CheckRespawn();
				return m_Current;
			}
		}

		public HarvestVein Vein
		{
			get
			{
				CheckRespawn();
				return m_Vein;
			}
			set
			{
				m_Vein = value;
			}
		}

		public HarvestVein DefaultVein
		{
			get
			{
				CheckRespawn();
				return m_DefaultVein;
			}
		}

		public void CheckRespawn()
		{
			if ( m_Current == m_Maximum || m_NextRespawn > DateTime.Now )
				return;

			m_Current = m_Maximum;
			/*
			if ( m_Definition.RandomizeVeins )
			{
				m_DefaultVein = m_Definition.GetVeinFrom( Utility.RandomDouble() );
			}

			m_Vein = m_DefaultVein;
			*/
		}

		public void Consume( int amount, Mobile from, Map map, int x, int y,int z, int tileID )
		{
			CheckRespawn();

			if ( m_Current == m_Maximum )
			{
				#region mod by Arlas: pol + regional spawn
				if ( m_NextRespawn < DateTime.Now )
				{
					if (m_Definition.RandomizeVeins)
						m_Vein = m_DefaultVein = m_Definition.GetVeinAt( map, x, y, z, tileID );
					m_Vein.FoundVein = false;
				}
				#endregion

				double min = m_Definition.MinRespawn.TotalMinutes;
				double max = m_Definition.MaxRespawn.TotalMinutes;

				#region mod by Dies Irae
				if( m_MinRespawnDelay > 0.0 || m_MaxRespawnDelay > 0.0 )
				{
					min = m_MinRespawnDelay;
					max = m_MaxRespawnDelay;
				}
				#endregion

				double rnd = Utility.RandomDouble();

				m_Current = m_Maximum - amount;

				double minutes = min + (rnd * (max - min));
				if ( m_Definition.RaceBonus && from.Race == Race.Elf )	//def.RaceBonus = Core.ML
					minutes *= .75;	//25% off the time.  

				m_NextRespawn = DateTime.Now + TimeSpan.FromMinutes( minutes );
			}
			else
			{
				m_Current -= amount;
			}

			if ( m_Current < 0 )
				m_Current = 0;
		}

		public void Consume( int amount, Mobile from )
		{
			CheckRespawn();

			if ( m_Current == m_Maximum )
			{
				#region mod by Arlas:
				//m_Vein = m_DefaultVein = def.GetVeinAt( map, x, y, z, tileID );
				m_Vein.FoundVein = false;
				#endregion

				double min = m_Definition.MinRespawn.TotalMinutes;
				double max = m_Definition.MaxRespawn.TotalMinutes;

				#region mod by Dies Irae
				if( m_MinRespawnDelay > 0.0 || m_MaxRespawnDelay > 0.0 )
				{
					min = m_MinRespawnDelay;
					max = m_MaxRespawnDelay;
				}
				#endregion

				double rnd = Utility.RandomDouble();

				m_Current = m_Maximum - amount;

				double minutes = min + (rnd * (max - min));
				if ( m_Definition.RaceBonus && from.Race == Race.Elf )	//def.RaceBonus = Core.ML
					minutes *= .75;	//25% off the time.  

				m_NextRespawn = DateTime.Now + TimeSpan.FromMinutes( minutes );
			}
			else
			{
				m_Current -= amount;
			}

			if ( m_Current < 0 )
				m_Current = 0;
		}

		#region modifica by Dies Irae
		/// <summary>
		/// Minimo delay tra l'esaurimento della banca e il suo successivo respawn
		/// </summary>
		private double m_MinRespawnDelay;

		/// <summary>
		/// Massimo delay tra l'esaurimento della banca e il suo successivo respawn
		/// </summary>
		private double m_MaxRespawnDelay;

		/// <summary>
		/// Costruttore per la HarvestBank con specificati i delay minimi e massimi di respawn
		/// </summary>
		public HarvestBank( HarvestDefinition def, HarvestVein defaultVein, double minRespawnDelay, double maxRespawnDelay )
		{
			m_Maximum = Utility.RandomMinMax( def.MinTotal, def.MaxTotal );
			m_Current = m_Maximum;
			m_DefaultVein = defaultVein;
			m_Vein = m_DefaultVein;

			m_Definition = def;

			m_MinRespawnDelay = minRespawnDelay;
			m_MaxRespawnDelay = maxRespawnDelay;
			m_NextRespawn = DateTime.Now;
		}
		#endregion

		public HarvestBank( HarvestDefinition def, HarvestVein defaultVein ) : this( def, defaultVein, 0.0, 0.0 ) // mod by Dies Irae
		{
			m_Maximum = Utility.RandomMinMax( def.MinTotal, def.MaxTotal );
			m_Current = m_Maximum;
			m_DefaultVein = defaultVein;
			m_Vein = m_DefaultVein;

			m_Definition = def;
			m_NextRespawn = DateTime.Now;
		}
	}
}