/*npctemplate                  snoworc2
{
    name                     <random> the snoworc
    script                   killpcs
    objtype                  0x29
    color                    1154
    truecolor                1154
    gender                   0
    str                      140
    int                      50
    dex                      80
    hits                     140
    mana                     50
    stam                     80
    magicresistance          50
    macefighting             70
    wrestling                85
    tactics                  75
    attackspeed              30
    attackdamage             2d6+2
    attackskillid            macefighting
    attackhitsound           0x13c
    attackmisssound          0x239
    ar                       25
    hostile                  1
    provoke                  41
    dstart                   10
    lootgroup                16
    powergroup               24
    karma                    -800    -1000
    fame                     400     500
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
	public class SnowOrc : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
		public SnowOrc() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "orc" ) + " the snow orc";
			Body = 0x29;
			BaseSoundID = 0x45A;
			Hue = 1154;

			SetStr( 140 );
			SetDex( 80 );
			SetInt( 50 );

			SetHits( 140 );
			SetStam( 80 );
			SetMana( 50 );

			SetDamage( 4, 14 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 50.0 );
			SetSkill( SkillName.Tactics, 75.0 );
			SetSkill( SkillName.Wrestling, 70.0, 85.0 );

			Fame = Utility.RandomMinMax( 400, 500 );
			Karma = Utility.RandomMinMax( -800, -1000 );

			VirtualArmor = 25;

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
		public override int Hides{ get{ return 2; } }
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

		public SnowOrc( Serial serial ) : base( serial )
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
