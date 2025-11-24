/*npctemplate                  poisonelemental
{
    name                     a poison elemental
    script                   poisonkillpcs
    objtype                  0x0d
    color                    0x07d6
    truecolor                0x07d6
    gender                   0
    str                      330
    int                      250
    dex                      175
    hits                     330
    mana                     250
    stam                     175
    tactics                  100
    poisoning                100
    wrestling                200
    magicresistance          100
    evaluatingintelligence   100
    attackspeed              35
    attackdamage             25d2
    attackskillid            wrestling
    attackhitsound           0x1d5
    attackmisssound          0x239
    ar                       30
    poisondamagelvl          2d2+1
    hostile                  1
    speech                   7
    provoke                  95
    dstart                   10
    lootgroup                33
    powergroup               71
    karma                    -6500    -7500
    fame                     3250     3750
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a poison elemental corpse" )]
	public class OldPoisonElemental : BaseCreature
	{
		[Constructable]
		public OldPoisonElemental () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a poison elemental";
			Body = 0x0d;
			BaseSoundID = 263;
			Hue = 0x07d6;

			SetStr( 330 );
			SetDex( 175 );
			SetInt( 250 );

			SetHits( 330 );
			SetStam( 175 );
			SetMana( 250 );

			SetDamage( 25, 50 );

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Poison, 90 );

			SetResistance( ResistanceType.Physical, 60, 70 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 40, 50 );
			SetSkill( SkillName.Meditation, 80.2, 120.0 );
			SetSkill( SkillName.Poisoning, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 200.0 );

			Fame = Utility.RandomMinMax( 3250, 3750 );
			Karma = Utility.RandomMinMax( -6500, -7500 );

			VirtualArmor = 30;

			PackItem( new Nightshade( 4 ) );
			PackItem( new LesserPoisonPotion() );
		}

		public override void GenerateLoot()
		{
			// AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Gems );

			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override double HitPoisonChance{ get{ return 0.75; } }

		public override int TreasureMapLevel{ get{ return 5; } }

		public OldPoisonElemental( Serial serial ) : base( serial )
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