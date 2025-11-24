/*npctemplate                  whitewyrm
{
    name                     a white wyrm
    script                   spellkillpcs
    objtype                  0x3b
    color                    1176
    truecolor                1176
    gender                   0
    str                      2000
    int                      400
    dex                      200
    hits                     2000
    mana                     400
    stam                     200
    tactics                  120
    wrestling                140
    magicresistance          120
    magery                   130
    evaluatingintelligence   115
    attackspeed              35
    attackdamage             8d8
    attackskillid            wrestling
    attackhitsound           0x16b
    attackmisssound          0x239
    cast_pct                 40
    num_casts                8
    ar                       60
    tameskill                200
    hostile                  1
    provoke                  95
    movemode                 LS
    dstart                   10
    lootgroup                42
    aspell                   9
    dspell                   9
    magicgroups              10
    powergroup               81
    karma                    -9000    -9500
    fame                     4500     4750
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a white wyrm corpse" )]
	public class OldWhiteWyrm : BaseCreature
	{
		[Constructable]
		public OldWhiteWyrm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 0x3b;
			Name = "a white wyrm";
			BaseSoundID = 362;
			Hue = 1153;//1176;

			SetStr( 2000 );
			SetDex( 200 );
			SetInt( 400 );

			SetHits( 2000 );
			SetStam( 200 );
			SetMana( 400 );

			SetDamage( 8, 64 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 55, 70 );
			SetResistance( ResistanceType.Fire, 15, 25 );
			SetResistance( ResistanceType.Cold, 80, 90 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.EvalInt, 115.0 );
			SetSkill( SkillName.Magery, 130.0 );
			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 140.0 );

			Fame = Utility.RandomMinMax( 4500, 4750 );
			Karma = Utility.RandomMinMax( -9000, -9500 );

			VirtualArmor = 60;

			//Tamable = true;
			//ControlSlots = 3;
			//MinTameSkill = 96.3;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		

			if( !IsBonded )	
				c.DropItem( new WyrmHeart() );
		}

		public override bool AutoDispel{ get{ return !Controlled; } }
		public override bool ReacquireOnMovement{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 19; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Scales{ get{ return 9; } }
		public override ScaleType ScaleType{ get{ return ScaleType.White; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Gold; } }
		public override bool CanAngerOnTame { get { return true; } }

		public OldWhiteWyrm( Serial serial ) : base( serial )
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

			if ( Core.AOS && Body == 49 )
				Body = 180;
		}
	}
}