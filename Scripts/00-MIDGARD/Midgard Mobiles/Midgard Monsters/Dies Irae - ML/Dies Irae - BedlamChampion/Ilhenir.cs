/*using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    public class Ilhenir : BaseChampion
    {
        public override ChampionSkullType SkullType { get { return ChampionSkullType.Power; } } // TODO verify

        [Constructable]
        public Ilhenir()
            : base( AIType.AI_Melee )
        {
            Name = "Ilhenir";				// VERIFIED
            Title = "the stained";			// VERIFIED

            Body = 0x103;					// VERIFIED
            BaseSoundID = 589;				// VERIFIED

            SetStr( 1191, 1264 );			// VERIFIED
            SetDex( 134 );					// VERIFIED
            SetInt( 605, 644 );				// VERIFIED

            SetHits( 9000 );				// VERIFIED
            SetStam( 134 );					// VERIFIED
            SetMana( 605, 644 );				// VERIFIED

            SetDamage( 25, 35 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 56 );		// VERIFIED
            SetResistance( ResistanceType.Fire, 51 );			// VERIFIED
            SetResistance( ResistanceType.Cold, 63 );			// VERIFIED
            SetResistance( ResistanceType.Poison, 90 );			// VERIFIED
            SetResistance( ResistanceType.Energy, 74 );			// VERIFIED

            SetSkill( SkillName.Wrestling, 114.2, 119.9 );		// VERIFIED
            SetSkill( SkillName.Tactics, 111.7, 119.9 );		// VERIFIED 
            SetSkill( SkillName.MagicResist, 120.0 );			// VERIFIED 
            SetSkill( SkillName.Anatomy, 114.1, 117.5 );		// VERIFIED 
            SetSkill( SkillName.Poisoning, 0.0, 5.4 );			// VERIFIED
            SetSkill( SkillName.Magery, 100.0 );				// VERIFIED 
            SetSkill( SkillName.EvalInt, 100.0 );				// VERIFIED
            SetSkill( SkillName.Meditation, 100.0 );			// VERIFIED

            Fame = 22500;										// TODO verify
            Karma = -22500;										// TODO verify

            VirtualArmor = 68;									// TODO verify
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.UltraRich, 3 );					// TODO verify
        }

        public override bool ShowFameTitle { get { return false; } }
        public override bool ClickTitle { get { return false; } }

        public override bool HasBreath { get { return true; } }	// VERIFIED

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.DoubleStrike;					// VERIFIED
        }

        public Ilhenir( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}
*/