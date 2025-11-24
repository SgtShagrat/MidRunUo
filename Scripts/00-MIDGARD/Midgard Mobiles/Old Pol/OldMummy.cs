/*npctemplate                  mummy
{
    name                     a mummy
    script                   killpcs
    objtype                  0x03
    color                    0x0455
    truecolor                0x0455
    gender                   0
    str                      400
    int                      30
    dex                      40
    hits                     400
    mana                     30
    stam                     40
    magicresistance          100
    tactics                  100
    wrestling                150
    attackspeed              20
    attackdamage             8d8
    attackskillid            wrestling
    attackhitsound           0x1d8
    attackmisssound          0x239
    cprop                    undead	i5
    ar                       25
    hostile                  1
    dstart                   10
    lootgroup                34
    powergroup               47
    karma                    -2500    -3000
    fame                     1250     1500
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
	[CorpseName( "a mummy corpse" )]
	public class OldMummy : BaseCreature
	{
		[Constructable]
		public OldMummy() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			Name = "a mummy";
			Body = 0x03;
			BaseSoundID = 471;
			Hue = 0x0455;

			SetStr( 400 );
			SetDex( 40 );
			SetInt( 30 );

			SetHits( 400 );

			SetDamage( 8, 64 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 150.0 );

			Fame = Utility.RandomMinMax( 1250, 1500 );
			Karma = Utility.RandomMinMax( -2500, -3000 );

			VirtualArmor = 25;

			if ( /* Core.ML && */ Utility.RandomDouble() < .33 )
				PackItem( Engines.Plants.Seed.RandomPeculiarSeed(2) );

			PackItem( new Garlic( 5 ) );
			PackItem( new Bandage( 10 ) );
			PackNecroReg( 12, 40 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems );
			AddLoot( LootPack.Potions );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }

		public OldMummy( Serial serial ) : base( serial )
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