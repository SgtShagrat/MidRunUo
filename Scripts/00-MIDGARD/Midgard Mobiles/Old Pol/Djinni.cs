/*npctemplate                  djinni
{
    name                     a djinni
    script                   spellkillpcs
    objtype                  0x0d
    color                    93
    truecolor                93
    gender                   0
    str                      400
    int                      210
    dex                      90
    hits                     400
    mana                     210
    stam                     90
    tactics                  85
    wrestling                140
    magery                   130
    evaluatingintelligence   115
    magicresistance          100
    cast_pct                 60
    num_casts                12
    attackspeed              35
    attackdamage             5d3
    attackskillid            wrestling
    attackhitsound           0x10a
    attackmisssound          0x239
    ar                       35
    cprop                    nocorpse   i1
    dstart                   10
    saywords                 1
    lootgroup                32
    aspell                   8
    dspell                   9
    powergroup               59
    karma                    -3500    -4500
    fame                     1750     2250
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
	[CorpseName( "a djinni corpse" )]
	public class Djinni : BaseCreature
	{
		[Constructable]
		public Djinni() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			Name = "a djinni";
			Body = 0x0d;
			//BaseSoundID = 471;
			Hue = 93;

			SetStr( 400 );
			SetDex( 90 );
			SetInt( 210 );

			SetHits( 400 );
			SetMana( 210 );

			SetDamage( 5, 15 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.EvalInt, 115.0 );
			SetSkill( SkillName.Magery, 130.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 85.0 );
			SetSkill( SkillName.Wrestling, 140.0 );

			Fame = Utility.RandomMinMax( 1750, 2250 );
			Karma = Utility.RandomMinMax( -3500, -4500 );

			VirtualArmor = 35;
		}

		public override void GenerateLoot()
		{
			//AddLoot( LootPack.Rich );
			//AddLoot( LootPack.Gems );
			//AddLoot( LootPack.Potions );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }

		public Djinni( Serial serial ) : base( serial )
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