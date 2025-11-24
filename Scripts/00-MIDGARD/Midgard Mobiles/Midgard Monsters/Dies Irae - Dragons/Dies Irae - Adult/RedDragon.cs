/***************************************************************************
 *                               RedDragon.cs
 *
 *   begin                : 19 luglio 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "the corpse of a red dragon" )]
    public class RedDragon : BaseDragon
    {
        [Constructable]
        public RedDragon()
            : base( AIType.AI_Mage, FightMode.Closest )
        {
            Name = "a red dragon";

            Hue = Utility.RandomList( 0x991, 0x993, 0x78D, 0xA73 );
            Body = Utility.RandomList( 12, 59 );
            BaseSoundID = 362;

            if( Core.AOS )
            {
                SetStr( 796, 825 );
                SetDex( 86, 105 );
                SetInt( 436, 475 );

                SetHits( 478, 495 );

                SetDamage( 16, 22 );

                VirtualArmor = 90;
            }
            else
            {
                SetStr( 1500 );
                SetDex( 175 );
                SetInt( 500 );

                SetHits( 1500 );
                SetMana( 500 );
                SetStam( 175 );

                SetDamage( "10d6" );

                VirtualArmor = 45;

                Karma = -15000;
                Fame = +15000;
            }

            SetDamageType( ResistanceType.Physical, 0 );
            SetDamageType( ResistanceType.Fire, 100 );
            SetDamageType( ResistanceType.Cold, 0 );
            SetDamageType( ResistanceType.Poison, 0 );
            SetDamageType( ResistanceType.Energy, 0 );

            SetResistance( ResistanceType.Physical, 90, 100 );
            SetResistance( ResistanceType.Fire, 90, 100 );
            SetResistance( ResistanceType.Cold, 70, 100 );
            SetResistance( ResistanceType.Poison, 90, 100 );
            SetResistance( ResistanceType.Energy, 90, 100 );

            SetSkill( SkillName.EvalInt, 110.0, 120.0 );
            SetSkill( SkillName.Magery, 110.0, 120.0 );
            SetSkill( SkillName.Meditation, 110.0, 120.0 );
            SetSkill( SkillName.MagicResist, 110.0, 120.0 );
            SetSkill( SkillName.Tactics, 110.0, 120.0 );
            SetSkill( SkillName.Wrestling, 110.0, 120.0 );

            Tamable = false;
        }

        public override int BreathColdDamage
        {
            get { return 0; }
        }

        public override int BreathFireDamage
        {
            get { return 100; }
        }

        public override int BreathEnergyDamage
        {
            get { return 0; }
        }

        public override int BreathPhysicalDamage
        {
            get { return 0; }
        }

        public override int BreathPoisonDamage
        {
            get { return 0; }
        }

        public override ScaleType ScaleType
        {
            get { return ScaleType.Red; }
        }

        public override HideType HideType
        {
            get { return HideType.RedDragon; }
        }

        public override int Hides
        {
            get { return 20; }
        }

        public override void GenerateLoot()
        {
			AddLoot( LootPack.FilthyRich, 4 );
			AddLoot( LootPack.Gems, 8 );
        }

        public override int CustomWeaponSpeed
        {
            get { return 50; }
        }

        #region Serialize-Deserialize
        public RedDragon( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }
}