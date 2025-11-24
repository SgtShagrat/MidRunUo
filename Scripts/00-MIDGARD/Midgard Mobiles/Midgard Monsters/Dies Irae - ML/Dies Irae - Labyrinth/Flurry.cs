using Server.Engines.XmlSpawner2;

namespace Server.Mobiles
{
    [CorpseName( "a flurry corpse" )]
    public class Flurry : AirElemental
    {
        [Constructable]
        public Flurry()
        {
            Name = "Flurry";
            Hue = 3;

            SetStr( 149, 195 );
            SetDex( 218, 264 );
            SetInt( 130, 199 );

            SetHits( 1762, 2502 );

            SetDamage( 18, 20 );

            SetDamageType( ResistanceType.Energy, 20 );
            SetDamageType( ResistanceType.Cold, 80 );

            SetResistance( ResistanceType.Physical, 56, 57 );
            SetResistance( ResistanceType.Fire, 38, 44 );
            SetResistance( ResistanceType.Cold, 40, 45 );
            SetResistance( ResistanceType.Poison, 31, 37 );
            SetResistance( ResistanceType.Energy, 39, 41 );

            SetSkill( SkillName.EvalInt, 99.1, 100.2 );
            SetSkill( SkillName.Magery, 105.1, 108.8 );
            SetSkill( SkillName.MagicResist, 104.0, 112.8 );
            SetSkill( SkillName.Tactics, 113.1, 119.8 );
            SetSkill( SkillName.Wrestling, 105.6, 106.4 );

            XmlGeneralEnemyMastery mastery = new XmlGeneralEnemyMastery( 500 );
            mastery.Name = "generalEnemyMastery";
            XmlAttach.AttachTo( this, mastery );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich, 5 );
            base.GenerateLoot();
        }

        public Flurry( Serial serial )
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