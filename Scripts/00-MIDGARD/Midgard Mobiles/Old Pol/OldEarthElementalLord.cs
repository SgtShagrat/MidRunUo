/*npctemplate                  earthelementallord
{
    name                     an earth elemental lord
    script                   killpcs
    objtype                  0x0e
    color                    1538
    truecolor                1538
    gender                   0
    str                      400
    int                      90
    dex                      200
    hits                     400
    mana                     90
    stam                     200
    tactics                  150
    wrestling                175
    attackspeed              35
    attackdamage             5d5
    attackskillid            wrestling
    attackhitsound           0x10f
    attackmisssound          0x239
    ar                       30
    lootgroup                37
    magicgroups              4
    powergroup               67
    karma                    -5500    -6500
    fame                     2750     3250
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an earth elemental corpse" )]
	public class OldEarthElementalLord : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public OldEarthElementalLord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an earth elemental lord";
			Body = 0x0e;
			BaseSoundID = 268;
			Hue = 1538;

			SetStr( 400 );
			SetDex( 200 );
			SetInt( 90 );

			SetHits( 400 );
			SetMana( 90 );
			SetStam( 200 );

			SetDamage( 5, 25 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 15, 25 );
			SetResistance( ResistanceType.Energy, 15, 25 );

			SetSkill( SkillName.MagicResist, 50.1, 95.0 );
			SetSkill( SkillName.Tactics, 150.0 );
			SetSkill( SkillName.Wrestling, 175.0 );

			Fame = Utility.RandomMinMax( 2750, 3250 );
			Karma = Utility.RandomMinMax( -5500, -6500 );

			VirtualArmor = 30;
			//ControlSlots = 2;

			PackItem( new FertileDirt( Utility.RandomMinMax( 1, 4 ) ) );
			PackItem( new IronOre( 3 ) ); // TODO: Five small iron ore
			PackItem( new MandrakeRoot( 4 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems );
			AddLoot( LootPack.Potions );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }

		public OldEarthElementalLord( Serial serial ) : base( serial )
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