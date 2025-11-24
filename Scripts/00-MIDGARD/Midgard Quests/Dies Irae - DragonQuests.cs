using System;

using Midgard.Mobiles;

using Server;
using Server.Engines.Quests;
using Server.Items;

namespace Midgard.Engines.Quests.DraconomiconRevised
{
    public class FirstChapterQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.DraconomiconRevised; } }

        public override Type NextQuest { get { return typeof( SecondChapterQuest ); } }

        public override object Title { get { return "A first look in Destard"; } }

        public override object Description
        {
            get
            {
                return "Hey, good guy, i have a mission for you.<br>" +
                "My name is Rathvold, a scientist obviously.<br>" +
                "You have to know that I'm studying draconian species.<br>" +
                "In my book, the Draconomicon revised, I would like to take all knowledge about dragons.<br>" +
                "Uhm are you bored about this? Bah.<br>" +
                "So to get the start point of my study completed you have to defeat some young dragons in Destard.<br>" +
                "One per color: green, red, blue, black and the undead one.<br>" +
                "Slay down ONLY young dragons because in my first chapter I'll take news about them.<br>" +
                "Return to me when you will have slay one per type down for me.<br>" +
                "Good luck.<br>";
            }
        }

        public override object Refuse { get { return "So don't you get worried... I'll pay another man for this job."; } }

        public override object Uncomplete { get { return "Your mission is not yet completed"; } }

        public override object Complete
        {
            get
            {
                return "Well done friend. My study has started in a right way. Thank You";
            }
        }

        public FirstChapterQuest()
        {
            AddObjective( new SlayObjective( typeof( GreenYoungDragon ), "green young dragon", 1, "Destard" ) );
            AddObjective( new SlayObjective( typeof( RedYoungDragon ), "red young dragon", 1, "Destard" ) );
            AddObjective( new SlayObjective( typeof( BlueYoungDragon ), "blue young dragon", 1, "Destard" ) );
            AddObjective( new SlayObjective( typeof( BlackYoungDragon ), "black young dragon", 1, "Destard" ) );
            AddObjective( new SlayObjective( typeof( UndeadYoungDragon ), "undead young dragon", 1, "Destard" ) );

            AddReward( new BaseReward( typeof( Gold ), 3500, "gold" ) );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class SecondChapterQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.DraconomiconRevised; } }

        public override object Title { get { return "The second chapter"; } }

        public override object Description
        {
            get
            {
                return "Oh, my friend, how are you?<br>" +
                        "I'm now interested in a really hard mission, could you be interested?<br>" +
                        "You would have to slay three red adult dragon for me.<br>" +
                        "Where you you think they could be?<br>" +
                        "In destard, exactly.<br>" +
                        "Slay them for me and i will pay you for this... uhm... dirty job.<br>" +
                        "You will have also to collect 10 red scales for me<br>." +
                        "Good luck, my friend, good luck.<br>";
            }
        }

        public override object Refuse { get { return "I know you could do better... return to me when you would try this mission."; } }

        public override object Uncomplete { get { return "Your mission is not yet completed: red dragons are still there."; } }

        public override object Complete
        {
            get
            {
                return "Perfect. Your... ehm... my knowledge in dracos is enanched. Thank You.";
            }
        }

        public SecondChapterQuest()
        {
            AddObjective( new SlayObjective( typeof( RedDragon ), "red dragon", 3, "Destard" ) );
            AddObjective( new ObtainObjective( typeof( RedScales ), "red scales", 10 ) );

            AddReward( new BaseReward( typeof( Gold ), 10000, "gold" ) );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class Rathvold : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof( FirstChapterQuest ) }; } }

        [Constructable]
        public Rathvold()
            : base( "Rathvold", "the draconians expert" )
        {
            SetSkill( SkillName.EvalInt, 65.0, 90.0 );
            SetSkill( SkillName.Inscribe, 65.0, 90.0 );
            SetSkill( SkillName.Magery, 65.0, 90.0 );
            SetSkill( SkillName.MagicResist, 65.0, 90.0 );
            SetSkill( SkillName.Wrestling, 65.0, 90.0 );
            SetSkill( SkillName.Meditation, 65.0, 90.0 );
        }

        public Rathvold( Serial serial )
            : base( serial )
        {
        }

        public override void InitBody()
        {
            InitStats( 100, 100, 25 );

            Female = true;
            Race = Race.Human;
            CantWalk = true;
        }

        public override void InitOutfit()
        {
            AddItem( new Backpack() );
            AddItem( new Robe( 0x592 ) );
            AddItem( new Sandals() );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}