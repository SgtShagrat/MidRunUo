/***************************************************************************
 *                                  Viurgor.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Viurgor e' il signore dei draghi blu.
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Midgard.Mobiles
{
    [CorpseName( "the corpse of a Blue Dragonlord" )]
    public class Viurgor : BaseLordOfDragons
    {
        #region costruttori
        [Constructable]
        public Viurgor()
            : base( AIType.AI_Mage, FightMode.Closest )
        {
            Name = "Viurgor";
            Title = ", the Evil Blue DragonLord";
            Hue = Utility.RandomBlueHue();		// Blu intenso
            Body = 106;		// ShadowWyrm
            BaseSoundID = 362;

            SetStr( 898, 1030 );
            SetDex( 68, 200 );
            SetInt( 488, 620 );

            SetHits( 10000 );
            SetMana( 3000 );

            SetDamage( 29, 35 );

            SetDamageType( ResistanceType.Physical, 0 );
            SetDamageType( ResistanceType.Fire, 0 );
            SetDamageType( ResistanceType.Cold, 100 );
            SetDamageType( ResistanceType.Poison, 0 );
            SetDamageType( ResistanceType.Energy, 0 );

            SetResistance( ResistanceType.Physical, 95, 100 );
            SetResistance( ResistanceType.Fire, 60, 80 );
            SetResistance( ResistanceType.Cold, 95, 100 );
            SetResistance( ResistanceType.Poison, 95, 100 );
            SetResistance( ResistanceType.Energy, 95, 100 );

            SetSkill( SkillName.EvalInt, 120.1, 130.0 );
            SetSkill( SkillName.Magery, 120.1, 130.0 );
            SetSkill( SkillName.Meditation, 110.0, 120.0 );
            SetSkill( SkillName.MagicResist, 150.5, 200.0 );
            SetSkill( SkillName.Tactics, 70.1, 100.0 );
            SetSkill( SkillName.Wrestling, 160.1, 180.0 );

            Fame = 30000;
            Karma = -30000; // Caotico Malvagio

            VirtualArmor = 110;

            Tamable = false;

            // Mosse speciali dell' ATS
            FireBreathAttack = Utility.RandomMinMax( 120, 150 );
            PetPoisonAttack = Utility.RandomMinMax( 120, 150 );
            RoarAttack = Utility.RandomMinMax( 150, 175 );

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

        public Viurgor( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override double BreathDamageScalar { get { return 0.01; } }
        public override int BreathColdDamage { get { return 100; } }
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathEnergyDamage { get { return 0; } }
        public override int BreathPhysicalDamage { get { return 0; } }
        public override int BreathPoisonDamage { get { return 0; } }

        public override ScaleType ScaleType { get { return ScaleType.Blue; } }
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
