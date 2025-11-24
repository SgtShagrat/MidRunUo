/*npctemplate                  wingedavenger
{
    name                     a winged avenger
    script                   spellkillpcs
    objtype                  0x01e
    color                    0x0455
    truecolor                0x0455
    gender                   0
    str                      310
    int                      150
    dex                      100
    hits                     310
    mana                     150
    stam                     100
    tactics                  140
    wrestling                200
    magicresistance          100
    evaluatingintelligence   75
    magery                   85
    attackspeed              25
    attackdamage             6d6
    attackskillid            wrestling
    attackhitsound           0x195
    attackmisssound          0x239
    ar                       30
    cast_pct                 40
    num_casts                8
    hostile                  1
    provoke                  70
    dstart                   10
    lootgroup                33
    aspell                   7
    dspell                   9
    powergroup               56
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
	[CorpseName( "a winged avenger corpse" )]
	public class WingedAvenger : BaseCreature
	{
		[Constructable]
		public WingedAvenger() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a winged avenger";
			Body = 0x01e;
			BaseSoundID = 402;
			Hue = 0x0455;

			SetStr( 310 );
			SetDex( 100 );
			SetInt( 150 );

			SetHits( 310 );
			SetMana( 150 );

			SetDamage( 6, 36 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 10, 30 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.EvalInt, 75.0 );
			SetSkill( SkillName.Magery, 85.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 140.0 );
			SetSkill( SkillName.Wrestling, 200.0 );

			Fame = Utility.RandomMinMax( 1750, 2250 );
			Karma = Utility.RandomMinMax( -3500, -4500 );

			VirtualArmor = 30;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Gems, 5 );
		}

		public override int GetAttackSound()
		{
			return 0x195;//916;
		}
		/*
		public override int GetAngerSound()
		{
			return 916;
		}

		public override int GetDeathSound()
		{
			return 917;
		}

		public override int GetHurtSound()
		{
			return 919;
		}

		public override int GetIdleSound()
		{
			return 918;
		}*/

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 4; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Feathers { get { return 50; } }

		public WingedAvenger( Serial serial ) : base( serial )
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