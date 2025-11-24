using Server.Engines.XmlSpawner2;

namespace Server.Mobiles
{
    [CorpseName( "the remains of Tempest" )]
    public class Tempest : AirElemental
    {
        [Constructable]
        public Tempest()
        {
            Name = "Tempest";
            Body = 0xD;
            Hue = 0x497;
            BaseSoundID = 0x28F;

            SetStr( 116, 135 );
            SetDex( 166, 185 );
            SetInt( 101, 125 );

            SetHits( 1762, 2502 );

            SetDamage( 18, 20 );

            SetDamageType( ResistanceType.Energy, 80 );
            SetDamageType( ResistanceType.Cold, 20 );

            SetResistance( ResistanceType.Physical, 46 );
            SetResistance( ResistanceType.Fire, 39 );
            SetResistance( ResistanceType.Cold, 33 );
            SetResistance( ResistanceType.Poison, 36 );
            SetResistance( ResistanceType.Energy, 58 );

            SetSkill( SkillName.EvalInt, 99.6 );
            SetSkill( SkillName.Magery, 101.0 );
            SetSkill( SkillName.MagicResist, 104.6 );
            SetSkill( SkillName.Tactics, 111.8 );
            SetSkill( SkillName.Wrestling, 116.0 );

            XmlGeneralEnemyMastery mastery = new XmlGeneralEnemyMastery( 500 );
            mastery.Name = "generalEnemyMastery";
            XmlAttach.AttachTo( this, mastery );
        }

        public override bool BardImmune { get { return true; } }

        public Tempest( Serial serial )
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