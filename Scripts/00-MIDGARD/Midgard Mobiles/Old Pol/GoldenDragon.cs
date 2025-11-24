/*npctemplate                  goldendragon
{
    name                     a golden dragon
    script                   firebreather
    objtype                  0xc
    color                    48
    truecolor                48
    gender                   0

    str                      1500
    int                      500
    dex                      175
    hits                     1500
    mana                     500
    stam                     175

    tactics                  135
    wrestling                150
    magery                   150
    magicresistance          125
    evaluatingintelligence   120
    cast_pct                 40
    num_casts                8
    attackspeed              50

    attackdamage             10d6
    attackskillid            wrestling
    attackhitsound           0x16b
    attackmisssound          0x239
    tameskill                200
    ar                       45
    alignment                good
    movemode                 LS
    hostile                  1
    provoke                  99
    dstart                   10
    lootgroup                44
    aspell                   9
    dspell                   9
    magicgroups              11
    powergroup               83

    karma                    10000     10000
    fame                     5000     10000
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a golden dragon corpse" )]
	public class GoldenDragon : BaseCreature
	{
		[Constructable]
		public GoldenDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a golden dragon";
			Body = 0xc;
			BaseSoundID = 362;
			Hue = 48;

			SetStr( 1500 );
			SetDex( 175 );
			SetInt( 500 );

			SetHits( 1500 );
			SetMana( 500 );

			SetDamage( 10, 60 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 35, 45 );

			SetSkill( SkillName.EvalInt, 120.0 );
			SetSkill( SkillName.Magery, 150.0 );
			SetSkill( SkillName.MagicResist, 125.0 );
			SetSkill( SkillName.Tactics, 135.0 );
			SetSkill( SkillName.Wrestling, 150.0 );

			Fame = Utility.RandomMinMax( 5000, 10000 );
			Karma = 10000;

			VirtualArmor = 45;

			//Tamable = true;
			//ControlSlots = 3;
			//MinTameSkill = 93.9;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Gems, 8 );
		}

		//public override bool StatLossAfterTame { get { return true; } } // mod by Dies Irae
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return !Controlled; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 19; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Scales{ get{ return 7; } }
		public override ScaleType ScaleType{ get{ return ( ScaleType.Yellow ); } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		//public override bool CanAngerOnTame { get { return true; } }

		public GoldenDragon( Serial serial ) : base( serial )
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