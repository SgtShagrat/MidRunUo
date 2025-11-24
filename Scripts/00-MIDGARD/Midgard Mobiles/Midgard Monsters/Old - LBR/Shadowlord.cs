using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a shadowlord corpse" )]
    public class Shadowlord : BaseCreature
    {
        [Constructable]
        public Shadowlord()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Body = 146;
            Name = "a Shadowlord";
            Kills = 20;

            SetStr( 146, 180 );
            SetDex( 103, 127 );
            SetInt( 187, 209 );
            SetSkill( SkillName.Wrestling, 55, 77 );
            SetSkill( SkillName.Tactics, 50, 75 );
            SetSkill( SkillName.MagicResist, 66, 90 );
            SetSkill( SkillName.Magery, 74, 100 );
            SetSkill( SkillName.EvalInt, 70, 80 );

            VirtualArmor = 44;
            SetFameLevel( 4 );
            SetKarmaLevel( 4 );

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
            PackItem( new Longsword() );
            /*
			PackGold( 800, 1200 );
			PackScroll( 2, 8 );
			PackArmor( 2, 5 );
			PackWeapon( 3, 5 );
			PackWeapon( 5, 5 );*/
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
            AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
        }

        public override int Hides
        {
            get { return 8; }
        }

        public override HideType HideType
        {
            get { return HideType.Spined; }
        }

        public Shadowlord( Serial serial )
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