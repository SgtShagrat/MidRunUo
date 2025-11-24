/*npctemplate                  icefiend
{
    name                     <random> the ice fiend
    script                   spellkillpcs
    objtype                  0x09
    color                    1152
    truecolor                1152
    gender                   0
    str                      380
    int                      250
    dex                      100
    hits                     380
    mana                     250
    stam                     100
    magicresistance          100
    tactics                  125
    wrestling                175
    magery                   110
    evaluatingintelligence   110
    attackspeed              35
    attackdamage             3d6+3
    attackskillid            wrestling
    attackhitsound           0x168
    attackmisssound          0x239
    ar                       35
    hostile                  1
    dstart                   10
    saywords                 1
    cprop                    undead i10
    cast_pct                 35
    num_casts                8
    aspell                   9
    dspell                   9
    provoke                  110
    lootgroup                39
    magicgroups              4
    powergroup               70
    karma                    -5500    -6500
    fame                     2750     3250
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ice fiend corpse" )]
	public class OldIceFiend : BaseCreature
	{
		[Constructable]
		public OldIceFiend () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ice fiend";
			Body = 0x09;
			BaseSoundID = 357;
			Hue = 1152;

			SetStr( 380 );
			SetDex( 100 );
			SetInt( 250 );

			SetHits( 380 );
			SetStam( 100 );
			SetMana( 250 );

			SetDamage( 6, 21 );

			SetSkill( SkillName.EvalInt, 110.0 );
			SetSkill( SkillName.Magery, 110.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 125.0 );
			SetSkill( SkillName.Wrestling, 175.0 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			Fame = Utility.RandomMinMax( 2750, 3250 );
			Karma = Utility.RandomMinMax( -5500, -6500 );

			VirtualArmor = 35;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int Hides{ get{ return 15; } }
		public override HideType HideType{ get{ return HideType.Spined; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 1; } }

		public OldIceFiend( Serial serial ) : base( serial )
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