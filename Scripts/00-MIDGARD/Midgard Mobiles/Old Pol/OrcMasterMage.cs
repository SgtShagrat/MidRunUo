/*npctemplate                  orcmastermage
{
    name                     <random> the orc master mage
    script                   spellkillpcs
    objtype                  0x11
    color                    0x0612
    truecolor                0x06122
    gender                   0
    str                      255
    int                      175
    dex                      80
    hits                     255
    mana                     175
    stam                     80
    magicresistance          100
    tactics                  50
    wrestling                75
    evaluatingintelligence   120
    magery                   100
    cast_pct                 50
    num_casts                10
    attackspeed              30
    attackdamage             2d20
    attackskillid            wrestling
    attackhitsound           0x1b3
    attackmisssound          0x239
    ar                       40
    hostile                  1
    speech                   6
    provoke                  115
    saywords                 1
    dstart                   10
    lootgroup                33
    aspell                   8
    dspell                   9
    powergroup               51
    karma                    -3000    -3500
    fame                     1500     1750
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a glowing orc corpse" )]
	public class OrcMasterMage : BaseCreature, IOrc
	{
		[Constructable]
		public OrcMasterMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a orc master mage";
			Body = 0x11;
			BaseSoundID = 0x45A;
			Hue = 0x0612;

			SetStr( 255 );
			SetDex( 80 );
			SetInt( 175 );

			SetHits( 255 );
			SetMana( 175 );
			SetStam( 80 );

			SetDamage( 2, 40 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 120.0 );
			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 50.0 );
			SetSkill( SkillName.Wrestling, 75.0 );

			Fame = Utility.RandomMinMax( 1500, 1750 );
			Karma = Utility.RandomMinMax( -3000, -3500 );

			VirtualArmor = 40;

			PackReg( 6 );

			if ( 0.05 > Utility.RandomDouble() )
				PackItem( new OrcishKinMask() );
		}
		public override bool CanSpeakMantra{get { return true; }}
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.LowScrolls );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.SavagesAndOrcs; }
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && m.FindItemOnLayer( Layer.Helm ) is OrcishKinMask )
				return false;

			return base.IsEnemy( m );
		}

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );

			Item item = aggressor.FindItemOnLayer( Layer.Helm );

			if ( item is OrcishKinMask )
			{
				AOS.Damage( aggressor, 50, 0, 100, 0, 0, 0 );
				item.Delete();
				aggressor.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
				aggressor.PlaySound( 0x307 );
			}
		}

		public OrcMasterMage( Serial serial ) : base( serial )
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
