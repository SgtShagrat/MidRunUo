using System;

using Server.Items;

namespace Server.Engines.Quests
{
    public class TestCraftQuest : BaseQuest
    {
        public TestCraftQuest()
        {
            switch( Utility.Random( 3 ) )
            {
                case 0:
                    {
                        AddObjective( new ObtainObjective( typeof( IronIngot ), "iron ingot", Utility.RandomMinMax( 500, 1000 ) ) );
                        AddObjective( new ObtainObjective( typeof( DullCopperIngot ), "dull copper ingot", Utility.RandomMinMax( 500, 1000 ) ) );
                        AddObjective( new ObtainObjective( typeof( ShadowIronIngot ), "shadow iron ingot", Utility.RandomMinMax( 500, 1000 ) ) );
                        AddObjective( new ObtainObjective( typeof( BronzeIngot ), "bronze ingot", Utility.RandomMinMax( 500, 1000 ) ) );
                        AddObjective( new ObtainObjective( typeof( Gold ), "gold coins", Utility.RandomMinMax( 5000, 10000 ) ) );

                        AddReward( new BaseReward( typeof( SmithsCraftsmanSatchel ), "smith satchel" ) );
                        break;
                    }
                case 1:
                    {
                        AddObjective( new ObtainObjective( typeof( GoldIngot ), "gold ingot", Utility.RandomMinMax( 500, 1000 ) ) );
                        AddObjective( new ObtainObjective( typeof( AgapiteIngot ), "agapite ingot", Utility.RandomMinMax( 500, 1000 ) ) );
                        AddObjective( new ObtainObjective( typeof( VeriteIngot ), "verite ingot", Utility.RandomMinMax( 500, 1000 ) ) );
                        AddObjective( new ObtainObjective( typeof( Gold ), "gold coins", Utility.RandomMinMax( 50000, 100000 ) ) );

                        AddReward( new BaseReward( typeof( SmithsCraftsmanSatchel ), "smith satchel" ) );
                        break;
                    }
                case 2:
                    {
                        AddObjective( new ObtainObjective( typeof( PlatinumIngot ), "platinum ingot", Utility.RandomMinMax( 500, 1000 ) ) );
                        AddObjective( new ObtainObjective( typeof( TitaniumIngot ), "titanium ingot", Utility.RandomMinMax( 500, 1000 ) ) );
                        AddObjective( new ObtainObjective( typeof( ShadowIronIngot ), "shadow iron ingot", Utility.RandomMinMax( 500, 1000 ) ) );
                        AddObjective( new ObtainObjective( typeof( Gold ), "gold coins", Utility.RandomMinMax( 5000, 10000 ) ) );

                        AddReward( new BaseReward( typeof( SmithsCraftsmanSatchel ), "smith satchel" ) );
                        break;
                    }
                default:
                    break;
            }
        }

        public override object Title
        {
            get { return "Craft Test"; }
        }

        public override object Description
        {
            get { return "Hey, good guy, i have a mission for you.<br>"; }
        }

        public override object Refuse
        {
            get { return "So don't you get worried... I'll pay another man for this job."; }
        }

        public override object Uncomplete
        {
            get { return "Your mission is not yet completed"; }
        }

        public override object Complete
        {
            get { return "Well done friend. My study has started in a right way. Thank You"; }
        }

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
    }

    public class Dick : MondainQuester
    {
        [Constructable]
        public Dick()
            : base( "Dick", "the craft expert" )
        {
            SetSkill( SkillName.EvalInt, 65.0, 90.0 );
            SetSkill( SkillName.Inscribe, 65.0, 90.0 );
            SetSkill( SkillName.Magery, 65.0, 90.0 );
            SetSkill( SkillName.MagicResist, 65.0, 90.0 );
            SetSkill( SkillName.Wrestling, 65.0, 90.0 );
            SetSkill( SkillName.Meditation, 65.0, 90.0 );
        }

        public Dick( Serial serial )
            : base( serial )
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[]
                       {
                           typeof (TestCraftQuest)
                       };
            }
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

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}