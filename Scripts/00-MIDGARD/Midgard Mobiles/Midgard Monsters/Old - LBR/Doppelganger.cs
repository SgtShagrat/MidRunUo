using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a doppelgangers corpse" )]
    public class Doppelganger : BaseCreature
    {
        [Constructable]
        public Doppelganger()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Body = 777;
            Name = "a doppelganger";
            Kills = 200;

            SetStr( 339, 382 );
            SetDex( 100, 115 );
            SetInt( 32, 52 );
            SetSkill( SkillName.Wrestling, 83, 91 );
            SetSkill( SkillName.Tactics, 81, 97 );
            SetSkill( SkillName.MagicResist, 77, 102 );

            VirtualArmor = 48;
            SetFameLevel( 3 );
            SetKarmaLevel( 3 );

            PackGem();

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

            PackGold( 800, 1200 );
            PackScroll( 2, 8 );
            /*PackArmor( 2, 5 );
			PackWeapon( 3, 5 );*/
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
            AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
        }

        public override int TreasureMapLevel
        {
            get { return 3; }
        }

        public Doppelganger( Serial serial )
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