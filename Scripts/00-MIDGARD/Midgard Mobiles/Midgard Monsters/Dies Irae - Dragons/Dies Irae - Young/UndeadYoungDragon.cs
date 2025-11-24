/***************************************************************************
 *                                  UndeadYoungDragon.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			template per un drago non-morto giovane.
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Midgard.Mobiles
{
    [CorpseName( "the corpse of an Undead Young Dragon" )]
    public class UndeadYoungDragon : BaseYoungDragon
    {
        #region costruttori
        [Constructable]
        public UndeadYoungDragon()
            : base( AIType.AI_Mage, FightMode.Closest )
        {
            Name = "an Undead Dragon (young)";
            Hue = 20000;	// Trasparente
            Body = Utility.RandomList( 60, 61 );
            BaseSoundID = 362;

            SetStr( 401, 430 );
            SetDex( 133, 152 );
            SetInt( 101, 140 );

            SetHits( 241, 258 );

            SetDamage( 11, 17 );

            SetDamageType( ResistanceType.Physical, 0 );
            SetDamageType( ResistanceType.Fire, 0 );
            SetDamageType( ResistanceType.Cold, 0 );
            SetDamageType( ResistanceType.Poison, 0 );
            SetDamageType( ResistanceType.Energy, 100 );

            SetResistance( ResistanceType.Physical, 80, 85 );
            SetResistance( ResistanceType.Fire, 60, 65 );
            SetResistance( ResistanceType.Cold, 80, 85 );
            SetResistance( ResistanceType.Poison, 80, 85 );
            SetResistance( ResistanceType.Energy, 80, 85 );

            SetSkill( SkillName.EvalInt, 90.0, 110.0 );
            SetSkill( SkillName.Magery, 90.0, 110.0 );
            SetSkill( SkillName.Meditation, 90.0, 110.0 );
            SetSkill( SkillName.MagicResist, 90.0, 110.0 );
            SetSkill( SkillName.Tactics, 90.0, 110.0 );
            SetSkill( SkillName.Wrestling, 90.0, 110.0 );

            Fame = 2000;
            Karma = 0; // Neutrale

            VirtualArmor = 60;

            Tamable = false;

            // Mosse speciali dell' ATS
            FireBreathAttack = Utility.RandomMinMax( 120, 130 );
            PetPoisonAttack = Utility.RandomMinMax( 120, 130 );
            RoarAttack = Utility.RandomMinMax( 130, 155 );

            #region xmlattach
            // Attachment EnemyMastery
            XmlEnemyMastery WyrmMastery = new XmlEnemyMastery( "WhiteWyrm", 100, 1000 );
            WyrmMastery.Name = "WyrmMastery";
            XmlAttach.AttachTo( this, WyrmMastery );

            XmlEnemyMastery DragonMastery = new XmlEnemyMastery( "Dragon", 100, 1000 );
            DragonMastery.Name = "DragonMastery";
            XmlAttach.AttachTo( this, DragonMastery );

            XmlEnemyMastery NightmareMastery = new XmlEnemyMastery( "Nightmare", 100, 1000 );
            NightmareMastery.Name = "NightmareMastery";
            XmlAttach.AttachTo( this, NightmareMastery );

            XmlEnemyMastery KirinMastery = new XmlEnemyMastery( "Kirin", 100, 1000 );
            KirinMastery.Name = "KirinMastery";
            XmlAttach.AttachTo( this, KirinMastery );

            XmlEnemyMastery UnicornMastery = new XmlEnemyMastery( "Unicorn", 100, 1000 );
            UnicornMastery.Name = "UnicornMastery";
            XmlAttach.AttachTo( this, UnicornMastery );
            #endregion
        }

        public UndeadYoungDragon( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override double BreathDamageScalar { get { return 0.15; } }
        public override int BreathColdDamage { get { return 0; } }
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathEnergyDamage { get { return 100; } }
        public override int BreathPhysicalDamage { get { return 0; } }
        public override int BreathPoisonDamage { get { return 0; } }

        public override ScaleType ScaleType { get { return (ScaleType)Utility.Random( 4 ); } }
        #endregion

        #region Serialize-Deserialize
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
