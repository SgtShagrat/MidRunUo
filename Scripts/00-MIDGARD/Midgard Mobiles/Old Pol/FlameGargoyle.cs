/*npctemplate                  flamegargoyle
{
    name                     a flaming gargoyle
    script                   spellkillpcs
    objtype                  0x4
    color                    232
    truecolor                232
    gender                   0
    str                      240
    int                      150
    dex                      85
    hits                     240
    mana                     150
    stam                     85
    magicresistance          80
    evaluatingintelligence   75
    tactics                  90
    wrestling                150
    magery                   85
    cast_pct                 30
    num_casts                8
    attackspeed              30
    attackdamage             6d6
    attackskillid            wrestling
    attackhitsound           0x177
    attackmisssound          0x239
    ar                       33
    hostile                  1
    speech                   54
    provoke                  85
    dstart                   10
    lootgroup                33
    aspell                   4
    dspell                   9
    powergroup               48
    karma                    -2500    -3000
    fame                     1250     1500
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
} */
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a charred corpse" )]
	public class FlameGargoyle : BaseCreature
	{
		[Constructable]
		public FlameGargoyle() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "fire gargoyle" );
			Body = 130;
			BaseSoundID = 0x174;
			Hue = 232;

			SetStr( 240 );
			SetDex( 85 );
			SetInt( 150 );

			SetHits( 240 );
			SetStam( 85 );
			SetMana( 150 );

			SetDamage( 6, 36 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.EvalInt, 75.0 );
			SetSkill( SkillName.Magery, 85.0 );
			SetSkill( SkillName.MagicResist, 80.0 );
			SetSkill( SkillName.Tactics, 90.0 );
			SetSkill( SkillName.Wrestling, 150.0 );

			Fame = Utility.RandomMinMax( 1250, 1500 );
			Karma = Utility.RandomMinMax( -2500, -3000 );

			VirtualArmor = 33;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }

		public FlameGargoyle( Serial serial ) : base( serial )
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