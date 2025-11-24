/*npctemplate                  skeletonherosdead
{
    name                     skeleton hero
    script                   killpcs
    objtype                  0x32
    color                    0
    truecolor                0
    gender                   0
    str                      65
    int                      15
    dex                      100
    hits                     65
    mana                     0
    stam                     50
    tactics                  50
    wrestling                95
    magicresistance          30
    attackspeed              25
    attackdamage             2d6+3
    attackskillid            wrestling
    attackhitsound           0x1c6
    attackmisssound          0x239
    deathsound               0x1c8
    ar                       6
    cprop                    undead	i2
    lootgroup                10
    alignment                evil
    dstart                   10
    provoke                  49
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
	[CorpseName( "a skeleton hero corpse" )]
	public class SkeletonHero : BaseCreature, ISkeleton
	{
		[Constructable]
		public SkeletonHero() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a skeleton hero";
			Body = 0x32;
			BaseSoundID = 451;
			Hue = 0;

			SetStr( 65 );
			SetDex( 100 );
			SetInt( 15 );

			SetHits( 65 );
			SetStam( 100 );
			SetMana( 15 );

			SetDamage( 5, 15 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 15, 25 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.MagicResist, 30.0 );
			SetSkill( SkillName.Tactics, 50.0 );
			SetSkill( SkillName.Wrestling, 95.0 );

			Fame = Utility.RandomMinMax( 100, 125 );
			Karma = Utility.RandomMinMax( -200, -250 );

			VirtualArmor = 6;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
		}

		public override bool BleedImmune{ get{ return true; } }

		public SkeletonHero( Serial serial ) : base( serial )
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