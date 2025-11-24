/*npctemplate                  bat
{
    name                     a bat
    noloot                   1
    script                   spellkillpcs
    magicresistance          20
    objtype                  0x06
    color                    5555
    truecolor                5555
    gender                   0
    str                      10
    int                      100
    dex                      70
    hits                     10
    mana                     100
    stam                     70
    tactics                  70
    wrestling                70
    magery                   20
    cast_pct                 20
    num_casts                2
    attackspeed              45
    attackdamage             3d3
    attackskillid            wrestling
    ar                       10
    tameskill                110
    food                     veggie
    guardignore              1
    provoke                  10
    dstart                   10
    lootgroup                15
    aspell                   7
    dspell                   9
    powergroup               15
    karma                    -400    -600
    fame                     200     300
    attackhitscript          :combat:npchitscript
    vendorsellsfor           30
    vendorbuysfor            21
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a bat corpse" )]
	public class Bat : BaseCreature
	{
		[Constructable]
		public Bat() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a bat";
			Body = 317;
			BaseSoundID = 0x270;
			Hue = 2501;//5555;

			SetStr( 10 );
			SetDex( 70 );
			SetInt( 100 );

			SetHits( 10 );
			SetStam( 70 );
			SetMana( 100 );

			SetDamage( 5, 10 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 15 );
			SetResistance( ResistanceType.Fire, 10, 15 );
			SetResistance( ResistanceType.Cold, 10, 15 );
			SetResistance( ResistanceType.Poison, 10, 15 );
			SetResistance( ResistanceType.Energy, 10, 15 );

			SetSkill( SkillName.Magery, 20.0 );
			SetSkill( SkillName.Tactics, 70.0 );
			SetSkill( SkillName.Wrestling, 70.0 );

			Fame = Utility.RandomMinMax( 200, 300 );
			Karma = Utility.RandomMinMax( -400, -600 );

			VirtualArmor = 10;

			//ControlSlots = 1;
		}

		public Bat( Serial serial ) : base( serial )
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