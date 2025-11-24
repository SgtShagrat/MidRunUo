/***************************************************************************
 *                               Spirituality.cs
 *
 *   begin                : 18 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.Quests;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class SpiritualityQuestOne : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.VirtueChampionSpirituality; } }

        public override Type NextQuest { get { return typeof( SpiritualityQuestTwo ); } }

        public override TimeSpan RestartDelay { get { return TimeSpan.FromHours( 24.0 ); } }

        public override bool CanOffer()
        {
            return base.CanOffer() && ( ClassSystem.IsPaladine( Owner ) || Owner.AccessLevel > AccessLevel.Player );
        }

        public override object Title { get { return "Spirituality (the unholy gem)"; } }

        public override object Description
        {
            get
            {
                return "<em>Spirituality is nonjudgmental empathy for one's fellow creatures.</em><br>" +
                       "Give me a pure stone of Spirituality to complete the overall mission.<br>" +
                       "Find the virtue altar in the deepest dungeon of Despise.<br>" +
                       "Wisper the word of power to summon the deamon which keeps the corrupted gem.<br>" +
                       "Slay down that evil creature and take its mysterious gem with you.<br>";
            }
        }

        public override object Refuse { get { return "Fair enough, the challegne isn't for everyone. Good day then!"; } }

        public override object Uncomplete { get { return "Return once ye have purified a gem of Spirituality."; } }

        public override object Complete { get { return "Well donw. Now you are ready for the main mission I have to give you."; } }

        public SpiritualityQuestOne()
        {
            AddObjective( new InternalObjective() );
        }

        private class InternalObjective : SlayObjective
        {
            public InternalObjective()
                : base( typeof( CorruptedDaemon ), "corrupted daemon", 1 )
            {
            }

            public override bool IsObjective( Mobile mob )
            {
                return mob is CorruptedDaemon && ( (CorruptedDaemon)mob ).AntiVirtue == AntiVirtues.Despise;
            }
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class SpiritualityQuestTwo : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.VirtueChampionSpirituality; } }

        public override Type NextQuest { get { return typeof( SpiritualityQuestThree ); } }

        public override TimeSpan RestartDelay { get { return TimeSpan.FromHours( 24.0 ); } }

        public override bool CanOffer()
        {
            return base.CanOffer() && ( ClassSystem.IsPaladine( Owner ) || Owner.AccessLevel > AccessLevel.Player );
        }

        public override object Title { get { return "Spirituality (the purification)"; } }

        public override object Description
        {
            get
            {
                return "Purify your Spirituality corrupted gem at the shrine of Spirituality.<br>" +
                       "Say the mantra \"mu\" at the shrine to summon the keeper of purification.<br>" +
                       "Fight its powerful sword to purify the gem.";
            }
        }

        public override object Refuse { get { return "Fair enough, the challegne isn't for everyone. Good day then!"; } }

        public override object Uncomplete { get { return "Return once ye have purified a gem of Spirituality."; } }

        public SpiritualityQuestTwo()
        {
            AddObjective( new InternalObjective() );
        }

        private class InternalObjective : SlayObjective
        {
            public InternalObjective()
                : base( typeof( Soulkeeper ), "corrupted daemon", 1 )
            {
            }

            public override bool IsObjective( Mobile mob )
            {
                return mob is Soulkeeper && ( (Soulkeeper)mob ).Virtue == Virtues.Spirituality;
            }
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class SpiritualityQuestThree : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.VirtueChampionSpirituality; } }

        public override TimeSpan RestartDelay { get { return TimeSpan.FromHours( 24.0 ); } }

        public override bool CanOffer()
        {
            return base.CanOffer() && ( ClassSystem.IsPaladine( Owner ) || Owner.AccessLevel > AccessLevel.Player );
        }

        public override object Title { get { return "Spirituality (the holy gem)"; } }

        public override object Description
        {
            get
            {
                return "Let me to touch the gem of Spirituality purified by sins and cleaned with sacred blood.";
            }
        }

        public override object Refuse { get { return "Fair enough, the challegne isn't for everyone. Good day then!"; } }

        public override object Uncomplete { get { return "Return once ye have purified a gem of Spirituality."; } }

        public SpiritualityQuestThree()
        {
            AddObjective( new InternalObjective() );
        }

        private class InternalObjective : ObtainObjective
        {
            public InternalObjective()
                : base( typeof( VirtueChampionGem ), "corrupted gem (purified)", 1 )
            {
            }

            public override bool IsObjective( Item item )
            {
                return item is VirtueChampionGem && ( (VirtueChampionGem)item ).Virtue == Virtues.Spirituality && ( (VirtueChampionGem)item ).Purified;
            }
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}