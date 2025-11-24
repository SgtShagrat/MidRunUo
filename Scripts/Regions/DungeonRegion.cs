using System;
using System.Collections.Generic;
using System.Xml;
using Midgard.Items;
using Server;
using Server.Misc;
using Server.Mobiles;
using Server.Gumps;

using Midgard.Engines.MonsterMasterySystem;

namespace Server.Regions
{
	public class DungeonRegion : BaseRegion
	{
		private Point3D m_EntranceLocation;
		private Map m_EntranceMap;
		
		public new Point3D EntranceLocation{ get{ return m_EntranceLocation; } set{ m_EntranceLocation = value; } }
		public new Map EntranceMap{ get{ return m_EntranceMap; } set{ m_EntranceMap = value; } }

		public DungeonRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
            #region mod by Dies Irae
            XmlElement enemyMastery = xml[ "enemyMasteryDefinition" ];
            if (enemyMastery != null)
            {
                EnemyMasteryDefinition definition = null;

                if (ReadEnemyMasteryDefinition(enemyMastery, ref definition))
                    m_EnMastDefinition = definition;
            }

		    #endregion

			XmlElement entrEl = xml["entrance"];

			Map entrMap = map;
			ReadMap( entrEl, "map", ref entrMap, false );

			if ( ReadPoint3D( entrEl, entrMap, ref m_EntranceLocation, false ) )
				m_EntranceMap = entrMap;
        }

        #region mod by Dies Irae
	    private EnemyMasteryDefinition m_EnMastDefinition;
	    
	    public EnemyMasteryDefinition EnMastDefinition{ get{ return m_EnMastDefinition; } set{ m_EnMastDefinition = value; } }

        public static bool ReadEnemyMasteryDefinition( XmlElement xml, ref EnemyMasteryDefinition definition )
        {
            definition = new EnemyMasteryDefinition( xml );
            return definition != null;
        }
        #endregion

        public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

		public override void OnEnter( Mobile m )
		{
            base.DisplayRegionalInfo( m ); // mod by Dies Irae

			if ( m is PlayerMobile && ((PlayerMobile)m).Young )
				m.SendGump( new YoungDungeonWarning() );

            // RegisterMobileEnter( m );
		}

        #region mod by Dies Irae
        private Dictionary<Mobile, int> m_Table;

        private void RegisterMobileEnter( Mobile mobile )
        {
            if( m_Table == null )
                m_Table = new Dictionary<Mobile, int>();

            if( !m_Table.ContainsKey( mobile ) )
                m_Table[ mobile ] = 1;
            else
                m_Table[ mobile ] = m_Table[ mobile ] + 1;
        }
        #endregion

	    public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
		{
			global = LightCycle.DungeonLevel;
		}

		public override bool CanUseStuckMenu( Mobile m )
		{
			if ( this.Map == Map.Felucca )
				return false;

			return base.CanUseStuckMenu( m );
		}
	}
}