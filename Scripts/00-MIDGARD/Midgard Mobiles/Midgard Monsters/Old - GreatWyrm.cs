using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a great wyrm corpse" )]
    public class GreatWyrm : BaseCreature
    {
        [Constructable]
        public GreatWyrm()
            : base( AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4 )
        {
            Name = "a great wyrm";
            Body = 12;
            Hue = 1635;
            BaseSoundID = 362;

            SetStr( 1600, 1600 );
            SetDex( 201, 220 );
            SetInt( 1001, 1040 );

            SetHits( 2200 );

            SetDamage( 5, 12 );

            SetDamageType( ResistanceType.Physical, 75 );
            SetDamageType( ResistanceType.Poison, 25 );

            SetResistance( ResistanceType.Physical, 35, 40 );
            SetResistance( ResistanceType.Fire, 70, 70 );
            SetResistance( ResistanceType.Cold, 60, 60 );
            SetResistance( ResistanceType.Poison, 25, 35 );
            SetResistance( ResistanceType.Energy, 66, 70 );

            SetSkill( SkillName.EvalInt, 100.1, 110.0 );
            SetSkill( SkillName.Magery, 110.1, 120.0 );
            SetSkill( SkillName.Meditation, 100.0 );
            SetSkill( SkillName.MagicResist, 100.0 );
            SetSkill( SkillName.Tactics, 50.1, 60.0 );
            SetSkill( SkillName.Wrestling, 30.1, 100.0 );

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 60;

            PackGold( Utility.Random( 500, 1000 ) );
        }

        #region mod by Dies Irae
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if( !IsBonded )
                c.DropItem( new WyrmHeart() );
        }
        #endregion

        public override void GenerateLoot()
        {
            AddLoot( LootPack.FilthyRich, 2 );
            AddLoot( LootPack.Rich, 3 );
            AddLoot( LootPack.Gems, 18 );
        }

        public override int GetIdleSound()
        {
            return 0x2C4;
        }

        public override int GetAttackSound()
        {
            return 0x2C0;
        }

        public override int GetDeathSound()
        {
            return 0x2C1;
        }

        public override int GetAngerSound()
        {
            return 0x2C4;
        }

        public override int GetHurtSound()
        {
            return 0x2C3;
        }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return true; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Hides { get { return 20; } }
        public override int Meat { get { return 19; } }
        public override int Scales { get { return 12; } }
        public override ScaleType ScaleType { get { return ( Utility.RandomBool() ? ScaleType.Black : ScaleType.White ); } }
        public override int TreasureMapLevel { get { return 5; } }

        public GreatWyrm( Serial serial )
            : base( serial )
        {
        }

        public override void OnGotMeleeAttack( Mobile attacker )
        {
            base.OnGotMeleeAttack( attacker );

            if( attacker is BaseCreature )
            {
                BaseCreature c = (BaseCreature)attacker;

                if( c.Controlled && c.ControlMaster != null )
                {
                    c.ControlTarget = c.ControlMaster;
                    c.ControlOrder = OrderType.Attack;
                    c.Combatant = c.ControlMaster;
                }
            }
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
