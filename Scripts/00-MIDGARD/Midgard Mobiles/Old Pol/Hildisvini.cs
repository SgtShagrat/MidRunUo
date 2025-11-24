/*npctemplate                  hildisvini
{
    name                     hildisvini
    noloot                   1
    script                   killpcs
    objtype                  0xcb
    color                    448
    truecolor                448
    gender                   0
    str                      60
    int                      45
    dex                      80
    hits                     60
    mana                     45
    stam                     80
    tactics                  60
    wrestling                65
    magicresistance          75
    attackspeed              30
    attackdamage             3d3+3
    attackskillid            wrestling
    attackhitsound           0xc7
    attackmisssound          0x239
    ar                       10
    tameskill                70
    food                     meat
    guardignore              1
    provoke                  60
    dstart                   10
    lootgroup                15
    powergroup               15
    karma                    -400    -600
    fame                     200     300
    attackhitscript          :combat:npchitscript
    vendorsellsfor           200
    vendorbuysfor            140
}*/
using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a pig corpse" )]
	public class Hildisvini : BaseCreature
	{
		[Constructable]
		public Hildisvini() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "hildisvini";
			Body = 0xCB;
			BaseSoundID = 0xC4;
			Hue = 448;

			SetStr( 60 );
			SetDex( 80 );
			SetInt( 45 );

			SetHits( 60 );
			SetMana( 45 );
			SetStam( 80 );

			SetDamage( 6, 12 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 15 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Poison, 5, 10 );

			SetSkill( SkillName.MagicResist, 75.0 );
			SetSkill( SkillName.Tactics, 60.0 );
			SetSkill( SkillName.Wrestling, 65.0 );

			Fame = Utility.RandomMinMax( 200, 300 );
			Karma = Utility.RandomMinMax( -400, -600 );

			VirtualArmor = 10;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 70.1;
		}

		public override int Meat{ get{ return 2; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Hildisvini(Serial serial) : base(serial)
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