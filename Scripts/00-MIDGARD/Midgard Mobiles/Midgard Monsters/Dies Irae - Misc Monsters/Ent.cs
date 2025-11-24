using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a ents corpse" )]
    public class Ent : BaseCreature
    {
        #region proprietà
        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override int TreasureMapLevel { get { return 2; } }
        #endregion

        #region costruttori
        [Constructable]
        public Ent()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "a ent";
            Body = 47;
            BaseSoundID = 442;

            SetStr( 66, 215 );
            SetDex( 66, 75 );
            SetInt( 30, 40 );

            SetHits( 200, 250 );


            SetDamage( 10, 15 );

            SetDamageType( ResistanceType.Physical, 80 );
            SetDamageType( ResistanceType.Poison, 20 );

            SetResistance( ResistanceType.Physical, 45, 55 );
            SetResistance( ResistanceType.Fire, 15, 25 );
            SetResistance( ResistanceType.Cold, 10, 20 );
            SetResistance( ResistanceType.Poison, 40, 50 );
            SetResistance( ResistanceType.Energy, 50, 60 );


            SetSkill( SkillName.MagicResist, 100.1, 125.0 );
            SetSkill( SkillName.Tactics, 100.0 );
            SetSkill( SkillName.Wrestling, 100.0 );

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 40;

            PackItem( new Log( 10 ) );
        }

        public Ent( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
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
