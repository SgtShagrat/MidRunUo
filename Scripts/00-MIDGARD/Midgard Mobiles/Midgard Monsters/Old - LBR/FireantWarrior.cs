using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "an fire ant warriors corpse" )]
    public class FireantWarrior : BaseCreature
    {
        [Constructable]
        public FireantWarrior()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Body = 782;
            Name = "a fireant warrior";

            Kills = 20;
            ControlSlots = 5;

            SetStr( 80, 150 );
            SetDex( 70, 90 );
            SetInt( 85, 90 );
            SetSkill( SkillName.Wrestling, 60, 80 );
            SetSkill( SkillName.Tactics, 50, 80 );
            SetSkill( SkillName.MagicResist, 46, 590 );

            VirtualArmor = Utility.RandomMinMax( 52, 58 );
            SetFameLevel( 2 );
            SetKarmaLevel( 2 );


            switch( Utility.Random( 8 ) )
            {
                case 0:
                    PackItem( new SpidersSilk( 3 ) );
                    break;
                case 1:
                    PackItem( new BlackPearl( 3 ) );
                    break;
                case 2:
                    PackItem( new Bloodmoss( 3 ) );
                    break;
                case 3:
                    PackItem( new Garlic( 3 ) );
                    break;
                case 4:
                    PackItem( new MandrakeRoot( 3 ) );
                    break;
                case 5:
                    PackItem( new Nightshade( 3 ) );
                    break;
                case 6:
                    PackItem( new SulfurousAsh( 3 ) );
                    break;
                case 7:
                    PackItem( new Ginseng( 3 ) );
                    break;
            }
            PackScroll( 2, 8 );
            /*PackGold( 800, 1200 );
			PackScroll( 2, 8 );
			PackArmor( 2, 5 );*/
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
        }

        public override int TreasureMapLevel
        {
            get { return 4; }
        }

        public override int Meat
        {
            get { return 1; }
        }

        public FireantWarrior( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}