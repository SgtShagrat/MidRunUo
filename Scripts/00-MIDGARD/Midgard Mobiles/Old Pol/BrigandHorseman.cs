/*npctemplate                  brigandhorseman
{
    name                     a brigand
    script                   killpcs
    objtype                  0x190
    color                    0
    truecolor                0
    gender                   0
    str                      160
    int                      195
    dex                      80
    hits                     160
    mana                     195
    stam                     80
    tactics                  85
    fencing                  85
    swordsmanship            80
    parrying                 80
    attackspeed              30
    attackdamage             30
    attackskillid            fencing
    attackhitsound           0x168
    attackmisssound          0x239
    ar                       20
    magicresistance          50
    equip                    brigand1
    mount                    0x3ea0 0
    provoke                  94
    dstart                   10
    lootgroup                18
    powergroup               42
    karma                    -2000    -2500
    fame                     1000     1250
    attackhitscript          :combat:npchitscript
    vendorsellsfor           0
    vendorbuysfor            0
}*/
using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class BrigandHorseman : BaseCreature
	{
		public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.Brigand; } }
		public override bool ClickTitle{ get{ return false; } }

		[Constructable]
		public BrigandHorseman() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the brigand";
			Hue = Utility.RandomSkinHue();

			new Horse().Rider = this;

			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				AddItem( new Skirt( Utility.RandomNeutralHue() ) );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
				AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
			}

			SetStr( 160 );
			SetDex( 80 );
			SetInt( 195 );

			SetHits( 160 );
			SetStam( 80 );
			SetMana( 195 );

			SetDamage( 10, 23 );

			SetSkill( SkillName.Fencing, 85.0 );
			SetSkill( SkillName.Macing, 85.0 );
			SetSkill( SkillName.MagicResist, 50.0 );
			SetSkill( SkillName.Swords, 80.0 );
			SetSkill( SkillName.Tactics, 85.0 );
			SetSkill( SkillName.Wrestling, 80.0 );

			Fame = Utility.RandomMinMax( 1000, 1250 );
			Karma = Utility.RandomMinMax( -2000, -2500 );

			VirtualArmor = 20;

			AddItem( new Boots( Utility.RandomNeutralHue() ) );
			AddItem( new FancyShirt());
			AddItem( new Bandana());

			switch ( Utility.Random( 7 ))
			{
				case 0: AddItem( new Longsword() ); break;
				case 1: AddItem( new Cutlass() ); break;
				case 2: AddItem( new Broadsword() ); break;
				case 3: AddItem( new Axe() ); break;
				case 4: AddItem( new Club() ); break;
				case 5: AddItem( new Dagger() ); break;
				case 6: AddItem( new Spear() ); break;
			}

			Utility.AssignRandomHair( this );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public BrigandHorseman( Serial serial ) : base( serial )
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