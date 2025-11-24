/*npctemplate                  beholder
{
    name                     a beholder
    script                   spellkillpcs
    objtype                  0x16
    color                    0x8455
    truecolor                0x8455
    gender                   0
    str                      160
    int                      170
    dex                      90
    hits                     160
    mana                     170
    stam                     90
    magicresistance          100
    evaluatingintelligence   90
    tactics                  65
    wrestling                100
    magery                   125
    cast_pct                 50
    num_casts                40
    saywords                 1
    attackspeed              33
    attackdamage             1d3+2
    attackskillid            wrestling
    attackhitsound           0x17c
    attackmisssound          0x239
    ar                       8d4
    hostile                  1
    provoke                  75
    dstart                   10
    lootgroup                27
    aspell                   15
    dspell                   9
    powergroup               41
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
	[CorpseName( "a beholder corpse" )]
	public class Beholder : BaseCreature
	{
		[Constructable]
		public Beholder () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a beholder";
			Body = 0x16;
			BaseSoundID = 377;
			Hue = 0x8455;

			SetStr( 160 );
			SetDex( 90 );
			SetInt( 170 );

			SetHits( 160 );
			SetMana( 170 );
			SetStam( 90 );

			SetDamage( 3, 5 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.EvalInt, 90.0 );
			SetSkill( SkillName.Magery, 125.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 65.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			Fame = Utility.RandomMinMax( 1000, 1250 );
			Karma = Utility.RandomMinMax( -2000, -2500 );

			VirtualArmor = Utility.RandomMinMax( 8, 32 );

			PackItem( new Nightshade( 4 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Potions );
		}

		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }

		public Beholder( Serial serial ) : base( serial )
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