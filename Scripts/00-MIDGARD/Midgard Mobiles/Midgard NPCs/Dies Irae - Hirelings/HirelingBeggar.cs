using Server.Items;

namespace Server.Mobiles
{
    public class HirelingBeggar : BaseHireling
    {
        [Constructable]
        public HirelingBeggar()
        {
            GenBody( Utility.RandomBool() );

            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Beggar";

            InitStats( 26, 21, 36 );

            SetDamage( 1 );
            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.Begging, 66.0, 97.0 );
            SetSkill( SkillName.Magery, 2.0 );
            SetSkill( SkillName.Tactics, 5.0, 27.0 );
            SetSkill( SkillName.Wrestling, 5.0, 27.0 );

            Fame = 0;
            Karma = 0;

            VirtualArmor = 5;

            SetHair( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ), Utility.RandomNeutralHue() );

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

            AddItem( new Sandals( Utility.RandomNeutralHue() ) );

            PackGold( 0, 25 );
        }

        #region Characteristics

        public override bool ClickTitle
        {
            get { return false; }
        }

        #endregion

        #region Serialization

        public HirelingBeggar( Serial serial )
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