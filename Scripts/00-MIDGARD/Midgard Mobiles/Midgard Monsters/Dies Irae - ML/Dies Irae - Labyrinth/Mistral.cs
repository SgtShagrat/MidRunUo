using Server.Engines.XmlSpawner2;

namespace Server.Mobiles
{
    [CorpseName( "a Mistral corpse" )]
    public class Mistral : AirElemental
    {
        [Constructable]
        public Mistral()
        {
            Name = "Mistral";
            Body = 0xD;
            Hue = 0x39C;

            SetStr( 134, 201 );
            SetDex( 226, 238 );
            SetInt( 126, 134 );

            SetHits( 1762, 2502 );

            SetDamage( 18, 20 );

            SetDamageType( ResistanceType.Energy, 20 );
            SetDamageType( ResistanceType.Cold, 40 );
            SetDamageType( ResistanceType.Physical, 40 );

            SetResistance( ResistanceType.Physical, 55, 64 );
            SetResistance( ResistanceType.Fire, 36, 40 );
            SetResistance( ResistanceType.Cold, 33, 39 );
            SetResistance( ResistanceType.Poison, 30, 39 );
            SetResistance( ResistanceType.Energy, 49, 53 );

            SetSkill( SkillName.EvalInt, 96.2, 97.8 );
            SetSkill( SkillName.Magery, 100.8, 112.9 );
            SetSkill( SkillName.MagicResist, 106.2, 111.2 );
            SetSkill( SkillName.Tactics, 110.2, 117.1 );
            SetSkill( SkillName.Wrestling, 100.3, 104.0 );

            XmlGeneralEnemyMastery mastery = new XmlGeneralEnemyMastery( 500 );
            mastery.Name = "generalEnemyMastery";
            XmlAttach.AttachTo( this, mastery );
        }

        public override bool BardImmune { get { return true; } }

        public Mistral( Serial serial )
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
