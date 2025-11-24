/*npctemplate                  deadlizard
{
    name                     an undead lizard
    script                   spellkillpcs
    objtype                  0x21
    color                    0x4631
    truecolor                0x4631
    gender                   0
    str                      280
    int                      175
    dex                      75
    hits                     280
    mana                     175
    stam                     75
    magicresistance          100
    tactics                  70
    macefighting             125
    evaluatingintelligence   75
    magery                   90
    attackspeed              30
    attackdamage             3d10+5
    attackskillid            macefighting
    attackhitsound           0x19f
    attackmisssound          0x239
    cprop                    undead	i7
    ar                       28
    cast_pct                 42
    num_casts                8
    tameskill                90
    food                     meat
    hostile                  1
    speech                   35
    dstart                   10
    saywords                 1
    lootgroup                32
    aspell                   15
    dspell                   9
    powergroup               53
    karma                    -3000    -3500
    fame                     1500     1750
    attackhitscript          :combat:npchitscript
    vendorsellsfor           10000
    vendorbuysfor            7000
}*/
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an undead lizardman corpse" )]
	public class UndeadLizard : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Lizardman; } }

		[Constructable]
		public UndeadLizard() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an undead lizard";//NameList.RandomName( "lizardman" );
			Body = 0x21;
			BaseSoundID = 417;
			Hue = 0x4631;

			SetStr( 280 );
			SetDex( 75 );
			SetInt( 175 );

			SetHits( 280 );
			SetMana( 175 );
			SetStam( 75 );

			SetDamage( 8, 35 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Cold, 5, 10 );
			SetResistance( ResistanceType.Poison, 10, 20 );

			SetSkill( SkillName.EvalInt, 75.0 );
			SetSkill( SkillName.Magery, 90.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 70.0 );
			SetSkill( SkillName.Wrestling, 125.0 );

			Fame = Utility.RandomMinMax( 1500, 1750 );
			Karma = Utility.RandomMinMax( -3000, -3500 );

			VirtualArmor = 28;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.Average );
			// TODO: weapon
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		//public override int Hides{ get{ return 12; } }
		//public override HideType HideType{ get{ return HideType.Spined; } }

		public UndeadLizard( Serial serial ) : base( serial )
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