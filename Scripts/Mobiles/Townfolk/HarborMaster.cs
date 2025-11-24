using System;
using Server.Items;
using Server;
using Server.Misc;

namespace Server.Mobiles
{
	public class HarborMaster : BaseCreature, ITownFolk
	{
        public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.Sailor; } } // mod by Dies Irae

		public override bool CanTeach { get { return false; } }

		[Constructable]
		public HarborMaster()
			: base( AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4 )
		{
			InitStats( 31, 41, 51 );

			SetSkill( SkillName.Mining, 36, 68 );


			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();
			Blessed = true;


			if( this.Female = Utility.RandomBool() )
			{
				this.Body = 0x191;
				this.Name = NameList.RandomName( "female" );
				Title = "the Harbor Mistress";
			}
			else
			{
				this.Body = 0x190;
				this.Name = NameList.RandomName( "male" );
				Title = "the Harbor Master";
			}
			AddItem( new Shirt( Utility.RandomDyedHue() ) );
			AddItem( new Boots() );
			AddItem( new LongPants( Utility.RandomNeutralHue() ) );
			AddItem( new QuarterStaff() );

			Utility.AssignRandomHair( this );

			Container pack = new Backpack();

            #region mod by Dies Irae
            //pack.DropItem( new Gold( 250, 300 ) ); 
            PackGold( 20, 30 );
            #endregion

			pack.Movable = false;

			AddItem( pack );
		}

		public override bool ClickTitle { get { return false; } }


		public HarborMaster( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
