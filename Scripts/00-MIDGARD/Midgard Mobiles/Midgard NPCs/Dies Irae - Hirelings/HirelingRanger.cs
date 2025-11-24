using Server.Items;

namespace Server.Mobiles
{
    public class HirelingRanger : BaseHireling
    {
        [Constructable]
        public HirelingRanger()
            : base( AIType.AI_Archer )
        {
            GenBody( Utility.RandomBool() );

            Hue = Utility.RandomSkinHue();
            RangeFight = 5;
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Ranger";

            InitStats( 91, 76, 61 );

            SetDamage( 13, 24 );
            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.Fencing, 15.0, 37.0 );
            SetSkill( SkillName.Swords, 35.0, 57.0 );
            SetSkill( SkillName.Tactics, 65.0, 87.0 );
            SetSkill( SkillName.Wrestling, 15.0, 37.0 );

            Fame = 100;
            Karma = 125;

            VirtualArmor = 20;

            SetHair( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ), Utility.RandomNeutralHue() );

            if( Utility.RandomBool() && !Female )
            {
                SetBeard( Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D ), HairHue );
            }

            AddItem( new RangerArms() );
            AddItem( new RangerChest() );
            AddItem( new RangerGloves() );
            AddItem( new RangerGorget() );
            AddItem( new RangerLegs() );
            AddItem( new Shirt() );
            AddItem( new Shoes( Utility.RandomNeutralHue() ) );

            if( !Female )
            {
                AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
            }

            Item weapon = null;

            switch( Utility.Random( 3 ) )
            {
                case 0:
                    weapon = new Broadsword();
                    break;

                case 1:
                    weapon = new Longsword();
                    break;

                case 2:
                    weapon = new VikingSword();
                    break;
            }

            AddItem( weapon );

            PackGold( 10, 75 );
        }

        #region Characteristics

        public override bool ClickTitle
        {
            get { return false; }
        }

        #endregion

        #region Serialization

        public HirelingRanger( Serial serial )
            : base( serial )
        {
        }

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

        #endregion
    }
}