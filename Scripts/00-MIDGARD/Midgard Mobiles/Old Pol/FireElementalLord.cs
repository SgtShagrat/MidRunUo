/*npctemplate                  fireelementallord
{
    name                     a fire elemental lord
    script                   spellkillpcs
    objtype                  0x0f
    color                    137
    truecolor                137
    gender                   0
    str                      400
    int                      400
    dex                      200
    hits                     400
    mana                     400
    stam                     200
    tactics                  130
    wrestling                175
    magicresistance          100
    evaluatingintelligence   100
    magery                   100
    attackspeed              35
    attackdamage             5d5
    attackskillid            wrestling
    attackhitsound           0x114
    attackmisssound          0x239
    ar                       30
    cprop                    nocorpse i1
    cast_pct                 35
    num_casts                8
    aspell                   9
    dspell                   9
    lootgroup                38
    magicgroups              4
    powergroup               77
    karma                    -7500    -8000
    fame                     3750     4000
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a fire elemental corpse" )]
	public class FireElementalLord : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public FireElementalLord () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a fire elemental lord";
			Body = 0x0f;
			BaseSoundID = 838;
			Hue = 137;

			SetStr( 400 );
			SetDex( 200 );
			SetInt( 400 );

			SetHits( 400 );
			SetStam( 200 );
			SetMana( 400 );

			SetDamage( 5, 25 );

			SetDamageType( ResistanceType.Physical, 25 );
			SetDamageType( ResistanceType.Fire, 75 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 60, 80 );
			SetResistance( ResistanceType.Cold, 5, 10 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 130.0 );
			SetSkill( SkillName.Wrestling, 175.0 );

			Fame = Utility.RandomMinMax( 3750, 4000 );
			Karma = Utility.RandomMinMax( -7500, -8000 );

			VirtualArmor = 30;
			//ControlSlots = 4;

			PackItem( new SulfurousAsh( 10 ) );

			AddItem( new LightSource() );
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

		public FireElementalLord( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 274 )
				BaseSoundID = 838;
		}
	}
}
