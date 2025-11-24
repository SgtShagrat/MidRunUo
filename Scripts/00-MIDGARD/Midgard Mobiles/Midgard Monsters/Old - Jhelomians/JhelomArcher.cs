using System;
using Server.Items;

namespace Server.Mobiles
{
    public class JhelomArcher : BaseCreature, IJhelomFolk
    {
        [Constructable]
        public JhelomArcher()
            : base( AIType.AI_Archer, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            SpeechHue = Utility.RandomDyedHue();
            //       Title = "the Jhelom Archer"; 
            Hue = Utility.RandomSkinHue();

            if( this.Female = Utility.RandomBool() )
            {
                Body = 0x191;
                Female = true;
                //          Name = NameList.RandomName( "female" ); 
            }
            else
            {
                Body = 0x190;
                //          Name = NameList.RandomName( "male" ); 
            }

            Name = "a Jhelom Archer";

            SetStr( 125, 165 );
            SetDex( 90, 130 );
            SetInt( 90, 125 );

            SetHits( 225, 275 );

            SetDamage( 10, 22 );

            SetSkill( SkillName.Archery, 90.0, 108.0 );
            SetSkill( SkillName.MagicResist, 152.0, 178.0 );
            SetSkill( SkillName.Tactics, 87.0, 100.0 );
            SetSkill( SkillName.Wrestling, 85.0, 100.0 );
            SetSkill( SkillName.Parry, 100.0, 105.0 );
            SetSkill( SkillName.Anatomy, 100.0, 120.0 );

            Fame = 0;
            Karma = 0;

            VirtualArmor = 47;

            RingmailArms arms = new RingmailArms();
            arms.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            arms.LootType = LootType.Blessed;
            AddItem( arms );

            RingmailGloves gloves = new RingmailGloves();
            gloves.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            gloves.LootType = LootType.Blessed;
            AddItem( gloves );

            RingmailChest tunic = new RingmailChest();
            tunic.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            tunic.LootType = LootType.Blessed;
            AddItem( tunic );

            RingmailLegs legs = new RingmailLegs();
            legs.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            legs.LootType = LootType.Blessed;
            AddItem( legs );

            StuddedGorget gorget = new StuddedGorget();
            gorget.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            gorget.LootType = LootType.Blessed;
            AddItem( gorget );

            ChainCoif helm = new ChainCoif();
            helm.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            helm.LootType = LootType.Blessed;
            AddItem( helm );

            Cloak cloak = new Cloak();
            cloak.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            cloak.LootType = LootType.Blessed;
            AddItem( cloak );

            Boots boots = new Boots();
            boots.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            boots.LootType = LootType.Blessed;
            AddItem( boots );

            switch( Utility.Random( 2 ) )
            {
                case 0: AddItem( new Bow() ); break;
                case 1: AddItem( new Crossbow() ); break;
                case 2: AddItem( new HeavyCrossbow() ); break;
            }

            Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x203C ) );
            hair.Hue = Utility.RandomHairHue();
            hair.Layer = Layer.Hair;
            hair.Movable = false;
            AddItem( hair );


        }
        public override void GenerateLoot()
        {
            // luck fix by snow,gariod,ngetal
            AddLoot( LootPack.Poor );
        }

        //      public override bool AlwaysMurderer{ get{ return true; } } 

        public JhelomArcher( Serial serial )
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
