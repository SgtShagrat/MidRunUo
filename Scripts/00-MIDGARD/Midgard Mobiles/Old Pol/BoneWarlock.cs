/*npctemplate                  bonewarlock
{
    name                     a bone warlock
    script                   spellkillpcs
    objtype                  0x38
    color                    0
    truecolor                0
    gender                   0
    str                      200
    int                      145
    dex                      84
    hits                     200
    mana                     145
    stam                     84
    magicresistance          100
    tactics                  100
    wrestling                110
    evaluatingintelligence   90
    magery                   100
    cast_pct                 40
    num_casts                10
    attackspeed              30
    attackdamage             2d5
    attackskillid            wrestling
    attackhitsound           0x16c
    attackmisssound          0x239
    ar                       25
    cprop                    undead	i6
    hostile                  1
    speech                   54
    dstart                   10
    saywords                 1
    lootgroup                26
    aspell                   15
    dspell                   9
    powergroup               42
    karma                    -2000    -2500
    fame                     1000     1250
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a bone warlock corpse" )]
	public class BoneWarlock : BaseCreature, ISkeleton
	{
		[Constructable]
		public BoneWarlock() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a bone warlock";
			Body = 0x38;
			BaseSoundID = 451;
			Hue = 0;

			SetStr( 200 );
			SetDex( 84 );
			SetInt( 145 );

			SetHits( 200 );
			SetMana( 145 );

			SetDamage( 2, 10 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 90.0 );
			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 110.0 );

			Fame = Utility.RandomMinMax( 1000, 1250 );
			Karma = Utility.RandomMinMax( -2000, -2500 );

			VirtualArmor = 25;
			PackReg( 3 );
			PackNecroReg( 3, 10 );
			PackItem( new Bone() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.LowScrolls );
			AddLoot( LootPack.Potions );
		}
		
		public override bool BleedImmune{ get{ return true; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public BoneWarlock( Serial serial ) : base( serial )
		{
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