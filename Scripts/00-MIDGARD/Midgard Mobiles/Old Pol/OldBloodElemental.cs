/*npctemplate                  bloodelemental
{
    name                     a blood elemental
    script                   spellkillpcs
    objtype                  0x10
    color                    0x04b9
    truecolor                0x04b9
    gender                   0
    str                      300
    int                      220
    dex                      175
    hits                     300
    mana                     220
    stam                     175
    tactics                  100
    wrestling                200
    magicresistance          100
    evaluatingintelligence   100
    magery                   100
    attackspeed              35
    attackdamage             25d2
    attackskillid            wrestling
    attackhitsound           0x1d5
    attackmisssound          0x239
    ar                       40
    hostile                  1
    speech                   7
    provoke                  95
    dstart                   10
    cast_pct                 35
    num_casts                8
    aspell                   9
    dspell                   9
    lootgroup                35
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

namespace Server.Mobiles
{
	[CorpseName( "a blood elemental corpse" )]
	public class OldBloodElemental : BaseCreature
	{
		[Constructable]
		public OldBloodElemental() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a blood elemental";
			Body = 0x10;
			BaseSoundID = 278;
			Hue = 0x04b9;

			SetStr( 300 );
			SetDex( 175 );
			SetInt( 220 );

			SetHits( 300 );
			SetMana( 220 );
			SetStam( 175 );

			SetDamage( 17, 27 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Poison, 50 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 200.0 );

			Fame = Utility.RandomMinMax( 2750, 3250 );
			Karma = Utility.RandomMinMax( -5500, -6500 );

			VirtualArmor = 40;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Gems );
            
			AddLoot( LootPack.Rich );
		}

		public override int TreasureMapLevel{ get{ return 5; } }

		public OldBloodElemental( Serial serial ) : base( serial )
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