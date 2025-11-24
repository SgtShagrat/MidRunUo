using System;
using System.Collections;
using Server.Engines.XmlSpawner2;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a ronin corpse" )]
	public class Ronin : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }

		[Constructable]
		public Ronin() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();
			Name = "a ronin";
			Body = (( this.Female = Utility.RandomBool() ) ? Body = 0x191 : Body = 0x190);
			
			Hue = Utility.RandomSkinHue();

			SetStr( 326, 375 );
			SetDex( 31, 45 );
			SetInt( 101, 110 );

	        SetHits( 301, 400 );
			SetMana( 101, 110 );

			SetDamage( 17, 25 );

			SetDamageType( ResistanceType.Physical, 90 );
			SetDamageType( ResistanceType.Poison, 10 );

			SetResistance( ResistanceType.Physical, 55, 75 );
			SetResistance( ResistanceType.Fire, 40, 60 );
			SetResistance( ResistanceType.Cold, 35, 55 );
			SetResistance( ResistanceType.Poison, 50, 70 );
			SetResistance( ResistanceType.Energy, 55, 75 );

			SetSkill( SkillName.MagicResist, 42.6, 57.5 );
			SetSkill( SkillName.Tactics, 115.1, 130.0 );
			SetSkill( SkillName.Wrestling, 92.6, 107.5 );
			SetSkill( SkillName.Anatomy, 110.1, 125.0 );

			SetSkill( SkillName.Fencing, 92.6, 107.5 );
			SetSkill( SkillName.Macing, 92.6, 107.5 );
			SetSkill( SkillName.Swords, 92.6, 107.5 );

			Fame = 8500;
			Karma = -8500;

			AddItem( new SamuraiTabi() );
			AddItem( new LeatherHiroSode());
			AddItem( new LeatherDo());

			switch ( Utility.Random( 4 ))
			{
				case 0: AddItem( new LightPlateJingasa()); break;
				case 1: AddItem( new ChainHatsuburi() ); break;
				case 2: AddItem( new DecorativePlateKabuto() ); break;
				case 3: AddItem( new LeatherJingasa()); break;
			}

			switch ( Utility.Random( 3 ))
			{
				case 0: AddItem( new StuddedHaidate()); break;
				case 1: AddItem( new LeatherSuneate() ); break;
				case 2: AddItem( new PlateSuneate() ); break;
			}
			

			
			if( Utility.RandomDouble() > .2 )
				AddItem( new NoDachi() );
			else
				AddItem( new Halberd() );

			PackItem( new Wakizashi() );
			PackItem( new Longsword() );

			Utility.AssignRandomHair( this );

			#region xmlattach
			// Attachment EnemyMastery
			XmlEnemyMastery WyrmMastery = new XmlEnemyMastery( "WhiteWyrm", 100, 1000);
			WyrmMastery.Name = "WyrmMastery";
			XmlAttach.AttachTo(this, WyrmMastery);
			
			XmlEnemyMastery DragonMastery = new XmlEnemyMastery( "Dragon", 100, 1000);
			DragonMastery.Name = "DragonMastery";
			XmlAttach.AttachTo(this, DragonMastery);
			
			XmlEnemyMastery NightmareMastery = new XmlEnemyMastery( "Nightmare", 100, 1000);
			NightmareMastery.Name = "NightmareMastery";
			XmlAttach.AttachTo(this, NightmareMastery);
			
			XmlEnemyMastery KirinMastery = new XmlEnemyMastery( "Kirin", 100, 1000);
			KirinMastery.Name = "KirinMastery";
			XmlAttach.AttachTo(this, KirinMastery);
			
			XmlEnemyMastery UnicornMastery = new XmlEnemyMastery( "Unicorn", 100, 1000);
			UnicornMastery.Name = "UnicornMastery";
			XmlAttach.AttachTo(this, UnicornMastery);
			#endregion	
		}
		
		public override void OnDeath( Container c )
 		{
			base.OnDeath( c );
			#region modifica by Dies Irae
	 		// c.DropItem( new BookOfBushido() );
	 		#endregion
 		}

		// TODO: Bushido abilities

		#region modifica by Dies Irae
		public override WeaponAbility GetWeaponAbility() { return WeaponAbility.Dismount; }
		#endregion

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems, 2 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }

		public Ronin( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
