using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "Dawns corpse" )]
    public class Dawn : BaseCreature
    {
        [Constructable]
        public Dawn()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Body = 774;
            Name = "Dawn The Crazed";
            Kills = 200;
            SetStr( 1000, 1320 );
            SetDex( 400, 420 );
            SetInt( 100, 120 );

            SetSkill( SkillName.Wrestling, 80.0, 85.0 );
            SetSkill( SkillName.Tactics, 85.3, 97.7 );
            SetSkill( SkillName.MagicResist, 70.8, 77.8 );

            VirtualArmor = 80;

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
            /*
			PackGold( 800, 1200 );
			PackScroll( 2, 8 );
			PackWeapon( 5, 5 );
			*/
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich );
            AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
        }

        public override int Meat
        {
            get { return 1; }
        }

        public Dawn( Serial serial )
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