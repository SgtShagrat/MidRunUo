using Server.Items;

namespace Server.Mobiles
{
    public class HirelingSailor : BaseHireling
    {
        [Constructable]
        public HirelingSailor()
        {
            GenBody( Utility.RandomBool() );

            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Sailor";

            InitStats( 86, 66, 41 );

            SetDamage( 10, 23 );
            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.Fencing, 65.0, 87.5 );
            SetSkill( SkillName.Healing, 65.0, 87.5 );
            SetSkill( SkillName.MagicResist, 25.0, 47.5 );
            SetSkill( SkillName.Peacemaking, 65.0, 87.5 );
            SetSkill( SkillName.Swords, 65.0, 87.5 );
            SetSkill( SkillName.Tactics, 65.0, 87.5 );
            SetSkill( SkillName.Wrestling, 65.0, 87.5 );

            Fame = 100;
            Karma = 0;

            VirtualArmor = 20;

            SetHair( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ), Utility.RandomNeutralHue() );

            if( Utility.RandomBool() && !Female )
            {
                SetBeard( Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D ), HairHue );
            }

            AddItem( new Cutlass() );
            AddItem( new Shirt( Utility.RandomNeutralHue() ) );
            AddItem( new Shoes( Utility.RandomNeutralHue() ) );
            AddItem( new ShortPants( Utility.RandomNeutralHue() ) );

            PackGold( 0, 25 );
        }

        #region Characteristics

        public override bool ClickTitle
        {
            get { return false; }
        }

        #endregion

        #region Serialization

        public HirelingSailor( Serial serial )
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