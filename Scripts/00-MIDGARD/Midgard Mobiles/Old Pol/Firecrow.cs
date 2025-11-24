/*npctemplate                  firecrow
{
    name                     a firecrow
    script                   firebreather
    magicresistance          20
    objtype                  0x06
    color                    0x0455
    truecolor                0x0455
    gender                   0
    str                      24
    int                      15
    dex                      60
    hits                     24
    mana                     15
    stam                     60
    tactics                  50
    wrestling                10
    attackspeed              20
    attackdamage             1d6
    attackskillid            wrestling
    attackhitsound           0x7f
    attackmisssound          0x7e
    cast_pct                 40
    num_casts                8
    ar                       12
    tameskill                15
    food                     meat
    guardignore              1
    provoke                  10
    dstart                   10
    lootgroup                1
    aspell                   7
    dspell                   9
    powergroup               6
    karma                    -200    -400
    fame                     100     200
    attackhitscript          :combat:npchitscript
    vendorsellsfor           50
    vendorbuysfor            35
}*/
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a fire crow corpse" )]
	public class FireCrow : BaseCreature
	{
		[Constructable]
		public FireCrow() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a firecrow";
			Body = 0x06;
			BaseSoundID = 0x1B;
			Hue = 1652;//0x0455;

			SetStr( 20, 30 );
			SetDex( 56, 65 );
			SetInt( 10, 20 );

			SetDamage( 1, 6 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetSkill( SkillName.MagicResist, 24.1, 26.0 );
			SetSkill( SkillName.Tactics, 48.6, 52.0 );
			SetSkill( SkillName.Wrestling, 10.1, 15.0 );

			Fame = Utility.RandomMinMax( 100, 200 );
			Karma = Utility.RandomMinMax( -200, -400 );

			VirtualArmor = Utility.RandomMinMax( 0, 6 );

			Tamable = false;
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int Meat{ get{ return 1; } }
		public override int Feathers { get { return (int)( 25 * FeatherScalar ); } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public FireCrow( Serial serial ) : base( serial )
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
