using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a rat corpse" )]
	public class Rat : BaseCreature
	{
		public override void OnCarve( Mobile from, Corpse corpse, Item with )
		{
			corpse.DropItem( MakeInstanceOwner( new CodaDiTopo( 1 ), from ) );
			from.SendMessage( (from.Language == "ITA" ? "Stacchi con cura la coda, ti servirà." : "You take his tail, it will be useful." ) ); // You shear it, and the wool is now on the corpse.
			base.OnCarve( from, corpse, with );
		}

		[Constructable]
		public Rat() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a rat";
			Body = 238;
			BaseSoundID = 0xCC;

			SetStr( 9 );
			SetDex( 35 );
			SetInt( 5 );

			SetHits( 6 );
			SetMana( 0 );

			SetDamage( 1, 2 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 5, 10 );
			SetResistance( ResistanceType.Poison, 5, 10 );

			SetSkill( SkillName.MagicResist, 4.0 );
			SetSkill( SkillName.Tactics, 4.0 );
			SetSkill( SkillName.Wrestling, 4.0 );

			Fame = 150;
			Karma = -150;

			VirtualArmor = 6;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = -0.9;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish | FoodType.Eggs | FoodType.GrainsAndHay; } }

		public Rat(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}