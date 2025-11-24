using System.Collections;

namespace Server.Mobiles
{
    [CorpseName( "an interred grizzle corpse" )]
    public class InterredGrizzle : BaseCreature
    {
        [Constructable]
        public InterredGrizzle()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "an interred grizzle";
            Body = 0x103;
            BaseSoundID = 589;

            SetStr( 466, 487 );
            SetDex( 100, 110 );
            SetInt( 59, 65 );

            SetHits( 466, 487 );

            SetDamage( 18, 28 );

            SetDamageType( ResistanceType.Physical, 30 );
            SetDamageType( ResistanceType.Energy, 70 );

            SetResistance( ResistanceType.Physical, 50, 55 );
            SetResistance( ResistanceType.Fire, 56, 61 );
            SetResistance( ResistanceType.Cold, 56, 60 );
            SetResistance( ResistanceType.Poison, 31, 32 );
            SetResistance( ResistanceType.Energy, 64, 67 );

            SetSkill( SkillName.EvalInt, 72.9, 79.9 );
            SetSkill( SkillName.Magery, 87.5, 89.7 );
            SetSkill( SkillName.Meditation, 81.2, 82.1 );
            SetSkill( SkillName.MagicResist, 80.5, 87.6 );
            SetSkill( SkillName.Tactics, 102.4, 107.6 );
            SetSkill( SkillName.Wrestling, 103.9, 104.5 );

            Fame = 18000;
            Karma = -18000;

            VirtualArmor = 60;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.FilthyRich, 2 );
            AddLoot( LootPack.MedScrolls, 2 );
        }

        public override int Meat { get { return 1; } }
        public override int TreasureMapLevel { get { return 5; } }

        public void DrainLife()
        {
            ArrayList list = new ArrayList();

            foreach( Mobile m in GetMobilesInRange( 10 ) )
            {
                if( m == this || !CanBeHarmful( m ) )
                    continue;

                if( m is BaseCreature && ( ( (BaseCreature)m ).Controlled || ( (BaseCreature)m ).Summoned || ( (BaseCreature)m ).Team != Team ) )
                    list.Add( m );
                else if( m.Player )
                    list.Add( m );
            }

            foreach( Mobile m in list )
            {
                DoHarmful( m );

                m.FixedParticles( 0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist );
                m.PlaySound( 0x231 );

                m.SendMessage( "Your Life is Mine to feed on!" );

                int toDrain = Utility.RandomMinMax( 10, 40 );

                Hits += toDrain;
                m.Damage( toDrain, this );
            }
        }

        public override void OnGaveMeleeAttack( Mobile defender )
        {
            base.OnGaveMeleeAttack( defender );

            if( 0.1 >= Utility.RandomDouble() )
                DrainLife();
        }

        public override void OnGotMeleeAttack( Mobile attacker )
        {
            base.OnGotMeleeAttack( attacker );

            if( 0.1 >= Utility.RandomDouble() )
                DrainLife();
        }

        public InterredGrizzle( Serial serial )
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