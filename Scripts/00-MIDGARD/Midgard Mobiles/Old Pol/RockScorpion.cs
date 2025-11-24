/*
npctemplate                  rockscorpion
{
    name                     a giant rock scorpion
    script                   poisonkillpcs
    noloot                   1
    objtype                  0x30
    color                    1118
    truecolor                1118
    gender                   0
    str                      200
    int                      60
    dex                      70
    hits                     200
    mana                     60
    stam                     70
    tactics                  70
    wrestling                130
    poisoning                90
    magicresistance          50
    attackspeed              35
    attackdamage             8d4+3
    attackskillid            wrestling
    attackhitsound           0x190
    attackmisssound          0x239
    ar                       32
    poisondamagelvl          2d2
    tameskill                90
    food                     meat
    provoke                  70
    dstart                   10
    lootgroup                23
    powergroup               31
    karma                    -1300    -1600
    fame                     650     800
    attackhitscript          :combat:npchitscript
    vendorsellsfor           2000
    vendorbuysfor            1400
}*/
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a scorpion corpse" )]
	public class RockScorpion : BaseCreature
	{
		[Constructable]
		public RockScorpion() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a giant rock scorpion";
			Body = 0x30;
			BaseSoundID = 397;
			Hue = 1118;

			SetStr( 200 );
			SetDex( 70 );
			SetInt( 60 );

			SetHits( 200 );
			SetStam( 70 );
			SetMana( 60 );

			SetDamage( 11, 35 );

			SetDamageType( ResistanceType.Physical, 60 );
			SetDamageType( ResistanceType.Poison, 40 );

			SetResistance( ResistanceType.Physical, 20, 25 );
			SetResistance( ResistanceType.Fire, 10, 15 );
			SetResistance( ResistanceType.Cold, 20, 25 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 10, 15 );

			SetSkill( SkillName.Poisoning, 90.0 );
			SetSkill( SkillName.MagicResist, 50.0 );
			SetSkill( SkillName.Tactics, 70.0 );
			SetSkill( SkillName.Wrestling, 130.0 );

			Fame = Utility.RandomMinMax( 650, 800 );
			Karma = Utility.RandomMinMax( -1300, -1600 );

			VirtualArmor = 32;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 90.1;

			PackItem( new LesserPoisonPotion() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override Poison HitPoison{ get{ return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly); } }

		public RockScorpion( Serial serial ) : base( serial )
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