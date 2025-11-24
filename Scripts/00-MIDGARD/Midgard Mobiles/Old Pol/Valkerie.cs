/*npctemplate                  valkerie
{
    name                     a valkerie
    script                   killpcs
    objtype                  0x1e
    color                    148
    truecolor                148
    gender                   0
    str                      160
    int                      140
    dex                      80
    hits                     160
    mana                     140
    stam                     80
    tactics                  90
    wrestling                95
    magicresistance          80
    attackspeed              30
    attackdamage             4d6
    attackskillid            wrestling
    attackhitsound           0x195
    attackmisssound          0x239
    ar                       3d6+3
    provoke                  80
    dstart                   10
    lootgroup                21
    powergroup               36
    karma                    -1600    -2000
    fame                     800     1000
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
	[CorpseName( "a valkerie corpse" )]
	public class Valkerie : BaseCreature
	{
		[Constructable]
		public Valkerie() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a valkerie";
			Body = 0x01e;
			BaseSoundID = 402;
			Hue = 148;

			SetStr( 160 );
			SetDex( 80 );
			SetInt( 140 );

			SetHits( 160 );
			SetStam( 80 );
			SetMana( 140 );

			SetDamage( 4, 24 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 10, 30 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.MagicResist, 80.0 );
			SetSkill( SkillName.Tactics, 90.0 );
			SetSkill( SkillName.Wrestling, 95.0 );

			Fame = Utility.RandomMinMax( 800, 1000 );
			Karma = Utility.RandomMinMax( -1600, -2000 );

			VirtualArmor = Utility.RandomMinMax( 6, 21 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager, 2 );
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

		public Valkerie( Serial serial ) : base( serial )
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