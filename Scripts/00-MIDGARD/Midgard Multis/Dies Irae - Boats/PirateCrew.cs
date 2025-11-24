using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a Pirate's Corpse" )]
    public class PirateCrew : BaseCreature
    {
        [Constructable]
        public PirateCrew()
            : base( AIType.AI_Archer, FightMode.Closest, 15, 1, 0.2, 0.4 )
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            if( Female = Utility.RandomBool() )
            {
                Body = 0x191;
                Name = NameList.RandomName( "female" );
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );
            }

            Title = ", part of the crew";

            AddItem( new ThighBoots() );

            HairItemID = Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A );
            HairHue = Utility.RandomNondyedHue();

            if( Utility.RandomBool() && !Female )
            {
                FacialHairItemID = Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D );
                FacialHairHue = HairHue;
            }

            SetStr( 160, 205 );
            SetDex( 90, 135 );
            SetInt( 90, 125 );

            SetHits( 250, 300 );

            SetDamage( 10, 23 );

            SetSkill( SkillName.Archery, 95.0, 115.5 );
            SetSkill( SkillName.Archery, 95.0, 115.5 );
            SetSkill( SkillName.Tactics, 95.0, 100.5 );
            SetSkill( SkillName.Wrestling, 100.1, 105.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Anatomy, 100.0, 120.0 );

            Fame = 1000;
            Karma = -1000;

            VirtualArmor = 40;

            switch( Utility.Random( 1 ) )
            {
                case 0:
                    AddItem( new LongPants( Utility.RandomRedHue() ) );
                    break;
                case 1:
                    AddItem( new ShortPants( Utility.RandomRedHue() ) );
                    break;
            }

            switch( Utility.Random( 3 ) )
            {
                case 0:
                    AddItem( new FancyShirt( Utility.RandomRedHue() ) );
                    break;
                case 1:
                    AddItem( new Shirt( Utility.RandomRedHue() ) );
                    break;
                case 2:
                    AddItem( new Doublet( Utility.RandomRedHue() ) );
                    break;
            }

            switch( Utility.Random( 3 ) )
            {
                case 0:
                    AddItem( new Bandana( Utility.RandomRedHue() ) );
                    break;
                case 1:
                    AddItem( new SkullCap( Utility.RandomRedHue() ) );
                    break;
            }

            switch( Utility.Random( 2 ) )
            {
                case 0: AddItem( new Bow() ); break;
                case 1: AddItem( new Crossbow() ); break;
                case 2: AddItem( new HeavyCrossbow() ); break;
            }
        }

        public PirateCrew( Serial serial )
            : base( serial )
        {
        }

        public override bool IsScaredOfScaryThings
        {
            get { return false; }
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }

        public override bool CanRummageCorpses
        {
            get { return true; }
        }

        public override bool PlayerRangeSensitive
        {
            get { return false; }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}