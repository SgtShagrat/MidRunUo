/*npctemplate                  revenant
{
    name                     a revenant
    script                   killpcs
    objtype                  0x03
    color                    1160
    truecolor                1160
    gender                   0
    str                      600
    int                      30
    dex                      60
    hits                     600
    mana                     30
    stam                     60
    magicresistance          100
    tactics                  120
    wrestling                150
    attackspeed              20
    attackdamage             6d8
    attackskillid            wrestling
    attackhitsound           0x1d8
    attackmisssound          0x239
    ar                       35
    cprop                    undead	i10
    hostile                  1
    dstart                   10
    lootgroup                39
    powergroup               67
    karma                    -5500    -6500
    fame                     2750     3250
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a revenant corpse" )]
	public class OldRevenant : BaseCreature
	{
		[Constructable]
		public OldRevenant() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.18, 0.36 )
		{
			Name = "a revenant";
			Body = 0x03;
			Hue = 1921;//1160;
			// TODO: Sound values?

			SetStr( 600 );
			SetDex( 60 );
			SetInt( 30 );

			SetDamage( 6, 54 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 150.0 );

			SetResistance( ResistanceType.Physical, 40 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Fire, 20 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 40 );

			Fame = Utility.RandomMinMax( 2750, 3250 );
			Karma = Utility.RandomMinMax( -5500, -6500 );

			VirtualArmor = 35;

			PackNecroReg( 12, 40 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 4; } }

		public OldRevenant( Serial serial ) : base( serial )
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