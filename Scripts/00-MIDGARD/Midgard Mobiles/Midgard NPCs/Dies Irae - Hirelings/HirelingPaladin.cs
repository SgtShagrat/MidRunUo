using Server.Items;

namespace Server.Mobiles
{
    public class HirelingPaladin : BaseHireling
    {
        [Constructable]
        public HirelingPaladin()
        {
            GenBody( Utility.RandomBool() );

            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Paladin";

            SetStr( 86, 100 );
            SetDex( 81, 95 );
            SetInt( 61, 75 );

            SetDamage( 12, 24 );
            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.Anatomy, 65.0, 87.5 );
            SetSkill( SkillName.Chivalry, 85.0, 100.0 );
            SetSkill( SkillName.Healing, 65.0, 87.5 );
            SetSkill( SkillName.MagicResist, 25.0, 47.5 );
            SetSkill( SkillName.Parry, 45.0, 60.5 );
            SetSkill( SkillName.Swords, 66.0, 97.5 );
            SetSkill( SkillName.Tactics, 65.0, 87.5 );
            SetSkill( SkillName.Wrestling, 15.0, 37.5 );

            Fame = 100;
            Karma = 250;

            VirtualArmor = 25;

            switch( Utility.Random( 5 ) )
            {
                case 0:
                    break;

                case 1:
                    AddItem( new Bascinet() );
                    break;

                case 2:
                    AddItem( new CloseHelm() );
                    break;

                case 3:
                    AddItem( new Helmet() );
                    break;

                case 4:
                    AddItem( new NorseHelm() );
                    break;
            }

            SetHair( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ), Utility.RandomNeutralHue() );

            if( !Female )
            {
                SetBeard( Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D ), HairHue );

                AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
            }

            AddItem( new LeatherGorget() );
            AddItem( new MetalKiteShield() );
            AddItem( new PlateArms() );
            AddItem( new PlateChest() );
            AddItem( new PlateLegs() );
            AddItem( new Shirt() );
            AddItem( new Shoes( Utility.RandomNeutralHue() ) );
            AddItem( new VikingSword() );

            PackGold( 20, 100 );
        }

        #region Characteristics

        public override bool ClickTitle
        {
            get { return false; }
        }

        #endregion

        #region Serialization

        public HirelingPaladin( Serial serial )
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