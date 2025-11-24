using System;
using Server.Items;

namespace Server.Mobiles
{
    public class JhelomWarriorRider : BaseCreature, IJhelomFolk
    {
        [Constructable]
        public JhelomWarriorRider()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            SpeechHue = Utility.RandomDyedHue();
            //       Title = "the Jhelom Rider"; 
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

            Name = "a Jhelom Rider";

            SetStr( 100, 150 );
            SetDex( 95, 127 );
            SetInt( 90, 125 );

            SetHits( 225, 275 );

            SetDamage( 10, 23 );

            SetSkill( SkillName.Fencing, 90.0, 120.0 );
            SetSkill( SkillName.Macing, 85.0, 105.5 );
            SetSkill( SkillName.MagicResist, 150.0, 175.0 );
            SetSkill( SkillName.Swords, 85.0, 105.0 );
            SetSkill( SkillName.Tactics, 95.0, 100.0 );
            SetSkill( SkillName.Wrestling, 70.0, 90.0 );
            SetSkill( SkillName.Parry, 90.0, 110.0 );
            SetSkill( SkillName.Lumberjacking, 100.0, 120.0 );

            Fame = 0;
            Karma = 0;

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

            Bascinet helm = new Bascinet();
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
            /*				
                        BronzeShield shield = new BronzeShield();
                        shield.Hue = Utility.RandomList (1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899);
                        shield.LootType = LootType.Blessed;
                        AddItem( shield );
            */
            switch( Utility.Random( 7 ) )
            {
                case 0: AddItem( new Spear() ); break;
                case 1:
                case 2: AddItem( new DoubleAxe() ); break;
                case 3: AddItem( new Axe() ); break;
                case 4: AddItem( new WarHammer() ); break;
                case 5: AddItem( new DoubleBladedStaff() ); break;
                case 6: AddItem( new ShortSpear() ); break;
            }

            Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x203C ) );
            hair.Hue = Utility.RandomHairHue();
            hair.Layer = Layer.Hair;
            hair.Movable = false;
            AddItem( hair );


            new Horse().Rider = this;

        }
        public override void GenerateLoot()
        {
            // luck fix by snow,gariod,ngetal
            AddLoot( LootPack.Poor );
        }

        //      public override bool AlwaysMurderer{ get{ return true; } } 

        public override bool OnBeforeDeath()
        {
            IMount mount = this.Mount;

            if( mount != null )
                mount.Rider = null;

            if( mount is Mobile )
                ( (Mobile)mount ).Delete();

            return base.OnBeforeDeath();
        }

        public JhelomWarriorRider( Serial serial )
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
