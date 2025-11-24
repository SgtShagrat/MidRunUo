using System;
using Server;
using Server.Engines.Harvest;
using Server.Engines.Quests;
using Server.Mobiles;

namespace Midgard.Engines.Quests
{
    public abstract class HarvestObjective : BaseObjective
    {
        public Point3D Center { get; set; }

        public int Radius { get; set; }

        public double ChanceOfUpdate { get; set; }

        public abstract HarvestSystem HarvestSystem { get; }

        public HarvestDefinition Definition { get; set; }

        public Map HarvestMap { get; set; }

        public Item Tool { get; set; }

        public int TileID { get; set; }

        protected HarvestObjective( Map map, Point3D center, int radius ) :
            this( null, map, center, radius, 0.0, null, 0 )
        {
        }

        protected HarvestObjective( HarvestDefinition def, Map map, Point3D center, int radius, double chanceOfUpdate, Item tool, int tileID )
        {
            Definition = def;
            HarvestMap = map;
            Center = center;
            Radius = radius;
            ChanceOfUpdate = chanceOfUpdate;
            TileID = tileID;
            Tool = tool;
        }

        public override void OnAccept()
        {
        }

        public override void OnCompleted()
        {
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
        #endregion

        public static bool CheckHarvest( HarvestSystem system, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, int tileID )
        {
            PlayerMobile player = from as PlayerMobile;
            if( player == null )
                return false;

            for( int i = player.Quests.Count - 1; i >= 0; i-- )
            {
                BaseQuest quest = player.Quests[ i ];

                for( int j = quest.Objectives.Count - 1; j >= 0; j-- )
                {
                    BaseObjective objective = quest.Objectives[ j ];

                    if( !( objective is HarvestObjective ) )
                        continue;

                    HarvestObjective harvestObjective = (HarvestObjective)objective;
                    if( !harvestObjective.Update( system, tool, def, map, loc, tileID ) )
                        continue;

                    if( quest.Completed )
                        quest.OnCompleted();
                    else if( harvestObjective.Completed )
                        player.PlaySound( quest.UpdateSound );
                }
            }

            return false;
        }

        public bool InRange( Point3D target, Point3D center, int range )
        {
            return ( target.X >= ( center.X - range ) ) && ( target.X <= ( center.X + range ) ) && ( target.Y >= ( center.Y - range ) ) && ( target.Y <= ( center.Y + range ) );
        }

        private bool Update( HarvestSystem system, Item tool, HarvestDefinition def, Map map, Point3D loc, int tileID )
        {
            if( Completed )
                return false;
            else if( system != HarvestSystem || map == null || !InRange( loc, Center, Radius ) )
                return false;
            else if( ChanceOfUpdate > 0.0 && Utility.RandomDouble() > ChanceOfUpdate )
                return false;
            else if( Tool != null && tool.GetType() != Tool.GetType() )
                return false;
            else if( TileID > 0 && tileID != TileID )
                return false;
            else if( Definition != null && def != Definition )
                return false;
            else
            {
                Complete();
                return true;
            }
        }
    }

    public class MiningObjective : HarvestObjective
    {
        public MiningObjective( Map map, Point3D center, int radius )
            : base( map, center, radius )
        {
        }

        public MiningObjective( HarvestDefinition def, Map map, Point3D center, int radius, double chanceOfUpdate, Item tool, int tileID ) :
            base( def, map, center, radius, chanceOfUpdate, tool, tileID )
        {
        }

        public override HarvestSystem HarvestSystem
        {
            get { return Mining.System; }
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
        #endregion
    }

    public class LumberjackingObjective : HarvestObjective
    {
        public LumberjackingObjective( Map map, Point3D center, int radius )
            : base( map, center, radius )
        {
        }

        public LumberjackingObjective( HarvestDefinition def, Map map, Point3D center, int radius, double chanceOfUpdate, Item tool, int tileID ) :
            base( def, map, center, radius, chanceOfUpdate, tool, tileID )
        {
        }

        public override HarvestSystem HarvestSystem
        {
            get { return Lumberjacking.System; }
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
        #endregion
    }

    public class FishingObjective : HarvestObjective
    {
        public FishingObjective( Map map, Point3D center, int radius )
            : base( map, center, radius )
        {
        }

        public FishingObjective( HarvestDefinition def, Map map, Point3D center, int radius, double chanceOfUpdate, Item tool, int tileID ) :
            base( def, map, center, radius, chanceOfUpdate, tool, tileID )
        {
        }

        public override HarvestSystem HarvestSystem
        {
            get { return Fishing.System; }
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
        #endregion
    }

    public class FrozenQuestMiningObjective : MiningObjective
    {
        public static Point3D QuestCenter = new Point3D( 0, 0, 0 );
        public static int QuestRadius = 5;

        public FrozenQuestMiningObjective()
            : base( Map.Felucca, QuestCenter, QuestRadius )
        {
        }
    }

    public delegate void HintCallback( string message, object state );

    public abstract class BaseQuestHintHelper
    {
        private readonly string[] m_Hints;
        private readonly double m_DelayInSeconds;
        private readonly double m_Interval;
        private readonly HintCallback m_Callback;
        private readonly object m_State;
private readonly InternalTimer m_InternalTimer;
	    private readonly int m_Count;
        private int m_Index;

        protected BaseQuestHintHelper( string[] hints, double delayInSeconds, double interval, HintCallback callback, object state )
        {
            m_Hints = hints;
            m_DelayInSeconds = delayInSeconds;
            m_Interval = interval;
            m_Callback = callback;
            m_State = state;
            m_Count = m_Hints.Length;
		    m_Index = 0;

                        m_InternalTimer = new InternalTimer( this );
            m_InternalTimer.Start();
        }

        public void DoCallback()
        {
       		if ( m_Index < m_Count )
       		{
       		    m_Callback( m_Hints[ m_Index ], m_State );
       		    m_Index++;
       		}

			if ( m_Index == m_Count && m_InternalTimer != null && m_InternalTimer.Running )
				m_InternalTimer.Stop();
        }

	private class InternalTimer : Timer
	{
	    private readonly BaseQuestHintHelper m_Helper;

		public InternalTimer( BaseQuestHintHelper helper ) : base( TimeSpan.FromSeconds( helper.m_DelayInSeconds ), 
            TimeSpan.FromSeconds( helper.m_Interval ) )
		{
		    m_Helper = helper;
		}

		protected override void OnTick()
		{
		    m_Helper.DoCallback();
		}
	}
    }
}