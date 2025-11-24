using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "Spider Lords corpse" )]
    public class SpiderLord : BaseCreature
    {
        [Constructable]
        public SpiderLord()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Body = 173;
            Name = "Spider Lord";
            BaseSoundID = 357;
            Kills = 20;

            SetStr( 1000, 1200 );
            SetDex( 150, 200 );
            SetInt( 151, 250 );
            SetSkill( SkillName.Wrestling, 91, 99 );
            SetSkill( SkillName.Tactics, 90, 99 );
            SetSkill( SkillName.MagicResist, 116, 122 );
            SetSkill( SkillName.Magery, 96, 99 );
            SetSkill( SkillName.Poisoning, 80, 93 );
            SetSkill( SkillName.Meditation, 27, 50 );
            SetSkill( SkillName.Anatomy, 25, 35 );

            VirtualArmor = Utility.RandomMinMax( 74, 90 );
            SetFameLevel( 5 );
            SetKarmaLevel( 5 );


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

            PackItem( new Longsword() );
            //PackGold( 800, 1200 );
            PackScroll( 2, 8 );
            //PackArmor( 2, 5 );
            //PackWeapon( 3, 5 );
            //PackWeapon( 5, 5 );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich, 2 ); // mod by Dies Irae
            AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
        }

        public override int TreasureMapLevel
        {
            get { return 5; }
        }

        public override int Meat
        {
            get { return 1; }
        }

        public SpiderLord( Serial serial )
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