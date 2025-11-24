using System;
using Server.Items;

namespace Server.Mobiles
{
    public class JhelomWarrior : BaseCreature, IJhelomFolk
    {
        [Constructable]
        public JhelomWarrior()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            SpeechHue = Utility.RandomDyedHue();
            //       Title = "the Jhelom Warrior"; 
            Hue = Utility.RandomSkinHue();

            if( this.Female = Utility.RandomBool() )
            {
                Body = 0x191;
                Female = true;
                //          Name = NameList.RandomName( "female" ); 

                FemalePlateChest tunic = new FemalePlateChest();
                tunic.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                tunic.LootType = LootType.Blessed;
                AddItem( tunic );
            }
            else
            {
                Body = 0x190;
                //          Name = NameList.RandomName( "male" ); 

                PlateChest tunic = new PlateChest();
                tunic.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                tunic.LootType = LootType.Blessed;
                AddItem( tunic );
            }

            Name = "A Jhelom Warrior";

            SetStr( 90, 125 );
            SetDex( 90, 120 );
            SetInt( 90, 125 );

            SetHits( 160, 210 );

            SetDamage( 10, 22 );

            SetSkill( SkillName.Fencing, 95.0, 110.0 );
            SetSkill( SkillName.Macing, 85.0, 105.5 );
            SetSkill( SkillName.MagicResist, 155.0, 178.0 );
            SetSkill( SkillName.Swords, 95.0, 110.0 );
            SetSkill( SkillName.Tactics, 95.0, 103.0 );
            SetSkill( SkillName.Wrestling, 70.0, 90.0 );
            SetSkill( SkillName.Parry, 95.0, 115.0 );
            SetSkill( SkillName.Lumberjacking, 100.0, 120.0 );

            Fame = 0;
            Karma = -1000;

            VirtualArmor = 50;

            PlateArms arms = new PlateArms();
            arms.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            arms.LootType = LootType.Blessed;
            AddItem( arms );

            PlateGloves gloves = new PlateGloves();
            gloves.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            gloves.LootType = LootType.Blessed;
            AddItem( gloves );

            PlateLegs legs = new PlateLegs();
            legs.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            legs.LootType = LootType.Blessed;
            AddItem( legs );

            PlateGorget gorget = new PlateGorget();
            gorget.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            gorget.LootType = LootType.Blessed;
            AddItem( gorget );

            Helmet helm = new Helmet();
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

            BronzeShield shield = new BronzeShield();
            shield.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            shield.LootType = LootType.Blessed;
            AddItem( shield );

            switch( Utility.Random( 7 ) )
            {
                case 0: AddItem( new Longsword() ); break;
                case 1:
                case 2: AddItem( new Katana() ); break;
                case 3: AddItem( new VikingSword() ); break;
                case 4: AddItem( new Halberd() ); break;
                case 5: AddItem( new Bardiche() ); break;
                case 6: AddItem( new Broadsword() ); break;
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

        //      public override bool AlwaysCriminal{ get{ return true; } } 

        public override bool OnBeforeDeath()
        {
            IMount mount = this.Mount;

            if( mount != null )
                mount.Rider = null;

            if( mount is Mobile )
                ( (Mobile)mount ).Delete();

            return base.OnBeforeDeath();
        }

        public JhelomWarrior( Serial serial )
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
