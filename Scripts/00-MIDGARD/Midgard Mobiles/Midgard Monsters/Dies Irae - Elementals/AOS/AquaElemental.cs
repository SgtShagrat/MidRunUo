using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "an ore elemental corpse" )]
    public class AquaElemental : BaseCreature
    {
        public AquaElemental()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            // TODO: Gas attack
            Name = "an aqua elemental";
            Body = 108;
            BaseSoundID = 268;
            Hue = 0x98F;
            SetStr( 226, 255 );
            SetDex( 126, 145 );
            SetInt( 71, 92 );

            SetHits( 300, 400 );

            SetDamage( 20, 30 );

            SetDamageType( ResistanceType.Physical, 30 );
            SetDamageType( ResistanceType.Cold, 70 );

            SetResistance( ResistanceType.Physical, 30, 40 );
            SetResistance( ResistanceType.Fire, 30, 40 );
            SetResistance( ResistanceType.Cold, 10, 20 );
            SetResistance( ResistanceType.Poison, 70, 80 );
            SetResistance( ResistanceType.Energy, 20, 30 );

            SetSkill( SkillName.MagicResist, 50.1, 95.0 );
            SetSkill( SkillName.Tactics, 60.1, 100.0 );
            SetSkill( SkillName.Wrestling, 60.1, 100.0 );

            Fame = 5000;
            Karma = -5000;

            VirtualArmor = 29;

            PackItem( new AquaOre( 1 ) );
        }

        public AquaElemental( Serial serial )
            : base( serial )
        {
        }

        public override bool AutoDispel
        {
            get { return true; }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich );
            AddLoot( LootPack.Potions );
        }

        public override void CheckReflect( Mobile caster, ref bool reflect )
        {
            reflect = true; // Every spell is reflected back to the caster
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}