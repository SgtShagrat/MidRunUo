/*npctemplate                  swampboar
{
    name                     a swamp boar
    script                   killpcs
    objtype                  0xcb
    color                    0x07d6
    truecolor                0x07d6
    gender                   0
    str                      100
    int                      30
    dex                      40
    hits                     100
    mana                     30
    stam                     40
    magicresistance          40
    tactics                  50
    wrestling                80
    attackspeed              30
    attackdamage             3d3
    attackskillid            wrestling
    attackhitsound           0xc9
    attackmisssound          0x239
    ar                       6
    tameskill                90
    food                     meat
    guardignore              1
    provoke                  20
    dstart                   10
    lootgroup                1
    powergroup               13
    karma                    -400    -600
    fame                     200     300
    attackhitscript          :combat:npchitscript
    vendorsellsfor           300
    vendorbuysfor            210
}*/
using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a pig corpse" )]
	public class SwampBoar : BaseCreature
	{
		[Constructable]
		public SwampBoar() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a swamp boar";
			Body = 0xCB;
			BaseSoundID = 0xC4;
			Hue = 0x07d6;

			SetStr( 100 );
			SetDex( 40 );
			SetInt( 30 );

			SetHits( 100 );
			SetMana( 30 );
			SetStam( 40 );

			SetDamage( 3, 9 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 15 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Poison, 5, 10 );

			SetSkill( SkillName.MagicResist, 40.0 );
			SetSkill( SkillName.Tactics, 50.0 );
			SetSkill( SkillName.Wrestling, 80.0 );

			Fame = Utility.RandomMinMax( 200, 300 );
			Karma = Utility.RandomMinMax( -400, -600 );

			VirtualArmor = 10;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 90.1;
		}

		public override int Meat{ get{ return 2; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public SwampBoar(Serial serial) : base(serial)
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