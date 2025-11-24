using Server.Items;

namespace Server.Mobiles
{
    public class DuergarRider : BaseCreature
    {
        [Constructable]
        public DuergarRider()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Duergar Rider";
            Hue = 800;

            if( this.Female = Utility.RandomBool() )
            {
                Body = 0x191;
                Name = NameList.RandomName( "female" );
                AddItem( new Skirt( Utility.RandomNeutralHue() ) );
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );
                AddItem( new Kilt( Utility.RandomNeutralHue() ) );

                Item beard = new Item( Utility.RandomList( 0x204C, 0x204B, 0x204D ) );

                beard.Hue = Utility.RandomBlueHue();
                beard.Layer = Layer.FacialHair;
                beard.Movable = false;
                AddItem( beard );

            }

            SetStr( 175, 225 );
            SetDex( 90, 135 );
            SetInt( 70, 95 );

            SetHits( 300, 350 );

            SetDamage( 10, 23 );

            SetSkill( SkillName.Macing, 95.0, 125.5 );
            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Swords, 95.0, 125.5 );
            SetSkill( SkillName.Tactics, 95.0, 110.5 );
            SetSkill( SkillName.Wrestling, 100.1, 115.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Lumberjacking, 100.0, 120.0 );

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 50;

            AddItem( new Boots( Utility.RandomRedHue() ) );
            AddItem( new ChainChest() );
            AddItem( new ChainLegs() );

            switch( Utility.Random( 7 ) )
            {
                case 0: AddItem( new Halberd() ); break;
                case 1: AddItem( new Longsword() ); break;
                case 2: AddItem( new DoubleAxe() ); break;
                case 3: AddItem( new Axe() ); break;
                case 4: AddItem( new WarHammer() ); break;
                case 5: AddItem( new WarMace() ); break;
                case 6: AddItem( new WarAxe() ); break;
            }

            Item hair = new Item( Utility.RandomList( 0x2049, 0x2048, 0x204A, 0x2044 ) );
            hair.Hue = Utility.RandomBlueHue();
            hair.Layer = Layer.Hair;
            hair.Movable = false;
            AddItem( hair );

            /*   PackGem(); 
               PackGold( 300, 375 ); 
               PackMagicItems( 3, 5 ); */
            PackSlayer();

            new Beetle().Rider = this;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
            AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }


        public override bool OnBeforeDeath()
        {
            IMount mount = Mount;

            if( mount != null )
                mount.Rider = null;

            if( mount is Mobile && Utility.Random( 20 ) != 1 ) // mod by Dies Irae
                ( (Mobile)mount ).Delete();

            return base.OnBeforeDeath();
        }

        public DuergarRider( Serial serial )
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
