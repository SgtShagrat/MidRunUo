/*npctemplate                  raptor
{
    name                     a raptor
    noloot                   1
    script                   killpcs
    objtype                  0xd2
    color                    0x0455
    truecolor                0x0455
    gender                   0
    str                      200
    int                      30
    dex                      150
    hits                     200
    mana                     30
    stam                     150
    magicresistance          60
    tactics                  100
    wrestling                120
    attackspeed              50
    attackdamage             3d8
    attackskillid            wrestling
    attackhitsound           0xbe
    attackmisssound          0x239
    ar                       20
    tameskill                115
    food                     meat
    provoke                  30
    dstart                   10
    lootgroup                1
    powergroup               36
    karma                    -1600    -2000
    fame                     800     1000
    attackhitscript          :combat:npchitscript
    vendorsellsfor           50000
    vendorbuysfor            35000
}*/
using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
	[CorpseName( "an raptor corpse" )]
	public class OldRaptor : BaseCreature
	{
		[Constructable]
		public OldRaptor() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a raptor";
			Body = 0xD2;
			//BaseSoundID = 422;
			Hue = 0x0455;

			BaseSoundID = 0x275;

			SetStr( 200 );
			SetDex( 150 );
			SetInt( 30 );

			SetHits( 200 );
			SetMana( 30 );
			SetStam( 150 );

			SetDamage( 3, 24 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30, 45 );
			SetResistance( ResistanceType.Fire, 15, 20 );
			SetResistance( ResistanceType.Poison, 25, 30 );
			SetResistance( ResistanceType.Energy, 25, 30 );

			SetSkill( SkillName.MagicResist, 60.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 120.0 );

			Fame = Utility.RandomMinMax( 800, 1000 );
			Karma = Utility.RandomMinMax( -1600, -2000 );

			VirtualArmor = 20;

			//Tamable = true;
			//ControlSlots = 1;
			//MinTameSkill = 29.1;
		}

		public override int Meat { get { return 5; } }
		public override FoodType FavoriteFood { get { return FoodType.Meat; } }
		public override PackInstinct PackInstinct { get { return PackInstinct.Ostard; } }
		public override bool SubdueBeforeTame { get { return false; } } // Must be beaten into submission

		public OldRaptor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}