using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a lizardman corpse" )]
    public class LizardmanArcher : BaseCreature
    {
        #region proprietà
        public override int Meat { get { return 1; } }
        public override int Hides { get { return 12; } }
        public override HideType HideType { get { return HideType.Spined; } }
        #endregion

        #region costruttori
        [Constructable]
        public LizardmanArcher()
            : base( AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = NameList.RandomName( "lizardman" );
            Body = Utility.RandomList( 33 );
            BaseSoundID = 417;

            SetStr( 96, 120 );
            SetDex( 86, 105 );
            SetInt( 36, 60 );

            SetHits( 158, 172 );

            SetDamage( 15, 17 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 25, 30 );
            SetResistance( ResistanceType.Fire, 5, 10 );
            SetResistance( ResistanceType.Cold, 5, 10 );
            SetResistance( ResistanceType.Poison, 10, 20 );

            SetSkill( SkillName.MagicResist, 35.1, 60.0 );
            SetSkill( SkillName.Tactics, 55.1, 80.0 );
            SetSkill( SkillName.Wrestling, 50.1, 70.0 );

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 28;

            AddItem( new Bow() );
            PackItem( new Arrow( Utility.Random( 50, 120 ) ) );
        }

        public LizardmanArcher( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void GenerateLoot()
        {
            AddLoot( LootPack.Poor );
            AddLoot( LootPack.Potions );
        }
        #endregion

        #region serial-deserial
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
