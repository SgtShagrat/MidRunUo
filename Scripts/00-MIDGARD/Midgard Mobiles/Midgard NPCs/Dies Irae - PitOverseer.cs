// Created on 8/28/2009 11:21:12 AM
// Template by -=Derrick=-
// http://www.joinuo.com

using Server;
using Server.Mobiles;
using Server.Items;

namespace Midgard.Mobiles
{
   [CorpseName("a pit overseer corpse")]
    public class PitOverseer : BaseCreature
    {
        public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.PitOverseer; } }

        [Constructable]
        public PitOverseer()
            : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.25, 1.0 )
        {
            Name = NameList.RandomName( "male" );
            Title = "the pit overseer";
            BodyValue = 400;
            Hue = Utility.RandomSkinHue();

            SetStr( "10d8+8" );
            SetDex( "10d8+8" );
            SetInt( "6d7+7" );

            SetSkillF( SkillName.Tactics, "25d10+425" );
            SetSkillF( SkillName.MagicResist, "25d10+425" );
            SetSkillF( SkillName.Parry, "25d10+425" );
            SetSkillF( SkillName.Swords, "25d10+425" );
            SetSkillF( SkillName.Macing, "25d10+425" );
            SetSkillF( SkillName.Fencing, "25d10+425" );
            SetSkillF( SkillName.Wrestling, "25d10+425" );
            SetSkillF( SkillName.ArmsLore, "25d10+400" );

            HairItemID = Hair.LongHair;
            FacialHairItemID = Beard.Vandyke;

            Fame = 500;
            Karma = 150;

            AddItem( new PlateChest() );
            AddItem( new PlateArms() );
            AddItem( new PlateLegs() );
            AddItem( new LeatherGloves() );
            AddItem( new Bardiche() );

            PackItem( new Gold( Utility.RandomMinMax( 80, 180 ) ) );
            PackItem( new Bandage( Utility.RandomMinMax( 2, 18 ) ) );
        }

        public PitOverseer( Serial serial ) : base( serial ) { }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}