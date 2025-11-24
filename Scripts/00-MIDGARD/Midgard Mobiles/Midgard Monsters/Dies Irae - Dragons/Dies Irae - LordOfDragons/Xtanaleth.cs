/***************************************************************************
 *                                  Xtanaleth.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Xtanaleth e' il cucciolo di un signore dei draghi blu.
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Midgard.Mobiles
{
    [CorpseName( "the corpse of a little Dragonlord" )]
    public class Xtanaleth : BaseLordOfDragons
    {
        #region costruttori
        [Constructable]
        public Xtanaleth()
            : base( AIType.AI_Mage, FightMode.Closest )
        {
            Name = "Xtanaleth";
            Title = ", the Evil Blu DragonLord (young)";
            Hue = 1765;		// Celeste
            Body = 60;		// Drake
            BaseSoundID = 362;

            SetStr( 300 );
            SetDex( 100 );
            SetInt( 750 );

            SetHits( 3000 );
            SetMana( 2000 );

            SetDamage( 13, 15 );

            SetDamageType( ResistanceType.Physical, 0 );
            SetDamageType( ResistanceType.Fire, 0 );
            SetDamageType( ResistanceType.Cold, 100 );
            SetDamageType( ResistanceType.Poison, 0 );
            SetDamageType( ResistanceType.Energy, 0 );

            SetResistance( ResistanceType.Physical, 75, 100 );
            SetResistance( ResistanceType.Fire, 65, 75 );
            SetResistance( ResistanceType.Cold, 75, 100 );
            SetResistance( ResistanceType.Poison, 75, 100 );
            SetResistance( ResistanceType.Energy, 75, 100 );

            SetSkill( SkillName.EvalInt, 100.0, 110.0 );
            SetSkill( SkillName.Magery, 100.0, 110.0 );
            SetSkill( SkillName.Meditation, 100.0, 110.0 );
            SetSkill( SkillName.MagicResist, 100.0, 110.0 );
            SetSkill( SkillName.Tactics, 100.0, 110.0 );
            SetSkill( SkillName.Wrestling, 100.0, 110.0 );

            Fame = 10000;
            Karma = -5000; // Legale Malvagio

            VirtualArmor = 70;

            Tamable = false;

            // Mosse speciali dell' ATS
            FireBreathAttack = Utility.RandomMinMax( 100, 150 );
            PetPoisonAttack = Utility.RandomMinMax( 100, 150 );
            RoarAttack = Utility.RandomMinMax( 100, 150 );

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

        public Xtanaleth( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override double BreathDamageScalar { get { return 0.03; } }
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
