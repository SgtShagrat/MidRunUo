using Server.Items;

namespace Server.Mobiles
{
    public class HirelingThief : BaseHireling
    {
        [Constructable]
        public HirelingThief()
        {
            GenBody( Utility.RandomBool() );

            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Thief";

            SetStr( 81, 95 );
            SetDex( 86, 100 );
            SetInt( 61, 75 );

            SetDamage( 10, 23 );
            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.Fencing, 65.0, 87.5 );
            SetSkill( SkillName.Healing, 65.0, 87.5 );
            SetSkill( SkillName.Hiding, 65.0, 87.0 );
            SetSkill( SkillName.Lockpicking, 65.0, 87.0 );
            SetSkill( SkillName.MagicResist, 25.0, 47.5 );
            SetSkill( SkillName.Peacemaking, 65.0, 87.5 );
            SetSkill( SkillName.Snooping, 65.0, 87.0 );
            SetSkill( SkillName.Stealing, 66.0, 97.5 );
            SetSkill( SkillName.Tactics, 65.0, 87.5 );
            SetSkill( SkillName.Wrestling, 65.0, 87.5 );

            Fame = 100;
            Karma = 0;

            VirtualArmor = 20;

            SetHair( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ), Utility.RandomNeutralHue() );

            AddItem( new Dagger() );

            switch( Utility.Random( 2 ) )
            {
                case 0:
                    AddItem( new Doublet( Utility.RandomNeutralHue() ) );
                    break;

                case 1:
                    AddItem( new Shirt( Utility.RandomNeutralHue() ) );
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

        public HirelingThief( Serial serial )
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