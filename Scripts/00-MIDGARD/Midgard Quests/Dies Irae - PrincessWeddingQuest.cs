/***************************************************************************
 *                                  PrincessWeddingQuest.cs
 *                            		-------------------
 *  begin                	: Ottobre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;

using Midgard.Items;

using Server.Items;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Engines.Quests
{
    public class PrincessWeddingChapterOne : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.PrincessWedding; } }

        public override Type NextQuest { get { return typeof( PrincessWeddingChapterTwo ); } }

        public override object Title { get { return "A Princess Wedding Gift (Chapter One)"; } }

        public override object Description
        {
            get
            {
                return "Hey, good guy, i have a mission for you.<br>" +
                        "My name is Ilinor, the Crown Wedding Organiser obviously.<br>" +
                        "You have to know that our Princess has promised in weeding, and my work is to collect the gift to brought to the new husband.<br>" +
                        "I receive a complete list of goods, but obviousely it is impossible for to collect all in my own.<br>" +
                        "So, are you ready to help a poor servant?<br>" +
                        "First of all, to help me, you need to find many leathers, so i can make some of the works requested to me.<br>" +
                        "50 pieces per color: normale leather, barbed leather, horned leather and the spined leather.<br>" +
                        "Go fast, you will need to walk a lot to found all that materials.<br>" +
                        "Return to me when you will have the right quantity.<br>" +
                        "Good luck.<br>";
            }
        }

        public override object Refuse { get { return "So don't you get worried... I'll pay another man for this job."; } }

        public override object Uncomplete { get { return "Your mission is not yet completed"; } }

        public override object Complete
        {
            get
            {
                return "Well done friend. We just complete the first step of the wedding gift. Thank You.";
            }
        }

        public PrincessWeddingChapterOne()
        {
            AddObjective( new ObtainObjective( typeof( Leather ), "normal leather", 50 ) );
            AddObjective( new ObtainObjective( typeof( SpinedLeather ), "spined leather", 50 ) );
            AddObjective( new ObtainObjective( typeof( HornedLeather ), "horned leather", 50 ) );
            AddObjective( new ObtainObjective( typeof( BarbedLeather ), "barbed leather", 50 ) );

            AddReward( new BaseReward( typeof( Gold ), 3500, "gold" ) );
            AddReward( new BaseReward( typeof( TreasureBag ), "treasure bag" ) );
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

    public class PrincessWeddingChapterTwo : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.PrincessWedding; } }

        public override Type NextQuest { get { return typeof( PrincessWeddingChapterThree ); } }

        public override object Title { get { return "A Princess Wedding Gift (Chapter Two)"; } }

        public override object Description
        {
            get
            {
                return "Hey, good guy, thank you for the leathers, do you want help me with the carpets and pillows?<br>" +
                        "My name is Ilinor, the Crown Wedding Organiser obviously.<br>" +
                        "As you know our Princess has promised in weeding, and my work is to collect the gift to brought to the new husband.<br>" +
                        "We already collect the leathers and now i need to found some other goods.<br>" +
                        "So, are you still ready to help a poor servant?<br>" +
                        "Well after the leathers my request now is to find carpets and pillows, to decorate the Wedding Bedroom.<br>" +
                        "8 Large Red Carpet and 30 pillows.<br>" +
                        "If you have any friend who is tailor, run to him, maybe i can help you!<br>" +
                        "Return to me when you will have the right quantity.<br>" +
                        "Good luck.<br>";
            }
        }

        public override object Refuse { get { return "So don't you get worried... I'll pay another man for this job."; } }

        public override object Uncomplete { get { return "Your mission is not yet completed"; } }

        public override object Complete
        {
            get
            {
                return "Well done friend. We complete now the second step of the wedding gift. Thank You.";
            }
        }

        public PrincessWeddingChapterTwo()
        {
            AddObjective( new ObtainObjective( typeof( LargeArabesqueCarpetEastDeed ), "Large Arabesque Carpet East", 2 ) );
            AddObjective( new ObtainObjective( typeof( RedCarpetEastDeed ), "Red Carpet East", 2 ) );
            AddObjective( new ObtainObjective( typeof( RedDecoratedCarpetSouthDeed ), "Red Decorated Carpet South", 2 ) );
            AddObjective( new ObtainObjective( typeof( RedCarpetSouthDeed ), "Red Carpet South", 2 ) );
            AddObjective( new ObtainObjective( typeof( Pillow1 ), "Pillow (Type 1)", 8 ) );
            AddObjective( new ObtainObjective( typeof( Pillow2 ), "Pillow (Type 2)", 8 ) );
            AddObjective( new ObtainObjective( typeof( Pillow3 ), "Pillow (Type 3)", 8 ) );
            AddObjective( new ObtainObjective( typeof( Pillow4 ), "Pillow (Type 4)", 8 ) );

            AddReward( new BaseReward( typeof( Gold ), 10000, "gold" ) );
            AddReward( new BaseReward( typeof( RewardBox ), 1072584 ) );
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

    public class PrincessWeddingChapterThree : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.PrincessWedding; } }

        public override object Title { get { return "A Princess Wedding Gift (Chapter Three)"; } }

        public override object Description
        {
            get
            {
                return "Hey, good guy, your help was very good until now, do you want help me with a very dangerous request?<br>" +
                        "I am still Ilinor, the Crown Wedding Organiser obviously.<br>" +
                        "As you know our Princess has promised in weeding, and my work is to collect the gift to brought to the new husband.<br>" +
                        "Till this momento We collect only goods but now we need to found something special.<br>" +
                        "So, have you enough courage to help me now?<br>" +
                        "Well after the leathers, the carpets and pillows, now I need to found a good heart to prepare the love potion.<br>" +
                        "Your heart stolen by a demon will be good.<br>" +
                        "Go to the shame dungeon, in the second level you will meet after the bridge a demon who will attack you, if you kill him, in his body you will found a copy of your rightneous heart, be courageous!<br>" +
                        "Return to me when you will have the good heart.<br>" +
                        "Good luck.<br>";
            }
        }

        public override object Refuse { get { return "So don't you get worried... I'll pay another man for this job."; } }

        public override object Uncomplete { get { return "Your mission is not yet completed"; } }

        public override object Complete
        {
            get
            {
                return "Well done friend. Now I may prepare the love potion, the wedding night will be perfect. Thank You.";
            }
        }

        public PrincessWeddingChapterThree()
        {
            AddObjective( new SlayObjective( typeof( HolyLich ), "Holy Lich", 1, "Shame" ) );
            AddObjective( new ObtainObjective( typeof( PureHeart ), "A Pure Heart", 1 ) );

            AddReward( new BaseReward( typeof( Gold ), 10000, "gold" ) );
            AddReward( new BaseReward( typeof( TailorsCraftsmanSatchel ), 1074282 ) );
            AddReward( new BaseReward( typeof( RewardBox ), 1072584 ) );
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

    public class Ilinor : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof( PrincessWeddingChapterOne ) }; } }

        [Constructable]
        public Ilinor()
            : base( "Ilinor", ", the Crown Wedding Organiser" )
        {
            SetSkill( SkillName.Meditation, 60.0, 83.0 );
            SetSkill( SkillName.Focus, 60.0, 83.0 );
        }

        public Ilinor( Serial serial )
            : base( serial )
        {
        }

        public override void InitBody()
        {
            InitStats( 100, 100, 25 );

            Female = false;
            Race = Race.Human;

            Hue = 0x8389;
            HairItemID = 0x2FCF;
            HairHue = 0x389;
        }

        public override void InitOutfit()
        {
            AddItem( new Backpack() );
            AddItem( new Robe( 0x73D ) );
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

    public class HolyLich : AncientLich
    {
        [Constructable]
        public HolyLich()
        {
            Name = NameList.RandomName( "ancient lich" );
            Title = ", the holy lich";
            Hue = 0x819;

            #region xmlattach
            // Attachment EnemyMastery
            XmlEnemyMastery WyrmMastery = new XmlEnemyMastery( "WhiteWyrm", 100, 1000 );
            WyrmMastery.Name = "WyrmMastery";
            XmlAttach.AttachTo( this, WyrmMastery );

            XmlEnemyMastery DragonMastery = new XmlEnemyMastery( "Dragon", 100, 1000 );
            DragonMastery.Name = "DragonMastery";
            XmlAttach.AttachTo( this, DragonMastery );

            XmlEnemyMastery NightmareMastery = new XmlEnemyMastery( "Nightmare", 100, 1000 );
            NightmareMastery.Name = "NightmareMastery";
            XmlAttach.AttachTo( this, NightmareMastery );

            XmlEnemyMastery KirinMastery = new XmlEnemyMastery( "Kirin", 100, 1000 );
            KirinMastery.Name = "KirinMastery";
            XmlAttach.AttachTo( this, KirinMastery );

            XmlEnemyMastery UnicornMastery = new XmlEnemyMastery( "Unicorn", 100, 1000 );
            UnicornMastery.Name = "UnicornMastery";
            XmlAttach.AttachTo( this, UnicornMastery );

            XmlEnemyMastery EnergyVortexMastery = new XmlEnemyMastery( "EnergyVortex", 100, 1000 );
            EnergyVortexMastery.Name = "EnergyVortexMastery";
            XmlAttach.AttachTo( this, EnergyVortexMastery );
            #endregion
        }

        public HolyLich( Serial serial )
            : base( serial )
        {
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            c.DropItem( new PureHeart() );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class PureHeart : Item
    {
        [Constructable]
        public PureHeart()
            : base( 0xF91 )
        {
            Name = "Pure Heart";
            Hue = 0x819;
        }

        public PureHeart( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}