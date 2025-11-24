/*npctemplate                  firelizard
{
    name                     a fire lizard
    script                   firebreather
    noloot                   1
    objtype                  0xca
    color                    0x075a
    truecolor                0x075a
    gender                   0
    str                      170
    int                      60
    dex                      84
    hits                     170
    mana                     60
    stam                     84
    magicresistance          100
    tactics                  100
    wrestling                130
    attackspeed              30
    attackdamage             4d7
    attackskillid            wrestling
    attackhitsound           0x28c
    attackmisssound          0x28a
    cast_pct                 40
    num_casts                8
    ar                       10
    movemode                 LS
    tameskill                100
    food                     meat
    provoke                  90
    dstart                   10
    lootgroup                23
    aspell                   7
    dspell                   9
    powergroup               29
    karma                    -1000    -1300
    fame                     500     650
    attackhitscript          :combat:npchitscript
    vendorsellsfor           3000
    vendorbuysfor            2100
}*/
using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a fire lizard corpse" )]
	public class FireLizard : BaseCreature
	{
		[Constructable]
		public FireLizard() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a fire lizard";
			Body = 0xCA;
			Hue = 1652;//0x75a;
			BaseSoundID = 0x5A;

			SetStr( 170 );
			SetDex( 84 );
			SetInt( 60 );

			SetHits( 170 );
			SetMana( 60 );

			SetDamage( 4, 28 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 30, 45 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 130.0 );

			Fame = Utility.RandomMinMax( 500, 650 );
			Karma = Utility.RandomMinMax( -1000, -1300 );

			VirtualArmor = 10;

			//Tamable = true;
			//ControlSlots = 1;
			//MinTameSkill = 80.7;

			PackItem( new SulfurousAsh( Utility.Random( 4, 10 ) ) );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		

			if( !IsBonded )	
				c.DropItem( new EyeOfNewt() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int Hides{ get{ return 12; } }
		public override HideType HideType{ get{ return HideType.Spined; } }

		public FireLizard(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}