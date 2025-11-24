/***************************************************************************
 *                                  Fangor.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Fangor e' il signore dei draghi verdi.
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Midgard.Mobiles
{
    [CorpseName( "the corpse of a Green Dragonlord" )]
    public class Fangor : BaseLordOfDragons
    {
        #region costruttori
        [Constructable]
        public Fangor()
            : base( AIType.AI_Mage, FightMode.Closest )
        {
            Name = "Fangor";
            Title = ", the Evil Green DragonLord";
            Hue = Utility.RandomGreenHue();
            Body = 106;		// ShadowWyrm
            BaseSoundID = 362;

            SetStr( 898, 1030 );
            SetDex( 68, 200 );
            SetInt( 488, 620 );

            SetHits( 30000 );
            SetMana( 5000 );

            SetDamage( 29, 35 );

            SetDamageType( ResistanceType.Physical, 0 );
            SetDamageType( ResistanceType.Fire, 0 );
            SetDamageType( ResistanceType.Cold, 0 );
            SetDamageType( ResistanceType.Poison, 100 );
            SetDamageType( ResistanceType.Energy, 0 );

            SetResistance( ResistanceType.Physical, 95, 100 );
            SetResistance( ResistanceType.Fire, 95, 100 );
            SetResistance( ResistanceType.Cold, 75, 100 );
            SetResistance( ResistanceType.Poison, 95, 100 );
            SetResistance( ResistanceType.Energy, 95, 100 );

            SetSkill( SkillName.EvalInt, 120.1, 130.0 );
            SetSkill( SkillName.Magery, 120.1, 130.0 );
            SetSkill( SkillName.Meditation, 110.0, 120.0 );
            SetSkill( SkillName.MagicResist, 150.5, 200.0 );
            SetSkill( SkillName.Tactics, 70.1, 100.0 );
            SetSkill( SkillName.Wrestling, 160.1, 180.0 );

            Fame = 50000;
            Karma = -5000;  // Legale Malvagio

            VirtualArmor = 130;

            Tamable = false;

            // Mosse speciali dell' ATS
            FireBreathAttack = Utility.RandomMinMax( 175, 200 );
            PetPoisonAttack = Utility.RandomMinMax( 175, 200 );
            RoarAttack = Utility.RandomMinMax( 175, 200 );

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

        public Fangor( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override double BreathDamageScalar { get { return 0.003; } }
        public override int BreathColdDamage { get { return 0; } }
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathEnergyDamage { get { return 0; } }
        public override int BreathPhysicalDamage { get { return 0; } }
        public override int BreathPoisonDamage { get { return 100; } }

        public override ScaleType ScaleType { get { return ScaleType.Green; } }
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
