/*npctemplate                  ghast
{
    name                     a ghast
    script                   spellkillpcs
    objtype                  0x03
    color                    1154
    truecolor                1154
    gender                   0
    str                      200
    int                      100
    dex                      90
    hits                     200
    mana                     100
    stam                     90
    parry                    90
    magicresistance          80
    tactics                  110
    wrestling                120
    magery                   85
    evaluatingintelligence   75
    cast_pct                 42
    num_casts                8
    attackspeed              35
    attackdamage             3d8+8
    attackskillid            wrestling
    attackhitsound           0x1da
    attackmisssound          0x239
    ar                       25
    cprop                    undead	i7
    hostile                  1
    dstart                   10
    lootgroup                26
    aspell                   15
    dspell                   9
    powergroup               37
    karma                    -1600    -2000
    fame                     800     1000
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a ghast corpse" )]
	public class Ghast : BaseCreature
	{
		[Constructable]
		public Ghast() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			Name = "a ghast";
			Body = 0x03;
			BaseSoundID = 471;
			Hue = 1154;

			SetStr( 200 );
			SetDex( 90 );
			SetInt( 100 );

			SetHits( 200 );
			SetMana( 100 );

			SetDamage( 11, 32 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.EvalInt, 75.0 );
			SetSkill( SkillName.Magery, 85.0 );
			SetSkill( SkillName.MagicResist, 80.0 );
			SetSkill( SkillName.Tactics, 110.0 );
			SetSkill( SkillName.Wrestling, 120.0 );

			Fame = Utility.RandomMinMax( 800, 1000 );
			Karma = Utility.RandomMinMax( -1600, -2000 );

			VirtualArmor = 25;

			if ( /* Core.ML && */ Utility.RandomDouble() < .33 )
				PackItem( Engines.Plants.Seed.RandomPeculiarSeed(2) );

			PackItem( new Garlic( 5 ) );
			PackItem( new Bandage( 10 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems );
			AddLoot( LootPack.Potions );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }

		public Ghast( Serial serial ) : base( serial )
		{
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}