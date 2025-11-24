/*npctemplate                  alligator
{
    name                     an alligator
    script                   killpcs
    objtype                  0xca
    color                    33784
    truecolor                33784
    gender                   0
    str                      175
    int                      40
    dex                      70
    hits                     175
    mana                     40
    stam                     70
    tactics                  100
    wrestling                130
    magicresistance          80
    parry                    55
    attackspeed              25
    attackdamage             2d7
    attackskillid            wrestling
    attackhitsound           0x5e
    attackmisssound          0x239
    ar                       12
    movemode                 LS
    tameskill                60
    food                     meat
    hostile                  1
    provoke                  50
    dstart                   10
    lootgroup                1
    powergroup               25
    karma                    -800    -1000
    fame                     400     500
    attackhitscript          :combat:npchitscript
    vendorsellsfor           200
    vendorbuysfor            140
}*/
using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "an alligator corpse" )]
	public class OldAlligator : BaseCreature
	{
		[Constructable]
		public OldAlligator() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an alligator";
			Body = 0xCA;
			BaseSoundID = 660;
			Hue = 33784;

			SetStr( 175 );
			SetDex( 70 );
			SetInt( 40 );

			SetHits( 175 );
			SetStam( 70 );
			SetMana( 40 );

			SetDamage( 2, 14 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Poison, 5, 10 );

			SetSkill( SkillName.MagicResist, 80.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 130.0 );

			Fame = Utility.RandomMinMax( 400, 500 );
			Karma = Utility.RandomMinMax( -800, -1000 );

			VirtualArmor = 12;

			//Tamable = true;
			//ControlSlots = 1;
			//MinTameSkill = 60.1;
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 12; } }
		public override HideType HideType{ get{ return HideType.Spined; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public OldAlligator(Serial serial) : base(serial)
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

			if ( BaseSoundID == 0x5A )
				BaseSoundID = 660;
		}
	}
}