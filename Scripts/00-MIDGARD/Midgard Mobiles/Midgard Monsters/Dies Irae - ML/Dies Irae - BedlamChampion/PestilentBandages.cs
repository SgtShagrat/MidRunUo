using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a pestilent bandage corpse" )]
    public class PestilentBandages : BaseCreature
    {
        [Constructable]
        public PestilentBandages()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
        {
            Name = "a pestilent bandage";					// VERIFIED
            Body = 154;										// VERIFIED
            BaseSoundID = 471;								// 

            SetStr( 709, 740 );								// VERIFIED
            SetDex( 144, 180 );								// VERIFIED			
            SetInt( 55, 79 );								// VERIFIED

            SetHits( 434 );									// VERIFIED
            SetStam( 144, 180 );							// VERIFIED
            SetMana( 55, 79 );								// VERIFIED

            SetDamage( 13, 23 );

            SetDamageType( ResistanceType.Physical, 40 );	// VERIFIED
            SetDamageType( ResistanceType.Cold, 20 );		// VERIFIED
            SetDamageType( ResistanceType.Cold, 40 );		// VERIFIED

            SetResistance( ResistanceType.Physical, 46, 47 );	// VERIFIED
            SetResistance( ResistanceType.Fire, 10, 12 );		// VERIFIED
            SetResistance( ResistanceType.Cold, 54, 56 );		// VERIFIED
            SetResistance( ResistanceType.Poison, 22, 25 );		// VERIFIED
            SetResistance( ResistanceType.Energy, 21 );			// VERIFIED

            SetSkill( SkillName.Wrestling, 70.8, 97.8 );	// VERIFIED
            SetSkill( SkillName.Tactics, 75.7, 91.0 );		// VERIFIED         
            SetSkill( SkillName.MagicResist, 48.4, 76.0 );	// VERIFIED

            Fame = 4000;									// TODO VERIFY
            Karma = -4000;									// TODO VERIFY

            VirtualArmor = 50;								// TODO VERIFY

            PackItem( new Garlic( 5 ) );					// TODO VERIFY
            PackItem( new Bandage( 10 ) );					// TODO VERIFY
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich );
            AddLoot( LootPack.Gems );
            AddLoot( LootPack.Potions );
        }

        public override bool BleedImmune { get { return true; } }				// TODO VERIFY			
        public override Poison PoisonImmune { get { return Poison.Lesser; } }	// TODO VERIFY
        public override Poison HitPoison { get { return Poison.Lethal; } }	// TODO VERIFY

        public PestilentBandages( Serial serial )
            : base( serial )
        {
        }

        public override OppositionGroup OppositionGroup
        {
            get { return OppositionGroup.FeyAndUndead; }
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
