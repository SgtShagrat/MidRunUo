/*npctemplate                  naga
{
    name                     a crimson naga
    script                   spellkillpcs
    objtype                  0x96
    color                    1645
    truecolor                1645
    gender                   0
    str                      300
    dex                      200
    int                      200
    hits                     300
    stam                     200
    mana                     200
    hiding                   200
    stealth                  200
    tactics                  100
    wrestling                100
    tameskill                115
    magery                   100
    evaluatingintelligence   90
    food                     meat
    magicresistance          95
    attackspeed              35
    attackdamage             3d10
    attackskillid            wrestling
    ar                       30
    attackhitsound           0x1c0
    attackmisssound          0x239
    cast_pct                 40
    num_casts                8
    lootgroup                32
    aspell                   12
    dspell                   9
    provoke                  90
    dstart                   10
    lootgroup                28
    powergroup               54
    karma                    -3000    -3500
    fame                     1500     1750
    attackhitscript          :combat:npchitscript
    vendorsellsfor           25000
    vendorbuysfor            17500
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a crimson naga corpse" )]
	public class CrimsonNaga : BaseCreature
	{
		[Constructable]
		public CrimsonNaga() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a crimson naga";
			Body = 0x96;
			BaseSoundID = 412;
			Hue = 1645;

			SetStr( 300 );
			SetDex( 200 );
			SetInt( 200 );

			SetHits( 300 );
			SetStam( 200 );
			SetMana( 200 );

			SetDamage( 8, 35 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 60 );
			SetDamageType( ResistanceType.Energy, 40 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.EvalInt, 90.0 );
			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.MagicResist, 95.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );
			SetSkill( SkillName.Hiding, 200.0 );
			SetSkill( SkillName.Stealth, 200.0 );

			Fame = Utility.RandomMinMax( 1500, 1750 );
			Karma = Utility.RandomMinMax( -3000, -3500 );

			VirtualArmor = 30;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 4; } }

		public CrimsonNaga( Serial serial ) : base( serial )
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