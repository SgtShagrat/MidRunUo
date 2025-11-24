/*npctemplate                  frostgazer
{
    name                     a frigid eye
    script                   spellkillpcs
    objtype                  0x16
    color                    1154
    truecolor                1154
    gender                   0
    str                      160
    int                      170
    dex                      90
    hits                     160
    mana                     170
    stam                     90
    magicresistance          100
    evaluatingintelligence   75
    tactics                  65
    wrestling                100
    magery                   95
    cast_pct                 50
    num_casts                10
    attackspeed              33
    attackdamage             5d3+5
    attackskillid            wrestling
    attackhitsound           0x17c
    attackmisssound          0x239
    ar                       8d4
    hostile                  1
    provoke                  75
    dstart                   10
    lootgroup                27
    aspell                   7
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
	[CorpseName( "a eye corpse" )]
	public class FrostGazer : BaseCreature
	{
		[Constructable]
		public FrostGazer () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a frigid eye";
			Body = 0x16;
			BaseSoundID = 377;
			Hue = 1154;

			SetStr( 160 );
			SetDex( 90 );
			SetInt( 170 );

			SetHits( 160 );
			SetMana( 170 );
			SetStam( 90 );

			SetDamage( 10, 20 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.EvalInt, 75.0 );
			SetSkill( SkillName.Magery, 95.0 );
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

		public FrostGazer( Serial serial ) : base( serial )
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