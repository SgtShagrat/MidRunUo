/***************************************************************************
 *                               GreenDragon.cs
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
    [CorpseName( "the corpse of a green dragon" )]
    public class GreenDragon : BaseDragon
    {
        [Constructable]
        public GreenDragon()
            : base( AIType.AI_Mage, FightMode.Closest )
        {
            Name = "a green dragon";

            Hue = Utility.RandomList( 0x557, 0x88D, 0x874, 0x780 );

            Body = Utility.RandomList( 12, 59 );
            BaseSoundID = 362;

            if( Core.AOS )
            {
                SetStr( 796, 825 );
                SetDex( 86, 105 );
                SetInt( 436, 475 );

                SetHits( 478, 495 );

                SetDamage( 16, 22 );

                Fame = 10000;
                Karma = -1000; // Legale Malvagio

                VirtualArmor = 80;
            }
            else
            {
                SetStr( 600 );
                SetDex( 120 );
                SetInt( 200 );

                SetHits( 600 );
                SetMana( 200 );
                SetStam( 120 );

                SetDamage( "6d8" );

                VirtualArmor = 55;

                Karma = -8000;
                Fame = +8000;
            }

            SetDamageType( ResistanceType.Physical, 0 );
            SetDamageType( ResistanceType.Fire, 0 );
            SetDamageType( ResistanceType.Cold, 0 );
            SetDamageType( ResistanceType.Poison, 100 );
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
            get { return 0; }
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
            get { return 100; }
        }

        public override ScaleType ScaleType
        {
            get { return ScaleType.Green; }
        }

        public override HideType HideType
        {
            get { return HideType.GreenDragon; }
        }

        public override int Hides
        {
            get { return 20; }
        }

        public override int CustomWeaponSpeed
        {
            get { return 45; }
        }

        public override bool AlwaysMurderer
        {
            get { return false; }
        }

        public override void GenerateLoot()
        {
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.Gems, 8 );
        }

        #region Serialize-Deserialize
        public GreenDragon( Serial serial )
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