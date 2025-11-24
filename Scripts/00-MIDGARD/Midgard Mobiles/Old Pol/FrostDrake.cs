/*npctemplate                  frostdrake
{
    name                     a frost drake
    script                   spellkillpcs
    objtype                  0x3c
    color                    1153
    truecolor                1153
    gender                   0
    str                      400
    int                      200
    dex                      90
    hits                     400
    mana                     200
    stam                     90
    magicresistance          100
    tactics                  100
    wrestling                120
    evaluatingintelligence   120
    magery                   110
    cast_pct                 20
    num_casts                2
    attackspeed              30
    attackdamage             8d6+2
    attackskillid            wrestling
    attackhitsound           0x16d
    attackmisssound          0x239
    ar                       35
    tameskill                200
    food                     meat
    hostile                  1
    movemode                 LS
    provoke                  90
    dstart                   10
    lootgroup                39
    aspell                   9
    dspell                   9
    magicgroups              6
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
	[CorpseName( "a drake corpse" )]
	public class FrostDrake : BaseCreature
	{
		[Constructable]
		public FrostDrake () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a frost drake";
			Body = 0x3c;
			BaseSoundID = 362;
			Hue = 0x4631;

			SetStr( 400 );
			SetDex( 90 );
			SetInt( 200 );

			SetHits( 400 );
			SetStam( 90 );
			SetMana( 200 );

			SetDamage( 10, 56 );

			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Fire, 20 );

			SetResistance( ResistanceType.Physical, 45, 50 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 120.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Magery, 110.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 120.0 );

			Fame = Utility.RandomMinMax( 2750, 3250 );
			Karma = Utility.RandomMinMax( -5500, -6500 );

			VirtualArmor = 35;

			//Tamable = true;
			//ControlSlots = 2;
			//MinTameSkill = 84.3;

			PackReg( 3 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

        //public override bool AutoDispel{ get{ return !Controlled; } } // mod by Dies Irae
		public override bool AutoDispel{ get{ return false; } }//mod by magius(che)
        public override bool StatLossAfterTame { get { return true; } } // mod by Dies Irae
		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
        public override double BreathDamageScalar{ get{ return 0.10; } } // mod by Dies Irae
		public override int TreasureMapLevel{ get{ return 2; } }
		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override int Scales{ get{ return 2; } }
		public override ScaleType ScaleType{ get{ return ScaleType.White; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public FrostDrake( Serial serial ) : base( serial )
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