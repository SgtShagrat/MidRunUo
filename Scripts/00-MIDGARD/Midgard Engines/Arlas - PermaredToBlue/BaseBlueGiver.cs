using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.PermaredToBlue
{
	public abstract class BaseBlueGiver : BaseCreature
	{
		private List<Mobile> m_Candidates;
		public List<Mobile> Candidates{get { return m_Candidates; }}

		public virtual string IsEligibleString( Mobile mob )
		{
			return ( mob.Language == "ITA" ? "Non puoi chedere una redenzione." : "Thou doesn't need redemption." );
		}

		public static bool IsPermared( Midgard2PlayerMobile m )//da modificare
		{
			return ( m != null && (m.PermaRed || m.LifeTimeKills > 4) );
		}

		public virtual bool IsEligible( Mobile mob )
		{
			return ( mob != null && mob.Player && mob is Midgard2PlayerMobile && IsPermared((Midgard2PlayerMobile)mob) );
		}

		public bool IsCandidate( Mobile from ){return from != null && m_Candidates != null && m_Candidates.Contains( from );}

		public void AddCandidate( Mobile m )
		{
			if( m_Candidates != null && m_Candidates.Contains( m ) )
				return;

			if( m_Candidates == null )
				m_Candidates = new List<Mobile>();

			m_Candidates.Add( m );
		}

		public void RemoveCandidate( Mobile m )
		{
			if( m_Candidates == null )
				return;

			if( m_Candidates.Contains( m ) )
				m_Candidates.Remove( m );
		}

		private const int ListenRange = 12;

		public BaseBlueGiver( string title ) : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4 )
		{
			Title = title;
			m_Candidates = new List<Mobile>();
		}

		public BaseBlueGiver( Serial serial ) : base( serial )
		{
		}

		public virtual string Greetings
		{
			get { return "Greetings. Do you want to redeem yourself?"; }
		}

		public virtual string JoinQuestion
		{
			get { return "Do you want redemption?"; }
		}

		public override bool DisallowAllMoves{get { return true; }}
		public override bool ClickTitle{get { return false; }}
		public override bool CanTeach{get { return false; }}
		public override bool BardImmune{get { return true; }}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if( InRange( from, ListenRange ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			Mobile from = e.Mobile;

			if( !e.Handled && from is Midgard2PlayerMobile && from.InRange( Location, 2 ) )
			{
				if( WasNamed( e.Speech ) )
				{
					SpellHelper.Turn( this, from );

					if( e.HasKeyword( 0x0004 ) ) // *join* | *member*
					{
						if( IsCandidate( from ) )
							SayTo( from, ( from.Language == "ITA" ? "Sei già in lista per la redenzione!" : "Thou art already in my redemption list!" ) );
						else if( IsEligible( from ) )
						{
							SayTo( from, Greetings );
							from.SendGump( new ConfirmJoinGump( from, this ) );
						}
						else
							SayTo( from, IsEligibleString( @from ) );

						e.Handled = true;
					}
					else if( e.HasKeyword( 0x0005 ) ) // *resign* | *quit*
					{
						if( !IsCandidate( from ) )
						{
							SayTo( from, ( from.Language == "ITA" ? "Non appartieni alla mia congrega!" : "Thou dost not belong to my congregation!" ) );
						}
						else
						{
							SayTo( from, 501054 ); // I accept thy resignation.
							RemoveCandidate( from );
						}

						e.Handled = true;
					}
				}
			}

			base.OnSpeech( e );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( from.AccessLevel > AccessLevel.GameMaster )
				from.SendGump( new PermaredCandidateApprovalGump( this, from ) );

			base.OnDoubleClick( from );
		}

		public virtual void EndJoin( Mobile joiner, bool join )
		{
			if( join )
				AddCandidate( joiner );
		}

		public virtual int GetRandomHue()
		{
			switch( Utility.Random( 5 ) )
			{
				default:
					return Utility.RandomBlueHue();
				case 1:
					return Utility.RandomGreenHue();
				case 2:
					return Utility.RandomRedHue();
				case 3:
					return Utility.RandomYellowHue();
				case 4:
					return Utility.RandomNeutralHue();
			}
		}

		public virtual int GetShoeHue()
		{
			if( 0.1 > Utility.RandomDouble() )
				return 0;

			return Utility.RandomNeutralHue();
		}

		public virtual void InitOutfit()
		{
			switch( Utility.Random( 3 ) )
			{
				case 0:
					AddItem( new FancyShirt( GetRandomHue() ) );
					break;
				case 1:
					AddItem( new Doublet( GetRandomHue() ) );
					break;
				case 2:
					AddItem( new Shirt( GetRandomHue() ) );
					break;
			}

			switch( Utility.Random( 4 ) )
			{
				case 0:
					AddItem( new Shoes( GetShoeHue() ) );
					break;
				case 1:
					AddItem( new Boots( GetShoeHue() ) );
					break;
				case 2:
					AddItem( new Sandals( GetShoeHue() ) );
					break;
				case 3:
					AddItem( new ThighBoots( GetShoeHue() ) );
					break;
			}

			GenerateRandomHair();

			if( Female )
			{
				switch( Utility.Random( 6 ) )
				{
					case 0:
						AddItem( new ShortPants( GetRandomHue() ) );
						break;
					case 1:
					case 2:
						AddItem( new Kilt( GetRandomHue() ) );
						break;
					case 3:
					case 4:
					case 5:
						AddItem( new Skirt( GetRandomHue() ) );
						break;
				}
			}
			else
			{
				switch( Utility.Random( 2 ) )
				{
					case 0:
						AddItem( new LongPants( GetRandomHue() ) );
						break;
					case 1:
						AddItem( new ShortPants( GetRandomHue() ) );
						break;
				}
			}

			PackGold( 100, 200 );
		}

		public virtual void GenerateRandomHair()
		{
			Utility.AssignRandomHair( this );
			Utility.AssignRandomFacialHair( this, HairHue );
		}

		public Item Immovable( Item item )
		{
			item.Movable = false;
			return item;
		}

		public Item Newbied( Item item )
		{
			item.LootType = LootType.Newbied;
			return item;
		}

		public Item Rehued( Item item, int hue )
		{
			item.Hue = hue;
			return item;
		}

		public Item Layered( Item item, Layer layer )
		{
			item.Layer = layer;
			return item;
		}

		public Item Resourced( BaseWeapon weapon, CraftResource resource )
		{
			weapon.Resource = resource;
			return weapon;
		}

		public Item Resourced( BaseArmor armor, CraftResource resource )
		{
			armor.Resource = resource;
			return armor;
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			c.Delete();
		}

		public virtual void GenerateBody( bool isFemale, bool randomHair )
		{
			Hue = Utility.RandomSkinHue();

			if( isFemale )
			{
				Female = true;
				Body = 401;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Female = false;
				Body = 400;
				Name = NameList.RandomName( "male" );
			}

			if( randomHair )
				GenerateRandomHair();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 1 ); // version
			writer.Write( m_Candidates, true );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			if (version >= 1)	
				m_Candidates = reader.ReadStrongMobileList();
			else
				m_Candidates = new List<Mobile>();
		}
	}
}