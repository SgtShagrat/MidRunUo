using System;
using Server.Items;

namespace Server.Mobiles
{
    public class JhelomPeople : BaseCreature, IJhelomFolk
    {
        [Constructable]
        public JhelomPeople()
            : base( AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.15, 0.4 )
        {
            SpeechHue = Utility.RandomDyedHue();
            //       Title = "the Jhelom citizen"; 
            Hue = Utility.RandomSkinHue();

            if( this.Female = Utility.RandomBool() )
            {
                Body = 0x191;
                Female = true;
                //          Name = NameList.RandomName( "female" ); 

                switch( Utility.Random( 2 ) )
                {
                    case 0: Skirt legs0 = new Skirt();
                        legs0.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                        legs0.LootType = LootType.Blessed;
                        AddItem( legs0 );
                        break;
                    case 1: PlainDress legs1 = new PlainDress();
                        legs1.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                        legs1.LootType = LootType.Blessed;
                        AddItem( legs1 );
                        break;
                }
            }
            else
            {
                Body = 0x190;
                //          Name = NameList.RandomName( "male" ); 

                LongPants legs = new LongPants();
                legs.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                legs.LootType = LootType.Blessed;
                AddItem( legs );
            }

            Name = "a Jhelom citizen";

            SetStr( 80, 90 );
            SetDex( 80, 90 );
            SetInt( 80, 90 );

            SetHits( 100, 120 );

            SetDamage( 7, 18 );

            SetSkill( SkillName.Fencing, 70.0, 90.0 );
            SetSkill( SkillName.Macing, 70.0, 90.0 );
            SetSkill( SkillName.MagicResist, 90.0, 110.0 );
            SetSkill( SkillName.Swords, 70.0, 90.0 );
            SetSkill( SkillName.Tactics, 70.0, 90.0 );
            SetSkill( SkillName.Wrestling, 70.0, 90.0 );
            SetSkill( SkillName.Parry, 70.0, 90.0 );

            Fame = 0;
            Karma = 0;

            VirtualArmor = 35;

            switch( Utility.Random( 5 ) )
            {
                case 0: FancyShirt tunic0 = new FancyShirt();
                    tunic0.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                    tunic0.LootType = LootType.Blessed;
                    AddItem( tunic0 );
                    break;
                case 1: FancyShirt tunic1 = new FancyShirt();
                    tunic1.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                    tunic1.LootType = LootType.Blessed;
                    AddItem( tunic1 );
                    break;
                case 2: Shirt tunic2 = new Shirt();
                    tunic2.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                    tunic2.LootType = LootType.Blessed;
                    AddItem( tunic2 );
                    break;
                case 3: Shirt tunic3 = new Shirt();
                    tunic3.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                    tunic3.LootType = LootType.Blessed;
                    AddItem( tunic3 );
                    break;
                case 4: Robe tunic4 = new Robe();
                    tunic4.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                    tunic4.LootType = LootType.Blessed;
                    AddItem( tunic4 );
                    break;
            }

            Cloak cloak = new Cloak();
            cloak.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
            cloak.LootType = LootType.Blessed;
            AddItem( cloak );

            switch( Utility.Random( 3 ) )
            {
                case 0: Boots boots0 = new Boots();
                    boots0.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                    boots0.LootType = LootType.Blessed;
                    AddItem( boots0 );
                    break;
                case 1: Shoes boots1 = new Shoes();
                    boots1.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                    boots1.LootType = LootType.Blessed;
                    AddItem( boots1 );
                    break;
                case 2: Sandals boots2 = new Sandals();
                    boots2.Hue = Utility.RandomList( 1891, 1891, 1891, 1891, 2985, 2985, 2985, 1899 );
                    boots2.LootType = LootType.Blessed;
                    AddItem( boots2 );
                    break;
            }

            switch( Utility.Random( 7 ) )
            {
                case 0: AddItem( new Club() ); break;
                case 1: AddItem( new Pitchfork() ); break;
                case 2: AddItem( new BoneHarvester() ); break;
                case 3: AddItem( new Scythe() ); break;
                case 4: AddItem( new Club() ); break;
                case 5: AddItem( new Pitchfork() ); break;
                case 6: AddItem( new Scythe() ); break;
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

        public JhelomPeople( Serial serial )
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
