using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a werewolf corpse" )]
	public class Werewolf : BaseCreature
	{
		private bool IsInHumanForm;

		public override bool ShowFameTitle { get { return false; } }

		public override bool AlwaysMurderer { get { return !IsInHumanForm; } }

		public override bool BardImmune { get { return true; } }

		public override Poison PoisonImmune { get { return Poison.Regular; } }

		[Constructable]
		public Werewolf()
			: base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Female = Utility.RandomBool();

			HumanForm();
		}

		public void HumanForm()
		{
			if( Female )
			{
				Name = NameList.RandomName( "female" );
				Body = 401;
			}
			else
			{
				Name = NameList.RandomName( "male" );
				Body = 400;
			}

			switch( Utility.Random( 4 ) )
			{
				case 0:
					AddItem( new ShortHair( 1 ) );
					break;
				case 1:
					AddItem( new TwoPigTails( 1 ) );
					break;
				case 2:
					AddItem( new PonyTail( 1 ) );
					break;
				case 3:
					AddItem( new LongHair( 1 ) );
					break;
			}

			AddItem( new FancyShirt( Utility.RandomNeutralHue() ) );
			AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
			AddItem( new Boots( Utility.RandomNeutralHue() ) );

			Hue = Utility.RandomSkinHue();

			SetStr( 196, 250 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 118, 150 );

			SetDamage( 8, 18 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 85.1, 100.0 );
			SetSkill( SkillName.Wrestling, 85.1, 95.0 );

			#region Mod by Magius(CHE)
			/*Aggiunto Karma negativo altrimenti uccidendolo si perdeva karma.*/
			#endregion
			Fame = 3000;
			Karma = -3000;

			VirtualArmor = 40;
			ControlSlots = 1;

			IsInHumanForm = true;
		}

		public void WolfForm()
		{
			Name = "a WereWolf";
			Body = 250;
			BaseSoundID = 0xE5;

			SetStr( 196, 250 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 118, 150 );

			SetDamage( 8, 18 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 85.1, 100.0 );
			SetSkill( SkillName.Wrestling, 85.1, 95.0 );

			Fame = 3000;
			Karma = -3000;

			VirtualArmor = 60;
			ControlSlots = 1;

			IsInHumanForm = false;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}

		public Werewolf( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public override bool OnBeforeDeath()
		{
			if( IsInHumanForm )
			{
				WolfForm();
				return false;
			}

			AddLoot( LootPack.Poor );
			return true;
		}

		public override void OnGotMeleeAttack( Mobile from )
		{
			if( IsInHumanForm )
				WolfForm();
		}

		public override void OnDamagedBySpell( Mobile from )
		{
			if( IsInHumanForm )
				WolfForm();
		}

		public override void OnThink()
		{
			if( !IsInHumanForm )
			{
				base.OnThink();
				return;
			}

			if( IsDay( this ) && !IsInHumanForm )
			{
				Hidden = false;
				HumanForm();
			}
			else if( IsDay( this ) == false && IsInHumanForm )
			{
				Hidden = false;
				WolfForm();
			}
		}

		public static bool IsDay( Mobile from )
		{
			int hours, minutes;
			Clock.GetTime( from.Map, from.X, from.Y, out hours, out minutes );

			return hours >= 4 && hours < 22;
		}
	}
}