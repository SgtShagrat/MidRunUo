/*npctemplate                  hellhound
{
    name                     a hell hound
    script                   spellkillpcs
    noloot                   1
    objtype                  0xe1
    color                    232
    truecolor                232
    gender                   0
    str                      175
    int                      100
    dex                      135
    hits                     175
    mana                     100
    stam                     135
    tactics                  100
    wrestling                110
    magery                   90
    evaluatingintelligence   75
    magicresistance          90
    cast_pct                 35
    num_casts                5
    attackspeed              35
    attackdamage             4d4
    attackskillid            wrestling
    attackhitsound           0xe8
    attackmisssound          0x239
    ar                       25
    tameskill                90
    food                     meat
    hostile                  1
    provoke                  80
    dstart                   10
    lootgroup                28
    aspell                   15
    dspell                   9
    powergroup               39
    karma                    -1600    -2000
    fame                     800     1000
    attackhitscript          :combat:npchitscript
    vendorsellsfor           10000
    vendorbuysfor            7000
}*/
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a hell hound corpse" )]
	public class OldHellHound : BaseCreature
	{
		[Constructable]
		public OldHellHound() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a hell hound";
			Body = 0xe1;
			BaseSoundID = 229;
			Hue = 232;

			SetStr( 175 );
			SetDex( 135 );
			SetInt( 100 );

			SetHits( 175 );
			SetStam( 135 );
			SetMana( 100 );

			SetDamage( 4, 16 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.EvalInt, 75.0 );
			SetSkill( SkillName.Magery, 90.0 );
			SetSkill( SkillName.MagicResist, 90.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 110.0 );

			Fame = Utility.RandomMinMax( 800, 1000 );
			Karma = Utility.RandomMinMax( -1600, -2000 );

			VirtualArmor = 25;

			//Tamable = true;
			//ControlSlots = 1;
			//MinTameSkill = 85.5;

			PackItem( new SulfurousAsh( 5 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

		public OldHellHound( Serial serial ) : base( serial )
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