namespace Server.Mobiles
{
    [CorpseName( "a plague beast lord corpse" )]
    public class PlagueBeastLord : PlagueBeast
    {
        [Constructable]
        public PlagueBeastLord()
        {
            Name = "a plague beast lord";

            SetStr( 500 );
            SetDex( 100 );
            SetInt( 30 );

            SetHits( 1800 );

            SetDamage( 20, 24 );

            SetDamageType( ResistanceType.Physical, 50 );
            SetDamageType( ResistanceType.Poison, 25 );
            SetDamageType( ResistanceType.Fire, 25 );

            SetResistance( ResistanceType.Physical, 45, 55 );
            SetResistance( ResistanceType.Fire, 40, 50 );
            SetResistance( ResistanceType.Cold, 25, 35 );
            SetResistance( ResistanceType.Poison, 75, 85 );
            SetResistance( ResistanceType.Energy, 25, 35 );

            SetSkill( SkillName.MagicResist, 0.0 );
            SetSkill( SkillName.Tactics, 100.0 );
            SetSkill( SkillName.Wrestling, 100.0 );
        }

        public PlagueBeastLord( Serial serial )
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
