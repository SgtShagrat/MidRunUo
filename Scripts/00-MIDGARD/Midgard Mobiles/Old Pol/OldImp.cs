/*npctemplate                  imp
{
    name                     an imp
    script                   firebreather
    objtype                  0x27
    color                    0x23
    truecolor                0x23
    gender                   0
    str                      225
    int                      80
    dex                      60
    hits                     225
    mana                     130
    stam                     60
    tactics                  70
    wrestling                50
    magicresistance          80
    attackspeed              30
    attackdamage             2d15
    attackskillid            wrestling
    attackhitsound           0x1a9
    attackmisssound          0x239
    cast_pct                 40
    num_casts                8
    ar                       14
    hostile                  1
    provoke                  55
    dstart                   10
    lootgroup                28
    aspell                   7
    dspell                   9
    powergroup               40
    karma                    -1600    -2000
    fame                     800     1000
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an imp corpse" )]
	public class OldImp : BaseCreature
	{
		[Constructable]
		public OldImp() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an imp";
			Body = 0x27;
			BaseSoundID = 422;
			Hue = 0x23;

			SetStr( 225 );
			SetDex( 60 );
			SetInt( 80 );

			SetHits( 225 );

			SetDamage( 2, 30 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Fire, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 80.0 );
			SetSkill( SkillName.Tactics, 70.0 );
			SetSkill( SkillName.Wrestling, 50.0 );

			Fame = Utility.RandomMinMax( 800, 1000 );
			Karma = Utility.RandomMinMax( -1600, -2000 );

			VirtualArmor = 14;

			//Tamable = true;
			//ControlSlots = 2;
			//MinTameSkill = 83.1;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
			//AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool HasBreath{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 6; } }
		public override HideType HideType{ get{ return HideType.Spined; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Daemon; } }

		public OldImp( Serial serial ) : base( serial )
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