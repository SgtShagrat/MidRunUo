using Server.Items;

namespace Server.Mobiles
{
    public class HirelingBardArcher : BaseHireling
    {
        [Constructable]
        public HirelingBardArcher()
        {
            GenBody( Utility.RandomBool() );

            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Bard";

            InitStats( 16, 26, 26 );

            SetDamage( 5, 10 );
            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.Archery, 36.0, 67.0 );
            SetSkill( SkillName.Magery, 22.0 );
            SetSkill( SkillName.Musicianship, 66.0, 97.5 );
            SetSkill( SkillName.Peacemaking, 65.0, 87.5 );
            SetSkill( SkillName.Swords, 45.0, 67.0 );
            SetSkill( SkillName.Tactics, 35.0, 57.0 );

            Fame = 100;
            Karma = 100;

            VirtualArmor = 20;

            SetHair( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ), Utility.RandomNeutralHue() );

            AddItem( new Arrow( 100 ) );
            AddItem( new Bow() );

            switch( Utility.Random( 2 ) )
            {
                case 0:
                    AddItem( new Shirt( Utility.RandomNeutralHue() ) );
                    break;

                case 1:
                    AddItem( new Doublet( Utility.RandomNeutralHue() ) );
                    break;
            }

            if( Female )
            {
                switch( Utility.Random( 2 ) )
                {
                    case 0:
                        AddItem( new Kilt( Utility.RandomNeutralHue() ) );
                        break;

                    case 1:
                        AddItem( new Skirt( Utility.RandomNeutralHue() ) );
                        break;
                }
            }

            else
            {
                if( Utility.RandomBool() )
                {
                    SetBeard( Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D ), HairHue );
                }

                AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
            }

            AddItem( new Shoes( Utility.RandomNeutralHue() ) );

            PackGold( 10, 50 );
            PackItem( new Longsword() );

            switch( Utility.Random( 4 ) )
            {
                case 0:
                    PackItem( new Drums() );
                    break;

                case 1:
                    PackItem( new Harp() );
                    break;

                case 2:
                    PackItem( new Lute() );
                    break;

                case 3:
                    PackItem( new Tambourine() );
                    break;
            }
        }

        #region Characteristics

        public override bool ClickTitle
        {
            get { return false; }
        }

        #endregion

        #region Serialization

        public HirelingBardArcher( Serial serial )
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