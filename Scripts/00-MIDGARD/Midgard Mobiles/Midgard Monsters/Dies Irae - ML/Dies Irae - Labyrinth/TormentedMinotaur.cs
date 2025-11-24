namespace Server.Mobiles
{
    [CorpseName( "a tormented minotaur corpse" )]
    public class TormentedMinotaur : Minotaur
    {
        [Constructable]
        public TormentedMinotaur()
        {
            Name = "a tormented minotaur";
            Body = 262;

            SetStr( 767, 945 );
            SetDex( 166, 175 );
            SetInt( 146, 170 );

            SetHits( 676, 752 );

            SetDamage( 30, 35 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 66, 75 );
            SetResistance( ResistanceType.Fire, 35, 45 );
            SetResistance( ResistanceType.Cold, 40, 50 );
            SetResistance( ResistanceType.Poison, 41, 50 );
            SetResistance( ResistanceType.Energy, 41, 50 );

            SetSkill( SkillName.Wrestling, 90.5, 105.2 );
            SetSkill( SkillName.Tactics, 92.0, 107.1 );
            SetSkill( SkillName.MagicResist, 66.5, 74.9 );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.AosFilthyRich, 2 );
        }

        public TormentedMinotaur( Serial serial )
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
    }
}
