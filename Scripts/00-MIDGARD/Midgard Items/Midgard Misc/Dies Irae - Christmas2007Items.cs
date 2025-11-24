/***************************************************************************
 *                                  .cs
 *                            		-------------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class Christmas2007
	{
		public static readonly string StaticMidgardGreetings = "Auguri di un Buon Natale 2007 da tutto lo staff di Midgard!";
		public static readonly DateTime EndOfChristmasPeriod = new DateTime( 2008, 1, 6 );

		public static void VerifyEndPeriod_Callback( object state )
		{
			Item  item = (Item)state;

			if( item == null )
			{
				Console.WriteLine( "Warning: null cast in Christmas2007.VerifyEndPeriod_Callback" );
			}
			else
			{
				if( DateTime.Now > EndOfChristmasPeriod && !item.Deleted )
				{
					item.Delete();
				}
			}
		}

		private static readonly string[] MidgardGreetings = new string[]
		{
			"Buon Natale 2007 su Midgard!",
			"Buone feste e felice anno nuovo!",
			"Un augurio di buon Natale dallo staff di Midgard!",
			"Buone feste su Midgard!",
			"Uno splendido Natale a tutti dallo staff di Midgard!"
		};

		public static string GetMidgardGreetings()
		{
			return MidgardGreetings[ Utility.Random( MidgardGreetings.Length ) ];
		}

		public enum MidgardStaff2007
		{
			Dandi = 0, 
			Eclisse, 
			Saerial, 
			DiesIrae,
			Bertoldo,
			Faxx,
			Angel, 
			Thaurus, 
			Dagon, 
			Maarbale, 
			Laijsa, 
			Vingilot, 
			Radister, 
			Belnar, 
			Mignight,
			Riccio
		}

		public static string GetStaffMember( MidgardStaff2007 member )
  		{
   			string name = string.Empty;

  			switch( member )
  			{
  				case( MidgardStaff2007.Dandi ):		name = "Dandi, m4d boss"; break;
				case( MidgardStaff2007.Eclisse ):	name = "Eclisse"; break;
				case( MidgardStaff2007.Saerial ):	name = "Saerial"; break;  
				case( MidgardStaff2007.DiesIrae ):	name = "Dies Iræ"; break;
				case( MidgardStaff2007.Bertoldo ):	name = "Bertoldo"; break; 
				case( MidgardStaff2007.Faxx ):		name = "Faxx"; break; 
				case( MidgardStaff2007.Angel ):		name = "Angel"; break;  
				case( MidgardStaff2007.Thaurus ):	name = "Thaurus"; break;
				case( MidgardStaff2007.Dagon ):		name = "Dagon"; break;
				case( MidgardStaff2007.Maarbale ):	name = "Maarbale"; break;  
				case( MidgardStaff2007.Laijsa ):	name = "Laijsa"; break;
				case( MidgardStaff2007.Vingilot ):	name = "Vingilot"; break; 
				case( MidgardStaff2007.Radister ):	name = "Radister"; break;  
				case( MidgardStaff2007.Belnar ):	name = "Belnar"; break;  
				case( MidgardStaff2007.Mignight ):	name = "Mignight"; break; 
				case( MidgardStaff2007.Riccio ):	name = "Riccio"; break; 
  				default:							name = "Merry Christmass from Midgard Staff!"; break;				
  			}
 
  			return name;
  		}

		public static MidgardStaff2007 GetRandomStaffMember()
		{
			return (MidgardStaff2007)Utility.Random( Enum.GetNames(typeof(MidgardStaff2007)).Length );
		}

		public static string GetRandomStaffMemberName()
		{
			return GetStaffMember( GetRandomStaffMember() );
		}

		public enum SnowScenes2007
		{
			Minoc,
			Vesper,
			Cove,
			Yew,
			Britain,
			SkaraBrae,
			Trinsic,
			SerpentsHold,
			Nejelm,
			Haven,
			BuccaneersDen,
			Jhelom,
			Moonglow,
			Delucia,
			Papua,
			Occlo,
			EmpathsAbbey,
			TheLycaeum,
			Wind,
			Magincia,
			Luna,
			Umbra,
			CityOfMistas,
			CityOfMontor,
			EtherealFortress,
			AncientCitadel,
			ShrineOfValor,
			ShrineOfSpirtuality,
			ShrineOfSacifice,
			ShrineOfJustice,
			ShrineOfHumility,
			ShrineOfHonor,
			ShrineOfHonesty,
			ShrineOfCompassion,
			PassOfKarnaugh
		}

		public static string GetSceneName( SnowScenes2007 type )
		{
   			string name = string.Empty;

  			switch( type )
  			{
				case( SnowScenes2007.Minoc ): 				name = "Minoc"; break;
				case( SnowScenes2007.Vesper ):				name = "Vesper"; break;
				case( SnowScenes2007.Cove ):				name = "Cove"; break;
				case( SnowScenes2007.Yew ):					name = "Yew"; break;
				case( SnowScenes2007.Britain ):				name = "Britain"; break;
				case( SnowScenes2007.SkaraBrae ):			name = "Skara Brae"; break;
				case( SnowScenes2007.Trinsic ):				name = "Trinsic"; break;
				case( SnowScenes2007.SerpentsHold ):		name = "Serpent's Hold"; break;
				case( SnowScenes2007.Nejelm ):				name = "Nejelm"; break;
				case( SnowScenes2007.Haven ):				name = "Haven"; break;
				case( SnowScenes2007.BuccaneersDen ):		name = "Buccaneers Den"; break;
				case( SnowScenes2007.Jhelom ):				name = "Jhelom"; break;
				case( SnowScenes2007.Moonglow ):			name = "Moonglow"; break;
				case( SnowScenes2007.Delucia ):				name = "Delucia"; break;
				case( SnowScenes2007.Papua ):				name = "Papua"; break;
				case( SnowScenes2007.Occlo ):				name = "Occlo"; break;
				case( SnowScenes2007.EmpathsAbbey ):		name = "Empaths Abbey"; break;
				case( SnowScenes2007.TheLycaeum ):			name = "The Lycaeum"; break;
				case( SnowScenes2007.Wind ):				name = "Wind"; break;
				case( SnowScenes2007.Magincia ):			name = "Magincia"; break;
				case( SnowScenes2007.Luna ):				name = "Luna"; break;
				case( SnowScenes2007.Umbra ):				name = "Umbra"; break;
				case( SnowScenes2007.CityOfMistas ):		name = "City Of Mistas"; break;
				case( SnowScenes2007.CityOfMontor ):		name = "City Of Montor"; break;
				case( SnowScenes2007.EtherealFortress ):	name = "Ethereal Fortress"; break;
				case( SnowScenes2007.AncientCitadel ):		name = "Ancient Citadel"; break;
				case( SnowScenes2007.ShrineOfValor ):		name = "Shrine Of Valor"; break;
				case( SnowScenes2007.ShrineOfSpirtuality ):	name = "Shrine Of Spirtuality"; break;
				case( SnowScenes2007.ShrineOfSacifice ):	name = "Shrine Of Sacifice"; break;
				case( SnowScenes2007.ShrineOfJustice ):		name = "Shrine Of Justice"; break;
				case( SnowScenes2007.ShrineOfHumility ):	name = "Shrine Of Humility"; break;
				case( SnowScenes2007.ShrineOfHonor ):		name = "Shrine Of Honor"; break;
				case( SnowScenes2007.ShrineOfHonesty ):		name = "Shrine Of Honesty"; break;
				case( SnowScenes2007.ShrineOfCompassion ):	name = "Shrine Of Compassion"; break;
				case( SnowScenes2007.PassOfKarnaugh ):		name = "Pass Of Karnaugh"; break;
  				default:									name = "Merry Christmass from Midgard Staff!"; break;				
  			}
 
  			return name;		
		}

		#region food
		public class CandyCane : Food
		{
			private InternalTimer toothache;

			[Constructable]
			public CandyCane() : base( Utility.Random( 0x2BDD, 4 ) )
			{
				Stackable = false;
				Weight = 1.0;
				FillFactor = 0;
				LootType = LootType.Blessed;
			}

			public CandyCane(Serial serial) : base(serial)
			{
			}

			public override bool Eat(Mobile from)
			{
					from.PlaySound(Utility.Random(0x3A, 3));
	
					if (from.Body.IsHuman && !from.Mounted)
						from.Animate(34, 5, 1, true, false, 0);
	
					if (Poison != null)
						from.ApplyPoison(Poisoner, Poison);
	
					if (Utility.RandomDouble() < 0.05)
						GiveToothAche(from, 0);
					else
						from.SendLocalizedMessage(1077387);
	
					Consume();
					return true;
			}

			public void GiveToothAche(Mobile from, int seq)
			{
				if (toothache != null)
					toothache.Stop();
	
				from.SendLocalizedMessage(1077388 + seq);
	
				if (seq < 5)
				{
					toothache = new InternalTimer(this, from, seq, TimeSpan.FromSeconds( 15 ));
					toothache.Start();
				}
			}

	        public override void GetProperties( ObjectPropertyList list )
			{
	        	base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			#region serial-deserial
			public override void Serialize(GenericWriter writer)
			{
				base.Serialize(writer);
				writer.Write((int)0); // version
			}
	
			public override void Deserialize(GenericReader reader)
			{
				base.Deserialize(reader);
				int version = reader.ReadInt();
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion

			private class InternalTimer : Timer
			{
				private CandyCane m_Candycane;
				private int m_Sequencer;
				private Mobile m_Mobile;

				public InternalTimer(CandyCane candycane, Mobile m, int sequencer, TimeSpan delay) : base(delay)
				{
					Priority = TimerPriority.OneSecond;
					m_Candycane = candycane;
					m_Mobile = m;
					m_Sequencer = sequencer;
				}

				protected override void OnTick()
				{
					if (m_Mobile != null)
					{
						m_Candycane.GiveToothAche(m_Mobile, m_Sequencer + 1);
					}
				}
			}
		}

		public class GingerBreadCookie : Food
		{
			private InternalTimer toothache;
	
			[Constructable]
			public GingerBreadCookie() : base(Utility.Random(0x2BE1, 2))
			{
				Stackable = false;
				Weight = 1.0;
				FillFactor = 0;
			}

			public GingerBreadCookie(Serial serial) : base(serial)
			{
			}

			public override bool Eat(Mobile from)
			{
				if (Utility.RandomDouble() > 0.33)
				{
					// Play a random "eat" sound
					from.PlaySound(Utility.Random(0x3A, 3));
	
					if (from.Body.IsHuman && !from.Mounted)
						from.Animate(34, 5, 1, true, false, 0);
	
					if (Poison != null)
						from.ApplyPoison(Poisoner, Poison);
	
					if (Utility.RandomDouble() < 0.05)
						GiveToothAche(from, 0);
					else
						from.SendLocalizedMessage(1077387);
					Consume();
					return true;
				}
				from.SendLocalizedMessage(Utility.Random(1077405, 5));
				return false;
			}

			public void GiveToothAche(Mobile from, int seq)
			{
				if (toothache != null)
					toothache.Stop();
				from.SendLocalizedMessage(1077388 + seq);
				if (seq < 5)
				{
					toothache = new InternalTimer(this, from, seq, TimeSpan.FromSeconds(15));
					toothache.Start();
				}
			}

	        public override void GetProperties( ObjectPropertyList list )
			{
	        	base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			#region serial-deserial
			public override void Serialize(GenericWriter writer)
			{
				base.Serialize(writer);
	
				writer.Write((int)0); // version
			}

			public override void Deserialize(GenericReader reader)
			{
				base.Deserialize(reader);
	
				int version = reader.ReadInt();
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion

			private class InternalTimer : Timer
			{
				private GingerBreadCookie i_cookie;
				private int i_sequencer;
				private Mobile i_mobile;
	
				public InternalTimer(GingerBreadCookie cookie, Mobile m, int sequencer, TimeSpan delay)
					: base(delay)
				{
					Priority = TimerPriority.OneSecond;
					i_cookie = cookie;
					i_mobile = m;
					i_sequencer = sequencer;
				}
	
				protected override void OnTick()
				{
					if (i_mobile != null)
					{
						i_cookie.GiveToothAche(i_mobile, i_sequencer + 1);
					}
				}
			}
		}
		#endregion

		#region plants
		public class DecorativeTopiary2007 : Item
		{
			[Constructable]
			public DecorativeTopiary2007() : base( 0x2378 )
			{
				Weight = 1.0;
				LootType = LootType.Blessed;
			}

			public DecorativeTopiary2007( Serial serial ) : base( serial )
			{
			}

			public override void OnSingleClick( Mobile from )
			{
				base.OnSingleClick( from );

				LabelTo( from, "Christmas 2007" );
			}
	
			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				PublicOverheadMessage( MessageType.Regular, Utility.RandomRedHue(), true, GetMidgardGreetings() );

				base.OnDoubleClick(from);
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
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
		}
	
		public class FestiveCactus2007 : Item
		{
			[Constructable]
			public FestiveCactus2007() : base( 0x2376 )
			{
				Weight = 1.0;
				LootType = LootType.Blessed;
			}

			public FestiveCactus2007( Serial serial ) : base( serial )
			{
			}

			public override void OnSingleClick( Mobile from )
			{
				base.OnSingleClick( from );
	
				LabelTo( from, "Christmas 2007" );
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );
	
				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				PublicOverheadMessage( MessageType.Regular, Utility.RandomRedHue(), true, Christmas2007.GetMidgardGreetings() );

				base.OnDoubleClick(from);
			}

			#region serial-deserial
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
				writer.Write( (int) 0 ); // version
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
				int version = reader.ReadInt();
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion
		}

		public class SnowyTree2007 : Item
		{
			[Constructable]
			public SnowyTree2007() : base( 0x2377 )
			{
				Weight = 1.0;
				LootType = LootType.Blessed;
			}

			public SnowyTree2007( Serial serial ) : base( serial )
			{
			}

			public override void OnSingleClick( Mobile from )
			{
				base.OnSingleClick( from );
	
				LabelTo( from, "Christmas 2007" );
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				PublicOverheadMessage( MessageType.Regular, Utility.RandomRedHue(), true, GetMidgardGreetings() );

				base.OnDoubleClick(from);
			}

			#region serial-deserial
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
				writer.Write( (int) 0 ); // version
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
				int version = reader.ReadInt();
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion
		}
		#endregion

		#region holiday bells
		public enum HolidayBell2007Sound
		{
			Sound1 = 0,
			Sound2,
			Sound3,
			Sound4,
			Sound5,
			Sound6
		}
	
		public class HolidayBell2007 : Item
		{
			private MidgardStaff2007 m_Type;
			private HolidayBell2007Sound m_Sound;

			[CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
			public MidgardStaff2007 Type
			{
				get{ return m_Type; }
				set{ m_Type = value; }
			}

			[CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
			public HolidayBell2007Sound Sound
			{
				get{ return m_Sound; }
				set{ m_Sound = value; }
			}

			[Constructable]
			public HolidayBell2007() : base( 0x1C12 )
			{
				m_Type = Christmas2007.GetRandomStaffMember();
				m_Sound = (HolidayBell2007Sound)Utility.Random( Enum.GetNames(typeof(HolidayBell2007Sound)).Length );

				Hue = Utility.RandomList( 1150 , 55, 65, 75, 85, 95, 105, 115, 125, 135, 145, 30, 35, 37 );
				LootType = LootType.Blessed;

				Name = "Midgard Holiday Bell";
			}

			public HolidayBell2007( Serial serial ) : base( serial )
			{
			}

	  		public override void OnDoubleClick( Mobile from ) 
	  		{
	  			if( Utility.InsensitiveCompare( Name, "Midgard Holiday Bell" ) == 0 )
	  			{
		  			string name = Christmas2007.GetStaffMember( m_Type );

		  			if( !String.IsNullOrEmpty( name ) )
		  			{
		  				from.SendMessage( "Hey, you notice some words on the bell! " );
						from.PlaySound( 0x1FA );
						Effects.SendLocationEffect( this, this.Map, 14201, 16 );
						Name = String.Format( "an holiday bell gifted by {0}", name );
						InvalidateProperties();
		  			}
		  			return;
	  			}

	  			int sound = 0x100;

	  			switch( m_Sound )
	  			{
	  				case( HolidayBell2007Sound.Sound1 ): 	sound = 0x100; break;
	  				case( HolidayBell2007Sound.Sound2 ): 	sound = 0x101; break;
	  				case( HolidayBell2007Sound.Sound3 ): 	sound = 0x103; break;
	  				case( HolidayBell2007Sound.Sound4 ): 	sound = 0x104; break;
	  				case( HolidayBell2007Sound.Sound5 ): 	sound = 0x16;  break;
	  				case( HolidayBell2007Sound.Sound6 ): 	sound = 0x428; break;
	  				default:  								sound = 0x100; break;				
	  			}
	
	  			from.PlaySound( sound );
	  			PublicOverheadMessage( MessageType.Regular, Utility.RandomRedHue(), true, Christmas2007.GetMidgardGreetings() );
	 
	  			base.OnDoubleClick( from );
	      	}

			public override void AddNameProperties( ObjectPropertyList list )
			{
				base.AddNameProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			#region serial-deserial
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
	
				writer.Write( (int) 0 ); // version
	
				writer.WriteEncodedInt( (int) m_Type );
				writer.WriteEncodedInt( (int) m_Sound );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
	
				int version = reader.ReadInt();
				
				m_Type = (MidgardStaff2007)reader.ReadEncodedInt();
				m_Sound = (HolidayBell2007Sound)reader.ReadEncodedInt();
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion
		}
		#endregion

		#region Mistletoe
		public class MistletoeAddon2007 : Item, IDyable, IAddon
		{
			[Constructable]
			public MistletoeAddon2007() : this( Utility.RandomDyedHue() )
			{
			}

			[Constructable]
			public MistletoeAddon2007( int hue ) : base( 0x2375 )
			{
				Hue = hue;
				Movable = false;
			}

			public MistletoeAddon2007( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( Christmas2007.StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public bool CouldFit( IPoint3D p, Map map )
			{
				if ( !map.CanFit( p.X, p.Y, p.Z, this.ItemData.Height ) )
					return false;
	
				if ( this.ItemID == 0x2375 )
					return BaseAddon.IsWall( p.X, p.Y - 1, p.Z, map ); // North wall
				else
					return BaseAddon.IsWall( p.X - 1, p.Y, p.Z, map ); // West wall
			}

			#region serial-deserial
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
				writer.Write( (int) 0 ); // version
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
				int version = reader.ReadInt();
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( FixMovingCrate ) );
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion

			private void FixMovingCrate()
			{
				if ( this.Deleted )
					return;

				if ( this.Movable || this.IsLockedDown )
				{
					Item deed = this.Deed;

					if ( this.Parent is Item )
					{
						((Item)this.Parent).AddItem( deed );
						deed.Location = this.Location;
					}
					else
					{
						deed.MoveToWorld( this.Location, this.Map );
					}

					Delete();
				}
			}

			public Item Deed
			{
				get{ return new MistletoeDeed2007( this.Hue ); }
			}

			public override void OnDoubleClick( Mobile from )
			{
				BaseHouse house = BaseHouse.FindHouseAt( this );
	
				if ( house != null && house.IsCoOwner( from ) )
				{
					if ( from.InRange( this.GetWorldLocation(), 3 ) )
					{
						from.CloseGump( typeof( MistletoeAddonGump2007 ) );
						from.SendGump( new MistletoeAddonGump2007( from, this ) );
					}
					else
					{
						from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
					}
				}
			}

			public virtual bool Dye( Mobile from, DyeTub sender )
			{
				if ( Deleted )
					return false;
	
				BaseHouse house = BaseHouse.FindHouseAt( this );
	
				if ( house != null && house.IsCoOwner( from ) )
				{
					if ( from.InRange( GetWorldLocation(), 1 ) )
					{
						Hue = sender.DyedHue;
						return true;
					}
					else
					{
						from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
						return false;
					}
				}
				else 
				{
					return false;
				}
			}
	
			private class MistletoeAddonGump2007 : Gump
			{
				private Mobile m_From;
				private MistletoeAddon2007 m_Addon;

				public MistletoeAddonGump2007( Mobile from, MistletoeAddon2007 addon ) : base( 150, 50 )
				{
					m_From = from;
					m_Addon = addon;
	
					AddPage( 0 );
	
					AddBackground( 0, 0, 220, 170, 0x13BE );
					AddBackground( 10, 10, 200, 150, 0xBB8 );
					AddHtmlLocalized( 20, 30, 180, 60, 1062839, false, false ); // Do you wish to re-deed this decoration?
					AddHtmlLocalized( 55, 100, 160, 25, 1011011, false, false ); // CONTINUE
					AddButton( 20, 100, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
					AddHtmlLocalized( 55, 125, 160, 25, 1011012, false, false ); // CANCEL
					AddButton( 20, 125, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0 );
				}

				public override void OnResponse( NetState sender, RelayInfo info )
				{
					if ( m_Addon.Deleted )
						return;
	
					if ( info.ButtonID == 1 )
					{
						if ( m_From.InRange( m_Addon.GetWorldLocation(), 3 ) )
						{
							m_From.AddToBackpack( m_Addon.Deed );
							m_Addon.Delete();
						}
						else
						{
							m_From.SendLocalizedMessage( 500295 ); // You are too far away to do that.
						}
					}
				}
			}
		}

		[Flipable( 0x14F0, 0x14EF )]
		public class MistletoeDeed2007 : Item
		{
			public override int LabelNumber{ get{ return 1070882; } } // Mistletoe Deed

			[Constructable]
			public MistletoeDeed2007() : this( 0 )
			{
			}

			[Constructable]
			public MistletoeDeed2007( int hue ) : base( 0x14F0 )
			{
				Hue = hue;
				Weight = 1.0;
				LootType = LootType.Blessed;
			}

			public MistletoeDeed2007( Serial serial ) : base( serial )
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
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}

			public override void OnSingleClick( Mobile from )
			{
				base.OnSingleClick( from );
	
				LabelTo( from, "Christmas 2007" );
			}
	
			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}
	
			public override void OnDoubleClick( Mobile from )
			{
				if ( IsChildOf( from.Backpack ) )
				{
					BaseHouse house = BaseHouse.FindHouseAt( from );
	
					if ( house != null && house.IsCoOwner( from ) )
					{
						from.SendLocalizedMessage( 1062838 ); // Where would you like to place this decoration?
						from.BeginTarget( -1, true, TargetFlags.None, new TargetStateCallback( Placement_OnTarget ), null );
					}
					else
					{
						from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
					}
				}
				else
				{
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
			}
	
			public void Placement_OnTarget( Mobile from, object targeted, object state )
			{
				IPoint3D p = targeted as IPoint3D;

				if ( p == null )
					return;

				Point3D loc = new Point3D( p );

				BaseHouse house = BaseHouse.FindHouseAt( loc, from.Map, 16 );

				if ( house != null && house.IsCoOwner( from ) )
				{
					bool northWall = BaseAddon.IsWall( loc.X, loc.Y - 1, loc.Z, from.Map );
					bool westWall = BaseAddon.IsWall( loc.X - 1, loc.Y, loc.Z, from.Map );
	
					if ( northWall && westWall )
						from.SendGump( new MistletoeDeedGump2007( from, loc, this ) );
					else
						PlaceAddon( from, loc, northWall, westWall );
				}
				else
				{
					from.SendLocalizedMessage( 1042036 ); // That location is not in your house.
				}
			}
	
			private void PlaceAddon( Mobile from, Point3D loc, bool northWall, bool westWall )
			{
				if ( Deleted )
					return;

				BaseHouse house = BaseHouse.FindHouseAt( loc, from.Map, 16 );

				if ( house == null || !house.IsCoOwner( from ) )
				{
					from.SendLocalizedMessage( 1042036 ); // That location is not in your house.
					return;
				}

				int itemID = 0;

				if ( northWall )
					itemID = 0x2374;
				else if ( westWall )
					itemID = 0x2375;
				else
					from.SendLocalizedMessage( 1070883 ); // The mistletoe must be placed next to a wall.
	
				if ( itemID > 0 )
				{
					Item addon = new MistletoeAddon2007( this.Hue );
	
					addon.ItemID = itemID;
					addon.MoveToWorld( loc, from.Map );
	
					house.Addons.Add( addon );
					Delete();
				}
			}

			private class MistletoeDeedGump2007 : Gump
			{
				private Mobile m_From;
				private Point3D m_Loc;
				private MistletoeDeed2007 m_Deed;
	
				public MistletoeDeedGump2007( Mobile from, Point3D loc, MistletoeDeed2007 deed ) : base( 150, 50 )
				{
					m_From = from;
					m_Loc = loc;
					m_Deed = deed;
	
					AddBackground( 0, 0, 300, 150, 0xA28 );
	
					AddPage( 0 );
	
					AddItem( 90, 30, 0x2375 );
					AddItem( 180, 30, 0x2374 );
					AddButton( 50, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0 );
					AddButton( 145, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0 );
				}
	
				public override void OnResponse( NetState sender, RelayInfo info )
				{
					if ( m_Deed.Deleted )
						return;
	
					switch( info.ButtonID )
					{
						case 1:	m_Deed.PlaceAddon( m_From, m_Loc, false, true );	break;
						case 2:	m_Deed.PlaceAddon( m_From, m_Loc, true, false );	break;
					}
				}
			}
		}
		#endregion

		#region snow things
		public class PileOfGlacialSnow2007 : Item
		{
			[Constructable]
			public PileOfGlacialSnow2007() : base( 0x913 )
			{
				Hue = 0x480;
				Weight = 1.0;
				LootType = LootType.Blessed;
			}

			public override int LabelNumber{ get{ return 1070874; } } // a Pile of Glacial Snow

			public PileOfGlacialSnow2007( Serial serial ) : base( serial )
			{
			}

			#region serial-deserial
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
	
				writer.Write( (int) 1 ); // version
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
	
				int version = reader.ReadInt();
	
				if ( version == 0 )
				{
					Weight = 1.0;
					LootType = LootType.Blessed;
				}
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion

			public override void OnSingleClick( Mobile from )
			{
				base.OnSingleClick( from );
	
				LabelTo( from, "Christmas 2007" ); // Winter 2004
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				if ( !IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1042010 ); // You must have the object in your backpack to use it.
				}
				else if ( from.CanBeginAction( typeof( SnowPile2007 ) ) )
				{
					from.SendLocalizedMessage( 1005575 ); // You carefully pack the snow into a ball...
					from.Target = new SnowTarget( from, this );
				}
				else
				{
					from.SendLocalizedMessage( 1005574 ); // The snow is not ready to be packed yet.  Keep trying.
				}
			}

			private class InternalTimer : Timer
			{
				private Mobile m_From;
	
				public InternalTimer( Mobile from ) : base( TimeSpan.FromSeconds( 5.0 ) )
				{
					m_From = from;
				}
	
				protected override void OnTick()
				{
					m_From.EndAction( typeof( SnowPile2007 ) );
				}
			}

			private class SnowTarget : Target
			{
				private Mobile m_Thrower;
				private Item m_Snow;

				public SnowTarget( Mobile thrower, Item snow ) : base ( 10, false, TargetFlags.None )
				{
					m_Thrower = thrower;
					m_Snow = snow;
				}

				protected override void OnTarget( Mobile from, object target )
				{
					if ( target == from )
					{
						from.SendLocalizedMessage( 1005576 ); // You can't throw this at yourself.
					}
					else if ( target is Mobile )
					{
						Mobile targ = (Mobile) target;
						Container pack = targ.Backpack;
	
						if ( pack != null && pack.FindItemByType( new Type[]{ typeof( SnowPile2007 ), typeof( PileOfGlacialSnow2007 ) } ) != null )
						{
							if ( from.BeginAction( typeof( SnowPile2007 ) ) )
							{
								new InternalTimer( from ).Start();
	
								from.PlaySound( 0x145 );
	
								from.Animate( 9, 1, 1, true, false, 0 );
	
								targ.SendLocalizedMessage( 1010572 ); // You have just been hit by a snowball!
								from.SendLocalizedMessage( 1010573 ); // You throw the snowball and hit the target!
	
								Effects.SendMovingEffect( from, targ, 0x36E4, 7, 0, false, true, 0x47F, 0 );
							}
							else
							{
								from.SendLocalizedMessage( 1005574 ); // The snow is not ready to be packed yet.  Keep trying.
							}
						}
						else
						{
							from.SendLocalizedMessage( 1005577 ); // You can only throw a snowball at something that can throw one back.
						}
					}
					else
					{
						from.SendLocalizedMessage( 1005577 ); // You can only throw a snowball at something that can throw one back.
					}
				}
			}
		}

		public class SnowGlobe2007 : Item
		{
			private SnowScenes2007 m_Type;

			[CommandProperty( AccessLevel.GameMaster )]
			public SnowScenes2007 Type
			{
				get{ return m_Type; }
				set{ m_Type = value; }
			}

			[Constructable]
			public SnowGlobe2007() : base( 0xE2D )
			{
				m_Type = (SnowScenes2007)Utility.Random( Enum.GetNames( typeof(SnowScenes2007) ).Length );
				LootType = LootType.Blessed;
				Name = "A Midgard Snow Globe";
			}
	
			public SnowGlobe2007( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
	  			if( Utility.InsensitiveCompare( Name, "A Midgard Snow Globe" ) == 0 )
	  			{
		  			string scene = Christmas2007.GetSceneName( m_Type );

		  			if( !String.IsNullOrEmpty( scene ) )
		  			{
		  				from.SendMessage( "Hey, you notice some words on the globe! " );
						from.PlaySound( 0x1FA );
						Effects.SendLocationEffect( this, this.Map, 14201, 16 );
						Name = String.Format( "a snowy scene of {0}", scene );
						InvalidateProperties();
		  			}
		  			return;
	  			}

	  			PublicOverheadMessage( MessageType.Regular, Utility.RandomRedHue(), true, Christmas2007.GetMidgardGreetings() );

				base.OnDoubleClick( from );
			}
			#region serial-deserial
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
	
				writer.Write( (int) 0 ); // version
	
				writer.WriteEncodedInt( (int) m_Type );
			}
	
			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
	
				int version = reader.ReadInt();
				
				m_Type = (SnowScenes2007)reader.ReadEncodedInt();
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion
		}

		[Flipable( 0x2328, 0x2329 )]
		public class Snowman2007 : Item, IDyable
		{
			private static readonly TimeSpan MessageDelay = TimeSpan.FromSeconds( 5.0 );

			private DateTime m_NextMessage;
			private MidgardStaff2007 m_Type;
	
			[CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
			public MidgardStaff2007 Type
			{
				get{ return m_Type; }
				set{ m_Type = value; }
			}
	
			[Constructable]
			public Snowman2007() : this( Utility.RandomDyedHue(), Christmas2007.GetRandomStaffMember())
			{
			}
	
			[Constructable]
			public Snowman2007( int hue ) : this( hue, Christmas2007.GetRandomStaffMember() )
			{
			}
	
			[Constructable]
			public Snowman2007( MidgardStaff2007 type ) : this( Utility.RandomDyedHue(), type )
			{
			}
	
			[Constructable]
			public Snowman2007( int hue, MidgardStaff2007 type ) : base( Utility.Random( 0x2328, 2 ) )
			{
				Weight = 10.0;
				LootType = LootType.Blessed;

				Hue = hue;
				m_Type = type;

				Name = "a midgard snowman";
			}

			public Snowman2007( Serial serial ) : base( serial )
			{
			}

			public bool Dye( Mobile from, DyeTub sender )
			{
				if( Deleted )
					return false;
	
				Hue = sender.DyedHue;
	
				return true;
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			private static string[] m_Shouts = new string[]
			{
				"Ho Ho Ho! Sto congelando!",
				"Hey! Tu! Ma non hai freddo?",
				"Brrrrrrrrrrr",
				"Buon Natale su Midgard!",
				"Ma cosa mi avete messo al posto del naso!?",
				"Via con quella lampada, mi sciolgo!",
				"Niente scherzi eh, non fare flame!"
			};

			public override bool HandlesOnMovement{ get{ return true; } }

			public override void OnMovement( Mobile m, Point3D oldLocation )
			{
				if( (!m.Hidden || m.AccessLevel == AccessLevel.Player) && Utility.InRange( m.Location, this.Location, 2 ) && !Utility.InRange( oldLocation, this.Location, 2 ) )
				{
					if( DateTime.Now > m_NextMessage )
					{
						m_NextMessage = DateTime.Now + MessageDelay;
						PublicOverheadMessage( MessageType.Regular, 37, true, m_Shouts[ Utility.Random( m_Shouts.Length ) ] );
					}
				}
	
				base.OnMovement( m, oldLocation );
			}

			public override void OnDoubleClick( Mobile from )
			{
	  			if( Utility.InsensitiveCompare( Name, "a midgard snowman" ) == 0 )
	  			{
		  			string name = Christmas2007.GetStaffMember( m_Type );

		  			if( !String.IsNullOrEmpty( name ) )
		  			{
		  				from.SendMessage( "The snowman seems to be someone you know..." );
						from.PlaySound( 0x1FA );
						Effects.SendLocationEffect( this, this.Map, 14201, 16 );
						Name = String.Format( "{0}, the Snowman", name );
						InvalidateProperties();
		  			}
		  			return;
	  			}

				PublicOverheadMessage( MessageType.Regular, 37, true, m_Shouts[ Utility.Random( m_Shouts.Length ) ] );

				base.OnDoubleClick( from );
			}

			#region serial-deserial
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 0 ); // version
				writer.WriteEncodedInt( (int) m_Type );
			}
	
			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
	
				int version = reader.ReadInt();
				m_Type = (MidgardStaff2007)reader.ReadEncodedInt();
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion
		}

		public class SnowPile2007 : Item
		{
			[Constructable]
			public SnowPile2007() : base( 0x913 )
			{
				Hue = 0x481;
				Weight = 1.0;
				LootType = LootType.Blessed;
			}
	
			public override int LabelNumber{ get{ return 1005578; } } // a pile of snow
	
			public SnowPile2007( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			#region serial-deserial
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
	
				writer.Write( (int) 1 ); // version
			}
	
			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
	
				int version = reader.ReadInt();
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion

			public override void OnDoubleClick( Mobile from )
			{
				if( !IsChildOf( from.Backpack ) )
					from.SendLocalizedMessage( 1042010 ); // You must have the object in your backpack to use it.
				else if( from.CanBeginAction( typeof( SnowPile2007 ) ) )
				{
					from.SendLocalizedMessage( 1005575 ); // You carefully pack the snow into a ball...
					from.Target = new SnowTarget( from, this );
				}
				else
					from.SendLocalizedMessage( 1005574 ); // The snow is not ready to be packed yet.  Keep trying.
			}

			private class InternalTimer : Timer
			{
				private Mobile m_From;

				public InternalTimer( Mobile from ) : base( TimeSpan.FromSeconds( 5.0 ) )
				{
					m_From = from;
				}

				protected override void OnTick()
				{
					m_From.EndAction( typeof( SnowPile2007 ) );
				}
			}

			private class SnowTarget : Target
			{
				private Mobile m_Thrower;
				private Item m_Snow;

				public SnowTarget( Mobile thrower, Item snow ) : base ( 10, false, TargetFlags.None )
				{
					m_Thrower = thrower;
					m_Snow = snow;
				}

				protected override void OnTarget( Mobile from, object target )
				{
					if ( target == from )
					{
						from.SendLocalizedMessage( 1005576 ); // You can't throw this at yourself.
					}
					else if ( target is Mobile )
					{
						Mobile targ = (Mobile) target;
						Container pack = targ.Backpack;
	
						if ( pack != null && pack.FindItemByType( new Type[]{ typeof( SnowPile2007 ), typeof( PileOfGlacialSnow2007 ) } ) != null )
						{
							if ( from.BeginAction( typeof( SnowPile2007 ) ) )
							{
								new InternalTimer( from ).Start();

								from.PlaySound( 0x145 );

								from.Animate( 9, 1, 1, true, false, 0 );

								targ.SendLocalizedMessage( 1010572 ); // You have just been hit by a snowball!
								from.SendLocalizedMessage( 1010573 ); // You throw the snowball and hit the target!
	
								Effects.SendMovingEffect( from, targ, 0x36E4, 7, 0, false, true, 0x480, 0 );
							}
							else
							{
								from.SendLocalizedMessage( 1005574 ); // The snow is not ready to be packed yet.  Keep trying.
							}
						}
						else
						{
							from.SendLocalizedMessage( 1005577 ); // You can only throw a snowball at something that can throw one back.
						}
					}
					else
					{
						from.SendLocalizedMessage( 1005577 ); // You can only throw a snowball at something that can throw one back.
					}
				}
			}
		}
		#endregion

		[FlipableAttribute( 0x236E, 0x2371 )]
		public class LightOfTheWinterSolstice2007  : Item
		{
			private MidgardStaff2007 m_Type;

			[CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
			public MidgardStaff2007 Type
			{
				get{ return m_Type; }
				set{ m_Type = value; }
			}

			[Constructable]
			public LightOfTheWinterSolstice2007() : base( 0x236E )
			{
				m_Type = (MidgardStaff2007)Utility.Random( Enum.GetNames(typeof(MidgardStaff2007)).Length );
	
				Weight = 1.0;
				Hue = 1154;
				Light = LightType.Circle300;
				LootType = LootType.Blessed;

				Name = "light of the 2007 winter solstice";
			}

			public LightOfTheWinterSolstice2007( Serial serial ) : base( serial )
			{
			}

			public override void OnSingleClick( Mobile from )
			{
				base.OnSingleClick( from );
	
				string name = Christmas2007.GetStaffMember( m_Type );
	
				if( !String.IsNullOrEmpty( name ) )
					LabelTo( from, 1070881, name ); // Hand Dipped by ~1_name~
	
				LabelTo( from, "Christmas 2007" );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
	  			if( Utility.InsensitiveCompare( Name, "light of the 2007 winter solstice" ) == 0 )
	  			{
		  			string name = Christmas2007.GetStaffMember( m_Type );

		  			if( !String.IsNullOrEmpty( name ) )
		  			{
		  				from.SendMessage( "You see some words on the lightsource..." );
						from.PlaySound( 0x1FA );
						Effects.SendLocationEffect( this, this.Map, 14201, 16 );
						Name = String.Format( "light of the winter solstice, gifted by {0}", name );
						Hue = Utility.RandomDyedHue();
						InvalidateProperties();
		  			}
		  			return;
	  			}

	  			PublicOverheadMessage( MessageType.Regular, Utility.RandomRedHue(), true, Christmas2007.GetMidgardGreetings() );

				base.OnDoubleClick( from );
			}

			#region serial-deserial
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
	
				writer.Write( (int) 0 ); // version
				writer.WriteEncodedInt( (int) m_Type );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
	
				int version = reader.ReadInt();
				m_Type = (MidgardStaff2007)reader.ReadEncodedInt();
				Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
			#endregion
		}

		[Furniture]
		[Flipable( 0x2BDB, 0x2BDC )]
	    public class RedStockin : BaseContainer
	    {
	        public override int MaxWeight { get { return 0; } }
	        public override int DefaultDropSound { get { return 0x42; } }
			public override int DefaultGumpID { get { return 0x103; } }

	        [Constructable]
	        public RedStockin() : base( Utility.Random( 0x2BDB, 2 ) )
	        {
	            Weight = 1.0;
	        }

	        public RedStockin(Serial serial): base(serial)
	        {
	        }

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				PublicOverheadMessage( MessageType.Regular, 37, true, Christmas2007.GetMidgardGreetings() );

				base.OnDoubleClick( from );
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
	    }
 
		public class GreenStockin : Bag
	    {
	        public override int MaxWeight { get { return 0; } }
	        public override int DefaultDropSound { get { return 0x42; } }

	        [Constructable]
	        public GreenStockin()
	        {
	            Movable = true;
	            GumpID = 259;
	            Weight = 1.0;
	            ItemID = Utility.RandomList(0x2BD9, 0x2BDA); //Green Stockin
	        }

	        public GreenStockin(Serial serial) : base(serial)
	        {
	        }

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }
			public override bool DisplaysContent { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				PublicOverheadMessage( MessageType.Regular, 37, true, Christmas2007.GetMidgardGreetings() );

				base.OnDoubleClick( from );
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
	    }

		public class GingerbreadHouseAddonDeed : MiniHouseDeed
		{
			[Constructable]
	        public GingerbreadHouseAddonDeed() : base( MiniHouseType.GingerbreadHouse )
			{
				LootType = LootType.Blessed;
	        }

	        public GingerbreadHouseAddonDeed( Serial serial ) : base( serial )
	        {
	        }

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}

		#region reindeers
		public class BaseReindeer : Item
		{
			private static readonly TimeSpan MessageDelay = TimeSpan.FromSeconds( 5.0 );

			private DateTime m_NextMessage;

			public BaseReindeer( int itemID, int hue, string name ) : base( itemID )
			{
				Name = name;
				Hue = hue;
				Weight = 10.0;
				LootType = LootType.Blessed; 
			}

			public BaseReindeer( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			private static string[] m_Shouts = new string[]
			{
				"* Yuk *",
				"* Sgrunt *",
				"* Brrrrr *",
				"* Yawn *"
			};

			public override bool HandlesOnMovement{ get{ return true; } }

			public override void OnMovement( Mobile m, Point3D oldLocation )
			{
				if( (!m.Hidden || m.AccessLevel == AccessLevel.Player) && Utility.InRange( m.Location, this.Location, 2 ) && !Utility.InRange( oldLocation, this.Location, 2 ) )
				{
					if( DateTime.Now > m_NextMessage )
					{
						m_NextMessage = DateTime.Now + MessageDelay;
						PublicOverheadMessage( MessageType.Regular, 37, true, m_Shouts[ Utility.Random( m_Shouts.Length ) ] );
					}
				}
	
				base.OnMovement( m, oldLocation );
			}

			public override void OnDoubleClick( Mobile from )
			{
				if( DateTime.Now > m_NextMessage )
				{
					m_NextMessage = DateTime.Now + MessageDelay;
					PublicOverheadMessage( MessageType.Regular, 37, true, m_Shouts[ Utility.Random( m_Shouts.Length ) ] );
				}

				base.OnDoubleClick( from );
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	        }
	        #endregion			
		}
		
		[Flipable( 0x3A5F, 0x3A65 )]
		public class OliveTheOtherReindeer : BaseReindeer
		{
			[Constructable]
			public OliveTheOtherReindeer() : this( 0 )
			{
			}

			[Constructable]
			public OliveTheOtherReindeer( int hue ) : base( 0x3A5F, hue, "Olive The Other Reindeer" )
			{
			}

			public OliveTheOtherReindeer( Serial serial ) : base( serial )
			{
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}

	 	[Flipable( 0x3A55, 0x3A56 )]
		public class SantasReindeer1 : BaseReindeer
		{
			[Constructable]
			public SantasReindeer1() : this( 0 )
			{
			}

			[Constructable]
			public SantasReindeer1( int hue ) : base( 0x3A55, hue, NameList.RandomName( "reindeer" ) )
			{
			}

			public SantasReindeer1( Serial serial ) : base( serial )
			{
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            if( String.IsNullOrEmpty( Name ) )
	            	Name = NameList.RandomName( "reindeer" );

	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}

	 	[Flipable( 0x3A67, 0x3A68 )]
		public class SantasReindeer2 : BaseReindeer
		{
			[Constructable]
			public SantasReindeer2() : this( 0 )
			{
			}

			[Constructable]
			public SantasReindeer2( int hue ) : base( 0x3A67, hue, NameList.RandomName( "reindeer" ) )
			{
			}

			public SantasReindeer2( Serial serial ) : base( serial )
			{
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            if( String.IsNullOrEmpty( Name ) )
	            	Name = NameList.RandomName( "reindeer" );

	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}

	 	[Flipable( 0x3A6F, 0x3A72 )]
		public class SantasReindeer3 : BaseReindeer
		{
			[Constructable]
			public SantasReindeer3() : this( 0 )
			{
			}

			[Constructable]
			public SantasReindeer3( int hue ) : base( 0x3A6F, hue, NameList.RandomName( "reindeer" ) )
			{
			}

			public SantasReindeer3( Serial serial ) : base( serial )
			{
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            if( String.IsNullOrEmpty( Name ) )
	            	Name = NameList.RandomName( "reindeer" );

	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}
		#endregion

		#region santasleight
		public class SantasSleigh : BaseAddon
		{
			public override BaseAddonDeed Deed{ get{ return new SantasSleighDeed(); } }

			[Constructable]
			public SantasSleigh( bool east )
			{
				AddonComponent ac;

				if( east )
				{
					ac = new AddonComponent( 14964 );
					ac.Name = "Santa's Sleigh";
					AddComponent( ac, 0, 0, 0);

					ac = new AddonComponent( 14963 );
					ac.Name = "Santa's Sleigh";	
					AddComponent( ac, 1, 0, 0);
				}
				else
				{
					ac = new AddonComponent( 14984 );
					ac.Name = "Santa's Sleigh";
					AddComponent( ac, 0, 0, 0);

					ac = new AddonComponent( 14983 );
					ac.Name = "Santa's Sleigh";	
					AddComponent( ac, 0, 1, 0);	
				}
			}

			public SantasSleigh( Serial serial ) : base( serial )
			{
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            
	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}

		public class SantasSleighDeed : BaseAddonDeed
		{
			private bool m_East;

			public override BaseAddon Addon{ get{ return new SantasSleigh( m_East ); } }

			[Constructable]
			public SantasSleighDeed()
			{
				Name = "A deed for Santa's Sleigh";
				LootType = LootType.Blessed;
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( Christmas2007.StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				if ( IsChildOf( from.Backpack ) )
				{
					from.CloseGump( typeof( InternalGump ) );
					from.SendGump( new InternalGump( this ) );
				}
				else
				{
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
			}

			private void SendTarget( Mobile m )
			{
				base.OnDoubleClick( m );
			}

			private class InternalGump : Gump
			{
				private SantasSleighDeed m_Deed;
	
				public InternalGump( SantasSleighDeed deed ) : base( 150, 50 )
				{
					m_Deed = deed;
	
					AddBackground( 0, 0, 350, 250, 0xA28 );
	
					AddItem( 90, 52, 14984 );
					AddItem( 73, 53, 14983 );
					AddButton( 70, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0 ); // South
	
					AddItem( 217, 51, 14964 );
					AddItem( 244, 52, 14963 );
					AddButton( 185, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0 ); // East
				}
	
				public override void OnResponse( NetState sender, RelayInfo info )
				{
					if ( m_Deed.Deleted || info.ButtonID == 0 )
						return;
	
					m_Deed.m_East = (info.ButtonID != 1);
					m_Deed.SendTarget( sender.Mobile );
				}
			}

			public SantasSleighDeed( Serial serial ) : base( serial )
			{
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            
	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}
		#endregion

		#region clothing
		public class ReggisenoNatalizio2007 : BaseShirt
		{
			public override bool AllowMaleWearer{ get{ return false; } }
			
			[Constructable]
			public ReggisenoNatalizio2007() : this( 0 )
			{			
			}
	
			[Constructable]
			public ReggisenoNatalizio2007( int hue ) : base( 5150, hue )
			{
				Name = "Holiday Brassiere";
				LootType = LootType.Blessed;
				Weight = 2.0;
			}
	
			public ReggisenoNatalizio2007( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( Christmas2007.StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
				writer.Write( (int) 0 ); // version
			}
	
			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
				int version = reader.ReadInt();
				
	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
		}

		public class CappellinoNatalizio2007 : BaseHat
		{
			public override int BasePhysicalResistance{ get{ return 1; } }
			public override int BaseFireResistance{ get{ return 2; } }
			public override int BaseColdResistance{ get{ return 3; } }
			public override int BasePoisonResistance{ get{ return 4; } }
			public override int BaseEnergyResistance{ get{ return 5; } }
			public override int InitMinHits{ get{ return 19; } }
			public override int InitMaxHits{ get{ return 38; } }

			[Constructable]
			public CappellinoNatalizio2007() : this( 0 )
			{		
			}

			[Constructable]
			public CappellinoNatalizio2007( int hue ) : base( 5149, hue )
			{
				Name = "Holiday Cap";
				LootType = LootType.Blessed;
				Weight = 2.0;
			}

			public CappellinoNatalizio2007( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( Christmas2007.StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override bool OnEquip( Mobile from )
			{
				return Validate( from ) && base.OnEquip( from );
			}

			public bool Validate( Mobile m )
			{
				if ( m == null || !m.Player || !m.Female )			
					return true;
				return true;
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

	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
		}

		public class GonnellinoNatalizio2007 : BaseOuterLegs
		{
			public override bool AllowMaleWearer{ get{ return false; } }

			[Constructable]
			public GonnellinoNatalizio2007() : this( 0 )
			{
			}

			[Constructable]
			public GonnellinoNatalizio2007( int hue ) : base( 5151, hue )
			{
				Name = "Holiday Skirt";
				LootType = LootType.Blessed;
				Weight = 2.0;
			}
	
			public GonnellinoNatalizio2007( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( Christmas2007.StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override bool OnEquip( Mobile from )
			{
				return Validate( from ) && base.OnEquip( from );
			}
				
			public bool Validate( Mobile m )
			{
				if ( m == null || !m.Player || !m.Female )			
					return true;
				return true;
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

	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
			}
		}
		#endregion

		#region snowflakes
		public class BlueSnowflake2007 : Item
		{
			[Constructable]
			public BlueSnowflake2007() : base( 0x232E )
			{
				Weight = 1.0;
				LootType = LootType.Blessed;
			}

			public BlueSnowflake2007( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( Christmas2007.StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				PublicOverheadMessage( MessageType.Regular, 37, true, Christmas2007.GetMidgardGreetings() );

				base.OnDoubleClick( from );
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();

	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}

		public class WhiteSnowflake2007 : Item
		{
			[Constructable]
			public WhiteSnowflake2007() : base( 0x232F )
			{
				Weight = 1.0;
				LootType = LootType.Blessed;
			}

			public WhiteSnowflake2007( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( Christmas2007.StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				PublicOverheadMessage( MessageType.Regular, 37, true, Christmas2007.GetMidgardGreetings() );

				base.OnDoubleClick( from );
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}
		#endregion

		#region poinsettias
		public class RedPoinsettia2007 : Item
		{
			[Constructable]
			public RedPoinsettia2007() : base( 0x2330 )
			{
				Weight = 1.0;
				LootType = LootType.Blessed;
			}

			public RedPoinsettia2007( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( Christmas2007.StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				PublicOverheadMessage( MessageType.Regular, 37, true, Christmas2007.GetMidgardGreetings() );

				base.OnDoubleClick( from );
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}
	
		public class WhitePoinsettia2007 : Item
		{
			[Constructable]
			public WhitePoinsettia2007() : base( 0x2331 )
			{
				Weight = 1.0;
				LootType = LootType.Blessed;
			}

			public WhitePoinsettia2007( Serial serial ) : base( serial )
			{
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( Christmas2007.StaticMidgardGreetings );
			}

			public override bool DisplayLootType { get { return false; } }
			public override bool DisplayWeight { get { return false; } }

			public override void OnDoubleClick( Mobile from )
			{
				PublicOverheadMessage( MessageType.Regular, 37, true, Christmas2007.GetMidgardGreetings() );

				base.OnDoubleClick( from );
			}

			#region serial-deserial
	        public override void Serialize(GenericWriter writer)
	        {
	            base.Serialize(writer);
	            writer.Write((int)0); // version 
	        }

	        public override void Deserialize(GenericReader reader)
	        {
	            base.Deserialize(reader);
	            int version = reader.ReadInt();
	            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Christmas2007.VerifyEndPeriod_Callback ), this );
	        }
	        #endregion
		}
		#endregion
	}
}
