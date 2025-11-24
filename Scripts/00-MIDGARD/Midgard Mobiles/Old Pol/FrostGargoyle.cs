/*npctemplate                  frostgargoyle
{
    name                     a frost gargoyle
    script                   spellkillpcs
    objtype                  0x4
    color                    1154
    truecolor                1154
    gender                   0
    str                      270
    int                      170
    dex                      85
    hits                     270
    mana                     170
    stam                     85
    magicresistance          100
    tactics                  120
    wrestling                135
    magery                   85
    evaluatingintelligence   75
    cast_pct                 25
    num_casts                5
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
    lootgroup                35
    aspell                   5
    dspell                   9
    powergroup               52
    karma                    -3000    -3500
    fame                     1500     1750
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
	public class FrostGargoyle : BaseCreature
	{
		[Constructable]
		public FrostGargoyle() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "fire gargoyle" );
			Body = 130;
			BaseSoundID = 0x174;
			Hue = 1154;

			SetStr( 270 );
			SetDex( 85 );
			SetInt( 170 );

			SetHits( 270 );
			SetStam( 85 );
			SetMana( 170 );

			SetDamage( 6, 36 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.EvalInt, 75.0 );
			SetSkill( SkillName.Magery, 85.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 135.0 );

			Fame = Utility.RandomMinMax( 1500, 1750 );
			Karma = Utility.RandomMinMax( -3000, -3500 );

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

		public FrostGargoyle( Serial serial ) : base( serial )
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