using System;

using Server.Items;

namespace Server.Engines.Harvest
{
	public class HarvestResource
	{
		private Type[] m_Types;
		private double m_ReqSkill, m_MinSkill, m_MaxSkill;
		private object m_SuccessMessage;

		public Type[] Types{ get{ return m_Types; } set{ m_Types = value; } }
		public double ReqSkill{ get{ return m_ReqSkill; } set{ m_ReqSkill = value; } }
		public double MinSkill{ get{ return m_MinSkill; } set{ m_MinSkill = value; } }
		public double MaxSkill{ get{ return m_MaxSkill; } set{ m_MaxSkill = value; } }
		public object SuccessMessage{ get{ return m_SuccessMessage; } }

		public void SendSuccessTo( Mobile m )
		{
			if ( m_SuccessMessage is int )
				m.SendLocalizedMessage( (int)m_SuccessMessage );
			else if ( m_SuccessMessage is string )
				m.SendMessage( (string)m_SuccessMessage );
		}

		public HarvestResource( double reqSkill, double minSkill, double maxSkill, object message, params Type[] types )
		{
			m_ReqSkill = reqSkill;
			m_MinSkill = minSkill;
			m_MaxSkill = maxSkill;
			m_Types = types;
			m_SuccessMessage = message;
		}

        #region mod by Arlas rev Dies Irae
        public override string ToString()
        {
            CraftResource craftResource = CraftResources.GetFromType( m_Types[ 0 ] ); // es: typeof(IronOre)
            string name = CraftResources.GetName( craftResource );

            if( !string.IsNullOrEmpty( name ) )
                return name;
            else if( m_Types != null )
                return m_Types[ 0 ].Name;

            return base.ToString();
        }
        #endregion
	}
}