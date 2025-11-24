/*npctemplate                  flamingspider
{
    name                     a giant flaming spider
    script                   firebreather
    noloot                   1
    objtype                  0x1c
    color                    232
    truecolor                232
    gender                   0
    str                      160
    int                      80
    dex                      56
    hits                     160
    mana                     110
    stam                     56
    tactics                  70
    wrestling                85
    magery                   100
    evaluatingintelligence   85
    magicresistance          90
    cast_pct                 40
    num_casts                8
    attackspeed              30
    attackdamage             3d8+2
    attackskillid            wrestling
    attackhitsound           0x186
    attackmisssound          0x239
    ar                       27
    tameskill                95
    food                     meat
    provoke                  70
    dstart                   10
    hostile                  1
    lootgroup                24
    aspell                   20
    dspell                   9
    powergroup               31
    karma                    -1300    -1600
    fame                     650     800
    attackhitscript          :combat:npchitscript
    vendorsellsfor           4000
    vendorbuysfor            2800
}*/
using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a flaming spider corpse" )]
	public class FlamingSpider : BaseCreature
	{
		[Constructable]
		public FlamingSpider() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a flaming spider";
			Body = 0x1c;
			BaseSoundID = 0x388;
			Hue = 232;

			SetStr( 150, 170 );
			SetDex( 50, 70 );
			SetInt( 75, 85 );

			SetHits( 150, 160 );
			SetMana( 110 );

			SetDamage( 5, 26 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.EvalInt, 80.1, 90.0 );
			SetSkill( SkillName.Magery, 95.1, 100.0 );
			SetSkill( SkillName.MagicResist, 85.1, 90.0 );
			SetSkill( SkillName.Tactics, 65.0, 75.0 );
			SetSkill( SkillName.Wrestling, 80.1, 90.0 );

			Fame = Utility.RandomMinMax( 650, 800 );
			Karma = Utility.RandomMinMax( -1300, -1600 );

			VirtualArmor = 16;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 95.1;

			PackItem( new SulfurousAsh( 5 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
		}

		public override bool HasBreath{ get{ return true; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int Hides{ get{ return 4; } }
		public override HideType HideType{ get{ return HideType.Lava; } }

		public FlamingSpider( Serial serial ) : base( serial )
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