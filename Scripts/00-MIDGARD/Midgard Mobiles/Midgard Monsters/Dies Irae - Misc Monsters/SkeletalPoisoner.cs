using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a skeletal knight corpse" )]
    public class SkeletalPoisoner : BaseCreature, ISkeleton
    {
        #region proprietà
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return ( 0.8 >= Utility.RandomDouble() ? Poison.Deadly : Poison.Lethal ); } }
        #endregion

        #region costruttori
        [Constructable]
        public SkeletalPoisoner()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "a skeletal poisoner";
            Body = 147;
            BaseSoundID = 451;

            SetStr( 196, 250 );
            SetDex( 76, 95 );
            SetInt( 36, 60 );

            SetHits( 200, 250 );

            SetDamage( 15, 30 );

            SetDamageType( ResistanceType.Physical, 20 );
            SetDamageType( ResistanceType.Cold, 80 );

            SetResistance( ResistanceType.Physical, 55, 65 );
            SetResistance( ResistanceType.Fire, 50, 70 );
            SetResistance( ResistanceType.Cold, 50, 60 );
            SetResistance( ResistanceType.Poison, 20, 30 );
            SetResistance( ResistanceType.Energy, 30, 40 );

            SetSkill( SkillName.MagicResist, 65.1, 80.0 );
            SetSkill( SkillName.Tactics, 85.1, 100.0 );
            SetSkill( SkillName.Wrestling, 85.1, 95.0 );

            Fame = 3000;
            Karma = -3000;

            VirtualArmor = 40;

            switch( Utility.Random( 6 ) )
            {
                case 0: PackItem( new PlateArms() ); break;
                case 1: PackItem( new PlateChest() ); break;
                case 2: PackItem( new PlateGloves() ); break;
                case 3: PackItem( new PlateGorget() ); break;
                case 4: PackItem( new PlateLegs() ); break;
                case 5: PackItem( new PlateHelm() ); break;
            }

            PackItem( new Scimitar() );
            PackItem( new Arrow( 10 ) );
            PackItem( new WoodenShield() );

        }

        public SkeletalPoisoner( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich );
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
