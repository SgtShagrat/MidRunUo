/*npctemplate                  waterelementallord
{
    name                     a water elemental lord
    script                   spellkillpcs
    objtype                  0x10
    color                    0
    truecolor                0
    gender                   0
    str                      400
    int                      420
    dex                      200
    hits                     400
    mana                     420
    stam                     200
    tactics                  150
    wrestling                160
    magicresistance          100
    evaluatingintelligence   100
    magery                   100
    movemode                 LS
    attackspeed              40
    attackdamage             5d5
    attackskillid            wrestling
    attackhitsound           0x119
    attackmisssound          0x239
    ar                       30
    cast_pct                 35
    num_casts                8
    aspell                   9
    dspell                   9
    cprop                    nocorpse i1
    lootgroup                38
    magicgroups              4
    powergroup               78
    karma                    -8000    -8500
    fame                     4000     4250
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a water elemental corpse" )]
	public class WaterElementalLord : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public WaterElementalLord () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a water elemental lord";
			Body = 0x10;
			BaseSoundID = 278;

			SetStr( 400 );
			SetDex( 200 );
			SetInt( 420 );

			SetHits( 400 );
			SetStam( 200 );
			SetMana( 420 );

			SetDamage( 5, 25 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 10, 25 );
			SetResistance( ResistanceType.Cold, 10, 25 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 5, 10 );

			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 150.0 );
			SetSkill( SkillName.Wrestling, 160.0 );

			Fame = Utility.RandomMinMax( 4000, 4250 );
			Karma = Utility.RandomMinMax( -8000, -8500 );

			VirtualArmor = 30;
			//ControlSlots = 3;
			CanSwim = true;

			PackItem( new BlackPearl( 10 ) );
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
		public override int TreasureMapLevel{ get{ return 4; } }

		public WaterElementalLord( Serial serial ) : base( serial )
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