/*npctemplate                  lizardmanmarksman
{
    name                     <random>
    script                   archerkillpcs
    objtype                  0x24
    color                    0
    truecolor                0
    gender                   0
    str                      170
    int                      70
    dex                      95
    hits                     170
    mana                     70
    stam                     95
    magicresistance          80
    tactics                  75
    macefighting             75
    archery                  90
    attackspeed              30
    attackdamage             6d3
    attackskillid            archery
    attackhitsound           0x13c
    attackmisssound          0x239
    ar                       3d4+2
    ammotype                 0xf3f
    missileweapon            archer
    ammoamount               30
    provoke                  70
    dstart                   10
    lootgroup                21
    powergroup               31
    karma                    -1300    -1600
    fame                     650     800
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a lizardman corpse" )]
	public class LizardmanMarksman : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Lizardman; } }

		[Constructable]
		public LizardmanMarksman() : base( AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "lizardman" );
			Body = 0x24;
			BaseSoundID = 417;

			SetStr( 170 );
			SetDex( 95 );
			SetInt( 70 );

			SetHits( 170 );
			SetStam( 95 );
			SetMana( 70 );

			SetDamage( 6, 18 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Cold, 5, 10 );
			SetResistance( ResistanceType.Poison, 10, 20 );

			SetSkill( SkillName.MagicResist, 80.0 );
			SetSkill( SkillName.Tactics, 75.0 );
			SetSkill( SkillName.Archery, 90.0 );

			Fame = Utility.RandomMinMax( 650, 800 );
			Karma = Utility.RandomMinMax( -1300, -1600 );

			VirtualArmor = Utility.RandomMinMax( 5, 14 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
			// TODO: weapon
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 12; } }
		public override HideType HideType{ get{ return HideType.Spined; } }

		public LizardmanMarksman( Serial serial ) : base( serial )
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