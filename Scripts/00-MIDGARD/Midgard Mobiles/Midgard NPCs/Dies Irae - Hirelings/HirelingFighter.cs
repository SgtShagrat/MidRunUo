using Server.Items;

namespace Server.Mobiles
{
    public class HirelingFighter : BaseHireling
    {
        [Constructable]
        public HirelingFighter()
        {
            GenBody( Utility.RandomBool() );

            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Fighter";

            InitStats( 91, 91, 50 );

            SetDamage( 7, 14 );
            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.Fencing, 36.0, 67.0 );
            SetSkill( SkillName.Focus, 36.0, 67.0 );
            SetSkill( SkillName.Macing, 36.0, 67.0 );
            SetSkill( SkillName.Parry, 60.0, 82.0 );
            SetSkill( SkillName.Swords, 64.0, 100.0 );
            SetSkill( SkillName.Tactics, 36.0, 67.0 );
            SetSkill( SkillName.Wrestling, 36.0, 67.0 );

            Fame = 100;
            Karma = 100;

            VirtualArmor = 25;

            SetHair( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ), Utility.RandomNeutralHue() );

            if( !Female )
            {
                if( Utility.RandomBool() )
                {
                    SetBeard( Utility.RandomList( 0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D ), HairHue );
                }

                AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
            }

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

            switch( Utility.Random( 6 ) )
            {
                case 0:
                    AddItem( new BronzeShield() );
                    break;
                case 1:
                    AddItem( new HeaterShield() );
                    break;
                case 2:
                    AddItem( new MetalKiteShield() );
                    break;
                case 3:
                    AddItem( new MetalShield() );
                    break;
                case 4:
                    AddItem( new WoodenKiteShield() );
                    break;
                case 5:
                    AddItem( new WoodenShield() );
                    break;
            }

            switch( Utility.Random( 4 ) )
            {
                case 0:
                    AddItem( new ChainChest() );
                    AddItem( new ChainCoif() );
                    AddItem( new ChainLegs() );
                    break;

                case 1:
                    AddItem( new LeatherArms() );
                    AddItem( new LeatherChest() );
                    AddItem( new LeatherGloves() );
                    AddItem( new LeatherGorget() );
                    AddItem( new LeatherLegs() );
                    break;

                case 2:
                    AddItem( new LeatherArms() );
                    AddItem( new RingmailChest() );
                    AddItem( new RingmailGloves() );
                    AddItem( new RingmailLegs() );
                    break;

                case 3:
                    AddItem( new StuddedArms() );
                    AddItem( new StuddedChest() );
                    AddItem( new StuddedGloves() );
                    AddItem( new StuddedGorget() );
                    AddItem( new StuddedLegs() );
                    break;
            }

            AddItem( new Shirt() );
            AddItem( new Shoes( Utility.RandomNeutralHue() ) );

            Item weapon = null;

            switch( Utility.Random( 8 ) )
            {
                case 0:
                    weapon = new BattleAxe();
                    break;

                case 1:
                    weapon = new Broadsword();
                    break;

                case 2:
                    weapon = new Dagger();
                    break;

                case 3:
                    weapon = new Longsword();
                    break;

                case 4:
                    weapon = new TwoHandedAxe();
                    break;

                case 5:
                    weapon = new VikingSword();
                    break;

                case 6:
                    weapon = new WarHammer();
                    break;

                case 7:
                    weapon = new WarMace();
                    break;
            }

            AddItem( weapon );

            PackGold( 25, 100 );
        }

        #region Characteristics

        public override bool ClickTitle
        {
            get { return false; }
        }

        #endregion

        #region Serialization

        public HirelingFighter( Serial serial )
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