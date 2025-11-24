using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Midgard.Engines.GroupsHandler;

namespace Server.Items
{
    public class MoongateMessage
    {
        private int Number { get; set; }
        private string ItaMessage { get; set; }
        private string EngMessage { get; set; }

        public MoongateMessage( int number, string itaMessage, string engMessage )
        {
            Number = number;
            ItaMessage = itaMessage;
            EngMessage = engMessage;
        }

        public void SendMessageTo( Mobile m, Item gate )
        {
            if( m.TrueLanguage == LanguageType.Ita && !string.IsNullOrEmpty( ItaMessage ) )
                gate.LabelTo( m, ItaMessage );
            else if( !string.IsNullOrEmpty( EngMessage ) )
                gate.LabelTo( m, EngMessage );
            else if( Number > 0 )
                gate.LabelTo( m, Number );
        }
    }

	public class PublicMoongate : Item
	{
		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled; } }

        #region mod by Dies Irae
	    [CommandProperty( AccessLevel.GameMaster )]
	    public bool ForceShowGump { get; set; }
	    #endregion

		[Constructable]
		public PublicMoongate() : base( 0xF6C )
		{
			Movable = false;
			Light = LightType.Circle300;

            ForceShowGump = false; // mod by Dies Irae
		}

		public PublicMoongate( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.Player )
				return;

			if ( from.InRange( GetWorldLocation(), 1 ) )
				UseGate( from );
			else
				from.SendLocalizedMessage( 500446 ); // That is too far away.
		}

        #region mod by DIes Irae
        public override void OnSingleClick( Mobile from )
        {
            if( !from.Player )
                return;

            if( Utility.InRange( from.Location, Location, 3 ) )
            {
                if( ForceShowGump || ( Map != Map.Felucca && Map != Map.Trammel ) )
                {
                    LabelTo( from, 1023948 ); // moongate
                    return;
                }

#if lunarGate
                PMEntry entry = GateEntry;

                if( entry.Description is MoongateMessage )
                    ( (MoongateMessage)entry.Description ).SendMessageTo( from, this );
                if( entry.Description is string )
                    LabelTo( from, (string)entry.Description );
                else if( entry.Description is int )
                    LabelTo( from, (int)entry.Description );

                if( from.AccessLevel > AccessLevel.Player || TestCenter.Enabled )
                    LabelTo( from, "[{0}]", StringList.GetClilocString( null, entry.Number, from.Language ) );
#endif
            }
            else
                from.SendLocalizedMessage( 500446 ); // That is too far away.
        }
        #endregion

        public override bool OnMoveOver( Mobile m )
        {
            #region mod by Dies Irae
            if( !Core.AOS )
            {
                UseGate( m );
                return false;
            }
            #endregion

            // Changed so criminals are not blocked by it.
			if ( m.Player )
				UseGate( m );

			return true;
		}

		public override bool HandlesOnMovement{ get{ return ForceShowGump; } }

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( m is PlayerMobile )
			{
				if ( !Utility.InRange( m.Location, this.Location, 1 ) && Utility.InRange( oldLocation, this.Location, 1 ) )
					m.CloseGump( typeof( MoongateGump ) );
			}
		}

        #region mod by Dies Irae
        private static PMList SecondAgeList = PMList.UORFelucca;

