using System;
using Server.Engines.XmlSpawner2;
using Server.Items;

namespace Server.Mobiles
{
	public class Ninja : BaseCreature
	{
		public override bool CanTeach{ get{ return true; } }
		public override bool ClickTitle{ get{ return false; } }

		#region modifica by Dies Irae
		public override bool AlwaysMurderer { get { return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override WeaponAbility GetWeaponAbility() { return WeaponAbility.Dismount; }
		public override bool CanRummageCorpses{ get{ return true; } }
		#endregion

		[Constructable]
		public Ninja() : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Title = "the ninja";

			InitStats( 100, 100, 25 );

			SetSkill( SkillName.Fencing, 64.0, 80.0 );
			SetSkill( SkillName.Macing, 64.0, 80.0 );
			SetSkill( SkillName.Ninjitsu, 60.0, 80.0 );
			SetSkill( SkillName.Parry, 64.0, 80.0 );
			SetSkill( SkillName.Tactics, 64.0, 85.0 );
			SetSkill( SkillName.Swords, 64.0, 85.0 );

			SpeechHue = Utility.RandomDyedHue();

			Hue = Utility.RandomSkinHue();

			if ( Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}

			if ( !Female )
				AddItem( new LeatherNinjaHood() );

			AddItem( new LeatherNinjaPants() );
			AddItem( new LeatherNinjaBelt() );
			AddItem( new LeatherNinjaJacket() );
			AddItem( new NinjaTabi() );

			int hairHue = Utility.RandomNondyedHue();


			Utility.AssignRandomHair( this, hairHue );

			if( Utility.Random( 7 ) != 0 )
				Utility.AssignRandomFacialHair( this, hairHue );

			PackGold( 250, 300 );

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

		public Ninja( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
