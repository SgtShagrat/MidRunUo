/*npctemplate                  snoworclord
{
    name                     <random> the orcish lord
    script                   killpcs
    objtype                  7
    color                    1154
    truecolor                1154
    gender                   0
    str                      195
    int                      40
    dex                      110
    hits                     195
    mana                     40
    stam                     110
    swordsmanship            100
    tactics                  95
    magicresistance          85
    attackspeed              35
    attackdamage             4d5+6
    attackskillid            swordsmanship
    attackhitsound           0x23d
    attackmisssound          0x239
    ar                       30
    provoke                  78
    dstart                   10
    lootgroup                24
    powergroup               32
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
	[CorpseName( "an orcish corpse" )]
	public class SnowOrcLord : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
		public SnowOrcLord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "orc" ) + " the orcish lord";
			Body = 7;
			BaseSoundID = 0x45A;
			Hue = 1154;

			SetStr( 195 );
			SetDex( 110 );
			SetInt( 40 );

			SetHits( 195 );
			SetStam( 110 );
			SetMana( 40 );

			SetDamage( 10, 26 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 85.0 );
			SetSkill( SkillName.Tactics, 95.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			Fame = Utility.RandomMinMax( 650, 800 );
			Karma = Utility.RandomMinMax( -1300, -1600 );

			VirtualArmor = 30;

			switch ( Utility.Random( 20 ) )
			{
				case 0: PackItem( new Scimitar() ); break;
				case 1: PackItem( new Katana() ); break;
				case 2: PackItem( new WarMace() ); break;
				case 3: PackItem( new WarHammer() ); break;
				case 4: PackItem( new Kryss() ); break;
				case 5: PackItem( new Pitchfork() ); break;
			}

			PackItem( new ThighBoots() );

			switch ( Utility.Random( 3 ) )
			{
				case 0: PackItem( new Ribs() ); break;
				case 1: PackItem( new Shaft() ); break;
				case 2: PackItem( new Candle() ); break;
			}

			if ( 0.2 > Utility.RandomDouble() )
				PackItem( new BolaBall() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 4; } }
		public override HideType HideType{ get{ return HideType.Spined; } }

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

		public SnowOrcLord( Serial serial ) : base( serial )
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
