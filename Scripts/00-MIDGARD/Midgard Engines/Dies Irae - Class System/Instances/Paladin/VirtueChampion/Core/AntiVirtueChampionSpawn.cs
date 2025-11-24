/***************************************************************************
 *                               VirtueChampionSpawn.cs
 *
 *   begin                : 18 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Engines.Quests;
using Server.Mobiles;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class AntiVirtueChampionSpawn : BaseVirtueChampionSpawn
    {
        private AntiVirtues m_AntiVirtue;
        protected AntiVirtueDefinition Definition { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public AntiVirtues AntiVirtue
        {
            get { return m_AntiVirtue; }
            set
            {
                if( value != m_AntiVirtue )
                {
                    AntiVirtues oldValue = m_AntiVirtue;
                    OnVirtueChanged( oldValue );
                    m_AntiVirtue = value;
                }
            }
        }

        private void OnVirtueChanged( AntiVirtues oldValue )
        {
            Definition = Core.FindDefinitionByAntiVirtue( AntiVirtue );
            Key = Definition.AntiMantra;
        }

        [Constructable]
        public AntiVirtueChampionSpawn()
            : this( AntiVirtues.Despise )
        {
        }

        [Constructable]
        public AntiVirtueChampionSpawn( AntiVirtues virtue )
        {
            AntiVirtue = virtue;
        }

        protected override Item GenSpawner()
        {
            return null;
        }

        public override Mobile GenChampion()
        {
            if( ChampionInfo != null && ChampionInfo is AntiVirtueChampionInfo )
                return ( (AntiVirtueChampionInfo)ChampionInfo ).GenChampion();

            return null;
        }

        public override bool HasRightQuest( Mobile to )
        {
            if( to is PlayerMobile )
                return QuestHelper.GetQuest( (PlayerMobile)to, Core.FindQuestByVirtue( AntiVirtue, 1 ) ) != null;
            else
                return false;
        }

        #region serialization
        public AntiVirtueChampionSpawn( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( (int)m_AntiVirtue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_AntiVirtue = (AntiVirtues)reader.ReadInt();
        }
        #endregion
    }
}