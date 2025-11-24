/*npctemplate                  lichelord
{
    name                     a liche lord
    script                   spellkillpcs
    objtype                  0x18
    color                    0x4631
    truecolor                0x4631
    gender                   0
    str                      280
    int                      260
    dex                      75
    hits                     280
    mana                     260
    stam                     75
    magicresistance          100
    evaluatingintelligence   110
    tactics                  70
    macefighting             125
    magery                   140
    attackspeed              30
    attackdamage             3d10+5
    attackskillid            macefighting
    attackhitsound           0x19f
    attackmisssound          0x239
    ar                       28
    cprop                    undead	i8
    cast_pct                 42
    num_casts                8
    hostile                  1
    speech                   35
    dstart                   10
    saywords                 1
    lootgroup                33
    aspell                   9
    dspell                   9
    magicgroups              1
    powergroup               61
    karma                    -4500    -5500
    fame                     2250     2750
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a liche's corpse" )]
	public class OldLichLord : BaseCreature
	{
		[Constructable]
		public OldLichLord() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		// public LichLord() : base( AIType.AI_Necromage, FightMode.Closest, 10, 1, 0.2, 0.4 ) // Modifica by Dies Irae
		{
			Name = "a lich lord";
			Body = 79;
			BaseSoundID = 412;
			Hue = 0x4631;

			SetStr( 280 );
			SetDex( 75 );
			SetInt( 260 );

			SetHits( 280 );
			SetStam( 75 );
			SetMana( 260 );

			SetDamage( 8, 35 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 60 );
			SetDamageType( ResistanceType.Energy, 40 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.EvalInt, 110.0 );
			SetSkill( SkillName.Magery, 140.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 70.0 );
			SetSkill( SkillName.Wrestling, 125.0 );

			Fame = Utility.RandomMinMax( 2250, 2750 );
			Karma = Utility.RandomMinMax( -4500, -5500 );

			VirtualArmor = 28;
			PackItem( new GnarledStaff() );
			PackNecroReg( 12, 40 );
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
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

		public OldLichLord( Serial serial ) : base( serial )
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