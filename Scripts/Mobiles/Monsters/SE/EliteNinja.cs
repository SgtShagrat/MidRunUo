using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

using Server.Engines.XmlSpawner2;

namespace Server.Mobiles
{
	public class EliteNinja : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }

		[Constructable]
		public EliteNinja() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();
			Name = "an elite ninja";

			Body = ( this.Female = Utility.RandomBool() ) ? 0x191 : 0x190;

			SetHits( 251, 350 );

			SetStr( 126, 225 );
			SetDex( 81, 95 );
			SetInt( 151, 165 );

			SetDamage( 12, 20 );

			SetDamageType( ResistanceType.Physical, 65 );
			SetDamageType( ResistanceType.Fire, 15 );
			SetDamageType( ResistanceType.Poison, 15 );
			SetDamageType( ResistanceType.Energy, 5 );

			SetResistance( ResistanceType.Physical, 35, 65 );
			SetResistance( ResistanceType.Fire, 40, 60 );
			SetResistance( ResistanceType.Cold, 25, 45 );
			SetResistance( ResistanceType.Poison, 40, 60 );
			SetResistance( ResistanceType.Energy, 35, 55 );

			SetSkill( SkillName.Anatomy, 105.0, 120.0 );
			SetSkill( SkillName.MagicResist, 80.0, 100.0 );
			SetSkill( SkillName.Tactics, 115.0, 130.0 );
			SetSkill( SkillName.Wrestling, 95.0, 120.0 );
			SetSkill( SkillName.Fencing, 95.0, 120.0 );
			SetSkill( SkillName.Macing, 95.0, 120.0 );
			SetSkill( SkillName.Swords, 95.0, 120.0 );
			SetSkill( SkillName.Ninjitsu, 95.0, 120.0 );


			Fame = 8500;
			Karma = -8500;

			/* TODO:	
					Uses Smokebombs
					Hides
					Stealths
					Can use Ninjitsu Abilities
					Can change weapons during a fight
			*/
					

			AddItem( new NinjaTabi() );
			AddItem( new LeatherNinjaJacket());
			AddItem( new LeatherNinjaHood());
			AddItem( new LeatherNinjaPants());
			AddItem( new LeatherNinjaMitts());
			
			if( Utility.RandomDouble() < 0.33 )
				AddItem( new SmokeBomb() );

			switch ( Utility.Random( 8 ))
			{
				case 0: AddItem( new Tessen() ); break;
				case 1: AddItem( new Wakizashi() ); break;
				case 2: AddItem( new Nunchaku() ); break;
				case 3: AddItem( new Daisho() ); break;
				case 4: AddItem( new Sai() ); break;
				case 5: AddItem( new Tekagi() ); break;
				case 6: AddItem( new Kama() ); break;
				case 7: AddItem( new Katana() ); break;
			}

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
			//c.DropItem( new BookOfNinjitsu() );
			#endregion
		}

		#region modifica by Dies Irae
		public override WeaponAbility GetWeaponAbility() { return WeaponAbility.Dismount; }
		public override bool CanRummageCorpses{ get{ return true; } }
		#endregion

		public override bool BardImmune{ get{ return true; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems, 2 );
		}
		
		public override bool AlwaysMurderer{ get{ return true; } }

		public EliteNinja( Serial serial ) : base( serial )
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