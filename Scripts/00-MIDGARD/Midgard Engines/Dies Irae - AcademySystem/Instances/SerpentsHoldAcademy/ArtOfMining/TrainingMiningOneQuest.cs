/***************************************************************************
 *                               TrainingMiningOneQuest.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.Quests;
using Server.Items;

namespace Midgard.Engines.Academies
{
    public class TrainingMiningOneQuest : BaseArtOfMiningQuest
    {
        public override TimeSpan RestartDelay { get { return TimeSpan.FromHours( 1.0 ); } }

        public override object Title { get { return "Mining for Iron"; } }

        public override object Description
        {
            get
            {
                return "Nice to meet you<br>" +
                       "Today I will ask you to collect some resouces to advance you knowledge about mining materials.<br>" +
                       "Good work and good luck.<br>";
            }
        }

        public override object Refuse { get { return "Come back and talk to me if you’re interested in learnin’ ‘bout minin’."; } }

        public override object Uncomplete { get { return "Where ya been off a gallivantin’ all day, pilgrim? You ain’t seen no hard work yet! Get yer arse back out there to my mine and dig up some more iron. Don’t forget to take a pickaxe or shovel, and if you’re so inclined, a packhorse too."; } }

        public override object Complete { get { return "Dang gun it! If that don't beat all! Ya went and did it, didn’t ya?"; } }

        public TrainingMiningOneQuest()
        {
            AddObjective( new ApprenticeObjective( SkillName.Mining, 50, "Serpent's hold Mine", 1077751, 1077752 ) );

            // 1077751 You can almost smell the ore in the rocks here! Your ability to improve your Mining skill is enhanced in this area.
            // 1077752 So many rocks, so little ore… Your potential to increase your Mining skill is no longer enhanced.

            AddReward( new BaseReward( typeof( Gold ), 1000, "gold" ) );
        }

        public override bool CanOffer()
        {
            return Owner.Skills.Mining.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendMessage( "You have achieved the rank of Apprentice Miner. Return to Serpent's Hold as soon as you can to claim your reward." );
            Owner.PlaySound( CompleteSound );
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
