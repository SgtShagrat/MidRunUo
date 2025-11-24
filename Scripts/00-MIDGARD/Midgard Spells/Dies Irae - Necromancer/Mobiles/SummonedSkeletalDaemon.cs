using Server;
using Server.Mobiles;

namespace Midgard.Engines.SpellSystem
{
    [CorpseName( "a skeletal dragon corpse" )]
    public class SummonedSkeletalDragon : BaseCreature, ISkeleton
    {
        [Constructable]
        public SummonedSkeletalDragon()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "a skeletal dragon";
            Body = 104;
            BaseSoundID = 0x488;

            SetStr( 400 );
            SetDex( 86, 105 );
            SetInt( 436, 475 );

            SetHits( 250 );

            SetDamage( 16, 22 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 55, 65 );
            SetResistance( ResistanceType.Fire, 60, 70 );
            SetResistance( ResistanceType.Cold, 30, 40 );
            SetResistance( ResistanceType.Poison, 25, 35 );
            SetResistance( ResistanceType.Energy, 35, 45 );

            SetSkill( SkillName.EvalInt, 30.1, 40.0 );
            SetSkill( SkillName.Magery, 30.1, 40.0 );
            SetSkill( SkillName.MagicResist, 99.1, 100.0 );
            SetSkill( SkillName.Tactics, 97.6, 100.0 );
            SetSkill( SkillName.Wrestling, 90.1, 92.5 );

            VirtualArmor = 60;

            Tamable = true;

            ControlSlots = 3;
            MinTameSkill = 93.9;
        }

        public override bool ReacquireOnMovement { get { return false; } }
        public override bool HasBreath { get { return true; } }
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathColdDamage { get { return 100; } }
        public override int BreathEffectHue { get { return 0x480; } }

        public override bool AutoDispel { get { return false; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public SummonedSkeletalDragon( Serial serial )
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
