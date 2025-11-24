/*
using Server.Engines.CannedEvil;
using Server.Engines.XmlSpawner2;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "the remains of Meraktus" )]
    public class Meraktus : Minotaur
    {
        [Constructable]
        public Meraktus()
        {
            Name = "Meraktus the tormented";
            Body = 0x107;
            BaseSoundID = 0x2A8;
            Hue = 0x835;

            SetStr( 1550 );
            SetDex( 339 );
            SetInt( 127 );

            SetHits( 4122 );

            SetDamage( 20 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 73 );
            SetResistance( ResistanceType.Cold, 49 );
            SetResistance( ResistanceType.Poison, 59 );
            SetResistance( ResistanceType.Energy, 57 );
            SetResistance( ResistanceType.Fire, 60, 70 );

            SetSkill( SkillName.MagicResist, 111.5 );
            SetSkill( SkillName.Tactics, 104.9 );
            SetSkill( SkillName.Wrestling, 105.0 );

            Fame = 17000;
            Karma = -17000;

            VirtualArmor = 55;

            XmlGeneralEnemyMastery mastery = new XmlGeneralEnemyMastery( 500 );
            mastery.Name = "generalEnemyMastery";
            XmlAttach.AttachTo( this, mastery );

            XmlChampionBoss champion = new XmlChampionBoss( true, true, true, ChampionSkullType.Enlightenment, 6, 12 );
            champion.Name = "ChampionBoss";
            XmlAttach.AttachTo( this, champion );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich, 5 );
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Dismount;
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            c.DropItem( new MalletAndChisel() );

            if( Utility.RandomDouble() < 0.05 )
            {
                switch( Utility.Random( 4 ) )
                {
                    case 0: c.DropItem( new MinotaurHedge() ); break;
                    case 1: c.DropItem( new TormentedChains() ); break;
                    case 2: c.DropItem( new TormentedMinotaurStatuette() ); break;
                    case 3: c.DropItem( new LightYarn() ); break;
                    default: break;
                }
            }
        }

        public override int Meat { get { return 2; } }
        public override int Hides { get { return 10; } }
        public override HideType HideType { get { return HideType.Regular; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override int TreasureMapLevel { get { return 3; } }
        public override bool BardImmune { get { return true; } }

        public Meraktus( Serial serial )
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
*/