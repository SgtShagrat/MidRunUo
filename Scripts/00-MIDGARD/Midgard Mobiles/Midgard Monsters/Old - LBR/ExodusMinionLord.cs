using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "Exodus Minion Lords corpse" )]
    public class ExodusMinionLord : BaseCreature
    {
        [Constructable]
        public ExodusMinionLord()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Body = 763;
            Name = "an Exodus Minion Lord";
            BaseSoundID = 357;
            Kills = 20;
            ControlSlots = 5;

            SetStr( 1500, 1600 );
            SetDex( 71, 71 );
            SetInt( 71, 73 );
            SetSkill( SkillName.Wrestling, 70, 80 );
            SetSkill( SkillName.Tactics, 70, 90 );
            SetSkill( SkillName.MagicResist, 86, 95 );
            SetSkill( SkillName.Magery, 75, 100 );

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

            //PackGold( 800, 1200 );
            PackScroll( 2, 8 );
            //PackWeapon( 3, 5 );
            //PackWeapon( 5, 5 );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich );
        }

        public override int TreasureMapLevel
        {
            get { return 4; }
        }

        public override int Meat
        {
            get { return 1; }
        }

        public ExodusMinionLord( Serial serial )
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