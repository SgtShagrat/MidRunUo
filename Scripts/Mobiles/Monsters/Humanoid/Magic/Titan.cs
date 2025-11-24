using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a titans corpse" )]
	public class Titan : BaseCreature
	{
		public override void OnCarve( Mobile from, Corpse corpse, Item with )
		{
			corpse.DropItem( MakeInstanceOwner( new DitoDiGigante( 1 ), from ) );
			from.SendMessage( (from.Language == "ITA" ? "Prendi un dito come trofeo." : "You take a finger as trophy." ) ); // You shear it, and the wool is now on the corpse.
			base.OnCarve( from, corpse, with );
		}

		[Constructable]
		public Titan() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a titan";
			Body = 76;
			BaseSoundID = 609;

			SetStr( 536, 585 );
			SetDex( 126, 145 );
			SetInt( 281, 305 );

			SetHits( 322, 351 );

			SetDamage( 13, 16 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 25, 35 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 85.1, 100.0 );
			SetSkill( SkillName.Magery, 85.1, 100.0 );
			SetSkill( SkillName.MagicResist, 80.2, 110.0 );
			SetSkill( SkillName.Tactics, 60.1, 80.0 );
			SetSkill( SkillName.Wrestling, 40.1, 50.0 );

			Fame = 11500;
			Karma = -11500;

			VirtualArmor = 40;

			if ( /* Core.ML && */ Utility.RandomDouble() < .33 )
				PackItem( Engines.Plants.Seed.RandomPeculiarSeed(1) );
		}

		public override void GenerateLoot()
		{
            AddLoot( LootPack.Rich );
			// AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Average );
			AddLoot( LootPack.MedScrolls );
		}

		public override int Meat{ get{ return 4; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 5; } }

		public Titan( Serial serial ) : base( serial )
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