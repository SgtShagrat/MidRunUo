/***************************************************************************
 *                                  SanValentine2008.cs
 *                            		-------------------
 *  begin                	: Febbraio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info: 
 * 			Pacco dono per San Valentino 2008.
 * 			Contiene 3 Candy di San Valentino.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Midgard.Misc
{
	public class SanValentine2008 : GiftGiver
	{
		public static void Initialize()
		{
			GiftGiving.Register( new SanValentine2008() );
		}

		public override DateTime Start{ get{ return new DateTime( 2008, 2, 14 ); } }
		public override DateTime Finish{ get{ return new DateTime( 2008, 2, 15 ); } }

		public override void GiveGift( Mobile mob )
		{
			GiftBox box = new GiftBox();
			box.Name = "San Valentino 2008";
			box.Hue = Utility.RandomPinkHue();

			box.DropItem( new Items.ValentineCandy() );
			box.DropItem( new Items.ChocolateHeart() );
			box.DropItem( new Items.ChocolateCandy() );

			switch ( GiveGift( mob, box ) )
			{
				case GiftResult.Backpack:
					mob.SendMessage( 0x482, "Buon San Valentino dallo staff di Midgard! Cerca un piccolo regalo nel tuo zaino." );
					break;
				case GiftResult.BankBox:
					mob.SendMessage( 0x482, "Buon San Valentino dallo staff di Midgard! Cerca un piccolo regalo nella tua banca." );
					break;
			}
		}
	}
}

namespace Midgard.Items
{
	public class SugarItem : Food
	{
		private InternalTimer toothache;

		public SugarItem( int ItemID ) : base( ItemID )
		{
			Stackable = false;
			Weight = 1.0;
			FillFactor = 0;
		}

		public SugarItem( Serial serial ) : base( serial )
		{
		}

		public override bool Eat( Mobile from )
		{
			from.PlaySound( Utility.Random(0x3A, 3) );

			if( from.Body.IsHuman && !from.Mounted )
				from.Animate(34, 5, 1, true, false, 0 );

			if( Poison != null )
				from.ApplyPoison( Poisoner, Poison );

			if( Utility.RandomDouble() < 0.05 )
				GiveToothAche( from, 0 );
			else
				from.SendLocalizedMessage( 1077387 );

			from.PublicOverheadMessage( MessageType.Regular, Hue, false, GetPhrase() );

			Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x36B0, 1, 14, Hue - 1, 7, 9915, 0 );
			Effects.PlaySound( from.Location, from.Map, 0x229 );

			Consume();

			return true;
		}

		public override bool DisplayLootType { get { return false; } }
		public override bool DisplayWeight { get { return false; } }

		protected virtual string GetPhrase()
		{
			return string.Empty;
		}

		public void GiveToothAche( Mobile from, int seq )
		{
			if( toothache != null )
				toothache.Stop();

			from.SendLocalizedMessage( 1077388 + seq );

			if( seq < 5 )
			{
				toothache = new InternalTimer( this, from, seq, TimeSpan.FromSeconds( 15.0 ) );
				toothache.Start();
			}
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

		private class InternalTimer : Timer
		{
			private SugarItem m_Item;
			private int m_Sequencer;
			private Mobile m_Mobile;

			public InternalTimer( SugarItem item, Mobile mobile, int sequencer, TimeSpan delay ) : base( delay )
			{
				Priority = TimerPriority.OneSecond;

				m_Item = item;
				m_Mobile = mobile;
				m_Sequencer = sequencer;
			}

			protected override void OnTick()
			{
				if( m_Mobile != null )
				{
					m_Item.GiveToothAche( m_Mobile, m_Sequencer + 1 );
				}
			}
		}
	}

	public class ValentineCandy : SugarItem
	{
		private string[] NicePhrases = new string[] {   "Be Mine" , "Be Good", "Miss You", "Nice Girl", "So Fine", "Got Love?",
		                                                "Be True", "Kiss Me", "Sure Love", "Thank You", "True Love", "Go Girl",
														"Meet me in Tram", "To Fel!", "Doom Doom Doom", "Let's do a Champ!",
														"Luna Bank Time", "Wanna Hunt?", "IMA Mage", "Tamer's Rock", "Warrior's have big feet",
														"To The Casino!", "Necros r Scary", "Do U Sew?", "Miner's Rock!",
														"Where's My Money?", "I Love this Shard", "No Geeks Here", "Are you sure?",
														"Whaaaat's UuuuuP?", "Me & You", "Let's Go", "Champ ne1?", "What's an FAQ?",
														"Wishful thinking", "You Wanna What?", "Bow To Me!", "I'm Broke", "I'm Rich",
														"You Rule", "You Wish", "TLC", "Only You", "Marry Me", "Let's Kiss",
														"I Heart U", "Friend", "My Hero", "EUO Rocks", "ROFLMAO", "Whatever",
														"Let It Be", "It's Love", "Unreadable!", "How Nice", "Hug Me", "Hi Love",
		                                                "Sweet Talk", "My Pet", "ur a 10", "ur a qt", "ur a star", "ur kind", "LOL",
		                                                "Bear Hug", "Go Fish", "Love Bird", "Whiz Kid", "Wise Up", "Write Me",
														"Dream Girl", "Get Real", "First Kiss", "Call Me", "CEO Rulez", "Sweet Thing",
		                                                "Take A Walk", "Purr Fect", "Cool Cat", "Top Dog", "Yes Dear", "You & Me",
		                                                "Puppy Love", "URA Tiger", "Lover Boy", "Lover Girl", "Email Me" };
		private string[] MeanPhrases = new string[] {   "That Stinks" , "What Smells?", "Take a Hike", "You Stink", "Get Lost", "Tamer's r Lamers",
														"Noob", "Trammy", "PVP Meat", "Loser", "Balron Breath", "Lizard Face", "You're Wierd", "Fight Me!",
		                                                "Wishful Thinking", "Eat My Fireball!", "Mage's r weak", "Warrior's r Dumb", "I'm Bad",
														"Die!", "Eat Dirt", "RoadKill", "Meany", "Huh?", "RTFM", "Get a Job", "Bank Sitter" };
		[Constructable]
		public ValentineCandy() : base( 0x9EA )
		{
			Name = "a Valentine's Day Candy";
			Hue = Utility.RandomList( 32, 54, 356, 18, 43, 88 );
		}

		public ValentineCandy( Serial serial ) : base( serial )
		{
		}

		protected override string GetPhrase()
		{
			int phraseIndex;
			if( Utility.RandomDouble() < 0.10 )
				phraseIndex = Utility.Random( MeanPhrases.Length ) * -1;
			else
				phraseIndex = Utility.Random( NicePhrases.Length );

			return phraseIndex < 0 ? MeanPhrases[ Math.Abs( phraseIndex ) ] : NicePhrases[ phraseIndex ];
		}

        public override void GetProperties( ObjectPropertyList list )
		{
        	base.GetProperties( list );

			list.Add( "Buon San Valentino dallo Staff di Midgard!" );
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

	public class ChocolateHeart : SugarItem
	{
        [Constructable]
		public ChocolateHeart() : base( 0x24B )
		{
            Hue = Utility.RandomList( 1120, 1125, 1150 );

            switch( Hue )
            {
            	case 1120: Name = "A Milk Chocolate Valentine's Heart"; break;
            	case 1125: Name = "A Dark Milk Chocolate Valentine's Heart"; break;
            	case 1150: Name = "An Iced Chocolate Valentine's Heart"; break;
            	default: break;
            }
		}

		public ChocolateHeart( Serial serial ) : base( serial )
		{
		}

		protected override string GetPhrase()
		{
			return "Buon San Valentino dallo Staff di Midgard!";
		}

        public override void GetProperties( ObjectPropertyList list )
		{
        	base.GetProperties( list );

			list.Add( "Buon San Valentino dallo Staff di Midgard!" );
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

	public class ChocolateCandy : SugarItem
	{
        [Constructable]
		public ChocolateCandy() : base( 0xF10 )
		{
            Hue = Utility.RandomList( 1120, 1125, 1150 );

            switch( Hue )
            {
            	case 1120: Name = "A Milk Chocolate Valentine's Candy"; break;
            	case 1125: Name = "A Dark Milk Chocolate Valentine's Candy"; break;
            	case 1150: Name = "An Iced Chocolate Valentine's Candy"; break;
            	default: break;
            }
		}

		public ChocolateCandy( Serial serial ) : base( serial )
		{
		}

		protected override string GetPhrase()
		{
			return "Buon San Valentino dallo Staff di Midgard!";
		}

        public override void GetProperties( ObjectPropertyList list )
		{
        	base.GetProperties( list );

			list.Add( "Buon San Valentino dallo Staff di Midgard!" );
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
