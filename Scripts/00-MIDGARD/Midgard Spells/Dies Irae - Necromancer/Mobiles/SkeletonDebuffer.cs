using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class SkeletonDebuffer : BaseNecroFamiliar, ISkeleton
	{
		[Constructable]
		public SkeletonDebuffer() : base( AIType.AI_MageDebuffer, FightMode.Aggressor, 10, 1, 0.1, 0.2 )
		{
			Name = "debuffer minion";
			Body = Utility.RandomList( 50, 56 );
			BaseSoundID = 0x48D;
			Hue = 1077;

			SetStr( 50, 100 );
			SetDex( 56, 75 );
			SetInt( 50, 100 );

			SetHits( 50, 100 );

			SetDamage( 3, 6 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 20 );
			SetResistance( ResistanceType.Fire, 15, 20 );
			SetResistance( ResistanceType.Cold, 15, 20 );
			SetResistance( ResistanceType.Poison, 15, 20 );
			SetResistance( ResistanceType.Energy, 15, 20 );

			SetSkill( SkillName.EvalInt, 60.1, 80.0 );
			SetSkill( SkillName.Magery, 70.1, 80.0 );
			SetSkill( SkillName.MagicResist, 60.1, 80.0 );
			SetSkill( SkillName.Tactics, 20.6, 40.0 );
			SetSkill( SkillName.Wrestling, 20.1, 40.0 );
			SetSkill( SkillName.Necromancy, 150.0 );
			SetSkill( SkillName.SpiritSpeak, 150.0 );

			Fame = 450;
			Karma = -450;

			VirtualArmor = 16;
			Kills = 0;
			MinTameSkill = 0.0;
			Tamable = false;
			ControlSlots = 1;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }

		public SkeletonDebuffer( Serial serial ) : base( serial )
		{
		}

		//public override OppositionGroup OppositionGroup
		//{
		//	get{ return OppositionGroup.FeyAndUndead; }
		//}

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
