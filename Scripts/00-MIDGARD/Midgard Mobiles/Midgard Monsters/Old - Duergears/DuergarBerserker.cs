using Server.Items;

namespace Server.Mobiles
{
    public class DuergarBerserker : BaseCreature
    {
        [Constructable]
        public DuergarBerserker()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Duergar Berserker";
            Hue = 800;

            if( this.Female = Utility.RandomBool() )
            {
                Body = 0x191;
                Name = NameList.RandomName( "female" );
                AddItem( new Skirt( Utility.RandomRedHue() ) );
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );
                AddItem( new Kilt( Utility.RandomRedHue() ) );

                Item beard = new Item( Utility.RandomList( 0x204C, 0x204B, 0x204D ) );

                beard.Hue = Utility.RandomBlueHue();
                beard.Layer = Layer.FacialHair;
                beard.Movable = false;
                AddItem( beard );

            }

            SetStr( 300, 350 );
            SetDex( 90, 135 );
            SetInt( 70, 95 );

            SetHits( 350, 375 );

            SetDamage( 15, 25 );

            SetSkill( SkillName.Macing, 105.0, 135.5 );
            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Swords, 105.0, 135.5 );
            SetSkill( SkillName.Tactics, 105.0, 120.5 );
            SetSkill( SkillName.Wrestling, 100.1, 125.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Lumberjacking, 120.0, 140.0 );

            Fame = 12000;
            Karma = -12000;

            VirtualArmor = 50;

            AddItem( new Boots( Utility.RandomNeutralHue() ) );
            AddItem( new ChainChest() );
            AddItem( new ChainLegs() );

            switch( Utility.Random( 7 ) )
            {
                case 0: AddItem( new Halberd() ); break;
                case 1: AddItem( new TwoHandedAxe() ); break;
                case 2: AddItem( new DoubleAxe() ); break;
                case 3: AddItem( new Axe() ); break;
                case 4: AddItem( new WarHammer() ); break;
                case 5: AddItem( new LargeBattleAxe() ); break;
                case 6: AddItem( new WarAxe() ); break;
            }

            Item hair = new Item( Utility.RandomList( 0x2049, 0x2048, 0x204A, 0x2044 ) );
            hair.Hue = Utility.RandomBlueHue();
            hair.Layer = Layer.Hair;
            hair.Movable = false;
            AddItem( hair );

            /*PackGem(); 
            PackGold( 350, 475 ); 
            PackMagicItems( 3, 5 ); 
            PackMagicItems( 5, 5 ); */
            PackSlayer();
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
            AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

        public DuergarBerserker( Serial serial )
            : base( serial )
        {
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
