using Server.Items;

namespace Server.Mobiles
{
    public class HirelingMage : BaseHireling
    {
        [Constructable]
        public HirelingMage()
            : base( AIType.AI_Mage )
        {
            GenBody( Utility.RandomBool() );

            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Mage";

            SetStr( 61, 75 );
            SetDex( 81, 95 );
            SetInt( 86, 100 );

            SetDamage( 8, 17 );
            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.EvalInt, 100.0, 120.0 );
            SetSkill( SkillName.Magery, 100, 120.0 );
            SetSkill( SkillName.MagicResist, 100, 120.0 );
            SetSkill( SkillName.Meditation, 100, 120.0 );
            SetSkill( SkillName.Tactics, 100, 120.0 );
            SetSkill( SkillName.Wrestling, 100, 120.0 );

            Fame = 100;
            Karma = 100;

            VirtualArmor = 20;

            SetHair( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ), Utility.RandomNeutralHue() );

            if( !Female )
            {
                if( Utility.RandomBool() )
                {
                    SetBeard( Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D ), HairHue );
                }

                AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
            }

            AddItem( new Robe( Utility.RandomNeutralHue() ) );
            AddItem( new Shirt() );
            AddItem( new Shoes( Utility.RandomNeutralHue() ) );
            AddItem( new ThighBoots() );

            PackGold( 20, 100 );
        }

        #region Characteristics

        public override bool ClickTitle
        {
            get { return false; }
        }

        #endregion

        #region Serialization

        public HirelingMage( Serial serial )
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