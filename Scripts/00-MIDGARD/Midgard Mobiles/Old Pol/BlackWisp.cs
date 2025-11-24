/*npctemplate                  blackwisp
{
    name                     a black wisp
    script                   spellkillpcs
    noloot                   1
    objtype                  0x3a
    color                    0x0455
    truecolor                0x0455
    gender                   0
    str                      350
    int                      250
    dex                      175
    hits                     350
    mana                     250
    stam                     175
    tactics                  100
    evaluatingintelligence   120
    magery                   130
    wrestling                100
    magicresistance          100
    attackspeed              35
    attackdamage             3d12
    attackskillid            wrestling
    attackhitsound           0x1d5
    attackmisssound          0x239
    ar                       30
    cast_pct                 40
    num_casts                8
    hostile                  1
    speech                   7
    provoke                  95
    dstart                   10
    lootgroup                46
    aspell                   21
    dspell                   9
    powergroup               72
    karma                    -6500    -7500
    fame                     3250     3750
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName( "a wisp corpse" )]
	public class BlackWisp : BaseCreature
	{
		[Constructable]
		public BlackWisp() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a black wisp";
			Body = 0x3a;
			BaseSoundID = 466;
			Hue = 0x0455;

			SetStr( 350 );
			SetDex( 175 );
			SetInt( 250 );

			SetHits( 350 );
			SetMana( 250 );
			SetStam( 175 );

			SetDamage( 3, 36 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 20, 40 );
			SetResistance( ResistanceType.Cold, 10, 30 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 50, 70 );

			SetSkill( SkillName.EvalInt, 120.0 );
			SetSkill( SkillName.Magery, 130.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			Fame = Utility.RandomMinMax( 3250, 3750 );
			Karma = Utility.RandomMinMax( -6500, -7500 );

			VirtualArmor = 30;

			AddItem( new LightSource() );
		}

		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Wisp; } }
		public override Ethics.Ethic EthicAllegiance { get { return Ethics.Ethic.Evil; } }
		public override TimeSpan ReacquireDelay { get { return TimeSpan.FromSeconds( 1.0 ); } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.FilthyRich );
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public BlackWisp( Serial serial ) : base( serial )
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