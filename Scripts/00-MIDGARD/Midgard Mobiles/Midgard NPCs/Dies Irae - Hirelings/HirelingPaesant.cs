using Server.Items;

namespace Server.Mobiles
{
    public class HirelingPaesant : BaseHireling
    {
        [Constructable]
        public HirelingPaesant()
        {
            GenBody( Utility.RandomBool() );

            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Peasant";

            InitStats( 26, 21, 16 );

            SetDamage( 8, 17 );
            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.Tactics, 5.0, 27.0 );
            SetSkill( SkillName.Wrestling, 5.0, 5.0 );

            Fame = 0;
            Karma = 0;

            VirtualArmor = 10;

            SetHair( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ), Utility.RandomNeutralHue() );

            if( Utility.RandomBool() && !Female )
            {
                SetBeard( Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D ), HairHue );
            }

            switch( Utility.Random( 2 ) )
            {
                case 0:
                    AddItem( new Doublet( Utility.RandomNeutralHue() ) );
                    break;

                case 1:
                    AddItem( new Shirt( Utility.RandomNeutralHue() ) );
                    break;
            }

            AddItem( new Katana() );

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

        public HirelingPaesant( Serial serial )
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