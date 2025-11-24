/*npctemplate                  stonegargoyle
{
    name                     a stone gargoyle
    script                   spellkillpcs
    objtype                  0x4
    color                    1154
    truecolor                1154
    gender                   0
    str                      195
    int                      150
    dex                      70
    hits                     195
    mana                     150
    stam                     70
    tactics                  70
    wrestling                135
    magery                   80
    evaluatingintelligence   75
    cast_pct                 33
    num_casts                6
    magicresistance          100
    attackspeed              30
    attackdamage             6d6
    attackskillid            wrestling
    attackhitsound           0x177
    attackmisssound          0x239
    ar                       4d5+2
    hostile                  1
    speech                   54
    provoke                  85
    dstart                   10
    lootgroup                1
    aspell                   15
    dspell                   9
    powergroup               40
    karma                    -1600    -2000
    fame                     800     1000
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
} */
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a charred corpse" )]
	public class OldStoneGargoyle : BaseCreature
	{
		[Constructable]
		public OldStoneGargoyle() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "fire gargoyle" );
			Body = 130;
			BaseSoundID = 0x174;
			Hue = 1154;

			SetStr( 195 );
			SetDex( 70 );
			SetInt( 150 );

			SetHits( 195 );
			SetStam( 70 );
			SetMana( 150 );

			SetDamage( 6, 36 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.EvalInt, 75.0 );
			SetSkill( SkillName.Magery, 80.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 70.0 );
			SetSkill( SkillName.Wrestling, 135.0 );

			Fame = Utility.RandomMinMax( 800, 1000 );
			Karma = Utility.RandomMinMax( -1600, -2000 );

			VirtualArmor = Utility.RandomMinMax( 6, 22 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }

		public OldStoneGargoyle( Serial serial ) : base( serial )
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