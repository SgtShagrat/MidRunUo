/*npctemplate                  sandworm
{
    name                     a giant sand worm
    script                   killpcs
    objtype                  0x96
    color                    351
    truecolor                351
    gender                   0
    str                      300
    dex                      200
    int                      40
    hits                     300
    stam                     200
    mana                     40
    hiding                   200
    stealth                  200
    tactics                  80
    wrestling                150
    tameskill                115
    food                     meat
    magicresistance          80
    attackspeed              35
    attackdamage             3d10
    attackskillid            wrestling
    ar                       30
    attackhitsound           0x1c0
    attackmisssound          0x239
    provoke                  90
    dstart                   10
    lootgroup                28
    powergroup               54
    karma                    -3000    -3500
    fame                     1500     1750
    attackhitscript          :combat:npchitscript
    vendorsellsfor           10000
    vendorbuysfor            7000
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a giant sand worm corpse" )]
	public class SandWorm : BaseCreature
	{
		[Constructable]
		public SandWorm() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a giant sand worm";
			Body = 0x96;
			BaseSoundID = 447;

			Hue = 351;

			SetStr( 300 );
			SetDex( 200 );
			SetInt( 40 );

			SetHits( 300 );

			SetDamage( 3, 30 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30, 40 );
			SetResistance( ResistanceType.Fire, 70, 80 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 15, 20 );

			SetSkill( SkillName.MagicResist, 80.0 );
			SetSkill( SkillName.Tactics, 80.0 );
			SetSkill( SkillName.Wrestling, 150.0 );

			Fame = Utility.RandomMinMax( 1500, 1750 );
			Karma = Utility.RandomMinMax( -3000, -3500 );

			VirtualArmor = 30;
			//CanSwim = true;
			//CantWalk = true;

			if ( Utility.RandomBool() )
			PackItem( new SulfurousAsh( 4 ) );
			else
			PackItem( new BlackPearl( 4 ) );

			//PackItem( new SpecialFishingNet() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override bool HasBreath{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		public override int Scales{ get{ return 6; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Yellow; }}

		public SandWorm( Serial serial ) : base( serial )
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