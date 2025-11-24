/*npctemplate                  skeletalassassin
{
    name                     a skeletal assassin
    script                   poisonkillpcs
    objtype                  0x32
    color                    0x4631
    truecolor                0x4631
    gender                   0
    str                      240
    int                      100
    dex                      115
    hits                     240
    mana                     100
    stam                     115
    magicresistance          100
    poisoning                70
    tactics                  125
    swordsmanship            125
    hiding                   90
    stealth                  85
    attackspeed              45
    attackdamage             3d4+3
    attackskillid            swordsmanship
    poisondamagelvl          1d3
    attackhitsound           0x1c6
    attackmisssound          0x239
    ar                       34
    cprop                    undead	i6
    hostile                  1
    buddytext                "emos hetairos"
    leadertext               "ego akoloutheou"
    targettext               "ego apokteinou"
    dstart                   10
    lootgroup                24
    powergroup               45
    karma                    -2000    -2500
    fame                     1000     1250
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
	[CorpseName( "a skeletal assassin corpse" )]
	public class SkeletalAssassin : BaseCreature, ISkeleton
	{
		[Constructable]
		public SkeletalAssassin() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a skeletal assassin";
			Body = 0x32;
			BaseSoundID = 451;
			Hue = 0x4631;

			SetStr( 240 );
			SetDex( 115 );
			SetInt( 100 );

			SetHits( 240 );

			SetDamage( 6, 15 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Poisoning, 70.0 );
			SetSkill( SkillName.Tactics, 125.0 );
			SetSkill( SkillName.Wrestling, 125.0 );

			Fame = Utility.RandomMinMax( 1000, 1250 );
			Karma = Utility.RandomMinMax( -2000, -2500 );

			VirtualArmor = 34;
			PackNecroReg( 12, 40 );
		}

		public override Poison HitPoison{ get{ return Poison.Regular; } }
		public override double HitPoisonChance{ get{ return 0.75; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
		}

		public override bool BleedImmune{ get{ return true; } }

		public SkeletalAssassin( Serial serial ) : base( serial )
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