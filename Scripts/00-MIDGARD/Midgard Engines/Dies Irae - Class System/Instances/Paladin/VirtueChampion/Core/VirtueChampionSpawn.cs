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
    public class VirtueChampionSpawn : BaseVirtueChampionSpawn
    {
        private Virtues m_Virtue;
        protected VirtueDefinition Definition { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Virtues Virtue
        {
            get { return m_Virtue; }
            set
            {
                if( value != m_Virtue )
                {
                    Virtues oldValue = m_Virtue;
                    m_Virtue = value;
                    OnVirtueChanged( oldValue );
                }
            }
        }

        private void OnVirtueChanged( Virtues oldValue )
        {
            Definition = Core.FindDefinitionByVirtue( Virtue );
            Key = Definition.Mantra;
        }

        [Constructable]
        public VirtueChampionSpawn()
            : this( Virtues.Compassion )
        {
        }

        [Constructable]
        public VirtueChampionSpawn( Virtues virtue )
        {
            Virtue = virtue;
        }

        protected override Item GenSpawner()
        {
            VirtueSymbolAddon addon = Core.FindItemByType( Definition.SymbolLocation, Map.Felucca, Definition.AddonType ) as VirtueSymbolAddon;

            if( addon == null )
            {
                addon = Construct( Definition.AddonType, Definition.East ) as VirtueSymbolAddon;
                if( addon != null )
                {
                    addon.Hue = 37; // red to test
                    addon.MoveToWorld( Definition.SymbolLocation, Map.Felucca );
                    Config.Pkg.LogInfoLine( "Gen addon for {0}: ok.", Definition.Virtue );
                }
                else
                    Config.Pkg.LogErrorLine( "Unable to create a {0} addon.", Definition.AddonType );
            }

            return addon;
        }

        public override Mobile GenChampion()
        {
            if( ChampionInfo != null && ChampionInfo is VirtueChampionInfo )
                return ( (VirtueChampionInfo)ChampionInfo ).GenChampion();

            return null;
        }

        public override bool HasRightQuest( Mobile to )
        {
            if( to is PlayerMobile )
                return QuestHelper.GetQuest( (PlayerMobile)to, Core.FindQuestByVirtue( Virtue, 2 ) ) != null;
            else
                return false;
        }

        #region serialization
        public VirtueChampionSpawn( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( (int)m_Virtue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Virtue = (Virtues)reader.ReadInt();
        }
        #endregion
    }
}