using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a glowing corpse" )]
	public class EvilWitch : BaseCreature
	{
		[Constructable]
		public EvilWitch () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "female" );
			Body = 401;
			Title = "the Witch";
			BaseSoundID = 0x482;;
			Hue = Utility.RandomGreenHue();
			Kills = 5;
			SetStr( 416, 505 );
			SetDex( 146, 165 );
			SetInt( 566, 655 );

			SetHits( 250, 303 );

			SetDamage( 11, 13 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 60 );
			SetDamageType( ResistanceType.Energy, 40 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 150.5, 200.0 );
			SetSkill( SkillName.Tactics, 50.1, 70.0 );
			SetSkill( SkillName.Wrestling, 75.1, 85.0 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 55;

			PackGem();
			//PackGem();
			//PackGold( 900, 1300 );
			PackScroll( 3, 8 );
			PackScroll( 3, 8 );
			//PackMagicItems( 1, 5, 0.80, 0.75 );
			//PackMagicItems( 3, 5, 0.60, 0.45 );
			PackSlayer( 1 );

			PackNecroScroll( 2 ); // Corpse Skin
			AddItem( new GnarledStaff() );
			AddItem( new Server.Items.Skirt( Utility.RandomGreenHue() ) );
			AddItem( new Server.Items.FancyShirt( Utility.RandomGreenHue() ) );
			AddItem( new TallStrawHat() );
			AddItem( new TwoPigTails( Utility.RandomGreenHue() ) );
			AddItem( new Pumpkin() );
		
		}

		public override void GenerateLoot()
		{
			// luck fix by snow,gariod,ngetal
			AddLoot( LootPack.Poor );
			AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
		}
		

		

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 1; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.SavagesAndOrcs; }
		}

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );

			Item item = aggressor.FindItemOnLayer( Layer.Helm );

			if ( item is OrcishKinMask )
			{
				AOS.Damage( aggressor, 50, 0, 100, 0, 0, 0 );
				item.Delete();
				aggressor.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
				aggressor.PlaySound( 0x365 );
			}
		}

		public EvilWitch( Serial serial ) : base( serial )
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
