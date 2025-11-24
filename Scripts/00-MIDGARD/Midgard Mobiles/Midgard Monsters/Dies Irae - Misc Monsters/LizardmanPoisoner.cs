namespace Server.Mobiles
{
    [CorpseName( "a lizardman corpse" )]
    public class LizardmanPoisoner : BaseCreature
    {
        #region proprietà
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return ( 0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly ); } }
        public override int Meat { get { return 1; } }
        public override int Hides { get { return 12; } }
        public override HideType HideType { get { return HideType.Spined; } }
        #endregion

        #region costruttori
        [Constructable]
        public LizardmanPoisoner()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = NameList.RandomName( "lizardman" );
            Body = Utility.RandomList( 35 );
            BaseSoundID = 417;

            SetStr( 96, 120 );
            SetDex( 86, 105 );
            SetInt( 36, 60 );

            SetHits( 300, 372 );

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
        }

        public LizardmanPoisoner( Serial serial )
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
