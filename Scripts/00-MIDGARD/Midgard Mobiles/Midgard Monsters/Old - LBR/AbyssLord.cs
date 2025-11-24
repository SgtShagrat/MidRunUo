using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a abyss lords corpse" )]
    public class AbyssLord : BaseCreature
    {
        [Constructable]
        public AbyssLord()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Body = 174;
            Name = "a abyss lord";
            BaseSoundID = 357;
            Kills = 20;

            ControlSlots = 5;

            SetStr( 478, 505 );
            SetDex( 82, 94 );
            SetInt( 305, 325 );
            SetSkill( SkillName.Wrestling, 64, 80 );
            SetSkill( SkillName.Tactics, 70, 79 );
            SetSkill( SkillName.MagicResist, 86, 95 );
            SetSkill( SkillName.Magery, 75, 82 );

            VirtualArmor = Utility.RandomMinMax( 52, 58 );
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
            PackGold( 800, 1200 );
            PackScroll( 2, 8 );
            PackArmor( 2, 5 );
            PackWeapon( 3, 5 );
            PackWeapon( 5, 5 );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
            AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
        }

        public override int TreasureMapLevel
        {
            get { return 4; }
        }

        public override int Meat
        {
            get { return 1; }
        }

        public AbyssLord( Serial serial )
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