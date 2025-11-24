/*npctemplate                  frostwolf
{
    name                     a frost wolf
    script                   killpcs
    objtype                  0xe1
    color                    0x0482
    truecolor                0x0482
    gender                   0
    str                      200
    int                      50
    dex                      100
    hits                     200
    mana                     50
    stam                     100
    magicresistance          90
    tactics                  100
    wrestling                100
    attackspeed              35
    attackdamage             4d4+4
    attackskillid            wrestling
    attackhitsound           0xe8
    attackmisssound          0x239
    ar                       20
    tameskill                90
    food                     meat
    hostile                  1
    provoke                  80
    dstart                   10
    lootgroup                1
    powergroup               33
    karma                    -1300    -1600
    fame                     650     800
    attackhitscript          :combat:npchitscript
    vendorsellsfor           800
    vendorbuysfor            560
}*/
using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a frost wolf corpse" )]
	public class FrostWolf : BaseCreature
	{
		[Constructable]
		public FrostWolf() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a frost wolf";
			Body = 0xe1;
			BaseSoundID = 0xE5;
			Hue = 0x0482;

			SetStr( 200 );
			SetDex( 100 );
			SetInt( 50 );

			SetHits( 200 );
			SetMana( 100 );

			SetDamage( 8, 20 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 20 );
			SetResistance( ResistanceType.Fire, 10, 15 );
			SetResistance( ResistanceType.Cold, 20, 25 );
			SetResistance( ResistanceType.Poison, 10, 15 );
			SetResistance( ResistanceType.Energy, 10, 15 );

			SetSkill( SkillName.MagicResist, 90.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			Fame = Utility.RandomMinMax( 650, 800 );
			Karma = Utility.RandomMinMax( -1300, -1600 );

			VirtualArmor = 20;

			//Tamable = true;
			//ControlSlots = 1;
			//MinTameSkill = 90.1;
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 6; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

		public FrostWolf(Serial serial) : base(serial)
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