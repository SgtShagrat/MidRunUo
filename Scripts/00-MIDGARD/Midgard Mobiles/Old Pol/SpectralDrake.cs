/*npctemplate                  spectraldrake
{
    name                     a spectral drake
    script                   spellkillpcs
    objtype                  0x3c
    color                    0x4631
    truecolor                0x4631
    gender                   0
    str                      400
    int                      220
    dex                      95
    hits                     400
    mana                     220
    stam                     95
    tactics                  120
    wrestling                140
    magery                   125
    evaluatingintelligence   95
    magicresistance          120
    cast_pct                 17
    num_casts                3
    attackspeed              35
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
    magicgroups              5
    powergroup               69
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
	public class SpectralDrake : BaseCreature
	{
		[Constructable]
		public SpectralDrake () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a spectral drake";
			Body = 0x3c;
			BaseSoundID = 362;
			Hue = 0x4631;

			SetStr( 400 );
			SetDex( 95 );
			SetInt( 220 );

			SetHits( 400 );
			SetStam( 95 );
			SetMana( 220 );

			SetDamage( 10, 56 );

			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Fire, 20 );

			SetResistance( ResistanceType.Physical, 45, 50 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 95.0 );
			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Magery, 125.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 140.0 );

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
			AddLoot( LootPack.FilthyRich );
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
		public override ScaleType ScaleType{ get{ return ScaleType.Black; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public SpectralDrake( Serial serial ) : base( serial )
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