#if lunarGate
        [CommandProperty( AccessLevel.GameMaster )]
        public string DestName
        {
            get
            {
                PMEntry entry = GateEntry;
                return entry == null ? "***" : StringList.GetClilocString( null, entry.Number, from.Language );
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DestNumber
        {
            get
            {
                PMEntry entry = GateEntry;
                return entry == null ? -1 : entry.Number;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int GateNumber
        {
            get
            {
                int gateNum = 0;

                for( int i = 0; i < SecondAgeList.Entries.Length; ++i )
                {
                    PMEntry entry = SecondAgeList.Entries[ i ];
                    if( Location == entry.Location )
                    {
                        gateNum = i;
                        break;
                    }
                }

                return gateNum;
            }
        }

        public PMEntry GateEntry
        {
            get
            {
                return SecondAgeList.Entries[ ( GateNumber + Offset ) % SecondAgeList.Entries.Length ];
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
	    public int Offset
	    {
	        get
	        {
	            int hours;
                int minutes;
                Clock.GetTime( Map, X, Y, out hours, out minutes );

                int steps = 0;
                int cycle = ( 60 * hours + minutes ) % 120;
                if( cycle > 7 )
                    ++steps;
                if( cycle > 27 )
                    ++steps;
                if( cycle > 37 )
                    ++steps;
                if( cycle > 57 )
                    ++steps;
                if( cycle > 67 )
                    ++steps;
                if( cycle > 87 )
                    ++steps;
                if( cycle > 97 )
                    ++steps;
                if( cycle > 117 )
                    steps = 0;

	            return steps;
	        }
	    }
#else
        private PMEntry GetEntryByDirection( PMList list, Mobile m )
        {
            Direction d = m.GetDirectionTo( this );

            foreach( PMEntry entry in list.Entries )
            {
                if( entry.Direction == d )
                    return entry;
            }

            return list.Entries[ 0 ];
        }
#endif
        #endregion

		public bool UseGate( Mobile m )
		{
            /*
			if ( m.Criminal )
			{
				m.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
				return false;
			}
			else if ( SpellHelper.CheckCombat( m ) )
			{
				m.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
				return false;
			}
			else if ( m.Spell != null )
			{
				m.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment.
				return false;
            }
            */

            #region mod by Dies Irae
            /* else */ if( !Core.AOS && ( Map == Map.Felucca || Map == Map.Trammel ) && !ForceShowGump )
			{
                PMList list = SecondAgeList;
#if lunarGate
			    PMEntry entry = GateEntry;
#else
                PMEntry entry = GetEntryByDirection( list, m );
#endif
                m.Combatant = null;
                m.Warmode = false;

                m.MoveToWorld( entry.Location, list.Map );

                TeleportPets( m, entry.Location, list.Map, Location, Map );

                Effects.PlaySound( entry.Location, list.Map, 0x1FE );
                return true;
            }
            #endregion
            else
			{
				m.CloseGump( typeof( MoongateGump ) );
				m.SendGump( new MoongateGump( m, this ) );

				if ( !m.Hidden || m.AccessLevel == AccessLevel.Player )
					Effects.PlaySound( m.Location, m.Map, 0x20E );

				return true;
			}
		}

        public static void TeleportPets( Mobile master, Point3D loc, Map map, Point3D from, Map mapFrom )
        {
            List<Mobile> move = new List<Mobile>();

            foreach( Mobile m in mapFrom.GetMobilesInRange( from, 3 ) )
            {
                if( !( m is BaseCreature ) )
                    continue;

                BaseCreature pet = (BaseCreature)m;

                if( pet.Controlled && pet.ControlMaster == master )
                    move.Add( pet );
            }

            foreach( Mobile m in move )
                m.MoveToWorld( loc, map );
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

            writer.Write( ForceShowGump ); // mod by Dies Irae
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            ForceShowGump = reader.ReadBool(); // mod by Dies Irae
		}

		public static void Initialize()
		{
			CommandSystem.Register( "MoonGen", AccessLevel.Developer, new CommandEventHandler( MoonGen_OnCommand ) ); // mod by Dies Irae
		}

		[Usage( "MoonGen" )]
		[Description( "Generates public moongates. Removes all old moongates." )]
		public static void MoonGen_OnCommand( CommandEventArgs e )
		{
			DeleteAll();

			int count = 0;

            if( Core.AOS )
            {
			    count += MoonGen( PMList.Trammel );
			    count += MoonGen( PMList.Felucca );
			    count += MoonGen( PMList.Ilshenar );
			    count += MoonGen( PMList.Malas );
			    count += MoonGen( PMList.Tokuno );
            }
            else
                count += MoonGen( PMList.UORFelucca );

			World.Broadcast( 0x35, true, "{0} moongates generated.", count );
		}

		private static void DeleteAll()
		{
			List<Item> list = new List<Item>();

			foreach ( Item item in World.Items.Values )
			{
				if ( item is PublicMoongate )
					list.Add( item );
			}

			foreach ( Item item in list )
				item.Delete();

			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} moongates removed.", list.Count );
		}

		private static int MoonGen( PMList list )
		{
			foreach ( PMEntry entry in list.Entries )
			{
				Item item = new PublicMoongate();

				item.MoveToWorld( entry.Location, list.Map );
				GroupsHandler.MoonGenGroup.AddItem( item, true );

				if ( entry.Number == 1060642 ) // Umbra
					item.Hue = 0x497;
			}

			return list.Entries.Length;
		}
	}

	public class PMEntry
	{
	    public object Description { get; set; }  // mod by Dies Irae
	    public Direction Direction { get; set; }  // mod by Dies Irae
	    private Point3D m_Location;
		private int m_Number;

		public Point3D Location
		{
			get
			{
				return m_Location;
			}
		}

		public int Number
		{
			get
			{
				return m_Number;
			}
		}

        #region mod by Dies Irae
        public PMEntry( Point3D loc, int number )
            : this( loc, number, 1005397 ) //The moongate is cloudy, and nothing can be made out. 
        {
        }

        public PMEntry( Point3D loc, int number, object description )
            : this( loc, number, description, Direction.Up )
        {
        }

        public PMEntry( Point3D loc, int number, object description, Direction direction )
        {
            Description = description;
            Direction = direction;

            m_Location = loc;
            m_Number = number;
        }
        #endregion
    }

	public class PMList
	{
		private int m_Number, m_SelNumber;
		private Map m_Map;
		private PMEntry[] m_Entries;

		public int Number
		{
			get
			{
				return m_Number;
			}
		}

		public int SelNumber
		{
			get
			{
				return m_SelNumber;
			}
		}

		public Map Map
		{
			get
			{
				return m_Map;
			}
		}

		public PMEntry[] Entries
		{
			get
			{
				return m_Entries;
			}
		}

		public PMList( int number, int selNumber, Map map, PMEntry[] entries )
		{
			m_Number = number;
			m_SelNumber = selNumber;
			m_Map = map;
			m_Entries = entries;
		}

		public static readonly PMList Trammel =
			new PMList( 1012000, 1012012, Map.Trammel, new PMEntry[]
				{
					new PMEntry( new Point3D( 1336, 1997, 5 ), 1012004, 1005390 ), // Britain
					new PMEntry( new Point3D( 4467, 1283, 5 ), 1012003, 1005389 ), // Moonglow
					new PMEntry( new Point3D( 3563, 2139, 34), 1012010, 1005396 ), // Magincia
					new PMEntry( new Point3D(  643, 2067, 5 ), 1012009, 1005395 ), // Skara Brae
					new PMEntry( new Point3D( 1828, 2948,-20), 1012008, 1005394 ), // Trinsic
					new PMEntry( new Point3D( 2701,  692, 5 ), 1012007, 1005393 ), // Minoc
					new PMEntry( new Point3D(  771,  752, 5 ), 1012006, 1005392 ), // Yew
					new PMEntry( new Point3D( 1499, 3771, 5 ), 1012005, 1005391 ), // Jhelom
                    // comment out New Haven entry for OSI correct Old Style Moongates
					// new PMEntry( new Point3D( 3450, 2677, 25), 1078098 )  // New Haven
				} );

        #region mod by Dies Irae
        public static readonly PMList UORFelucca =
            new PMList( 1012001, 1012013, Map.Felucca, new PMEntry[]
                {
                    // "North", "Right", "East", "Down", "South", "Left", "West", "Up"
                    new PMEntry( new Point3D( 1336, 1997, 5 ), 1012004, new MoongateMessage( 1005390, "Attraverso il portale lunare scorgi una strada a est, che conduce ad una grande città. Tutt'attorno grandi montagne rocciose.", null ), Direction.North ), // Britain
                    new PMEntry( new Point3D( 4467, 1283, 5 ), 1012003, new MoongateMessage( 1005389, "Attraverso il portale lunare noti un'isola con un piccolo insediamento a Sud e una grande città a nord.", null ), Direction.Right ), // Moonglow
                    new PMEntry( new Point3D( 3563, 2139, 34), 1012010, new MoongateMessage( 1005396, "Attraverso il portale lunare vedi una piccola isola immersa nel verde, circondata da un'aurea magica.", null ), Direction.East ), // Magincia
                    new PMEntry( new Point3D(  643, 2067, 5 ), 1012009, new MoongateMessage( 1005395, "Attraverso il portale lunare scorgi una piccola città collegata alla terra ferma da un ponte di navi.", null ), Direction.Down ), // Skara Brae
                    new PMEntry( new Point3D( 1828, 2948,-20), 1012008, new MoongateMessage( 1005394, "Attraverso il portale lunare vedi una bellissima città di pietra gialla con alte mura circondate da un fiume.", null ), Direction.South ), // Trinsic
                    new PMEntry( new Point3D( 2701,  692, 5 ), 1012007, new MoongateMessage( 1005393, "Attraverso il portale lunare vedi una laboriosa cittadina d'innanzi ad un'enorme montagna e un fiume a nord.", null ), Direction.Left ), // Minoc
                    new PMEntry( new Point3D(  771,  752, 5 ), 1012006, new MoongateMessage( 1005392, "Attraverso il portale lunare non vedi nient'altro che gli alberi di un fitto bosco.", null ), Direction.West ), // Yew
                    // new PMEntry( new Point3D( 1499, 3771, 5 ), 1012005, 1005391 ), // Jhelom
                    new PMEntry( new Point3D( 3014, 3527, 15 ), 1011348, new MoongateMessage( 0, "Attraverso il portale lunare scorgi una cittadella dalle mirabili mura e dalle altissime torri.", 
                        "Through the moongate you see a citadel with majestic walls and towers." ), Direction.Up ),  // Serpent's Hold
                    // new PMEntry( new Point3D( 2711, 2234, 0 ), 1019001 )  // Buccaneer's Den // remove to be osi correct
                } );
        #endregion

        public static readonly PMList Felucca =
			new PMList( 1012001, 1012013, Map.Felucca, new PMEntry[]
				{
					new PMEntry( new Point3D( 4467, 1283, 5 ), 1012003 ), // Moonglow
					new PMEntry( new Point3D( 1336, 1997, 5 ), 1012004 ), // Britain
                    #region Modifica by Dies Irae per il moongate a Serpent's Hold e Cove
					new PMEntry( new Point3D( 2299, 1189, 0 ), 1011033 ), // Cove
//					new PMEntry( new Point3D( 1499, 3771, 5 ), 1012005 ), // Jhelom
                    new PMEntry( new Point3D( 3014, 3527, 15 ), 1011348 ), // Serpent's Hold
					#endregion
					new PMEntry( new Point3D(  771,  752, 5 ), 1012006 ), // Yew
					new PMEntry( new Point3D( 2701,  692, 5 ), 1012007 ), // Minoc
					new PMEntry( new Point3D( 1828, 2948,-20), 1012008 ), // Trinsic
					new PMEntry( new Point3D(  643, 2067, 5 ), 1012009 ), // Skara Brae
					new PMEntry( new Point3D( 3563, 2139, 34), 1012010 ), // Magincia
					new PMEntry( new Point3D( 2711, 2234, 0 ), 1019001 ), // Buccaneer's Den
				} );

		public static readonly PMList Ilshenar =
			new PMList( 1012002, 1012014, Map.Ilshenar, new PMEntry[]
				{
					new PMEntry( new Point3D( 1215,  467, -13 ), 1012015 ), // Compassion
					new PMEntry( new Point3D(  722, 1366, -60 ), 1012016 ), // Honesty
					new PMEntry( new Point3D(  744,  724, -28 ), 1012017 ), // Honor
					new PMEntry( new Point3D(  281, 1016,   0 ), 1012018 ), // Humility
					new PMEntry( new Point3D(  987, 1011, -32 ), 1012019 ), // Justice
					new PMEntry( new Point3D( 1174, 1286, -30 ), 1012020 ), // Sacrifice
					new PMEntry( new Point3D( 1532, 1340, - 3 ), 1012021 ), // Spirituality
					new PMEntry( new Point3D(  528,  216, -45 ), 1012022 ), // Valor
					new PMEntry( new Point3D( 1721,  218,  96 ), 1019000 )  // Chaos
				} );

		public static readonly PMList Malas =
			new PMList( 1060643, 1062039, Map.Malas, new PMEntry[]
				{
					new PMEntry( new Point3D( 1015,  527, -65 ), 1060641 ), // Luna
					new PMEntry( new Point3D( 1997, 1386, -85 ), 1060642 )  // Umbra
				} );

		public static readonly PMList Tokuno =
			new PMList( 1063258, 1063415, Map.Tokuno, new PMEntry[]
				{
					new PMEntry( new Point3D( 1169,  998, 41 ), 1063412 ), // Isamu-Jima
					new PMEntry( new Point3D(  802, 1204, 25 ), 1063413 ), // Makoto-Jima
					new PMEntry( new Point3D(  270,  628, 15 ), 1063414 )  // Homare-Jima
				} );

        #region modifica by Dies Irae per il post esodo su Felucca
//		public static readonly PMList[] UORLists		= new PMList[] { Trammel, Felucca };
//		public static readonly PMList[] UORlistsYoung	= new PMList[] { Trammel };
//		public static readonly PMList[] LBRLists		= new PMList[] { Trammel, Felucca, Ilshenar };
//		public static readonly PMList[] LBRListsYoung	= new PMList[] { Trammel, Ilshenar };
//		public static readonly PMList[] AOSLists		= new PMList[] { Trammel, Felucca, Ilshenar, Malas };
//		public static readonly PMList[] AOSListsYoung	= new PMList[] { Trammel, Ilshenar, Malas };
//		public static readonly PMList[] SELists			= new PMList[] { Trammel, Felucca, Ilshenar, Malas, Tokuno };
//		public static readonly PMList[] SEListsYoung	= new PMList[] { Trammel, Ilshenar, Malas, Tokuno };
		public static readonly PMList[] RedLists		= new PMList[] { Felucca };
		public static readonly PMList[] SigilLists		= new PMList[] { Felucca };

		public static readonly PMList[] UORLists		= new PMList[] { UORFelucca };
		public static readonly PMList[] UORlistsYoung	= new PMList[] { UORFelucca };
		public static readonly PMList[] LBRLists		= new PMList[] { Felucca };
		public static readonly PMList[] LBRListsYoung	= new PMList[] { Felucca };
		public static readonly PMList[] AOSLists		= new PMList[] { Felucca };
		public static readonly PMList[] AOSListsYoung	= new PMList[] { Felucca };
		public static readonly PMList[] SELists			= new PMList[] { Felucca };
		public static readonly PMList[] SEListsYoung	= new PMList[] { Felucca };
		#endregion
	}

	public class MoongateGump : Gump
	{
		private Mobile m_Mobile;
		private Item m_Moongate;
		private PMList[] m_Lists;

		public MoongateGump( Mobile mobile, Item moongate ) : base( 100, 100 )
		{
			m_Mobile = mobile;
			m_Moongate = moongate;

			PMList[] checkLists;

			if ( mobile.Player )
			{
				if ( Factions.Sigil.ExistsOn( mobile ) )
				{
					checkLists = PMList.SigilLists;
				}
				else if ( mobile.Kills >= 5 )
				{
					checkLists = PMList.RedLists;
				}
				else
				{
					ClientFlags flags = mobile.NetState == null ? ClientFlags.None : mobile.NetState.Flags;
					bool young = mobile is PlayerMobile ? ((PlayerMobile)mobile).Young : false;

				    #region mod by Dies Irae
					/*
					if ( Core.SE && (flags & ClientFlags.Tokuno) != 0 )
						checkLists = young ? PMList.SEListsYoung : PMList.SELists;
					else if ( Core.AOS && (flags & ClientFlags.Malas) != 0 )
						checkLists = young ? PMList.AOSListsYoung : PMList.AOSLists;
					else if ( (flags & ClientFlags.Ilshenar) != 0 )
						checkLists = young ? PMList.LBRListsYoung : PMList.LBRLists;
					else
						checkLists = young ? PMList.UORListsYoung : PMList.UORLists;
				    */
					checkLists = PMList.UORLists;
                    #endregion
                }
			}
			else
			{
				checkLists = PMList.SELists;
			}

			m_Lists = new PMList[checkLists.Length];

			for ( int i = 0; i < m_Lists.Length; ++i )
				m_Lists[i] = checkLists[i];

			for ( int i = 0; i < m_Lists.Length; ++i )
			{
				if ( m_Lists[i].Map == mobile.Map )
				{
					PMList temp = m_Lists[i];

					m_Lists[i] = m_Lists[0];
					m_Lists[0] = temp;

					break;
				}
			}

			AddPage( 0 );

			AddBackground( 0, 0, 380, 280, 5054 );

			AddButton( 10, 210, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 210, 140, 25, 1011036, false, false ); // OKAY

			AddButton( 10, 235, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 235, 140, 25, 1011012, false, false ); // CANCEL

			#region Modifica by Dies Irae per il restyling del gump
			AddHtmlLocalized( 20, 10, 200, 20, 1012011, false, false ); // Pick your destination:
//			AddHtmlLocalized( 5, 5, 200, 20, 1012011, false, false ); // Pick your destination:

//			for ( int i = 0; i < checkLists.Length; ++i )
//			{
//				AddButton( 10, 35 + (i * 25), 2117, 2118, 0, GumpButtonType.Page, Array.IndexOf( m_Lists, checkLists[i] ) + 1 );
//				AddHtmlLocalized( 30, 35 + (i * 25), 150, 20, checkLists[i].Number, false, false );
//			}
			#endregion
			
			for ( int i = 0; i < m_Lists.Length; ++i )
				RenderPage( i, Array.IndexOf( checkLists, m_Lists[i] ) );
		}

		private void RenderPage( int index, int offset )
		{
			PMList list = m_Lists[index];

			AddPage( index + 1 );

			#region Modifica by Dies Irae per il restyling del gump
//			AddButton( 10, 35 + (offset * 25), 2117, 2118, 0, GumpButtonType.Page, index + 1 );
//			AddHtmlLocalized( 30, 35 + (offset * 25), 150, 20, list.SelNumber, false, false );
			#endregion
			
			PMEntry[] entries = list.Entries;

			for ( int i = 0; i < entries.Length; ++i )
			{
				#region Modifica by Dies Irae per il Moongate a Serpent's Hold
//				AddRadio( 200, 35 + (i * 25), 210, 211, false, (index * 100) + i );
//				AddHtmlLocalized( 225, 35 + (i * 25), 150, 20, entries[i].Number, false, false );
				AddRadio( 200, 20 + (i * 25), 210, 211, false, (index * 100) + i );
				AddHtmlLocalized( 225, 20 + (i * 25), 150, 20, entries[i].Number, false, false );
				#endregion
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( info.ButtonID == 0 ) // Cancel
				return;
			else if ( m_Mobile.Deleted || m_Moongate.Deleted || m_Mobile.Map == null )
				return;

			int[] switches = info.Switches;

			if ( switches.Length == 0 )
				return;

			int switchID = switches[0];
			int listIndex = switchID / 100;
			int listEntry = switchID % 100;

			if ( listIndex < 0 || listIndex >= m_Lists.Length )
				return;

			PMList list = m_Lists[listIndex];

			if ( listEntry < 0 || listEntry >= list.Entries.Length )
				return;

			PMEntry entry = list.Entries[listEntry];

			if ( !m_Mobile.InRange( m_Moongate.GetWorldLocation(), 1 ) || m_Mobile.Map != m_Moongate.Map )
			{
				m_Mobile.SendLocalizedMessage( 1019002 ); // You are too far away to use the gate.
			}
			#region modifica By Bertoldo
//			else if ( m_Mobile.Player && m_Mobile.Kills >= 5 && list.Map != Map.Felucca )
//			{
//				m_Mobile.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.
//			}
			#endregion
			else if ( Factions.Sigil.ExistsOn( m_Mobile ) && list.Map != Factions.Faction.Facet )
			{
				m_Mobile.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.
			}
			else if ( m_Mobile.Criminal )
			{
				m_Mobile.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
			}
			else if ( SpellHelper.CheckCombat( m_Mobile ) )
			{
				m_Mobile.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
			}
			else if ( m_Mobile.Spell != null )
			{
				m_Mobile.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment.
			}
			else if ( m_Mobile.Map == list.Map && m_Mobile.InRange( entry.Location, 1 ) )
			{
				m_Mobile.SendLocalizedMessage( 1019003 ); // You are already there.
			}
			else
			{
				BaseCreature.TeleportPets( m_Mobile, entry.Location, list.Map );

				m_Mobile.Combatant = null;
				m_Mobile.Warmode = false;
				m_Mobile.Hidden = true;

				m_Mobile.MoveToWorld( entry.Location, list.Map );

				Effects.PlaySound( entry.Location, list.Map, 0x1FE );
			}
		}
	}
}