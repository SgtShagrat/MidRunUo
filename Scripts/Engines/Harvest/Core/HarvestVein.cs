using System;

namespace Server.Engines.Harvest
{
	public class HarvestVein
	{
		private bool m_FoundVein;//edit by Arlas
		private double m_VeinChance;
		private double m_ChanceToFallback;
		private HarvestResource m_PrimaryResource;
		private HarvestResource m_FallbackResource;

		public bool FoundVein{ get{ return m_FoundVein; } set{ m_FoundVein = value; } }
		public double VeinChance{ get{ return m_VeinChance; } set{ m_VeinChance = value; } }
		public double ChanceToFallback{ get{ return m_ChanceToFallback; } set{ m_ChanceToFallback = value; } }
		public HarvestResource PrimaryResource{ get{ return m_PrimaryResource; } set{ m_PrimaryResource = value; } }
		public HarvestResource FallbackResource{ get{ return m_FallbackResource; } set{ m_FallbackResource = value; } }

		public HarvestVein( double veinChance, double chanceToFallback, HarvestResource primaryResource, HarvestResource fallbackResource )
		{
			m_VeinChance = veinChance;
			m_ChanceToFallback = chanceToFallback;
			m_PrimaryResource = primaryResource;
			m_FallbackResource = fallbackResource;
			m_FoundVein = false;
		}

		public override string ToString()
		{
			return m_PrimaryResource != null ? m_PrimaryResource.ToString() : base.ToString();
		}
	}
}